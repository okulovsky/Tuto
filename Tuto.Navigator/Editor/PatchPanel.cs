using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Tuto.Model;

namespace Tuto.Navigator.Editor
{

    public class PatchPanel : TimelineBase
    {
        bool drag;
        Point menuCalled;
        PatchSelection selection
        {
            get { return editorModel.WindowState.PatchSelection; }
            set { editorModel.WindowState.PatchSelection = value; }
        }

        public PatchPanel() 
        {
            MouseDown += PatchPanel_MouseDown;
            MouseUp += PatchPanel_MouseUp;
            MouseMove += PatchPanel_MouseMove;
          
            Background = new SolidColorBrush(Colors.Transparent);

            var delete = new MenuItem { Header = "Delete" };
            delete.Click += delete_Click;
            var create = new MenuItem { Header = "Create" };
            create.Click += create_Click;

            forExisting = new ContextMenu { Items = { delete } };
            forEmpty = new ContextMenu { Items = { create } };
        }

        void create_Click(object sender, RoutedEventArgs e)
        {
            var ms = MsAtPoint(menuCalled);
            model.Patches.Add(new Patch { Begin = ms, End = ms + 1000, Subtitles = new SubtitlePatch { Text = "AAAAAAAAA!" } });
        }

        void delete_Click(object sender, RoutedEventArgs e)
        {
            if (selection != null)
                editorModel.Montage.Patches.Remove(selection.Item);
            selection = null;
            InvalidateVisual();
        }

        ContextMenu forExisting, forEmpty;

        void PatchPanel_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (editorModel == null) return;

            if (!drag || selection == null) return;
            if (e.LeftButton!= System.Windows.Input.MouseButtonState.Pressed)
            {
                StopDrag();
                return;
            }
            var thisMs = MsAtPoint(e.GetPosition(this));
            var oldMs = MsAtPoint(new Point(selection.SelectionStartX,selection.SelectionStartY));
            var delta = thisMs - oldMs;
            var p=e.GetPosition(this);
            selection.SelectionStartX = p.X;
            selection.SelectionStartY = p.Y;

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

        void StopDrag()
        {
             drag = false;
        }

        void PatchPanel_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

            if (editorModel == null) return;
            StopDrag();
        }

        void PatchPanel_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

            if (editorModel == null) return;
            selection = FindSelection(e.GetPosition(this));
            drag = selection != null;
            InvalidateVisual();

            if (selection == null && e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
                OnTimeSelected(MsAtPoint(e.GetPosition(this)), false);


            if (e.RightButton== System.Windows.Input.MouseButtonState.Pressed)
            {
                menuCalled = e.GetPosition(this);
                ContextMenu=selection==null?forEmpty:forExisting;
                //ContextMenu.IsOpen=true;
            }
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

        PatchSelection FindSelection(Point p)
        {
            foreach(var e in model.Patches)
            {
                foreach (var r in GetRects(e.Begin, e.End, 0, RelativeBarHeight))
                    if (r.Contains(p))
                        return new PatchSelection(SelectionType.Drag, e, p.X,p.Y);


                SelectionType? type = null;
                double delta = 0;
                if (TestEdge(p, e.Begin, -EdgeWidth, ref delta)) type = SelectionType.LeftDrag;
                if (TestEdge(p, e.End, EdgeWidth, ref delta)) type = SelectionType.RightDrag;

                if (type == null) continue;

                if (e.End - e.Begin < 100 && delta > EdgeHalf)
                    type = SelectionType.Drag;

                return new PatchSelection(type.Value, e, p.X,p.Y);

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

        double RelativeBarHeight { get { return 0.6; } }

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

        void DrawPatch(DrawingContext context, Patch data)
        {

            var pen = Pen;
            if (selection != null && selection.Item == data) pen = SelectedPen;

            
            foreach (var e in GetRects(data.Begin, data.End, 0, RelativeBarHeight))
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
            if (editorModel == null) return;
            base.OnRender(drawingContext);
            foreach (var e in model.Patches)
                DrawPatch(drawingContext, e);
        }
    }
}
