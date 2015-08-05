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
            SetProgressBorder();
            SetLowerValueVisibility();
        }

        private void SetProgressBorder()
        {
            double lowerPoint = (this.ActualWidth * (LowerValue - Minimum)) / (Maximum - Minimum);
            double upperPoint = (this.ActualWidth * (UpperValue - Minimum)) / (Maximum - Minimum);
            upperPoint = this.ActualWidth - upperPoint;
            progressBorder.Margin = new Thickness(lowerPoint, 0, upperPoint, 0);
        }

        public void SetLowerValueVisibility()
        {
            if (DisableLowerValue)
            {
                LowerSlider.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                LowerSlider.Visibility = System.Windows.Visibility.Visible;
            }
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

        public double LowerValue
        {
            get { return (double)GetValue(LowerValueProperty); }
            set { SetValue(LowerValueProperty, value); }
        }

        public double UpperValue
        {
            get { return (double)GetValue(UpperValueProperty); }
            set { SetValue(UpperValueProperty, value); }
        }

        public bool DisableLowerValue
        {
            get { return (bool)GetValue(DisableLowerValueProperty); }
            set { SetValue(DisableLowerValueProperty, value); }
        }

        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register("Minimum", typeof(double), typeof(RangeSlider), new UIPropertyMetadata(0d, new PropertyChangedCallback(PropertyChanged)));

        public static readonly DependencyProperty LowerValueProperty =
            DependencyProperty.Register("LowerValue", typeof(double), typeof(RangeSlider), new UIPropertyMetadata(10d, new PropertyChangedCallback(PropertyChanged)));

        public static readonly DependencyProperty UpperValueProperty =
            DependencyProperty.Register("UpperValue", typeof(double), typeof(RangeSlider), new UIPropertyMetadata(90d, new PropertyChangedCallback(PropertyChanged)));

        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register("Maximum", typeof(double), typeof(RangeSlider), new UIPropertyMetadata(100d, new PropertyChangedCallback(PropertyChanged)));

        public static readonly DependencyProperty DisableLowerValueProperty =
            DependencyProperty.Register("DisableLowerValue", typeof(bool), typeof(RangeSlider), new UIPropertyMetadata(false, new PropertyChangedCallback(DisabledLowerValueChanged)));

        private static void DisabledLowerValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RangeSlider slider = (RangeSlider)d;
            slider.SetLowerValueVisibility();
        }

        private static void PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RangeSlider slider = (RangeSlider)d;
            if (e.Property == RangeSlider.LowerValueProperty)
            {
                slider.UpperSlider.Value = Math.Max(slider.UpperSlider.Value, slider.LowerSlider.Value);
            }
            else if (e.Property == RangeSlider.UpperValueProperty)
            {
                slider.LowerSlider.Value = Math.Min(slider.UpperSlider.Value, slider.LowerSlider.Value);
            }
            slider.SetProgressBorder();
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

            var pixPerUnit = this.Width / Math.Abs(Maximum - Minimum);

            var leftShift = (LowerSlider.Value - LowerSlider.Minimum) * pixPerUnit;
            var rightShift = UpperSlider.Value * pixPerUnit;

            double left = Canvas.GetLeft(this) + leftShift;
            double right = Canvas.GetLeft(this) + rightShift;

            

            const double GAP = 10;
            if (point.X - left < GAP)
            {
                return HitType.L;
            }
            if (right - point.X < GAP)
            {
                return HitType.R;
            }
            return HitType.Body;
        }

        private void SetMouseCursor()
        {
            //Cursor desired_cursor = Cursors.Arrow;
            //switch (MouseHitType)
            //{
            //    case HitType.Body:
            //        desired_cursor = Cursors.ScrollWE;
            //        break;
            //}
            //if (Cursor != desired_cursor) Cursor = desired_cursor;
        }


        private void root_MouseUp(object sender, MouseButtonEventArgs e)
        {
            DragInProgress = false;
            ReleaseMouseCapture();
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
                Point point = Mouse.GetPosition((Canvas)this.Parent);
                double offset_x = point.X - LastPoint.X;
                double new_x = Canvas.GetLeft(this);
                new_x += offset_x;
                Canvas.SetLeft(this, new_x);
                LastPoint = point;
            }
        }

        private void root_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MouseHitType = SetHitType(Mouse.GetPosition((Canvas)this.Parent));
            SetMouseCursor();
            LastPoint = Mouse.GetPosition((Canvas)this.Parent);
            
            if (MouseHitType == HitType.Body)
            {
                CaptureMouse();
                DragInProgress = true;
            }
        }

        private Point GetPosition()
        {
            return new Point(0, 0);
        }

    }
}