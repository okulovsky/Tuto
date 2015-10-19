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
    public class CreateCleanSoundWork : BatchWork
    {

        public CreateCleanSoundWork(FileInfo source, EditorModel model)
        {
            Name = "Make clean sound: " + source;
            Model = model;
            this.source = source;
        }

        private FileInfo source;
        private List<string> filesToDel = new List<string>() { "\\result.wav", "\\noise", "\\input.wav", "\\loud.wav" };

        public override void Work()
        {
            var progPath = Model.Locations.NoiseReductionFolder; //get program's folder for noicereduction utility.
            var ffExe = Model.Locations.FFmpegExecutable;
            var soxExe = Model.Locations.SoxExecutable;
            var printMode = false;
            var temp = Model.Locations.TemporalDirectory;

            Shell.Exec(printMode, ffExe, string.Format(@"-i ""{0}"" -y ""{1}\input.wav""", source.FullName, temp));
            Shell.Exec(printMode, soxExe, string.Format(@"""{0}\input.wav"" ""{0}\loud.wav"" --norm", temp));

            //profile for noise creation
            if (!File.Exists(string.Format("{0}\\noise", temp)))
            {
                Shell.Exec(printMode, ffExe, string.Format(@"-i ""{0}\loud.wav"" -y -ss 0 -t 3 -shortest ""{0}\sample.wav"" -y", temp));
                Shell.Exec(printMode, new FileInfo(Path.Combine(progPath.FullName, "gnp")), string.Format(@"""{0}\sample.wav"" ""{0}\noise""", temp));
                File.Delete(Path.Combine(temp.FullName, "sample.wav"));

            }
            Shell.Exec(printMode, new FileInfo(Path.Combine(progPath.FullName, "nr")), string.Format(@"""{0}\loud.wav"" ""{0}\noise"" ""{0}\result.wav""", temp));
            Shell.Exec(printMode, ffExe, string.Format(@"-i ""{0}\result.wav"" -ar 44100 -ac 2 -ab 192k -f mp3 -qscale 0 ""{1}\cleaned-tmp.mp3"" -y", temp.FullName, Model.Locations.FaceVideo.Directory.FullName));
            File.Delete(Path.Combine(temp.FullName, "result.wav"));
            File.Delete(Path.Combine(temp.FullName, "loud.wav"));
            File.Delete(Path.Combine(temp.FullName, "input.wav"));

            Thread.Sleep(500);
            var file = Model.Locations.ClearedSound;
            var mp3Path = Path.GetFileNameWithoutExtension(source.Name) + ".mp3";
            var output = Path.Combine(file.Directory.FullName, mp3Path);
            if (File.Exists(file.FullName))
                File.Delete(file.FullName);
            Thread.Sleep(200);
            //File.Move(GetTempFile(file).FullName, output);

            var tempAudioFile = GetTempFile(source); 
            var args = string.Format(
                @"-i ""{0}"" -i ""{1}"" -map 0:0 -map 1 -vcodec copy -acodec copy ""{2}"" -y",
                source.FullName,
                Path.Combine(Model.Locations.FaceVideo.Directory.FullName,mp3Path),
                tempAudioFile.FullName
                );
            Shell.Exec(printMode, ffExe, args);
            Thread.Sleep(200);
            File.Delete(tempAudioFile.FullName);
            OnTaskFinished();
            
        }


        public override bool Finished()
        {
            return Model.Locations.ClearedSound.Exists;
        }

        public override void Clean()
        {
            foreach (var temp in filesToDel)
                if (File.Exists(Model.Locations.TemporalDirectory.FullName + temp))
                {
                    TryToDelete(Model.Locations.TemporalDirectory.FullName + temp);
                }
        }
    }
}
