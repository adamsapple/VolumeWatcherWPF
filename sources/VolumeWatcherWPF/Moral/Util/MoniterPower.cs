using System;
using System.Runtime.InteropServices;

namespace Moral.Util
{
    public static class MoniterPower
    {

        private const int HWND_BROADCAST = -1;
        private const uint WM_SYSCOMMAND = 0x0112;
        private const int SC_MONITORPOWER = 0xF170;
        private const int POWER_ON = -1;
        private const int POWER_OFF = 2;
        private const int SC_SCREENSAVE = 0xF140;


        public static void PowerOff()
        {
            NativeMethods.PostMessage(HWND_BROADCAST, WM_SYSCOMMAND, SC_MONITORPOWER, POWER_OFF);
        }

        public static void PowerOn()
        {
            NativeMethods.PostMessage(HWND_BROADCAST, WM_SYSCOMMAND, SC_MONITORPOWER, POWER_ON);
        }


        public static void ExecScreenSaver()
        {
            NativeMethods.SendMessage(HWND_BROADCAST, WM_SYSCOMMAND, SC_SCREENSAVE, 0);

        }
    }
}