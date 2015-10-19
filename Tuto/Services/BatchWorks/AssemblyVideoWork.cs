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
                BeforeWorks.Add(new ConvertVideoWork(model, model.Locations.DesktopVideo));
            if (!model.Locations.ConvertedFaceVideo.Exists)
                BeforeWorks.Add(new ConvertVideoWork(model, model.Locations.FaceVideo));
            this.crossFades = fadeMode;
            if (Model.Global.WorkSettings.AudioCleanSettings.CurrentOption != Options.Skip)
            {
                var index = 0;
                var serv = new AssemblerService(crossFades);
                foreach (var e in serv.GetEpisodesNodes(Model))
                {
                    var name = Model.Locations.GetOutputFile(index);
                    AfterWorks.Add(new CreateCleanSoundWork(name, Model));
                    index++;
                }
            }
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
