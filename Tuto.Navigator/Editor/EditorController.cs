using Editor;
using System;
using System.Collections.Generic;
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
        readonly bool faceAvailable;
        readonly bool desktopAvailable;

		public EditorController(IEditorInterface panel, EditorModel model)
		{
			this.panel = panel;
			this.model = model;

			panel.KeyDown += panel_KeyDown;
			panel.TimelineMouseDown += panel_TimelineMouseDown;

			model.WindowState.SubsrcibeByExpression(z => z.Paused, PauseChanged);
			model.WindowState.SubsrcibeByExpression(z => z.CurrentMode, ModeChanged);
			model.WindowState.SubsrcibeByExpression(z => z.SpeedRatio, RatioChanged);
			model.WindowState.SubsrcibeByExpression(z => z.CurrentPosition, PositionChanged);
			model.WindowState.SubsrcibeByExpression(z => z.FaceVideoIsVisible, VideoVisibilityChanged);
			model.WindowState.SubsrcibeByExpression(z => z.DesktopVideoIsVisible, VideoVisibilityChanged);

            ModeChanged();
            RatioChanged();
            VideoVisibilityChanged();
            PositionChanged();
            PauseChanged();

            if (model.Locations.FaceVideoThumb.Exists)
            {
                panel.Face.SetFile(model.Locations.FaceVideoThumb);
                faceAvailable = true;
            }
            else if (model.Locations.FaceVideo.Exists)
            {
                panel.Face.SetFile(model.Locations.FaceVideo);
                faceAvailable = true;
            }
            else faceAvailable = false;

            if (model.Locations.DesktopVideoThumb.Exists)
            {
                panel.Desktop.SetFile(model.Locations.DesktopVideoThumb);
                desktopAvailable = true;
            }
            else if (model.Locations.DesktopVideo.Exists)
            {
                panel.Desktop.SetFile(model.Locations.DesktopVideoThumb);
                desktopAvailable = true;
            }
            else desktopAvailable = true;



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
				panel.KeyDown -= panel_KeyDown;
				panel.TimelineMouseDown -= panel_TimelineMouseDown;
                panel.Face.Stop();
                panel.Desktop.Stop();
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
			if (faceAvailable)
			{
                model.WindowState.CurrentPosition = panel.Face.Position;
				if (desktopAvailable)
				{
					var desktopVideoPosition = panel.Desktop.Position + model.Montage.SynchronizationShift;
					if (Math.Abs(desktopVideoPosition - model.WindowState.CurrentPosition) > 50)
						panel.Desktop.Position = model.WindowState.CurrentPosition - model.Montage.SynchronizationShift;
				}
				else
				{
					if (!model.WindowState.Paused)
						model.WindowState.CurrentPosition += (int)(timerInterval * model.WindowState.SpeedRatio);
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

		void panel_TimelineMouseDown(int arg1, MouseButtonEventArgs arg2)
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
			panel.Desktop.PlayPause(model.WindowState.Paused);
            panel.Face.PlayPause(model.WindowState.Paused);
		}

		void ModeChanged()
		{
			if (model.WindowState.CurrentMode == EditorModes.Border)
				currentMode = new BorderMode(model);
			if (model.WindowState.CurrentMode == EditorModes.General)
				currentMode = new GeneralMode(model);
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

		void VideoVisibilityChanged()
		{
			panel.Face.Visibility = model.WindowState.FaceVideoIsVisible;
			panel.Desktop.Visibility = model.WindowState.DesktopVideoIsVisible;
		}
		#endregion
	}
}