using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Tuto.Navigator.Editor
{
   public class PatchData
    {
        public int Begin { get; set; }
        public int End { get; set; }
        public string Text { get; set; }
    }

    public class TempModel
    {
        public ObservableCollection<PatchData> Patches { get; private set; }

        public TempModel() { 
            Patches = new ObservableCollection<PatchData>();
            Patches.Add(new PatchData { Begin = 1000, End = 2000, Text = "A" });

        }
    }

    public class PatchPanel : TimelineBase
    {
        TempModel model = new TempModel();


        StreamGeometry GetRightGeometry(Point p)
        {
           var rightEdge = new StreamGeometry();
           using (StreamGeometryContext geometryContext = rightEdge.Open())
            {
                geometryContext.BeginFigure(new Point(p.X+0, p.Y+0), true, true);
                geometryContext.LineTo(new Point(p.X + 0, p.Y + RowHeight), true, false);
                geometryContext.LineTo(new Point(p.X + EdgeWidth, p.Y + EdgeHalf), true, false);
                geometryContext.LineTo(new Point(p.X + EdgeWidth, p.Y + 0), true, false);
            }
           return rightEdge;
        }

        StreamGeometry GetLeftGeometry(Point p)
        {
            var leftEdge = new StreamGeometry();
            using (StreamGeometryContext geometryContext = leftEdge.Open())
            {
                geometryContext.BeginFigure(new Point(p.X+0, p.Y+0), true, true);
                geometryContext.LineTo(new Point(p.X + 0, p.Y + EdgeHalf), true, false);
                geometryContext.LineTo(new Point(p.X + EdgeWidth, p.Y + RowHeight), true, false);
                geometryContext.LineTo(new Point(p.X + EdgeWidth, p.Y + 0), true, false);
            }
            return leftEdge;

        }



        int EdgeWidth { get { return RowHeight / 2; } }
        int EdgeHalf { get { return RowHeight / 2; } }

        SolidColorBrush Brush = Brushes.Azure;
        Pen Pen = new Pen(Brushes.Transparent, 0);


        void DrawGeometry(DrawingContext context, Point p, Func<Point,Geometry> geometry)
        {
            context.DrawGeometry(Brush, Pen, geometry(p));
            if (p.X + EdgeWidth > Width)
                context.DrawGeometry(Brush, Pen, geometry(new Point(Width - p.X - EdgeWidth, p.Y+RowHeight)));
            if (p.X < 0)
                context.DrawGeometry(Brush, Pen, geometry(new Point(Width + p.X, p.Y - RowHeight)));

        }

        void DrawPatch(DrawingContext context, PatchData data)
        {
            var point = GetCoordinate(data.Begin);
            point.X -= EdgeWidth;
            DrawGeometry(context, point, GetLeftGeometry);
            point = GetCoordinate(data.End);
            DrawGeometry(context, point, GetRightGeometry);
            foreach (var e in GetRects(data.Begin, data.End, 0, 1))
                context.DrawRectangle(Brush, Pen, e);
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            foreach (var e in model.Patches)
                DrawPatch(drawingContext, e);
        }
    }
}
