using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.ComponentModel;

// http://hp.vector.co.jp/authors/VA016117/hook.html
// http://mrxray.on.coocan.jp/Delphi/Others/A_KindOfHook.htm
// http://qiita.com/rbtnn/items/74d0b1438776cca1c1ea
namespace Moral.Etc
{
    public enum EHookType : int
    {
        WH_JOURNALRECORD   = 0,
        WH_JOURNALPLAYBACK = 1,
        WH_KEYBOARD        = 2,
        WH_GETMESSAGE      = 3,
        WH_CALLWNDPROC     = 4,
        WH_CBT             = 5,
        WH_SYSMSGFILTER    = 6,
        WH_MOUSE           = 7,
        WH_HARDWARE        = 8,
        WH_DEBUG           = 9,
        WH_SHELL           = 10,
        WH_FOREGROUNDIDLE  = 11,
        WH_CALLWNDPROCRET  = 12,
        WH_KEYBOARD_LL     = 13,
        WH_MOUSE_LL        = 14
    }

    public class NativeMethods
    {
        public delegate int HookHandler(int code, IntPtr message, IntPtr state);
        public delegate void CallBackHandler(object sender, HookedEventArgs e);
        
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr SetWindowsHookEx(EHookType hookType, HookHandler hookDelegate, IntPtr hInstance, uint threadId);
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool UnhookWindowsHookEx(IntPtr hook);
        [DllImport("user32.dll", SetLastError = true)]
        public static extern int CallNextHookEx(IntPtr hook, int code, IntPtr message, IntPtr state);
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

    public class HookedEventArgs : CancelEventArgs
    {

        private IntPtr wParam;
        private IntPtr lParam;

        
        internal HookedEventArgs(IntPtr wParam, IntPtr lParam)
        {
            this.wParam = wParam;
            this.lParam = lParam;
        }
    }

    public class KH : IDisposable
    {
        IntPtr hook   = IntPtr.Zero;
        IntPtr module = IntPtr.Zero;
        //event NativeMethods.HookHandler hookDelegate = delegate { return 0; };
        event NativeMethods.HookHandler     hookCallback;
        event NativeMethods.CallBackHandler callbacks;
        GCHandle hookHandle;

        public event NativeMethods.CallBackHandler Callbacks
        {
            add
            {
                callbacks += value;
                if(callbacks.GetInvocationList().Length == 1)
                {
                    Hook();
                }
            }
            remove
            {
                callbacks -= value;
                if (callbacks.GetInvocationList().Length == 0)
                {
                    UnHook();
                }
            }
        }

        public virtual EHookType EHookType => EHookType.WH_KEYBOARD_LL;

        public KH()
        {
            if (Environment.OSVersion.Platform != PlatformID.Win32NT)
                throw new PlatformNotSupportedException("Windows 98/Meではサポートされていません。");

            this.module           = Marshal.GetHINSTANCE(System.Reflection.Assembly.GetExecutingAssembly().GetModules()[0]);
            this.hookCallback     = new NativeMethods.HookHandler(OnHook);
            this.hookHandle       = GCHandle.Alloc(this.hookCallback);
        }

        int OnHook(int code, IntPtr wParam, IntPtr lParam)
        {
            if (code >= 0)
            {
                HookedEventArgs e = new HookedEventArgs(wParam, lParam);
                callbacks?.Invoke(this, e);
                if (e.Cancel)
                {
                    return -1;
                }
            }
            return NativeMethods.CallNextHookEx(hook, code, wParam, lParam);
        }

        /*
        int OnHook(int code, IntPtr wParam, IntPtr lParam)
        {
            var kb = (KBDLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(KBDLLHOOKSTRUCT));
            var keycode = KeyInterop.KeyFromVirtualKey((int)kb.vkCode);
            Console.WriteLine($"keycode={keycode}");
            return NativeMethods.CallNextHookEx(hook, code, wParam, lParam);
        }
        */

        private void Hook()
        {
            if (this.hook != IntPtr.Zero) return;

            //
            // Hook.
            //
            this.hook = NativeMethods.SetWindowsHookEx(this.EHookType, this.hookCallback, this.module, 0);
            if (this.hook == IntPtr.Zero)
            {
                var error = Marshal.GetLastWin32Error();
                throw new Win32Exception(error);
            }
        }

        private void UnHook()
        {
            if (this.hook == IntPtr.Zero) return;

            //
            // UnHook.
            //
            NativeMethods.UnhookWindowsHookEx(this.hook);
            this.hook     = IntPtr.Zero;
        }

        public void Dispose()
        {
            if (this.hookHandle.IsAllocated)
            {
                UnHook();
                this.hookHandle.Free();
            }
        }

        ~KH()
        {
            Dispose();
        }
    }
}
