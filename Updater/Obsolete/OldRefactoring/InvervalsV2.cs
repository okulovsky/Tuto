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




        public List<ChunkDataV4> Chunks { get; set; }


        public List<BorderV4> Borders { get; set; }

        public List<IntervalV2> Intervals { get; set; }


        public VideoInformationV4 Information { get; set; }
    }

    public class FileContainerV2
    {
        public MontageModelV2 Montage { get; set; }
        public WindowStateV4 WindowState { get; set; }
    }
}
