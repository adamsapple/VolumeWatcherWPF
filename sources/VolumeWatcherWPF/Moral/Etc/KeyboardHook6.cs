using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Input;

// http://hongliang.seesaa.net/article/7539988.html
//
// hook4�Fhook�C�x���g���P���o�^����ĂȂ��ꍇ�́AUnhook���ăV�X�e���p�t�H�[�}���X���҂��悤�ɕύX
// hook5�F���z�L�[(System.Windows.Forms.Keys)����WPF�L�[(System.Windows.Input.Key)�ɕύX
// hook6�F����
//
namespace HongliangSoft.Utilities.Gui {
    
    ///<summary>�L�[�{�[�h�����삳�ꂽ�Ƃ��Ɏ��s����郁�\�b�h��\���C�x���g�n���h���B</summary>
    public delegate void KeyboardHookedEventHandler(object sender, KeyboardHookedEventArgs e);

    ///<summary>KeyboardHooked�C�x���g�̃f�[�^��񋟂���B</summary>
    public class KeyboardHookedEventArgs : CancelEventArgs {

        ///<summary>�V�����C���X�^���X���쐬����B</summary>
        internal KeyboardHookedEventArgs(KeyboardMessage message, ref KeyboardState state) {
            this.message = message;
            this.state = state;
        }
        private KeyboardMessage message;
        private KeyboardState state;
        ///<summary>�L�[�{�[�h�������ꂽ�������ꂽ����\���l���擾����B</summary>
        public KeyboardUpDown UpDown {
            get {
                return (message == KeyboardMessage.KeyDown || message == KeyboardMessage.SysKeyDown) ?
                    KeyboardUpDown.Down : KeyboardUpDown.Up;
            }
        }
        ///<summary>���삳�ꂽ�L�[�̉��z�L�[�R�[�h��\���l���擾����B</summary>
        public Key KeyCode {get {return state.KeyCode;}}
        ///<summary>���삳�ꂽ�L�[�̃X�L�����R�[�h��\���l���擾����B</summary>
        public int ScanCode {get {return state.ScanCode;}}
        ///<summary>���삳�ꂽ�L�[���e���L�[�Ȃǂ̊g���L�[���ǂ�����\���l���擾����B</summary>
        public bool IsExtendedKey {get {return state.Flags.IsExtended;}}
        ///<summary>ALT�L�[��������Ă��邩�ǂ�����\���l���擾����B</summary>
        public bool AltDown {get {return state.Flags.AltDown;}}
    }

    ///<summary>�L�[�{�[�h��������Ă��邩������Ă��邩��\���B</summary>
    public enum KeyboardUpDown {
        ///<summary>�L�[�͉�����Ă���B</summary>
        Down,
        ///<summary>�L�[�͕�����Ă���B</summary>
        Up,
    }

    ///<summary>���b�Z�[�W�R�[�h��\���B</summary>
    internal enum KeyboardMessage {
        ///<summary>�L�[�������ꂽ�B</summary>
        KeyDown    = 0x100,
        ///<summary>�L�[�������ꂽ�B</summary>
        KeyUp      = 0x101,
        ///<summary>�V�X�e���L�[�������ꂽ�B</summary>
        SysKeyDown = 0x104,
        ///<summary>�V�X�e���L�[�������ꂽ�B</summary>
        SysKeyUp   = 0x105,
    }

    ///<summary>�L�[�{�[�h�̏�Ԃ�\���B</summary>
    internal struct KeyboardState {
        ///<summary>���z�L�[�R�[�h�B</summary>
        public Key KeyCode;
        ///<summary>�X�L�����R�[�h�B</summary>
        public int ScanCode;
        ///<summary>�e�����t���O�B</summary>
        public KeyboardStateFlag Flags;
        ///<summary>���̃��b�Z�[�W������ꂽ�Ƃ��̎��ԁB</summary>
        public int Time;
        ///<summary>���b�Z�[�W�Ɋ֘A�Â���ꂽ�g�����B</summary>
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

    ///<summary>�L�[�{�[�h�̏�Ԃ�⑫����B</summary>
    internal struct KeyboardStateFlag {
        private KBDLLHOOKSTRUCTFlags flag;
       
        ///<summary>�L�[���e���L�[��̃L�[�̂悤�Ȋg���L�[���ǂ�����\���B</summary>
        public bool IsExtended => flag.HasFlag(KBDLLHOOKSTRUCTFlags.LLKHF_EXTENDED);

        ///<summary>�C�x���g���C���W�F�N�g���ꂽ���ǂ�����\���B</summary>
        public bool IsInjected => flag.HasFlag(KBDLLHOOKSTRUCTFlags.LLKHF_INJECTED);
        
        ///<summary>ALT�L�[��������Ă��邩�ǂ�����\���B</summary>
        public bool AltDown => flag.HasFlag(KBDLLHOOKSTRUCTFlags.LLKHF_ALTDOWN);
        
        ///<summary>�L�[�������ꂽ�ǂ�����\���B</summary>
        public bool IsUp => flag.HasFlag(KBDLLHOOKSTRUCTFlags.LLKHF_UP);
    }
    
    ///<summary>�L�[�{�[�h�̑�����t�b�N���A�C�ӂ̃��\�b�h��}������B</summary>
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
        ///<summary>�L�[�{�[�h�����삳�ꂽ�Ƃ��ɔ�������B</summary>
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
        ///KeyboardHooked�C�x���g�𔭐�������B
        ///</summary>
        ///<param name="e">�C�x���g�̃f�[�^�B</param>
        protected virtual void OnKeyboardHooked(KeyboardHookedEventArgs e) {
            var handler = base.Events[EventKeyboardHooked] as KeyboardHookedEventHandler;
            handler?.Invoke(this, e);
        }

        ///<summary>
        ///�V�����C���X�^���X���쐬����B
        ///</summary>
        public KeyboardHook() {
            if (Environment.OSVersion.Platform != PlatformID.Win32NT)
                throw new PlatformNotSupportedException("Windows 98/Me�ł̓T�|�[�g����Ă��܂���B");

            this.callback     = new KeyboardHookDelegate(CallNextHook);
            this.hookDelegate = GCHandle.Alloc(callback);
            this.module       = Marshal.GetHINSTANCE(System.Reflection.Assembly.GetExecutingAssembly().GetModules()[0]);
            //this.module = Marshal.GetHINSTANCE(typeof(KeyboardHook).Assembly.GetModules()[0]);
        }

        ///<summary>
        ///�L�[�{�[�h�����삳�ꂽ�Ƃ��Ɏ��s����f���Q�[�g���w�肵�ăC���X�^���X���쐬����B
        ///</summary>
        ///<param name="handler">�L�[�{�[�h�����삳�ꂽ�Ƃ��Ɏ��s���郁�\�b�h��\���C�x���g�n���h���B</param>
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
        ///�g�p����Ă���A���}�l�[�W���\�[�X��������A�I�v�V�����Ń}�l�[�W���\�[�X���������B
        ///</summary>
        ///<param name="disposing">�}�l�[�W���\�[�X���������ꍇ��true�B</param>
        public new void Dispose(bool disposing) {
            if (this.hookDelegate.IsAllocated) {
                UnHook();
                this.hookDelegate.Free();
            }
            base.Dispose(disposing);
        }
    }
}
