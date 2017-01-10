using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Moral.Etc.Hook
{
    public delegate void KeyboardHookedEventHandler(object sender, KeyboardHookedEventArgs e);

    public class KeyboardHookedEventArgs
    {

        public KBDLLHOOKSTRUCT kb;

        internal KeyboardHookedEventArgs(HookedEventArgs e)
        {
            var kb = (KBDLLHOOKSTRUCT)Marshal.PtrToStructure(e.lParam, typeof(KBDLLHOOKSTRUCT));
        }
    }

    public class GlobalKeyHook : GlobalHook
    {
        protected override EHookType HookType => EHookType.WH_KEYBOARD_LL;
        private event KeyboardHookedEventHandler callbacks;
        public new event KeyboardHookedEventHandler Callbacks
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
            var ee = new KeyboardHookedEventArgs(e);
            var keycode = KeyInterop.KeyFromVirtualKey((int)ee.kb.vkCode);

            this.callbacks?.Invoke(sender, ee);
        }
    }
}
