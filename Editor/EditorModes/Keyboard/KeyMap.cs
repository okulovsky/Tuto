using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Editor
{
    public static class KeyMap
    {
        static Dictionary<Keys, KeyboardCommands> map;

        static KeyMap()
        {
            map = new Dictionary<Keys, KeyboardCommands>();
            map[Keys.Left] = KeyboardCommands.Left;
            map[Keys.D2] = KeyboardCommands.Left;

            map[Keys.Right] = KeyboardCommands.Right;
            map[Keys.D3] = KeyboardCommands.Right;

            map[Keys.D1] = KeyboardCommands.LargeLeft;
            map[Keys.D4] = KeyboardCommands.LargeRight;

            map[Keys.Up] = KeyboardCommands.SpeedUp;
            map[Keys.Down] = KeyboardCommands.SpeedDown;
            map[Keys.Space] = KeyboardCommands.PauseResume;

            map[Keys.D0] = KeyboardCommands.Face;
            map[Keys.OemMinus] = KeyboardCommands.Desktop;
            map[Keys.Oemplus] = KeyboardCommands.Drop;
            map[Keys.Back] = KeyboardCommands.Clear;

            map[Keys.Q] = KeyboardCommands.LeftToLeft;
            map[Keys.W] = KeyboardCommands.LeftToRight;

            map[Keys.O] = KeyboardCommands.RightToLeft;
            map[Keys.P] = KeyboardCommands.RightToRight;

        }



        public static KeyboardCommands GetCommand(Keys key)
        {
            if (!map.ContainsKey(key)) return KeyboardCommands.None;
            return map[key];
        }
    }
}