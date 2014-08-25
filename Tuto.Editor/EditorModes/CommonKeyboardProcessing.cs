using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuto.Model;

namespace Editor
{
    public class CommonKeyboardProcessing
    {
        public static bool NavigationKeysProcessing(EditorModel model, KeyboardCommandData key)
            
        {
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

        public static bool ProcessCommonKeys(EditorModel model, KeyboardCommandData key)
        {
            if (NavigationKeysProcessing(model, key)) return true;

            var delta = 1000;
            if (key.Shift) delta = 200;
            if (key.Ctrl) delta = 50;

            switch (key.Command)
            {
               
                case KeyboardCommands.Face:
                    model.MarkHere(Mode.Face, key.Ctrl);
                    return true;

                case KeyboardCommands.Desktop:
                    model.MarkHere(Mode.Screen, key.Ctrl);
                    return true;

                case KeyboardCommands.Drop:
                    model.MarkHere(Mode.Drop, key.Ctrl);
                    return true;

                case KeyboardCommands.Clear:
                    model.RemoveChunkHere();
                    return true;

              

                case KeyboardCommands.NewEpisodeHere:
                    model.NewEpisodeHere();
                    return true;

            }
            return false;
        }
    }
}
