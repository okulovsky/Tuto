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

        internal FileInfo Make(DirectoryInfo info, string fname)
        {
            return new FileInfo(Path.Combine(info.FullName, fname));
        }

        public FileInfo ProjectFile { get { return Make(data.GlobalDataFolder, Locations.GlobalFileName); } }
        public FileInfo PraatExecutable { get { return Make(model.ProgramFolder, "praatcon.exe"); } }
        public FileInfo PraatScriptSource { get { return Make(model.ProgramFolder, "split_pauses.praat"); } }
        public FileInfo AvsLibrary { get { return Make(model.ProgramFolder, "library.avs"); } }
        public FileInfo AutoLevelsLibrary { get { return Make(model.ProgramFolder, "autolevels.dll"); } }
        public FileInfo VSFilterLibrary { get { return new FileInfo(@"C:\Program Files\VSFilter\VSFilter.dll"); } }


        public FileInfo TutoExecutable { get { return Make(model.ProgramFolder, "Tuto.exe"); } }


        public FileInfo IntroImage { get { return Make(model.VideoFolder, "intro.png"); } }
        public FileInfo WatermarkImage { get { return Make(model.VideoFolder, "watermark.png"); } }


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
