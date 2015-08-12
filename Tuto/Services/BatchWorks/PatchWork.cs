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
        private bool crossFades { get; set; }
        private PatchModel pmodel;
        private AvsNode result;

        private string oldName;
        private string newName;

        public PatchWork(PatchModel model, bool fadeMode, EditorModel emodel)
        {
            this.pmodel = model;
            this.Model = emodel;
            Name = "Patching: " + model.SourceInfo.Name;
            if (!Model.Locations.PatchesDirectory.Exists)
                Model.Locations.PatchesDirectory.Create();

            foreach (var ep in model.MediaTracks)
            {
                var name = Path.Combine(Model.Locations.TemporalDirectory.FullName, ep.ConvertedName);
                if (!File.Exists(name))
                {
                    var fileInPatches = new FileInfo(Path.Combine(emodel.Locations.PatchesDirectory.FullName, new FileInfo(ep.Path.LocalPath).Name));
                    BeforeWorks.Add(new PreparePatchWork(emodel, fileInPatches, new FileInfo(name)));
                }
            }
        }

        public override void Work()
        {
            var src = pmodel.SourceInfo;
            List<AvsNode> chunks = new List<AvsNode>();
            var tracks = pmodel.MediaTracks;
            oldName = pmodel.SourceInfo.FullName;
            newName = Path.Combine(pmodel.SourceInfo.Directory.FullName, Guid.NewGuid().ToString() + ".avi");
            File.Move(oldName, newName);
            double previous = 0;
            int index = 0;
            string mode = "main";
            while (Math.Abs(previous - pmodel.Duration) >= 0.5 && tracks.Count != 0)
            {
                var avs = new AvsPatchChunk();
                if (mode == "main")
                {
                    var endTime = index >= tracks.Count ? pmodel.Duration : tracks[index].StartSecond + tracks[index].LeftShiftInSeconds / pmodel.Scale;
                    avs.Load(newName, previous, endTime);
                    previous = endTime;
                    chunks.Add(avs);
                    mode = "patch";
                    continue;
                }
                var name = Path.Combine(Model.Locations.TemporalDirectory.FullName, tracks[index].ConvertedName);
                avs.Load(name, tracks[index].StartSecond, tracks[index].EndSecond);
                chunks.Add(avs);
                previous = tracks[index].EndSecond + tracks[index].LeftShiftInSeconds / pmodel.Scale;
                index++;
                mode = "main";
            }

            if (tracks.Count == 0)
            {
                var s = new AvsPatchChunk();
                s.Load(newName, 0, pmodel.Duration);
                chunks.Add(s);
            }

            var final = new AvsConcatList();
            final.Items = chunks;
            var avsContext = new AvsContext();

            AvsNode payload = final;
            var currentSub = new AvsSub();
            foreach (var sub in pmodel.Subtitles)
            {
                currentSub = new AvsSub();
                currentSub.Payload = payload;
                currentSub.X = (int)sub.X;
                currentSub.Y = (int)sub.Y;
                currentSub.Content = sub.Content;
                
                payload = currentSub;
            }
            currentSub.SerializeToContext(avsContext);
            var serv = new AssemblerService(crossFades);
            var args = @"-i ""{0}"" -q:v 0 -vf ""scale=1280:720, fps=25"" -q:v 0 -acodec libmp3lame -ar 44100 -ab 32k ""{1}"" -y";



            var avsScript = string.Format(@"import(""{0}"")", Model.Locations.AvsLibrary.FullName) + "\r\n" + avsContext.GetContent() + "var_0";
            File.WriteAllText(newName + "test.avs", avsScript);

            var dir = Model.Locations.OutputDirectory.FullName;
            var patchedName = GetTempFile(pmodel.SourceInfo, "-patched").FullName;
            var path = Path.Combine(dir, patchedName);
            args = string.Format(args, newName + "test.avs", path);
            RunProcess(args, Model.Locations.FFmpegExecutable.FullName);
            File.Delete(newName + "test.avs");
            OnTaskFinished();
            File.Move(newName, oldName);
        }

        public override void Clean()
        {
            if (Process != null && !Process.HasExited)
                Process.Kill();
            while (!File.Exists(oldName))
                try
                {
                    File.Move(newName, oldName);
                }
                catch { }
        }
    }
}
