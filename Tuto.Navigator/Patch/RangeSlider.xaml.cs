using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Tuto.Model;

namespace Tuto.Navigator
{
    /// <summary>
    /// Interaction logic for RangeSlider.xaml
    /// </summary>
    public partial class RangeSlider : UserControl
    {
        public RangeSlider()
        {
            this.InitializeComponent();
            this.LayoutUpdated += new EventHandler(RangeSlider_LayoutUpdated);
        }

        void RangeSlider_LayoutUpdated(object sender, EventArgs e)
        {
            Track.Width = EndSecond - StartSecond;
        }

        private void SetProgressBorder()
        {
            //double lowerPoint = (this.ActualWidth * (LeftShift - Minimum)) / (Maximum - Minimum);
            //double upperPoint = (this.ActualWidth * (EndSecond - Minimum)) / (Maximum - Minimum);
            //upperPoint = this.ActualWidth - upperPoint;
            //progressBorder.Margin = new Thickness(lowerPoint, 0, upperPoint, 0);
        }

        public void SetLowerValueVisibility()
        {
            //if (DisableLowerValue)
            //{
            //    LowerSlider.Visibility = System.Windows.Visibility.Collapsed;
            //}
            //else
            //{
            //    LowerSlider.Visibility = System.Windows.Visibility.Visible;
            //}
        }

        public double Minimum
        {
            get { return (double)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

        public double Maximum
        {
            get { return (double)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        public double LeftShift
        {
            get { return (double)GetValue(LeftShiftValueProperty); }
            set { SetValue(LeftShiftValueProperty, value); }
        }

        public double EndSecond
        {
            get { return (double)GetValue(UpperValueProperty); }
            set { SetValue(UpperValueProperty, value);  }
        }

        public double StartSecond
        {
            get { 
                return (double)GetValue(LowerValueProperty); 
            }
            set { SetValue(LowerValueProperty, value); }
        }

        public double Duration
        {
            get { return (double)GetValue(DurationProperty); }
            set {  SetValue(DurationProperty, value); }
        }

        public double CurrentWidth
        {
            get {
                return (double)GetValue(CurrentWidthProperty);
            }
            set
            {
                Track.Width = value; 
                SetValue(CurrentWidthProperty, value); }
        }

        public double Scale
        {
            get { return (double)GetValue(ScaleProperty); }
            set { SetValue(ScaleProperty, value); }
        }

        public static readonly DependencyProperty ScaleProperty =
            DependencyProperty.Register("Scale", typeof(double), typeof(RangeSlider), new UIPropertyMetadata(0d, new PropertyChangedCallback(PropertyChanged)));

        public static readonly DependencyProperty CurrentWidthProperty =
            DependencyProperty.Register("CurrentWidth", typeof(double), typeof(RangeSlider), new UIPropertyMetadata(0d, new PropertyChangedCallback(a)));

        public static readonly DependencyProperty DurationProperty =
            DependencyProperty.Register("Duration", typeof(double), typeof(RangeSlider), new UIPropertyMetadata(0d, new PropertyChangedCallback(PropertyChanged)));

        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register("Minimum", typeof(double), typeof(RangeSlider), new UIPropertyMetadata(0d, new PropertyChangedCallback(PropertyChanged)));

        public static readonly DependencyProperty LeftShiftValueProperty =
            DependencyProperty.Register("LeftShift", typeof(double), typeof(RangeSlider), new UIPropertyMetadata(10d, new PropertyChangedCallback(PropertyChanged)));

        public static readonly DependencyProperty LowerValueProperty =
            DependencyProperty.Register("StartSecond", typeof(double), typeof(RangeSlider), new UIPropertyMetadata(10d, new PropertyChangedCallback(PropertyChanged)));

        public static readonly DependencyProperty UpperValueProperty =
            DependencyProperty.Register("EndSecond", typeof(double), typeof(RangeSlider), new UIPropertyMetadata(90d, new PropertyChangedCallback(PropertyChanged)));

        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register("Maximum", typeof(double), typeof(RangeSlider), new UIPropertyMetadata(100d, new PropertyChangedCallback(PropertyChanged)));

        private static void a(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }

        private static void PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            
        }

        private enum HitType
        {
            None, Body, L, R
        };

        public bool DragInProgress = false;

        private Point LastPoint;

        HitType MouseHitType = HitType.None;

        private HitType SetHitType(Point point)
        {
            var a = Track.Width;
            double left = LeftShift;
            double top = Canvas.GetTop(this);
            double right = left + Track.Width;
            double bottom = top + Track.Height;
            if (point.X < left) return HitType.None;
            if (point.X > right) return HitType.None;

            const double GAP = 10;
            if (point.X - left < GAP)
            {
                // Left edge.
                return HitType.L;
            }
            if (right - point.X < GAP)
            {
                // Right edge.
                return HitType.R;
            }
            return HitType.Body;
        }

        private void SetMouseCursor()
        {
            Cursor desired_cursor = Cursors.Arrow;
            switch (MouseHitType)
            {
                case HitType.R:
                    desired_cursor = Cursors.ScrollE;
                    break;

                case HitType.L:
                    desired_cursor = Cursors.ScrollW;
                    break;
            }
            if (Cursor != desired_cursor) Cursor = desired_cursor;
        }


        private void root_MouseUp(object sender, MouseButtonEventArgs e)
        {
            
            ReleaseMouseCapture();
            DragInProgress = false;
        }

        private void root_MouseMove(object sender, MouseEventArgs e)
        {
            if (!DragInProgress)
            {
                MouseHitType = SetHitType(Mouse.GetPosition((Canvas)this.Parent));
                SetMouseCursor();
            }
            else
            {
                // See how much the mouse has moved.
                Point point = Mouse.GetPosition((Canvas)this.Parent);
                double offset_x = point.X - LastPoint.X;

                // Get the rectangle's current position.
                double new_x = LeftShift;
                double new_width = Track.Width;

                // Update the rectangle.
                switch (MouseHitType)
                {
                    case HitType.Body:
                        new_x += offset_x;
                        break;
                    case HitType.L:
                        new_x += offset_x;
                        new_width -= offset_x;
                        break;
                    case HitType.R:
                        new_width += offset_x;
                        break;
                }

                // Don't use negative width or height.
                if ((new_width > 0))
                {
                    
                    // Update the rectangle.

                    CurrentWidth = new_width;

                    if (MouseHitType == HitType.L)
                    {
                        StartSecond += offset_x;
                    }

                    

                    EndSecond = StartSecond + new_width;
                    
                    LeftShift = new_x;
                    LastPoint = point;
                    // Save the mouse's new location.
                }
            }
        }

        private void root_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MouseHitType = SetHitType(Mouse.GetPosition((Canvas)this.Parent));
            SetMouseCursor();
            if (MouseHitType == HitType.None) return;
            LastPoint = Mouse.GetPosition((Canvas)this.Parent);
            
            DragInProgress = true;
            CaptureMouse();
        }
    }
}