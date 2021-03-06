﻿using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using LinqToXaml;

using Moral.Util;
using Moral.Linq;

using VolumeWatcher.Audio;
using VolumeWatcher.Model;
using VolumeWatcher.ViewModel;


namespace VolumeWatcher.View
{
    /// <summary>
    /// オプション画面
    /// </summary>
    public partial class WindowOption : Window
    {
        private VolumeWatcherMain     main      = null;
        private VolumeWatcherModel    model     = null;
        private OptionWindowViewModel viewmodel = new OptionWindowViewModel();
        private MicPlayer micPlayer = new MicPlayer();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public WindowOption()
        {
            main  = ((App)System.Windows.Application.Current).main;
            model = main.model;

            InitializeComponent();

            viewmodel.SetBinding(this, main);

            RenderMeter.DataContext       = viewmodel;       // peakmeterはviewmodelを参照
            CaptureMeter.DataContext      = viewmodel;       // peakmeterはviewmodelを参照
            ScreenSaverToggle.DataContext = viewmodel;
            chkIsStartUp.DataContext      = viewmodel;
            chkIsKeyHook.DataContext      = viewmodel;

            //{ 
            //    chkIsKeyHook.CommandParameter = chkIsKeyHook;
            //    chkIsKeyHook.Command          = viewmodel.KeyboardHookCommand;
            //}


            this.DataContext              = model;           // それ以外はmodelを参照

            micPlayer.OnStateChanged += MicPlayter_StateChanged;

            // 高速化に寄与するかな
            //this.Descendants().OfType<Freezable>().ToList().Where(e => e.CanFreeze).ToList().ForEach(e => e.Freeze());
            this.Descendants().OfType<Freezable>().Where(e => e.CanFreeze).ForEach(e => e.Freeze());
        }

        /// <summary>
        /// Load時。初回表示前に1回だけ呼ばれる。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //
            // ウィンドウ位置が保存されてない場合は初期位置(中央)に
            //
            if (this.Left < 0 && this.Top < 0)
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }

            //
            // Tabの高さを自動計算(最大値)で統一する処理
            //
            { 
                var list = this.tabControl.Items.OfType<TabItem>().Select(el => el.Content as FrameworkElement);

                //var list2 = this.tabControl.Items.OfType<TabItem>().ForEach(el =>
                //{
                //    //el.IsSelected = true;
                //    var content = el.Content as FrameworkElement;
                //    if (!content.IsMeasureValid)
                //    {
                //        content.Measure(new Size(int.MaxValue, int.MaxValue));
                //    }
                //    Debug.WriteLine($"item.actualheight={content.ActualHeight},  content={content.DesiredSize.Height}");
                //});
                var ratio = 1.0;
                var maxHeight = list.Max( el => {
                    //el.ActualHeight;
                    ((TabItem)el.Parent).IsSelected = true;
                    el.Measure(new Size(int.MaxValue, int.MaxValue));
                    //el.Measure(el.RenderSize);
                    if (el.ActualHeight != 0)
                    {
                        ratio = el.ActualHeight / el.DesiredSize.Height;
                        Debug.WriteLine($"ratio={ratio}");
                    }

                    Debug.WriteLine($"item actualheight={el.ActualHeight}, DesiredSize={el.DesiredSize.Height}, RenderSize={el.RenderSize.Height} ratedDesired={el.DesiredSize.Height * ratio}");

                    var source = PresentationSource.FromVisual(el);
                    Debug.WriteLine($"M11={source?.CompositionTarget?.TransformToDevice.M11}, M22={source?.CompositionTarget?.TransformToDevice.M22}");

                    return el.DesiredSize.Height * ratio;
                });
                Debug.WriteLine($"maxHeight={maxHeight}");
                list.Where(el => maxHeight > el.ActualHeight).ForEach(el => el.Height = maxHeight);

                // 初期表示タブを選択
                ((TabItem)tabControl.Items[0]).IsSelected = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Hide();
            viewmodel.StopPeakMeter();            // Meterタイマーを停止しておく。
            e.Cancel = true;
            //switch (e.CloseReason)
            //{
            //    case CloseReason.UserClosing:   // ユーザーインターフェイスによる
            //        e.Cancel = true;            // クローズイベントをキャンセル
            //        break;
            //}
        }

