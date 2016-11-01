using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace VolumeWatcher.UI
{
    /*
    public static class ExtensionMethods
    {
        private static Action EmptyDelegate = delegate () { };

        public static void Refresh(this UIElement uiElement)
        {
            uiElement.Dispatcher.Invoke(DispatcherPriority.Render, EmptyDelegate);
        }
    }
    */

    public delegate void ValueChangedDelegate(object sender, int value);

    /// <summary>
    /// LevelBar.xaml の相互作用ロジック
    /// </summary>
    public partial class LevelBar : UserControl
    {
        int min = 0;         // Minimum value for progress range
        int max = 100;       // Maximum value for progress range
        //int val = 0;         // Current progress
        const int headHeight = 12;

        Brush _HeadColor = Brushes.White;
        Brush _BodyColor = Brushes.Purple;
        Brush _BackColor = Brushes.Gray;

        public event ValueChangedDelegate OnValueChanged;

        /*
        protected override void OnResize(EventArgs e)
        {
            // Invalidate the control to get a repaint.
            this.Invalidate();
        }
        */



        //*
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            "Value",
            typeof(int),
            typeof(LevelBar),
            new FrameworkPropertyMetadata(0, new PropertyChangedCallback(_OnValueChanged)));

        public static void _OnValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var self = (LevelBar)sender;
            self.InvalidateVisual();
            self.OnValueChanged?.Invoke(sender, (int)e.NewValue);
        }
        //*/

        public LevelBar()
        {
            InitializeComponent();
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            var val = Value;
            Rect rect = new Rect(0, 0, ActualWidth, ActualHeight);
            drawingContext.DrawRectangle(_BackColor, null, rect);

            Rect rectHead = new Rect(rect.X, rect.Y, rect.Width, headHeight);
            var percent = (float)(val - min) / (float)(max - min);

            var temp = (int)Math.Ceiling(percent * (rect.Height - headHeight));
            temp = Math.Max(0, temp);
            rect.Y = rect.Height - temp;
            rect.Height = temp;
            rectHead.Y = rect.Y - headHeight;

            //*/
            drawingContext.DrawRectangle(_HeadColor, null, rectHead);
            drawingContext.DrawRectangle(_BodyColor, null, rect);
            /*/
            this.rectHead.Height = rectHead.Height;
            this.rectBody.Height = rect.Height;
            //*/
        }

        public int Minimum
        {
            get
            {
                return min;
            }
            set
            {
                min = value;

                if (min > max) max = min;
                if (Value < min) Value = min;

                // Invalidate the control to get a repaint.
                this.InvalidateVisual();
            }
        }

        public int Maximum
        {
            get
            {
                return max;
            }
            set
            {
                max = value;

                if (max < min) min = max;
                if (Value > max) Value = max;

                // Invalidate the control to get a repaint.
                this.InvalidateVisual(); 
            }
        }

        public int Value
        {
            get
            {
                //return val;
                return (int)this.GetValue(ValueProperty);
            }
            set
            {
                //val = Math.Max(min, Math.Min(value, max));
                var v = Math.Max(min, Math.Min(value, max));
                this.SetValue(ValueProperty, v);
                                
                this.InvalidateVisual();
            }
        }

        public Brush HeadColor
        {
            get
            {
                return _HeadColor;
            }
            set
            {
                _HeadColor = value;
                
                this.InvalidateVisual();
            }
        }

        public Brush BarColor
        {
            get
            {
                return _BodyColor;
            }
            set
            {
                _BodyColor = value;

                this.InvalidateVisual();
            }
        }

        public Brush BackColor
        {
            get
            {
                return _BackColor;
            }
            set
            {
                _BackColor = value;

                this.InvalidateVisual();
            }
        }

        /*
        private void Draw3DBorder(Graphics g)
        {
            int PenWidth = (int)Pens.White.Width;

            g.DrawLine(Pens.DarkGray,
                new Point(this.ClientRectangle.Left, this.ClientRectangle.Top),
                new Point(this.ClientRectangle.Width - PenWidth, this.ClientRectangle.Top));
            g.DrawLine(Pens.DarkGray,
                new Point(this.ClientRectangle.Left, this.ClientRectangle.Top),
                new Point(this.ClientRectangle.Left, this.ClientRectangle.Height - PenWidth));
            g.DrawLine(Pens.White,
                new Point(this.ClientRectangle.Left, this.ClientRectangle.Height - PenWidth),
                new Point(this.ClientRectangle.Width - PenWidth, this.ClientRectangle.Height - PenWidth));
            g.DrawLine(Pens.White,
                new Point(this.ClientRectangle.Width - PenWidth, this.ClientRectangle.Top),
                new Point(this.ClientRectangle.Width - PenWidth, this.ClientRectangle.Height - PenWidth));
        }

        private void DrawBorder(Graphics g)
        {
            int PenWidth = (int)Pens.White.Width;

            g.DrawRectangle(Pens.Aqua, this.ClientRectangle.X, this.ClientRectangle.Y, this.ClientRectangle.Width - PenWidth, this.ClientRectangle.Height - PenWidth);

        }
        */
    }
}
