using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Tuto.Model;
using System.Threading;

namespace Tuto.BatchWorks
{
    public class ConvertVideoWork : FFmpegWork
    {
        public ConvertVideoWork() { }

        public FileInfo Source;

        public FileInfo TempFile;

        private FileInfo convertedFile;

        private FileInfo nonConvertedFile;

        public override void Work()
        {
            nonConvertedFile = new FileInfo(Path.Combine(Model.Locations.TemporalDirectory.FullName, Path.ChangeExtension(Source.Name, ".avi")));
            TempFile = GetTempFile(nonConvertedFile);
            convertedFile = GetTempFile(nonConvertedFile, "-converted");

            if (!File.Exists(Source.FullName))
                throw new ArgumentException(Source.FullName + " not found");      
            var args = string.Format(@"-i ""{0}"" -vf ""scale=1280:720, fps=25"" -q:v 0 -acodec libmp3lame -ar 44100 -ab 32k ""{1}"" -y",
                   Source.FullName, TempFile.FullName);
            var fullPath = Model.Videotheque.Locations.FFmpegExecutable;
            RunProcess(args, fullPath.FullName);
            Thread.Sleep(500);
            if (convertedFile.Exists)
                convertedFile.Delete();
            File.Move(TempFile.FullName, convertedFile.FullName);
            OnTaskFinished();
        }

        public override bool Finished()
        {
            return convertedFile.Exists;
        }

        public override void Clean()
        {
            FinishProcess();
            TryToDelete(TempFile.FullName);
        }
    }
}
