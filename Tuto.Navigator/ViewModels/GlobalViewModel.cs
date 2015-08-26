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

namespace Tuto.Navigator
{
    public class GlobalViewModel: NotifierModel
    {
        public GlobalViewModel()
        {

            //NewCommand = new RelayCommand(New);
            //OpenCommand = new RelayCommand(Open);
            //CloseCommand = new RelayCommand(Close, () => IsLoaded);

            PreWorks = new AssemblySettings();
            SaveCommand = new RelayCommand(Save, () => IsLoaded);
            RefreshCommand = new RelayCommand(ReadSubdirectories, () => IsLoaded);


            Func<bool> somethingSelected=() => 
                    IsLoaded && Subdirectories.Any(z => z.Selected);


            AssembleSelectedCommand = new RelayCommand(AssembleSelected, somethingSelected);
            AssembleSelectedWithOptionsCommand = new RelayCommand(AssembleWithOptions, somethingSelected);
            RemontageSelectedCommand = new RelayCommand(MontageSelected, somethingSelected);
			RepairFaceSelectedCommand = new RelayCommand(RepairFaceSelected, somethingSelected);				 
            CreateBackupCommand = new RelayCommand(CreateBackup);
            UploadSelectedClipsCommand = new RelayCommand(UploadClips);
        }

        public void AssembleWithOptions()
        {
            var work = Subdirectories.Where(z => z.Selected);
            var models = work.Select(x => EditorModelIO.Load(x.FullPath));
            var tasks = new List<BatchWork>();
            foreach (var m in models)
            {
                tasks.AddRange(PreWorks.GetWorksAccordingSettings(m));
            }
            if (tasks.Count != 0)
                queueWindow.Run(tasks);
        }

        public void UploadClips()
        {
            var work = Subdirectories.
                Select(x => x.EpisodesNames).
                SelectMany(x => x).
                Where(x => x.Checked).
                Select(x => new YoutubeWork(x)).
                ToList();
            queueWindow.Run(work);
        }

        public void Load(FileInfo file)
        {
            if (IsLoaded)
            {
                // save and close current
                //SaveCommand.Execute(null);
                //CloseCommand.Execute(null);
            }

            LoadedFile = file;
            ReadSubdirectories();
        }

        List<EditorModel> models { get; set; }
        public BatchWorkWindow queueWindow = new BatchWorkWindow();

        public void ReadSubdirectories()
        {
            var data=EditorModelIO.ReadAllProjectData(LoadedFile.Directory);

            //for older version
            if (data.Global.WorkSettings == null)
            {
                data.Global.WorkSettings = new WorkSettings();
            }

            this.globalData=data.Global;
            models = new List<EditorModel>();
            foreach (var m in data.Models)
            {
                models.Add(m);
            }
            Subdirectories = new ObservableCollection<SubfolderViewModel>();
            foreach (var e in data.Models)
            {
                var m = new SubfolderViewModel(e);
                m.addTaskToQueue = queueWindow.Run;
                Subdirectories.Add(m);
            }
            Publish = new PublishViewModel(globalData);
            FillQueue(data);
        }

        
        void FillQueue(AllProjectData data)
        {
            var tasks = new List<BatchWork>();
            for (var i = 0; i < data.Models.Count; i++)
            {
                var works = new List<BatchWork>();
                var e = data.Models[i];
                var m = Subdirectories[i];
                if (!e.Locations.PraatVoice.Exists)
                {
                    works.Add(new PraatWork(e));
                }
                works.AddRange(data.Global.WorkSettings.GetBeforeEditingWorks(e));
                if (works.Count() != 0)
                {
                    var t = works.Last();
                    t.TaskFinished += (s, a) => { m.ReadyToEdit = true; e.Save(); };
                    tasks.AddRange(works);
                }
                else m.ReadyToEdit = true;
            }
            if (tasks.Count != 0)
                queueWindow.Run(tasks);
        }

        void Montage()
        {

        }

        void CreateBackup()
        {
            File.WriteAllText(
                Path.Combine(globalData.GlobalDataFolder.FullName, "backup.bat"),
                Backup.CreateBackup(globalData, models));
        }

