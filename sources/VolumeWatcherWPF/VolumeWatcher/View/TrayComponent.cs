using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Input;
using Hardcodet.Wpf.TaskbarNotification;
using HongliangSoft.Utilities.Gui;
using Audio.CoreAudio;
using Moral.Util;
using VolumeWatcher.Audio;
using VolumeWatcher.UI;
using VolumeWatcher.Model;
using VolumeWatcher.ViewModel;

namespace VolumeWatcher.View
{
    /// <summary>
    /// 
    /// </summary>
    public class TrayComponent: TaskbarIcon
    {
        VolumeWatcherMain main = null;
        VolumeWatcherModel model = null;
        TrayComponentViewModel viewmodel = new TrayComponentViewModel();

        private bool isKeyHook = false;
        private KeyboardHook keyboardHook1;
        private Dictionary<System.Windows.Input.Key, Action> KeyShortcuts;

        /// <summary>
        /// コンストラクタ。
        /// </summary>
        public TrayComponent() {
            // notifyIcon settings.
            this.ContextMenu = new MainContextMenu();
            this.keyboardHook1 = new KeyboardHook();
            this.TrayMouseDoubleClick += (_o, _e) => {
                var main = ((App)System.Windows.Application.Current).main;

                // オプション画面の表示/非表示を変更
                if (main.optionWindow.IsVisible)
                {
                    main.optionWindow.Hide();
                }
                else
                {
                    main.optionWindow.Show();
                    main.optionWindow.Activate();   // ダブルクリックするとTrayにFocusを持ってかれるので、窓にFocusを再度与える(強制)
                }
            };
            viewmodel.SetBinding(this);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Initialize()
        {
            main = ((App)System.Windows.Application.Current).main;
            model = main.model;
            this.ToolTip = model.StartupName;

            this.DataContext = model;
            

            const float add = 0.02f;
            var dict = new Dictionary<System.Windows.Input.Key, Action>()
            {
                {
                    Key.OemComma, () => {
                        // 音量-
                        main.VolumeMonitor1.AudioDevice.AudioEndpointVolume.MasterVolumeLevelScalar -= add;
                    }
                },
                {
                    Key.OemPeriod, () => {
                        // 音量+
                        main.VolumeMonitor1.AudioDevice.AudioEndpointVolume.MasterVolumeLevelScalar += add;
                    }
                },
                {
                    Key.M, () => {
                        // Mute
                        main.VolumeMonitor1.AudioDevice.AudioEndpointVolume.Mute = !main.VolumeMonitor1.AudioDevice.AudioEndpointVolume.Mute;
                    }
                }
            };
            KeyShortcuts = dict;
        }

        /// <summary>
        /// 
        /// </summary>
        public new void Dispose()
        {
            base.Dispose();
            keyboardHook1.Dispose(true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="iconpath"></param>
        public void SetDeviceIcon(string iconpath)
        {
            System.Drawing.Icon ico = WindowsUtil.GetIconFromEXEDLL2(iconpath, false);
            this.Icon = ico;
        }

        /// <summary>
        /// 通知のTooltipを更新
        /// </summary>
        public void UpdateTrayText(string devname)
        {
            var dispatcher = System.Windows.Application.Current.Dispatcher;

            if (!dispatcher.CheckAccess())
            {
                dispatcher.Invoke((Action)delegate ()
                {
                    this.ToolTipText = model.StartupName + "\n(" + devname + ")";
                });
            }
            else
            {
                this.ToolTipText = model.StartupName + "\n(" + devname + ")";
            }
        }

        /// <summary>
        /// キーフックの活性化
        /// </summary>
        public bool EnableKeyHook
        {
            get
            {
                return isKeyHook;
            }
            set
            {

                if (value)
                {
                    keyboardHook1.KeyboardHooked += keyboardHook1_KeyboardHooked;
                }
                else
                {
                    keyboardHook1.KeyboardHooked -= keyboardHook1_KeyboardHooked;
                }
                isKeyHook = value;
            }
        }

        /// <summary>
        /// キーフック時に通知されるイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void keyboardHook1_KeyboardHooked(object sender, KeyboardHookedEventArgs e)
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

            var act = KeyShortcuts.GetValueOrDefault(e.KeyCode, null);
            if (act != null)
            {
                act.Invoke();
                if (main.optionWindow.IsActive)
                {
                    // Window上でのキー操作でBeep音が鳴るのを防ぐ(コントロール・ウィンドウにキーイベントをバブリングしない)
                    e.Cancel = true;
                }
            }
        }
    }
}
