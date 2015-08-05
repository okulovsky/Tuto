using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuto.Model
{
    public class PatchModel
    {
        public ObservableCollection<TrackInfo> MediaTracks { get; set; }
        public FileInfo SourceInfo { get; set; }
        public double Duration { get; set; }
        public PatchModel(string sourcePath)
        {
            SourceInfo = new FileInfo(sourcePath);
            MediaTracks = new ObservableCollection<TrackInfo>();
        }
    }

    public class TrackInfo : INotifyPropertyChanged
    {
        public Uri Path { get; set; }

        private double startSecond;
        public double StartSecond { get { return startSecond; } set { startSecond = value; ValueChanged("StartSecond"); } } //left border of chunk

        private double endSecond;
        public double EndSecond { get { return endSecond; } set { endSecond = value; ValueChanged("EndSecond"); } } //right border of chunk

        public double LeftShift { get; set; } //position relative to main track
        public double TopShift { get; set; }

        private double durationInSeconds;
        public double DurationInSeconds { get { return durationInSeconds; } set { durationInSeconds = value; ValueChanged("DurationInSeconds"); } }

        private double scale;
        public double Scale { get { return scale; } set { scale = value; ValueChanged("Scale"); } } //pixels per sec

        //not used yet
        private double positionRelativeToMain;
        private double PositionRelativeToMain { get { return positionRelativeToMain; } set { positionRelativeToMain = value; ValueChanged("PositionRelativeToMain"); } } //pixels per sec

        public TrackInfo(string path)
        {
            Path = new Uri(path);
            Scale = 1;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void ValueChanged(string property)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
        }
    }
}
