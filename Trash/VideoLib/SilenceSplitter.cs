//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Globalization;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace VideoLib
//{
//    public static class SilenceSplitter
//    {
//        const string PraatExecutable = "praatcon.exe";
//        public const string Script = "split_pauses.praat";
//        public const string TextGridFilename = "output.TextGrid";

//        const string SilentLabel = "--";
//        const string SoundLabel = "++";

//        const double MinPitch = 100;
//        const double TimeStep = 0;

//        const double SilenceThreshold = -27;
//        const double MinSilentInterval = 0.5;
//        const double MinSoundInterval = 0.1;

//        /*
//                #		filename = "in.mp3"  ; sound to analyze
//                #		output_filename = "out.textgrid"  ; file to write results into
//                #		silent_label = "--"  ; marks inside the file
//                #		sound_label = "++"
//                # parameters for intensity analysis
//                #		min_pitch = 100  ; (Hz)
//                #		time_step = 0  ; (sec) 0 is auto
//                # parameters for silent intervals detection
//                #		silence_threshold = -27  ; (dB)
//                #		min_silent_interval = 0.5  ; (sec)
//                #		min_sound_interval = 0.1  ; (sec)
//             * */

//        public static List<Interval> GetIntervalsFromAudioFile(string audioFilename)
//        {
//            /*
//             * видеоролик => [(вреняНачала, времяКонца, естьГолос), ...]
//             * нужно это один раз сделать для всего видеоролика и сохранить результат
//             */


//            // call praat script
//            CallShellCommand("", PraatExecutable, GetCommandLine(Script, audioFilename, TextGridFilename));

//            return GetIntervals(TextGridFilename);
//        }

//        public static List<Interval> GetIntervals(string textGridFilename)
//        {
//            // parse file
//            var intervals = new List<Interval>();
//            using (var reader = new StreamReader(textGridFilename))
//            {

//                for (var i = 0; i < 11; i++)
//                    reader.ReadLine();

//                var intervalCount = int.Parse(reader.ReadLine());
//                for (int i = 0; i < intervalCount; i++)
//                {
//                    var startTime = double.Parse(reader.ReadLine(), CultureInfo.InvariantCulture);
//                    var endTime = double.Parse(reader.ReadLine(), CultureInfo.InvariantCulture);
//                    var hasVoice = reader.ReadLine() == '"' + SoundLabel + '"';
//                    intervals.Add(new Interval(startTime, endTime, hasVoice));
//                }
//            }
//            return intervals;
//        }

//        private static string GetCommandLine(string script, string audio, string output)
//        {
//            // test.praat in.mp3 out.textgrid -- ++ 100 0 -27 0.5 0.1 
//            return String.Format(CultureInfo.InvariantCulture,"{0} {1} {2} {3} {4} {5} {6} {7} {8} {9}", script, audio, output,
//                SilentLabel, SoundLabel, MinPitch, TimeStep, SilenceThreshold, MinSilentInterval, MinSoundInterval);
//        }

//    }

//}
