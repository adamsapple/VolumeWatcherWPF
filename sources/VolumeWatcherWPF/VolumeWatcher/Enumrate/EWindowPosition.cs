using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows;

namespace VolumeWatcher.Enumrate
{
    /// <summary>
    /// Volumeウィンドウの位置に関する定数
    /// </summary>
    public enum EWindowPosition
    {
        /// <summary>未設定を示す値</summary>
        UNKNOWN,
        /// <summary>左上を示す値</summary>
        LEFT_TOP,

        /// <summary>右上を示す値</summary>
        RIGHT_TOP,

        /// <summary>左下を示す値</summary>
        LEFT_BOTTOM,

        /// <summary>右下を示す値</summary>
        RIGHT_BOTTOM
    }

    internal static class EVolumeWindowPositionExtMethod
    {
        public static void SetWindowPosition(this Window win, EWindowPosition epos)
        {
            Rect rect = new Rect(0, 0, win.ActualWidth, win.ActualHeight);

            if(rect.Width==0 || rect.Height == 0) {
                //win.Measure(new Size(win.Width, win.Height));
                //win.Arrange(new Rect(0, 0, win.Width, win.Height));
                win.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                Size desire = win.DesiredSize;
                win.Arrange(new Rect(0, 0, desire.Width, desire.Height));

                rect.Width  = win.ActualWidth;
                rect.Height = win.ActualHeight;
            }
            

            //Rect margin = new Rect(win.Margin.Left, win.Margin.Top, win.Margin.Right, win.Margin.Bottom);
            //Rect screen = new Rect(0, 0, System.Windows.SystemParameters.PrimaryScreenWidth, System.Windows.SystemParameters.PrimaryScreenHeight);
            Rect screen = new Rect(0, 0, System.Windows.SystemParameters.WorkArea.Width, System.Windows.SystemParameters.WorkArea.Height);

            //Rectangle rect = formVolume.Bounds;
            //Rectangle margin = formVolume.Margin;
            //Rectangle screen = Screen.PrimaryScreen.WorkingArea;

            // ポジションパターンによる位置変更
            switch (epos)
            {
                case EWindowPosition.LEFT_TOP:        // 左上
                    rect.X = win.Margin.Left;
                    rect.Y = win.Margin.Top;
                    break;
                case EWindowPosition.RIGHT_TOP:       // 右上
                    rect.X = screen.Width - rect.Width - win.Margin.Right - 0;
                    rect.Y = win.Margin.Top;
                    break;
                case EWindowPosition.LEFT_BOTTOM:     // 左下
                    rect.X = win.Margin.Left;
                    rect.Y = screen.Height - rect.Height - win.Margin.Bottom - 0;
                    break;
                case EWindowPosition.RIGHT_BOTTOM:    // 右下
                    rect.X = screen.Width - rect.Width - win.Margin.Right - 0;
                    rect.Y = screen.Height - rect.Height - win.Margin.Bottom - 0;
                    break;
            }
            //formVolume.Bounds = rect;
            win.Left = rect.X;
            win.Top = rect.Y;
        }
    }
}
