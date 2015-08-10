using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Tuto.Model
{
    [DataContract]
    public class PatchModel : NotifierModel
    {
        [DataMember]
        public ObservableCollection<TrackInfo> MediaTracks { get; set; }

        [DataMember]
        public FileInfo SourceInfo { get; set; }

        [DataMember]
        public double Duration { get; set; }

        [DataMember]
        private int scale; //to model
        [DataMember]
        public int Scale { get { return scale; } set { scale = value; NotifyPropertyChanged(); } } //pixels per sec

        public PatchModel(string sourcePath)
        {
            SourceInfo = new FileInfo(sourcePath);
            MediaTracks = new ObservableCollection<TrackInfo>();
            Scale = 1;
        }

        public void DeleteTrackAccordingPosition(int index, EditorModel m)
        {
            var trackName = MediaTracks[index].ConvertedName;
            MediaTracks.RemoveAt(index);
            var name = System.IO.Path.Combine(m.Locations.TemporalDirectory.FullName, trackName);
            if (File.Exists(name))
                try { File.Delete(name); }
                catch { }
        }
    }

    [DataContract]
    public class TrackInfo : NotifierModel
    {

        public int Scale { get; set; }

        [DataMember]
        public Uri Path { get; set; }
        [DataMember]
        public string ConvertedName { get; set; }

        [DataMember]
        public double StartSecond { get; set; } //left border of chunk

        [DataMember]
        public double StartPixel { get { return StartSecond * Scale; } set { StartSecond = value / Scale; NotifyPropertyChanged(); } }

        [DataMember]
        public double EndPixel { get { return EndSecond * Scale; } set { EndSecond = value / Scale; NotifyPropertyChanged(); } }

        [DataMember]
        public double EndSecond { get; set; } //right border of chunk

        [DataMember]
        private double leftShift { get; set; }

        [DataMember]
        public double LeftShift { get { return leftShift; } set { leftShift = value; NotifyPropertyChanged(); } }//position of whole track relative to main track

        [DataMember]
        public double TopShift { get; set; }

        [DataMember]
        public double DurationInSeconds { get; set; }

        [DataMember]
        public double DurationInPixels { get { return DurationInSeconds * Scale; } set { DurationInSeconds = value / Scale; NotifyPropertyChanged(); } }

        //not used yet
        [DataMember]
        private double positionRelativeToMain;
        [DataMember]
        private double PositionRelativeToMain { get { return positionRelativeToMain; } set { positionRelativeToMain = value; NotifyPropertyChanged(); } } //pixels per sec

        
        public TrackInfo(string path, int scale)
        {
            Path = new Uri(path);
            ConvertedName = Guid.NewGuid().ToString() + ".avi";
            Scale = scale;
        }

    }
}
