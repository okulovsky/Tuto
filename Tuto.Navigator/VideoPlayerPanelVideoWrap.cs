using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Tuto.Navigator.Editor;

namespace Tuto.Navigator
{
    public class VideoPlayerPanelVideoWrap : IVideoInterface
    {
        MediaElement wrappedOver;
        public VideoPlayerPanelVideoWrap(MediaElement wrappedOver)
        {
            this.wrappedOver = wrappedOver;
        }

        public int Position
        {
            get
            {
                return (int)wrappedOver.Position.TotalMilliseconds;
            }
            set
            {
                wrappedOver.Position = TimeSpan.FromMilliseconds(value);
            }
        }

        public void SetFile(System.IO.FileInfo location)
        {
            wrappedOver.Source = new Uri(location.FullName);
        }

        public bool Visibility
        {
            get
            {
                return wrappedOver.Visibility == System.Windows.Visibility.Visible;
            }
            set
            {
                wrappedOver.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        public void PlayPause(bool pause)
        {
            if (pause) wrappedOver.Pause();
            else wrappedOver.Play();
        }
    }
}
