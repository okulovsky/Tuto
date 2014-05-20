using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Editor
{

    public class EditorModelV4
    {
        public DirectoryInfo RootFolder { get; set; }
        public DirectoryInfo VideoFolder { get; set; }
        public DirectoryInfo ChunkFolder { get; set; }
        public DirectoryInfo ProgramFolder { get; set; }

        public LocationsV4 Locations { get; private set; }

        public GlobalDataV4 Global { get; set; }
        public MontageModelV4 Montage { get; set; }

        public WindowStateV4 WindowState { get; set; }

        public EditorModelV4()
        {
            Montage = new MontageModelV4();
            Locations = new LocationsV4(this);
            WindowState = new WindowStateV4();
            Global = new GlobalDataV4();
        }


        #region I'm not sure EditorModel is a right place for that

        public void SetChunkMode(Mode mode, bool ctrl)
        {
            SetChunkMode(WindowState.CurrentPosition, mode, ctrl);
            Montage.SetChanged();
        }

        public void RemoveChunk()
        {

            var position = WindowState.CurrentPosition;
            var index = Montage.Chunks.FindChunkIndex(position);
            if (index == -1) return;
            var chunk = Montage.Chunks[index];
            chunk.Mode = Mode.Undefined;
            if (index != Montage.Chunks.Count - 1 && Montage.Chunks[index + 1].Mode == Mode.Undefined)
            {
                chunk.Length += Montage.Chunks[index + 1].Length;
                Montage.Chunks.RemoveAt(index + 1);
            }
            if (index != 0 && Montage.Chunks[index - 1].Mode == Mode.Undefined)
            {
                chunk.StartTime = Montage.Chunks[index - 1].StartTime;
                chunk.Length += Montage.Chunks[index - 1].Length;
                Montage.Chunks.RemoveAt(index - 1);
            }
            Montage.SetChanged();
        }

        public void SetChunkMode(int position, Mode mode, bool ctrl)
        {
            var index = Montage.Chunks.FindChunkIndex(position);
            if (index == -1) return;
            var chunk = Montage.Chunks[index];
            if (chunk.Mode == Mode.Undefined && chunk.Length > 500 && !ctrl)
            {
                var chunk1 = new ChunkDataV4 { StartTime = chunk.StartTime, Length = position - chunk.StartTime, Mode = mode };
                var chunk2 = new ChunkDataV4 { StartTime = position, Length = chunk.Length - chunk1.Length, Mode = Mode.Undefined };
                Montage.Chunks.RemoveAt(index);
                Montage.Chunks.Insert(index, chunk1);
                Montage.Chunks.Insert(index + 1, chunk2);
            }
            else
            {
                chunk.Mode = mode;
            }
            CorrectBorderBetweenChunksBySound(index - 1);
            CorrectBorderBetweenChunksBySound(index);

        }


        public void CorrectBorderBetweenChunksBySound(int leftChunkIndex)
        {
            if (leftChunkIndex < 0) return;
            var rightChunkIndex = leftChunkIndex + 1;
            if (rightChunkIndex >= Montage.Chunks.Count) return;
            var leftChunk = Montage.Chunks[leftChunkIndex];
            var rightChunk = Montage.Chunks[rightChunkIndex];
            if (leftChunk.Mode == Mode.Undefined || rightChunk.Mode == Mode.Undefined) return;
            
            var interval = Montage.Intervals
                .Where(z => !z.HasVoice && z.DistanceTo(rightChunk.StartTime) < Global.VoiceSettings.MaxDistanceToSilence)
                .FirstOrDefault();
            if (interval == null) return;

            var leftDistance = Math.Abs(interval.StartTime - rightChunk.StartTime);
            var rightDistance = Math.Abs(interval.EndTime - rightChunk.StartTime);
            var distance = interval.DistanceTo(rightChunk.StartTime);
            bool LeftIn = leftDistance < Global.VoiceSettings.MaxDistanceToSilence;
            bool RightIn = rightDistance  < Global.VoiceSettings.MaxDistanceToSilence;

            if (!LeftIn && !RightIn) return;

            int NewStart = rightChunk.StartTime;
            if (LeftIn && RightIn)
            {
                //значит, оба конца интервала - близко от точки сечения, и точку нужно передвинуть на середину интервада
                NewStart = interval.MiddleTimeMS;
            }
            else if (LeftIn && !RightIn)
            {
                //значит, только левая граница где-то недалеко. 
                NewStart = interval.StartTime + Global.VoiceSettings.SilenceMargin;
            }
            else if (!LeftIn && RightIn)
            {
                NewStart = interval.EndTime - Global.VoiceSettings.SilenceMargin;
            }

            //не вылезли за границы интервала при перемещении
            if (interval.DistanceTo(NewStart) > 0) return;

            //не выскочили за границы чанков при перемещении
            if (!rightChunk.Contains(NewStart) && !leftChunk.Contains(NewStart)) return;

            Montage.Chunks.ShiftLeftBorderToRight(rightChunkIndex, rightChunk.StartTime-NewStart);
        }

        public void CreateFileChunks()
        {
            // Collapse adjacent chunks of same type into one FileChunk
            Montage.FileChunks = new List<FileChunk>();
            var activeChunks = Montage.Chunks.Where(c => c.IsActive).ToList();
            activeChunks.Add(new ChunkDataV4
            {
                StartsNewEpisode = true
            });
            var oldChunk = activeChunks[0];
            for (var i = 1; i < activeChunks.Count; i++)
            {
                var currentChunk = activeChunks[i];
                var prevChunk =  activeChunks[i-1];
                // collect adjacent chunks starting with oldChunk
                if (!currentChunk.StartsNewEpisode && currentChunk.Mode == oldChunk.Mode)
                    continue;
                // or flush adjacent chunks into one and start new sequence
                Montage.FileChunks.Add(new FileChunk
                {
                    Mode = oldChunk.Mode,
                    StartTime = oldChunk.StartTime,
                    Length = prevChunk.EndTime - oldChunk.StartTime,
                    //SourceFilename = oldChunk.Mode == Mode.Face ? Locations.FaceVideo : Locations.DesktopVideo,
                    StartsNewEpisode = oldChunk.StartsNewEpisode
                });
                oldChunk = currentChunk;
            }
        }

        public void NewEpisodeHere()
        {
            var index = Montage.Chunks.FindChunkIndex(WindowState.CurrentPosition);
            if (index != -1)
            {
                Montage.Chunks[index].StartsNewEpisode = !Montage.Chunks[index].StartsNewEpisode;
                Montage.SetChanged();
            }
        }

        #endregion
    }
}

