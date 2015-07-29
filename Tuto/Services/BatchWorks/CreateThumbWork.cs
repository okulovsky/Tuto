using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Tuto.Model;
using System.Windows.Controls;
using System.Threading;

namespace Tuto.BatchWorks
{
    public class CreateThumbWork : BatchWork
    {
        public CreateThumbWork(FileInfo source, EditorModel model)
        {
            Name = "Thumb Video: " + source;
            this.Source = source;
            this.Model = model;
        }

        public FileInfo Source;
        private FileInfo tempFile;
        public FileInfo ThumbName;

        public override void Work()
        {
            var codec = "-vcodec libxvid";
            var newPath = Source.FullName.Split('\\');
            var nameAndExt = Source.Name.Split('.');
            nameAndExt[0] = nameAndExt[0] + "-thumb";
            newPath[newPath.Length - 1] = string.Join(".", nameAndExt);
            ThumbName = new FileInfo(string.Join("\\", newPath));
            tempFile = GetTempFile(Source);

            var args = string.Format(@"-i ""{0}"" -r 25 -q:v 15 {2} -acodec libmp3lame -ar 44100 -ab 32k ""{1}"" -y",
                    Source.FullName, tempFile.FullName, codec);
            var fullPath = Model.Locations.FFmpegExecutable;
            RunProcess(args, fullPath.FullName);
            Thread.Sleep(500);
            if (ThumbName.Exists)
                ThumbName.Delete();
            File.Move(tempFile.FullName, ThumbName.FullName);
            OnTaskFinished();
        }

        public override bool Finished()
        {
            return Model.Locations.GetThumbName(Source).Exists;
        }

        public override void Clean()
        {
            if (Process != null && !Process.HasExited)
                Process.Kill();
            if (tempFile.Exists)
            {
                while (tempFile.Exists)
                    try
                    {
                        File.Delete(tempFile.FullName);
                    }
                    catch { }
            }
        }
    }
}
