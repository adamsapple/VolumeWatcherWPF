using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Input;

// http://hongliang.seesaa.net/article/7539988.html
//
// hook4：hookイベントが１つも登録されてない場合は、Unhookしてシステムパフォーマンスを稼ぐように変更
// hook5：仮想キー(System.Windows.Forms.Keys)からWPFキー(System.Windows.Input.Key)に変更
// hook6：調整
//
namespace HongliangSoft.Utilities.Gui {
    
    ///<summary>キーボードが操作されたときに実行されるメソッドを表すイベントハンドラ。</summary>
    public delegate void KeyboardHookedEventHandler(object sender, KeyboardHookedEventArgs e);

    ///<summary>KeyboardHookedイベントのデータを提供する。</summary>
    public class KeyboardHookedEventArgs : CancelEventArgs {

        ///<summary>新しいインスタンスを作成する。</summary>
        internal KeyboardHookedEventArgs(KeyboardMessage message, ref KeyboardState state) {
            this.message = message;
            this.state = state;
        }
        private KeyboardMessage message;
        private KeyboardState state;
        ///<summary>キーボードが押されたか放されたかを表す値を取得する。</summary>
        public KeyboardUpDown UpDown {
            get {
                return (message == KeyboardMessage.KeyDown || message == KeyboardMessage.SysKeyDown) ?
                    KeyboardUpDown.Down : KeyboardUpDown.Up;
            }
        }
        ///<summary>操作されたキーの仮想キーコードを表す値を取得する。</summary>
        public Key KeyCode {get {return state.KeyCode;}}
        ///<summary>操作されたキーのスキャンコードを表す値を取得する。</summary>
        public int ScanCode {get {return state.ScanCode;}}
        ///<summary>操作されたキーがテンキーなどの拡張キーかどうかを表す値を取得する。</summary>
        public bool IsExtendedKey {get {return state.Flags.IsExtended;}}
        ///<summary>ALTキーが押されているかどうかを表す値を取得する。</summary>
        public bool AltDown {get {return state.Flags.AltDown;}}
    }

    ///<summary>キーボードが押されているか放されているかを表す。</summary>
    public enum KeyboardUpDown {
        ///<summary>キーは押されている。</summary>
        Down,
        ///<summary>キーは放されている。</summary>
        Up,
    }

    ///<summary>メッセージコードを表す。</summary>
    internal enum KeyboardMessage {
        ///<summary>キーが押された。</summary>
        KeyDown    = 0x100,
        ///<summary>キーが放された。</summary>
        KeyUp      = 0x101,
        ///<summary>システムキーが押された。</summary>
        SysKeyDown = 0x104,
        ///<summary>システムキーが放された。</summary>
        SysKeyUp   = 0x105,
    }

    ///<summary>キーボードの状態を表す。</summary>
    internal struct KeyboardState {
        ///<summary>仮想キーコード。</summary>
        public Key KeyCode;
        ///<summary>スキャンコード。</summary>
        public int ScanCode;
        ///<summary>各種特殊フラグ。</summary>
        public KeyboardStateFlag Flags;
        ///<summary>このメッセージが送られたときの時間。</summary>
        public int Time;
        ///<summary>メッセージに関連づけられた拡張情報。</summary>
        public IntPtr ExtraInfo;
    }

    [Flags]
    internal enum KBDLLHOOKSTRUCTFlags : int
    {
        LLKHF_EXTENDED = 0x01,
        LLKHF_INJECTED = 0x10,
        LLKHF_ALTDOWN  = 0x20,
        LLKHF_UP       = 0x80,
    }

    ///<summary>キーボードの状態を補足する。</summary>
    internal struct KeyboardStateFlag {
        private KBDLLHOOKSTRUCTFlags flag;
       
        ///<summary>キーがテンキー上のキーのような拡張キーかどうかを表す。</summary>
        public bool IsExtended => flag.HasFlag(KBDLLHOOKSTRUCTFlags.LLKHF_EXTENDED);

        ///<summary>イベントがインジェクトされたかどうかを表す。</summary>
        public bool IsInjected => flag.HasFlag(KBDLLHOOKSTRUCTFlags.LLKHF_INJECTED);
        
        ///<summary>ALTキーが押されているかどうかを表す。</summary>
        public bool AltDown => flag.HasFlag(KBDLLHOOKSTRUCTFlags.LLKHF_ALTDOWN);
        
        ///<summary>キーが放されたどうかを表す。</summary>
        public bool IsUp => flag.HasFlag(KBDLLHOOKSTRUCTFlags.LLKHF_UP);
    }
    
