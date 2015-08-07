using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Tuto.Navigator;
using System.IO;
using System.Threading;
using System.Windows.Threading;
using System.ComponentModel;
using System.Collections.ObjectModel;
using Tuto.Model;
using Tuto.BatchWorks;

namespace Tuto.Navigator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {

        public PatchModel Model;
        public EditorModel EModel;
        public MainWindow()
        {
            InitializeComponent();
            PatchWindow.LoadedBehavior = MediaState.Manual;
        }

        public void LoadModel(PatchModel model, EditorModel em)
        {
            this.DataContext = model;
            Model = model;
            EModel = em;
        }

        private int prevoiusTop = 5;
        private DispatcherTimer timer;
        private int trackHeight = 30;
        private double mainVideoLength = 0;
        private double volume;
        private TrackInfo currentPatch;
        private bool isPlaying;
        private bool isLoaded;

        private void Tracks_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                addTrack(files[0]);
            }
        }

        private void addTrack(string path)
        {
            var seconds = ViewTimeline.Position.TotalSeconds;

            var track = new TrackInfo(path);
            track.LeftShift = seconds;
            track.TopShift = prevoiusTop;
            track.DurationInSeconds = 10;

            Model.MediaTracks.Add(track);

            PatchWindow.MediaOpened += SetPatchDuration;
            PatchWindow.Stop();
            PatchWindow.Source = null;
            PatchWindow.Source = new Uri(path);
            PatchWindow.Play(); //need to fire event to get duration
            PatchWindow.Pause();

            //prevoiusTop += 30;
            //TimeScroll.Height += trackHeight;
            //mainwindow.Height += trackHeight;
            //CurrentTime.Height += trackHeight;
        }

        private void doInitialLoad()
        {
            ViewTimeline.Source = new Uri(Model.SourceInfo.FullName);
            ViewTimeline.LoadedBehavior = MediaState.Manual;

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(10);
            timer.Tick += (s, a) => { CheckPlayTime(); };
            ViewTimeline.MediaOpened += SetMainVideo;
            ViewTimeline.Play();
            ViewTimeline.Pause();
            isLoaded = true;
        }
        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            if (isPlaying)
            {
                ViewTimeline.Pause();
                isPlaying = false;
                return;
            }
            else if (!isLoaded)
            {
                doInitialLoad();
                return;
            }
            else { ViewTimeline.Play(); isPlaying = true; }
        }

        private void SetMainVideo(object s, RoutedEventArgs a)
        {
            double length = 0;
            length = ViewTimeline.NaturalDuration.TimeSpan.TotalSeconds;
            Model.Duration = length;
            mainVideoLength = length;
            mainSlider.Maximum = length;
            mainSlider.Minimum = 0;
            mainSlider.Width = length;
            Tracks.Width = length;
            TimeLine.Width = length;
            mainSlider.UpperValue = length;
            mainSlider.Minimum = 0;
            volume = ViewTimeline.Volume;
            timer.Start();
            ViewTimeline.MediaOpened -= SetMainVideo; //should be once
        }

        private void SetPatchDuration(object s, RoutedEventArgs a)
        {
            var track = Model.MediaTracks.Last();
            var elem = (MediaElement)s;
            var duration = elem.NaturalDuration.TimeSpan.TotalSeconds;
            track.StartSecond = 0;
            track.EndSecond = duration;
            track.DurationInSeconds = duration;
            PatchWindow.MediaOpened -= SetPatchDuration; //should be once
        }



        private void CheckPlayTime()
        {
            var seconds = ViewTimeline.Position.TotalSeconds;
            Canvas.SetLeft(CurrentTime, seconds);
            for (var i = Model.MediaTracks.Count - 1; i >= 0; i--)
                if (InPatchSection(Model.MediaTracks[i], seconds))
                {
                    if (currentPatch == Model.MediaTracks[i])
                        return;

                    currentPatch = Model.MediaTracks[i];
                    PatchWindow.Source = Model.MediaTracks[i].Path;

                    var shift = currentPatch.LeftShift;
                    var position = seconds - (shift);
                    PatchWindow.Position = TimeSpan.FromSeconds(position);
                    PatchWindow.Play();

                    ViewTimeline.Volume = 0;
                    ViewTimeline.Visibility = System.Windows.Visibility.Collapsed;

                    return;
                }
            PatchWindow.Pause();
            ViewTimeline.Volume = volume;
            ViewTimeline.Visibility = System.Windows.Visibility.Visible;
            currentPatch = null;
        }

        private bool InPatchSection(TrackInfo track, double seconds)
        {
            var leftIn = seconds >= track.LeftShift + track.StartSecond;
            var rightIn = seconds <= track.LeftShift + track.EndSecond;
            return leftIn && rightIn;
        }

        private void TimeLine_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var pos = e.GetPosition(Tracks);
            var span = TimeSpan.FromSeconds(pos.X);
            Canvas.SetLeft(CurrentTime, pos.X);
            ViewTimeline.Position = span;
            if (currentPatch != null)
            {
                var shift = currentPatch.LeftShift;
                var seconds = ViewTimeline.Position.TotalSeconds;
                var position = seconds - shift;
                PatchWindow.Position = TimeSpan.FromSeconds(position);
            }
        }

        private void Patch_Click(object sender, RoutedEventArgs e)
        {
            Program.BatchWorkQueueWindow.Run(new List<BatchWork>(){new PatchWork(Model, true, EModel)});
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
           EModel.Save();
        }

        private void mainwindow_Loaded(object sender, RoutedEventArgs e)
        {
            doInitialLoad();
        }


        private void RangeSlider_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var pos = e.GetPosition(Tracks).X;
            for (var i = 0; i < Model.MediaTracks.Count; )
            {
                var track = Model.MediaTracks[i];
                if (track.LeftShift + track.StartSecond <= pos && track.LeftShift + track.EndSecond >= pos)
                {
                    Model.MediaTracks.RemoveAt(i);
                    return;
                }
            }
        }
    }
}
