using Editor;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Tuto.TutoServices.Assembler;
using Tuto.Model;

namespace Tuto.Model2
{
    public class EditorModel
    {
		public readonly Locations Locations;
        public readonly Videotheque Global;
		public readonly MontageModel Montage;
		public readonly WindowState WindowState;
		public readonly FileInfo ModelFileLocation;
		public readonly DirectoryInfo RawLocation;
		public DirectoryInfo TempFolder
		{
			get
			{
				var relative = MyPath.RelativeTo(RawLocation.FullName, Global.RawFolder.FullName);
				var temp = Path.Combine(Global.TempFolder.FullName, relative);
				var directory = new DirectoryInfo(temp);
				if (!directory.Exists) directory.Create();
				return directory;
			}
		}

        public event EventHandler MontageModelChanged;

        public void OnNonSignificantChanged()
        {
            if (MontageModelChanged != null)
                MontageModelChanged(this, EventArgs.Empty);
        }

        public void OnMontageModelChanged()
        {
            OnNonSignificantChanged();
            //Montage.Montaged = false;
        }


		public EditorModel(
			FileInfo model,
			DirectoryInfo raw, 
			Videotheque videotheque,
			MontageModel montage, 
			WindowState windowState)
        {
			Montage = montage;
			WindowState = windowState;
			Global = videotheque;
			RawLocation = raw;
			ModelFileLocation = model;
			Locations = new Locations(this);
        }


        public void Save()
        {
			throw new NotImplementedException();
        }

        #region Basic algorithms

        private StreamChunksArray Tokens { get { return Montage.Chunks; } }

        public int FindChunkIndex(int time)
        {
            return Tokens.FindIndex(time);
        }

        public void MoveLeftChunkBorder(int index, int newTime)
        {
            if (index == 0) return;
            Tokens.MoveToken(index, newTime);
            OnMontageModelChanged();
        }

        public void MoveRightChunkBorder(int index, int newTime)
        {
            if (index == Tokens.Count - 1) return;
            Tokens.MoveToken(index + 1, newTime);
            OnMontageModelChanged();

        }

        public void ShiftLeftChunkBorder(int index, int deltaTime)
        {
            if (index == 0) return;
            Tokens.MoveToken(index, Tokens[index].StartTime + deltaTime);
            OnMontageModelChanged();

        }

        public void ShiftRightChunkBorder(int index, int deltaTime)
        {
            if (index == Tokens.Count - 1) return;
            Tokens.MoveToken(index + 1, Tokens[index].EndTime + deltaTime);
            OnMontageModelChanged();
        }
        
        #endregion
        #region Algorithms using WindowState properies

        static public bool[] ModeToBools(Mode mode)
        {
            if (mode == Mode.Drop) return new bool[] { false, false };
            if (mode == Mode.Face) return new bool[] { true, false };
            if (mode == Mode.Desktop) return new bool[] { false, true};
            throw new ArgumentException();

        }

        public void MarkHere(Mode mode, bool ctrl)
        {
            var time=WindowState.CurrentPosition;
            Tokens.Mark(time, ModeToBools(mode), ctrl);
            var index = Tokens.FindIndex(time);
            CorrectBorderBetweenChunksBySound(index - 1);
            CorrectBorderBetweenChunksBySound(index);
            OnMontageModelChanged();
        }

        public void RemoveChunkHere()
        {
            var position = WindowState.CurrentPosition;
            var index = Tokens.FindIndex(position);
            Tokens.Clear(index);
            OnMontageModelChanged();
        }

        
        public void NewEpisodeHere()
        {
            var index = Tokens.FindIndex(WindowState.CurrentPosition);
            if (index != -1)
            {
                Tokens.NewEpisode(index);
                OnMontageModelChanged();
            }
        }
        #endregion
        #region Correction by sound
        public void CorrectBorderBetweenChunksBySound(int leftChunkIndex)
        {
            if (leftChunkIndex < 0) return;
            var rightChunkIndex = leftChunkIndex + 1;
            if (rightChunkIndex >= Tokens.Count) return;
            var leftChunk = Tokens[leftChunkIndex];
            var rightChunk = Tokens[rightChunkIndex];
            if (leftChunk.Mode== Mode.Undefined || rightChunk.Mode == Mode.Undefined) return;

            var interval = Montage.SoundIntervals
                .Where(z => !z.HasVoice && z.DistanceTo(rightChunk.StartTime) < Global.Settings.VoiceSettings.MaxDistanceToSilence)
                .FirstOrDefault();
            if (interval == null) return;

            var leftDistance = Math.Abs(interval.StartTime - rightChunk.StartTime);
            var rightDistance = Math.Abs(interval.EndTime - rightChunk.StartTime);
            var distance = interval.DistanceTo(rightChunk.StartTime);
            bool LeftIn = leftDistance < Global.Settings.VoiceSettings.MaxDistanceToSilence;
            bool RightIn = rightDistance < Global.Settings.VoiceSettings.MaxDistanceToSilence;

            if (!LeftIn && !RightIn) return;

            int NewStart = rightChunk.StartTime;
            if (LeftIn && RightIn)
            {
                //значит, оба конца интервала - близко от точки сечения, и точку нужно передвинуть на середину интервада
                NewStart = interval.MiddleTime;
            }
            else if (LeftIn && !RightIn)
            {
                //значит, только левая граница где-то недалеко. 
                NewStart = interval.StartTime + Global.Settings.VoiceSettings.SilenceMargin;
            }
            else if (!LeftIn && RightIn)
            {
                NewStart = interval.EndTime - Global.Settings.VoiceSettings.SilenceMargin;
            }

            //не вылезли за границы интервала при перемещении
            if (interval.DistanceTo(NewStart) > 0) return;

            //не выскочили за границы чанков при перемещении
            if (!rightChunk.Contains(NewStart) && !leftChunk.Contains(NewStart)) return;

            Tokens.MoveToken(rightChunkIndex, NewStart);
        }
        #endregion
        #region Creation of final preparing
        public void FormPreparedChunks()
        {
             //Collapse adjacent chunks of same type into one FileChunk
            Montage.PreparedChunks = new List<StreamChunk>();
            var activeChunks = Tokens.ToList();

            if(!activeChunks.Any())
                return;
            activeChunks.Add(new StreamChunk(activeChunks.Last().EndTime,activeChunks.Last().EndTime,Mode.Undefined,true));
            var oldChunk = activeChunks[0];
            for (var i = 1; i < activeChunks.Count; i++)
            {
                
                var currentChunk = activeChunks[i];
                var prevChunk = activeChunks[i - 1];
                // collect adjacent chunks starting with oldChunk
                if (!currentChunk.StartsNewEpisode && currentChunk.Mode == oldChunk.Mode)
                    continue;
                // or flush adjacent chunks into one and start new sequence
                if (oldChunk.Length != 0)
                {
                    var preparedChunk = new StreamChunk(
                        oldChunk.StartTime,
                        prevChunk.EndTime,
                        oldChunk.Mode,
                        oldChunk.StartsNewEpisode
                        );
                    Montage.PreparedChunks.Add(preparedChunk);
                }
                oldChunk = currentChunk;
            }
        }

      
        #endregion

    }
}

