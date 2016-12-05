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

        private Stopwatch stopwatch       = new Stopwatch();
        private readonly long OPACITY_ANIMATION_RENDER_INTERVAL = 33 * 10000;
        private DispatcherTimer OpacityAnimationTimer = new DispatcherTimer(DispatcherPriority.Normal);

        public float MaxOpacity { get; set; }     = 0f;
        private bool isBindInitialized = false;
        /// <summary>
        /// Bindが完了しているかどうか.
        /// 初期同期処理の最中は、
        /// ・表示しない
        /// ・ViewModeがころころ変わってしまうのを抑制。
        /// </summary>
        public bool IsBindInitialized {
            get {
                return isBindInitialized;
            }
            set {
                if(isBindInitialized == false && value == true)
                {
                    ViewMode = EVolumeViewMode.Render;
                }
                isBindInitialized = true;
            }
        }

        internal bool IsSizeCalculated { get; private set; } = false;

        private EWindowPosition _WindowPosition = EWindowPosition.UNKNOWN;
        /// <summary>表示位置のプロパティ</summary>
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

        private EVolumeViewMode viewMode = EVolumeViewMode.Render;
        /// <summary>Render/CaptureどちらのVolumeを表示するかのプロパティ</summary>
        public EVolumeViewMode ViewMode
        {
            get
            {
                return viewMode;
            }
            set
            {
                if (value == EVolumeViewMode.NotChange)
                {
                    return;
                }
                viewMode = value;
                if (viewMode == EVolumeViewMode.Render)
                {
                    CapturePanel.Visibility = Visibility.Collapsed;
                    RenderPanel.Visibility  = Visibility.Visible;
                }
                else
                {
                    RenderPanel.Visibility  = Visibility.Collapsed;
                    CapturePanel.Visibility = Visibility.Visible;
                }
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
            OpacityAnimationTimer.Interval= new TimeSpan( OPACITY_ANIMATION_RENDER_INTERVAL );
            OpacityAnimationTimer.Tick += OnOpacityAnimation;

            // Stopwatch
            stopwatch.Reset();

            // ease.
            ease.addFrame(showDuration);
            ease.addFrame(visibleDuration);
            ease.addFrame(hideDuration);

            this.NoActiveWindow();
        }

        /// <summary>
        /// Closeイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            OpacityAnimationTimer.Stop(); // タイマーイベント発行を停止
            stopwatch.Stop();             // 時間計測処理を停止
        }

        /// <summary>
        /// Showのオーバーライド
        /// </summary>
        public new void Show()
        {
            base.Show();
            CheckWindowPosition();
        }

        /// <summary>
        /// 
        /// </summary>
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
        public void ShowVolume(EVolumeViewMode viewmode = EVolumeViewMode.NotChange)
        {
            // バインディングが未完了の場合は表示しない
            if (!IsBindInitialized) return;
            // ウィンドウのActual～の計算が未完了の場合は表示しない
            //if (!IsSizeCalculated && !CheckWindowPosition()) return;

            this.ViewMode = viewmode;

            //if (timer.Enabled)
            if(OpacityAnimationTimer.IsEnabled)
            {
                // 既に実行中
                var ms      = stopwatch.ElapsedMilliseconds;
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
                easeoffs     = 0;
                this.Opacity = 0;
                Show();
                OpacityAnimationTimer.Start();
                stopwatch.Reset();
                stopwatch.Start();
            }
        }

        /// <summary>
        /// Timerイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnOpacityAnimation(object sender, EventArgs e)
        {
            var ms = stopwatch.ElapsedMilliseconds + easeoffs;
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
                OpacityAnimationTimer.Stop();           // タイマーイベント発行を停止
                stopwatch.Stop();       // 時間計測処理を停止
            }
        }
    }
}
