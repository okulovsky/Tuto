using Editor;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoLib;

namespace Editor
{

    public class MontageModelV1 : INotifyPropertyChanged
    {
        public int TotalLength { get; set; }
        public int Shift { get; set; }

        public EditorModes EditorMode { get; set; }

        Mode currentMode;
        public Mode CurrentMode
        {
            get { return currentMode; }
            set
            {
                currentMode = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("CurrentMode"));
            }
        }

        int currentPosition;
        public int CurrentPosition
        {
            get { return currentPosition; }
            set
            {
                currentPosition = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("CurrentPosition"));
            }
        }



        public List<ChunkData> Chunks { get; set; }


        public List<Border> Borders { get; set; }

        

        public VideoInformation Information { get; set; }

        public MontageModelV1()
        {
            Chunks = new List<ChunkData>();
            Borders = new List<Border>();
            Information = new VideoInformation();
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
