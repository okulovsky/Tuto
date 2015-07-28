using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Tuto.Model;
using System.Windows.Controls;

namespace Tuto.BatchWorks
{
    public class CreateThumbWork : BatchWork
    {
        public CreateThumbWork(FileInfo source, EditorModel model)
        {
            Name = "Thumb Video: " + source;
            this.source = source;
            this.Model = model;
        }

        private FileInfo source;
        private string temp;
        public override void Work()
        {
            if (source.Exists)
            {
                var codec = "-vcodec libxvid";
                var newPath = source.FullName.Split('\\');
                var nameAndExt = source.Name.Split('.');
                nameAndExt[0] = nameAndExt[0] + "-thumb";
                newPath[newPath.Length - 1] = string.Join(".", nameAndExt);
                temp = string.Join("\\", newPath);
                var args = string.Format(@"-i ""{0}"" -r 25 -q:v 15 {2} -acodec libmp3lame -ar 44100 -ab 32k ""{1}"" -y",
                        source.FullName, temp, codec);
                var fullPath = Model.Locations.FFmpegExecutable;
                RunProcess(args, fullPath.FullName);
            }
            OnTaskFinished();
        }

        public override void Clean()
        {
            if (Process != null && !Process.HasExited)
                Process.Kill();
            if (File.Exists(temp))
            {
                while (File.Exists(temp))
                    try
                    {
                        File.Delete(temp);
                    }
                    catch { }
            }
        }
    }
}
