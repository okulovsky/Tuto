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
        public GlobalModel(string rootDirectory)
        {
            // load or create empty
            RootDirectory = new DirectoryInfo(rootDirectory);
            GlobalData = GlobalFileContainer.Load(RootDirectory).GlobalData;
            ReadSubdirectories();
        }

        public void Save()
        {
            /*
             * need save local data in each subdir?
             */
            var container = new GlobalFileContainer {GlobalData = GlobalData};
            container.Save(RootDirectory);
        }

        public void ReadSubdirectories()
        {
            Subdirectories = new ObservableCollection<SubfolderViewModel>(RootDirectory.GetDirectories()
                .Where(dir => dir.GetFiles(Locations.LocalFileName).Any())
                .Select(dir => new SubfolderViewModel(dir.FullName)));
        }

        #region properties
        public DirectoryInfo RootDirectory
        {
            get { return directory; }
            private set
            {
                directory = value;
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

        private DirectoryInfo directory;
        private GlobalData globalData;
        private ObservableCollection<SubfolderViewModel> subdirectories;
    }
}
