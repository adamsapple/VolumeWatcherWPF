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
        VolumeWatcherMain      main      = null;
        VolumeWatcherModel     model     = null;
        TrayComponentViewModel viewmodel = new TrayComponentViewModel();

        private bool isKeyHook = false;
        private KeyboardHook keyboardHook1;
        private Dictionary<System.Windows.Input.Key, Action> KeyShortcuts;

        private ImageSource defaultIcon  = null;
        public ImageSource DefaultIcon {
            get
            {
                return defaultIcon;
            }
            set
            {
                defaultIcon = value;
                if(this.IconSource == null)
                {
                    this.IconSource = defaultIcon;
                }
            }
        }

        /// <summary>
        /// コンストラクタ。
        /// </summary>
        public TrayComponent() {
            // notifyIcon settings.
            this.ContextMenu   = new MainContextMenu();
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
            main  = ((App)System.Windows.Application.Current).main;
            model = main.model;
            this.ToolTip = model.StartupName;

            this.DataContext = model;

            const float add = 0.02f;
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
                        device.AudioEndpointVolume.MasterVolumeLevelScalar -= add;
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
                        device.AudioEndpointVolume.MasterVolumeLevelScalar += add;
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
                        device.AudioEndpointVolume.MasterVolumeLevelScalar -= add;
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
                        device.AudioEndpointVolume.MasterVolumeLevelScalar += add;
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
            this.ToolTipText = string.Format("{0}\n({1})", model.StartupName, devname);
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

                if(isKeyHook == value)
                {
                    return;
                }

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
            if (act == null)
            {
                return;
            }

            act.Invoke();
            if (main.optionWindow.IsActive|main.volumeWindow.IsActive)
            {
                // Window上でのキー操作でBeep音が鳴るのを防ぐ(コントロール・ウィンドウにキーイベントをバブリングしない)
                e.Cancel = true;
            }
        }
    }
}
