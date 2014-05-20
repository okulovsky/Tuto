using Editor;
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
        public readonly Mode Mode;
        public readonly bool StartsNewEpisode;

        public int Length { get { return EndTime - StartTime; } }
        public bool IsActive { get { return Mode != Editor.Mode.Undefined && Mode != Editor.Mode.Drop; } }
        public bool IsNotActive { get { return !IsActive; } } 

   
        public StreamChunk(int startTime, int endTime, Mode mode, bool startsNewEpisode)
        {
            StartsNewEpisode = startsNewEpisode;
            StartTime = startTime;
            EndTime = endTime;
            Mode = mode;
         }

        public bool Contains(int ms)
        {
            return StartTime <= ms && ms < EndTime;
        }
    }
}
