using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuto.Model
{
    public class WindowState
    {
        EditorModes currentMode;
        public EditorModes CurrentMode
        {
            get { return currentMode; }
            set
            {
                if (currentMode != value)
                {
                    currentMode = value;
                    if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("CurrentMode"));
                }
            }
        }

        int currentPosition;
        public int CurrentPosition
        {
            get { return currentPosition; }
            set
            {
                if (currentPosition != value)
                {
                    currentPosition = value;
                    if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("CurrentPosition"));
                }
            }
        }


        bool paused;
        public bool Paused
        {
            get { return paused; }
            set
            {
                if (paused != value)
                {
                    paused = value;
                    if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("Paused"));
                }
            }
        }

        double speedRatio;
        public double SpeedRatio
        {
            get { return speedRatio; }
            set
            {
                if (speedRatio != value)
                {
                    speedRatio = value;
                    if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("SpeedRatio"));
                }
            }
        }

        bool faceVideoIsVisible;
        public bool FaceVideoIsVisible
        {
            get { return faceVideoIsVisible; }
            set
            {
                if (faceVideoIsVisible != value)
                {
                    faceVideoIsVisible = value;
                    if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("FaceVideoIsVisible"));
                }
            }
        }

        bool desktopVideoIsVisible;
        public bool DesktopVideoIsVisible
        {
            get { return desktopVideoIsVisible; }
            set
            {
                if (desktopVideoIsVisible != value)
                {
                    desktopVideoIsVisible = value;
                    if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("DesktopVideoIsVisible"));
                }
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        public WindowState()
        {
            Paused = true;
            CurrentMode = EditorModes.General;
            speedRatio = 1;
            FaceVideoIsVisible = DesktopVideoIsVisible = true;
        }
    }
}
