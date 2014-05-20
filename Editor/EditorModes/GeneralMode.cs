using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Tuto.Model;

namespace Editor
{
    public class GeneralMode : IEditorMode
    {
        EditorModel model;

        MontageModel montage { get { return model.Montage; } }

        public GeneralMode(EditorModel edModel)
        {
            this.model = edModel;
            model.WindowState.FaceVideoIsVisible = model.WindowState.DesktopVideoIsVisible = true;
        }

        public void CheckTime()
        {
        }


        public void MouseClick(int selectedLocation, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var index = montage.Chunks.FindIndex(selectedLocation);
                if (index == -1) return;
                model.WindowState.CurrentPosition=montage.Chunks[index].StartTime;
            }
            else
            {
                model.WindowState.CurrentPosition=selectedLocation;
            }
        }


        public void ProcessKey(KeyboardCommandData key)
        {
            var shiftValue = 200;
            if (key.Shift || key.Ctrl) shiftValue = 50;
            if (CommonKeyboardProcessing.ProcessCommonKeys(model, key)) return;
           
          
            switch (key.Command)
            {
                 case KeyboardCommands.LargeLeft:
                    PrevChunk();
                    return;

                case KeyboardCommands.LargeRight:
                    NextChunk();
                    return;

                
                case KeyboardCommands.LeftToLeft:
                    ShiftLeft(-shiftValue);
                    return;

                case KeyboardCommands.LeftToRight:
                    ShiftLeft(shiftValue);
                    return;

                case KeyboardCommands.RightToLeft:
                    ShiftRight(-shiftValue);
                    return;
                
                case KeyboardCommands.RightToRight:
                    ShiftRight(shiftValue);
                    return;

                case KeyboardCommands.SpeedUp:
                    model.WindowState.SpeedRatio+=0.5;
                    return;

                case KeyboardCommands.SpeedDown:
                    model.WindowState.SpeedRatio -= 0.5;
                    return;
            }

        }



        void ShiftLeft(int value)
        {
            var index = model.Montage.Chunks.FindIndex(model.WindowState.CurrentPosition);
            if (index == -1) return;
            model.ShiftLeftChunkBorder(index, value);
            model.WindowState.CurrentPosition = model.Montage.Chunks[index].StartTime;
        }

        void ShiftRight(int value)
        {
            var index = model.Montage.Chunks.FindIndex(model.WindowState.CurrentPosition);
            if (index == -1) return;
            model.ShiftRightChunkBorder(index, value);
            model.WindowState.CurrentPosition = model.Montage.Chunks[index].EndTime-2000;
        }

        void NextChunk()
        {
            var index = montage.Chunks.FindIndex(model.WindowState.CurrentPosition);
            index++;
            if (index < 0 || index >= montage.Chunks.Count) return;
            model.WindowState.CurrentPosition=montage.Chunks[index].StartTime;
        }

        void PrevChunk()
        {
            var index = montage.Chunks.FindIndex(model.WindowState.CurrentPosition);
            index--;
            if (index < 0 || index >= montage.Chunks.Count) return;
            model.WindowState.CurrentPosition=montage.Chunks[index].StartTime;

        }

      

    }
}
