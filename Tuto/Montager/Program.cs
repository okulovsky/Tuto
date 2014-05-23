using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Montager
{
    class Program
    {
        static void DeletePattern(string pattern)
        {
            foreach (var e in Directory.GetFiles(".\\", pattern))
                File.Delete(e);

        }

        static StreamWriter OpenMontageBat(string name)
        {
            var batFile = new StreamWriter(name);
            batFile.WriteLine("rmdir /s /q chunks");
            batFile.WriteLine("mkdir chunks");
            batFile.WriteLine("cd chunks");
            return batFile;
        }

        static void CloseMontageBat(StreamWriter wr)
        {
            wr.WriteLine("cd ..");
            wr.Close();

        }

        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Montager.exe <folder>");
                Console.ReadKey();
                return;
            }

            Environment.CurrentDirectory = args[0];
            var log = VideoLib.MontageCommandIO.ReadCommands("log.txt");
            var chunks = Montager.CreateChunks(log, "..\\face-converted.avi", "..\\desktop-converted.avi");


            var lines = chunks.Select(z => z.OutputVideoFile).ToList();
            var folder = new DirectoryInfo(".");

            File.WriteAllLines("ConcatFilesList.txt", lines.Select(z => "file 'chunks\\" + z + "'").ToList());

            using (var xspf = new StreamWriter("list.xspf"))
            {
                xspf.WriteLine(@"<?xml version=""1.0"" encoding=""UTF-8""?>
                <playlist xmlns=""http://xspf.org/ns/0/"" xmlns:vlc=""http://www.videolan.org/vlc/playlist/ns/0/"" version=""1"">
	            <title>Плейлист</title>
	            <trackList>
                ");

                int id = 0;
                foreach (var e in chunks)
                {
                        xspf.WriteLine(@"
		                <track>
			                <location>file:///{0}/chunks/{1}</location>
			                <duration>{2}</duration>
			                <extension application=""http://www.videolan.org/vlc/playlist/0"">
			            	<vlc:id>{3}</vlc:id>
			                </extension>
		                </track>"
                            , folder.FullName.Replace("\\", "/"), e.OutputVideoFile, 0, id++);
                }

                xspf.WriteLine(@"
	                </trackList>
                   
                    </playlist>");
               
            }

            File.WriteAllLines("Recode.bat",
                new string[]
                {
                "ffmpeg -i face.mp4    -vf scale=1280:720 -r 30 -q:v 0 -acodec libmp3lame -ar 44100 -ab 32k face-converted.avi",
                "ffmpeg -i desktop.avi -vf scale=1280:720 -r 30 -qscale 0 -an desktop-converted.avi"
                });



            var batFile = OpenMontageBat("MakeChunksLow.bat");

            var context = new BatchCommandContext
            {
                batFile = batFile,
                lowQuality = true
            };
            foreach (var e in Montager.Processing1(chunks, "result.avi"))
            {
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.WriteLine(e.Caption);
                Console.ForegroundColor = ConsoleColor.Gray;
                e.WriteToBatch(context);
            }

            CloseMontageBat(batFile);


            batFile = OpenMontageBat("MakeChunksHigh.bat");

            context = new BatchCommandContext
            {
                batFile = batFile,
                lowQuality = false
            };
            foreach (var e in Montager.Processing1(chunks, "result.avi"))
            {
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.WriteLine(e.Caption);
                Console.ForegroundColor = ConsoleColor.Gray;
                e.WriteToBatch(context);
            }

            CloseMontageBat(batFile);

        }
    }
}
