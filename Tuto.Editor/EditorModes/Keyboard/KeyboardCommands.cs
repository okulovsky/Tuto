using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuto.Model;

namespace Editor
{
    public enum KeyboardCommands
    {
        None,

        [CmdHelp(KeyboardGroup.FlowControl, EditorModes.General, "Increase the playback speed")]
        [CmdHelp(KeyboardGroup.FlowControl, EditorModes.Border, "Increase the playback speed outside of borders")]
        SpeedUp,


        [CmdHelp(KeyboardGroup.FlowControl, EditorModes.General, "Decrease the playback speed")]
        [CmdHelp(KeyboardGroup.FlowControl, EditorModes.Border, "Decrease the playback speed outside of borders")]
        SpeedDown,

        [CmdHelp(KeyboardGroup.FlowControl, EditorModes.General, "Pauses/resumes video")]
        PauseResume,

        [CmdHelp(KeyboardGroup.Navigation, "Jumps backward")]  
        Left,

        [CmdHelp(KeyboardGroup.Navigation, EditorModes.General, "Jumps to the previous chunk")]
        [CmdHelp(KeyboardGroup.Navigation, EditorModes.General, "Jumps to the previous border")]
        LargeLeft,

        [CmdHelp(KeyboardGroup.Navigation, "Jumps forward")]  
        Right,

        [CmdHelp(KeyboardGroup.Navigation, EditorModes.General, "Jumps to the next chunk")]
        [CmdHelp(KeyboardGroup.Navigation, EditorModes.General, "Jumps to the next border")]
        LargeRight,

        [CmdHelp(KeyboardGroup.Marking, "Marks the fragment as belonging to the face video")]
        Face,
        [CmdHelp(KeyboardGroup.Marking, "Marks the fragment as belonging to the desktop video")]
        Desktop,
        [CmdHelp(KeyboardGroup.Marking, "Marks the fragment as dropped")]
        Drop,
        [CmdHelp(KeyboardGroup.Marking, "Clears mark on the current fragment")]
        Clear,

        [CmdHelp(KeyboardGroup.BorderEditing,"Shifts the left border to left")]
        LeftToLeft,
        [CmdHelp(KeyboardGroup.BorderEditing, "Shifts the left border to right")]
        LeftToRight,
        [CmdHelp(KeyboardGroup.BorderEditing, "Shifts the right border to left")]
        RightToLeft,
        [CmdHelp(KeyboardGroup.BorderEditing, "Shifts the right border to right")]
        RightToRight,

        [CmdHelp(KeyboardGroup.Other,"Specifies that the current fragment starts the new episode.")]
        NewEpisodeHere
    }
}
