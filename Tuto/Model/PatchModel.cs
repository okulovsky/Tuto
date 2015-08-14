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
        public ObservableCollection<MediaTrack> MediaTracks { get; set; }

        [DataMember]
        public ObservableCollection<Subtitle> Subtitles { get; set; }

        [DataMember]
        public FileInfo SourceInfo { get; set; }

        [DataMember]
        private double duration;

        [DataMember]
        public double Duration { get { return duration; } set { duration = value; NotifyPropertyChanged(); NotifyPropertyChanged("DurationInPixels"); } }

        [DataMember]
        public double DurationInPixels { get { return duration * Scale; } set { duration = value / Scale; NotifyPropertyChanged(); } }

        [DataMember]
        private int scale; //to model

        [DataMember]
        public int Scale
        {
            get { return scale; }
            set
            {
                scale = value;
                foreach (var track in MediaTracks)
                {
                    var oldScale = track.Scale;
                    track.Scale = Scale;
                }

                if (Subtitles != null)
                    foreach (var sub in Subtitles)
                    {
                        var oldScale = sub.Scale;
                        sub.Scale = Scale;
                    }

                NotifyPropertyChanged();
            }
        } //pixels per sec

        public PatchModel(string sourcePath)
        {
            SourceInfo = new FileInfo(sourcePath);
            MediaTracks = new ObservableCollection<MediaTrack>();
            Subtitles = new ObservableCollection<Subtitle>() { new Subtitle("hello", 1, 10), new Subtitle("kitty", 1, 100) };
            Duration = 10;
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

    public class MediaTrack : TrackInfo 
    {
        public MediaTrack(string path, int scale)
        {
            Path = new Uri(path);
            ConvertedName = Guid.NewGuid().ToString() + ".avi";
            Scale = scale;
        }
    }

    public class Subtitle : TrackInfo
    {
        public string Content;
        public double X;
        public double Y;

        public Subtitle(string content, int scale, double leftShift)
        {
            StartSecond = 0;
            EndSecond = 50;
            DurationInSeconds = 500;
            LeftShiftInSeconds = leftShift;
            Content = content;
            Scale = scale;
        }
    }

    [DataContract]
    public abstract class TrackInfo : NotifierModel
    {
        [DataMember]
        private int scale;

        [DataMember]
        public int Scale { get { return scale; } set { scale = value; NotifyScaleChanged(); } }

        [DataMember]
        public Uri Path { get; set; }

        [DataMember]
        public string ConvertedName { get; set; }

        [DataMember]
        private double startSecond;

        [DataMember]
        public double StartSecond { get { return startSecond; } set { startSecond = value; NotifyPropertyChanged(); NotifyPropertyChanged("StartPixel"); } } //left border of chunk

        [DataMember]
        public double StartPixel { get { return StartSecond * Scale; } set { StartSecond = value / Scale; NotifyPropertyChanged(); } }

        [DataMember]
        private double endSecond { get; set; }

        [DataMember]
        public double EndSecond { get { return endSecond; } set { endSecond = value; NotifyPropertyChanged(); NotifyPropertyChanged("EndPixel"); } } //right border of chunk

        [DataMember]
        public double EndPixel { get { return EndSecond * Scale; } 
            set { EndSecond = value / Scale; NotifyPropertyChanged(); } }

        [DataMember]
        private double durationInSeconds { get; set; }

        [DataMember]
        public double DurationInSeconds { get { return durationInSeconds; } set { durationInSeconds = value; NotifyPropertyChanged(); NotifyPropertyChanged("DurationInPixels"); } }

        [DataMember]
        public double DurationInPixels { get { return DurationInSeconds * Scale; } set { DurationInSeconds = value / Scale; NotifyPropertyChanged(); } }

        //[DataMember]
        //private double currentWidthInPixels;

        //[DataMember]
        //public double CurrentWidthInPixels { get { return  currentWidthInPixels} set { currentWidthInPixels = value; NotifyPropertyChanged(); } }

        [DataMember]
        private double leftShiftInSeconds { get; set; }

        [DataMember]
        public double LeftShiftInSeconds { get { return leftShiftInSeconds; } set { leftShiftInSeconds = value; NotifyPropertyChanged(); NotifyPropertyChanged("LeftShiftInPixels"); } }//position of whole track relative to main track

        [DataMember]
        public double LeftShiftInPixels { get { return leftShiftInSeconds * Scale; } set { leftShiftInSeconds = value / Scale; NotifyPropertyChanged(); } }

        [DataMember]
        public double TopShift { get; set; }
     

        private void NotifyScaleChanged()
        {
            NotifyPropertyChanged("DurationInPixels");
            NotifyPropertyChanged("StartPixel");
            NotifyPropertyChanged("EndPixel");          
            NotifyPropertyChanged("LeftShiftInPixels");
            NotifyPropertyChanged("CurrentWidthInPixels");
        }

    }
}
