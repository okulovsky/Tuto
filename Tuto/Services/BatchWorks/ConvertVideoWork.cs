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
    public class ConvertVideoWork : BatchWork
    {
        public ConvertVideoWork(EditorModel model, FileInfo src)
        {
            Model = model;
            Name = "Converting Video: " + src;
            source = src;
            nonConvertedFile = new FileInfo(Path.Combine(Model.Locations.TemporalDirectory.FullName, Path.ChangeExtension(src.Name, ".avi")));
            tempFile = GetTempFile(nonConvertedFile);
            convertedFile = GetTempFile(nonConvertedFile, "-converted");
        }

        private FileInfo source;

        private FileInfo tempFile;

        private FileInfo convertedFile;

        private FileInfo nonConvertedFile;

        public override void Work()
        {
            if (!File.Exists(source.FullName))
                throw new ArgumentException(source.FullName + " not found");      
            var args = string.Format(@"-i ""{0}"" -vf ""scale=1280:720, fps=25"" -q:v 0 -acodec libmp3lame -ar 44100 -ab 32k ""{1}"" -y",
                   source.FullName, tempFile.FullName);
            var fullPath = Model.Locations.FFmpegExecutable;
            RunProcess(args, fullPath.FullName);
            Thread.Sleep(500);
            if (convertedFile.Exists)
                convertedFile.Delete();
            File.Move(tempFile.FullName, convertedFile.FullName);
            OnTaskFinished();
        }

        public override bool Finished()
        {
            return convertedFile.Exists;
        }

        public override void Clean()
        {
            FinishProcess();
            TryToDelete(tempFile.FullName);
        }
    }
}
