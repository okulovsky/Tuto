using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Tuto.Model;
using Tuto.Navigator.ViewModels;

namespace Tuto.Navigator.Editor
{
	/// <summary>
	/// Interaction logic for VideoPlayerPanel.xaml
	/// </summary>
	public partial class VideoPlayerPanel : UserControl, IEditorInterface
	{
		public VideoPlayerPanel()
		{
			InitializeComponent();
            
            Face = new VideoPlayerPanelVideoWrap(FaceVideo);
            Desktop = new VideoPlayerPanelVideoWrap(DesktopVideo);
            Patch = new VideoPlayerPanelVideoWrap(PatchContainer.VideoPatch.patch);
            ModelView.TimeSelected += OnTimeSelected;
            Slider.TimeSelected += OnTimeSelected;
            PatchView.TimeSelected += OnTimeSelected;
            DesktopVideo.Volume = 0;
			MouseDown+=(s,a)=>Focus();
			KeyDown += VideoPlayerPanel_KeyDown;
            var timer = new System.Windows.Threading.DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(500);
            timer.Tick += (_, __) => SetPatchUISize();
            timer.Start();
		}

        void SetPatchUISize()
        {
            PatchContainer.Container.Width = FaceVideo.ActualWidth;
            PatchContainer.Container.Height = FaceVideo.ActualHeight;
   
        }

        void OnTimeSelected(int ms, bool alt)
        {
            if (TimeSelected != null)
                TimeSelected(ms, alt);
        }


		void VideoPlayerPanel_KeyDown(object sender, KeyEventArgs e)
		{

			if (ControlKeyDown != null)
				ControlKeyDown(sender, e);
		}

        void SetBothMode(int faceColumn, int faceSize)
        {
            Grid.SetColumn(FaceVideo, faceColumn);
            videoGrid.ColumnDefinitions[faceColumn].Width = new GridLength(faceSize, GridUnitType.Star);

            Grid.SetColumn(DesktopVideo,1- faceColumn);
            videoGrid.ColumnDefinitions[1-faceColumn].Width = new GridLength(100 - faceSize, GridUnitType.Star);

        }

        public void SetArrangeMode(ArrangeModes mode)
        {
            switch(mode)
            {
                case ArrangeModes.BothEqual:
                    SetBothMode(0,50);
                    break;
                case ArrangeModes.BothDesktopBigger:
                    SetBothMode(1,25);
                    break;
                case ArrangeModes.BothFaceBigger:
                    SetBothMode(0,75);
                    break;
                case ArrangeModes.Overlay:
                case ArrangeModes.Patching:
                    Grid.SetColumn(DesktopVideo, 0);
                    Grid.SetColumn(FaceVideo, 0);
                    videoGrid.ColumnDefinitions[0].Width = new GridLength(100, GridUnitType.Star);
                    videoGrid.ColumnDefinitions[1].Width = new GridLength(0, GridUnitType.Star);
                    break;
            }

            
            bool patching = mode == ArrangeModes.Patching;
            PatchContainer.Visibility = patching ? Visibility.Visible : Visibility.Hidden;
            PatchView.Visibility = patching ? Visibility.Visible : Visibility.Hidden;
            
        }


        public void Refresh()
        {
            InvalidateVisual();
            PatchView.InvalidateVisual();
        }

        public event Action<int, bool> TimeSelected;

        void Timeline_MouseDown(int ms, bool e)
        {
            if (TimeSelected != null)
            {
                 TimeSelected(ms, e);
            }
        }

        public void SetPatch(PatchData patch)
        {
            PatchContainer.Subtitles.Visibility = patch is SubtitlePatch ? Visibility.Visible : Visibility.Hidden;
            PatchContainer.Subtitles.DataContext = patch as SubtitlePatch;

            PatchContainer.VideoPatch.Visibility = patch is VideoPatchBase ? Visibility.Visible : Visibility.Hidden;
            PatchContainer.VideoPatch.DataContext = patch as VideoPatchBase;

            PatchContainer.Image.Visibility = patch is ImagePatch ? Visibility.Visible : Visibility.Hidden;
            PatchContainer.Image.DataContext = patch as ImagePatch;
            if (patch is ImagePatch)
            {
                var p = patch as ImagePatch;
                var model = (EditorModel)DataContext;
                var videotheque = model.Videotheque;
                var path = System.IO.Path.Combine(videotheque.PatchFolder.FullName, p.RelativeFilePath);
                var uri = new Uri(path);
                PatchContainer.Image.image.Source = new BitmapImage(uri);
            }
            
        }

        public void SetRatio(double ratio)
        {
            FaceVideo.SpeedRatio = ratio;
            DesktopVideo.SpeedRatio = ratio;
        }

        public IVideoInterface Face
        {
            get;
            private set;
        }

        public IVideoInterface Desktop
        {
            get;
            private set;
        }

        public IVideoInterface Patch
        {
            get;
            private set;
        }

        public event KeyEventHandler ControlKeyDown;
    }


}
