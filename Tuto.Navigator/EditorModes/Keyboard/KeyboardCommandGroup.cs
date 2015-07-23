using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuto.Model;

namespace Editor
{
    public enum KeyboardGroup
    {
        [EnumName("Naigation")]
        [Description("Allow you to navidate through the video.")]
        Navigation,
        [EnumName("Flow control")]
        [Description("Allow control over the playback speed")]
        FlowControl,
        [EnumName("Marking")]
        [Description("Allow you to divide the video to the fragment and to mark it. Initially, the video is not marked. By pressing mark buttons on the free space, you cut the fragment and mark it. You can mark the whole free fragment by pressing Ctrl. You can remark the fragment by pressing another mark button on it")]
        Marking,
        [EnumName("Border editing")]
        [Description("Allow you to adjust the borders between fragments. Holding Shift or Ctrl decreases the shift.")]
        [CmdHelp(EditorModes.General, "In this mode, the left and right border of current fragment is adjusted")]
        [CmdHelp(EditorModes.Border, "In this mode, if two active fragments are separated by the dropped fragment, the left and right border of this dropped fragment will be adjusted")]  
        BorderEditing,
        [EnumName("Other")]
        [Description("Other commands")]
        Other
    }

}
