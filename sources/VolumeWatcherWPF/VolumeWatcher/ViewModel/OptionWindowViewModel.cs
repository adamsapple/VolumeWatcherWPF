using System;
using System.ComponentModel;
using System.Windows.Threading;

using Moral.Model;

using VolumeWatcher.View;
using VolumeWatcher.Model;
using VolumeWatcher.Command;


namespace VolumeWatcher.ViewModel
{
    class OptionWindowViewModel : ModelBase
    {
        private int _RenderPeakValue = 0;
        public int RenderPeakValue
        {
            get { return _RenderPeakValue; }
            private set { SetProperty(ref _RenderPeakValue, value); }
        }
        private int _CapturePeakValue = 0;
        public int CapturePeakValue
        {
            get { return _CapturePeakValue; }
            private set { SetProperty(ref _CapturePeakValue, value); }
        }

        private bool _IsKeyHook;
        public bool IsKeyHook
        {
            get { return model.IsKeyHook; }
            set
            {
                model.IsKeyHook = value;
                SetProperty(ref _IsKeyHook, value);
                KeyboardHookCommand.Execute(value);
            }
        }

        public VolumeWatcherModel model;

        public KeepScreenSaverCommand KeepScreenSaverCommand { get; private set; } = new KeepScreenSaverCommand();
        public RegisterStartupCommand RegisterStartupCommand { get; private set; } = new RegisterStartupCommand();
        public KeyboardHookCommand KeyboardHookCommand       { get; private set; }



        /// <summary>
        /// メータ更新間隔(単位:100ナノ秒)。= 1/10000ミリ秒。
        /// 
        /// </summary>
        private readonly long PEAKMETER_RENDER_INTERVAL = 50 * 10000;

        private DispatcherTimer StatusTimer = new DispatcherTimer(DispatcherPriority.Normal);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="window"></param>
        /// <param name="main"></param>
        public void SetBinding(WindowOption window, VolumeWatcherMain main)
        {
            // 参照モデルの設定
            model = main.model;
            // Command初期化
            KeyboardHookCommand = new KeyboardHookCommand(main);
            RegisterStartupCommand.StartupName = model.StartupName;

            // Binding初期化
            IsKeyHook = model.IsKeyHook;

            
            // ピークメータ表示用タイマ
            StatusTimer.Interval = new TimeSpan(PEAKMETER_RENDER_INTERVAL);
            StatusTimer.Tick    += (o, el) => {
                var renderMeter  = main.VolumeMonitor1.AudioDevice?.AudioMeterInformation;
                var captureMeter = main.CaptureMonitor.AudioDevice?.AudioMeterInformation;
                if (StatusTimer.IsEnabled)
                {
                    RenderPeakValue  = (int)Math.Round((renderMeter?.PeakValue  ?? 0) * 100);
                    CapturePeakValue = (int)Math.Round((captureMeter?.PeakValue ?? 0) * 100);
                }
            };
        }

        public void StartPeakMeter()
        {
            StatusTimer.Start();
        }

        public void StopPeakMeter()
        {
            StatusTimer.Stop();
            RenderPeakValue  = 0;
            CapturePeakValue = 0;
        }
    }
}
