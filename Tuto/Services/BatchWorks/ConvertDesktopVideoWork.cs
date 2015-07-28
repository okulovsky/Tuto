﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Tuto.Model;

namespace Tuto.BatchWorks
{
    public class ConvertDesktopVideoWork : BatchWork
    {
        public ConvertDesktopVideoWork(EditorModel model)
        {
            Model = model;
            Name = "Preparing Desktop Video: " + model.Locations.DesktopVideo;
        }
        public override void Work()
        {
            if (Model.Locations.DesktopVideo.Exists && !Model.Locations.ConvertedDesktopVideo.Exists)
            {
                Args = string.Format(@"-i ""{0}"" -vf ""scale=1280:720, fps=25"" -q:v 0 -an ""{1}""",
                                            Model.Locations.DesktopVideo.FullName, Model.Locations.ConvertedDesktopVideo.FullName);
                FullPath = Model.Locations.FFmpegExecutable.FullName;
                RunProcess();
            }
            OnTaskFinished();
        }

        public override void Clean()
        {
            if (Process != null && !Process.HasExited)
                Process.Kill();
            while (Model.Locations.ConvertedDesktopVideo.Exists)
                try
                {
                    File.Delete(Model.Locations.ConvertedDesktopVideo.FullName);
                }
                catch { }
        }
    }
}
