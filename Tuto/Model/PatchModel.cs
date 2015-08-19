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
using System.Windows;

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
        public ScaleInfo ScaleInfo; //to model

        public double Width { get; set; }
        public double Height { get; set; }

        public double ActualWidth { get; set; }
        public double ActualHeight { get; set; }
        
        public int Scale
        {
            get { return ScaleInfo.Scale; }
            set
            {
                ScaleInfo.Scale = value;
                NotifyPropertyChanged();
            }
        } //pixels per sec

        public PatchModel(string sourcePath)
        {
            SourceInfo = new FileInfo(sourcePath);
            MediaTracks = new ObservableCollection<MediaTrack>();
            Subtitles = new ObservableCollection<Subtitle>() { new Subtitle("hello", new ScaleInfo(1), 10), new Subtitle("kitty", new ScaleInfo(1), 100) };
            Duration = 10;
            ScaleInfo = new ScaleInfo(1);
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

        public void RefreshReferences()
        {
            foreach (var e in MediaTracks)
            {
                e.ScaleInfo = ScaleInfo;
                PropertyChanged += (s,a) => e.NotifyScaleChanged();
            }
            foreach (var e in Subtitles)
            {
                e.ScaleInfo = ScaleInfo;
                PropertyChanged += (s, a) => e.NotifyScaleChanged();
            }
        }

    }

    public class ScaleInfo
    {
        public int Scale { get; set; }

        public ScaleInfo(int scale)
        {
            Scale = scale;
        }
    }

    public class MediaTrack : TrackInfo 
    {
        public MediaTrack(string path, ScaleInfo scale)
        {
            Path = new Uri(path);
            ConvertedName = Guid.NewGuid().ToString() + ".avi";
            ScaleInfo = scale;
        }
    }

    public class Subtitle : TrackInfo
    {
        public string Content;
        public double HeightShift;
        public Point Pos;
        public double X;
        public double Y;

        public Subtitle(string content, ScaleInfo scale, double leftShift)
        {
            StartSecond = 0;
            EndSecond = 50;
            DurationInSeconds = 500;
            LeftShiftInSeconds = leftShift;
            Content = content;
            ScaleInfo = scale;
        }
    }

    [DataContract]
    public abstract class TrackInfo : NotifierModel
    {
        [DataMember]
        public ScaleInfo ScaleInfo;

        [DataMember]
        public int Scale { 
            get { return ScaleInfo.Scale; } 
            set { ScaleInfo.Scale = value; } }

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

        [DataMember]
        private double leftShiftInSeconds { get; set; }

        [DataMember]
        public double LeftShiftInSeconds { get { return leftShiftInSeconds; } set { leftShiftInSeconds = value; NotifyPropertyChanged(); NotifyPropertyChanged("LeftShiftInPixels"); } }//position of whole track relative to main track

        [DataMember]
        public double LeftShiftInPixels { get { return leftShiftInSeconds * Scale; } set { leftShiftInSeconds = value / Scale; NotifyPropertyChanged(); } }

        [DataMember]
        public double TopShift { get; set; }
     

        public void NotifyScaleChanged()
        {
            NotifyPropertyChanged("DurationInPixels");
            NotifyPropertyChanged("StartPixel");
            NotifyPropertyChanged("EndPixel");          
            NotifyPropertyChanged("LeftShiftInPixels");
            NotifyPropertyChanged("Scale");
        }

    }
}
