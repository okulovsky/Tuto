using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Tuto.Model
{

    public enum PatchPlayingType
    {
        NoPatch,
        PatchOnly,
        PatchAlong
    }

    [DataContract]
    public class   WindowState : NotifierModel
    {
		internal EditorModel EditorModel;

        void SetAndNotify<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (field == null)
            {
                if (value == null) return;
            }
            else
            {
                if (field.Equals(value)) return;
            }

            field = value;
            NotifyPropertyChanged(propertyName);
        }

        [DataMember]
        EditorModes currentMode;
        public EditorModes CurrentMode
        {
            get { return currentMode; }
            set
            {
				SetAndNotify(ref currentMode, value);
            }
        }
       
        [DataMember]
        int currentPosition;
        public int CurrentPosition
        {
            get { return currentPosition; }
            set
            {
				SetAndNotify(ref currentPosition, value);
				UpdatePositions();
            }
        }

		

		public string CurrentPositionAbsolute { get; private set; }
		public string CurrentPositionRelative { get; private set; }

		void UpdatePositions()
		{
			var span = TimeSpan.FromMilliseconds(CurrentPosition);
			CurrentPositionAbsolute = string.Format("{0:D2}:{1:D2}:{2:D2}'{3:D3}", span.Hours, span.Minutes, span.Seconds, span.Milliseconds);
			var msFromStart=0;
			var episode = 0;
			foreach(var e in EditorModel.Montage.Chunks)
			{
				if (e.StartsNewEpisode) 
				{
					msFromStart = 0;
					episode++;
				}
				bool ends = e.EndTime>CurrentPosition;
				if (e.Mode != Mode.Drop)
				{
					if (ends) 
						msFromStart+=CurrentPosition-e.StartTime;
					else 
						msFromStart+=e.Length;
				}
				if (ends) break;
			}
			span = TimeSpan.FromMilliseconds(msFromStart);
			CurrentPositionRelative=string.Format("Episode-{0}:{1:D2}:{2:D2}'{3:D3}",
				episode,
				span.Minutes,
				span.Seconds,
				span.Milliseconds);
			this.NotifyByExpression(z => z.CurrentPositionAbsolute);
			this.NotifyByExpression(z=>z.CurrentPositionRelative);
		}
       
        [DataMember]
        bool paused;
        public bool Paused
        {
            get { return paused; }
            set
            {
				SetAndNotify(ref paused, value);
            }
        }
       

        [DataMember]
        double speedRatio;
        public double SpeedRatio
        {
            get { return speedRatio; }
            set
            {
				SetAndNotify(ref speedRatio, value);
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
				SetAndNotify(ref faceVideoIsVisible, value);
            }
        }
    
        [DataMember]
        bool desktopVideoIsVisible;
        public bool DesktopVideoIsVisible
        {
            get { return desktopVideoIsVisible; }
            set
            {
				SetAndNotify(ref desktopVideoIsVisible, value);
            }
        }

        [DataMember]
        ArrangeModes arrangeMode;

        public ArrangeModes ArrangeMode
        {
            get { return arrangeMode; }
            set { SetAndNotify(ref arrangeMode, value); }
        }

        public event Action GetBack;

		public void OnGetBack()
		{
			if (GetBack != null)
				GetBack();
		}



        int videoPatchPosition;
        public int VideoPatchPosition
        {
            get { return videoPatchPosition;}
            set { SetAndNotify(ref videoPatchPosition, value); }
        }

        public PatchPlayingType patchPlaying;

        public PatchPlayingType PatchPlaying
        {
            get
            {
                return patchPlaying;
            }
            set
            {
                SetAndNotify(ref patchPlaying, value);
            }
        }

        Patch currentPatch;
        public Patch CurrentPatch
        {
            get { return currentPatch; }
            set
            {
                SetAndNotify(ref currentPatch, value);
                this.NotifyByExpression(z => z.CurrentSubtitles);
                this.NotifyByExpression(z => z.CurrentVideoPatch);
                this.NotifyByExpression(z => z.PatchPlaying);
            }
        }

        public SubtitlePatch CurrentSubtitles { get { if (currentPatch == null) return null; return currentPatch.Subtitles; } }
        public VideoPatch CurrentVideoPatch { get { if (currentPatch == null) return null; return currentPatch.Video; } }

        public PatchSelection PatchSelection
        {
            get;
            set;
        }

        public WindowState()
        {
            Paused = true;
            CurrentMode = EditorModes.General;
            speedRatio = 1;
            FaceVideoIsVisible = DesktopVideoIsVisible = true;
        }
    }
}
