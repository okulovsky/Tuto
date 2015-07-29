using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Tuto.Model;

namespace Tuto.BatchWorks
{
    public class ConvertFaceVideoWork : BatchWork
    {
        public ConvertFaceVideoWork(EditorModel model)
        {
            Model = model;
            Name = "Preparing Face Video: " + model.Locations.FaceVideo;
        }
        public override void Work()
        {
            if (Model.Locations.FaceVideo.Exists && !Model.Locations.ConvertedFaceVideo.Exists)
            {
                var args = string.Format(@"-i ""{0}"" -vf ""scale=1280:720, fps=25"" -q:v 0 -acodec libmp3lame -ar 44100 -ab 32k ""{1}""",
                        Model.Locations.FaceVideo.FullName, Model.Locations.ConvertedFaceVideo.FullName);
                var fullPath = Model.Locations.FFmpegExecutable;
                RunProcess(args, fullPath.FullName);
            }
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