        /// <summary>
        /// キー入力を処理したことにしてBeep音が鳴ることを防いでいる。(e.Handled = true)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
        }

        public new void Show()
        {
            base.Show();
            tabControl_SelectionChanged(tabControl, null);      // タイマー再開の必要があれば行う。
        }

        /// <summary>
        /// Form内コントールの状態を更新
        /// </summary>
        public void updateControl()
        {
            // checkBoxIsStartupの状態を更新
            chkIsStartUp.IsChecked = WindowsUtil.ExistsStartUp_CurrentUserRun(model.StartupName);
        }

        /// <summary>
        /// ev:Formアクティブ時(表示時、フォーカス時)
        /// チェックボックス状態の更新.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Activated(object sender, EventArgs e)
        {
            // チェックボックス状態の更新
            updateControl();
        }

        /// <summary>
        /// keyhookのチェックON
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkIsKeyHook_Changed(object sender, RoutedEventArgs e)
        {
            var value = (sender as CheckBox)?.IsChecked ?? false;
            //main.trayComponent.EnableKeyHook = (bool)(sender as CheckBox).IsChecked;
            viewmodel.KeyboardHookCommand.Execute(value);
        }

        /// <summary>
        /// 選択Tabが変更になった際のイベント。
        /// 「状態」では、ピークメータ更新用タイマーを動作させ、それ以外の時は止める。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var ti = (sender as TabControl).SelectedItem as TabItem;

            if (ti == tiStatus)
            {
                viewmodel.StartPeakMeter();
            }
            else
            {
                viewmodel.StopPeakMeter();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListenToDevice_Click(object sender, RoutedEventArgs e)
        {
            var toggle = (ToggleButton)sender;
            var isChecked = (bool)toggle.IsChecked;

            if (isChecked)
            {
                // 再生
                micPlayer.Start();
            }
            else
            {
                // 停止
                Debug.WriteLine("stop start.");

                Task.Run(() =>
                {
                    var forceMute = !model.IsRecMute;
                    if (forceMute)
                    {
                        model.IsRecMute = !model.IsRecMute;
                        Thread.Sleep(150);  // 転送データ再生終了ち(超ザツ)
                    }
                    micPlayer.Stop();
                    if (forceMute)
                    {
                        model.IsRecMute = !model.IsRecMute;
                    }
                    Debug.WriteLine("stop finished.");
                });
            }
        }

        /// <summary>
        /// マイク音再生プレイヤーのイベント
        /// </summary>
        /// <param name="state"></param>
        void MicPlayter_StateChanged(EMicState state)
        {
            var dispatcher = System.Windows.Application.Current.Dispatcher;

            dispatcher.BeginInvoke((Action)delegate ()
            {
                MicListenToggle.IsChecked = (state == EMicState.Start);
            });
        }

        /// <summary>
        /// ハイパーリンククリック時のイベント
        /// ブラウザで所定URL(e.Uri.AbsoluteUri)に遷移
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        /// <summary>
        /// 特定のパネルでマウスホイールを操作した場合は、対象のVolumeを上下させる
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DockPanel_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            float add = 0.01f * (e.Delta > 0 ? 1 : -1);
            var monitor = ((sender == VolPanel)?main.VolumeMonitor1 : main.CaptureMonitor);
            var volume = monitor.AudioDevice?.AudioEndpointVolume;
            if (volume == null)
            {
                return;
            }
            volume.MasterVolumeLevelScalar += add;
        }


#if DEBUG
        //
        //
        //
        //
        // 以下、テストコード
        //
        //
        //
        //


        public void setKeyState(string state)
        {
            txtKeyCode.Text = state;
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MoniterPower.PowerOff();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            MoniterPower.ExecScreenSaver();
        }
#endif
    }
}
