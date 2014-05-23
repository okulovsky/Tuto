using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Tuto.Model
{
    /// <summary>
    /// This clas contains an interval of sound or silence in the video
    /// </summary>
    [DataContract]
    public class SoundInterval
    {
        [DataMember]
        public int StartTime { get; set; }
        [DataMember]
        public int EndTime { get; set; }

        [DataMember]
        public bool HasVoice { get; set; }
        
        public int MiddleTime { get { return (StartTime + EndTime) / 2; } }

        
        public SoundInterval() { }

        public SoundInterval(int start, int end, bool hasVoice)
        {
            StartTime = start;
            EndTime = end;
            HasVoice = hasVoice;
        }

        public int DistanceTo(int ms)
        {
            if (StartTime <= ms && ms <= EndTime) return 0;
            return Math.Min(Math.Abs(StartTime - ms), Math.Abs(EndTime - ms));
        }

        public override string ToString()
        {
            return String.Format("({0}..{1} {2})", StartTime, EndTime, HasVoice ? "+" : "-");
        }
    }
}
