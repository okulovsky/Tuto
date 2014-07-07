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
            RunSelectedCommand = new RelayCommand(RunSelected, () => IsLoaded && Subdirectories.Any(z => z.Selected));

            watcher = new FileSystemWatcher();
            watcher.IncludeSubdirectories = true;
            /* watcher.Filter = "local.tuto"; 
             * 
             * fails to detect deletion of directory with files
             * because files reported in DOS format in that case
             * and multiple filemask isn't possible
             * 
             * catching all events for now
             */
            watcher.NotifyFilter = NotifyFilters.Attributes
                                   | NotifyFilters.CreationTime
                                   | NotifyFilters.DirectoryName
                                   | NotifyFilters.FileName
                                   | NotifyFilters.LastAccess
                                   | NotifyFilters.LastWrite
                                   | NotifyFilters.Security
                                   | NotifyFilters.Size;
            watcher.Created += DirectoryChanged;
            watcher.Deleted += DirectoryChanged;
            watcher.Changed += DirectoryChanged;
            watcher.Renamed += DirectoryChanged;

        }

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
            var container = new GlobalFileContainer {GlobalData = new GlobalData()};
            container.Save(file);

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

        public void Load(FileInfo file)
        {
            if (IsLoaded)
            {
                // save and close current
                SaveCommand.Execute(null);
                CloseCommand.Execute(null);
            }

            LoadedFile = file;
            GlobalData = GlobalFileContainer.Load(LoadedFile).GlobalData;
            ReadSubdirectories();

            watcher.Path = LoadedFile.DirectoryName;
            watcher.EnableRaisingEvents = true;

            //force refresh every command's canexecute
            //CommandManager.InvalidateRequerySuggested();
        }

        private void DirectoryChanged(object sender, FileSystemEventArgs fileSystemEventArgs)
        {
            ReadSubdirectories();
        }

        public void Save()
        {
            // need to save local data in each subdir?
            var container = new GlobalFileContainer {GlobalData = GlobalData};
            container.Save(LoadedFile);
        }

        public void Close()
        {
            LoadedFile = null;
            GlobalData = null;
            Subdirectories.Clear();
            watcher.EnableRaisingEvents = false;
        }

        public void ReadSubdirectories()
        {
            var rootDir = new DirectoryInfo(LoadedFile.DirectoryName);
            Subdirectories = new ObservableCollection<SubfolderViewModel>(rootDir.GetDirectories()
                .Where(dir => dir.GetFiles(Locations.LocalFileName).Any())
                .Select(dir => new SubfolderViewModel(dir.FullName)));
        }

        public void RunSelected()
        {
            var work = Subdirectories
                .Where(z => z.Selected)
                .SelectMany(z => TutoProgram.MakeAll(z.FullPath))
                .ToArray();
            var window = new BatchWorkWindow();
            window.Run(work);
        }

        #region commands

        public RelayCommand NewCommand { get; private set; }
        public RelayCommand OpenCommand { get; private set; }
        public RelayCommand SaveCommand { get; private set; }
        public RelayCommand CloseCommand { get; private set; }
        public RelayCommand RefreshCommand { get; private set; }
        public RelayCommand RunSelectedCommand { get; private set; }

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
        private FileSystemWatcher watcher;

    }
}
