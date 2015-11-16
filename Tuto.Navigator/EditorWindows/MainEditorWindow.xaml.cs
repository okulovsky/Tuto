using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Editor.Windows;
using Tuto;
using Tuto.TutoServices;
using Tuto.Navigator;
using Tuto.BatchWorks;
using System.ComponentModel;
using Tuto.Model;

namespace Editor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainEditorWindow : Window
    {
        private double currentTime;
        EditorModel model;

        public MainEditorWindow()

        {
            Loaded += MainWindow_Initialized;
            InitializeComponent();
            FaceVideo.LoadedBehavior = MediaState.Manual;
            ScreenVideo.LoadedBehavior = MediaState.Manual;

        }

        void MainWindow_Initialized(object sender, EventArgs e)
        {
            model = (EditorModel)DataContext;
            model.WindowState.PropertyChanged += WindowState_PropertyChanged;

            var face = model.Locations.FaceVideoThumb.Exists ? model.Locations.FaceVideoThumb : model.Locations.FaceVideo;
            var desk = model.Locations.DesktopVideoThumb.Exists ? model.Locations.DesktopVideoThumb : model.Locations.DesktopVideo;
            FaceVideo.Source = new Uri(face.FullName);
            ScreenVideo.Source = new Uri(desk.FullName);

            FaceVideo.LoadedBehavior = MediaState.Manual;
            ScreenVideo.LoadedBehavior = MediaState.Manual;
			ScreenVideo.Volume = 0;

            
            ModeChanged();
            PositionChanged();
            PausedChanged();
            RatioChanged();
            videoAvailable = model.Locations.FaceVideo.Exists;


            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(timerInterval);
            timer.Tick += (s, a) => { CheckPlayTime(); };
            timer.Start();

            PreviewKeyDown += MainWindow_KeyDown;
            ModelView.MouseDown += Timeline_MouseDown;
            Slider.MouseDown += Timeline_MouseDown;


            Save.Click += (s, a) =>
            {
                model.Save();
            };


            Montage.Click += (s, a) =>
                {
                    model.Save();
                    var task = new ConvertDesktopWork(model, true);
                    Program.BatchWorkQueueWindow.Run(new List<BatchWork>(){task});
                    var task2 = new ConvertFaceWork(model, true);
                    Program.BatchWorkQueueWindow.Run(new List<BatchWork>() { task2 });
                };

            Assembly.Click += (s, a) =>
                {
                    model.Save();
                    var tasks = new List<BatchWork>();
                    tasks.Add(new AssemblyVideoWork(model, model.Global.CrossFadesEnabled));
                    Program.BatchWorkQueueWindow.Run(tasks);
                };

            RepairFace.Click += (s, a) =>
                {
                    model.Save();
                    var task = new RepairVideoWork(model, model.Locations.FaceVideo, true);
                    Program.BatchWorkQueueWindow.Run(new List<BatchWork> { task });
                };

            NoiseReduction.Click += (s, a) =>
            {
               model.Save();
                var task = new CreateCleanSoundWork(model.Locations.FaceVideo, model, true);
                Program.BatchWorkQueueWindow.Run(new List<BatchWork> { task });
            };

            RepairDesktop.Click += (s, a) =>
            {
                model.Save();
                var task = new RepairVideoWork(model, model.Locations.DesktopVideo, true);
                Program.BatchWorkQueueWindow.Run(new List<BatchWork> { task });
            };


            ThumbFace.Click += (s, a) =>
                {
                    model.Save();
                        var task = new CreateThumbWork(model.Locations.FaceVideo, model, true);
                        Program.BatchWorkQueueWindow.Run(new List<BatchWork> { task });
                        task.TaskFinished += (z, x) =>
                        {
                            Action t = () => { FaceVideo.Source = new Uri(((CreateThumbWork)z).ThumbName.FullName); };
                            this.Dispatcher.BeginInvoke((Delegate)t);
                        };
                };

            
            ThumbDesktop.Click += (s, a) =>
                {
                    model.Save();
                        var task = new CreateThumbWork(model.Locations.DesktopVideo, model, true);
                        Program.BatchWorkQueueWindow.Run(new List<BatchWork> { task });
                        task.TaskFinished += (z, x) =>
                            {
                                Action t = () => { ScreenVideo.Source = new Uri(((CreateThumbWork)z).ThumbName.FullName); };
                                this.Dispatcher.BeginInvoke((Delegate)t);
                            };
                };

            Help.Click += (s, a) =>
                {
                    var data = HelpCreator.CreateModeHelp();
                    var wnd = new HelpWindow();
                    wnd.DataContext = data;
                    wnd.Show();
                };

            Patch.Click += (s, a) =>
            {
                //FOR DEBUGGING!
                var patchWindow = new Tuto.Navigator.MainWindow();
                var serv = new AssemblerService(false);
                var episodeNumber = 0;
                var eps = model.Montage.Information.Episodes;
                foreach (var c in model.Montage.Chunks)
                {             
                    if (c.StartsNewEpisode)
                        episodeNumber++;
                    if (c.StartTime >= currentTime)
                        break;
                }
                var videoFile = model.Locations.GetOutputFile(episodeNumber);
                if (videoFile.Exists)
                {
                    var temp = model.Montage.Information.Episodes[episodeNumber];
                    var m = temp.PatchModel != null ? temp.PatchModel : new PatchModel(videoFile.FullName);
                    temp.PatchModel = m;
                    patchWindow.LoadModel(m, model);
                    patchWindow.Show();
                }
                else Program.BatchWorkQueueWindow.Run(new List<BatchWork>() { new AssemblyVideoWork(model, model.Global.CrossFadesEnabled) });
            };
            GoTo.Click += (s, a) =>
                {
                    var wnd = new FixWindow();
                    wnd.Title = "Enter time";
                    var result = wnd.ShowDialog();
                    if (!result.HasValue || !result.Value) return;
                    var parts = wnd.Text.Text.Split(',', '.', '-', ' ');
                    int time = 0;
                    try
                    {
                        time = int.Parse(parts[0]) * 60 + int.Parse(parts[1]);
                    }
                    catch
                    {
                        MessageBox.Show("Incorrect string. Expected format is '5-14'");
                        return;
                    }
                    time *= 1000;
                    int current = 0;
                    foreach (var z in model.Montage.Chunks)
                    {
                        if (z.IsNotActive) time += z.Length;
                        current += z.Length;
                        if (current > time) break;
                    }
                    model.WindowState.CurrentPosition = time;
                };

            Synchronize.Click += Synchronize_Click;
            Infos.Click += Infos_Click;
            AddDuringTasks(); 
        }

        void AddDuringTasks()
        {
            var toDo = model.Global.WorkSettings.GetDuringWorks(model);
            foreach (var t in toDo)
            {
                if (t is CreateThumbWork && ((CreateThumbWork)t).Source == model.Locations.DesktopVideo)
                    t.TaskFinished += (z, x) =>
                    {
                        Action act = () => { ScreenVideo.Source = new Uri((string)z); };
                        this.Dispatcher.BeginInvoke((Delegate)act);
                    };

                if (t is CreateThumbWork && ((CreateThumbWork)t).Source == model.Locations.FaceVideo)
                    t.TaskFinished += (z, x) =>
                    {
                        Action act = () => { FaceVideo.Source = new Uri((string)z); };
                        this.Dispatcher.BeginInvoke((Delegate)act);
                    };
            }
            if (toDo.Count != 0 )
                Program.BatchWorkQueueWindow.Run(toDo);
        }

        void Synchronize_Click(object sender, RoutedEventArgs e)
        {
            if (model.Montage.SynchronizationShift != 0)
            {
                var response = MessageBox.Show("Вы уже синхронизировали это видео. Точно хотите пересинхронизировать?", "", MessageBoxButton.YesNoCancel);
                if (response != MessageBoxResult.Yes) return;
            }
            model.Montage.SynchronizationShift = model.WindowState.CurrentPosition;
            model.WindowState.CurrentPosition = model.WindowState.CurrentPosition + 1;
            model.Save();
        }

       
        void Infos_Click(object sender, RoutedEventArgs e)
        {
            var times = new List<int>();
            var current = 0;
            foreach (var c in model.Montage.Chunks)
            {
                if (c.StartsNewEpisode)
                {
                    times.Add(current);
                    current = 0;
                }
                if (c.Mode == Mode.Face || c.Mode == Mode.Desktop)
                    current += c.Length;
            }
            times.Add(current);
            if (model.Montage.Information.Episodes.Count == 0)
            {
                model.Montage.Information.Episodes.AddRange(Enumerable.Range(0, times.Count).Select(z => new EpisodInfo(Guid.NewGuid())));
            }
            else if (model.Montage.Information.Episodes.Count != times.Count)
            {
                while (model.Montage.Information.Episodes.Count > times.Count)
                    model.Montage.Information.Episodes.RemoveAt(model.Montage.Information.Episodes.Count - 1);
                while (model.Montage.Information.Episodes.Count < times.Count)
                    model.Montage.Information.Episodes.Add(new EpisodInfo(Guid.NewGuid()));
            }

            for (int i = 0; i < times.Count; i++)
            {
                model.Montage.Information.Episodes[i].Duration = TimeSpan.FromMilliseconds(times[i]);
            }


            var wnd = new InfoWindow();
            wnd.DataContext = model.Montage.Information;
            wnd.ShowDialog();
            model.Save();
        }



        #region Реакция на изменение полей модели

        void PausedChanged()
        {
            if (model.WindowState.Paused)
            {
                FaceVideo.Pause();
                ScreenVideo.Pause();
       //         MessageBox.Show("Paused");
            }
            else
            {
                FaceVideo.Play();
                ScreenVideo.Play();
       //         MessageBox.Show("Played");
            }
        }

        void ModeChanged()
        {
            if (model.WindowState.CurrentMode == EditorModes.Border)
                currentMode = new BorderMode(model);
            if (model.WindowState.CurrentMode == EditorModes.General)
                currentMode = new GeneralMode(model);
            if (model.WindowState.CurrentMode == Tuto.Model.EditorModes.Fixes)
                currentMode = new FixesMode(model);

        }

        void RatioChanged()
        {
            FaceVideo.SpeedRatio = model.WindowState.SpeedRatio;
            ScreenVideo.SpeedRatio = model.WindowState.SpeedRatio;
        }

        bool pauseRequested;

        bool supressPositionChanged;

        void PositionChanged()
        {
            if (supressPositionChanged) return;

            if (model.WindowState.Paused)
            {
                model.WindowState.Paused = false;
                pauseRequested = true;
            }

            FaceVideo.Position = TimeSpan.FromMilliseconds(model.WindowState.CurrentPosition);
            ScreenVideo.Position = TimeSpan.FromMilliseconds(model.WindowState.CurrentPosition - model.Montage.SynchronizationShift);
        }


        void WindowState_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Paused") PausedChanged();
            if (e.PropertyName == "CurrentMode") ModeChanged();
            if (e.PropertyName == "SpeedRatio") RatioChanged();
            if (e.PropertyName == "CurrentPosition") PositionChanged();

            if (e.PropertyName == "FaceVideoIsVisible" || e.PropertyName == "DesktopVideoIsVisible")
            {
                FaceVideo.Visibility = model.WindowState.FaceVideoIsVisible ? Visibility.Visible : Visibility.Collapsed;
                ScreenVideo.Visibility = model.WindowState.DesktopVideoIsVisible? Visibility.Visible : Visibility.Collapsed;
            }

            if (e.PropertyName == "CurrentSubtitle")
            {

                Subtitles.Text = model.WindowState.CurrentSubtitle;
            }
        }



        void RunProcess(Services service, DirectoryInfo directory)
        {
            this.IsEnabled = false;
            Tuto.TutoProgram.Run(service, directory, ExecMode.Run);
            this.IsEnabled = true;
            MessageBox.Show("The action is complete");
            return;

        }



        #endregion

        #region Взаимодействие с контроллером

        int timerInterval = 10;
        bool videoAvailable;

        IEditorMode currentMode;

        void CheckPlayTime()
        {
            supressPositionChanged = true;
            if (videoAvailable)
                model.WindowState.CurrentPosition = (int)FaceVideo.Position.TotalMilliseconds;
            else
            {
                if (!model.WindowState.Paused)
                    model.WindowState.CurrentPosition += (int)(timerInterval * model.WindowState.SpeedRatio);
            }
            supressPositionChanged = false;

            if (pauseRequested)
            {
                model.WindowState.Paused = true;
                pauseRequested = false;
                return;
            }

            if (model.WindowState.Paused) return;

            currentMode.CheckTime();
           
        }

        void MainWindow_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.S)
            {
                model.Save();
                return;
            }
            
           currentMode.ProcessKey(KeyMap.KeyboardCommandData(e));
        }

        void Timeline_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var time = Slider.MsAtPoint(e.GetPosition(Slider));
            currentTime = time;
            currentMode.MouseClick(time, e);
        }
        #endregion

        private void Tools_Click(object sender, RoutedEventArgs e)
        {
            var mode = this.ToolsPanel.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
            this.ToolsPanel.Visibility = mode;
        }
    }
}
