using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuto.Model;

namespace Editor
{
    public static class IEditorModeExtensions
    {
        public static void PlayWithoutDeletions(this IEditorMode mode)
        {
            var ms = mode.Model.WindowState.CurrentPosition;
            var index = mode.Model.Montage.Chunks.FindIndex(ms);
            if (index == -1)
            {
                mode.Model.WindowState.Paused = true;
                return;
            }

            if (mode.Model.Montage.Chunks[index].IsNotActive)
            {
                while (index < mode.Model.Montage.Chunks.Count && mode.Model.Montage.Chunks[index].IsNotActive)
                    index++;

                if (index >= mode.Model.Montage.Chunks.Count)
                {
                    mode.Model.WindowState.Paused = true;
                    return;
                }
                ms = mode.Model.WindowState.CurrentPosition = mode.Model.Montage.Chunks[index].StartTime;
            }

            mode.Model.WindowState.FaceVideoIsVisible = mode.Model.Montage.Chunks[index].Mode == Mode.Face;
            mode.Model.WindowState.DesktopVideoIsVisible = mode.Model.Montage.Chunks[index].Mode == Mode.Desktop;

        }

        public static bool ProcessNavigationKey(this IEditorMode mode, KeyboardCommandData key)
        {
            var model = mode.Model;
            var delta = 1000;
            if (key.Shift) delta = 200;
            if (key.Ctrl) delta = 50;


            switch (key.Command)
            {
                case KeyboardCommands.Left:
                    model.WindowState.CurrentPosition = ((int)(model.WindowState.CurrentPosition - delta));
                    return true;

                case KeyboardCommands.Right:
                    model.WindowState.CurrentPosition = ((int)(model.WindowState.CurrentPosition + delta));
                    return true;

                case KeyboardCommands.PauseResume:
                    model.WindowState.Paused = !model.WindowState.Paused;
                    return true;
            }
            return false;
        }

        public static bool ProcessCommonMarkupKey(this IEditorMode mode, KeyboardCommandData key)
        {
            var model = mode.Model;
            switch (key.Command)
            {

                case KeyboardCommands.InsertDeletion:
                    model.InsertDeletion(model.WindowState.CurrentPosition);
                    return true;
                case KeyboardCommands.NewEpisodeHere:
                    model.NewEpisodeHere();
                    return true;
            }
            return false;
        }

        public static bool DefaultSpeedKey(this IEditorMode mode, KeyboardCommandData key)
        {
            var Model = mode.Model;
            switch (key.Command)
            {
                case KeyboardCommands.SpeedUp:
                    Model.WindowState.SpeedRatio += 0.5;
                    Model.WindowState.SpeedRatio = Math.Min(3, Model.WindowState.SpeedRatio);
                    return true;

                case KeyboardCommands.SpeedDown:
                        Model.WindowState.SpeedRatio -= 0.5;
                    Model.WindowState.SpeedRatio = Math.Max(0.5, Model.WindowState.SpeedRatio);
                    return true;
            }
            return false;
        }
    }
}
