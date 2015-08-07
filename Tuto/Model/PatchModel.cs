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
    public class PatchModel
    {
        [DataMember]
        public ObservableCollection<TrackInfo> MediaTracks { get; set; }

        [DataMember]
        public FileInfo SourceInfo { get; set; }

        [DataMember]
        public double Duration { get; set; }

        public PatchModel(string sourcePath)
        {
            SourceInfo = new FileInfo(sourcePath);
            MediaTracks = new ObservableCollection<TrackInfo>();
        }
    }

    [DataContract]
    public class TrackInfo : NotifierModel
    {
        [DataMember]
        public Uri Path { get; set; }
        [DataMember]
        public string ConvertedName { get; set; }

        [DataMember]
        private double startSecond;
        [DataMember]
        public double StartSecond { get { return startSecond; } set { startSecond = value; NotifyPropertyChanged(); } } //left border of chunk

        [DataMember]
        private double endSecond;
        [DataMember]
        public double EndSecond { get { return endSecond; } set { endSecond = value; NotifyPropertyChanged(); } } //right border of chunk

        [DataMember]
        public double LeftShift { get; set; } //position relative to main track
        [DataMember]
        public double TopShift { get; set; }

        [DataMember]
        private double durationInSeconds;
        [DataMember]
        public double DurationInSeconds { get { return durationInSeconds; } set { durationInSeconds = value; NotifyPropertyChanged(); } }

        [DataMember]
        private double scale; //to model
        [DataMember]
        public double Scale { get { return scale; } set { scale = value; NotifyPropertyChanged(); } } //pixels per sec

        //not used yet
        [DataMember]
        private double positionRelativeToMain;
        [DataMember]
        private double PositionRelativeToMain { get { return positionRelativeToMain; } set { positionRelativeToMain = value; NotifyPropertyChanged(); } } //pixels per sec

        public TrackInfo(string path)
        {
            Path = new Uri(path);
            ConvertedName = Guid.NewGuid().ToString() + ".avi";
            Scale = 1;
        }

    }
}
