using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Data;
using Moral.Model;
using VolumeWatcher;
using VolumeWatcher.View;
using VolumeWatcher.Enumrate;

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

        private System.Timers.Timer StatusTimer = new System.Timers.Timer(50);

        public void SetBinding(WindowOption window, VolumeWatcherMain main)
        {
            // ピークメータ表示処理の一例
            StatusTimer.Elapsed += (o, el) => {
                var dispatcher = System.Windows.Application.Current.Dispatcher;

                dispatcher.BeginInvoke((Action)delegate ()
                {
                    var renderMeter = main.VolumeMonitor1.AudioDevice?.AudioMeterInformation;
                    var captureMeter = main.CaptureMonitor.AudioDevice?.AudioMeterInformation;

                    RenderPeakValue  = (int)Math.Round((renderMeter?.PeakValue??0) * 100);
                    CapturePeakValue = (int)Math.Round((captureMeter?.PeakValue??0) * 100);
                });
            };
        }

        public void Start()
        {
            StatusTimer.Start();
        }

        public void Stop()
        {
            StatusTimer.Stop();
            RenderPeakValue  = 0;
            CapturePeakValue = 0;
        }
    }
}
