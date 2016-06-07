using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuto.Model
{
    public enum EditorModes
    {
        [EnumName("Preview")]
        [Description("In this mode, you roughly divide your video to the fragments, and mark each fragment with a source stream: Face for a video from a camera, Desktop for a screencap or Drop if the fragment should be omitted")]
        General,
        [EnumName("Final")]
        [Description("In this mode, you adjust borders between fragments of the different type. Pay the most attention to the borders between dropped and not-dropped fragments to eliminate scrappy voice. You also preview the video in this mode")]
        Border,
        Patch
    }
}
