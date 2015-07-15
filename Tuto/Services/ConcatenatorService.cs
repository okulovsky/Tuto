using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Editor;
using Tuto.Model;
using Tuto.TutoServices.Assembler;
using Tuto.TutoServices.Montager;


namespace Tuto.TutoServices
{
    class ConcatenatorService : Service
    {
        bool recodeBeforeAssembling = false;

        public override string Name
        {
            get { return Services.Concatenator.ToString(); }
        }

        public override string Description
        {
            get { return DescriptionString; }
        }

        public override string Help
        {
            get { return HelpString; }
        }

        void RecodeFles(EditorModel model)
        {
            foreach (var file in model.ChunkFolder.GetFiles("end_chunk*.*"))
                file.Delete();
            foreach (var e in model.Montage.FileChunks)
                Shell.FFMPEG(false,
                    @"-i ""{0}"" -vf scale=1280:720 -r 30 -q:v 0 -acodec libmp3lame -ar 44100 -ab 32k -copyts ""{1}""",
                    Path.Combine(model.ChunkFolder.FullName, e.ChunkFilename),
                    Path.Combine(model.ChunkFolder.FullName, e.EndChunkFileName));
        }

        void RecodeFilesMock(EditorModel model)
        {
            foreach (var file in model.ChunkFolder.GetFiles("end_chunk*.*"))
                file.Delete();
            foreach (var e in model.Montage.FileChunks)
                File.Copy(
                    Path.Combine(model.ChunkFolder.FullName, e.ChunkFilename),
                    Path.Combine(model.ChunkFolder.FullName, e.EndChunkFileName));
        }

        List<List<MontageChunk>> SeparateByEpisode(EditorModel model)
        {
            List<List<MontageChunk>> list = new List<List<MontageChunk>>();
            var temp = new List<MontageChunk>();
            foreach (var e in model.Montage.FileChunks)
            {
                if (e.StartsNewEpisode) { list.Add(temp); temp = new List<MontageChunk>(); }
                temp.Add(e);
            }
            list.Add(temp);
            return list;
        }

        string GetChunkFileName(MontageChunk chunk)
        {
            return recodeBeforeAssembling ? chunk.EndChunkFileName : chunk.ChunkFilename;
        }

        void AssemblyWithExternalFile(EditorModel model)
        {
            var list = SeparateByEpisode(model);
            var tempFileName = Path.Combine(model.ChunkFolder.FullName, "temp.txt");
            for (int i = 0; i < list.Count; i++)
            {
                var e = list[i];
                var endFile = model.Locations.GetOutputFile(i);
                if (endFile.Exists) endFile.Delete();
                var str = e.Select(z => "file '" + Path.Combine(model.ChunkFolder.FullName, GetChunkFileName(z)) + "'\r\n").Aggregate((a, b) => a + b);
                File.WriteAllText(tempFileName, str);
                Shell.FFMPEG(false, @"-f concat -i ""{0}"" -q:v 0 -q:a 0 ""{1}""",
                    tempFileName, endFile.FullName);
            }
        }

        void AssemblyFromCommandLine(EditorModel model)
        {
            var list = SeparateByEpisode(model);

            for (int i = 0; i < list.Count; i++)
            {
                var endFile = model.Locations.GetOutputFile(i);
                if (endFile.Exists) endFile.Delete(); 
                var e = list[i];
                var str = e.Select(z => Path.Combine(model.ChunkFolder.FullName, GetChunkFileName(z))).Aggregate((a, b) => a + "|" + b);
                Shell.FFMPEG(false, @"-i ""concat:" + str + @""" -c copy ""{0}""", endFile.FullName);
            }
        }


        public void DoWork(EditorModel model, bool print)
        {
            if (recodeBeforeAssembling)
                RecodeFles(model);
            //AssemblyFromCommandLine(model);
            AssemblyWithExternalFile(model);
        }


        public override void DoWork(string[] args)
        {
            if (args.Length < 3)
                throw (new ArgumentException(String.Format("Insufficient args")));
            var folder = args[1];
            ExecMode mode;
            if (!Enum.TryParse(args[2], true, out mode))
                throw (new ArgumentException(String.Format("Unknown mode: {0}", args[2])));
            var print = mode == ExecMode.Print;

            var model = EditorModelIO.Load(folder);
            DoWork(model, print);
        }
        const string DescriptionString =
            @"Assembles chunks with effects using Avisynth.";
        const string HelpString =
            @"<folder> <mode>

folder: directory containing video
mode: run or print. Execute commands or write them to stdout";
    }
}