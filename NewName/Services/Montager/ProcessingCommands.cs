using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Editor;

namespace Tuto.Services.Montager
{
    class ProcessingCommands
    {   

        public static IEnumerable<FFMPEGCommand> Processing(EditorModelV4 model, List<FileChunk> chunks)
        {
            return chunks.SelectMany(z => Commands(model, z));
        }

        public static IEnumerable<FFMPEGCommand> Commands(EditorModelV4 model, FileChunk chunk)
        {
            switch (chunk.Mode)
            {
                case Mode.Face:
                    yield return new ExtractFaceVideoCommand
                    {
                        VideoInput = /*chunk.SourceFilename,*/ model.Locations.FaceVideo,
                        StartTime = chunk.StartTime,
                        Duration = chunk.Length,
                        VideoOutput = model.Locations.Make(model.ChunkFolder, chunk.ChunkFilename)
                    };
                    break;
                case Mode.Screen:
                    yield return new ExtractAudioCommand
                    {
                        AudioInput = /*chunk.SourceFilename,*/ model.Locations.FaceVideo,
                        StartTime = chunk.StartTime,
                        Duration = chunk.Length,
                        AudioOutput = model.Locations.Make(model.ChunkFolder, chunk.AudioFilename)
                    };
                    yield return new ExtractScreenVideoCommand
                    {
                        VideoInput = /*chunk.SourceFilename,*/ model.Locations.DesktopVideo,
                        StartTime = chunk.StartTime,
                        Duration = chunk.Length,
                        VideoOutput = model.Locations.Make(model.ChunkFolder, chunk.VideoFilename)
                    };
                    yield return new MixVideoAudioCommand
                    {
                        VideoInput = model.Locations.Make(model.ChunkFolder, chunk.VideoFilename),
                        AudioInput = model.Locations.Make(model.ChunkFolder, chunk.AudioFilename),
                        VideoOutput = model.Locations.Make(model.ChunkFolder, chunk.ChunkFilename)
                    };
                    break;
            }
        }
    }
}
