using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Tuto.Model;

namespace Tuto.BatchWorks
{
    public class RepairVideoWork : BatchWork
    {
        public RepairVideoWork(EditorModel model, FileInfo source)
        {
            Model = model;
            Name = "Repair Video: " + source;
            this.source = source;
        }

        private string tempFile;
        private bool CopyingOver;
        private bool ConversionOver;
        private bool FileDeleted;
        private FileInfo source;
        public override void Work()
        {
            if (source.Exists)
            {
                var codec = "-vcodec libxvid";
                var newPath = source.FullName.Split('\\');
                var nameAndExt = source.Name.Split('.');
                nameAndExt[0] = nameAndExt[0] + "-origin";
                newPath[newPath.Length - 1] = string.Join(".", nameAndExt);
                var originPath = string.Join("\\", newPath);
                tempFile = originPath;
                File.Copy(source.FullName, originPath);
                CopyingOver = true;
                File.Delete(source.FullName);
                FileDeleted = true;
                Args = string.Format(@"-i ""{0}"" -vf scale=1280:720 -r 25 -q:v 0 {2} -acodec libmp3lame -ar 44100 -ab 32k ""{1}""",
                        originPath, source.FullName, codec);
                FullPath = Model.Locations.FFmpegExecutable.FullName;
                RunProcess();
                ConversionOver = true;
            }
        }

        public override void Clean()
        {
            if (Process != null && !Process.HasExited)
                Process.Kill();
            if (source.Exists && CopyingOver)
            {
                while (source.Exists)
                    try
                    {
                        File.Delete(source.FullName);
                    }
                    catch { }
                File.Move(tempFile, source.FullName);
            }
        }
    }
}
