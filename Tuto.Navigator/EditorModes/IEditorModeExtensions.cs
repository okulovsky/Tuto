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
    }
}
