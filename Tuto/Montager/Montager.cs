using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VideoLib;

namespace Montager
{
    public class Montager
    {


        #region Второй способ обработки
        public static IEnumerable<BatchCommand> Processing2(List<Chunk> chunks, string outputFile)
        {
            foreach (var e in chunks.SelectMany(z => Commands2(z))) yield return e;
            foreach(var e in Aggregation2(chunks,outputFile)) yield return e;
        }

        public static IEnumerable<BatchCommand> Aggregation2(List<Chunk> chunks, string outputFile)
        {
            yield return new ConcatCommand
            {
                Files = chunks.Select(z => z.TemporalAudioFile).ToList(),
                Result = "TotalAudio.mp3",
                AudioOnly = true

            };
            yield return new ConcatCommand
            {
                Files = chunks.Select(z => z.TemporalVideoFile).ToList(),
                Result = "TotalVideo.avi",
            };
            yield return new MixVideoAudioCommand
            {
                AudioInput = "TotalAudio.mp3",
                VideoInput = "TotalVideo.avi",
                VideoOutput = outputFile
            };

        }

        public static IEnumerable<BatchCommand> Commands2(Chunk chunk)
        {
            if (chunk.IsFaceChunk)
                yield return new ExtractFaceVideoCommand
                {
                    VideoInput = chunk.VideoSource.File,
                    StartTime = chunk.VideoSource.StartTime,
                    Duration = chunk.VideoSource.Duration,
                    VideoOutput = chunk.TemporalVideoFile
                };
            else
                yield return new ExtractScreenVideoCommand
                {
                    VideoInput = chunk.VideoSource.File,
                    StartTime = chunk.VideoSource.StartTime,
                    Duration = chunk.VideoSource.Duration,
                    VideoOutput = chunk.TemporalVideoFile
                };
            var s = chunk.AudioSource ?? chunk.VideoSource;
            yield return new ExtractAudioCommand
            {
                VideoInput = s.File,
                StartTime = s.StartTime,
                Duration = s.Duration,
                AudioOutput = chunk.TemporalAudioFile
            };
        }
        #endregion
        #region Первый способ обработки
        public static IEnumerable<BatchCommand> Processing1(List<Chunk> chunks, string outputFile)
        {
            foreach (var e in chunks.SelectMany(z => Commands1(z))) yield return e;
     //       foreach(var e in Aggregation1(chunks,outputFile)) yield return e;
        }
        public static IEnumerable<BatchCommand> Aggregation1(List<Chunk> chunks, string outputFile)
        {
            yield return new ConcatCommand
              {
                  Files = chunks.Select(z => z.OutputVideoFile).ToList(),
                  Result = outputFile
              };
        }
        

        public static IEnumerable<BatchCommand> Commands1(Chunk chunk)
        {
            if (chunk.IsFaceChunk)
            {
                yield return new ExtractFaceVideoCommand
                    {
                        VideoInput = chunk.VideoSource.File,
                        StartTime = chunk.VideoSource.StartTime,
                        Duration = chunk.VideoSource.Duration,
                        VideoOutput = chunk.OutputVideoFile
                    };
            }
            else
            {
               yield return new ExtractAudioCommand
                {
                    VideoInput=chunk.AudioSource.File,
                    StartTime=chunk.AudioSource.StartTime,
                    Duration=chunk.AudioSource.Duration,
                    AudioOutput=chunk.TemporalAudioFile
                };
               yield return new ExtractScreenVideoCommand
               {
                   VideoInput = chunk.VideoSource.File,
                   StartTime = chunk.VideoSource.StartTime,
                   Duration = chunk.VideoSource.Duration,
                   VideoOutput = chunk.TemporalVideoFile
               };
                yield return new MixVideoAudioCommand
                {
                    VideoInput=chunk.TemporalVideoFile,
                    AudioInput=chunk.TemporalAudioFile,
                    VideoOutput=chunk.OutputVideoFile
                };
            }

        }
        #endregion

        public static List<Chunk> CreateChunks(MontageLog log, string faceFile, string screenFile)
        {
            var commands = log.Commands;

            var result = new List<Chunk>();
            if (commands[0].Action != MontageAction.StartFace)
                throw new Exception("Expected StartFace as the first command");
            int faceLogSync = commands[0].Time;

            if (commands[1].Action != MontageAction.StartScreen)
                throw new Exception("Expected StartScreen as the second command");
            int screenSync = commands[1].Time;


            int currentTime=screenSync;
            bool isFace=true;
            int currentId=0;


            for (int i = 2; i < commands.Count; i++)
            {
                if (commands[i].Action == MontageAction.Delete)
                {
                    currentTime= commands[i].Time;
                    continue;
                }
             
                if (isFace)
                    result.Add(new Chunk
                    {
                        Id = commands[i].Id,
                        IsFaceChunk=true,
                        VideoSource = new ChunkSource
                           {
                               StartTime = currentTime - faceLogSync + log.FaceFileSync,
                               Duration = commands[i].Time - currentTime,
                               File = faceFile
                           }
                    });
                else
                    result.Add(new Chunk
                    {
                        Id = commands[i].Id,
                        IsFaceChunk=false,
                        VideoSource = new ChunkSource
                        {
                            StartTime = currentTime - screenSync,
                            Duration = commands[i].Time - currentTime,
                            File = screenFile
                        },
                        AudioSource = new ChunkSource
                        {
                            StartTime = currentTime - faceLogSync + log.FaceFileSync,
                            Duration = commands[i].Time - currentTime,
                            File = faceFile
                        }
                    });

                currentTime = commands[i].Time;
                if (commands[i].Action == MontageAction.Face)
                    isFace = true;
                if (commands[i].Action == MontageAction.Screen)
                    isFace = false;


                if (result[result.Count - 1].VideoSource.Duration == 0)
                    result.RemoveAt(result.Count - 1);

            }
            return result;

        }
    }
}
