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
    public class CreateCleanSoundWork : BatchWork
    {

        public CreateCleanSoundWork(FileInfo source, EditorModel model)
        {
            Name = "Make clean sound: " + source;
            this.model = model;
            this.source= source;
        }

        private string temp = "not_found";
        private EditorModel model;
        private FileInfo source;

        public override void Work()
        {
            if (source.Exists)
            {
                string args = "/C {0}&{1}&{2}&{3}";
                var loc = model.Locations.FaceVideo;
                var temp = model.Locations.TemporalDirectory;
                var firstArg = string.Format("ffmpeg -i {0} -y -shortest {1}\\temp.mp3", loc, temp);
                var secondArg = string.Format("ffmpeg -i {0}\\temp.mp3 -ss 0 -t 1 -y {0}\\sample.mp3", temp);
                var thirdArg = string.Format("sox {0}\\sample.mp3 -n noiseprof noise.prof", temp);
                var fourthArg = string.Format("sox {0}\\temp.mp3 {0}\\cleaned.mp3 noisered noise.prof", temp);
                var fifthArg = string.Format("ffmpeg -i {0}\\temp2.mp3 -qscale 0 -shortest -acodec libmp3lame {1}\\cleaned.mp3 -y", temp, temp);
                Args = string.Format(args, firstArg, secondArg, thirdArg, fourthArg);
                FullPath = "CMD.exe";
                RunProcess();
            }
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
