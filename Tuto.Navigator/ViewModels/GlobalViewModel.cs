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

namespace Tuto.Navigator
{
    public class GlobalViewModel: NotifierModel
    {
        public GlobalViewModel()
        {
            NewCommand = new RelayCommand(New);
            OpenCommand = new RelayCommand(Open);
            SaveCommand = new RelayCommand(Save, () => IsLoaded);
            CloseCommand = new RelayCommand(Close, () => IsLoaded);
            RefreshCommand = new RelayCommand(ReadSubdirectories, () => IsLoaded);


            Func<bool> somethingSelected=() => 
                    IsLoaded && Subdirectories.Any(z => z.Selected);


            AssembleSelectedCommand = new RelayCommand(AssembleSelected, somethingSelected);
            RemontageSelectedCommand = new RelayCommand(MontageSelected, somethingSelected);
        }

        public void Load(FileInfo file)
        {
            if (IsLoaded)
            {
                // save and close current
                SaveCommand.Execute(null);
                CloseCommand.Execute(null);
            }

            LoadedFile = file;
            GlobalData = GlobalFileIO.Load(LoadedFile);
            ReadSubdirectories();
        }

        public void ReadSubdirectories()
        {
            var rootDir = new DirectoryInfo(LoadedFile.DirectoryName);
            Subdirectories = new ObservableCollection<SubfolderViewModel>();

            var dirs = rootDir
                        .GetDirectories()
                        .Where(dir => dir.Name != "Output") //очень грубый костыль
                        .OrderByDescending(z => z.CreationTime);
            foreach (var e in dirs)
            {
                var model = EditorModelIO.Load(e.FullName);
                Subdirectories.Add(new SubfolderViewModel(model));
                foreach (var v in model.Montage.Information.Episodes)
                {
                    var ex = GlobalData.VideoData.Where(z => z.Guid == v.Guid).FirstOrDefault();
                    if (ex == null)
                        GlobalData.VideoData.Add(new PublishVideoData(v));
                    else
                    {
                        ex.Name = v.Name;
                        ex.Duration = v.Duration;
                    }
                }
            }

            Publish = new PublishViewModel(GlobalData);
        }
     
        public void Save()
        {
            Publish.Commit();
            GlobalFileIO.Save(GlobalData, LoadedFile);
        }
        void Run(bool forceMontage)
        {
            var work = Subdirectories
                .Where(z => z.Selected)
                .SelectMany(z => TutoProgram.MakeAll(z.FullPath, forceMontage))
                .ToArray();
            var window = new BatchWorkWindow();
            window.Run(work);

        }

        public void AssembleSelected()
        {
            Run(false);
        }

        public void MontageSelected()
        {
            Run(true);
        }

        #region commands

        public RelayCommand SaveCommand { get; private set; }
        public RelayCommand RefreshCommand { get; private set; }
        public RelayCommand AssembleSelectedCommand { get; private set; }
        public RelayCommand RemontageSelectedCommand { get; private set; }
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
        public void New()
        {
            var dialog = new SaveFileDialog
            {
                Filter = "Tuto project|project.tuto",
                FilterIndex = 0,
                OverwritePrompt = true,
                AddExtension = false,
                FileName = "Filename will be ignored",
                CheckFileExists = false
            };
            var result = dialog.ShowDialog();
            if (!(result.HasValue && result.Value))
                return;
            var path = Path.GetDirectoryName(dialog.FileName);
            var file = new FileInfo(Path.Combine(path, "project.tuto"));
            if (file.Exists)
            {
                var overwriteResult = MessageBox.Show("Project file exists, overwrite?", "Warning", MessageBoxButton.OKCancel);
                if (overwriteResult != MessageBoxResult.OK)
                    return;
            }
            file.Delete();
            file.Create().Close();

            GlobalFileIO.Save(new GlobalData(), file);

            Load(file);
        }

        public void Open()
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Tuto project|project.tuto",
                FilterIndex = 0,
            };
            var result = dialog.ShowDialog();
            if (!(result.HasValue && result.Value))
                return;
            var file = new FileInfo(dialog.FileName);
            Load(file);
        }





        public void Close()
        {
            LoadedFile = null;
            GlobalData = null;
            Subdirectories.Clear();
            //watcher.EnableRaisingEvents = false;
        }



        public RelayCommand NewCommand { get; private set; }
        public RelayCommand OpenCommand { get; private set; }
        public RelayCommand CloseCommand { get; private set; }
     

        #endregion

    }
}
