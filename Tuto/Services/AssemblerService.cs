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
    class AssemblerService : Service
    {

        public override string Name
        {
            get { return Services.Assembler.ToString(); }
        }

        public override string Description
        {
            get { return DescriptionString; }
        }

        public override string Help
        {
            get { return HelpString; }
        }

        public void DoWork(EditorModel model, bool print)
        {
            SrtMaker.WriteSrtFiles(model);
			model.CreateFileChunks();
			var epsodes = ListEpisodes(model.Montage.FileChunks).Select(e => MakeEpisode(model, e)).ToList();

            var episodeNumber = 0;
            foreach (var episode in epsodes)
            {
                var avsContext = new AvsContext();
                episode.SerializeToContext(avsContext);
                var avsScript = avsContext.Serialize(model);
                var avsFile = model.Locations.GetAvsStriptFile(episodeNumber);
                
                File.WriteAllText(avsFile.FullName, avsScript);
                
                var videoFile =  model.Locations.GetOutputFile(episodeNumber);
                if (videoFile.Exists) videoFile.Delete();
                
                var ffmpegCommand = new RenderAvsScript
                {
                    AvsInput = avsFile,
                    VideoOutput = videoFile
                };

                ffmpegCommand.Execute(print);
                episodeNumber++;
            }

        }

        private AvsNode MakeEpisode(EditorModel model, EpisodesChunks episode)
        {
            var fileChunks = episode.chunks;
            var avsChunks = new AvsConcatList { Items = new List<AvsNode>() };
            var fps = 25;

            avsChunks.Items.Add(AvsNode.NormalizedNode(model.Locations.Make(model.ChunkFolder, fileChunks[0].ChunkFilename), fps, fileChunks[0].Mode == Mode.Face));
            //making cross-fades
            for (int i = 1; i < fileChunks.Count; i++)
            {
                var currentChunk = fileChunks[i];
                var prevChunk = fileChunks[i - 1];
                AvsNode currentAvsChunk = AvsNode.NormalizedNode(model.Locations.Make(model.ChunkFolder, currentChunk.ChunkFilename), fps, currentChunk.Mode == Mode.Face);
                AvsNode prevAvsChunk = avsChunks.Items[avsChunks.Items.Count - 1];
                if (prevChunk.Mode == Mode.Face && currentChunk.Mode == Mode.Face)
                    avsChunks.Items[avsChunks.Items.Count - 1] = new AvsCrossFade
                    {
                        FadeFrom = prevAvsChunk,
                        FadeTo = currentAvsChunk
                    };
                else
                    avsChunks.Items.Add(currentAvsChunk);
            }

            // intro with fadein and fadeout
            //var intro = new AvsIntro
            //{
            //    VideoReference = model.Locations.Make(model.ChunkFolder, fileChunks[0].ChunkFilename),
            //    ImageFile = model.Locations.IntroImage
            //};
            //var normalizedIntro = AvsNode.NormalizedNode(intro);
            //var fadedIntro = new AvsFadeIn {Payload = new AvsFadeOut {Payload = normalizedIntro}};
            //avsChunks.Items.Insert(0, fadedIntro);

            // fadeout last item
            avsChunks.Items[avsChunks.Items.Count - 1] = new AvsFadeOut { Payload = avsChunks.Items[avsChunks.Items.Count - 1] };

            AvsNode resultedAvs = avsChunks;
            if (!string.IsNullOrEmpty(File.ReadAllText(model.Locations.GetSrtFile(episode.episodeNumber).FullName)))
            {
                resultedAvs = new AvsSubtitle { SrtPath = model.Locations.GetSrtFile(episode.episodeNumber).FullName, Payload = avsChunks };
            }


            // autolevel
            // ???

            return resultedAvs;;

            // watermark
            //return new AvsWatermark
            //{
            //    Payload = avsChunks,
            //    ImageFile = model.Locations.WatermarkImage
            //};

            /*
             * add intro
             * make crossfades
             * add fadein/fadeout
             * add autolevel?
             * add watermark
             */
        }


        class EpisodesChunks
        {
            public List<FileChunk> chunks = new List<FileChunk>();
            public int episodeNumber = 0;
        }

        private static List<EpisodesChunks> ListEpisodes(List<FileChunk> fileChunks)
        {
			var test = fileChunks.Any(z => z.StartsNewEpisode);

            var result = new List<EpisodesChunks>();
            var i = 0;
            while (i < fileChunks.Count)
            {
                var last = new EpisodesChunks();
                last.episodeNumber = result.Count;
                result.Add(last);

                while (i < fileChunks.Count && (!fileChunks[i].StartsNewEpisode || last.chunks.Count == 0))
                {
                    last.chunks.Add(fileChunks[i]);
                    i++;
                }
            }
            return result;
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