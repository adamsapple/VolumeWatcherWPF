using System;
using System.Collections.Generic;
using System.Windows.Input;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HongliangSoft.Utilities.Gui;
using Moral.Util;

namespace VolumeWatcher.Command
{
    /// <summary>
    /// キーボードのショートカット機能をCommandに逃がそうとして失敗した、作りかけ。
    /// ちゃんと動きません。
    /// </summary>
    class KeyboardHookCommand : SimpleCommandBase, IDisposable
    {
        public bool IsKeyHook { get; set; } = false;
        public float Step { get; set; } = 0.02f;
        private VolumeWatcherMain main;

        private KeyboardHook keyboardHook;
        private Dictionary<System.Windows.Input.Key, Action> KeyShortcutDictionary;

        public KeyboardHookCommand(VolumeWatcherMain main)
        {
            this.main = main;
            this.keyboardHook = main.trayComponent.keyboardHook1;

            var dict = new Dictionary<System.Windows.Input.Key, Action>()
            {
                {
                    Key.OemComma, () => {
                        // 音量-
                        var device = main.VolumeMonitor1.AudioDevice;
                        if(device == null || device.AudioEndpointVolume == null)
                        {
                            return;
                        }
                        device.AudioEndpointVolume.MasterVolumeLevelScalar -= Step;
                    }
                },
                {
                    Key.OemPeriod, () => {
                        // 音量+
                        var device = main.VolumeMonitor1.AudioDevice;
                        if(device == null || device.AudioEndpointVolume == null)
                        {
                            return;
                        }
                        device.AudioEndpointVolume.MasterVolumeLevelScalar += Step;
                    }
                },
                {
                    Key.M, () => {
                        // Mute
                        var device = main.VolumeMonitor1.AudioDevice;
                        if(device == null || device.AudioEndpointVolume == null)
                        {
                            return;
                        }
                        device.AudioEndpointVolume.Mute = !device.AudioEndpointVolume.Mute;
                    }
                },
                {
                    Key.K, () => {
                        // 音量-
                        var device = main.CaptureMonitor.AudioDevice;
                        if(device == null || device.AudioEndpointVolume == null)
                        {
                            return;
                        }
                        device.AudioEndpointVolume.MasterVolumeLevelScalar -= Step;
                    }
                },
                {
                    Key.L, () => {
                        // 音量+
                        var device = main.CaptureMonitor.AudioDevice;
                        if(device == null || device.AudioEndpointVolume == null)
                        {
                            return;
                        }
                        device.AudioEndpointVolume.MasterVolumeLevelScalar += Step;
                    }
                },
                {
                    Key.J, () => {
                        // Mute
                        var device = main.CaptureMonitor.AudioDevice;
                        if(device == null || device.AudioEndpointVolume == null)
                        {
                            return;
                        }
                        device.AudioEndpointVolume.Mute = !device.AudioEndpointVolume.Mute;
                    }
                }
            };
            KeyShortcutDictionary = dict;
            //foreach (var key in dict.Keys)
            //{
            //    KeyShortcutDictionary.Add(key, dict[key]);
            //}
        }

        public void AddShortcut( Key key, Action act)
        {
            KeyShortcutDictionary.Add(key, act);
        }

        public override void Execute(object parameter)
        {
            var IsExecute = parameter as bool? ?? false;

            // 既に希望の状態なら終了
            if(IsKeyHook == IsExecute)
            {
                return;
            }

            if (IsExecute)
            {
                keyboardHook.KeyboardHooked += OnKeyboardHooked;
            }
            else
            {
                keyboardHook.KeyboardHooked -= OnKeyboardHooked;
            }

            IsKeyHook = IsExecute;
        }

        /// <summary>
        /// キーフック時に通知されるイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnKeyboardHooked(object sender, KeyboardHookedEventArgs e)
        {
            if (e.UpDown != KeyboardUpDown.Down)
            {
                return;
            }

            //main.optionWindow.txtKeyCode.Text = e.ScanCode + ":" + e.KeyCode;

            if (!e.AltDown)
            {
                return;
            }

            var act = KeyShortcutDictionary.GetValueOrDefault(e.KeyCode, null);
            if (act == null)
            {
                return;
            }

            act.Invoke();
            if (main.optionWindow.IsActive | main.volumeWindow.IsActive)
            {
                // Window上でのキー操作でBeep音が鳴るのを防ぐ(コントロール・ウィンドウにキーイベントをバブリングしない)
                e.Cancel = true;
            }
        }
        #region IDisposable Support
        private bool disposedValue = false; // 重複する呼び出しを検出するには

        protected virtual void Dispose(bool disposing)
        {
            if (disposedValue) return;
            
            if (disposing)
            {
                // TODO: マネージ状態を破棄します (マネージ オブジェクト)。
                keyboardHook.KeyboardHooked -= OnKeyboardHooked;
                keyboardHook.Dispose();
            }

            // TODO: アンマネージ リソース (アンマネージ オブジェクト) を解放し、下のファイナライザーをオーバーライドします。
            // TODO: 大きなフィールドを null に設定します。

            disposedValue = true;
        }

        // TODO: 上の Dispose(bool disposing) にアンマネージ リソースを解放するコードが含まれる場合にのみ、ファイナライザーをオーバーライドします。
        // ~KeyboardHookCommand() {
        //   // このコードを変更しないでください。クリーンアップ コードを上の Dispose(bool disposing) に記述します。
        //   Dispose(false);
        // }

        // このコードは、破棄可能なパターンを正しく実装できるように追加されました。
        public void Dispose()
        {
            // このコードを変更しないでください。クリーンアップ コードを上の Dispose(bool disposing) に記述します。
            Dispose(true);
            // TODO: 上のファイナライザーがオーバーライドされる場合は、次の行のコメントを解除してください。
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
