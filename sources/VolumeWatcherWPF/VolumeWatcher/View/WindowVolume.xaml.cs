using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Diagnostics;
using System.Timers;
using System.Windows.Threading;

using Moral.Util;
using VolumeWatcher.Enumrate;
using VolumeWatcher.Model;
using VolumeWatcher.ViewModel;

namespace VolumeWatcher.View
{
    /// <summary>
    /// WindowVolume.xaml の相互作用ロジック
    /// </summary>
    public partial class WindowVolume : Window
    {
        private VolumeWatcherMain     main          = null;
        private VolumeWatcherModel    model         = null;
        private VolumeWindowViewModel viewmodel     = new VolumeWindowViewModel();

        private const int showDuration    = 150;       // 表示完了までの半透明の時間
        private const int hideDuration    = 700;       // 非表示完了までの半透明の時間
        private const int visibleDuration = 1800;      // 通常の表示時間

        private Ease ease = new Ease();
        private int easeoffs = 0;

        private Timer timer               = new Timer();
        private Stopwatch stopwatch       = new Stopwatch();

        private Brush VolumeColor = Brushes.DarkViolet;
        private Brush MuteColor   = Brushes.DarkGray;

        public float MaxOpacity { get; set; }     = 0f;
        public bool  IsMute { get; set; }         = false;
        public bool IsBindInitialized { get; set; } = false;
        public bool IsSizeCalculated { get; private set; } = false;
        private EWindowPosition _WindowPosition = EWindowPosition.UNKNOWN;
        public EWindowPosition WindowPosition
        {
            get
            {
                return _WindowPosition;
            }
            set
            {
                _WindowPosition = value;
                this.SetWindowPosition(WindowPosition);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public WindowVolume()
        {
            main = ((App)System.Windows.Application.Current).main;
            model = main.model;

            this.DataContext = model;

            InitializeComponent();
            viewmodel.SetBinding(this);

            

            // タイマーの生成
            timer.Interval = 33;
            timer.Elapsed += OnTimer;

            // Stopwatch
            stopwatch.Reset();

            // ease.
            ease.addFrame(showDuration);
            ease.addFrame(visibleDuration);
            ease.addFrame(hideDuration);

            this.NoActiveWindow();
        }

        /*
        /// <summary>
        /// 
        /// </summary>
        /// <param name="iconpath"></param>
        public void SetDeviceIcon(string iconpath)
        {
            ImageSource img = WindowsUtil.GetIconFromEXEDLL2(iconpath).ToImageSource();
            imgDevIcon.Source = img;
        }
        */
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            timer.Stop();           // タイマーイベント発行を停止
            stopwatch.Stop();       // 時間計測処理を停止
        }

        public void UpdateControls()
        {
            UpdateVolumeFont(IsMute);
        }

        public new void Show()
        {
            base.Show();
            CheckWindowPosition();
        }

        /// <summary>
        /// Mute状態に応じたStyleの変更
        /// </summary>
        /// <param name="mute">mute</param>
        void UpdateVolumeFont(bool mute)
        {
            labelVolume.Foreground = mute ? MuteColor : Brushes.White;  // 文字色変更
            barVolume.BarColor     = mute ? MuteColor : VolumeColor;    // Bar色変更
        }

        void CheckWindowPosition()
        {
            if (IsSizeCalculated) return;
            if (this.ActualWidth == 0) return;

            this.SetWindowPosition(WindowPosition);
            IsSizeCalculated = true;
            return;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vol"></param>
        public void ShowVolume()
        {
            // バインディングが未完了の場合は表示しない
            if (!IsBindInitialized) return;
            // ウィンドウのActual～の計算が未完了の場合は表示しない
            //if (!IsSizeCalculated && !CheckWindowPosition()) return;
            
            if (timer.Enabled)
            {
                // 既に実行中
                var ms = stopwatch.ElapsedMilliseconds;
                double rate = ease.getFrameRate(ms);
                if (rate >= 1.0)
                {
                    easeoffs = showDuration;
                    stopwatch.Restart();
                }
            }
            else
            {
                // 停止中(volumeウィンドウは消えているはず)
                easeoffs = 0;
                this.Opacity = 0;
                Show();
                timer.Start();
                stopwatch.Reset();
                stopwatch.Start();
            }
        }

        /// <summary>
        /// Timerイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnTimer(object sender, ElapsedEventArgs e)
        {
            var ms = stopwatch.ElapsedMilliseconds + easeoffs;
            //Console.WriteLine("OnTimer:" + ms + "ms");
            //this.Invoke((MethodInvoker)delegate ()
            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (Action)delegate () {
                //
                // stopwatchの経過時間ごとに処理を分岐
                //
                double rate = ease.getFrameRate(ms);
                if (rate <= 1.0)
                {
                    // showアニメーション中:不透過度を0⇒1.0へ
                    this.Opacity = rate * MaxOpacity;
                }
                else if (rate <= 2.0)
                {
                    // volume表示中:不透過度1.0
                    if (this.Opacity != MaxOpacity) this.Opacity = MaxOpacity;
                }
                else if (rate < 3.0)
                {
                    // hideアニメーション中：不透過度を1.0⇒0.0へ
                    rate -= 2.0;
                    this.Opacity = (1.0 - rate) * MaxOpacity;
                }
                else
                {
                    // hide完了
                    this.Opacity = 0.0;     // 不透過度0.0
                    Hide();                 // Formを非表示
                    timer.Stop();           // タイマーイベント発行を停止
                    stopwatch.Stop();       // 時間計測処理を停止
                }
            });
        }
    }
}
