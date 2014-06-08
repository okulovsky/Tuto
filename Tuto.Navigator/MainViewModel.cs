using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using Microsoft.Win32;

namespace Tuto.Navigator
{
    class MainViewModel: NotifierModel
    {
        public MainViewModel()
        {
            OpenCommand = new Command(Open);
            SaveCommand = new Command(null, false);
            CloseCommand = new Command(Close, false);
            RefreshCommand = new Command(null, false);
            
#if DEBUG
            LoadAndBindProject("c:\\tuto\\testmodels");
#endif
        }

        public void LoadAndBindProject(string directory)
        {
            LoadedGlobalModel = new GlobalModel(directory);
            RefreshCommand = new Command(LoadedGlobalModel.ReadSubdirectories);
            SaveCommand = new Command(LoadedGlobalModel.Save);
            CloseCommand.CanExecute = true;
            TestCommand = new Command(LoadedGlobalModel.Save);
        }

        public void Open()
        {
            CloseCommand.Execute(null);
            // WPF has no folder dialog??!
            /*var dialog = new OpenFileDialog
            {
                Filter = "Tuto project|project.tuto|Все файлы (*.*)|*.*", FilterIndex = 0
            };
            var result = dialog.ShowDialog();  // fail if there's no file
            if (!(result.HasValue && result.Value))
                return;
            var filename = dialog.FileName;
            var dir = Path.GetDirectoryName(filename);
            LoadAndBindProject(dir);*/
            // TODO: use any fancy dialog instead of this
            var dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() != DialogResult.OK)
                return;
            var dir = dialog.SelectedPath;
            LoadAndBindProject(dir);
        }

        public void Close()
        {
            SaveCommand.Execute(null);
            LoadedGlobalModel = null;
            SaveCommand.CanExecute = false;
            CloseCommand.CanExecute = false;
            RefreshCommand.CanExecute = false;
        }

        public GlobalModel LoadedGlobalModel
        {
            get { return loadedGlobalModel; }
            private set
            {
                loadedGlobalModel = value;
                NotifyPropertyChanged();
            }
        }

        #region commands

        public Command RefreshCommand
        {
            get { return refreshCommand; }
            private set
            {
                refreshCommand = value;
                NotifyPropertyChanged();
            }
        }

        public Command TestCommand
        {
            get { return testCommand; }
            private set
            {
                testCommand = value;
                NotifyPropertyChanged();
            }
        }

        public Command OpenCommand
        {
            get { return openCommand; }
            private set
            {
                openCommand = value;
                NotifyPropertyChanged();
            }
        }

        public Command SaveCommand
        {
            get { return saveCommand; }
            private set
            {
                saveCommand = value;
                NotifyPropertyChanged();
            }
        }

        public Command CloseCommand
        {
            get { return closeCommand; }
            private set
            {
                closeCommand = value;
                NotifyPropertyChanged();
            }
        }

        private Command refreshCommand;

        private Command testCommand;

        private Command openCommand;

        private Command saveCommand;

        private Command closeCommand;

        
        
        #endregion

        private GlobalModel loadedGlobalModel;

    }
}
