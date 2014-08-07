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
    public class GlobalFileIO
    {
        public static void Save(GlobalData data, FileInfo file)
        {
            EditorModelIO.WriteJSonWithHeader(file, Header, 0, data);
        }

        public static GlobalData Load(DirectoryInfo dir)
        {
            var file = dir.GetFiles(Locations.GlobalFileName).FirstOrDefault();
            if (file == null)
                return new GlobalData();
            return Load(file);
        }

        public static GlobalData Load(FileInfo file)
        {
            return EditorModelIO.ReadGlobalData(file.Directory);
        }

        private const string Header = "Tuto project file";
    }
}