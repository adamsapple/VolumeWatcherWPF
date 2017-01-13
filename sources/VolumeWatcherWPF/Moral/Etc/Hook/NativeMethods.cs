using System;
using System.Runtime.InteropServices;

namespace Moral.Etc.Hook
{
    delegate int HookHandler(int code, IntPtr message, IntPtr state);
    
    internal static class NativeMethods
    {
        [DllImport("user32.dll", SetLastError = true)]
        internal static extern IntPtr SetWindowsHookEx(EHookType hookType, HookHandler hookDelegate, IntPtr hInstance, uint threadId);
        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool UnhookWindowsHookEx(IntPtr hook);
        [DllImport("user32.dll", SetLastError = true)]
        internal static extern int CallNextHookEx(IntPtr hook, int code, IntPtr message, IntPtr state);
        [DllImport("kernel32.dll")]
        public static extern IntPtr GetModuleHandle(string name);

        //[DllImport("GlobalHookAssist.dll")]
        //internal extern static IntPtr BeginHook(EHookType hookType, HookHandler hookDelegate);
        //[DllImport("GlobalHookAssist.dll")]
        //internal extern static bool EndHook(IntPtr hook);
    }

    [StructLayout(LayoutKind.Sequential)]
    public class KBDLLHOOKSTRUCT
    {
        public uint vkCode;
        public uint scanCode;
        public KBDLLHOOKSTRUCTFlags flags;
        public uint time;
        public UIntPtr dwExtraInfo;
    }

    [Flags]
    public enum KBDLLHOOKSTRUCTFlags : uint
    {
        LLKHF_EXTENDED = 0x01,
        LLKHF_INJECTED = 0x10,
        LLKHF_ALTDOWN  = 0x20,
        LLKHF_UP       = 0x80,
    }
}
