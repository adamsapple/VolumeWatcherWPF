using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
//using System.Windows.Forms;
using Moral.Util;

namespace Moral.UI
{
    public partial class NotifyIconWPF : Component
    {
        private Window win;
        private ImageSource icon;

        public NotifyIconWPF()
        {
            InitializeComponent();

            // イベントハンドラの設定
            //toolStripMenuItemShow.Click += toolStripMenuItemShow_Click;
            //toolStripMenuItemExit.Click += toolStripMenuItemExit_Click;



            // タスクトレイ用のアイコンを設定
            //System.IO.Stream iconStream = System.Windows.Application.GetResourceStream(new Uri("pack://application:,,,/NotifyIcon;component/Icons/Icon1.ico")).Stream;
            //this.notifyIcon.Icon = new System.Drawing.Icon(iconStream);

            //this.ShutdownMode = System.Windows.ShutdownMode.OnExplicitShutdown;
        }

        public ImageSource Icon
        {
            get
            {
                if(icon == null)
                {
                    icon = this.notifyIcon.Icon.ToImageSource();
                }

                return icon;
            }
            set
            {

            }
        }

        public NotifyIconWPF(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }

        void toolStripMenuItemShow_Click(object sender, EventArgs e)
        {
            ShowWindow();
        }

        void toolStripMenuItemExit_Click(object sender, EventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ShowWindow();
        }

        private void ShowWindow()
        {
            if(win == null)
            {
                return;
            }

            // ウィンドウ表示&最前面に持ってくる
            if (win.WindowState == System.Windows.WindowState.Minimized)
                win.WindowState = System.Windows.WindowState.Normal;

            win.Show();
            win.Activate();
            // タスクバーでの表示をする
            win.ShowInTaskbar = true;
        }
    }
}
