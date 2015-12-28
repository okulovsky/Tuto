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
        EditorModel model;

        public MontageModel montage { get { return model.Montage; } }

        double FastSpeed;
   

        public BorderMode(EditorModel editorModel)
        {
            this.model = editorModel;
            model.GenerateBorders();
            FastSpeed = model.Videotheque.Data.EditorSettings.DefaultFinalAcceleration;
        }

        public void CheckTime()
        {
            CheckTime(model.WindowState.CurrentPosition);
        }

        public void CheckTime(int ms)
        {

            var index = montage.Chunks.FindIndex(ms);
            if (index == -1)
            {
                model.WindowState.Paused = true;
                return;
            }

            if (montage.Chunks[index].IsNotActive)
            {
                while (index < montage.Chunks.Count && montage.Chunks[index].IsNotActive)
                    index++;

                if (index >= montage.Chunks.Count)
                {
                    model.WindowState.Paused = true;
                    return;
                }
                ms=model.WindowState.CurrentPosition = montage.Chunks[index].StartTime;
            }

            model.WindowState.FaceVideoIsVisible = montage.Chunks[index].Mode == Mode.Face;
            model.WindowState.DesktopVideoIsVisible = montage.Chunks[index].Mode == Mode.Desktop;

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

            model.WindowState.SpeedRatio = speed;
            
        }


        public void MouseClick(int selectedLocation, MouseButtonEventArgs button)
        {
            model.WindowState.CurrentPosition = selectedLocation;
            CheckTime(selectedLocation);
        }


        public void ProcessKey(KeyboardCommandData key)
        {
            if (CommonKeyboardProcessing.ProcessCommonKeys(model, key)) return;


            switch (key.Command)
            {
                case KeyboardCommands.LargeRight:
                    var border = montage.Borders.Where(z => z.StartTime > model.WindowState.CurrentPosition).FirstOrDefault();
                    if (border != null)
                        model.WindowState.CurrentPosition = border.StartTime;
                    return;
                case KeyboardCommands.LargeLeft:
                    var border1 = montage.Borders.Where(z => z.EndTime < model.WindowState.CurrentPosition).LastOrDefault();
                    if (border1 != null)
                        model.WindowState.CurrentPosition = border1.StartTime;
                    return;
                case KeyboardCommands.SpeedDown:
                    FastSpeed -= 0.5;
					FastSpeed = Math.Max(0.5, FastSpeed);
                    return;
                case KeyboardCommands.SpeedUp:
                    FastSpeed += 0.5;
					FastSpeed = Math.Min(3, FastSpeed);
                    return;
            }



            var borderIndex = montage.Borders.FindBorder(model.WindowState.CurrentPosition);
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
            model.ShiftLeftChunkBorder(border.RightChunk, shiftSize);
            model.GenerateBorders();
            model.WindowState.CurrentPosition = montage.Borders[borderIndex].StartTime;
            if (montage.Borders[borderIndex].IsLeftBorder) model.WindowState.SpeedRatio = 1;
            model.OnMontageModelChanged();
        }
    }
}
