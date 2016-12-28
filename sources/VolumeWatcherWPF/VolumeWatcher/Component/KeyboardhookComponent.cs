using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HongliangSoft.Utilities.Gui;
using Moral.Util;


namespace VolumeWatcher.Component
{
    public class KeyboardHookComponent
    {
        private KeyboardHook keyboardHook;
        private Dictionary<int, Action> KeyShortcutDictionary;

        private bool _Enable = false;
        public bool Enable
        {
            get
            {
                return _Enable;
            }
            set
            {
                if (_Enable == value)
                {
                    return;
                }

                if (value)
                {
                    keyboardHook.KeyboardHooked += OnKeyboardHooked;
                }
                else
                {
                    keyboardHook.KeyboardHooked -= OnKeyboardHooked;
                }

                _Enable = value;
            }
        }

        public bool IsShiftDown { get; private set; } = false;
        public bool IsCtrlDown  { get; private set; } = false;
        public bool IsAltDown   { get; private set; } = false;

        /// <summary>
        /// 
        /// </summary>
        public KeyboardHookComponent()
        {
            keyboardHook          = new KeyboardHook();
            KeyShortcutDictionary = new Dictionary<int, Action>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mixedKey"></param>
        /// <param name="act"></param>
        public void AddShortcut(int mixedKey, Action act)
        {
            KeyShortcutDictionary.Add(mixedKey, act);
        }

        /// <summary>
        /// Keyと装飾キーを混ぜた値を作成して返す.
        /// </summary>
        /// <param name="keyCode"></param>
        /// <param name="isShift"></param>
        /// <param name="isCtrl"></param>
        /// <param name="isAlt"></param>
        /// <returns>Keyと装飾キーを混ぜた値</returns>
        public static int GetMixedKeyFromKey(Key keyCode, bool isShift, bool isCtrl, bool isAlt)
        {
            int result = (int)keyCode;
            result |= Convert.ToInt32(isShift) << 8;
            result |= Convert.ToInt32(isCtrl)  << 9;
            result |= Convert.ToInt32(isAlt)   << 10;
            return result;
        }

        /// <summary>
        /// 装飾キーの押下状態を更新
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private bool CheckEffectiveKeys(KeyboardHookedEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Key.LeftShift:
                case Key.RightShift:
                    IsShiftDown = e.UpDown.HasFlag(KeyboardUpDown.Down);
                    return true;
                case Key.LeftCtrl:
                case Key.RightCtrl:
                    IsCtrlDown  = e.UpDown.HasFlag(KeyboardUpDown.Down);
                    return true;
                case Key.LeftAlt:
                case Key.RightAlt:
                    IsAltDown   = e.UpDown.HasFlag(KeyboardUpDown.Down);
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// キーフック時に通知されるイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnKeyboardHooked(object sender, KeyboardHookedEventArgs e)
        {
            Debug.WriteLine($"[hooking key code]:{e.ScanCode}({e.KeyCode})");

            // Shift,Ctrl,Altならフラグ処理して終了
            if (CheckEffectiveKeys(e))
            {
                return;
            }
                
            // キー押下時以外は処理しないで終了
            if (e.UpDown != KeyboardUpDown.Down)
            {
                return;
            }

            // 対象のショートカットが見つかれば実行する
            KeyShortcutDictionary.GetValueOrDefault(GetMixedKeyFromKey(e.KeyCode, IsShiftDown, IsCtrlDown, IsAltDown), null)?.Invoke();

            // Window上でのキー操作でBeep音が鳴るのを防ぐ(コントロール・ウィンドウにキーイベントをバブリングしない)
            if (Application.Current.Windows.Cast<Window>().Where(win=>win.IsActive).FirstOrDefault() != null)
            {
                e.Cancel = true;
            }
        }

        ~KeyboardHookComponent()
        {
            Enable = false;
            keyboardHook.Dispose(true);
        }
    }
}
