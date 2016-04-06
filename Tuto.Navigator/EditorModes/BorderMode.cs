using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Tuto.Model;

namespace Editor
{

    public class BorderMode : IEditorMode
    {
        public EditorModel Model { get; private set;}

        public MontageModel montage { get { return Model.Montage; } }

        double FastSpeed;
   

        public BorderMode(EditorModel editorModel)
        {
            this.Model = editorModel;
            Model.GenerateBorders();
            FastSpeed = Model.Videotheque.Data.EditorSettings.DefaultFinalAcceleration;
        }

        public void CheckTime()
        {
            this.PlayWithoutDeletions();
            ResolveSpeed();
        }

        public void ResolveSpeed()
        {
            var ms = Model.WindowState.CurrentPosition;
            double speed = FastSpeed;
            var bindex = montage.Borders.FindBorder(ms);
            if (bindex != -1)
            {
                var border = montage.Borders[bindex];
                if (!border.IsLeftBorder)
                {
                    if (border.EndTime - ms < 2000) speed = 1;
                }
                else
                {
                    if (ms - border.StartTime < 1000) speed = 1;
                }

            }

            Model.WindowState.SpeedRatio = speed;
            
        }


        public void MouseClick(int selectedLocation, bool button)
        {
            Model.WindowState.CurrentPosition = selectedLocation;
            CheckTime();
        }


        public void ProcessKey(KeyboardCommandData key)
        {
            if (CommonKeyboardProcessing.ProcessCommonKeys(Model, key)) return;


            switch (key.Command)
            {
                case KeyboardCommands.LargeRight:
                    var border = montage.Borders.Where(z => z.StartTime > Model.WindowState.CurrentPosition).FirstOrDefault();
                    if (border != null)
                        Model.WindowState.CurrentPosition = border.StartTime;
                    return;
                case KeyboardCommands.LargeLeft:
                    var border1 = montage.Borders.Where(z => z.EndTime < Model.WindowState.CurrentPosition).LastOrDefault();
                    if (border1 != null)
                        Model.WindowState.CurrentPosition = border1.StartTime;
                    return;
                case KeyboardCommands.SpeedDown:
                    FastSpeed -= 0.5;
					FastSpeed = Math.Max(0.5, FastSpeed);
                    return;
                case KeyboardCommands.SpeedUp:
                    FastSpeed += 0.5;
					FastSpeed = Math.Min(3, FastSpeed);
                    return;
                case KeyboardCommands.Face:
                    Model.MarkHere(Mode.Face, key.Ctrl);
                    return;

                case KeyboardCommands.Desktop:
                    Model.MarkHere(Mode.Desktop, key.Ctrl);
                    return;

                case KeyboardCommands.Drop:
                    Model.MarkHere(Mode.Drop, key.Ctrl);
                    return;

                case KeyboardCommands.Clear:
                    Model.RemoveChunkHere();
                    return;

            }



            var borderIndex = montage.Borders.FindBorder(Model.WindowState.CurrentPosition);
            if (borderIndex == -1) return;
            int leftBorderIndex = -1;
            int rightBorderIndex = -1;
            if (montage.Borders[borderIndex].IsLeftBorder)
            {
                leftBorderIndex = borderIndex;
                if (borderIndex != 0 && !montage.Borders[borderIndex - 1].IsLeftBorder)
                    rightBorderIndex = borderIndex - 1;
            }
            else
            {
                rightBorderIndex = borderIndex;
                if (borderIndex != montage.Borders.Count - 1 && montage.Borders[borderIndex + 1].IsLeftBorder)
                    leftBorderIndex = borderIndex + 1;
            }

            var value = 200;
            if (key.Shift)
                value = 50;
            

            switch (key.Command)
            {
                case KeyboardCommands.LeftToLeft:
                    Shift(rightBorderIndex, -value);
                    return;

                case KeyboardCommands.LeftToRight:
                    Shift(rightBorderIndex, value);
                    return;

                case KeyboardCommands.RightToLeft:
                    Shift(leftBorderIndex, -value);
                    return;

                case KeyboardCommands.RightToRight:
                    Shift(leftBorderIndex, value);
                    return;
           }
        }

        void Shift(int borderIndex, int shiftSize)
        {
            if (borderIndex == -1) return;
            var border = montage.Borders[borderIndex];
            Model.ShiftLeftChunkBorder(border.RightChunk, shiftSize);
            Model.GenerateBorders();
            Model.WindowState.CurrentPosition = montage.Borders[borderIndex].StartTime;
            if (montage.Borders[borderIndex].IsLeftBorder) Model.WindowState.SpeedRatio = 1;
            Model.OnMarkupChanged();
        }
    }
}