        public void Save()
        {
            Publish.Commit();
            EditorModelIO.Save(GlobalData);
			if (GlobalData.RelativeVideoListPath!=null)
			{
				var file = new FileInfo(Path.Combine(
					GlobalData.GlobalDataFolder.FullName,
					GlobalData.RelativeVideoListPath,
					Tuto.Model.GlobalData.VideoListName));
				List<VideoPublishSummary> currentList = new List<VideoPublishSummary>();
				if (file.Exists)
					currentList = HeadedJsonFormat.Read<List<VideoPublishSummary>>(file);

				foreach (var e in models)
					for (int i = 0; i < e.Montage.Information.Episodes.Count; i++)
					{
						var alreadySaved = currentList.Where(z => z.Guid == e.Montage.Information.Episodes[i].Guid).FirstOrDefault();
						if (alreadySaved != null) currentList.Remove(alreadySaved);
						var fv = new FinishedVideo(e, i);
						var pv = new VideoPublishSummary { Guid = fv.Guid, Name = fv.Name, Duration = fv.Duration };
						pv.OrdinalSuffix = fv.RelativeSourceFolderLocation + "-" + fv.EpisodeNumber;
						currentList.Add(pv);
					}

				HeadedJsonFormat.Write(file, currentList);
			}
        }

        void Run(bool forceMontage)
        {
            var work = Subdirectories.Where(z => z.Selected);
            var models = work.Select(x => EditorModelIO.Load(x.FullPath));
            var tasks = models.Select(x =>
                new AssemblyVideoWork(x, x.Global.CrossFadesEnabled)).ToList();
            queueWindow.Run(tasks);

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
            var models = work.Select(x => EditorModelIO.Load(x.FullPath));
            var tasks = models.Select(x =>
                new RepairVideoWork(x, x.Locations.FaceVideo)).ToList();
            queueWindow.Run(tasks);
		}

        #region commands

        public RelayCommand SaveCommand { get; private set; }
        public RelayCommand RefreshCommand { get; private set; }
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

        public GlobalData GlobalData {
            get { return globalData; }
            private set
            {
                globalData = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged("IsLoaded");
            }
        }

        public ObservableCollection<SubfolderViewModel> Subdirectories {
            get { return subdirectories; }
            private set
            {
                subdirectories = value;
                NotifyPropertyChanged();
            }
        }

        public AssemblySettings PreWorks{get; set;}

        public PublishViewModel Publish
        {
            get { return publish; }
            private set { publish = value; NotifyPropertyChanged(); }
        }

        public bool IsLoaded
        {
            get { return LoadedFile != null && GlobalData != null; }
        }

        public string WindowTitle
        {
            get { return LoadedFile != null ? LoadedFile.DirectoryName : "Tuto.Navigator"; }
        }

  
        #endregion

        private FileInfo loadedFile;
        private GlobalData globalData;
        private ObservableCollection<SubfolderViewModel> subdirectories;
        private PublishViewModel publish;
        //private FileSystemWatcher watcher;


        #region Unused file commands
        //public void New()
        //{
        //    var dialog = new SaveFileDialog
        //    {
        //        Filter = "Tuto project|project.tuto",
        //        FilterIndex = 0,
        //        OverwritePrompt = true,
        //        AddExtension = false,
        //        FileName = "Filename will be ignored",
        //        CheckFileExists = false
        //    };
        //    var result = dialog.ShowDialog();
        //    if (!(result.HasValue && result.Value))
        //        return;
        //    var path = Path.GetDirectoryName(dialog.FileName);
        //    var file = new FileInfo(Path.Combine(path, "project.tuto"));
        //    if (file.Exists)
        //    {
        //        var overwriteResult = MessageBox.Show("Project file exists, overwrite?", "Warning", MessageBoxButton.OKCancel);
        //        if (overwriteResult != MessageBoxResult.OK)
        //            return;
        //    }
        //    file.Delete();
        //    file.Create().Close();

        //    GlobalFileIO.Save(new GlobalData(), file);

        //    Load(file);
        //}

        //public void Open()
        //{
        //    var dialog = new OpenFileDialog
        //    {
        //        Filter = "Tuto project|project.tuto",
        //        FilterIndex = 0,
        //    };
        //    var result = dialog.ShowDialog();
        //    if (!(result.HasValue && result.Value))
        //        return;
        //    var file = new FileInfo(dialog.FileName);
        //    Load(file);
        //}





        //public void Close()
        //{
        //    LoadedFile = null;
        //    GlobalData = null;
        //    Subdirectories.Clear();
        //    //watcher.EnableRaisingEvents = false;
        //}



        //public RelayCommand NewCommand { get; private set; }
        //public RelayCommand OpenCommand { get; private set; }
        //public RelayCommand CloseCommand { get; private set; }
     

        #endregion

    }
}
