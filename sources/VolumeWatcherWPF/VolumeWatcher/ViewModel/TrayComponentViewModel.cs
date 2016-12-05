using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

using Audio.CoreAudio;
using VolumeWatcher.View;

namespace VolumeWatcher.ViewModel
{
    public class TrayComponentViewModel
    {
        private static readonly DependencyProperty RenderDeviceProperty =
           DependencyProperty.Register("RenderDevice", typeof(MMDevice), typeof(TrayComponent),
                new FrameworkPropertyMetadata(null, new PropertyChangedCallback((sender, e) => {
                    var self   = (TrayComponent)sender;
                    var device = (MMDevice)e.NewValue;

                    var dispatcher = System.Windows.Application.Current.Dispatcher;
                    dispatcher.BeginInvoke((Action)delegate ()
                    {
                        if (device != null)
                        {
                            self.SetDeviceIcon(device.IconPath);
                            self.UpdateTrayText(device.DeviceFriendlyName);
                        }
                        else
                        {
                            self.IconSource = self.DefaultIcon;
                            self.UpdateTrayText("デバイス未接続");
                        }
                    });

                })));

        public void SetBinding(FrameworkElement view)
        {
            // binding設定:RenderDevice
            view.SetBinding(RenderDeviceProperty, new Binding("RenderDevice") { Mode = BindingMode.OneWay });
        }
    }
}
