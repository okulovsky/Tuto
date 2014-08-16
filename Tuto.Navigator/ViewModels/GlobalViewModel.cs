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

            //NewCommand = new RelayCommand(New);
            //OpenCommand = new RelayCommand(Open);
            //CloseCommand = new RelayCommand(Close, () => IsLoaded);
         

            SaveCommand = new RelayCommand(Save, () => IsLoaded);
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
                //SaveCommand.Execute(null);
                //CloseCommand.Execute(null);
            }

            LoadedFile = file;
            ReadSubdirectories();
        }

        public void ReadSubdirectories()
        {
            var data=EditorModelIO.ReadAllProjectData(LoadedFile.Directory);
            this.globalData=data.Global;
            Subdirectories = new ObservableCollection<SubfolderViewModel>();
            foreach(var e in data.Models)
                Subdirectories.Add(new SubfolderViewModel(e));
            Publish = new PublishViewModel(globalData);

                     
        }
     
        public void Save()
        {
            Publish.Commit();
            EditorModelIO.Save(GlobalData);
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
