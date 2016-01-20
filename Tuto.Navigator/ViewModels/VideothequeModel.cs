using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Media3D;
using Microsoft.Win32;
using Tuto.Model;
using Tuto.Navigator.ViewModels;
using Tuto.Publishing;
using Tuto.BatchWorks;

namespace Tuto.Navigator.ViewModels
{
    public class VideothequeModel: NotifierModel
    {
        public SearchViewModel Search { get; private set; }
        List<VideoViewModel> allModels;
        List<EditorModel> models { get; set; }

        public event Action<VideoViewModel> OpenEditor;

        public StatisticsViewModel Statistics { get; private set; }

        public ObservableCollection<VideoViewModel> Subdirectories
        {
            get { return subdirectories; }
            private set
            {
                subdirectories = value;
                NotifyPropertyChanged();
            }
        }
       
        public VideothequeModel(Videotheque videotheque)
        {

            this.videotheque = videotheque;

            

            Search = new SearchViewModel();
            Search.PropertyChanged += (s, a) => Filter();
            Search.RefreshRequested+=() => { videotheque.Reload(); UpdateSubdirectories(); };
            Search.SelectAllRequested += SelectAll;
            UpdateSubdirectories();

            PreWorks = new AssemblySettings();
            SaveCommand = new RelayCommand(Save, () => true);
            MakeAllCommand = new RelayCommand(() =>
                {
                    var work = Subdirectories.Where(z => z.Selected);
                    var models = work.Select(x => x.Model);
                    foreach (var e in models)
                        Program.WorkQueue.Run(new MakeAll(e));
                }
                );

            Func<bool> somethingSelected=() => Subdirectories.Any(z => z.Selected);


            AssembleSelectedCommand = new RelayCommand(AssembleSelected, somethingSelected);
            AssembleSelectedWithOptionsCommand = new RelayCommand(AssembleWithOptions, somethingSelected);
            RemontageSelectedCommand = new RelayCommand(MontageSelected, somethingSelected);
			RepairFaceSelectedCommand = new RelayCommand(RepairFaceSelected, somethingSelected);				 
            
            UploadSelectedClipsCommand = new RelayCommand(UploadClips);
        }



        void Filter()
        {

            IEnumerable<VideoViewModel> en = allModels;
            if (!string.IsNullOrWhiteSpace(Search.TextSearch))
            {
                var keyword = Search.TextSearch.ToLower();
                en = en
                .Select(z => new { VM = z, Rate = z.GetTextInfo().Where(x=>x!=null).Select(x => (double)NameMatchAlgorithm.MatchNames(x.ToLower(), keyword) / keyword.Length).Max() })
                .Where(z => z.Rate > 0.8)
                .OrderByDescending(z => z.Rate)
                .Select(z => z.VM);           
            }
            if (Search.OnlyWithSource)
            {
                en = en.Where(z => z.Model.Statuses.SourceIsPresent);
            }

            switch(Search.SortType.Value)
            {
                case SortType.CreationTime: en = en.OrderByDescending(z => z.Model.Montage.Information.CreationTime); break;
                case SortType.ModificationTime: en = en.OrderByDescending(z => z.Model.Montage.Information.LastModificationTime); break;
                default: en = en.OrderBy(z => z.Model.Montage.DisplayedRawLocation); break;
            }

            Subdirectories = new ObservableCollection<VideoViewModel>(en);
        }

        void UpdateSubdirectories()
        {
            allModels = new List<VideoViewModel>();
            foreach (var e in videotheque.EditorModels)
            {
                var m = new VideoViewModel(e);
                m.OpenMe += OpenVideoViewModel;
                m.SubsrcibeByExpression(z => z.Selected, UpdateStatistics);
				allModels.Add(m);
            }
            Filter();
        }

        void OpenVideoViewModel(VideoViewModel obj)
        {
            if (OpenEditor != null)
                OpenEditor(obj);
        }

