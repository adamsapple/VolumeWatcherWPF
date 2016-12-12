using System.Windows;

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
            //
            //二重起動をチェックする
            // ※プロセス名から判断するので、同名別アプリケーションがあった場合は意図しない動きになるかも
            
            //このアプリケーションのプロセス名を取得
            string processName = System.Diagnostics.Process.GetCurrentProcess().ProcessName; 
            if (System.Diagnostics.Process.GetProcessesByName(processName).Length > 1)
            {
                //すでに起動していると判断して終了
                this.Shutdown();        //Application.Current.Shutdown();
                return;
            }

            main = new VolumeWatcherMain();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            main?.Initialize();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            main?.OnExit(e);
        }
    }
}
