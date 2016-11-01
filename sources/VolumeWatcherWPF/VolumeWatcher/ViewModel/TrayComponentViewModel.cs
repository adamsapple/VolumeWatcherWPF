using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

using VolumeWatcher.View;

namespace VolumeWatcher.ViewModel
{
    public class TrayComponentViewModel
    {
        private static readonly DependencyProperty IconPathProperty =
            DependencyProperty.Register("IconPath", typeof(string), typeof(TrayComponent),
                                    new FrameworkPropertyMetadata(string.Empty, new PropertyChangedCallback((sender, e) => {
                                        var self = (TrayComponent)sender;
                                        var value = (string)e.NewValue;
                                        self.SetDeviceIcon(value);
                                    })));

        private static readonly DependencyProperty DeviceNameProperty =
           DependencyProperty.Register("DeviceName", typeof(string), typeof(TrayComponent),
                                   new FrameworkPropertyMetadata(string.Empty, new PropertyChangedCallback((sender, e) => {
                                       var self = (TrayComponent)sender;
                                       var value = (string)e.NewValue;
                                       self.UpdateTrayText(value);
                                   })));



        public void SetBinding(FrameworkElement view)
        {
            // binding設定:IconPathProperty
            view.SetBinding(IconPathProperty, new Binding("IconPath") { Mode = BindingMode.OneWay });
            // binding設定:DeviceName
            view.SetBinding(DeviceNameProperty, new Binding("DeviceName") { Mode = BindingMode.OneWay });
        }
    }
}
