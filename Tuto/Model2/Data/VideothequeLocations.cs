using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Tuto.Model
{
    public class VideothequeLocations
    {
        private readonly Videotheque videotheque;

        public VideothequeLocations(Videotheque videotheque)
        {
            this.videotheque = videotheque;
        }

        FileInfo Make(DirectoryInfo directory, params string[] fname)
        {
            fname=new[] { directory.FullName }.Concat(fname).ToArray();
            return new FileInfo(Path.Combine(fname));
        }



        public FileInfo PraatExecutable { get { return Make(videotheque.ProgramFolder, "praatcon.exe"); } }
        public FileInfo GNP { get { return Make(videotheque.ProgramFolder, "NoiseReduction", "gnp.exe"); } }
        public FileInfo NR { get { return Make(videotheque.ProgramFolder, "NoiseReduction", "nr.exe"); } }
		public FileInfo StartupSettings { get { return Make(videotheque.ProgramFolder, "startup.json"); } }
        

        public FileInfo FFmpegExecutable { get { return new FileInfo(videotheque.EnvironmentSettings.FFMPEGPath); } }
        public FileInfo SoxExecutable { get { return new FileInfo(videotheque.EnvironmentSettings.SoxPath); } }
        
    }
}
