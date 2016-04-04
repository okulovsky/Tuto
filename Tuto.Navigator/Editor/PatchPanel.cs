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
            Patches.Add(new PatchData { Begin = 1000, End = 10000, Text = "A" });
            Patches.Add(new PatchData { Begin = 40000, End = 40000, Text = "B" });
            Patches.Add(new PatchData { Begin = 100000, End = 700000, Text = "C" });
        }
    }

    public class PatchPanel : TimelineBase
    {
        TempModel model = new TempModel();
        PatchData selectedPatch;

        public PatchPanel()
        {
            MouseDown += PatchPanel_MouseDown;
        }

        void PatchPanel_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var toDo = FindPatch(e.GetPosition(this));
            if (toDo == null) return;
            selectedPatch = toDo.Item2;
            InvalidateVisual();
        }

        enum ActionType
        {
            Drag,
            LeftDrag,
            RightDrag
        }


        bool TestEdge(Point p, int ms, int deltaX, ref double deltaY)
        {
            var c= GetCoordinate(ms);
            var min = Math.Min(c.X, c.X + deltaX);
            var max = Math.Max(c.X, c.X + deltaX);
            if (min<=p.X && p.X<=max)
            {
                deltaY=p.Y-c.Y;
                return true;
            }
            return false;
        }

        Tuple<ActionType,PatchData> FindPatch(Point p)
        {
            foreach(var e in model.Patches)
            {
                foreach (var r in GetRects(e.Begin, e.End, 0, 1))
                    if (r.Contains(p))
                        return Tuple.Create(ActionType.Drag, e);


                ActionType? type = null;
                double delta = 0;
                if (TestEdge(p, e.Begin, -EdgeWidth, ref delta)) type = ActionType.LeftDrag;
                if (TestEdge(p, e.End, EdgeWidth, ref delta)) type = ActionType.RightDrag;

                if (type == null) continue;

                if (e.End - e.Begin > 100)
                    return Tuple.Create(type.Value, e);

                if (delta < EdgeHalf)
                    return Tuple.Create(type.Value, e);

                return Tuple.Create(ActionType.Drag, e);

            }
            return null;
        }

        StreamGeometry GetLeftGeometry(Point p)
        {
           var rightEdge = new StreamGeometry();
           using (StreamGeometryContext geometryContext = rightEdge.Open())
            {
                geometryContext.BeginFigure(new Point(p.X+EdgeWidth, p.Y+0), true, true);
                geometryContext.LineTo(new Point(p.X + 0, p.Y + EdgeHalf), true, false);
                geometryContext.LineTo(new Point(p.X + 0, p.Y + RowHeight), true, false);
                geometryContext.LineTo(new Point(p.X + EdgeWidth, p.Y + RowHeight), true, false);
                geometryContext.LineTo(new Point(p.X + EdgeWidth, p.Y + 0), false, false);
            }
           return rightEdge;
        }

        StreamGeometry GetRightGeometry(Point p)
        {
            var leftEdge = new StreamGeometry();
            using (StreamGeometryContext geometryContext = leftEdge.Open())
            {
                geometryContext.BeginFigure(new Point(p.X+0, p.Y+0), true, true);
                geometryContext.LineTo(new Point(p.X + EdgeWidth, p.Y + EdgeHalf), true, false);
                geometryContext.LineTo(new Point(p.X + EdgeWidth, p.Y + RowHeight), true, false);
                geometryContext.LineTo(new Point(p.X + 0, p.Y + RowHeight), true, false);
                geometryContext.LineTo(new Point(p.X + 0, p.Y + 0), false, false);
            }
            return leftEdge;
        }



        int EdgeWidth { get { return RowHeight / 2; } }
        int EdgeHalf { get { return RowHeight / 2; } }

        SolidColorBrush Brush = Brushes.Blue;
        Pen Pen = new Pen(Brushes.Transparent, 0);
        Pen SelectedPen = new Pen(Brushes.Gold, 2);


        void DrawGeometry(DrawingContext context, Point p, Func<Point,Geometry> geometry, Pen pen)
        {
            context.DrawGeometry(Brush, pen, geometry(p));
            if (p.X + EdgeWidth > Width)
                context.DrawGeometry(Brush, pen, geometry(new Point(Width - p.X - EdgeWidth, p.Y + RowHeight)));
            if (p.X < 0)
                context.DrawGeometry(Brush, pen, geometry(new Point(Width + p.X, p.Y - RowHeight)));

        }

        void DrawPatch(DrawingContext context, PatchData data)
        {

            var pen = data == selectedPatch ? SelectedPen : Pen;
            foreach (var e in GetRects(data.Begin, data.End, 0, 1))
            {
                context.DrawRectangle(Brush, Pen, e);
                context.DrawLine(pen, e.TopLeft, e.TopRight);
                context.DrawLine(pen, e.BottomLeft, e.BottomRight);

            }
            var point = GetCoordinate(data.Begin);
            point.X -= EdgeWidth-1;
            DrawGeometry(context, point, GetLeftGeometry, pen);
            point = GetCoordinate(data.End);
            point.X -= 1;
            DrawGeometry(context, point, GetRightGeometry, pen);
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            foreach (var e in model.Patches)
                DrawPatch(drawingContext, e);
        }
    }
}
