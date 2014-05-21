using Editor;
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
        public StreamChunksArray Chunks { get; private set; }

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

        [DataMember]
        public List<FileChunk> FileChunks { get; set; }

        [DataMember]
        public int SynchronizationShift { get; set; }

        /// <summary>
        /// Borders of each chunks. This information is required by one of the editor mode, but it is completely determined by tokens, so it is not stored
        /// </summary>
        public List<Border> Borders { get; set; }

        public MontageModel(int totalLength)
        {
            Chunks = new StreamChunksArray(totalLength);
            Borders = new List<Border>();
            Information = new VideoInformation();
            SoundIntervals = new List<SoundInterval>();
        }
    }
}
