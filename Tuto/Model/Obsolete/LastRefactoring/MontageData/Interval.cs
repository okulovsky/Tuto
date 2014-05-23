using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor
{

    public class IntervalV4
    {
        public int StartTime { get; set; }
        public int EndTime { get; set; }
        public int MiddleTimeMS { get { return (StartTime+EndTime)/2; } }

        public bool HasVoice { get; set; }

        public IntervalV4() { }

        public IntervalV4(int start, int end, bool hasVoice)
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
