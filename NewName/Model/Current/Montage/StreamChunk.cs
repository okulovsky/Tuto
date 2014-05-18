using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuto.Model
{
    /// <summary>
    /// The chunk is a region of video that is between two neighbors tokens
    /// </summary>
    public class StreamChunk
    {
        public readonly int StartTime;
        public readonly int EndTime;
        public readonly bool IsUndefined;
        public readonly bool IsDrop;
        public readonly bool IsFace;
        public readonly bool IsDesktop;
        public readonly bool StartsNewEpisode;
        public int Length { get { return EndTime - StartTime; } }

        public StreamChunk(int index, int startTime, int endTime, bool isUndefined, bool isDrop, bool isFace, bool isDesktop, bool startsNewEpisode)
        {
            StartsNewEpisode = startsNewEpisode;
            StartTime = startTime;
            EndTime = endTime;
            IsUndefined = isUndefined;
            IsFace = isFace;
            IsDrop = isDrop;
            IsDesktop = isDesktop;
        }
    }
}
