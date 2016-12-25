using System;
using System.Diagnostics;
using System.Windows.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

using Moral.Util;

namespace VolumeWatcher.Command
{
    class KeepScreenSaverCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;
        private DispatcherTimer timer = new DispatcherTimer(DispatcherPriority.Normal);

        private readonly long TIMER_INTERVAL = (5 * 1000) * 10000;

        public bool IsRunning { get; private set; } = false;


        public KeepScreenSaverCommand()
        {
            // タイマーの生成
            timer.Interval = new TimeSpan(TIMER_INTERVAL);
            timer.Tick    += OnTimer;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            if (!IsRunning)
            {
                timer.Start();
            }
            else
            {
                timer.Stop();
            }
            
            IsRunning = !IsRunning;
        }

        void OnTimer(object sender, EventArgs e)
        {
            Debug.WriteLine($"IsExecScreenSaver => {MoniterPower.IsExecScreenSaver()}");
            if (!MoniterPower.IsExecScreenSaver())
            {
                MoniterPower.ExecScreenSaver();
            }
        }
    }
}
