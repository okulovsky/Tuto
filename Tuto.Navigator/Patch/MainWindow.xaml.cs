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
using System.Windows.Forms.Integration;

namespace Tuto.Navigator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    
    public partial class MainWindow : Window, INotifyPropertyChanged
    {

        public PatchModel Model;
        public EditorModel EModel;
        public MainWindow()
        {
            InitializeComponent();
            PatchWindow.LoadedBehavior = MediaState.Manual;
            PreparePatchPicker();
        }

        void WindowState_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Paused") PausedChanged();
        }

        public void LoadModel(PatchModel model, EditorModel em)
        {
            this.DataContext = model;
            Model = model;
            EModel = em;
            Model.RefreshReferences();
            Model.WindowState.PropertyChanged += WindowState_PropertyChanged;
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

        void AddTrackFromPicker(object sender, MouseButtonEventArgs e)
        {
            var fileName = ((TreeViewItem)sender).Tag.ToString();
            addTrack(fileName);
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
                        subitem.MouseDoubleClick += AddTrackFromPicker;
                        item.Items.Add(subitem);
                    }

                }
                catch (Exception) { }
            }
        }


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
            track.TopShift = Top;
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

        DispatcherTimer timer { get; set; }

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
            Model.WindowState.isLoaded = true;
        }

        private void PausedChanged()
        {
            if (Model.WindowState.isPlaying)
            {
                ViewTimeline.Pause();
                PatchWindow.Pause();
                Model.WindowState.isPlaying = false;
                return;
            }
            else if (!Model.WindowState.isLoaded)
            {
                doInitialLoad();
                return;
            }
            else { ViewTimeline.Play(); PatchWindow.Play(); Model.WindowState.isPlaying = true; }
        }

        private void SubtitleProcess(object sender, RoutedEventArgs e)
        {
            AddEmptySubtitle();
        }

        private void AddEmptySubtitle()
        {
            var sub = new Subtitle("Sample Text", Model.ScaleInfo, Canvas.GetLeft(CurrentTime) / Model.Scale);
            Model.PropertyChanged += (s, a) => sub.NotifyScaleChanged();
            Model.Subtitles.Add(sub);
        }

        private void SetMainVideo(object s, RoutedEventArgs a)
        {
            Model.Duration = ViewTimeline.NaturalDuration.TimeSpan.TotalSeconds;
            Model.Width = ViewTimeline.NaturalVideoWidth;
            Model.Height = ViewTimeline.NaturalVideoHeight;
            Model.WindowState.volume = ViewTimeline.Volume != 0 ? ViewTimeline.Volume : Model.WindowState.volume;
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
                    if (Model.WindowState.currentSubtitle == e)
                        break;
                    Model.WindowState.currentSubtitle = e;
                    Canvas.SetTop(CurrentSubtitleWraper, e.Y);
                    Canvas.SetLeft(CurrentSubtitleWraper, e.X);
                    break;
                }
            }
            if (!subtitleFound)
                CurrentSubtitleWraper.Visibility = System.Windows.Visibility.Collapsed;
            else
                CurrentSubtitleWraper.Visibility = System.Windows.Visibility.Visible;
        }


        private void CheckPlayTime()
        {
            var pixelsRelativeToSeconds = ViewTimeline.Position.TotalSeconds * Model.Scale;
            Canvas.SetLeft(CurrentTime, pixelsRelativeToSeconds);
            CheckSubtitle(pixelsRelativeToSeconds);
            for (var i = Model.MediaTracks.Count - 1; i >= 0; i--)
                if (InPatchSection(Model.MediaTracks[i], pixelsRelativeToSeconds))
                {
                    if (Model.WindowState.currentPatch == Model.MediaTracks[i])
                        return;

                    Model.WindowState.currentPatch = Model.MediaTracks[i];
                    PatchWindow.Source = Model.MediaTracks[i].Path;

                    var shift = Model.WindowState.currentPatch.LeftShiftInPixels;
                    var position = pixelsRelativeToSeconds - shift + Model.WindowState.currentPatch.StartPixel;
                    PatchWindow.Position = TimeSpan.FromSeconds(position / Model.Scale);
                    PatchWindow.Play();

                    ViewTimeline.Volume = 0;
                    ViewTimeline.Visibility = System.Windows.Visibility.Hidden;
                    PatchWindow.Visibility = System.Windows.Visibility.Visible;

                    return;
                }
            PatchWindow.Pause();
            PatchWindow.Visibility = System.Windows.Visibility.Collapsed;
            ViewTimeline.Volume = Model.WindowState.volume;
            ViewTimeline.Visibility = System.Windows.Visibility.Visible;
            Model.WindowState.currentPatch = null;
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
            if (Model.WindowState.currentPatch != null)
            {
                var shift = Model.WindowState.currentPatch.LeftShiftInPixels;
                var seconds = ViewTimeline.Position.TotalSeconds * Model.Scale;
                var position = seconds - shift + Model.WindowState.currentPatch.StartPixel;
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


        private void Subtitle_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Model.WindowState.DragInProgress = false;
            var shift = CurrentSubtitle.FontSize;
            Model.WindowState.currentSubtitle.X = Canvas.GetLeft(CurrentSubtitleWraper);
            Model.WindowState.currentSubtitle.Y = Canvas.GetTop(CurrentSubtitleWraper);
            var pos = CurrentSubtitleWraper.TranslatePoint(new Point(0, 0), ViewTimeline);
            Model.WindowState.currentSubtitle.Pos = pos;
            Model.WindowState.currentSubtitle.HeightShift = shift;
        }


        private bool IsInVideoField(double offsetX, double offsetY)
        {
            var v = ViewTimeline;
            var s = CurrentSubtitleWraper;
            var posWrap = CurrentSubtitleWraper.TranslatePoint(new Point(0, 0), Clips);
            var posClip = ViewTimeline.TranslatePoint(new Point(0, 0), Clips);
            var VideoBoundingBox = new System.Drawing.Rectangle((int)posClip.X, (int)posClip.Y, (int)v.ActualWidth, (int)v.ActualHeight);
            var SubBox = new System.Drawing.Rectangle((int)(posWrap.X + offsetX), (int)(posWrap.Y + offsetY), (int)s.ActualWidth, (int)s.ActualHeight);
            return VideoBoundingBox.Contains(SubBox);


        }

        private void Subtitle_MouseMove(object sender, MouseEventArgs e)
        {
            if (Model.WindowState.DragInProgress)
            {
                Point point = Mouse.GetPosition(ViewTimeline);
                double offset_x = point.X - Model.WindowState.LastPoint.X;
                double offset_y = point.Y - Model.WindowState.LastPoint.Y;
                double new_x = Canvas.GetLeft(CurrentSubtitleWraper);
                double new_y = Canvas.GetTop(CurrentSubtitleWraper);
                new_x += offset_x;
                new_y += offset_y;
                if (IsInVideoField(offset_x, offset_y))
                {
                    Canvas.SetLeft(CurrentSubtitleWraper, new_x);
                    Canvas.SetTop(CurrentSubtitleWraper, new_y);
                    Model.WindowState.LastPoint = point;
                }
            }
        }

        private void Subtitle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount < 2)
            {
                Model.WindowState.LastPoint = Mouse.GetPosition(ViewTimeline);
                Model.WindowState.DragInProgress = true;
            }

            else
            {
                var m = new SubtitleEditor();
                m.DataContext = Model.WindowState.currentSubtitle;
                m.ShowDialog();
            }
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


        public event PropertyChangedEventHandler PropertyChanged;
    }
}
