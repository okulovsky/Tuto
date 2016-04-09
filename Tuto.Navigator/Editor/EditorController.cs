using Editor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using Tuto.Model;

namespace Tuto.Navigator.Editor
{
	public class EditorController : IDisposable
	{
		IEditorInterface panel;
		IEditorMode currentMode;
		EditorModel model;
		DispatcherTimer timer;
		const int timerInterval = 1;

		public EditorController(IEditorInterface panel, EditorModel model)
		{
			this.panel = panel;
			this.model = model;

			panel.ControlKeyDown += panel_KeyDown;
			panel.TimeSelected += panel_TimeSelected;

			model.WindowState.SubsrcibeByExpression(z => z.Paused, PauseChanged);
			model.WindowState.SubsrcibeByExpression(z => z.CurrentMode, ModeChanged);
			model.WindowState.SubsrcibeByExpression(z => z.SpeedRatio, RatioChanged);
			model.WindowState.SubsrcibeByExpression(z => z.CurrentPosition, PositionChanged);
			model.WindowState.SubsrcibeByExpression(z => z.FaceVideoIsVisible, VideoVisibilityChanged);
			model.WindowState.SubsrcibeByExpression(z => z.DesktopVideoIsVisible, VideoVisibilityChanged);
            model.WindowState.SubsrcibeByExpression(z => z.ArrangeMode, ArrangeModeChanged);
            model.WindowState.SubsrcibeByExpression(z => z.CurrentPatch, CurrentPatchChanged);
            model.WindowState.SubsrcibeByExpression(z => z.VideoPatchPosition, VideoPatchPositionChanged);
            model.WindowState.SubsrcibeByExpression(z => z.PatchPlaying, PatchPlayingChanged);

            ModeChanged();
            RatioChanged();
            ArrangeModeChanged();
            VideoVisibilityChanged();
            CurrentPatchChanged();
            PositionChanged();
            VideoPatchPositionChanged();
            PauseChanged();

            panel.Face.Die();
            panel.Desktop.Die();
            panel.Patch.Die();

            if (model.Locations.FaceVideoThumb.Exists)
            {
                panel.Face.SetFile(model.Locations.FaceVideoThumb);
            }
            else if (model.Locations.FaceVideo.Exists)
            {
                panel.Face.SetFile(model.Locations.FaceVideo);
            }
          
            if (model.Locations.DesktopVideoThumb.Exists)
            {
                panel.Desktop.SetFile(model.Locations.DesktopVideoThumb);
            }
            else if (model.Locations.DesktopVideo.Exists)
            {
                panel.Desktop.SetFile(model.Locations.DesktopVideo);
            }
      

			timer = new DispatcherTimer();
			timer.Interval = TimeSpan.FromMilliseconds(timerInterval);
			timer.Tick += (s, a) => { CheckPlayTime(); };
			timer.Start();
		}



		#region Disposable pattern
		bool isDisposed;
		~EditorController()
		{
			Dispose();
		}
		public void Dispose()
		{
			if (!isDisposed)
			{
				timer.Stop();
				model.WindowState.UnsubscribeAll(this);
				panel.ControlKeyDown -= panel_KeyDown;
				panel.TimeSelected -= panel_TimeSelected;
                panel.Face.Die();
                panel.Desktop.Die();
				isDisposed = true;
			}
		}
		#endregion

