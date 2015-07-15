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
using Tuto.Model;
using Tuto.TutoServices;

namespace Editor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        EditorModel model;



        public MainWindow()

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


            FaceVideo.Source = new Uri(model.Locations.FaceVideo.FullName);
            ScreenVideo.Source = new Uri(model.Locations.DesktopVideo.FullName);
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
                    RunProcess(Services.Montager,model.VideoFolder);
                };

            Assembly.Click += (s, a) =>
                {
                    model.Save();
                    RunProcess(Services.Assembler, model.VideoFolder);
                };

            RepairFace.Click += (s, a) =>
                {
                    model.Save();
                    IsEnabled = false;
                    new Tuto.TutoServices.RepairService().DoWork(model.Locations.FaceVideo,true);
                    IsEnabled = true;
                };

            RepairDesktop.Click += (s, a) =>
            {
                model.Save();
                IsEnabled = false;
                new Tuto.TutoServices.RepairService().DoWork(model.Locations.DesktopVideo,false);
                IsEnabled = true;
            };

            Help.Click += (s, a) =>
                {
                    var data = HelpCreator.CreateModeHelp();
                    var wnd = new HelpWindow();
                    wnd.DataContext = data;
                    wnd.Show();
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
                model.Montage.Information.Episodes[i].Duration = TimeSpan.FromMilliseconds(times[i]);

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


            //var process = new Process();
            //process.StartInfo.FileName = model.Locations.TutoExecutable.FullName;
            //process.StartInfo.Arguments = arguments
            //    .Select(z => z.Contains(" ") ? "\"" + z + "\"" : z)
            //    .Aggregate((a, b) => a + " " + b);
            //process.StartInfo.CreateNoWindow = false;
            //process.Start();

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
            currentMode.MouseClick(time, e);
        }
        #endregion


        /*



        EditorModel editorModel;

        MontageModel model { get { return editorModel.Montage; } }
        
        void SetMode(EditorModes mode)
        {
            editorModel.WindowState.CurrentMode = mode;
            switch (mode)
            {
                case EditorModes.Border: currentMode = new BorderMode(editorModel); break;
                case EditorModes.General: currentMode = new GeneralMode(editorModel); break;
            }
            Timeline.InvalidateVisual();
        }

        internal void Initialize(EditorModel edModel)
        {
            this.editorModel = edModel;


            FaceVideo.LoadedBehavior = MediaState.Manual;
            ScreenVideo.LoadedBehavior = MediaState.Manual;

            MouseDown += Timeline_MouseDown;
            PreviewKeyDown += MainWindow_KeyDown;
            Pause.Click += (a, b) => CmPause();

            var binding = new CommandBinding(Commands.Save);
            binding.Executed += Save;
            CommandBindings.Add(binding);


            Statistics.Click += ShowStatistics;


            SetMode(editorModel.WindowState.CurrentMode);
            switch (editorModel.WindowState.CurrentMode)
            {
                case EditorModes.Border: BordersMode.IsChecked = true; break;
                case EditorModes.General: GeneralMode.IsChecked = true; break;
            }

            BordersMode.Checked += (s, a) => { SetMode(EditorModes.Border); };
            GeneralMode.Checked += (s, a) => { SetMode(EditorModes.General); };

            Synchronizer.Click += (s, a) =>
                {
                    if (model.Shift != 0)
                    {
                        var response = MessageBox.Show("Вы уже синхронизировали это видео. Точно хотите пересинхронизировать?", "", MessageBoxButton.YesNoCancel);
                        if (response != MessageBoxResult.Yes) return;
                    }
                    model.Shift = editorModel.WindowState.CurrentPosition;
                    SetPosition(editorModel.WindowState.CurrentPosition);
                };

            Infos.Click += (s, a) =>
                {
                    if (model.Information.Episodes.Count == 0)
                        for (int i = 0; i < model.Chunks.Count(z => z.StartsNewEpisode); i++)
                            model.Information.Episodes.Add(new EpisodInfo());
                    var wnd = new InfoWindow();
                    wnd.DataContext = model.Information;
                    wnd.ShowDialog();
                };

            var facePath = editorModel.VideoFolder.FullName+"\\face.mp4";
            videoAvailable = File.Exists(facePath);

            FaceVideo.Source = new Uri(facePath, UriKind.Absolute);
            FaceVideo.SpeedRatio = 1;
            FaceVideo.Volume = 0.1;

            ScreenVideo.Source = new Uri(editorModel.VideoFolder.FullName + "\\desktop.avi", UriKind.Absolute);
            ScreenVideo.SpeedRatio = 1;
            ScreenVideo.Volume = 0.0001;
            
            SetPosition(editorModel.WindowState.CurrentPosition);

            Timeline.DataContext = editorModel;

            System.Windows.Forms.Timer t = new System.Windows.Forms.Timer();
            t.Interval = timerInterval;
            t.Tick += (s, a) => { CheckPlayTime(); };
            t.Start();
        }

        public MainWindow()
        {
            InitializeComponent();
          
        }

     
        void Save(object sender, ExecutedRoutedEventArgs e)
        {
            ModelIO.Save(editorModel);
        }

        void ShowStatistics(object sender, EventArgs e)
        {
            var times = new List<int>();
            var current = 0;
            foreach (var c in model.Chunks)
            {
                if (c.StartsNewEpisode)
                {
                    times.Add(current);
                    current = 0;
                }
                if (c.Mode == Mode.Face || c.Mode == Mode.Screen)
                    current += c.Length;
            }
            times.Add(current);
            times.Add(times.Sum());
            var text = times
                .Select(z => TimeSpan.FromMilliseconds(z))
                .Select(z => z.Minutes.ToString() + ":" + z.Seconds.ToString()+"\n")
                .Aggregate((a, b) => a + b);
            System.Windows.MessageBox.Show(text);
        }

        #region Keydown и все, что с ним связано

        void MainWindow_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            var response = currentMode.ProcessKey(e);
            if (response.RequestProcessed)
            {
                ProcessResponse(response);
                e.Handled = true;
                return;
            }
 
            switch (e.Key)
            {
                case Key.Up:
                    ChangeRatio(1.25);
                    e.Handled = true;
                    break;
                case Key.Down:
                    ChangeRatio(0.8);
                    e.Handled = true;
                    break;
                case Key.Space:
                    CmPause();
                    e.Handled = true;
                    break;
            }
        }

        #endregion 

        #region Навигация

        double SpeedRatio = 1;
        bool videoAvailable;
        int timerInterval = 10;
        bool paused = true;
        bool requestPause = false;


        void CmPause()
        {
            ProcessResponse(new WindowCommand { Pause = !paused });
        }

        void MakePause()
        {
            ProcessResponse(new WindowCommand { Pause = true });
        }
        void MakePlay()
        {
            ProcessResponse(new WindowCommand { Pause = false });
        }



        IEditorMode currentMode;// = new BorderMode();





        void ProcessResponse(WindowCommand r)
        {
            if (r.JumpToLocation.HasValue)
            {
                SetPosition(r.JumpToLocation.Value);
                Timeline.InvalidateVisual();
            }
            if (r.Invalidate)            
                Timeline.InvalidateVisual();
            if (r.Pause.HasValue)
            {
                paused = r.Pause.Value;
                if (!paused)
                {
                    FaceVideo.Play();
                    ScreenVideo.Play();
                    Pause.Content = "Pause";
                }
                else
                {
                    FaceVideo.Pause();
                    ScreenVideo.Pause();
                    Pause.Content = "Play";
                }
            }

        }
        void SetPosition(double ms)
        {
            editorModel.WindowState.CurrentPosition = (int)ms;

            if (!paused)
            {
                FaceVideo.Position = TimeSpan.FromMilliseconds(ms);
                ScreenVideo.Position = TimeSpan.FromMilliseconds(ms - model.Shift);
            }
            else
            {
                FaceVideo.Position = TimeSpan.FromMilliseconds(ms);
                ScreenVideo.Position = TimeSpan.FromMilliseconds(ms - model.Shift);
                MakePlay();
                requestPause = true;
            }
        }

        void ChangeRatio(double ratio)
        {
            SpeedRatio *= ratio;
            FaceVideo.SpeedRatio=SpeedRatio;
            ScreenVideo.SpeedRatio = FaceVideo.SpeedRatio;

        }

        void Timeline_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var time = Timeline.MsAtPoint(e.GetPosition(Timeline));
            ProcessResponse(currentMode.MouseClick(time, e));
        }
        #endregion 
         * */
    }
}
