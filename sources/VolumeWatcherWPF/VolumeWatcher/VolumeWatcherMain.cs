using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using System.Reflection;

using VolumeWatcher.Audio;
using VolumeWatcher.UI;
using VolumeWatcher.Model;
using VolumeWatcher.View;
using VolumeWatcher.ViewModel;


namespace VolumeWatcher
{
    /// <summary>
    /// 
    /// </summary>
    class VolumeWatcherMain
    {
        internal TrayComponent trayComponent = null;
        internal WindowOption optionWindow   = null;
        internal WindowVolume volumeWindow   = null;
        internal VolumeWatcherModel model    = null;

        /// <summary> Volumeやデバイスを監視し、通知を行う </summary>
        public VolumeMonitor VolumeMonitor1;               // CoreAudio連携(デバイスの状態変更を監視し通知)

        public int State = 0;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public VolumeWatcherMain()
        {
            var app = System.Windows.Application.Current;
            // Windowをクローズしてもアプリケーションが終了しないように設定しておく。
            app.ShutdownMode = System.Windows.ShutdownMode.OnExplicitShutdown;  // Exitを明示的にCallすることでアプリケーションが終了するように設定

            // VolumeMonitor初期化
            VolumeMonitor1 = new VolumeMonitor();
            VolumeMonitor1.initDevice();
            
            // Model初期化
            model = new VolumeWatcherModel();
            model.LoadSettings();
            
            var VolumeMonitorViewModel1 = new VolumeMonitorViewModel(VolumeMonitor1, model);
        }

        /// <summary>
        /// 初期化。
        /// 描画コンポーネント初期化、オーディオデバイス監視処理の初期化。
        /// </summary>
        public void Initialize()
        {
            // Initialize UI.
            trayComponent= new TrayComponent();
            volumeWindow = new WindowVolume();
            optionWindow = new WindowOption();
            

            trayComponent.Initialize();
            trayComponent.IconSource = optionWindow.Icon;

            
            //optionWindow.Show();

            // データバインディングが終了するまで待つ
            // 待ち方がわからないので遅延処理する、という愚策
            //Task.Run(async () => {
            //    await Task.Delay(200);
            //    State = 1;
            //});
            State = 1;

            var win = new Window1();
            win.Show();
        }

        /// <summary>
        /// 終了処理。App.OnExitからCallされる。
        /// </summary>
        /// <param name="e"></param>
        public void OnExit(ExitEventArgs e)
        {
            State = 2;
            model.SaveSettings();
            optionWindow.Close();
            volumeWindow.Close();
            VolumeMonitor1.releaseDevice();
        }
    }
}
