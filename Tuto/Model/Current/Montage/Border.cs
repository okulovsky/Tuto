using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Tuto.Model
{
    public class Border
    {
        public bool IsLeftBorder { get; set; }
        public int StartTime { get; set; }
        public int EndTime { get; set; }
        public int LeftChunk { get; set; }
        public int RightChunk { get; set; }

        public static Border Left(int border, int margin, int leftIndex, int rightIndex)
        {
            return new Border { IsLeftBorder = true, StartTime = border, EndTime = border + margin, LeftChunk = leftIndex, RightChunk=rightIndex};
        }
        public static Border Right(int border, int margin, int leftIndex, int rightIndex)
        {
            return new Border { IsLeftBorder = false, StartTime = border - margin, EndTime = border, LeftChunk = leftIndex, RightChunk = rightIndex };
        }

        public bool IsInside(int timeInMs)
        {
            return timeInMs >= StartTime && timeInMs < EndTime;
        }
    }
}
