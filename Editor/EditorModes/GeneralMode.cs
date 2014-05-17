using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

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
                var index = montage.Chunks.FindChunkIndex(selectedLocation);
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
            var value = 0.0;
            if (key.Shift)
                value = -1;
            if (key.Ctrl)
                value = -1.5;
           
          
            switch (key.Command)
            {
                case KeyboardCommands.Left:
                    model.WindowState.CurrentPosition=((int)(model.WindowState.CurrentPosition - 1000 * Math.Pow(5, value)));
                    return;

                case KeyboardCommands.Right:
                    model.WindowState.CurrentPosition = ((int)(model.WindowState.CurrentPosition + 1000 * Math.Pow(5, value)));
                    return;
                   
                case KeyboardCommands.LargeLeft:
                    PrevChunk();
                    return;

                case KeyboardCommands.LargeRight:
                    NextChunk();
                    return;

                case KeyboardCommands.Face:
                    model.SetChunkMode(Mode.Face, key.Ctrl);
                    return;
                
                case KeyboardCommands.Desktop:
                    model.SetChunkMode(Mode.Screen, key.Ctrl);
                    return;

                case KeyboardCommands.Drop:
                    model.SetChunkMode(Mode.Drop, key.Ctrl);
                    return;
                
                case KeyboardCommands.Clear:
                    RemoveChunk();

                    return;

                case KeyboardCommands.LeftToLeft:
                    ShiftLeft(-200);
                    return;

                case KeyboardCommands.LeftToRight:
                    ShiftLeft(200);
                    return;

                case KeyboardCommands.RightToLeft:
                    ShiftRight(-200);
                    return;
                
                case KeyboardCommands.RightToRight:
                    ShiftRight(200);
                    return;

                case KeyboardCommands.SpeedUp:
                    model.WindowState.SpeedRatio+=0.5;
                    return;

                case KeyboardCommands.SpeedDown:
                    model.WindowState.SpeedRatio -= 0.5;
                    return;

                case KeyboardCommands.PauseResume:
                    model.WindowState.Paused = !model.WindowState.Paused;
                    return;


                case KeyboardCommands.NewEpisodeHere:
                    var index = montage.Chunks.FindChunkIndex(model.WindowState.CurrentPosition);
                    if (index != -1)
                    {
                        montage.Chunks[index].StartsNewEpisode = !montage.Chunks[index].StartsNewEpisode;
                        model.Montage.SetChanged();
                    }
                    return;

            }

        }

        void RemoveChunk()
        {
            var position = model.WindowState.CurrentPosition;
            var index = montage.Chunks.FindChunkIndex(position);
            if (index == -1) return;
            var chunk = montage.Chunks[index];
            chunk.Mode = Mode.Undefined;
            if (index != montage.Chunks.Count - 1 && montage.Chunks[index + 1].Mode == Mode.Undefined)
            {
                chunk.Length += montage.Chunks[index + 1].Length;
                montage.Chunks.RemoveAt(index + 1);
            }
            if (index != 0 && montage.Chunks[index - 1].Mode == Mode.Undefined)
            {
                chunk.StartTime = montage.Chunks[index - 1].StartTime;
                chunk.Length += montage.Chunks[index - 1].Length;
                montage.Chunks.RemoveAt(index - 1);
            }
            montage.SetChanged();
        }

        void ShiftLeft(int delta)
        {
            var position = model.WindowState.CurrentPosition;
            var index = montage.Chunks.FindChunkIndex(position);
            if (index == -1 || index == 0) return;
            if (delta < 0 && montage.Chunks[index - 1].Length < -delta) return;
            if (delta > 0 && montage.Chunks[index].Length < delta) return ;
            montage.Chunks[index].StartTime += delta;
            montage.Chunks[index].Length -= delta;
            montage.Chunks[index - 1].Length += delta;

            model.WindowState.CurrentPosition = montage.Chunks[index].StartTime;
            montage.SetChanged();
        }

        void ShiftRight(int delta)
        {
            var position = model.WindowState.CurrentPosition;
            var index = montage.Chunks.FindChunkIndex(position);
            if (index == -1 || index == montage.Chunks.Count - 1) return;
            if (delta < 0 && montage.Chunks[index].Length < -delta) return;
            if (delta > 0 && montage.Chunks[index + 1].Length < delta) return;
            montage.Chunks[index].Length += delta;
            montage.Chunks[index + 1].Length -= delta;
            montage.Chunks[index + 1].StartTime += delta;

            model.WindowState.CurrentPosition = montage.Chunks[index + 1].StartTime - 2000;
            montage.SetChanged();
        }

        void NextChunk()
        {
            var index = montage.Chunks.FindChunkIndex(model.WindowState.CurrentPosition);
            index++;
            if (index < 0 || index >= montage.Chunks.Count) return;
            model.WindowState.CurrentPosition=montage.Chunks[index].StartTime;
        }

        void PrevChunk()
        {
            var index = montage.Chunks.FindChunkIndex(model.WindowState.CurrentPosition);
            index--;
            if (index < 0 || index >= montage.Chunks.Count) return;
            model.WindowState.CurrentPosition=montage.Chunks[index].StartTime;

        }

      

    }
}
