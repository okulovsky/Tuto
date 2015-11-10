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
		readonly bool isFace;
        public ConvertVideoWork(EditorModel model, FileInfo src, bool isFace)
        {
            Model = model;
            Name = "Converting Video: " + src;
            source = src;
			this.isFace = isFace;
        }

        private FileInfo source;

        private FileInfo tempFile;

        public override void Work()
        {
            if (!File.Exists(source.FullName))
                throw new ArgumentException(source.FullName + " not found");
            var nameAndExtension = source.Name.Split('.');
            nameAndExtension[1] = "avi";
            var nonConverted = new FileInfo(Path.Combine(Model.Locations.TemporalDirectory.FullName, string.Join(".", nameAndExtension)));
            tempFile = GetTempFile(nonConverted);        
            var args = string.Format(@"-i ""{0}"" -vf ""scale=1280:720, fps=25"" -q:v 0 -acodec libmp3lame -ar 44100 -ab 32k ""{1}"" -y",
                   source.FullName, tempFile.FullName);
            var fullPath = Model.Locations.FFmpegExecutable;
            RunProcess(args, fullPath.FullName);
            Thread.Sleep(500);
            var convertedFile = GetTempFile(nonConverted, "-converted");
            if (convertedFile.Exists)
                convertedFile.Delete();
            File.Move(tempFile.FullName, convertedFile.FullName);
            OnTaskFinished();
        }

        public override bool Finished()
        {
			if (!isFace)
				return Model.Locations.ConvertedDesktopVideo.Exists;
			else
				return Model.Locations.ConvertedFaceVideo.Exists;
		}

        public override void Clean()
        {
            if (Process != null && !Process.HasExited)
                Process.Kill();
            while (tempFile != null && tempFile.Exists)
                try
                {
                    File.Delete(tempFile.FullName);
                }
                catch { }
        }
    }
}
