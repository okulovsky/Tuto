using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Tuto.Model;

namespace Tuto.Navigator.Editor
{
    public class TimelineBase : UserControl
    {

        protected EditorModel editorModel { get { return (EditorModel)DataContext; } }
        protected MontageModel model { get { return editorModel.Montage; } }

        protected readonly int RowHeight = 20;
        protected readonly int msInRow = 300000;
        protected readonly int EdgeWidth = 20;

        protected int EdgeInMS { get { return (int)(msInRow * EdgeWidth / ActualWidth); } }

        protected const int EdgeHalf = 10;

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

        protected TimelineBase()
        {
            this.DataContextChanged += (s, a) =>
            {
                if (editorModel == null) return;
                InvalidateVisual();
                editorModel.WindowState.PropertyChanged += (ss, aa) => InvalidateVisual();
            };
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            return base.ArrangeOverride(finalSize);
        }



        protected IEnumerable<Rect> GetRects(int startMS, int endMS, double relativeY0, double relativeY1)
        {
            double SWidth = ActualWidth / msInRow;


            var length = endMS - startMS;

            int x = startMS % msInRow;
            int y = startMS / msInRow;

            while (true)
            {
                var min = Math.Min(length, msInRow-x);
                yield return new Rect(x * SWidth, y * RowHeight+RowHeight*relativeY0, min * SWidth, RowHeight*(relativeY1-relativeY0));
                length -= (msInRow - x);
                if (length <= 0) yield break;
                x = 0;
                y++;
            }
        }
        protected IEnumerable<Rect> GetRects(StreamChunk chunk)
        {
            return GetRects(chunk.StartTime, chunk.StartTime + chunk.Length, 0, 1);
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

        public event Action<int, bool> TimeSelected;

        protected void OnTimeSelected(int ms, bool alternative)
        {
            if (TimeSelected != null)
                TimeSelected(ms, alternative);
        }
    }


    public class Slider : TimelineBase
    {
        public Slider()
        {
            MouseDown += Slider_MouseDown;
        }

        void Slider_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (editorModel==null) return;
            OnTimeSelected(MsAtPoint(e.GetPosition(this)),e.RightButton == System.Windows.Input.MouseButtonState.Pressed);
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            if (editorModel == null) return;

            if (editorModel.WindowState.PatchPlaying != PatchPlayingType.PatchOnly)
            {
                var point = GetCoordinate(editorModel.WindowState.CurrentPosition);
                drawingContext.DrawLine(currentPen, point, new Point(point.X, point.Y + RowHeight));
            }

            if (editorModel.WindowState.PatchPlaying != PatchPlayingType.NoPatch 
                && editorModel.WindowState.CurrentPatch!=null 
                && editorModel.WindowState.CurrentPatch.IsVideoPatch
                )
            {
                var current = editorModel.WindowState.VideoPatchPosition;
                var availableLength = editorModel.WindowState.CurrentPatch.End - editorModel.WindowState.CurrentPatch.Begin + 2 * EdgeInMS;
                int whereSlider=0;
                
                if (editorModel.WindowState.CurrentPatch.VideoData.Duration>0)
                {
                    double k = ((double)current) / editorModel.WindowState.CurrentPatch.VideoData.Duration;
                    whereSlider = (int)(k * availableLength);
                }
                else
                {
                    whereSlider = current;
                }
                var  point = GetCoordinate(editorModel.WindowState.CurrentPatch.Begin-EdgeInMS+whereSlider);
                point.Y += RowHeight;
                drawingContext.DrawLine(currentPen, point, new Point(point.X, point.Y - EdgeHalf));

            }
        }
    }

    public class ModelView : TimelineBase
    {
        public ModelView()
        {
            MouseDown += ModelView_MouseDown;
        }

        void ModelView_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (editorModel == null) return;
            OnTimeSelected(MsAtPoint(e.GetPosition(this)), e.RightButton == System.Windows.Input.MouseButtonState.Pressed);
        }

        Brush soundBrush = new SolidColorBrush(Color.FromArgb(150, 0, 0, 0));
        Pen soundpen = new Pen(Brushes.Transparent, 0);
		Pen sync = new Pen(Brushes.Red,2);


        void FlushSoundLine(DrawingContext drawingContext, List<Point> points, double baseLine)
        {
            points.Insert(0, new Point(points[0].X, baseLine+RowHeight));
            points.Add(new Point(points[points.Count - 1].X, baseLine+RowHeight));
            StreamGeometry streamGeometry = new StreamGeometry();
            using (StreamGeometryContext geometryContext = streamGeometry.Open())
            {
                geometryContext.BeginFigure(points[0], true, true);
                geometryContext.PolyLineTo(points, true, false);
            }
            drawingContext.DrawGeometry(soundBrush, soundpen, streamGeometry);
        }

        public IEnumerable<Tuple<int,int,bool>> GetSoundPoints(IEnumerable<SoundInterval> soundIntervals)
        {
            foreach(var e in soundIntervals)
            {
                yield return Tuple.Create(e.StartTime,e.Volume,e.HasVoice);
                yield return Tuple.Create(e.EndTime,e.Volume,e.HasVoice);
            }
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
               var points=new List<Point>();
               double baseline=0;
               foreach(var e in GetSoundPoints(model.SoundIntervals))
               {
                   var c = GetCoordinate(e.Item1);
                   if (points.Count==0)
                   {
                       baseline=c.Y;
                   }
                   if (Math.Abs(c.Y-baseline)>0.001)
                   {
                       FlushSoundLine(drawingContext,points,baseline);
                       points.Clear();
                       continue;
                   }
                   points.Add(new Point(c.X,c.Y+RowHeight-RowHeight*( (0.5*e.Item2/100) + (e.Item3?0.2:0))));
               }
               if (points.Count!=0) FlushSoundLine(drawingContext,points,baseline);
           }

		   var ps = GetCoordinate(model.SynchronizationShift);
			StreamGeometry streamGeometry = new StreamGeometry();
            using (StreamGeometryContext geometryContext = streamGeometry.Open())
            {
                geometryContext.BeginFigure(new Point(ps.X-5,ps.Y),true,true);
				geometryContext.LineTo(new Point(ps.X+5,ps.Y),false,false);
				geometryContext.LineTo(new Point(ps.X,ps.Y+RowHeight/2),false,false);
			}
            drawingContext.DrawGeometry(Brushes.Red, soundpen, streamGeometry);

			//if (editorModel.WindowState.CurrentMode == EditorModes.Border && model.Borders!=null)
			//	foreach (var e in model.Borders)
			//	{
			//		DrawLine(drawingContext, border, e.StartTime, e.EndTime, 3);
			//	}
          
        }
    }
}
