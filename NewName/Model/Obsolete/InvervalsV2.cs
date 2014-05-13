using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor
{
    public class IntervalV2
    {
        public double StartTime { get; set; }
        public double EndTime { get; set; }
        public bool HasVoice { get; set; }
    }

    public class MontageModelV2
    {
        public int TotalLength { get; set; }
        public int Shift { get; set; }




        public List<ChunkData> Chunks { get; set; }


        public List<Border> Borders { get; set; }

        public List<IntervalV2> Intervals { get; set; }


        public VideoInformation Information { get; set; }
    }

    public class FileContainerV2
    {
        public MontageModelV2 Montage { get; set; }
        public WindowState WindowState { get; set; }
    }
}
