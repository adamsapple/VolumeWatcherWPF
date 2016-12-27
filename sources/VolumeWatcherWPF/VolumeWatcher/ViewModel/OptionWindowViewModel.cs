using System;
using System.Windows.Input;
using System.Windows.Threading;
using Moral.Model;
using VolumeWatcher.View;
using VolumeWatcher.Command;


namespace VolumeWatcher.ViewModel
{
    class OptionWindowViewModel : ModelBase
    {
        private int _RenderPeakValue = 0;
        public int RenderPeakValue
        {
            get { return _RenderPeakValue; }
            private set { _RenderPeakValue = value; SetProperty(ref _RenderPeakValue, value); }
        }
        private int _CapturePeakValue = 0;
        public int CapturePeakValue
        {
            get { return _CapturePeakValue; }
            private set { _CapturePeakValue = value; SetProperty(ref _CapturePeakValue, value); }
        }

        public KeepScreenSaverCommand KeepScreenSaverCommand { get; private set; } = new KeepScreenSaverCommand();
        public RegisterStartupCommand RegisterStartupCommand { get; private set; } = new RegisterStartupCommand();



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
            // ピークメータ表示用タイマ
            StatusTimer.Interval = new TimeSpan(PEAKMETER_RENDER_INTERVAL);
            StatusTimer.Tick += (o, el) => {
                var renderMeter  = main.VolumeMonitor1.AudioDevice?.AudioMeterInformation;
                var captureMeter = main.CaptureMonitor.AudioDevice?.AudioMeterInformation;
                if (StatusTimer.IsEnabled)
                {
                    RenderPeakValue  = (int)Math.Round((renderMeter?.PeakValue  ?? 0) * 100);
                    CapturePeakValue = (int)Math.Round((captureMeter?.PeakValue ?? 0) * 100);
                }
            };

            RegisterStartupCommand.StartupName = main.model.StartupName;
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
