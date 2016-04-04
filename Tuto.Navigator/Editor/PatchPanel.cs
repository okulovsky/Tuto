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


 public enum SelectionType
    {
        Drag,
        LeftDrag,
        RightDrag
    }

    public class PatchPanelSelection
    {
        public readonly PatchData Item;
        public readonly SelectionType Type;
        public Point SelectionStart;
        public PatchPanelSelection(SelectionType type, PatchData item, Point selectionStart)
        {
            this.Item=item;
            this.Type=type;
            this.SelectionStart = selectionStart;
        }
    }

    public class PatchPanel : TimelineBase
    {
        TempModel model = new TempModel();
        PatchPanelSelection selection;
        bool drag;

        public PatchPanel()
        {
            MouseDown += PatchPanel_MouseDown;
            MouseUp += PatchPanel_MouseUp;
            MouseMove += PatchPanel_MouseMove;
        }

        void PatchPanel_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (!drag || selection == null) return;
            if (e.LeftButton!= System.Windows.Input.MouseButtonState.Pressed)
            {
                drag = false;
                return;
            }
            var thisMs = MsAtPoint(e.GetPosition(this));
            var oldMs = MsAtPoint(selection.SelectionStart);
            var delta = thisMs - oldMs;
            selection.SelectionStart=e.GetPosition(this);
            switch(selection.Type)
            {
                case SelectionType.Drag:
                    selection.Item.Begin += delta;
                    selection.Item.End += delta;
                    break;
                case SelectionType.LeftDrag:
                    selection.Item.Begin += delta;
                    if (selection.Item.Begin > selection.Item.End)
                        selection.Item.Begin = selection.Item.End;
                    break;
                case SelectionType.RightDrag:
                    selection.Item.End += delta;
                    if (selection.Item.End < selection.Item.Begin)
                        selection.Item.End = selection.Item.Begin;
                    break;
            }
            InvalidateVisual();
        }

        void PatchPanel_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            drag = false;
        }

        void PatchPanel_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            selection = FindSelection(e.GetPosition(this));
            drag = selection != null;
            InvalidateVisual();
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

        PatchPanelSelection FindSelection(Point p)
        {
            foreach(var e in model.Patches)
            {
                foreach (var r in GetRects(e.Begin, e.End, 0, 1))
                    if (r.Contains(p))
                        return new PatchPanelSelection(SelectionType.Drag, e, p);


                SelectionType? type = null;
                double delta = 0;
                if (TestEdge(p, e.Begin, -EdgeWidth, ref delta)) type = SelectionType.LeftDrag;
                if (TestEdge(p, e.End, EdgeWidth, ref delta)) type = SelectionType.RightDrag;

                if (type == null) continue;

                if (e.End - e.Begin < 100 && delta > EdgeHalf)
                    type = SelectionType.Drag;

                return new PatchPanelSelection(type.Value, e, p);

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

            var pen = Pen;
            if (selection != null && selection.Item == data) pen = SelectedPen;

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
