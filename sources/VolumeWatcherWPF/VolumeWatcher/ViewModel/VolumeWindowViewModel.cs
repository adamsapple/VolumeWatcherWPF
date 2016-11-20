using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows;
using System.Windows.Data;

using Audio.CoreAudio;
using VolumeWatcher.View;
using VolumeWatcher.Enumrate;

namespace VolumeWatcher.ViewModel
{
    class VolumeWindowViewModel:DependencyObject
    {
        private static readonly DependencyProperty MaxOpacityProperty =
           DependencyProperty.Register("MaxOpacity", typeof(float), typeof(WindowVolume),
                                   new FrameworkPropertyMetadata(0.95f, new PropertyChangedCallback((sender, e) => {
                                       var self = (WindowVolume)sender;
                                       var value = (float)e.NewValue;
                                       self.MaxOpacity = value;
                                       self.ShowVolume();
                                   })));

        private static readonly DependencyProperty WindowPositionProperty =
           DependencyProperty.Register("WindowPosition", typeof(EWindowPosition), typeof(WindowVolume),
                                   new FrameworkPropertyMetadata(EWindowPosition.UNKNOWN, new PropertyChangedCallback((sender, e) => {
                                       var self = (WindowVolume)sender;
                                       var value = (EWindowPosition)e.NewValue;
                                       self.WindowPosition = value;
                                       self.ShowVolume();
                                       //var dispatcher = Application.Current.Dispatcher;
                                       //dispatcher.BeginInvoke((Action)delegate ()
                                       //{
                                       //  
                                       //});

                                       //var dispatcher = Application.Current.Dispatcher;
                                       //dispatcher.BeginInvoke((Action)delegate ()
                                       //{

                                       //});
                                   })));

        private static readonly DependencyProperty VolumeProperty =
           DependencyProperty.Register("Volume", typeof(int), typeof(WindowVolume),
                                   new FrameworkPropertyMetadata(0, new PropertyChangedCallback((sender, e) =>
                                   {
                                       var self = (WindowVolume)sender;
                                       self.ShowVolume();
                                   })));

        private static readonly DependencyProperty IsMuteProperty =
           DependencyProperty.Register("IsMute", typeof(bool), typeof(WindowVolume),
                                   new FrameworkPropertyMetadata(false, new PropertyChangedCallback((sender, e) => {
                                       var self  = (WindowVolume)sender;
                                       var value = (bool)e.NewValue;
                                       var old   = (bool)e.OldValue;
                                        self.IsMute = value;
                                        self.UpdateControls();
                                        self.ShowVolume();
                                   })));
        /*
        private static readonly DependencyProperty IconPathProperty =
            DependencyProperty.Register("IconPath", typeof(string), typeof(WindowVolume),
                                    new FrameworkPropertyMetadata(string.Empty, new PropertyChangedCallback((sender, e) => {
                                        var self = (WindowVolume)sender;
                                        var value = (string)e.NewValue;
                                        self.SetDeviceIcon(value);
                                        self.ShowVolume();
                                        self.IsBindInitialized = true;
                                    })));
        */
        private static readonly DependencyProperty RenderDeviceProperty =
           DependencyProperty.Register("RenderDevice", typeof(MMDevice), typeof(WindowVolume),
                                   new FrameworkPropertyMetadata(null, new PropertyChangedCallback((sender, e) => {
                                       var self  = (WindowVolume)sender;
                                       //var value = (MMDevice)e.NewValue;
                                       self.ShowVolume();
                                       self.IsBindInitialized = true;
                                   })));
        /// <summary>
        /// 
        /// </summary>
        /// <param name="view"></param>
        public void SetBinding(FrameworkElement view)
        {
            // binding設定:MaxOpacity
            view.SetBinding(MaxOpacityProperty, new Binding("Opacity") { Mode = BindingMode.OneWay });
            // binding設定:WindowPosition
            view.SetBinding(WindowPositionProperty, new Binding("WindowPosition") { Mode = BindingMode.OneWay });
            // binding設定:Volume
            view.SetBinding(VolumeProperty, new Binding("Volume") { Mode = BindingMode.OneWay });
            // binding設定:IsMute
            view.SetBinding(IsMuteProperty, new Binding("IsMute") { Mode = BindingMode.OneWay });
            // binding設定:IconPathProperty
            //view.SetBinding(IconPathProperty, new Binding("IconPath") { Mode = BindingMode.OneWay });
            // binding設定:RenderDeviceProperty
            view.SetBinding(RenderDeviceProperty, new Binding("RenderDevice") { Mode = BindingMode.OneWay });
        }
    }
}
