using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
        private VolumeWatcherMain main = null;
        private VolumeWatcherModel model = null;
        private OptionWindowViewModel viewmodel = new OptionWindowViewModel();
        private MicPlayer micPlayer = new MicPlayer();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public WindowOption()
        {
            main = ((App)System.Windows.Application.Current).main;
            model = main.model;

            this.DataContext = model;
            InitializeComponent();

            viewmodel.SetBinding(this, main);
            RenderMeter.DataContext = viewmodel;
            CaptureMeter.DataContext = viewmodel;


            micPlayer.OnStateChanged += MicPlayter_StateChanged;

            // 高速化に寄与するかな
            this.Descendants().OfType<Freezable>().ToList().Where(e => e.CanFreeze).ToList().ForEach(e => e.Freeze());
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
            var list = this.tabControl.Items.OfType<TabItem>().ToList().Select(el => (FrameworkElement)el.Content).ToList();
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
            viewmodel.Stop();                     // Meterタイマーを停止しておく。
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
                viewmodel.Start();
            }
            else
            {
                viewmodel.Stop();
            }
        }

        private void ToggleButton_Click(object sender, RoutedEventArgs e)
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
                micPlayer.Stop();
            }
        }

        void MicPlayter_StateChanged(EMicState state)
        {
            var dispatcher = System.Windows.Application.Current.Dispatcher;

            dispatcher.BeginInvoke((Action)delegate ()
            {
                MicListenToggle.IsChecked = (state == EMicState.Start);
            });
        }

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

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

        private void DockPanel_MouseEnter(object sender, MouseEventArgs e)
        {
            var element = (Panel)sender;
            var brush   = new SolidColorBrush(Color.FromRgb(218, 241, 255)); //FFDAF1FF AliceBlue LightSkyBlue
            element.Background = brush;
        }

        private void DockPanel_MouseLeave(object sender, MouseEventArgs e)
        {
            var element = (Panel)sender;
            element.Background = Brushes.Transparent;
        }
    }
}
