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

            var MS = Model.WindowState.CurrentPosition;

            if (Model.WindowState.CurrentPatch==null)
            {
                Model.WindowState.CurrentPatch = Model.Montage.Patches.Where(z => z.Begin <= MS && z.End >= MS).FirstOrDefault();
                var p= Model.WindowState.CurrentPatch;
                if(p!=null && p.Video!=null)
                {
                    if (p.Begin == p.End || p.Video.Duration <= 0) Model.WindowState.VideoPatchPosition = 0;
                    else
                    {
                        double k = ((double)(Model.WindowState.CurrentPosition - p.Begin)) / (p.End - p.Begin);
                        Model.WindowState.VideoPatchPosition= (int)(k * p.Video.Duration);
                    }
                }
            }

            if (Model.WindowState.CurrentVideoPatch != null)
                ProcessWhenVideoPatchIsPlaying();
            
        }
        void ProcessWhenVideoPatchIsPlaying()
        {
            Model.WindowState.PatchPlaying = PatchPlayingType.PatchOnly;
            if (Model.WindowState.CurrentVideoPatch.Duration>0 && Model.WindowState.VideoPatchPosition == Model.WindowState.CurrentVideoPatch.Duration)
            {
                Model.WindowState.CurrentPosition=Model.WindowState.CurrentPatch.End+1;
                Model.WindowState.PatchPlaying = PatchPlayingType.NoPatch;
                Model.WindowState.CurrentPatch = null;

            }
        }

        public void MouseClick(int SelectedLocation, bool alternative)
        {
            Model.WindowState.CurrentPosition = SelectedLocation;
            CheckTime();
        }

        public void ProcessKey(KeyboardCommandData key)
        {
            if (this.ProcessNavigationKey(key)) return;
            if (this.DefaultSpeedKey(key)) return;
            if (Model.WindowState.PatchSelection == null) return;
            var shiftValue = 200;
            var delta=1000;
            if (key.Shift || key.Ctrl) shiftValue = 50;
            var selection = Model.WindowState.PatchSelection;
            switch(key.Command)
            {
                case KeyboardCommands.LeftToLeft:
                    selection.Item.Begin = Math.Max(0, selection.Item.Begin - shiftValue);
                    Model.OnMarkupChanged();
                    Model.WindowState.CurrentPosition=selection.Item.Begin-delta;
                    break;
                case KeyboardCommands.RightToLeft:
                    selection.Item.End = Math.Max(selection.Item.Begin, selection.Item.End - shiftValue);
                    Model.OnMarkupChanged();
                    Model.WindowState.CurrentPosition=selection.Item.End-delta;
                    break;
               
                case KeyboardCommands.LeftToRight:
                    selection.Item.Begin = Math.Min(selection.Item.Begin + shiftValue, selection.Item.End);
                    Model.OnMarkupChanged();
                    Model.WindowState.CurrentPosition = selection.Item.Begin - delta;
                    break;

                case KeyboardCommands.RightToRight:
                    selection.Item.End = selection.Item.End + shiftValue;
                    Model.OnMarkupChanged();
                    Model.WindowState.CurrentPosition = selection.Item.End - delta;
                    break;
            }
        }
    }
}
