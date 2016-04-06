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
        public EditorModel Model { get; private set; }

        public PatchMode(EditorModel model)
        {
            this.Model = model;
        }


        public void CheckTime()
        {
            this.PlayWithoutDeletions();

            int ms = Model.WindowState.CurrentPosition;
            Model.WindowState.CurrentSubtitles = Model.Montage.Patches
                .Where(z => z.Subtitles!=null && z.Begin <= ms && z.End >= ms )
                .Select(z=>z.Subtitles)
                .FirstOrDefault();
        }

        public void MouseClick(int SelectedLocation, bool alternative)
        {
            Model.WindowState.CurrentPosition = SelectedLocation;
            CheckTime();
        }

        public void ProcessKey(KeyboardCommandData key)
        {
        
        }
    }
}
