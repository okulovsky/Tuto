using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Media3D;
using Tuto.Model;

namespace Tuto.Navigator
{
    public class GlobalModel : NotifierModel
    {
        public GlobalModel(FileInfo file)
        {
            LoadedFile = file;
            // load or create empty
            GlobalData = GlobalFileContainer.Load(LoadedFile).GlobalData;
            ReadSubdirectories();
        }

        public void Save()
        {
            /*
             * need save local data in each subdir?
             */
            var container = new GlobalFileContainer {GlobalData = GlobalData};
            container.Save(LoadedFile);
        }

        public void ReadSubdirectories()
        {
            var rootDir = new DirectoryInfo(LoadedFile.DirectoryName);
            Subdirectories = new ObservableCollection<SubfolderViewModel>(rootDir.GetDirectories()
                .Where(dir => dir.GetFiles(Locations.LocalFileName).Any())
                .Select(dir => new SubfolderViewModel(dir.FullName)));
        }

        #region properties
        public FileInfo LoadedFile
        {
            get { return loadedFile; }
            private set
            {
                loadedFile = value;
                NotifyPropertyChanged();
            }
        }
        
        public GlobalData GlobalData
        {
            get { return globalData; }
            private set
            {
                globalData = value;
                NotifyPropertyChanged();
            }
        }
        
        public ObservableCollection<SubfolderViewModel> Subdirectories
        {
            get { return subdirectories; }
            private set
            {
                subdirectories = value;
                NotifyPropertyChanged();
            }
        }
        #endregion

        private FileInfo loadedFile;
        private GlobalData globalData;
        private ObservableCollection<SubfolderViewModel> subdirectories;
    }
}
