using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Hardcodet.Wpf.TaskbarNotification;
using VolumeWatcher.UI;

namespace VolumeWatcher
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : Application
    {
        internal VolumeWatcherMain main;

        /// <summary>
        /// 
        /// </summary>
        public App()
        {
            //二重起動をチェックする
            //(プロセス名から判断するので、同名別アプリケーションがあった場合は意図しない動きになるかも)
            //if (System.Diagnostics.Process.GetProcessesByName(
            //      System.Diagnostics.Process.GetCurrentProcess().ProcessName).Length > 1)
            //{
                //すでに起動していると判断して終了
                //Application.Current.Shutdown();
            //    return;
            //}

            main = new VolumeWatcherMain();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            main.Initialize();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            main.OnExit(e);
        }
    }
}
