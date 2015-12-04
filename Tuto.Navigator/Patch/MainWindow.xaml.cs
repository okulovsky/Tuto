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
            Refresh.Click += (s, a) => { PreparePatchPicker(); };
            DecSpace.Click += (s, a) => { model.WorkspaceWidth -= 100; };
            IncSpace.Click += (s, a) => { model.WorkspaceWidth += 100; };
            PreparePatchPicker();
            PrepareTutoPatchPicker();
        }

        public void PrepareTutoPatchPicker()
        {
            foreach (var m in EModel.Videotheque.EditorModels)
            {
                for (var i = 0; i < m.Montage.Information.Episodes.Count; i++)
                {
                    var epInfo = m.Montage.Information.Episodes[i];
                    if (epInfo.OutputType != OutputTypes.Patch)
                        continue;
                    var item = new ListViewItem();
                    item.Content = epInfo.Name;
                    item.Tag = Tuple.Create(m, i);
                    item.MouseDoubleClick += TutoPatchItemClick;
                    TutoPatchesList.Items.Add(item);
                }
            }
        }


        public void TutoPatchItemClick(object sender, MouseButtonEventArgs e)
        {
            var item = sender as ListViewItem;
            var info = (Tuple<EditorModel, int>)item.Tag;
            var model = info.Item1;
            var index = info.Item2;
            var epInfo = model.Montage.Information.Episodes[index];
            var assembledName = model.Locations.GetOutputFile(epInfo).FullName;
            if (File.Exists(assembledName))
                addTrack(assembledName);
            else
            {
                var work = new AssemblyVideoWork(model);
                work.TaskFinished += (s, a) => { Dispatcher.Invoke(() => addTrack(assembledName));};
                Program.WorkQueue.Run(work);
            }

        }

        public void PreparePatchPicker()
        {
            PatchPicker.Items.Clear();
            if (!EModel.Locations.PatchesDirectory.Exists)
                EModel.Locations.PatchesDirectory.Create();
            Model.WorkspaceWidth = Model.WorkspaceWidth == 0 ? Model.Duration * scaleSlider.Maximum : Model.WorkspaceWidth;
            foreach (var f in EModel.Locations.PatchesDirectory.GetFiles())
            {
                var item = new ListViewItem();
                item.Content = f.Name;
                item.Tag = f.FullName;
                item.MouseDoubleClick += patchItemClick;
                PatchPicker.Items.Add(item);
            }
        }

        void patchItemClick(object sender, MouseButtonEventArgs e)
        {
            var item = sender as ListViewItem;
            if (item != null)
            {
                addTrack(item.Tag.ToString());
            }
        }

        private void addTrack(string path)
        {
            var seconds = Model.WindowState.TimeSet;
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
            timer.Interval = TimeSpan.FromMilliseconds(100);
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
            if (Model.WindowState.isPlaying)
                Model.WindowState.TimeSet += 0.1;
            var pixelsRelativeToSeconds = Model.WindowState.TimeSet * Model.Scale;
            CheckSubtitle(pixelsRelativeToSeconds);
            for (var i = Model.MediaTracks.Count - 1; i >= 0; i--)
                if (InPatchSection(Model.MediaTracks[i], pixelsRelativeToSeconds))
                {
                    ViewTimeline.Pause();
                    Canvas.SetLeft(CurrentTime, Model.WindowState.TimeSet * Model.Scale);
                    if (Model.WindowState.currentPatch == Model.MediaTracks[i])
                        return;

                    Model.WindowState.currentPatch = Model.MediaTracks[i];
                    PatchWindow.Source = Model.MediaTracks[i].Path;

                    var shift = Model.WindowState.currentPatch.LeftShiftInPixels;
                    var position = pixelsRelativeToSeconds - shift + Model.WindowState.currentPatch.StartPixel;
                    PatchWindow.Position = TimeSpan.FromSeconds(position / Model.Scale);
                    if (Model.WindowState.isPlaying)
                        PatchWindow.Play();

                    ViewTimeline.Volume = 0;
                    ViewTimeline.Visibility = System.Windows.Visibility.Hidden;
                    PatchWindow.Visibility = System.Windows.Visibility.Visible;
                    Canvas.SetLeft(CurrentTime, Model.WindowState.TimeSet * Model.Scale);
                    return;
                }
            PatchWindow.Pause();
            if (Model.WindowState.isPlaying)
                ViewTimeline.Play();
            PatchWindow.Visibility = System.Windows.Visibility.Collapsed;
            ViewTimeline.Volume = Model.WindowState.volume;
            ViewTimeline.Visibility = System.Windows.Visibility.Visible;
            Model.WindowState.currentPatch = null;
            Canvas.SetLeft(CurrentTime, Model.WindowState.TimeSet * Model.Scale);
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
            Model.WindowState.TimeSet = span.TotalSeconds;
            ViewTimeline.Position = span;
            if (Model.WindowState.currentPatch != null)
            {
                var shift = Model.WindowState.currentPatch.LeftShiftInPixels;
                var seconds = Model.WindowState.TimeSet * Model.Scale;
                var position = seconds - shift + Model.WindowState.currentPatch.StartPixel;
                PatchWindow.Position = TimeSpan.FromSeconds(position / Model.Scale);
            }
        }

        private void Patch_Click(object sender, RoutedEventArgs e)
        {
            Program.WorkQueue.Run( new PatchWork(Model, true, EModel, true));
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
            Model.FontCoefficent = (Model.Width / Model.ActualWidth + Model.Height / Model.ActualHeight) / 2;
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
