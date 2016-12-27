using System;
using System.Diagnostics;
using System.Windows.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

using Moral.Util;

namespace VolumeWatcher.Command
{
    /// <summary>
    /// スクリーンセーバー起動状態を5s間隔でチェックし、起動状態を維持するCommand.
    /// </summary>
    class KeepScreenSaverCommand : SimpleCommandBase
    {
        private DispatcherTimer timer = new DispatcherTimer(DispatcherPriority.Normal);

        private readonly long TIMER_INTERVAL = (5 * 1000) * 10000;

        public bool IsRunning { get; private set; } = false;


        public KeepScreenSaverCommand()
        {
            // タイマーの生成
            timer.Interval = new TimeSpan(TIMER_INTERVAL);
            timer.Tick    += OnTimer;
        }

        override public void Execute(object parameter)
        {
            var IsExecute = parameter as bool? ?? false;

            if(IsExecute == IsRunning)
            {
                return;
            }

            if (IsExecute)
            {
                timer.Start();
            }
            else
            {
                timer.Stop();
            }
            
            IsRunning = IsExecute;
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
