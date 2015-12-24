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
    public class AssemblyEpisodeWork : BatchWork
    {

        private List<string> filesToDelIfAborted { get; set; }
        private bool crossFades {get; set;}
        private EpisodInfo episodeInfo { get; set; }
        private int episodeNumber { get; set; }
        private AvsNode episodeNode { get; set; }

        public AssemblyEpisodeWork(EditorModel model, EpisodInfo episodeInfo)
        {
            Model = model;
            var videoFile = Model.Locations.GetOutputFile(episodeInfo);
            crossFades = model.Videotheque.Data.EditorSettings.CrossFadesEnabled;
            Name = "Assembly episode: " + videoFile.FullName;
            this.episodeInfo = episodeInfo;
            this.episodeNode = episodeNode;
            this.episodeNumber = Model.Montage.Information.Episodes.IndexOf(episodeInfo);
            filesToDelIfAborted = new List<string>();

            episodeNode = new AssemblerService(crossFades).GetEpisodesNodes(model)[episodeNumber];
            BeforeWorks.Add(new ConvertDesktopWork(model, false));
            BeforeWorks.Add(new ConvertFaceWork(model, false));
            BeforeWorks.Add(new CreateCleanSoundWork(model.Locations.FaceVideo, model, false));
            AfterWorks.Add(new NormalizeSoundWork(Model, videoFile));
            AfterWorks[AfterWorks.Count - 1].TaskFinished += (s, a) => OnTaskFinished();
        }

        public override void Work()
        {
                var args = @"-i ""{0}"" -q:v 0 -vf ""scale=1280:720, fps=25"" -q:v 0 -acodec libmp3lame -ac 2 -ar 44100 -ab 32k ""{1}""";
                var avsContext = new AvsContext();
                episodeNode.SerializeToContext(avsContext);
                var avsScript = avsContext.Serialize(Model);
                var avsFile = Model.Locations.GetAvsStriptFile(episodeNumber);

                File.WriteAllText(avsFile.FullName, avsScript, Encoding.GetEncoding("Windows-1251"));

                var videoFile = Model.Locations.GetOutputFile(episodeInfo);
                if (videoFile.Exists) videoFile.Delete();

                args = string.Format(args, avsFile.FullName, videoFile.FullName);
                filesToDelIfAborted.Add(videoFile.FullName);
                RunProcess(args, Model.Videotheque.Locations.FFmpegExecutable.FullName);
        }

        public override void Clean()
        {
            FinishProcess();
            foreach (var e in filesToDelIfAborted)
                TryToDelete(e);
        }
    }
}
