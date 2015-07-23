using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Tuto.Model;
using System.Windows.Controls;
using System.Text.RegularExpressions;

namespace Tuto.BatchWorks
{
    public class CreateCleanSoundWork : BatchWork
    {

        public CreateCleanSoundWork(FileInfo source, EditorModel model)
        {
            Name = "Make clean sound: " + source;
            this.model = model;
            this.source= source;
        }

        private string temp = "not_found";
        private EditorModel model;
        private FileInfo source;

        private FileInfo assemblyDirectory
        {
            get
            {
                string codeBase = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return new FileInfo(Path.GetDirectoryName(path));
            }
        }

        public override void Work()
        {
            if (source.Exists)
            {
                var progPath = model.Locations.NoiseReductionFolder; //get program's folder for noicereduction utility.
                var ffExe = model.Locations.FFmpegExecutable.FullName;
                var soxExe = model.Locations.SoxExecutable.FullName;
                List<string> commands = new List<string>();

                var loc = model.Locations.FaceVideo;
                var temp = model.Locations.TemporalDirectory;

                commands.Add(string.Format(@"""{2}"" -i ""{0}"" -y -shortest ""{1}\input.wav""", loc, temp, ffExe));
                commands.Add(string.Format(@"""{1}"" ""{0}\input.wav"" ""{0}\loud.wav"" --norm", temp, soxExe));

                //profile for noise creation
                if (!File.Exists(string.Format("{0}\\noise", temp)))
                {
                    commands.Add(string.Format(@"""{1}"" -i ""{0}\loud.wav"" -y -ss 0 -t 3 -shortest ""{0}\sample.wav"" -y", temp, ffExe));
                    commands.Add(string.Format(@"""{0}\gnp"" ""{1}\sample.wav"" ""{1}\noise""", progPath, temp));
                    commands.Add(string.Format(@"del ""{0}\sample.wav""", temp));

                }

                commands.Add(string.Format(@"""{0}\nr"" ""{1}\loud.wav"" ""{1}\noise"" ""{1}\result.wav""", progPath, temp));
                commands.Add(string.Format(@"""{1}"" -i ""{0}\result.wav"" -qscale 0 -shortest -acodec libmp3lame ""{0}\cleaned.mp3"" -y", temp, ffExe)); 
                commands.Add(string.Format(@"del ""{0}\result.wav""", temp));
                commands.Add(string.Format(@"del ""{0}\loud.wav""", temp));
                commands.Add(string.Format(@"del ""{0}\input.wav""", temp));
                File.WriteAllLines(string.Format(@"{0}\clean.bat", temp), commands.ToArray());
                Args = string.Format(@"/c ""{0}\clean.bat""", temp);
                FullPath = "CMD.exe";
                RunProcess();
            }
        }

        public override void Clean()
        {
            if (Process != null && !Process.HasExited)
                Process.Kill();
            if (File.Exists(temp))
            {
                while (File.Exists(temp))
                    try
                    {
                        File.Delete(temp);
                    }
                    catch { }
            }
        }
    }
}
