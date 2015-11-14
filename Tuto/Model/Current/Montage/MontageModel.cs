using Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace Tuto.Model
{
    /// <summary>
    /// The montage information that completely describes one video
    /// </summary>
    [DataContract]
    public partial class MontageModel
    {
        [DataMember]
        public string RawVideoHash { get; private set; }

        public DateTime ModificationTime { get; set; }

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
        public List<StreamChunk> PreparedChunks { get; set; }

        [DataMember]
        public int SynchronizationShift { get; set; }

        [DataMember]
        public bool CrossfadesEnabled { get; set; }

        /// <summary>
        /// String fixes for video, which are to encoded as subtitiles
        /// </summary>
        [DataMember]
        public List<SubtitleFix> SubtitleFixes { get; internal set; }

        /// <summary>
        /// Borders of each chunks. This information is required by one of the editor mode, but it is completely determined by tokens, so it is not stored
        /// </summary>
        public List<Border> Borders { get; set; }

        /// <summary>
        /// True, if the clips were prepared
        /// </summary>
        [DataMember]
        public bool ReadyToEdit { get; set; }

        public MontageModel(int totalLength, string filesHash = null)
        {
            RawVideoHash = filesHash;
            Chunks = new StreamChunksArray(totalLength);
            Borders = new List<Border>();
            Information = new VideoInformation();
            SoundIntervals = new List<SoundInterval>();
            SubtitleFixes = new List<SubtitleFix>();
            CrossfadesEnabled = true;
        }
    }
}
