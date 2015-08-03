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
            var ffExe = Model.Locations.FFmpegExecutable.FullName;
            var soxExe = Model.Locations.SoxExecutable.FullName;
            List<string> commands = new List<string>();

            var loc = Model.Locations.FaceVideo;
            var temp = Model.Locations.TemporalDirectory;

            commands.Add(string.Format(@"""{2}"" -i ""{0}"" -y ""{1}\input.wav""", loc, temp, ffExe));
            commands.Add(string.Format(@"""{1}"" ""{0}\input.wav"" ""{0}\loud.wav"" --norm", temp, soxExe));

            //profile for noise creation
            if (!File.Exists(string.Format("{0}\\noise", temp)))
            {
                commands.Add(string.Format(@"""{1}"" -i ""{0}\loud.wav"" -y -ss 0 -t 3 -shortest ""{0}\sample.wav"" -y", temp, ffExe));
                commands.Add(string.Format(@"""{0}\gnp"" ""{1}\sample.wav"" ""{1}\noise""", progPath, temp));
                commands.Add(string.Format(@"del ""{0}\sample.wav""", temp));

            }
            commands.Add(string.Format(@"""{0}\nr"" ""{1}\loud.wav"" ""{1}\noise"" ""{1}\result.wav""", progPath, temp));
            commands.Add(string.Format(@"""{2}"" -i ""{0}\result.wav"" -ar 44100 -ac 2 -ab 192k -f mp3 -qscale 0 ""{1}\cleaned-tmp.mp3"" -y",temp.FullName, Model.Locations.FaceVideo.Directory.FullName, ffExe));
            commands.Add(string.Format(@"del ""{0}\result.wav""", temp));
            commands.Add(string.Format(@"del ""{0}\loud.wav""", temp));
            commands.Add(string.Format(@"del ""{0}\input.wav""", temp));
            File.WriteAllLines(string.Format(@"{0}\clean.bat", temp), commands.ToArray());
            var args = string.Format(@"/c ""{0}\clean.bat""", temp);
            var fullPath = "CMD.exe";
            RunProcess(args, fullPath);
            Thread.Sleep(500);
            var file = Model.Locations.ClearedSound;
            if (File.Exists(file.FullName))
                File.Delete(file.FullName);
            Thread.Sleep(200);
            File.Move(GetTempFile(file).FullName, file.FullName);
            OnTaskFinished();
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
                    while (File.Exists(Model.Locations.TemporalDirectory.FullName + temp))
                        try
                        {
                            File.Delete(Model.Locations.TemporalDirectory.FullName + temp);
                        }
                        catch { }
                }
        }
    }
}
