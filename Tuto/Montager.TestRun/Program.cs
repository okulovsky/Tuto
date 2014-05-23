using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Montager.TestRun
{
    class Program
    {
        static void Main(string[] args)
        {
            Environment.CurrentDirectory = "..\\..\\..\\..\\Video\\TestFiles\\";
            try
            {
                Directory.CreateDirectory("Work");
            }
            catch { }
            Environment.CurrentDirectory += "\\Work\\";


            var chunks=new List<Chunk>
            {
                new Chunk 
                { 
                    Id = 1, 
                    VideoSource = new ChunkSource { File = "..\\Original\\123-en.mp4", StartTime = 0, Duration = 1000 }
                },
                new Chunk 
                { 
                    Id = 2, 
                    VideoSource = new ChunkSource { File = "..\\Original\\123-en.mp4", StartTime = 1000, Duration = 1000 },
                    AudioSource = new ChunkSource { File = "..\\Original\\123-fr.mp4", StartTime = 1000, Duration = 1000 }
                },
                new Chunk 
                { 
                    Id = 3, 
                    VideoSource = new ChunkSource { File = "..\\Original\\123-en.mp4", StartTime = 2000, Duration = 1000 },
                    AudioSource = new ChunkSource { File = "..\\Original\\123-de.mp4", StartTime = 2000, Duration = 1000 }
                }
            };

            foreach (var e in Directory.GetFiles(".\\","*.mp*"))
                File.Delete(e);


            var context = new BatchCommandContext
            {
                path = "C:\\ffmpeg\\bin\\ffmpeg.exe"
            };

            foreach (var e in Montager.Processing2(chunks,"output.mp4"))
            {
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.WriteLine(e.Caption);
                Console.ForegroundColor = ConsoleColor.Gray;
                e.WriteToBatch(context);
            }

        }
    }
}
