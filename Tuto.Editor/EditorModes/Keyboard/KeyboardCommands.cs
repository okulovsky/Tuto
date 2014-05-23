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

        [OfGroup(KeyboardGroup.FlowControl)]
        [CmdHelp(EditorModes.General, "Increase the playback speed")]
        [CmdHelp(EditorModes.Border, "Increase the playback speed outsKeyboardGroup.FlowControl, EditorModeside of borders")]
        SpeedUp,

         [OfGroup(KeyboardGroup.FlowControl)]
        [CmdHelp(EditorModes.General, "Decrease the playback speed")]
        [CmdHelp(EditorModes.Border, "Decrease the playback speed outside of borders")]
        SpeedDown,
        
        [OfGroup(KeyboardGroup.FlowControl)]
        [CmdHelp(EditorModes.General, "Pauses/resumes video")]
        PauseResume,

        [OfGroup(KeyboardGroup.Navigation)]
        [CmdHelp("Jumps backward")]  
        Left,

        [OfGroup(KeyboardGroup.Navigation)]
        [CmdHelp(EditorModes.General, "Jumps to the previous chunk")]
        [CmdHelp(EditorModes.General, "Jumps to the previous border")]
        LargeLeft,

        [OfGroup(KeyboardGroup.Navigation)]
        [CmdHelp("Jumps forward")]  
        Right,

        [OfGroup(KeyboardGroup.Navigation)]
        [CmdHelp(EditorModes.General, "Jumps to the next chunk")]
        [CmdHelp(EditorModes.General, "Jumps to the next border")]
        LargeRight,

        [OfGroup(KeyboardGroup.Marking)]
        [CmdHelp("Marks the fragment as belonging to the face video")]
        Face,

        [OfGroup(KeyboardGroup.Marking)]
        [CmdHelp("Marks the fragment as belonging to the desktop video")]
        Desktop,
        
        [OfGroup(KeyboardGroup.Marking)]
        [CmdHelp("Marks the fragment as dropped")]
        Drop,
        
        [OfGroup(KeyboardGroup.Marking)]
        [CmdHelp("Clears mark on the current fragment")]
        Clear,

        [OfGroup(KeyboardGroup.BorderEditing)]
        [CmdHelp("Shifts the left border to left")]
        LeftToLeft,
        [OfGroup(KeyboardGroup.BorderEditing)]
        [CmdHelp("Shifts the left border to right")]
        LeftToRight,
        [OfGroup(KeyboardGroup.BorderEditing)]
        [CmdHelp("Shifts the right border to left")]
        RightToLeft,
        [OfGroup(KeyboardGroup.BorderEditing)]
        [CmdHelp("Shifts the right border to right")]
        RightToRight,

        [OfGroup(KeyboardGroup.Other)]
        [CmdHelp("Specifies that the current fragment starts the new episode.")]
        NewEpisodeHere
    }
}
