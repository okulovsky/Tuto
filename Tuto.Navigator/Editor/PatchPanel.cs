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
            var createSubs = new MenuItem { Header = "Add subtitles" };
            createSubs.Click += AddSubtitles;
            var createVideo = new MenuItem { Header = "Add video" };
            createVideo.Click += AddVideo;

            var createImage = new MenuItem { Header = "Add image" };
            createImage.Click += createImage_Click;

            forExisting = new ContextMenu { Items = { delete } };
            forEmpty = new ContextMenu { Items = { createSubs, createVideo, createImage } };
        }

        void createImage_Click(object sender, RoutedEventArgs e)
        {
            var ms = MsAtPoint(menuCalled);
            model.Patches.Add(new Patch { Begin = ms, End = ms + 1000, Data = new ImagePatch { RelativeFilePath = "test.jpg" } });
       
        }

        void AddVideo(object sender, RoutedEventArgs e)
        {
            var ms = MsAtPoint(menuCalled);
            model.Patches.Add(new Patch { Begin = ms, End = ms + 1000, Data = new VideoFilePatch { RelativeFileName = "test.mp4" } });
        }

        void AddSubtitles(object sender, RoutedEventArgs e)
        {
            var ms = MsAtPoint(menuCalled);
            model.Patches.Add(new Patch { Begin = ms, End = ms + 1000, Data = new SubtitlePatch { Text = "AAAAAAAAA!" } });
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
                foreach (var r in GetRects(e.Begin, e.End, RelativeBarHeight,1))
                    if (r.Contains(p))
                        return new PatchSelection(SelectionType.Drag, e, p.X,p.Y);


                SelectionType? type = null;

                if (GetLeftGeometries(e).Any(z => z.FillContains(p)))
                    type = SelectionType.LeftDrag;
                if (GetRightGeometries(e).Any(z => z.FillContains(p)))
                    type = SelectionType.RightDrag;

                if (type == null) continue;

                var delta = ((int)p.Y) % RowHeight;
                
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
				geometryContext.LineTo(new Point(p.X + EdgeWidth / 2, p.Y + EdgeHalf), true, false);
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
				geometryContext.LineTo(new Point(p.X + EdgeWidth / 2, p.Y + EdgeHalf), true, false);
				geometryContext.LineTo(new Point(p.X + EdgeWidth, p.Y + EdgeHalf), true, false);
				geometryContext.LineTo(new Point(p.X + EdgeWidth, p.Y + RowHeight), true, false);
                geometryContext.LineTo(new Point(p.X + 0, p.Y + RowHeight), true, false);
                geometryContext.LineTo(new Point(p.X + 0, p.Y + 0), false, false);
            }
            return leftEdge;
        }



		double RelativeBarHeight { get { return (double)EdgeHalf / RowHeight; } }

        SolidColorBrush SubtitlesBrush = Brushes.Blue;
        SolidColorBrush VideoBrush = Brushes.Green;
        SolidColorBrush ImageBrush = Brushes.Red;
        Pen Pen = new Pen(Brushes.Transparent, 0);
        Pen SelectedPen = new Pen(Brushes.Gold, 2);


        IEnumerable<StreamGeometry> GetLeftGeometries(Patch data)
        {
            var p = GetCoordinate(data.Begin);
            p.X -= EdgeWidth - 1;
            yield return GetLeftGeometry(p);
            if (p.X + EdgeWidth > Width)
                yield return GetLeftGeometry(new Point(Width - p.X - EdgeWidth, p.Y + RowHeight));
            if (p.X < 0)
                yield return GetLeftGeometry(new Point(Width + p.X, p.Y - RowHeight));
        }

        IEnumerable<StreamGeometry> GetRightGeometries(Patch data)
        {   
            var p = GetCoordinate(data.End);
            p.X -= 1;
            yield return GetRightGeometry(p);
            if (p.X + EdgeWidth > Width)
                yield return GetRightGeometry(new Point(Width - p.X - EdgeWidth, p.Y + RowHeight));
            if (p.X < 0)
                yield return GetRightGeometry(new Point(Width + p.X, p.Y - RowHeight));

        }


        void DrawPatch(DrawingContext context, Patch data)
        {

            var pen = Pen;
            if (selection != null && selection.Item == data) pen = SelectedPen;

            var brush = SubtitlesBrush;
            if (data.IsVideoPatch) brush = VideoBrush;
            if (data.Data is ImagePatch) brush = ImageBrush;
            
            foreach (var e in GetRects(data.Begin, data.End, RelativeBarHeight, 1))
            {
                context.DrawRectangle(brush, Pen, e);
                context.DrawLine(pen, e.TopLeft, e.TopRight);
                context.DrawLine(pen, e.BottomLeft, e.BottomRight);
                
            }
            foreach(var e in GetLeftGeometries(data).Concat(GetRightGeometries(data)))
            {
                context.DrawGeometry(brush, pen, e);
            }
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
