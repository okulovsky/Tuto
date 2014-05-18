using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Tuto.Model
{
    /// <summary>
    /// The token in the stream marks the beginning of some chunk
    /// </summary>
    [DataContract]
    public class StreamToken
    {
        const int StreamCount = 2;

        [DataMember]
        public int Time { get; internal set; }

        [DataMember]
        public bool StartsNewEpisode { get; internal set; }

        [DataMember]
        public bool Defined { get; internal set; }

        /// <summary>
        /// This array shows the video from which streams should be included in the chunk
        /// </summary>
        [DataMember]
        public readonly bool[] FromStream=new bool[StreamCount];

        public void CopyStreams(bool[] data)
        {
            if (data.Length != FromStream.Length) throw new ArgumentException();
            for (int i = 0; i < data.Length; i++) FromStream[i] = data[i];
        }
    }
}
