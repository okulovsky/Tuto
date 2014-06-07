using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Windows;
using Tuto.Model;

namespace Tuto.Navigator
{
    [DataContract]
    class GlobalFileContainer
    {
        [DataMember]
        public GlobalData GlobalData { get; set; }

        [DataMember]
        public int Version { get; set; }

        public GlobalFileContainer()
        {
            Version = 1;
        }

        #region IO

        public void Save(DirectoryInfo dir)
        {
            var file = dir.GetFiles(Locations.GlobalFileName).FirstOrDefault();
            if (file == null)
                file = new FileInfo(Path.Combine(dir.FullName, Locations.GlobalFileName));
            Save(file);
        }

        public void Save(FileInfo file)
        {
            EditorModelIO.WriteJSonWithHeader(file, Header, Version, this);
        }

        public static GlobalFileContainer Load(DirectoryInfo dir)
        {
            var file = dir.GetFiles(Locations.GlobalFileName).FirstOrDefault();
            if (file == null)
                return new GlobalFileContainer{GlobalData = new GlobalData()};
            return Load(file);
        }

        public static GlobalFileContainer Load(FileInfo file)
        {
            return EditorModelIO.ReadJSonWithHeader<GlobalFileContainer>(file, Header);
        }
        #endregion

        private const string Header = "Tuto project file";
    }
}