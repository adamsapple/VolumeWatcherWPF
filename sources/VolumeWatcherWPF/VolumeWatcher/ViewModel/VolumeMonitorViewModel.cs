using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Audio.CoreAudio;
using VolumeWatcher.Audio;
using VolumeWatcher.Model;

namespace VolumeWatcher.ViewModel
{
    class VolumeMonitorViewModel
    {
        VolumeWatcherModel Model = null;

        public VolumeMonitorViewModel(VolumeMonitor renderMonitor, VolumeMonitor captureMonitor, VolumeWatcherModel model )
        {
            Model = model;

            renderMonitor.OnVolumeNotification += OnVolumeChanged;
            renderMonitor.OnDefaultDeviceChanged += OnDeviceChanged;

            captureMonitor.OnVolumeNotification += OnRecVolumeChanged;
            captureMonitor.OnDefaultDeviceChanged += OnRecDeviceChanged;

            Model.SetDeviceInfo(renderMonitor.AudioDevice);
            Model.SetRecDeviceInfo(captureMonitor.AudioDevice);
        }

        /// <summary>
        /// ev:MixerVolume変更時
        /// </summary>
        /// <param name="vol"></param>
        /// <param name="mute"></param>
        public void OnVolumeChanged(float vol, bool mute)
        {
            Model.Volume = (int)Math.Round(vol * 100);
            Model.IsMute = mute;
        }

        /// <summary>
        /// ev:規定の再生デバイス変更時
        /// </summary>
        /// <param name="device"></param>
        public void OnDeviceChanged(MMDevice device)
        {
            Model.SetDeviceInfo(device);
        }
        /// <summary>
         /// ev:MixerVolume変更時
         /// </summary>
         /// <param name="vol"></param>
         /// <param name="mute"></param>
        public void OnRecVolumeChanged(float vol, bool mute)
        {
            Model.RecVolume = (int)Math.Round(vol * 100);
            //Model.IsMute = mute;
        }

        /// <summary>
        /// ev:規定の再生デバイス変更時
        /// </summary>
        /// <param name="device"></param>
        public void OnRecDeviceChanged(MMDevice device)
        {
            Model.SetRecDeviceInfo(device);
        }
    }
}
