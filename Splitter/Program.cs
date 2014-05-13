using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoLib;

namespace Splitter
{
    class Program
    {
        static void Main(string[] args)
        {
            //if (args.Length < 1)
            //{
            //    Console.WriteLine("Splitter.exe <folder>");
            //    Console.ReadKey();
            //    return;
            //}
            ///*
            // 1. extract audio
            // 2. execute praat script
            // 3. parse result
            // 4. save it (where? montage.editor maybe?)
            // 5. (not here) draw data
            // */

            //var inputFilename = "face.mp4";
            //var audioFilename = "voice.mp3";

            //Environment.CurrentDirectory = args[0];
            //var extractAudioCommand = String.Format("-i {0} -vn -acodec copy {1}",
            //    inputFilename,
            //    audioFilename);

            //SilenceSplitter.CallShellCommand(Environment.SystemDirectory, "cmd.exe", string.Format("/c del {0}", audioFilename));
            //SilenceSplitter.CallShellCommand(Environment.SystemDirectory, "cmd.exe", string.Format("/c copy ..\\{0} .", SilenceSplitter.Script));
            //SilenceSplitter.CallShellCommand("", "ffmpeg", extractAudioCommand);

            //var intervals = SilenceSplitter.GetIntervalsFromAudioFile(audioFilename);
            //foreach (var i in intervals)
            //    Console.WriteLine(i.ToString());


            //// //var chunks = Montager.CreateChunks(log, "..\\face-converted.avi", "..\\desktop-converted.avi");

        }

        
    }
}
