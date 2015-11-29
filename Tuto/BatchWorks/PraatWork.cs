using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using Tuto.Model;

namespace Tuto.BatchWorks
{
    public class PraatWork : BatchWork
    {

        const double samplesLength = 0.25;
        const double silenceTime = 3;
        const int ignoreRate = 5;

        FileInfo wav;
        FileInfo dat;

        public PraatWork(EditorModel model)
        {
            Model = model;
            Name = "Praat working: " + model.Locations.FaceVideo.FullName;
            wav = new FileInfo(Path.Combine(Model.TempFolder.FullName, "voice.wav"));
            dat = new FileInfo(Path.Combine(Model.TempFolder.FullName, "voice.dat"));
        }

        public event EventHandler PraatCreated;

        public override void Work()
        {
            var ffmpegExecutable = Model.Videotheque.Locations.FFmpegExecutable;
            var args = string.Format("-i \"{0}\" -vn -q:a 0 \"{1}\" -y", Model.Locations.FaceVideo, Model.Locations.PraatVoice);
            //RunProcess(args, ffmpegExecutable.ToString());

            args = string.Format(@"-i ""{0}"" ""{1}"" -y",Model.Locations.PraatVoice.FullName,wav);
            //RunProcess(args,ffmpegExecutable.ToString());

            args = string.Format(@"""{0}"" ""{1}""", wav, dat);
            //RunProcess(args, Model.Videotheque.Locations.SoxExecutable.FullName);

            var numberPattern = @"\d\.eE\-\+";
            var regex = new Regex(@"([" + numberPattern + "]+)[ ]+([" + numberPattern + "]+)");
            var result = new List<Tuple<double, double>>();
            double sumAmplitude = 0;
            double startTime = 0;
            var sampleStarts = true;

            var reader = new StreamReader(dat.FullName);
            reader.ReadLine();
            reader.ReadLine();
            while(true)
            {
                for (int i = 0; i < ignoreRate; i++) reader.ReadLine();
                var line = reader.ReadLine();
                if (line == null) break;

                var match = regex.Match(line);
                if (!match.Success) 
                    continue;
                var time=double.Parse(match.Groups[1].Value,CultureInfo.InvariantCulture);
                var amplitute = double.Parse(match.Groups[2].Value,CultureInfo.InvariantCulture);
                if (sampleStarts)
                {
                    startTime = time;
                    sampleStarts = false;
                }
                sumAmplitude += Math.Abs(amplitute);
                if (time-startTime>samplesLength)
                {
                    result.Add(Tuple.Create(startTime, sumAmplitude / (time - startTime)));
                    sampleStarts = true;
                    sumAmplitude = 0;
                }
            }
            reader.Close();

            var silenceLevel = result.Where(z => z.Item1 < silenceTime).Max(z => z.Item2);
            var max = result.Max(z => z.Item2);

            var output = result.Select(z=>new SoundInterval
            {
                StartTime=(int)(z.Item1*1000),
                EndTime=(int)(1000*(z.Item1+samplesLength)),
                HasVoice=z.Item2>silenceLevel,
                Volume=z.Item2/max
            }).ToList();

            Model.Montage.SoundIntervals.Clear();
            foreach (var e in output)
                Model.Montage.SoundIntervals.Add(e);
            //wav.Delete();
            //dat.Delete();
            OnTaskFinished();
        }



        public override void Clean()
        {
            if (Process != null && !Process.HasExited)
                Process.Kill();
            TryToDelete(wav);
            TryToDelete(dat);
        }
    }
}
