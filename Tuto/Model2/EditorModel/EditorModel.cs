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

namespace Tuto.Model
{
    public class EditorModel
    {
        public Locations Locations { get; private set; }
        public Videotheque Videotheque { get; private set; }
        public MontageModel Montage { get; private set; }
        public WindowState WindowState { get; private set; }
		public readonly FileInfo ModelFileLocation;
		public readonly DirectoryInfo RawLocation;
        public readonly bool InputFilesAreOK;


		public DirectoryInfo TempFolder
		{
			get
			{
				var temp = Path.Combine(Videotheque.TempFolder.FullName, Montage.RawVideoHash);
				var directory = new DirectoryInfo(temp);
				if (!directory.Exists) directory.Create();
				return directory;
			}
		}

        public event EventHandler EnvironmentChanged;

        public void OnEnvironmentChanged()
        {
            if (EnvironmentChanged!=null)
                EnvironmentChanged(this,EventArgs.Empty);
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
			windowState.EditorModel = this;
			Videotheque = videotheque;
			RawLocation = raw;
			ModelFileLocation = model;
			Locations = new Locations(this);
            InputFilesAreOK = raw.Exists;
        }


        public void Save()
        {
            Videotheque.SaveEditorModel(this);
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
        
        public void InsertDeletion(int time)
        {
            const int pauseSize = 1000;
            var token = FindChunkIndex(time);
            if (Tokens[token].Length<=pauseSize)
            {
                Tokens.Mark(time, ModeToBools(Mode.Drop), true);
            }
            else if (Tokens[token].EndTime>time+pauseSize)
            {
                var type = Tokens[token].Mode;
                Tokens.Clear(token);
                Tokens.Mark(time, ModeToBools(type), false);
                Tokens.Mark(time + pauseSize, ModeToBools(Mode.Drop), false);
                Tokens.Mark(time + pauseSize + 1, ModeToBools(type), true);
            }
            else
            {
                var type = Tokens[token].Mode;
                var div = Tokens[token].EndTime - pauseSize;
                Tokens.Clear(token);
                Tokens.Mark(div, ModeToBools(type), false);
                Tokens.Mark(div + 1, ModeToBools(Mode.Drop), true);
            }
            GenerateBorders();
            OnMontageModelChanged();
        }

        #endregion

        #region Generating borders

        const int Margin = 3000;
        /*
         * Левая граница - это когда предыдущий чанк другого типа. Играется с левой границы до +Margin
         * Правая граница - это когда последующий чанк неактивен. Играется с -Margin до правой границы
         * Если области левой и правой границ перекрываются, делается пополам
         */
        IEnumerable<Border> GenerateBordersPreview()
        {
            for (int i = 1; i < Montage.Chunks.Count; i++)
            {
                if (Montage.Chunks[i].Mode != Montage.Chunks[i - 1].Mode)
                {
                    if (Montage.Chunks[i - 1].IsActive)
                    {
                        yield return Border.Right(Montage.Chunks[i].StartTime, Margin, i - 1, i);
                    }

                    if (Montage.Chunks[i].IsActive)
                    {
                        yield return Border.Left(Montage.Chunks[i].StartTime, Margin, i - 1, i);
                    }
                }
            }
        }

        public void GenerateBorders()
        {
            var borders = GenerateBordersPreview().ToList();
            for (int i = 1; i < borders.Count; i++)
            {
                if (borders[i - 1].EndTime > borders[i].StartTime)
                {
                    var time = (borders[i - 1].EndTime + borders[i].StartTime) / 2;
                    borders[i - 1].EndTime = time;
                    borders[i].StartTime = time;
                }
            }
            Montage.Borders = borders;

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
                .Where(z => !z.HasVoice && z.DistanceTo(rightChunk.StartTime) < Videotheque.Data.VoiceSettings.MaxDistanceToSilence)
                .FirstOrDefault();
            if (interval == null) return;

            var leftDistance = Math.Abs(interval.StartTime - rightChunk.StartTime);
            var rightDistance = Math.Abs(interval.EndTime - rightChunk.StartTime);
            var distance = interval.DistanceTo(rightChunk.StartTime);
            bool LeftIn = leftDistance < Videotheque.Data.VoiceSettings.MaxDistanceToSilence;
            bool RightIn = rightDistance < Videotheque.Data.VoiceSettings.MaxDistanceToSilence;

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
                NewStart = interval.StartTime + Videotheque.Data.VoiceSettings.SilenceMargin;
            }
            else if (!LeftIn && RightIn)
            {
                NewStart = interval.EndTime - Videotheque.Data.VoiceSettings.SilenceMargin;
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

