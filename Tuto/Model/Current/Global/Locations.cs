using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuto.Model
{
    public class GlobalLocations
    {
        GlobalData data;
        public GlobalLocations(GlobalData data)
        {
            this.data = data;
        }

        public FileInfo ProjectFile { get { return new FileInfo(Path.Combine(data.GlobalDataFolder.FullName, Locations.GlobalFileName)); } }

        public DirectoryInfo InputFolder { get { return new DirectoryInfo(Path.Combine(data.GlobalDataFolder.FullName, Locations.InputFolderName)); }} 

        public string RelativeToGlobal(string path)
        {
            if (!path.StartsWith(data.GlobalDataFolder.FullName))
                throw new ArgumentException();
            return path.Substring(data.GlobalDataFolder.FullName.Length, path.Length - data.GlobalDataFolder.FullName.Length);
        }

        public FileInfo AbsoluteFileLocation(string relativePath)
        {
            return new FileInfo(Path.Combine(data.GlobalDataFolder.FullName, relativePath));
        }
        public DirectoryInfo AbsoluteDirectoryLocation(string relativePath)
        {
            return new DirectoryInfo(Path.Combine(data.GlobalDataFolder.FullName, relativePath));
        }

    }
}
