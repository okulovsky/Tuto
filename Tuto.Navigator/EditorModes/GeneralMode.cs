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
            var index = montage.Chunks.FindIndex(model.WindowState.CurrentPosition);
            if (montage.Chunks[index].Mode == Mode.Face) model.WindowState.ArrangeMode = ArrangeModes.BothFaceBigger;
            if (montage.Chunks[index].Mode == Mode.Desktop) model.WindowState.ArrangeMode = ArrangeModes.BothDesktopBigger;

        }


        public void MouseClick(int selectedLocation, bool alternative)
        {
            if (!alternative)
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
					model.WindowState.SpeedRatio = Math.Min(3, model.WindowState.SpeedRatio);
                     return;

                case KeyboardCommands.SpeedDown:
                    model.WindowState.SpeedRatio -= 0.5;
					 model.WindowState.SpeedRatio = Math.Max(0.5, model.WindowState.SpeedRatio);
                  return;

                case KeyboardCommands.Face:
                  model.MarkHere(Mode.Face, key.Ctrl);
                  model.WindowState.ArrangeMode = ArrangeModes.BothFaceBigger;
                  return;

                case KeyboardCommands.Desktop:
                  model.MarkHere(Mode.Desktop, key.Ctrl);
                  model.WindowState.ArrangeMode = ArrangeModes.BothDesktopBigger;
                  return;

                case KeyboardCommands.Drop:
                  model.MarkHere(Mode.Drop, key.Ctrl);
                  return;

                case KeyboardCommands.Clear:
                  model.RemoveChunkHere();
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
