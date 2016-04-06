using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuto.Model;

namespace Editor
{
    public class PatchMode : IEditorMode
    {
        EditorModel model;
        public PatchMode(EditorModel model)
        {
            this.model = model;
        }


        public void CheckTime()
        {
            int ms = model.WindowState.CurrentPosition;
            model.WindowState.CurrentSubtitles = model.Montage.Patches
                .Where(z => z.Subtitles!=null && z.Begin <= ms && z.End >= ms )
                .Select(z=>z.Subtitles)
                .FirstOrDefault();
        }

        public void MouseClick(int SelectedLocation, bool alternative)
        {
            model.WindowState.CurrentPosition = SelectedLocation;
        }

        public void ProcessKey(KeyboardCommandData key)
        {
        
        }
    }
}
