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
    public class RepairVideoWork : BatchWork
    {
        public RepairVideoWork(EditorModel model, FileInfo source)
        {
            Model = model;
            Name = "Repair Video: " + source;
            this.source = source;
        }

        private string tempFile = "none";
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
                tempFile = GetTempFile(source).ToString();

                CopyingOver = true;
                var args = string.Format(@"-i ""{0}"" -vf scale=1280:720 -r 25 -q:v 0 {2} -acodec libmp3lame -ar 44100 -ab 32k ""{1}"" -y",
                        source.FullName, tempFile, codec);
                var fullPath = Model.Locations.FFmpegExecutable;
                RunProcess(args, fullPath.FullName);
                ConversionOver = true;
                File.Replace(tempFile, source.FullName, originPath);
            }
            OnTaskFinished();
        }

        public override void Clean()
        {
            if (Process != null && !Process.HasExited)
                Process.Kill();
            Thread.Sleep(1000); //this time is required for system to free recources
            if (File.Exists(tempFile) && CopyingOver)
            {
                if (source.Exists)
                    try
                    {
                        File.Delete(tempFile);
                    }
                    catch { }
            }
        }
    }
}
