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
        Videotheque data;
        public GlobalLocations(Videotheque data)
        {
            this.data = data;
        }

        public FileInfo ProjectFile { get { return new FileInfo(Path.Combine(data.GlobalDataFolder.FullName, Locations.GlobalFileName)); } }

        public DirectoryInfo InputFolder { get { return new DirectoryInfo(Path.Combine(data.GlobalDataFolder.FullName, Locations.InputFolderName)); } }
        public DirectoryInfo TemporalFolder { get { return new DirectoryInfo(Path.Combine(data.GlobalDataFolder.FullName, Locations.AllTemporaryFilesFolder)); } }
        public DirectoryInfo PatchesFolder { get { return new DirectoryInfo(Path.Combine(data.GlobalDataFolder.FullName, Locations.ConvertedPatchFilesFolder)); } }



        public string RelativeTo(string path, string root)
        {
            if (!path.StartsWith(root))
                throw new ArgumentException();
            var result=path.Substring(root.Length, path.Length - root.Length);
            if (result.StartsWith("\\"))
                result=result.Substring(1, result.Length - 1);
            return result;
        }
        public string RelativeToGlobal(string path)
        {
            return RelativeTo(path, data.GlobalDataFolder.FullName);
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
