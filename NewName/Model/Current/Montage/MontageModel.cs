using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Tuto.Model
{
    /// <summary>
    /// The montage information that completely describes one video
    /// </summary>
    [DataContract]
    public partial class MontageModel
    {
        /// <summary>
        /// Tokens of the episode
        /// </summary>
        [DataMember]
        public StreamTokenArray Tokens { get; private set; }

        /// <summary>
        /// Intervals of sound and silence
        /// </summary>
        [DataMember]
        public List<SoundInterval> SoundIntervals { get; private set; }

        /// <summary>
        /// Additional information about the video and its episodes
        /// </summary>
        [DataMember]
        public VideoInformation Information { get; private set; }

        /// <summary>
        /// Borders of each chunks. This information is required by one of the editor mode, but it is completely determined by tokens, so it is not stored
        /// </summary>
        public List<Border> Borders { get; private set; }

        public event EventHandler Changed;

        public void SetChanged()
        {
            if (Changed != null)
                Changed(this, EventArgs.Empty);
        }

        public MontageModel(int totalLength)
        {
            Tokens = new StreamTokenArray(totalLength);
            Borders = new List<Border>();
            Information = new VideoInformation();
            SoundIntervals = new List<SoundInterval>();
        }

        #region Basic Algorithms

        public void MoveLeftChunkBorder(int index, int newTime)
        {
            if (index == 0) return;
            Tokens.MoveToken(index, newTime);
            SetChanged();
        }

        public void MoveRightChunkBorder(int index, int newTime)
        {
            if (index == Tokens.Count - 1) return;
            Tokens.MoveToken(index + 1, newTime);
            SetChanged();
        }

        public void ShiftLeftChunkBorder(int index, int deltaTime)
        {
            if (index == 0) return;
            Tokens.MoveToken(index, Tokens[index].StartTime + deltaTime);
            SetChanged();
        }

        public void ShiftRightChunkBorder(int index, int deltaTime)
        {
            if (index == Tokens.Count - 1) return;
            Tokens.MoveToken(index + 1, Tokens[index].EndTime + deltaTime);
            SetChanged();
        }

        public void Mark(int time, bool[] sources, bool replace)
        {
            Tokens.Mark(time, sources, replace);
            SetChanged();
        }

        public void Clear(int index)
        {
            Tokens.Clear(index);
            SetChanged();
        }

        #endregion
    }
}
