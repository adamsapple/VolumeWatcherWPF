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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace VolumeWatcher.UI
{
    /// <summary>
    /// MainContextMenuUserControl1.xaml の相互作用ロジック
    /// </summary>
    public partial class MainContextMenu : ContextMenu
    {
        public MainContextMenu()
        {
            InitializeComponent();
        }

        private void MenuItem_Option_Click(object sender, RoutedEventArgs e)
        {
            var main = ((App)Application.Current).main;
            main.optionWindow.Show();
            if (!main.optionWindow.Focus())
            {
                main.optionWindow.Activate();
            }
        }

        private void MenuItem_Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
