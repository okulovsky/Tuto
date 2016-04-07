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
            map[Key.D2] = KeyboardCommands.Left;
            map[Key.Left] = KeyboardCommands.Left;

            map[Key.D3] = KeyboardCommands.Right;
            map[Key.Right] = KeyboardCommands.Right;

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

            map[Key.D9] = KeyboardCommands.NewEpisodeHere;

            map[Key.B] = KeyboardCommands.InsertDeletion;

			map[Key.F8] = KeyboardCommands.Sign;
        }



        public static KeyboardCommands GetCommand(Key key)
        {
            if (!map.ContainsKey(key)) return KeyboardCommands.None;
            return map[key];
        }

        public static List<string> GetKeySymbols(KeyboardCommands command)
        {
            var e = map.Where(z => z.Value == command).Select(z => z.Key).Select(GetKeySymbol).ToList();
            return e;
        }

        public static string GetKeySymbol(Key key)
        {
            var s=key.ToString();
            if (s.Length == 1) return s;
            if (s.StartsWith("D")) return s.Substring(1, 1);
            switch (key)
            {
                case Key.Back: return "␈";
                case Key.Left: return "←";
                case Key.Right: return "→";
                case Key.Up: return "↑";
                case Key.Down: return "↓";
                case Key.OemPlus: return "+";
                case Key.OemMinus: return "-";
                case Key.Space: return "␣";
            }
            return s;
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