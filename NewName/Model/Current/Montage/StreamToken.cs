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
        public bool Defined { get; private set; }

        public void SetUndefined()
        {
            Defined = false;
            for (int i = 0; i < StreamCount; i++) FromStream[i] = false;
        }

        public void SetDefined(bool[] data)
        {
            Defined = true;
            CopyStreams(data);
        }

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

        public bool HasTheSameType(StreamToken token)
        {
            if (Defined != token.Defined) return false;
            if (!Defined) return true;
            for (int i = 0; i < StreamCount; i++)
                if (FromStream[i] != token.FromStream[i]) return false;
            return true;
        }
    }
}
