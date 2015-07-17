using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuto;
using Tuto.Model;

namespace Tuto.TutoServices
{
    public class NoiseReductionService
    {
        public void CreateCleanedMP3(EditorModel model)
        {
            string args = "/C {0}&{1}&{2}&{3}";
            var loc = model.Locations.FaceVideo;
            var temp = model.Locations.TemporalDirectory;
            var firstArg = string.Format("ffmpeg -i {0} -y -shortest {1}\\temp.mp3", loc, temp);
            var secondArg = string.Format("ffmpeg -i {0}\\temp.mp3 -ss 0 -t 1 -y {0}\\sample.mp3", temp);
            var thirdArg = string.Format("sox {0}\\sample.mp3 -n noiseprof noise.prof", temp);
            var fourthArg = string.Format("sox {0}\\temp.mp3 {0}\\cleaned.mp3 noisered noise.prof", temp);
            var fifthArg = string.Format("ffmpeg -i {0}\\temp2.mp3 -qscale 0 -shortest -acodec libmp3lame {1}\\cleaned.mp3 -y", temp, temp);
            var strCmdText = string.Format(args, firstArg, secondArg, thirdArg, fourthArg);
            var pr = Process.Start("CMD.exe", strCmdText);
            pr.WaitForExit();
        }
    }
}
