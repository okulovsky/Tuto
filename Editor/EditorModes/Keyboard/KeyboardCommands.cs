using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor
{
    public enum KeyboardCommands
    {
        None,

        //flow control
        SpeedUp,
        SpeedDown,
        PauseResume,

        //navigation
        Left,
        Right,
        LargeLeft,
        LargeRight,

        //slicing
        Face,
        Desktop,
        Drop,
        Clear,

        //shifting
        LeftToLeft,
        LeftToRight,
        RightToLeft,
        RightToRight
    }
}
