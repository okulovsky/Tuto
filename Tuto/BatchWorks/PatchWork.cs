using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Tuto.Model;
using Tuto.TutoServices;
using Tuto.TutoServices.Assembler;
using System.Threading;

namespace Tuto.BatchWorks
{
    public class PatchWork : BatchWork
    {

        private List<string> filesToDelIfAborted { get; set; }
        private bool crossFades { get; set; }
        private PatchModel pmodel;
     
        private string oldName;
        private string newName;

        public PatchWork(PatchModel model, bool fadeMode, EditorModel emodel, bool forced)
        {
            this.pmodel = model;
            this.Model = emodel;
            Forced = forced;
            Name = "Patching: " + model.SourceInfo.FullName;
            foreach (var ep in model.MediaTracks)
            {
                if (!ep.IsTutoPatch)
                {
                    var patchInfo = ep.FullName;
                    var name = Path.Combine(Model.Locations.TemporalDirectory.FullName, patchInfo.Name);
                    var fileInPatches = new FileInfo(ep.Path.LocalPath);
                    BeforeWorks.Add(new PreparePatchWork(emodel, fileInPatches, new FileInfo(name), false));
                }
                else if (!ep.FullName.Exists)
                {
                    
                    var m = emodel.Videotheque.EditorModels.First(x => x.Montage.RawVideoHash == ep.ModelHash);   
                    
                    var epInfo = m.Montage.Information.Episodes[ep.EpisodeNumber];
                    BeforeWorks.Add(new AssemblyEpisodeWork(m, epInfo));
                }
            }
        }

        public override void Work()
        {
            var src = pmodel.SourceInfo;
            List<AvsNode> chunks = new List<AvsNode>();
            var tracks = pmodel.MediaTracks.OrderBy(x => x.LeftShiftInSeconds).ToList();
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
                    var endTime = index >= tracks.Count ? pmodel.Duration : tracks[index].StartSecond + tracks[index].LeftShiftInSeconds;
                    avs.Load(newName, previous, endTime);
                    previous = endTime;
                    chunks.Add(avs);
                    mode = "patch";
                    continue;
                }
                var name = tracks[index].IsTutoPatch ? tracks[index].Path.LocalPath : Path.Combine(Model.Locations.TemporalDirectory.FullName, tracks[index].FullName.Name);
                avs.Load(name, tracks[index].StartSecond, tracks[index].EndSecond);
                chunks.Add(avs);
                previous = tracks[index].EndSecond + tracks[index].LeftShiftInSeconds;
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
            if (pmodel.Subtitles.Count > 0)
            {
                var currentSub = new AvsSub();
                foreach (var sub in pmodel.Subtitles)
                {
                    currentSub = new AvsSub();
                    currentSub.Payload = payload;
                    currentSub.X = (int)(sub.Pos.X * pmodel.Width / pmodel.ActualWidth);
                    currentSub.Y = (int)(sub.Pos.Y * pmodel.Height / pmodel.ActualHeight + sub.HeightShift);
                    currentSub.Start = sub.LeftShiftInSeconds;
                    currentSub.End = sub.LeftShiftInSeconds + sub.EndSecond - sub.StartSecond;
                    currentSub.Content = sub.Content;
                    currentSub.FontSize = (sub.FontSize * pmodel.FontCoefficent).ToString();
                    currentSub.Stroke = sub.Stroke;
                    currentSub.Foreground = sub.Foreground;
                    payload = currentSub;
                }
                currentSub.SerializeToContext(avsContext);
            }
            else final.SerializeToContext(avsContext);

            var args = @"-i ""{0}"" -q:v 0 -vf ""scale=1280:720, fps=25"" -q:v 0 -acodec libmp3lame -ar 44100 -ab 32k ""{1}"" -y";

            var avsScript = string.Format(@"import(""{0}"")", Model.Locations.AvsLibrary.FullName) + "\r\n" + avsContext.GetContent() + "var_0";
            File.WriteAllText(newName + "test.avs", avsScript, Encoding.GetEncoding("Windows-1251"));

            //Патчер в аутпут все делает
            var scriptFile = newName + "test.avs";
            var path = Model.Locations.GetFinalOutputFile(pmodel.EpisodeNumber).FullName;
            args = string.Format(args, scriptFile , path);
            RunProcess(args, Model.Videotheque.Locations.FFmpegExecutable.FullName);
            File.Delete(scriptFile);
            OnTaskFinished();
            File.Move(newName, oldName);
        }

        public override void Clean()
        {
            FinishProcess();
            var tries = 0;
            while (!File.Exists(oldName) && tries < 5)
                try
                {
                    tries++;
                    File.Move(newName, oldName);
                    Thread.Sleep(200);
                }
                catch { }
        }
    }
}
