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
    public class PatchWork : BatchWork
    {

        private List<string> filesToDelIfAborted { get; set; }
        private bool crossFades {get; set;}
        private PatchModel model;
        private AvsNode result;

        public PatchWork(PatchModel model, bool fadeMode)
        {
            this.model = model;
        }

        public override void Work()
        {
            var src = model.SourceInfo;
            List<AvsNode> chunks = new List<AvsNode>();
            var tracks = model.MediaTracks;
            double previous = 0;
            for (var i = 0; i < tracks.Count - 1; i++)
            {
                var avs = new AvsPatchChunk();
                if (i % 2 == 0)
                {
                    avs.Load(model.SourceInfo.FullName, previous, tracks[i + 1].StartSecond + tracks[i + 1].LeftShift);
                    chunks.Add(avs);
                    continue;
                }
                else
                {
                    avs.Load(tracks[i].Path.AbsolutePath, tracks[i].StartSecond, tracks[i].EndSecond);
                }
            }

            if (tracks.Count == 0)
            {
                var s = new AvsPatchChunk();
                s.Load(model.SourceInfo.FullName, 0, model.Duration);
                chunks.Add(s);
            }
            var final = new AvsConcatList();
            final.Items = chunks;
            var avsContext = new AvsContext();
            final.SerializeToContext(avsContext);


            var serv = new AssemblerService(crossFades);
                var args = @"-i ""{0}"" -q:v 0 ""{1}""";
                var avsScript = avsContext.GetContent();
                File.WriteAllText(model.SourceInfo.FullName + "test", avsScript);
                //var videoFile = Model.Locations.GetOutputFile(episodeNumber);
                //if (videoFile.Exists) videoFile.Delete();

                //args = string.Format(args, avsFile.FullName, videoFile.FullName);
                //RunProcess(args, Model.Locations.FFmpegExecutable.FullName);
                //OnTaskFinished();
        }

        public override void Clean()
        {
            if (Process != null && !Process.HasExited)
                Process.Kill();
            while (filesToDelIfAborted.Count > 0)
                try
                {
                    File.Delete(filesToDelIfAborted[0]);
                    filesToDelIfAborted.RemoveAt(0);
                }
                catch { }
        }
    }
}
