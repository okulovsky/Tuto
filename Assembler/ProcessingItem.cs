using Montager;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoLib;

namespace Assembler
{
    public class ProcessingItem : BatchCommand
    {
        private string source;

        public string SourceFilename
        {
            get { return String.Format("{0}\\{1}", chunksDir, source); }
            set { source = value; }
        }

        public readonly List<AviSynthCommand> Transformations = new List<AviSynthCommand>();

        public string ResultFilename
        {
            get
            {
                return Transformations.Count == 0
                  ? SourceFilename
                  : String.Format("{0}\\{1}{2}", processingDir, namePrefix, source);
            }
        }

        public string AvsFilename
        {
            get
            {
                return Transformations.Count == 0
                    ? null
                    : String.Format("{0}\\{1}.avs", processingDir, source);  // TODO: cut ".avi"?
            }
        }

        public override void WriteToBatch(BatchCommandContext context)  // ffmpeg execution string for .BAT file
        {
            // open script 'AvsFilename' and produce video 'Filename'
            // take care of high and low profiles

	        if (String.IsNullOrEmpty(AvsFilename)) return;
	        Directory.CreateDirectory(processingDir);
	        var avsContext = new BatchCommandContext
		        {
			        batFile = new StreamWriter(AvsFilename, false, Encoding.GetEncoding(1251)),
			        path = pathToBase,  // NOTE: not FFMPEG, actually
			        lowQuality = context.lowQuality
		        };
	        WriteAvsScript(avsContext);
	        avsContext.batFile.Close();

	        // AviSynth outputs raw (?) video, so we need to recode it to match other clips' properties
	        // NOTE: no need to deal with HIGH/LOW profiles for now.
	        context.batFile.WriteLine("ffmpeg -i {0} -vf scale=1280:720 -r 30 -q:v 0 -acodec libmp3lame -ar 44100 -ab 32k {1}", AvsFilename, ResultFilename);
        }

        private void WriteAvsScript(BatchCommandContext context)  // transformations to write to .AVS file
        {
            // chain transformations inside .avs script
            // return video
            // NOTE: do not rely on SourceFilename, use data supplied in AviSynthCommands!

            context.batFile.WriteLine("import(\"{0}\")", AviSynthCommand.LibraryPath);
            foreach (var t in Transformations)
                t.WriteToAvs(context);
            
        }

        public override string Caption
        {
            get
            {
                return String.Format("{0} -> {1}{{{3}}} -> {2}",
                    SourceFilename,
                    AvsFilename,
                    ResultFilename,
                    String.Join(" => ", Transformations.Select(t => t.Caption)));
            }
        }

        private static string chunksDir = "chunks";
        private static string processingDir = "processing";
        private static string namePrefix = "processed_";
        private static string pathToBase = "..";
    }
}
