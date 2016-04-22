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
        public EditorModel Model { get; private set; }

        MontageModel montage { get { return Model.Montage; } }

        public GeneralMode(EditorModel edModel)
        {
            this.Model = edModel;
            Model.WindowState.FaceVideoIsVisible = Model.WindowState.DesktopVideoIsVisible = true;
        }

        public void CheckTime()
        {
            var index = montage.Chunks.FindIndex(Model.WindowState.CurrentPosition);
            if (montage.Chunks[index].Mode == Mode.Face) Model.WindowState.ArrangeMode = ArrangeModes.BothFaceBigger;
            if (montage.Chunks[index].Mode == Mode.Desktop) Model.WindowState.ArrangeMode = ArrangeModes.BothDesktopBigger;

        }


        public void MouseClick(int selectedLocation, bool alternative)
        {
            if (!alternative)
            {
                var index = montage.Chunks.FindIndex(selectedLocation);
                if (index == -1) return;
                Model.WindowState.CurrentPosition=montage.Chunks[index].StartTime;
            }
            else
            {
                Model.WindowState.CurrentPosition=selectedLocation;
            }
        }

        public void ProcessKey(KeyboardCommandData key)
        {
            var shiftValue = 200;
            if (key.Shift || key.Ctrl) shiftValue = 50;

            if (this.ProcessNavigationKey(key)) return;
            if (this.ProcessCommonMarkupKey(key)) return;
            if (this.DefaultSpeedKey(key)) return;
          
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

               

                case KeyboardCommands.Face:
                  Model.MarkHere(Mode.Face, key.Ctrl);
                  Model.WindowState.ArrangeMode = ArrangeModes.BothFaceBigger;
                  return;

                case KeyboardCommands.Desktop:
                  Model.MarkHere(Mode.Desktop, key.Ctrl);
                  Model.WindowState.ArrangeMode = ArrangeModes.BothDesktopBigger;
                  return;

                case KeyboardCommands.Drop:
                  Model.MarkHere(Mode.Drop, key.Ctrl);
                  return;

                case KeyboardCommands.Clear:
                  Model.RemoveChunkHere();
                  return;

            }

        }



        void ShiftLeft(int value)
        {
            var index = Model.Montage.Chunks.FindIndex(Model.WindowState.CurrentPosition);
            if (index == -1) return;
            Model.ShiftLeftChunkBorder(index, value);
            Model.WindowState.CurrentPosition = Model.Montage.Chunks[index].StartTime;
        }

        void ShiftRight(int value)
        {
            var index = Model.Montage.Chunks.FindIndex(Model.WindowState.CurrentPosition);
            if (index == -1) return;
            Model.ShiftRightChunkBorder(index, value);
            Model.WindowState.CurrentPosition = Model.Montage.Chunks[index].EndTime-2000;
        }

        void NextChunk()
        {
            var index = montage.Chunks.FindIndex(Model.WindowState.CurrentPosition);
            index++;
            if (index < 0 || index >= montage.Chunks.Count) return;
            Model.WindowState.CurrentPosition=montage.Chunks[index].StartTime;
        }

        void PrevChunk()
        {
            var index = montage.Chunks.FindIndex(Model.WindowState.CurrentPosition);
            index--;
            if (index < 0 || index >= montage.Chunks.Count) return;
            Model.WindowState.CurrentPosition=montage.Chunks[index].StartTime;

        }

      

    }
}
