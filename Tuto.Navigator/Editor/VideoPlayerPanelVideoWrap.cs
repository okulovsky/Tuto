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
            wrappedOver.LoadedBehavior = MediaState.Manual;
            Loaded=false;
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
            Loaded = true;
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
                wrappedOver.Visibility = value?System.Windows.Visibility.Visible :  System.Windows.Visibility.Hidden;
            }
        }

        bool paused = true;

        public bool Paused
        {
            get { return paused; }
            set
            {
                if (value)
                    wrappedOver.Pause();
                else
                    wrappedOver.Play();
                paused = value;
            }
        }

        public bool Loaded { get; private set; }

        public void Die()
        {
            try
            {
                paused = true;
                Loaded = false;
                wrappedOver.Dispatcher.Invoke(new Action(wrappedOver.Stop));
            }
            catch { }
        }

        public int GetDuration()
        {
            if (!wrappedOver.NaturalDuration.HasTimeSpan)
                return -1;
            return (int)wrappedOver.NaturalDuration.TimeSpan.TotalMilliseconds;
        }
    }
}
