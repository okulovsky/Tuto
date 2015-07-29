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
    public class ConvertDesktopVideoWork : BatchWork
    {
        public ConvertDesktopVideoWork(EditorModel model)
        {
            Model = model;
            Name = "Converting Desktop Video: " + model.Locations.DesktopVideo;
        }

        private FileInfo tempFile;

        public override void Work()
        {
            tempFile = GetTempFile(Model.Locations.ConvertedDesktopVideo);          
            var args = string.Format(@"-i ""{0}"" -vf ""scale=1280:720, fps=25"" -q:v 0 -an ""{1}"" -y",
                                        Model.Locations.DesktopVideo.FullName, tempFile.FullName);
            var fullPath = Model.Locations.FFmpegExecutable;
            RunProcess(args, fullPath.FullName);
            Thread.Sleep(500);
            if (Model.Locations.ConvertedDesktopVideo.Exists)
                Model.Locations.ConvertedDesktopVideo.Delete();
            File.Move(tempFile.FullName, Model.Locations.ConvertedDesktopVideo.FullName);
            OnTaskFinished();
        }

        public override bool Finished()
        {
            return Model.Locations.ConvertedDesktopVideo.Exists;
        }

        public override void Clean()
        {
            if (Process != null && !Process.HasExited)
                Process.Kill();
            while (tempFile.Exists)
                try
                {
                    File.Delete(tempFile.FullName);
                }
                catch { }
        }
    }
}
