using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using VideoLib;
using Montager;

namespace Assembler
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Assembler <dir>");
                return;
            }

	 

            Directory.SetCurrentDirectory(args[0]);  // to avoid ugly arg[0]+"\\blahblah"
            var titles = File.ReadAllLines(TitlesFileName);
	        var subtitles = File.ReadAllLines(SubtitlesFileName).ToList();

			int title = int.Parse(args[0]);  // assume <dir> has arbitrary name, not integer 

            XDocument doc = XDocument.Load("list.xspf");

            var tracks = doc
                        .Elements()
                        .Where(z => z.Name.LocalName == "playlist")
                        .Elements()
                        .Where(z => z.Name.LocalName == "trackList")
                        .Elements()
                        .Select(z => z.Elements().Where(x => x.Name.LocalName == "location").FirstOrDefault())
                        .Select(z => z.Value)
                        .Select(z => z.Substring(8, z.Length - 8))
                        .Select(z => z.Replace("/", "\\"))
                        .Select(z=> new FileInfo(z).Name)
                        .ToList();

            var log = MontageCommandIO.ReadCommands("log.txt");

            var parts = CreateParts(tracks, log, title);


			var batFile = new StreamWriter("Assembly.bat", false, Encoding.GetEncoding(866));
            batFile.WriteLine("del processing\\processed*");
            batFile.WriteLine("del result*");

            var context = new BatchCommandContext
            {
                batFile = batFile,
                lowQuality = false
            };

            foreach (var part in parts.Parts)
            {
                part.WritePartToBatch(context);
            }

            batFile.Close();

        }
        public static PartsList CreateParts(List<string> tracks, MontageLog log, int title)
        {
            // chunk numbers to split after
            var breakChunkNumbers = log.Commands
                .Where(z => z.Action == MontageAction.CommitAndSplit)
                .Select(z => z.Id)
                .ToList();

            var chunks = Montager.Montager.CreateChunks(log, "", "");
            var isFace = new Dictionary<int, bool>
	            {
                {0, true}  // starts with 'face'
            };
            foreach (var chunk in chunks.Where(chunk => !isFace.Keys.Contains(chunk.Id)))
	            isFace.Add(chunk.Id, chunk.IsFaceChunk);

            var parts = new PartsList(breakChunkNumbers);
            parts.MakeParts(tracks, isFace, title);

            return parts;
        }

		public const string TitlesFileName = "titles.txt";
	    public const string SubtitlesFileName = "titles.txt";  // inside working dir
    }
}
