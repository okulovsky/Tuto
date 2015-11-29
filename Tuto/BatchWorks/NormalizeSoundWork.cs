using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Tuto.Model;
using Tuto.TutoServices;
using Tuto.TutoServices.Assembler;

namespace Tuto.BatchWorks
{
    public class NormalizeSoundWork : BatchWork
    {
        private List<string> filesToDelIfAborted { get; set; }
        private FileInfo videoFile { get; set; }
        public NormalizeSoundWork(EditorModel model, FileInfo src)
        {
            Model = model;
            Name = "Normalize sound: " + src.FullName;
            videoFile = src;
            filesToDelIfAborted = new List<string>();
        }

        public override void Work()
        {
                //apply normalisation
            if (Model.Locations.ClearedSound.Exists && Model.Videotheque.Data.WorkSettings.AudioCleanSettings.CurrentOption != Options.Skip)
            {
                var soxExe = Model.Videotheque.Locations.SoxExecutable;
                var sound = Model.Locations.ClearedSound;
                var ffmpeg = Model.Videotheque.Locations.FFmpegExecutable;
                var tempSound = Path.Combine(Model.Locations.TemporalDirectory.FullName, "temp.wav");
                var normSound = Path.Combine(Model.Locations.TemporalDirectory.FullName, "norm.wav");
                filesToDelIfAborted.Add(tempSound);
                filesToDelIfAborted.Add(normSound);
                RunProcess(string.Format(@" -i ""{0}"" ""{1}"" -y", videoFile.FullName, tempSound), ffmpeg.FullName);
                RunProcess(string.Format(@"""{0}"" ""{1}"" --norm", tempSound, normSound), soxExe.FullName);
                RunProcess(string.Format(@"-i ""{0}"" -ar 44100 -ac 2 -ab 192k -f mp3 -qscale 0 ""{1}"" -y", normSound, tempSound), ffmpeg.FullName);
                var tempVideo = GetTempFile(videoFile).FullName;
                filesToDelIfAborted.Add(tempVideo);
                var arguments = string.Format(
                    @"-i ""{0}"" -i ""{1}"" -map 0:0 -map 1 -vcodec copy -acodec copy ""{2}"" -y",
                    videoFile.FullName,
                    tempSound,
                    tempVideo
                    );
                RunProcess(arguments, ffmpeg.FullName);
                File.Delete(tempSound);
                File.Delete(videoFile.FullName);
                File.Delete(normSound);
                File.Move(tempVideo, videoFile.FullName);
                File.Delete(tempVideo);
            }
            OnTaskFinished();
        }

        public override void Clean()
        {
            FinishProcess();
            foreach (var e in filesToDelIfAborted)
                TryToDelete(e);
        }
    }
}
