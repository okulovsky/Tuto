using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace Montager
{

    public abstract class FFMPEGCommand : BatchCommand
    {
        public void WriteFFMPEGCommand(BatchCommandContext context, string artuments)
        {
            context.batFile.WriteLine("ffmpeg " + artuments);
            
            /*Console.WriteLine("FFMPEG " + artuments);
            Console.WriteLine();
            var process = new Process();
            process.StartInfo.FileName = context.FFMPEGPath;
            process.StartInfo.Arguments = artuments;
            process.StartInfo.UseShellExecute = false;
            process.Start();
            process.WaitForExit();
            if (process.ExitCode != 0)
            {
                Console.BackgroundColor = ConsoleColor.Red;
                Console.ForegroundColor = ConsoleColor.Black;
                Console.WriteLine("ERROR");
                Console.ReadKey();
            }
            */
        }

        public string MS(int milliseconds)
        {
            var dr=(milliseconds % 1000).ToString();
            while (dr.Length<3) dr="0"+dr;

            return (milliseconds / 1000).ToString() + "." + dr;
        }
    }


    public class ExtractAudioCommand : FFMPEGCommand
    {
        public string VideoInput;
        public string AudioOutput;
        public int StartTime;
        public int Duration;

        public override string Caption
        {
            get { return string.Format("Извлечение аудио из {0} в {1} ({2}-{3})", VideoInput, AudioOutput, StartTime, StartTime + Duration); }
        }

        public override void WriteToBatch(BatchCommandContext context)
        {
            if (context.lowQuality)
            {
                WriteFFMPEGCommand(context,
                    string.Format("-i {0} -ss {1} -t {2} -acodec copy -vn {3}",
                        VideoInput,
                        MS(StartTime),
                        MS(Duration),
                        AudioOutput));
            }
            else
            {
                WriteFFMPEGCommand(context,
                    string.Format("-i {0} -ss {1} -t {2} -vn -qscale 0 {3}",
                        VideoInput,
                        MS(StartTime),
                        MS(Duration),
                        AudioOutput));
            }
        }
    }

    public class ExtractVideoCommand : FFMPEGCommand
    {
        public string VideoInput;
        public string VideoOutput;
        public int StartTime;
        public int Duration;

        public override void WriteToBatch(BatchCommandContext context)
        {
            if (context.lowQuality)
            {
                WriteFFMPEGCommand(context,
                    string.Format("-i {0} -ss {1} -t {2} -acodec copy -vcodec copy {3}",
                        VideoInput,
                        MS(StartTime),
                        MS(Duration),
                        VideoOutput));
            }
            else
            {

                WriteFFMPEGCommand(context,
                string.Format("-i {0} -ss {1} -t {2} -qscale 0 {3}",
                    VideoInput,
                    MS(StartTime),
                    MS(Duration),
                    VideoOutput));
            }
        }

        public override string Caption
        {
            get { return string.Format("Копирование видео из {0} в {1} ({2}-{3})", VideoInput, VideoOutput, StartTime, StartTime + Duration); }
        }
    }


    public class ExtractFaceVideoCommand : ExtractVideoCommand
    {
    }

    public class ExtractScreenVideoCommand : ExtractVideoCommand
    {
        
    }


    public class MixVideoAudioCommand : FFMPEGCommand
    {
        public string AudioInput;
        public string VideoInput;
        public string VideoOutput;


        public override string Caption
        {
            get { return string.Format("Микширование видео из {0} и аудио из {1} в {2}", VideoInput, AudioInput, VideoOutput); }
        }

        public override void WriteToBatch(BatchCommandContext context)
        {
            WriteFFMPEGCommand(context,
                string.Format("-i {1} -i {0} -acodec copy -vcodec copy {2}",
                    VideoInput,
                    AudioInput,
                    VideoOutput));

        }

    }

    public class ConcatCommand : FFMPEGCommand
    {
        public bool AudioOnly = false;
        public List<string> Files = new List<string>();
        public string Result;

        public override string Caption
        {
            get { return "Объединение файлов"; }
        }

        public override void WriteToBatch(BatchCommandContext context)
        {
            var temp="ConcatFilesList.txt";
            File.WriteAllText(temp, Files.Select(z => "file '" + z + "'").Aggregate((a, b) => a + "\r\n" + b));
            var args = "-f concat -i ConcatFilesList.txt ";
            if (AudioOnly)
                args += " -acodec copy ";
            else
                args += " -c copy ";
            args+=Result;
            WriteFFMPEGCommand(context, args);
            //File.Delete(temp);
        }
    }




}
