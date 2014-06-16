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
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

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
            LoadAndBindProject(new FileInfo("c:\\tuto\\testmodels\\project.tuto"));
#endif
        }

        public void LoadAndBindProject(FileInfo file)
        {
            LoadedGlobalModel = new GlobalModel(file);
            RefreshCommand = new Command(LoadedGlobalModel.ReadSubdirectories);
            SaveCommand = new Command(LoadedGlobalModel.Save);
            CloseCommand.CanExecute = true;
            TestCommand = new Command(LoadedGlobalModel.Save);
        }

        public void Open()
        {
            CloseCommand.Execute(null);
            var dialog = new OpenFileDialog
            {
                Filter = "Tuto project|project.tuto", FilterIndex = 0
            };
            var result = dialog.ShowDialog();  // fail if there's no file
            if (!(result.HasValue && result.Value))
                return;
            var file = new FileInfo(dialog.FileName);
            LoadAndBindProject(file);
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
