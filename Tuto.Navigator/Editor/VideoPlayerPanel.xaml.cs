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
            ModelView.MouseDown += Timeline_MouseDown;
            Slider.MouseDown += Timeline_MouseDown;
		}


        public event Action<int, MouseButtonEventArgs> TimelineMouseDown;

        void Timeline_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (TimelineMouseDown != null)
            {
                var time = Slider.MsAtPoint(e.GetPosition(Slider));
                TimelineMouseDown(time, e);
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
    }


}
