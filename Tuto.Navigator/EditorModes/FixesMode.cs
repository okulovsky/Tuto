using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Editor.Windows;
using Tuto.Model;

namespace Editor
{
    public class FixesMode : IEditorMode
    {

        EditorModel model;

        public FixesMode(EditorModel model)
        {
            this.model = model;
        }

        public void CheckTime()
        {
            var fix = GetCurrentFix();
            if (fix == null) model.WindowState.CurrentSubtitle = "";
            else model.WindowState.CurrentSubtitle = fix.Text;
        }

        public void MouseClick(int SelectedLocation, System.Windows.Input.MouseButtonEventArgs button)
        {
            model.WindowState.CurrentPosition = SelectedLocation;
        }

        public void ProcessKey(KeyboardCommandData key)
        {
            if (CommonKeyboardProcessing.NavigationKeysProcessing(model, key)) return;



            var fix = GetCurrentFix();

            if (fix==null && key.Command == KeyboardCommands.Face)
            {
                int position = 0;
                for (; position < model.Montage.SubtitleFixes.Count; position++)
                    if (model.Montage.SubtitleFixes[position].StartTime > model.WindowState.CurrentPosition)
                        break;

                model.Montage.SubtitleFixes.Insert(position, new SubtitleFix { StartTime = model.WindowState.CurrentPosition, Length = 2000 });

                model.OnNonSignificantChanged();
                return;
            }


            if (fix == null) return;

            int delta=200;
            if (key.Ctrl) delta=100;
            if (key.Shift) delta=50;


            switch (key.Command)
            {
                case KeyboardCommands.LeftToLeft:
                    fix.StartTime -= delta;
                    fix.Length += delta;
                    model.OnNonSignificantChanged();
                    return;

                case KeyboardCommands.LeftToRight:
                    fix.StartTime += delta;
                    fix.Length -= delta;
                    model.OnNonSignificantChanged();
                    return;

                case KeyboardCommands.RightToLeft:
                    fix.Length -= delta;
                    model.OnNonSignificantChanged();
                    return;
                case KeyboardCommands.RightToRight:
                    fix.Length += delta;
                    model.OnNonSignificantChanged();
                    return;
                case KeyboardCommands.Drop:
                    model.Montage.SubtitleFixes.Remove(fix);
                    model.OnNonSignificantChanged();
                    return;
                case KeyboardCommands.Desktop:
                    fix.Text = FixWindow.EnterText(fix.Text);
                    return;
            }
        }

        private SubtitleFix GetCurrentFix()
        {
            var fix = model.Montage.SubtitleFixes
                .Where(z => z.StartTime <= model.WindowState.CurrentPosition && z.StartTime + z.Length >= model.WindowState.CurrentPosition)
                .FirstOrDefault();
            return fix;
        }
    }
}
