using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Editor
{
    public static class KeyMap
    {
        static Dictionary<Key, KeyboardCommands> map;

        static KeyMap()
        {
            map = new Dictionary<Key, KeyboardCommands>();
            map[Key.Left] = KeyboardCommands.Left;
            map[Key.D2] = KeyboardCommands.Left;

            map[Key.Right] = KeyboardCommands.Right;
            map[Key.D3] = KeyboardCommands.Right;

            map[Key.D1] = KeyboardCommands.LargeLeft;
            map[Key.D4] = KeyboardCommands.LargeRight;

            map[Key.Up] = KeyboardCommands.SpeedUp;
            map[Key.Down] = KeyboardCommands.SpeedDown;
            map[Key.Space] = KeyboardCommands.PauseResume;

            map[Key.D0] = KeyboardCommands.Face;
            map[Key.OemMinus] = KeyboardCommands.Desktop;
            map[Key.OemPlus] = KeyboardCommands.Drop;
            map[Key.Back] = KeyboardCommands.Clear;

            map[Key.Q] = KeyboardCommands.LeftToLeft;
            map[Key.W] = KeyboardCommands.LeftToRight;

            map[Key.O] = KeyboardCommands.RightToLeft;
            map[Key.P] = KeyboardCommands.RightToRight;

        }



        public static KeyboardCommands GetCommand(Key key)
        {
            if (!map.ContainsKey(key)) return KeyboardCommands.None;
            return map[key];
        }

        public static KeyboardCommandData KeyboardCommandData(System.Windows.Input.KeyEventArgs args)
        {
            return new KeyboardCommandData
            {
                Command = GetCommand(args.Key),
                Shift = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift),
                Alt = Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt),
                Ctrl = Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)
            };
        }
    }
}