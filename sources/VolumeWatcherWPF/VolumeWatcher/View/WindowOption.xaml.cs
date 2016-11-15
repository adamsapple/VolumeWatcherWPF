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

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public WindowOption()
        {
            main = ((App)System.Windows.Application.Current).main;
            model = main.model;

            this.DataContext = model;
            InitializeComponent();
            
            // About表示の準備
            var asminfo = new AssemblyInfo(Assembly.GetEntryAssembly());
            this.Resources["AsmInfo_ProductName"] = asminfo.Product;
            this.Resources["AsmInfo_Version"] = asminfo.Version;
            this.Resources["AsmInfo_Date"] = asminfo.Copyright;

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
            //switch (e.CloseReason)
            //{
            //    case CloseReason.UserClosing:   // ユーザーインターフェイスによる
            //        e.Cancel = true;            // クローズイベントをキャンセル
            //        break;
            //}
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
    }
}
