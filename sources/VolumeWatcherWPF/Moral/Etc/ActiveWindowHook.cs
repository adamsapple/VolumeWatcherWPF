using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using Moral.Etc.Hook;

namespace Moral.Etc
{
    
    public delegate void ActiveWindowChangedEventHandler(object sender, ActiveWindowChangedEventArgs e);

    public class ActiveWindowChangedEventArgs
    {

        public CWPSTRUCT cwp;

        internal ActiveWindowChangedEventArgs(HookedEventArgs e)
        {
            cwp = (CWPSTRUCT)Marshal.PtrToStructure(e.lParam, typeof(CWPSTRUCT));
        }
    }

    ///<summary>キーボードの状態を表す。</summary>
    public struct CWPSTRUCT
    {
        public IntPtr wParam;
        public IntPtr lParam;
        public uint message;
        public IntPtr hwnd;

        public bool IsActiveEvent => (message == 0x0006 && ((int)wParam & 0xFFFF)!=0);

    }

    // public static readonly uint WM_ACTIVATE    = 0x0006;


    public class ActiveWindowHook : GlobalHook
    {
        protected override EHookType HookType => EHookType.WH_CALLWNDPROCRET;
        private event ActiveWindowChangedEventHandler callbacks;
        public new event ActiveWindowChangedEventHandler Callbacks
        {
            add
            {
                callbacks += value;
                if (callbacks.GetInvocationList().Length == 1)
                {
                    base.Callbacks += OnKeyHook;
                }
            }
            remove
            {
                callbacks -= value;
                if (callbacks.GetInvocationList().Length == 0)
                {
                    base.Callbacks -= OnKeyHook;
                }
            }
        }

        void OnKeyHook(object sender, HookedEventArgs e)
        {
            var ee = new ActiveWindowChangedEventArgs(e);

            // Hookしたイベントが不要なら終了
            if (!ee.cwp.IsActiveEvent) return;

            this.callbacks?.Invoke(sender, ee);
        }
    }
   
}
