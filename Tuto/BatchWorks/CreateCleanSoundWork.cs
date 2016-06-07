using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Tuto.Model;
using System.Windows.Controls;
using System.Text.RegularExpressions;
using System.Threading;

namespace Tuto.BatchWorks
{
    public class CreateCleanSoundWork : ProcessBatchWork
    {

        public CreateCleanSoundWork(FileInfo source, EditorModel model, bool forced)
        {
            Name = "Cleaning sound sound: " + model.RawLocation.Name;
            Model = model;
            this.source = source;
            this.Forced = forced;
        }

        private FileInfo source;
        private List<string> filesToDel = new List<string>() { "\\result.wav", "\\input.wav", "\\temp.wav", "\\sample.wav" };

        public override void Work()
        {
            var progPath = Model.Videotheque.Locations.GNP.Directory; //get program's folder for noicereduction utility.
            var ffExe = Model.Videotheque.Locations.FFmpegExecutable;
            var soxExe = Model.Videotheque.Locations.SoxExecutable;
            var loc = Model.Locations.FaceVideo;
            var temp = Model.Locations.TemporalDirectory;
            Progress = 0;
            RunProcess(string.Format(@"-i ""{0}"" -y ""{1}\input.wav""", loc, temp), ffExe.FullName);
            Progress = 10;
            RunProcess(string.Format(@"""{0}\input.wav"" ""{0}\temp.wav"" --norm", temp), soxExe.FullName);
            Progress = 20;
            //profile for noise creation
            if (!File.Exists(string.Format("{0}\\noise", temp)))
            {
                RunProcess(string.Format(@"-i ""{0}\temp.wav"" -y -ss 0 -t 3 -shortest ""{0}\sample.wav"" -y", temp), ffExe.FullName);
                RunProcess(string.Format(@"""{0}\sample.wav"" ""{0}\noise""", temp), new FileInfo(Path.Combine(progPath.FullName, "gnp")).FullName);
                File.Delete(Path.Combine(temp.FullName, "sample.wav"));

            }
            Progress = 30;
            RunProcess(string.Format(@"""{0}\temp.wav"" ""{0}\noise"" ""{0}\result.wav""", temp), Path.Combine(progPath.FullName, "nr"));
            Progress = 80;
            RunProcess(string.Format(@"-i ""{0}\result.wav"" -ar 44100 -ac 2 -ab 192k -f mp3 -qscale 0 ""{1}\cleaned-tmp.mp3"" -y", temp.FullName, Model.Locations.FaceVideo.Directory.FullName), ffExe.FullName);
            Thread.Sleep(500);
            var file = Model.Locations.ClearedSound;
            if (File.Exists(file.FullName))
                File.Delete(file.FullName);
            Thread.Sleep(200);
            File.Move(GetTempFile(file).FullName, file.FullName);
            DeleteTemps(temp);
            Progress = 100;
            OnTaskFinished();

        }

        private void DeleteTemps(DirectoryInfo temp)
        {
            File.Delete(Path.Combine(temp.FullName, "result.wav"));
            File.Delete(Path.Combine(temp.FullName, "input.wav"));
            File.Delete(Path.Combine(temp.FullName, "temp.wav"));
        }

        public override bool Finished()
        {
            return Model.Locations.ClearedSound.Exists;
        }

        public override void Clean()
        {
            if (Process != null && !Process.HasExited)
                Process.Kill();
            foreach (var temp in filesToDel)
                if (File.Exists(Model.Locations.TemporalDirectory.FullName + temp))
                {
                    TryToDelete(Model.Locations.TemporalDirectory.FullName + temp);
                }
        }
    }
}