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
    public class ConvertFaceVideoWork : BatchWork
    {
        public ConvertFaceVideoWork(EditorModel model)
        {
            Model = model;
            Name = "Converting Face Video: " + model.Locations.FaceVideo;
        }

        private FileInfo tempFile;

        public override void Work()
        {
            tempFile = GetTempFile(Model.Locations.ConvertedFaceVideo);
            var args = string.Format(@"-i ""{0}"" -vf ""scale=1280:720, fps=25"" -q:v 0 -acodec libmp3lame -ar 44100 -ab 32k ""{1}"" -y",
                    Model.Locations.FaceVideo.FullName, tempFile.FullName);
            var fullPath = Model.Locations.FFmpegExecutable;
                RunProcess(args, fullPath.FullName);
            Thread.Sleep(500);
            if (Model.Locations.ConvertedFaceVideo.Exists)
                Model.Locations.ConvertedFaceVideo.Delete();
            File.Move(tempFile.FullName, Model.Locations.ConvertedFaceVideo.FullName);
            OnTaskFinished();
        }

        public override bool Finished()
        {
            return Model.Locations.ConvertedFaceVideo.Exists;
        }

        public override void Clean()
        {
            if (Process != null && !Process.HasExited)
                Process.Kill();
            while (Model.Locations.ConvertedFaceVideo.Exists)
                try
                {
                    File.Delete(Model.Locations.ConvertedFaceVideo.FullName);
                }
                catch { }
        }
    }
}
