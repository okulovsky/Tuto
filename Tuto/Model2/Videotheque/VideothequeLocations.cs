using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Tuto.Model2
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

		FileInfo FileOrNull(string path)
		{
			if (path == null) return null;
			return new FileInfo(path);
		}

        public FileInfo PraatExecutable { get { return Make(videotheque.ProgramFolder, "praatcon.exe"); } }
		
		public FileInfo GNP { get { return Make(videotheque.ProgramFolder, "NoiseReduction", "gnp.exe"); } }
        public FileInfo NR { get { return Make(videotheque.ProgramFolder, "NoiseReduction", "nr.exe"); } }
		public FileInfo StartupSettings { get { return Make(videotheque.ProgramFolder, "startup.json"); } }

		[Obsolete]
		public FileInfo AvsLibrary { get { return Make(videotheque.ProgramFolder, "library.avs"); } }
		[Obsolete]
		public FileInfo AutoLevelsLibrary { get { return Make(videotheque.ProgramFolder, "autolevels.dll"); } }
		[Obsolete]
		public FileInfo VSFilterLibrary { get { return new FileInfo(@"C:\Program Files\VSFilter\VSFilter.dll"); } }
		[Obsolete]
		public FileInfo PraatScriptSource { get { return Make(videotheque.ProgramFolder, "split_pauses.praat"); } }
        
        public FileInfo FFmpegExecutable { get { return FileOrNull(videotheque.EnvironmentSettings.FFMPEGPath); } }
        public FileInfo SoxExecutable { get { return FileOrNull(videotheque.EnvironmentSettings.SoxPath); } }



	//	public string RelativeToGlobal(string path)
	//	{
	//		return RelativeTo(path, data.GlobalDataFolder.FullName);
	//	}

	//	public FileInfo AbsoluteFileLocation(string relativePath)
	//	{
	//		return new FileInfo(Path.Combine(data.GlobalDataFolder.FullName, relativePath));
	//	}
	//	public DirectoryInfo AbsoluteDirectoryLocation(string relativePath)
	//	{
	//		return new DirectoryInfo(Path.Combine(data.GlobalDataFolder.FullName, relativePath));
	//}	
    }
}
