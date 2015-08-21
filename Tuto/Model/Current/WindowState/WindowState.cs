using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Tuto.Model
{
    [DataContract]
    public class   WindowState
    {
        [DataMember]
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
                    if (CurrentModeChanged != null) CurrentModeChanged(this, EventArgs.Empty);
                }
            }
        }
        public event EventHandler CurrentModeChanged;

        [DataMember]
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
                    if (CurrentPositionChanged != null) CurrentPositionChanged(this, EventArgs.Empty);
                }
            }
        }
        public event EventHandler CurrentPositionChanged;

        [DataMember]
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
                    if (PausedChanged != null) PausedChanged(this, EventArgs.Empty);
                }
            }
        }
        public event EventHandler PausedChanged;

        [DataMember]
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
                    if (SpeedRatioChanged != null) SpeedRatioChanged(this, EventArgs.Empty);
                }
            }
        }
        public event EventHandler SpeedRatioChanged;

        [DataMember]
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
                    if (FaceVideoIsVisibleChanged != null) FaceVideoIsVisibleChanged(this, EventArgs.Empty);
                }
            }
        }
        public event EventHandler FaceVideoIsVisibleChanged;

        [DataMember]
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
                    if (DesktopVideoIsVisibleChanged != null) DesktopVideoIsVisibleChanged(this, EventArgs.Empty);
                }
            }
        }
        [DataMember]
        string currentSubtitle;
        public string CurrentSubtitle
        {
            get { return currentSubtitle; }
            set
            {
                if (currentSubtitle != value)
                {
                    currentSubtitle = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("CurrentSubtitle"));
                }
            }
        }


        public event EventHandler DesktopVideoIsVisibleChanged;

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