        void UpdateStatistics()
        {
            StatisticsViewModel stat = new StatisticsViewModel();
            foreach(var e in allModels.Where(z=>z.Selected))
            {
                stat.EpisodesCount += e.Model.Montage.Information.Episodes.Count;
                stat.TotalClean += (int)e.Model.Montage.Information.Episodes.Sum(z => z.Duration.TotalMinutes);
                stat.TotalDirty += e.Model.Montage.Chunks.Where(z => z.Mode != Editor.Mode.Undefined).Sum(z=>z.Length) / 60000;
            }
            Statistics = stat;
            this.NotifyByExpression(z => z.Statistics);
        }

        public void AssembleWithOptions()
        {
            var work = Subdirectories.Where(z => z.Selected);
            var models = work.Select(x => x.Model);
            var tasks = new List<BatchWork>();
            foreach (var m in models)
            {
                tasks.AddRange(PreWorks.GetWorksAccordingSettings(m));
            }
            if (tasks.Count != 0)
                Program.WorkQueue.Run(tasks);
        }

        void SelectAll()
        {
            bool select = Subdirectories.Any(z => !z.Selected);
            if (select)
                foreach (var e in Subdirectories)
                    e.Selected = true;
            else
                foreach (var e in allModels)
                    e.Selected = false;
        }

        public void UploadClips()
        {

        }




        
        public void FillQueue()
        {
            var tasks = new List<BatchWork>();
            foreach(var m in Subdirectories)
            {
                var works = new List<BatchWork>();
                var e = m.Model;
                if (videotheque.Data.WorkSettings.StartPraat && e.Statuses.SourceIsPresent && (!e.Locations.PraatVoice.Exists || m.Model.Montage.SoundIntervals.Count==0))
                {
                    works.Add(new PraatWork(e));
                }
                works.AddRange(videotheque.Data.WorkSettings.GetBeforeEditingWorks(e));
                if (works.Count() != 0)
                {
                    var t = works.Last();
                    t.TaskFinished += (s, a) => {e.Save(); };
                    tasks.AddRange(works);
                }
            }
            if (tasks.Count != 0)
                Program.WorkQueue.Run(tasks);
        }

        void Montage()
        {

        }



        public void Save()
        {
            videotheque.Save();
        }

        void Run(bool forceMontage)
        {
            var work = Subdirectories.Where(z => z.Selected);
            var models = work.Select(x => x.Model);
            var tasks = models.Select(x =>
                new AssemblyVideoWork(x)).ToList();
            Program.WorkQueue.Run(tasks);

        }

        public void AssembleSelected()
        {
            Run(false);
        }

        public void MontageSelected()
        {
            Run(true);
        }

		public void RepairFaceSelected()
		{
            var work = Subdirectories.Where(z => z.Selected);
            var models = work.Select(x => x.Model);
            var tasks = models.Select(x =>
                new RepairVideoWork(x, x.Locations.FaceVideo, true)).ToList();
            Program.WorkQueue.Run(tasks);
		}


        #region commands

        public RelayCommand SaveCommand { get; private set; }
         public RelayCommand MakeAllCommand { get; private set; }
        public RelayCommand AssembleSelectedCommand { get; private set; }
        public RelayCommand AssembleSelectedWithOptionsCommand { get; private set; }
        public RelayCommand RemontageSelectedCommand { get; private set; }
		public RelayCommand RepairFaceSelectedCommand { get; private set; }
        public RelayCommand UploadSelectedClipsCommand { get; private set; }
        public RelayCommand CreateBackupCommand { get; private set; }
        #endregion

        #region properties

        public FileInfo LoadedFile
        {
            get { return loadedFile; }
            private set
            {
                loadedFile = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged("WindowTitle");
                NotifyPropertyChanged("IsLoaded");
            }
        }


       

        public AssemblySettings PreWorks{get; set;}

       

        public string WindowTitle
        {
            get { return LoadedFile != null ? LoadedFile.DirectoryName : "Tuto.Navigator"; }
        }

  
        #endregion

        private FileInfo loadedFile;
        private Videotheque videotheque;
        private ObservableCollection<VideoViewModel> subdirectories;
        public Videotheque Videotheque { get { return videotheque; } }
        //private FileSystemWatcher watcher;


    }
}
