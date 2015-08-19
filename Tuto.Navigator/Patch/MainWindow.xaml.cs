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
            PreparePatchPicker();
        }

        public void LoadModel(PatchModel model, EditorModel em)
        {
            this.DataContext = model;
            Model = model;
            EModel = em;
            Model.RefreshReferences();
        }



        public void PreparePatchPicker()
        {
            foreach (string s in Directory.GetLogicalDrives())
            {
                TreeViewItem item = new TreeViewItem();
                item.Header = s;
                item.Tag = s;
                item.FontWeight = FontWeights.Normal;
                item.Expanded += new RoutedEventHandler(folder_Expanded);
                PatchPicker.Items.Add(item);
            }
        }

        void folder_Expanded(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = (TreeViewItem)sender;
            if (item.Items.Count == 0)
            {
                item.Items.Clear();
                try
                {
                    foreach (string s in Directory.GetDirectories(item.Tag.ToString()))
                    {
                        TreeViewItem subitem = new TreeViewItem();
                        subitem.Header = s.Substring(s.LastIndexOf("\\") + 1);
                        subitem.Tag = s;
                        subitem.FontWeight = FontWeights.Normal;;
                        subitem.Expanded += new RoutedEventHandler(folder_Expanded);
                        item.Items.Add(subitem);
                    }

                    foreach (string s in Directory.GetFiles(item.Tag.ToString()))
                    {
                        TreeViewItem subitem = new TreeViewItem();
                        subitem.Header = s.Substring(s.LastIndexOf("\\") + 1);
                        subitem.Tag = s;
                        subitem.FontWeight = FontWeights.Normal; ;
                        subitem.Expanded += new RoutedEventHandler(folder_Expanded);
                        item.Items.Add(subitem);
                    }

                }
                catch (Exception) { }
            }
        }


        private int prevoiusTop = 5;
        private DispatcherTimer timer;
        private double mainVideoLength = 0;
        private double volume;
        private TrackInfo currentPatch;
        private Subtitle currentSubtitle;
        private bool isPlaying;
        private bool isLoaded;

        private void Tracks_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                addTrack(files[0]);
                var processMode = EModel.Global.MovePatchOriginInsteadOfCopying;
                var name = System.IO.Path.Combine(EModel.Locations.PatchesDirectory.FullName, new FileInfo(files[0]).Name);
                if (!Directory.Exists(EModel.Locations.PatchesDirectory.FullName))
                    Directory.CreateDirectory(EModel.Locations.PatchesDirectory.FullName);
                if (!File.Exists(name))
                    Program.BatchWorkQueueWindow.Run(new List<BatchWork>() { new MoveCopyWork(files[0], name, processMode) });
            }
        }

        private void addTrack(string path)
        {
            var seconds = ViewTimeline.Position.TotalSeconds;

            var track = new MediaTrack(path, Model.ScaleInfo);
            track.LeftShiftInSeconds = seconds;
            track.TopShift = prevoiusTop;
            track.DurationInPixels = 10;
            Model.PropertyChanged += (s, a) => track.NotifyScaleChanged();
            Model.MediaTracks.Add(track);
            PatchWindow.MediaOpened += SetPatchDuration;
            PatchWindow.Stop();
            PatchWindow.Source = null;
            PatchWindow.Source = new Uri(path);
            PatchWindow.Play(); //need to fire event to get duration
            PatchWindow.Pause();
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
                PatchWindow.Pause();
                isPlaying = false;
                return;
            }
            else if (!isLoaded)
            {
                doInitialLoad();
                return;
            }
            else { ViewTimeline.Play(); PatchWindow.Play(); isPlaying = true; }
        }

        private void SetMainVideo(object s, RoutedEventArgs a)
        {
            Model.Duration = ViewTimeline.NaturalDuration.TimeSpan.TotalSeconds;
            Model.Width = ViewTimeline.NaturalVideoWidth;
            Model.Height = ViewTimeline.NaturalVideoHeight;
            volume = ViewTimeline.Volume != 0 ? ViewTimeline.Volume : volume;
            ViewTimeline.UpdateLayout();
            Model.ActualHeight = ViewTimeline.ActualHeight;
            Model.ActualWidth = ViewTimeline.ActualWidth;
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

        private void CheckSubtitle(double pixelsRelativeToSeconds)
        {
            var subtitleFound = false;
            foreach (var e in Model.Subtitles)
            {
                if (InPatchSection(e, pixelsRelativeToSeconds))
                {
                    subtitleFound = true;
                    if (currentSubtitle == e)
                        break;
                    CurrentSubtitle.Text = e.Content;
                    currentSubtitle = e;
                    Canvas.SetTop(CurrentSubtitle, e.Y);
                    Canvas.SetLeft(CurrentSubtitle, e.X);
                    break;
                }
            }
            if (!subtitleFound)
                CurrentSubtitle.Visibility = System.Windows.Visibility.Collapsed;
            else
                CurrentSubtitle.Visibility = System.Windows.Visibility.Visible;
        }


        private void CheckPlayTime()
        {
            var pixelsRelativeToSeconds = ViewTimeline.Position.TotalSeconds * Model.Scale;
            Canvas.SetLeft(CurrentTime, pixelsRelativeToSeconds);
            CheckSubtitle(pixelsRelativeToSeconds);
            for (var i = Model.MediaTracks.Count - 1; i >= 0; i--)
                if (InPatchSection(Model.MediaTracks[i], pixelsRelativeToSeconds))
                {
                    if (currentPatch == Model.MediaTracks[i])
                        return;

                    currentPatch = Model.MediaTracks[i];
                    PatchWindow.Source = Model.MediaTracks[i].Path;

                    var shift = currentPatch.LeftShiftInPixels;
                    var position = pixelsRelativeToSeconds - shift + currentPatch.StartPixel;
                    PatchWindow.Position = TimeSpan.FromSeconds(position / Model.Scale);
                    PatchWindow.Play();

                    ViewTimeline.Volume = 0;
                    ViewTimeline.Visibility = System.Windows.Visibility.Collapsed;
                    PatchWindow.Visibility = System.Windows.Visibility.Visible;

                    return;
                }
            PatchWindow.Pause();
            PatchWindow.Visibility = System.Windows.Visibility.Collapsed;
            ViewTimeline.Volume = volume;
            ViewTimeline.Visibility = System.Windows.Visibility.Visible;
            currentPatch = null;
        }

        private bool InPatchSection(TrackInfo track, double seconds)
        {
            var leftIn = seconds >= track.LeftShiftInPixels;
            var rightIn = seconds <= track.LeftShiftInPixels - track.StartPixel + track.EndPixel;
            return leftIn && rightIn;
        }

        private void TimeLine_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var pos = e.GetPosition(Tracks);
            var span = TimeSpan.FromSeconds(pos.X / Model.Scale);
            Canvas.SetLeft(CurrentTime, pos.X);
            ViewTimeline.Position = span;
            if (currentPatch != null)
            {
                var shift = currentPatch.LeftShiftInPixels;
                var seconds = ViewTimeline.Position.TotalSeconds * Model.Scale;
                var position = seconds - shift + currentPatch.StartPixel;
                PatchWindow.Position = TimeSpan.FromSeconds(position / Model.Scale);
            }
        }

        private void Patch_Click(object sender, RoutedEventArgs e)
        {
            Program.BatchWorkQueueWindow.Run(new List<BatchWork>() { new PatchWork(Model, true, EModel) });
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
            var slider = (RangeSlider)sender;
            var tr = slider.DataContext as TrackInfo;
            if (tr is MediaTrack)
                Model.MediaTracks.Remove((MediaTrack)tr);
            else
                Model.Subtitles.Remove((Subtitle)tr);
            PatchWindow.Stop();
            PatchWindow.Source = null;
        }

        private void mainwindow_Closing(object sender, CancelEventArgs e)
        {
            //EModel.Save();
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var newScale = (int)e.NewValue;
            if (e.OldValue != 0)
            {
                Model.Scale = newScale;
                SetMainVideo(null, null);
            }
        }

        private bool DragInProgress;
        private Point LastPoint;

        private void Subtitle_MouseUp(object sender, MouseButtonEventArgs e)
        {
            DragInProgress = false;
            var shift = CurrentSubtitle.FontSize;
            currentSubtitle.X = Canvas.GetLeft(CurrentSubtitle);
            currentSubtitle.Y = Canvas.GetTop(CurrentSubtitle);
            var pos = CurrentSubtitle.TranslatePoint(new Point(0, 0), ViewTimeline);
            currentSubtitle.Pos = pos;
            currentSubtitle.HeightShift = shift;
        }

        private void Subtitle_MouseMove(object sender, MouseEventArgs e)
        {
            if (DragInProgress)
            {
                Point point = Mouse.GetPosition(ViewTimeline);
                double offset_x = point.X - LastPoint.X;
                double offset_y = point.Y - LastPoint.Y;
                double new_x = Canvas.GetLeft(CurrentSubtitle);
                double new_y = Canvas.GetTop(CurrentSubtitle);
                new_x += offset_x;
                new_y += offset_y;
                Canvas.SetLeft(CurrentSubtitle, new_x);
                Canvas.SetTop(CurrentSubtitle, new_y);
                LastPoint = point;
            }
        }

        private void Subtitle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            LastPoint = Mouse.GetPosition(ViewTimeline);
            DragInProgress = true;
        }

        private void WrapPanel_RequestBringIntoView(object sender, RequestBringIntoViewEventArgs e)
        {
            e.Handled = true;
        }

        private void mainwindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Model.ActualHeight = ViewTimeline.ActualHeight;
            Model.ActualWidth = ViewTimeline.ActualWidth;
        }

        private void ViewTimeline_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show(CurrentSubtitle.TranslatePoint(new Point(0, 0), ViewTimeline).Y.ToString());
        }

    }
}
