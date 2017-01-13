using System;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Gl


// https://msdn.microsoft.com/en-us/library/ms644990%28VS.85%29.aspx?f=255&MSPPError=-2147217396

namespace Moral.Etc.Hook
{
    public delegate void CallBackHandler(object sender, HookedEventArgs e);

    public class HookedEventArgs : CancelEventArgs
    {
        public readonly IntPtr wParam;
        public readonly IntPtr lParam;

        internal HookedEventArgs(IntPtr wParam, IntPtr lParam) : base(false)
        {
            this.wParam = wParam;
            this.lParam = lParam;
        }
    }

    public class GlobalHook : IDisposable
    {
        IntPtr hook = IntPtr.Zero;
        IntPtr module = IntPtr.Zero;
        //event NativeMethods.HookHandler hookDelegate = delegate { return 0; };
        event HookHandler hookCallback;
        event CallBackHandler callbacks;
        GCHandle hookHandle;

        public event CallBackHandler Callbacks
        {
            add
            {
                callbacks += value;
                if (callbacks.GetInvocationList().Length == 1)
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

        protected virtual EHookType HookType => EHookType.WH_NOTHOOK;

        public GlobalHook()
        {
            if (Environment.OSVersion.Platform != PlatformID.Win32NT)
                throw new PlatformNotSupportedException("Windows 98/Meではサポートされていません。");

            this.module = Marshal.GetHINSTANCE(System.Reflection.Assembly.GetExecutingAssembly().GetModules()[0]);
            //this.module = NativeMethods.GetModuleHandle(Process.GetCurrentProcess().MainModule.ModuleName);
            this.hookCallback = new HookHandler(OnHook);
            this.hookHandle = GCHandle.Alloc(this.hookCallback);
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
            if (this.HookType == EHookType.WH_NOTHOOK) return;
            //
            // Hook.
            //
            //this.hook = NativeMethods.SetWindowsHookEx(this.HookType, this.hookCallback, this.module, 0);
      this.hook = GlobalHookAssistATL.BeginHook(this.HookType, this.hookCallback);
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
            //NativeMethods.UnhookWindowsHookEx(this.hook);
            NativeMethods.EndHook(this.hook);
            this.hook = IntPtr.Zero;
        }

        public void Dispose()
        {
            if (this.hookHandle.IsAllocated)
            {
                UnHook();
                this.hookHandle.Free();
            }
        }

        ~GlobalHook()
        {
            Dispose();
        }
    }
}
