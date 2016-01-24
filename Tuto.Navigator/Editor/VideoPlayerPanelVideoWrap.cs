using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;
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
                wrappedOver.Visibility = value?System.Windows.Visibility.Visible :  System.Windows.Visibility.Collapsed;
            }
        }

        public void PlayPause(bool pause)
        {
            if (pause) wrappedOver.Pause();
            else wrappedOver.Play();
        }

        public void Stop()
        {
            try
            {
                wrappedOver.Dispatcher.Invoke(new Action(wrappedOver.Stop));
            }
            catch { }
        }
    }
}
