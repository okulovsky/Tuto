using Editor;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor
{

    public partial class MontageModelV4
    {
        public int TotalLength { get; set; }
        public int Shift { get; set; }

        public List<ChunkDataV4> Chunks { get; set; }
        public List<FileChunk> FileChunks { get; set; }

        public List<BorderV4> Borders { get; set; }

        public List<IntervalV4> Intervals { get; set; }


        public VideoInformationV4 Information { get; set; }

        public event EventHandler Changed;

        public void SetChanged()
        {
            if (Changed != null)
                Changed(this, EventArgs.Empty);
        }

        public MontageModelV4()
        {
            Chunks = new List<ChunkDataV4>();
            FileChunks = new List<FileChunk>();
            Borders = new List<BorderV4>();
            Information = new VideoInformationV4();
            Intervals = new List<IntervalV4>();
        }
    }
}