		#region События от таймера и контрола
		bool supressPositionChanged;
		bool pauseRequested;
		void CheckPlayTime()
		{
			supressPositionChanged = true;
			if (!panel.Face.Paused)
			{
                model.WindowState.CurrentPosition = panel.Face.Position;
				if (!panel.Desktop.Paused)
				{
					var desktopVideoPosition = panel.Desktop.Position + model.Montage.SynchronizationShift;
					if (Math.Abs(desktopVideoPosition - model.WindowState.CurrentPosition) > 50)
						panel.Desktop.Position = model.WindowState.CurrentPosition - model.Montage.SynchronizationShift;
				}
			}

            if (model.WindowState.PatchPlaying != PatchPlayingType.NoPatch)
            {
                if (model.WindowState.CurrentPatch!=null && model.WindowState.CurrentPatch.IsVideoPatch)
                {
                    model.WindowState.VideoPatchPosition = panel.Patch.Position;
                    model.WindowState.CurrentPatch.VideoData.Duration = panel.Patch.GetDuration();
                }
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

		void panel_TimeSelected(int arg1, bool arg2)
		{
			currentMode.MouseClick(arg1, arg2);
		}

		void panel_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
		{
			if (e.Key == Key.S)
			{
				model.Save();
				return;
			}
			currentMode.ProcessKey(KeyMap.KeyboardCommandData(e));
		}
		#endregion

		#region Реакция на изменения полей модели

		void PauseChanged()
		{
            SetAllPauses();
		}

        void SetAllPauses()
        {
            switch (model.WindowState.PatchPlaying)
            {
                case PatchPlayingType.NoPatch:
                    panel.Face.Paused = model.WindowState.Paused;
                    panel.Desktop.Paused = model.WindowState.Paused;
                    panel.Patch.Paused = true;
                    break;
                case PatchPlayingType.PatchOnly:
                    panel.Face.Paused = true;
                    panel.Desktop.Paused = true;
                    panel.Patch.Paused = model.WindowState.Paused;
                    break;
                case PatchPlayingType.PatchAlong:
                    panel.Face.Paused = model.WindowState.Paused;
                    panel.Desktop.Paused = model.WindowState.Paused;
                    panel.Patch.Paused = model.WindowState.Paused;
                    break;

                    
            }
        }

		void ModeChanged()
		{
			if (model.WindowState.CurrentMode == EditorModes.Border)
            {
				currentMode = new BorderMode(model);
                model.WindowState.ArrangeMode = ArrangeModes.Overlay;
            }
			if (model.WindowState.CurrentMode == EditorModes.General)
            {
                currentMode = new GeneralMode(model);
            }
            if (model.WindowState.CurrentMode == EditorModes.Patch)
            {
                currentMode = new PatchMode(model);
                model.WindowState.ArrangeMode = ArrangeModes.Patching;
            }
            panel.Refresh();
		}

		void RatioChanged()
		{
			panel.SetRatio(model.WindowState.SpeedRatio);
		}

		void PositionChanged()
		{
			if (supressPositionChanged) return;

			if (model.WindowState.Paused)
			{
				model.WindowState.Paused = false;
				pauseRequested = true;
			}
			panel.Face.Position = model.WindowState.CurrentPosition;
			panel.Desktop.Position = model.WindowState.CurrentPosition - model.Montage.SynchronizationShift;
		}

        void VideoPatchPositionChanged()
        {
            if (supressPositionChanged) return;
                
            panel.Patch.Position = model.WindowState.VideoPatchPosition;
        }

		void VideoVisibilityChanged()
		{
			panel.Face.Visibility = model.WindowState.FaceVideoIsVisible;
			panel.Desktop.Visibility = model.WindowState.DesktopVideoIsVisible;
		}

        void ArrangeModeChanged()
        {
            panel.SetArrangeMode(model.WindowState.ArrangeMode);
        }

        void CurrentPatchChanged()
        {
            var patch = model.WindowState.CurrentPatch;
            
            if (patch==null)
            {
                panel.SetSubtitles(null);
                panel.SetVideoPatch(null);
                panel.Patch.Visibility = false;
                panel.Face.Muted = false;
                panel.Patch.Die();
                return;
            }


            if (patch.Type == PatchType.Subtitles)
            {
                panel.SetSubtitles(model.WindowState.CurrentPatch.Data as SubtitlePatch);
                return;
            }

            if (patch.IsVideoPatch)
            {
                panel.SetVideoPatch(patch.VideoData);
                panel.Patch.Visibility = true;
                var filePath = Path.Combine(model.Videotheque.PatchFolder.FullName, patch.VideoData.RelativeFileName);
                panel.Patch.SetFile(new FileInfo(filePath));
                panel.Patch.Paused = false;
                panel.Patch.Muted = patch.VideoData.OverlayType != VideoPatchOverlayType.Replace;
                panel.Face.Muted = patch.VideoData.OverlayType == VideoPatchOverlayType.Replace;
                return;
            }
        }

        void PatchPlayingChanged()
        {
            SetAllPauses();
        }

		#endregion
	}
}