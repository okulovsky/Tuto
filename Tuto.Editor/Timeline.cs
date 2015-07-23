using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Tuto.Model;

namespace Editor
{
    public class TimelineBase : FrameworkElement
    {
        protected EditorModel editorModel { get { return (EditorModel)DataContext; } }
        protected MontageModel model { get { return editorModel.Montage; } }

        protected readonly int RowHeight = 15;
        protected readonly int msInRow = 300000;

        protected readonly Brush[] fills = new Brush[] { Brushes.White, Brushes.MistyRose, Brushes.LightGreen, Brushes.LightBlue };
        protected readonly Pen borderPen = new Pen(Brushes.Black, 1);
        protected readonly Pen currentPen = new Pen(Brushes.Red, 3);
        protected readonly Pen episode = new Pen(Brushes.Yellow, 3);
        protected readonly Pen border = new Pen(Brushes.Gray, 3) { EndLineCap = PenLineCap.Triangle };
        protected readonly Pen fixes = new Pen(Brushes.Gold, 3);

        protected override Size MeasureOverride(Size availableSize)
        {
            var totalLength = 60 * 60 * 1000;
            var rows = (int)Math.Ceiling(((double)totalLength) / msInRow);
            return new Size(availableSize.Width, rows * RowHeight + 5);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            return base.ArrangeOverride(finalSize);
        }


        protected IEnumerable<Rect> GetRects(StreamChunk chunk)
        {
            double SWidth = ActualWidth / msInRow;

            var start = chunk.StartTime;
            var length = chunk.Length;

            int x = start % msInRow;
            int y = start / msInRow;

            while (true)
            {
                if (x + length <= msInRow)
                {
                    yield return new Rect(x * SWidth, y * RowHeight, length * SWidth, RowHeight);
                    yield break;
                }
                yield return new Rect(x * SWidth, y * RowHeight, (msInRow - x) * SWidth, RowHeight);
                length -= (msInRow - x);
                x = 0;
                y++;
            }
        }

        public int MsAtPoint(Point point)
        {
            var row = (int)point.Y / RowHeight;
            return (int)Math.Round(msInRow * (row + point.X / ActualWidth));
        }


        public Point GetCoordinate(int timeInMilliseconds)
        {
            int y = timeInMilliseconds / msInRow;
            double x = timeInMilliseconds % msInRow;
            return new Point(
                x * ActualWidth / msInRow,
                y * RowHeight);
        }

        protected void DrawLine(DrawingContext context, Pen pen, int startPoint, int endPoint, int verticalDisplacement)
        {
            var begin = GetCoordinate(startPoint);
            var end = GetCoordinate(endPoint);
            begin.Y += verticalDisplacement;
            end.Y += verticalDisplacement;
            if (begin.Y == end.Y)
            {
                context.DrawLine(pen, begin, end);
            }
            else
            {
                context.DrawLine(pen, begin, new Point(ActualWidth, begin.Y));
                context.DrawLine(pen, new Point(0, end.Y), end);
            }
        }
    }


    public class Slider : TimelineBase
    {
        public Slider()
        {
            this.DataContextChanged += (s, a) => { 
                InvalidateVisual();
                editorModel.WindowState.PropertyChanged += (ss, aa) => InvalidateVisual();
            };
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            if (editorModel == null) return;
            var point = GetCoordinate(editorModel.WindowState.CurrentPosition);
            drawingContext.DrawLine(currentPen, point, new Point(point.X, point.Y + RowHeight));
        }
    }

    public class ModelView : TimelineBase
    {
        public ModelView()
        {
            this.DataContextChanged += (s, a) => { 
                InvalidateVisual();
                editorModel.MontageModelChanged += (ss, aa) => InvalidateVisual();
            };
        }

        protected override void OnRender(System.Windows.Media.DrawingContext drawingContext)
        {
            if (editorModel == null) return;

           foreach (var c in model.Chunks)
               foreach (var r in GetRects(c))
               {
                   drawingContext.DrawRectangle(fills[(int)c.Mode], borderPen, r);
                   if (c.StartsNewEpisode)
                   {
                       var p = GetCoordinate(c.StartTime);
                       drawingContext.DrawLine(episode, p, new Point(p.X, p.Y + RowHeight));
                   }
               }

           if (model.SoundIntervals != null)
           {
               foreach (var i in model.SoundIntervals)
               {
                   if (!i.HasVoice)
                       DrawLine(drawingContext, border, i.StartTime, i.EndTime, RowHeight - 3);
               }
           }

            if (editorModel.WindowState.CurrentMode == EditorModes.Border && model.Borders!=null)
                foreach (var e in model.Borders)
                {
                    DrawLine(drawingContext, border, e.StartTime, e.EndTime, 3);
                }

            if (editorModel.WindowState.CurrentMode == EditorModes.Fixes)
                foreach (var e in model.SubtitleFixes)
                    DrawLine(drawingContext, fixes, e.StartTime, e.StartTime + e.Length, RowHeight / 2);

          
        }
    }
}
