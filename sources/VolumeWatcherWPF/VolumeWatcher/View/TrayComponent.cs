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
using VolumeWatcher.Component;


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

        //private bool isKeyHook = false;
        //internal KeyboardHook keyboardHook1;
        //private Dictionary<System.Windows.Input.Key, Action> KeyShortcuts;

        internal KeyboardHookComponent KeyboardHooker;

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
            //this.keyboardHook1 = new KeyboardHook();

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

            // Keyboardのフック処理
            KeyboardHooker = new KeyboardHookComponent();
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
        }

        /// <summary>
        /// 
        /// </summary>
        public new void Dispose()
        {
            base.Dispose();
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
    }
}
