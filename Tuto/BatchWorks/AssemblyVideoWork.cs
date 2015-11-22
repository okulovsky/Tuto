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
    public class AssemblyVideoWork : BatchWork
    {

        private List<string> filesToDelIfAborted { get; set; }
        private bool crossFades {get; set;}
        private List<AvsNode> episodes { get; set; }

        public AssemblyVideoWork (EditorModel model)
        {
            Model = model;
            crossFades = model.Videotheque.CrossFadesEnabled;
            Name = "Assembly video: " + model.Locations.FaceVideo.Directory.Name;
            filesToDelIfAborted = new List<string>();
            BeforeWorks.Add(new ConvertDesktopWork(model, false));
            BeforeWorks.Add(new ConvertFaceWork(model, false));
            BeforeWorks.Add(new CreateCleanSoundWork(model.Locations.FaceVideo, model, false));

            var service = new AssemblerService(crossFades);
            episodes = service.GetEpisodesNodes(Model);
            var episodeNumber = 0;
            foreach (var episode in episodes)
            {
                var videoFile = Model.Locations.GetOutputFile(episodeNumber);
                AfterWorks.Add(new NormalizeSoundWork(Model, videoFile));
                episodeNumber++;
            }
        }

        public override void Work()
        {
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
                RunProcess(args, Model.Videotheque.Locations.FFmpegExecutable.FullName);
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
