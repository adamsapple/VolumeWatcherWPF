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
                                       var self  = (WindowVolume)sender;
                                       var value = (EWindowPosition)e.NewValue;
                                       self.WindowPosition = value;
                                       self.ShowVolume();
                                   })));

        private static readonly DependencyProperty VolumeProperty =
           DependencyProperty.Register("Volume", typeof(int), typeof(WindowVolume),
                                   new FrameworkPropertyMetadata(0, new PropertyChangedCallback((sender, e) =>
                                   {
                                       var self = (WindowVolume)sender;
                                       self.ViewMode = EVolumeViewMode.Render;
                                       self.ShowVolume();
                                   })));

        private static readonly DependencyProperty IsMuteProperty =
           DependencyProperty.Register("IsMute", typeof(bool), typeof(WindowVolume),
                                   new FrameworkPropertyMetadata(false, new PropertyChangedCallback((sender, e) => {
                                       var self = (WindowVolume)sender;
                                       //var value = (bool)e.NewValue;
                                       //var old   = (bool)e.OldValue;
                                       //self.IsMute = value;
                                       self.ViewMode = EVolumeViewMode.Render;
                                       self.ShowVolume();
                                   })));

        private static readonly DependencyProperty RenderDeviceProperty =
           DependencyProperty.Register("RenderDevice", typeof(MMDevice), typeof(WindowVolume),
                                   new FrameworkPropertyMetadata(null, new PropertyChangedCallback((sender, e) => {
                                       var self = (WindowVolume)sender;
                                       //var value = (MMDevice)e.NewValue;
                                       self.ViewMode = EVolumeViewMode.Render;
                                       self.ShowVolume();
                                   })));


        
        private static readonly DependencyProperty RecVolumeProperty =
            DependencyProperty.Register("RecVolume", typeof(int), typeof(WindowVolume),
                                    new FrameworkPropertyMetadata(0, new PropertyChangedCallback((sender, e) =>
                                    {
                                        var self = (WindowVolume)sender;
                                        self.ViewMode = EVolumeViewMode.Capture;
                                        self.ShowVolume();
                                    })));
        
        private static readonly DependencyProperty IsRecMuteProperty =
           DependencyProperty.Register("IsRecMute", typeof(bool), typeof(WindowVolume),
                                   new FrameworkPropertyMetadata(false, new PropertyChangedCallback((sender, e) => {
                                       var self  = (WindowVolume)sender;
                                       //var value = (bool)e.NewValue;
                                       //var old   = (bool)e.OldValue;
                                       //self.IsRecMute = value;
                                       self.ViewMode = EVolumeViewMode.Capture;
                                       self.ShowVolume();
                                   })));

        private static readonly DependencyProperty CaptureDeviceProperty =
           DependencyProperty.Register("CaptureDevice", typeof(MMDevice), typeof(WindowVolume),
                                   new FrameworkPropertyMetadata(null, new PropertyChangedCallback((sender, e) => {
                                       var self = (WindowVolume)sender;
                                       //var value = (MMDevice)e.NewValue;
                                       self.ViewMode = EVolumeViewMode.Capture;
                                       self.ShowVolume();
                                       //self.IsBindInitialized = true;           // 最後のプロパティで行う
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
            // binding設定:RenderDeviceProperty
            view.SetBinding(RenderDeviceProperty, new Binding("RenderDevice") { Mode = BindingMode.OneWay });
            
            // binding設定:Volume
            view.SetBinding(RecVolumeProperty, new Binding("RecVolume") { Mode = BindingMode.OneWay });
            
            // binding設定:IsMute
            view.SetBinding(IsRecMuteProperty, new Binding("IsRecMute") { Mode = BindingMode.OneWay });
            // binding設定:RenderDeviceProperty
            view.SetBinding(CaptureDeviceProperty, new Binding("CaptureDevice") { Mode = BindingMode.OneWay });

            ((WindowVolume)view).IsBindInitialized = true;           // 
        }
    }
}