    ///<summary>キーボードの操作をフックし、任意のメソッドを挿入する。</summary>
    [DefaultEvent("KeyboardHooked")]
    public class KeyboardHook : Component {
        [DllImport("user32.dll", SetLastError=true)]
        private static extern IntPtr SetWindowsHookEx(int hookType, KeyboardHookDelegate hookDelegate, IntPtr hInstance, uint threadId);
        [DllImport("user32.dll", SetLastError=true)]
        private static extern int CallNextHookEx(IntPtr hook, int code, KeyboardMessage message, ref KeyboardState state);
        [DllImport("user32.dll", SetLastError=true)]
        private static extern bool UnhookWindowsHookEx(IntPtr hook);

        private delegate int KeyboardHookDelegate(int code, KeyboardMessage message, ref KeyboardState state);
        private const int KeyboardHookType = 13;            // KEYBOARD_LL = 13;
        private GCHandle hookDelegate;
        private IntPtr hook;
        private IntPtr module;
        private KeyboardHookDelegate callback;
        private int nNumCallbacks = 0;

        private static readonly object EventKeyboardHooked = new object();
        ///<summary>キーボードが操作されたときに発生する。</summary>
        public event KeyboardHookedEventHandler KeyboardHooked {
            add
            {
                if (nNumCallbacks++ == 0)
                {
                    Hook();
                }
                
                base.Events.AddHandler(EventKeyboardHooked, value);
            }
            remove
            {
                base.Events.RemoveHandler(EventKeyboardHooked, value);

                if (--nNumCallbacks == 0)
                {
                    UnHook();
                }
            }
        }

        ///<summary>
        ///KeyboardHookedイベントを発生させる。
        ///</summary>
        ///<param name="e">イベントのデータ。</param>
        protected virtual void OnKeyboardHooked(KeyboardHookedEventArgs e) {
            var handler = base.Events[EventKeyboardHooked] as KeyboardHookedEventHandler;
            handler?.Invoke(this, e);
        }

        ///<summary>
        ///新しいインスタンスを作成する。
        ///</summary>
        public KeyboardHook() {
            if (Environment.OSVersion.Platform != PlatformID.Win32NT)
                throw new PlatformNotSupportedException("Windows 98/Meではサポートされていません。");

            this.callback     = new KeyboardHookDelegate(CallNextHook);
            this.hookDelegate = GCHandle.Alloc(callback);
            this.module       = Marshal.GetHINSTANCE(System.Reflection.Assembly.GetExecutingAssembly().GetModules()[0]);
            //this.module = Marshal.GetHINSTANCE(typeof(KeyboardHook).Assembly.GetModules()[0]);
        }

        ///<summary>
        ///キーボードが操作されたときに実行するデリゲートを指定してインスタンスを作成する。
        ///</summary>
        ///<param name="handler">キーボードが操作されたときに実行するメソッドを表すイベントハンドラ。</param>
        public KeyboardHook(KeyboardHookedEventHandler handler) : this() {
            this.KeyboardHooked += handler;
        }

        private void Hook()
        {
            if (hook != IntPtr.Zero)
            {
                return;
            }

            this.hook = SetWindowsHookEx(KeyboardHookType, callback, module, 0);

            if (hook == IntPtr.Zero)
            {
                int errorCode = Marshal.GetLastWin32Error();
                throw new Win32Exception(errorCode);
            }
        }

        private void UnHook()
        {
            if (hook == IntPtr.Zero)
            {
                return;
            }
            UnhookWindowsHookEx(hook);
            this.hook = IntPtr.Zero;
        }

        private int CallNextHook(int code, KeyboardMessage message, ref KeyboardState state) {
            if (code >= 0) {
                state.KeyCode = KeyInterop.KeyFromVirtualKey((int)state.KeyCode);
                KeyboardHookedEventArgs e = new KeyboardHookedEventArgs(message, ref state);
                OnKeyboardHooked(e);
                if (e.Cancel) {
                    return -1;
                }
            }
            return CallNextHookEx(IntPtr.Zero, code, message, ref state);
        }

        ///<summary>
        ///使用されているアンマネージリソースを解放し、オプションでマネージリソースも解放する。
        ///</summary>
        ///<param name="disposing">マネージリソースも解放する場合はtrue。</param>
        public new void Dispose(bool disposing) {
            if (this.hookDelegate.IsAllocated) {
                UnHook();
                this.hookDelegate.Free();
            }
            base.Dispose(disposing);
        }
    }
}
