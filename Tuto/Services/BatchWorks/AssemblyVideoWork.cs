using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Tuto.Model;
using Tuto.TutoServices;
using Tuto.TutoServices.Montager;
using Tuto.TutoServices.Assembler;

namespace Tuto.BatchWorks
{
    public class AssemblyVideoWork : BatchWork
    {

        private List<string> filesToDelIfAborted { get; set; }
        private bool crossFades {get; set;}
        public AssemblyVideoWork (EditorModel model, bool fadeMode)
        {
            Model = model;
            Name = "Assembly video: " + model.Locations.FaceVideo.Directory.Name;
            filesToDelIfAborted = new List<string>();
            if (!model.Locations.ConvertedDesktopVideo.Exists)
                BeforeWorks.Add(new ConvertDesktopWork(model));
            if (!model.Locations.ConvertedFaceVideo.Exists)
                BeforeWorks.Add(new ConvertFaceWork(model));
            this.crossFades = fadeMode;
        }

        public override void Work()
        {
            var serv = new AssemblerService(crossFades);
            var episodes = serv.GetEpisodesNodes(Model);
            var episodeNumber = 0;
            var count = episodes.Count;

            foreach (var episode in episodes)
            {
                var args = @"-i ""{0}"" -q:v 0 -vf ""scale=1280:720, fps=25"" -q:v 0 -acodec libmp3lame -ac 2 -ar 44100 -ab 32k ""{1}""";
                var avsContext = new AvsContext();
                episode.SerializeToContext(avsContext);
                var avsScript = avsContext.Serialize(Model);
                var avsFile = Model.Locations.GetAvsStriptFile(episodeNumber);

                File.WriteAllText(avsFile.FullName, avsScript);

                var videoFile = Model.Locations.GetOutputFile(episodeNumber);
                if (videoFile.Exists) videoFile.Delete();

                args = string.Format(args, avsFile.FullName, videoFile.FullName);
                filesToDelIfAborted.Add(videoFile.FullName);
                episodeNumber++;
                RunProcess(args, Model.Locations.FFmpegExecutable.FullName);

                if (Model.Locations.ClearedSound.Exists && Model.Global.WorkSettings.AudioCleanSettings.CurrentOption != Options.Skip)
                {
                    var soxExe = Model.Locations.SoxExecutable;
                    var sound = Model.Locations.ClearedSound;
                    var ffmpeg = Model.Locations.FFmpegExecutable;
                    var tempSound = Path.Combine(Model.Locations.TemporalDirectory.FullName, "temp.wav");
                    var normSound = Path.Combine(Model.Locations.TemporalDirectory.FullName, "norm.wav");
                    Shell.Exec(false, ffmpeg, string.Format(@" -i ""{0}"" ""{1}"" -y", Model.Locations.ClearedSound.FullName, tempSound));
                    Shell.Exec(false, soxExe, string.Format(@"""{0}"" ""{1}"" --norm", tempSound, normSound));
                    Shell.Exec(false, ffmpeg, string.Format(@"-i ""{0}"" -ar 44100 -ac 2 -ab 192k -f mp3 -qscale 0 ""{1}"" -y", normSound, Model.Locations.ClearedSound));
                    var tempVideo =  GetTempFile(videoFile).FullName;
                    var arguments = string.Format(
                        @"-i ""{0}"" -i ""{1}"" -map 0:0 -map 1 -vcodec copy -acodec copy ""{2}"" -y",
                        videoFile.FullName,
                        Model.Locations.ClearedSound.FullName,
                        tempVideo
                        );
                    Shell.Exec(false, ffmpeg, arguments);
                    File.Delete(tempSound);
                    File.Delete(videoFile.FullName);
                    File.Delete(normSound);
                    File.Move(tempVideo, videoFile.FullName);
                    File.Delete(tempVideo);
                }
                OnTaskFinished();
            }
        }

        public override void Clean()
        {
            FinishProcess();
            foreach (var e in filesToDelIfAborted)
                TryToDelete(e);
        }
    }
}
