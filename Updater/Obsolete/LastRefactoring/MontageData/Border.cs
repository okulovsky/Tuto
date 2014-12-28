using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor
{
    public class BorderV4
    {
        public bool IsLeftBorder { get; set; }
        public int StartTime { get; set; }
        public int EndTime { get; set; }
        public int LeftChunk { get; set; }
        public int RightChunk { get; set; }

        public static BorderV4 Left(int border, int margin, int leftIndex, int rightIndex)
        {
            return new BorderV4 { IsLeftBorder = true, StartTime = border, EndTime = border + margin, LeftChunk = leftIndex, RightChunk=rightIndex};
        }
        public static BorderV4 Right(int border, int margin, int leftIndex, int rightIndex)
        {
            return new BorderV4 { IsLeftBorder = false, StartTime = border - margin, EndTime = border, LeftChunk = leftIndex, RightChunk = rightIndex };
        }

        public bool IsInside(int timeInMs)
        {
            return timeInMs >= StartTime && timeInMs < EndTime;
        }
    }
}
