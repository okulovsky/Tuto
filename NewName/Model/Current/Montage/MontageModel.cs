using Editor;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor
{

    public partial class MontageModel
    {
        public int TotalLength { get; set; }
        public int Shift { get; set; }

        public List<ChunkData> Chunks { get; set; }
        public List<FileChunk> FileChunks { get; set; }

        public List<Border> Borders { get; set; }

        public List<Interval> Intervals { get; set; }


        public VideoInformation Information { get; set; }

        public event EventHandler Changed;

        public void SetChanged()
        {
            if (Changed != null)
                Changed(this, EventArgs.Empty);
        }

        public MontageModel()
        {
            Chunks = new List<ChunkData>();
            FileChunks = new List<FileChunk>();
            Borders = new List<Border>();
            Information = new VideoInformation();
            Intervals = new List<Interval>();
        }
    }
}