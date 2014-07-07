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
            
            var epsodes = ListEpisodes(model.Montage.FileChunks).Select(e => MakeEpisode(model, e));

            var i = 0;
            foreach (var episode in epsodes)
            {
                var avsContext = new AvsContext();
                episode.SerializeToContext(avsContext);
                var avsScript = avsContext.Serialize(model.Locations.AvsLibrary, model.Locations.AutoLevelsLibrary);
                var avsFile = model.Locations.AvsTempFile;
                using (var file = new StreamWriter(avsFile.OpenWrite()))
                {
                    file.WriteLine(avsScript);
                }
                var videoFile = model.Locations.Make(model.VideoFolder, string.Format("{0:D2}.avi", i)); // TODO: real filename
                videoFile.Delete();  // TODO: move all cleanup to one method
                var ffmpegCommand = new RenderAvsScript
                {
                    AvsInput = avsFile,
                    VideoOutput = videoFile
                };
                ffmpegCommand.Execute(print);
                i++;
            }
            
        }

        private AvsNode MakeEpisode(EditorModel model, List<FileChunk> fileChunks)
        {
            var avsChunks = new AvsConcatList{Items = new List<AvsNode>()};

            avsChunks.Items.Add(AvsNode.NormalizedNode(model.Locations.Make(model.ChunkFolder, fileChunks[0].ChunkFilename), fileChunks[0].Mode == Mode.Face));
            //making cross-fades
            for (int i = 1; i < fileChunks.Count; i++)
            {
                var currentChunk = fileChunks[i];
                var prevChunk = fileChunks[i - 1];
                AvsNode currentAvsChunk = AvsNode.NormalizedNode(model.Locations.Make(model.ChunkFolder, currentChunk.ChunkFilename), currentChunk.Mode == Mode.Face);
                AvsNode prevAvsChunk =  avsChunks.Items[avsChunks.Items.Count-1];
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
            var intro = new AvsIntro
            {
                VideoReference = model.Locations.Make(model.ChunkFolder, fileChunks[0].ChunkFilename),
                ImageFile = model.Locations.IntroImage
            };
            var normalizedIntro = AvsNode.NormalizedNode(intro);
            var fadedIntro = new AvsFadeIn {Payload = new AvsFadeOut {Payload = normalizedIntro}};
            avsChunks.Items.Insert(0, fadedIntro);

            // fadeout last item
            avsChunks.Items[avsChunks.Items.Count - 1] = new AvsFadeOut { Payload = avsChunks.Items[avsChunks.Items.Count - 1] };


            // autolevel
            // ???

            // watermark
            return new AvsWatermark
            {
                Payload = avsChunks,
                ImageFile = model.Locations.WatermarkImage
            };

            /*
             * add intro
             * make crossfades
             * add fadein/fadeout
             * add autolevel?
             * add watermark
             */
        }


        private static List<List<FileChunk>> ListEpisodes(List<FileChunk> fileChunks)
        {
            var result = new List<List<FileChunk>>();
            var i = 0;
            while(i < fileChunks.Count)
            {
                result.Add(new List<FileChunk>());
                while (i < fileChunks.Count && (!fileChunks[i].StartsNewEpisode || result.Last().Count == 0))
                {
                    
                    result.Last().Add(fileChunks[i]);
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