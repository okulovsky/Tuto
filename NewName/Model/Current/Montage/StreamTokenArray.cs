using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Tuto.Model
{
    [DataContract]
    public class StreamTokenArray
    {
        [DataMember]
        private readonly List<StreamToken> tokens = new List<StreamToken>();
        [DataMember]
        public readonly int StreamLength;

        public StreamTokenArray(int StreamLength)
        {
            this.StreamLength = StreamLength;
        }

        /// <summary>
        /// Gets the immutable file chunk between index and index+1 tokens
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public StreamChunk this[int index]
        {
            get
            {
                if (index < 0 || index >= tokens.Count) throw new ArgumentException();
                var endTime = StreamLength;
                if (index != tokens.Count - 1)
                    endTime = tokens[index + 1].Time;
                return new StreamChunk(
                    index,
                    tokens[index].Time,
                    endTime,
                    !tokens[index].Defined,
                    !tokens[index].FromStream[0] && !tokens[index].FromStream[1],
                    tokens[index].FromStream[0],
                    tokens[index].FromStream[1],
                    tokens[index].StartsNewEpisode);
            }
        }

        #region Algorithms

        public int FindIndex(int time)
        {
            for (int i = 0; i < tokens.Count; i++)
                if (tokens[i].Time > time) return i - 1;
            return tokens.Count - 1;
        }

        Tuple<int, StreamToken> FindIndexAndToken(int time)
        {
            var index = FindIndex(time);
            return Tuple.Create(index, tokens[index]);
        }

        public void Mark(int time, bool[] streams, bool replace)
        {
            var toks = FindIndexAndToken(time);
            if (!toks.Item2.Defined)
            {
                toks.Item2.Defined = true;
                toks.Item2.CopyStreams(streams);
                if (!replace)
                {
                    var token = new StreamToken()
                    {
                        Time = time,
                        Defined = false,
                        StartsNewEpisode = false
                    };
                    tokens.Insert(toks.Item1 + 1, token);
                }
            }
            else
            {
                toks.Item2.CopyStreams(streams);
            }
        }
        


        #endregion
    }
}
