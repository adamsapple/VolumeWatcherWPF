using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using LinqToXaml;

using Audio.CoreAudio;
using Moral;
using Moral.Util;
using VolumeWatcher.Model;
using VolumeWatcher.ViewModel;

namespace VolumeWatcher.View
{
    /// <summary>
    /// オプション画面
    /// </summary>
    public partial class WindowOption : Window
    {
        private VolumeWatcherMain main = null;
        private VolumeWatcherModel model = null;

        private System.Timers.Timer StatusTimer = new System.Timers.Timer(100);
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public WindowOption()
        {
            main = ((App)System.Windows.Application.Current).main;
            model = main.model;

            this.DataContext = model;
            InitializeComponent();
            
            // 高速化に寄与するかな
            this.Descendants().OfType<Freezable>().ToList().Where(e => e.CanFreeze).ToList().ForEach(e => e.Freeze());


            // ピークメータ表示処理の一例
            {
                StatusTimer.Elapsed += (o, el) => {
                    //Console.WriteLine("peak={0:p1}", peak);
                    Dispatcher.BeginInvoke((Action)delegate () {
                        UpdateMeter(RenderMeter, main.VolumeMonitor1.AudioDevice?.AudioMeterInformation);
                        UpdateMeter(CaptureMeter, main.CaptureMonitor.AudioDevice?.AudioMeterInformation);
                    });
                };
            }
        }

        /// <summary>
        /// Load時。初回表示前に1回だけ呼ばれる。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // ウィンドウ位置が保存されてない場合は初期位置(中央)に
            if (this.Left < 0 && this.Top < 0)
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }

            // 各Tabの大きさを一番大きいものにそろえる
            var list = this.tabControl.Items.OfType<TabItem>().ToList().Select(el=> (FrameworkElement)el.Content).ToList();
            var maxHeight = list.Max(el => el.ActualHeight);
            list.Where(el => (maxHeight > el.ActualHeight)).ToList()
                .ForEach(el => el.Height = maxHeight);
            //Dispatcher.BeginInvoke((Action)delegate () {
            //});
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Hide();
            e.Cancel = true;
            StatusTimer.Stop();                 // Meterタイマーを停止しておく。
            //switch (e.CloseReason)
            //{
            //    case CloseReason.UserClosing:   // ユーザーインターフェイスによる
            //        e.Cancel = true;            // クローズイベントをキャンセル
            //        break;
            //}
        }

        public new void Show()
        {
            base.Show();
            tabControl_SelectionChanged(tabControl, null);      // タイマー再開の必要があれば行う。
        }

        /// <summary>
        /// ピークメーターの表示更新
        /// </summary>
        /// <param name="bar"></param>
        /// <param name="meter"></param>
        private void UpdateMeter(Moral.UI.LevelBar bar, AudioMeterInformation meter)
        {
            if (meter != null)
            {
                bar.Value = (int)(meter.PeakValue * 100);
            }
            else
            {
                bar.Value = 0;
            }
        }

        /// <summary>
        /// Form内コントールの状態を更新
        /// </summary>
        public void updateControl()
        {
            // checkBoxIsStartupの状態を更新
            if (WindowsUtil.ExistsStartUp_CurrentUserRun(model.StartupName))
            {
                chkIsStartUp.IsChecked = true;
            }
            else
            {
                chkIsStartUp.IsChecked = false;
            }
        }
        
        /// <summary>
        /// ev:chkIsStartUp チェックON
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkIsStartUp_Checked(object sender, RoutedEventArgs e)
        {
            WindowsUtil.RegiserStartUp_CurrentUserRun(model.StartupName);
        }

        /// <summary>
        /// ev:chkIsStartUp チェックOFF
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkIsStartUp_Unchecked(object sender, RoutedEventArgs e)
        {
            WindowsUtil.UnregiserStartUp_CurrentUserRun(model.StartupName);
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
            main.trayComponent.EnableKeyHook = (bool)((CheckBox)sender).IsChecked;
        }

        private void tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var ti = ((TabControl)sender).SelectedItem as TabItem;

            if (ti == tiStatus)
            {
                StatusTimer.Start();
                //Console.WriteLine("timer開始");
            }
            else
            {
                StatusTimer.Stop();
                UpdateMeter(RenderMeter, null);
                UpdateMeter(CaptureMeter, null);
            }

        }
    }
}
