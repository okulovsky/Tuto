using Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Tuto.Model
{
    [DataContract]
    public class StreamChunksArray : IEnumerable<StreamChunk>
    {
        [DataMember]
        private readonly List<StreamToken> tokens = new List<StreamToken>();
        [DataMember]
        public int StreamLength { get; private set; }

        public int Count { get { return tokens.Count; } }

        public StreamChunksArray(int StreamLength)
        {
            this.StreamLength = StreamLength;
            tokens.Add(new StreamToken { Time = 0 });
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

                var mode = Mode.Undefined;
                if (tokens[index].Defined)
                {
                    if (tokens[index].FromStream[0])
                        mode = Mode.Face;
                    else if (tokens[index].FromStream[1])
                        mode = Mode.Screen;
                    else
                        mode = Mode.Drop;
                };

                return new StreamChunk(
                    tokens[index].Time,
                    endTime,
                    mode,
                    tokens[index].StartsNewEpisode);
            }
        }

        public IEnumerator<StreamChunk> GetEnumerator()
        {
            for (int i = 0; i < Count; i++)
                yield return this[i];
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
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

        Tuple<int, StreamChunk> FindIndexAndChunk(int time)
        {
            var index = FindIndex(time);
            return Tuple.Create(index, this[index]);
        }

        public void Mark(int time, bool[] streams, bool replace)
        {
            var toks = FindIndexAndToken(time);
            if (!toks.Item2.Defined)
            {
                toks.Item2.SetDefined(streams);
                if (!replace)
                {
                    var token = new StreamToken()
                    {
                        Time = time,
                    };
                    tokens.Insert(toks.Item1 + 1, token);
                }
            }
            else
            {
                toks.Item2.CopyStreams(streams);
            }
            if (time > this.StreamLength)
                StreamLength = time;
        }

        public void Clear(int tokenIndex)
        {
            tokens[tokenIndex].SetUndefined();
            while (tokenIndex != tokens.Count - 1)
                if (!tokens[tokenIndex + 1].Defined) tokens.RemoveAt(tokenIndex + 1);
                else break;
            while (tokenIndex != 0)
                if (!tokens[tokenIndex - 1].Defined)
                {
                    tokens.RemoveAt(tokenIndex);
                    tokenIndex--;
                }
                else break;
        }

        public void MoveToken(int index, int newTime)
        {
            if (index < 0 || index >= tokens.Count) throw new ArgumentException();
            tokens[index].Time = newTime;
            for (int i = index - 1; i >= 0; i--)
                if (tokens[i].Time > newTime) tokens[i].Time = newTime;
                else break;
            for (int i = index + 1; i < tokens.Count; i++)
                if (tokens[i].Time < newTime) tokens[i].Time = newTime;
                else break;
        }


        public void NewEpisode(int index)
        {
            tokens[index].StartsNewEpisode = !tokens[index].StartsNewEpisode;
        }
   

        #endregion

 
    }
}
