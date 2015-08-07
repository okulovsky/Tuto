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
        private PatchModel model;
        private AvsNode result;

        private string oldName;
        private string newName;

        public PatchWork(PatchModel model, bool fadeMode, EditorModel emodel)
        {
            this.model = model;
            this.Model = emodel;
            Name = "Patching: " + model.SourceInfo.Name;
            if (!Model.Locations.PatchesDirectory.Exists)
                Model.Locations.PatchesDirectory.Create();

            foreach (var ep in model.MediaTracks)
            {
                var name = Path.Combine(Model.Locations.PatchesDirectory.FullName, ep.ConvertedName);
                if (!File.Exists(name))
                    BeforeWorks.Add(new PreparePatchWork(emodel, new FileInfo(ep.Path.LocalPath), new FileInfo(name)));
            }
        }

        public override void Work()
        {
            var src = model.SourceInfo;
            List<AvsNode> chunks = new List<AvsNode>();
            var tracks = model.MediaTracks;
            oldName = model.SourceInfo.FullName;
            newName = Path.Combine(model.SourceInfo.Directory.FullName, Guid.NewGuid().ToString() + ".avi");
            File.Move(oldName, newName);
            double previous = 0;
            int index = 0;
            string mode = "main";
            while (Math.Abs(previous - model.Duration) >= 0.5 && tracks.Count != 0)
            {
                var avs = new AvsPatchChunk();
                if (mode == "main")
                {
                    var endTime = index >= tracks.Count ? model.Duration : tracks[index].StartSecond + tracks[index].LeftShift;
                    avs.Load(newName, previous, endTime);
                    previous = endTime;
                    chunks.Add(avs);
                    mode = "patch";
                    continue;
                }
                var name = Path.Combine(Model.Locations.PatchesDirectory.FullName, tracks[index].ConvertedName);
                avs.Load(name, tracks[index].StartSecond, tracks[index].EndSecond);
                chunks.Add(avs);
                previous = tracks[index].EndSecond + tracks[index].LeftShift;
                index++;
                mode = "main";
            }

            if (tracks.Count == 0)
            {
                var s = new AvsPatchChunk();
                s.Load(newName, 0, model.Duration);
                chunks.Add(s);
            }

            var final = new AvsConcatList();
            final.Items = chunks;
            var avsContext = new AvsContext();
            final.SerializeToContext(avsContext);


            var serv = new AssemblerService(crossFades);
            var args = @"-i ""{0}"" -q:v 0 -vf ""scale=1280:720, fps=25"" -q:v 0 -acodec libmp3lame -ar 44100 -ab 32k ""{1}"" -y";
            var avsScript = string.Format(@"import(""{0}"")", Model.Locations.AvsLibrary.FullName) + "\r\n" + avsContext.GetContent() + "var_0";
            File.WriteAllText(newName + "test.avs", avsScript);
            //var videoFile = Model.Locations.GetOutputFile(0);
            //if (videoFile.Exists) videoFile.Delete();

            var dir = Model.Locations.OutputDirectory.FullName;
            var patchedName = GetTempFile(model.SourceInfo, "-patched").FullName;
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
