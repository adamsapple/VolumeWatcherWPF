using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Runtime.InteropServices;

namespace Moral.Util
{
    /// <summary>
    /// SourceInitializedイベント内でCallすると良い
    /// </summary>
    public static class NoActiveWindowExtension
    {
        [DllImport("user32.dll")]
        private static extern IntPtr SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_NOACTIVATE = 0x08000000;


        public static void NoActiveWindow(this Window window)
        {
            var helper = new WindowInteropHelper(window);
            var style  = GetWindowLong(helper.Handle, GWL_EXSTYLE) | WS_EX_NOACTIVATE;

            SetWindowLong(helper.Handle, GWL_EXSTYLE, style );
        }
    }
}
