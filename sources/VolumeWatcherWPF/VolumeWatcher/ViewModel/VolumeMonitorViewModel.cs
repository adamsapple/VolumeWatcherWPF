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

        public VolumeMonitorViewModel(VolumeMonitor monitor, VolumeWatcherModel model )
        {
            Model = model;
            Model.SetDeviceInfo(monitor.AudioDevice);

            monitor.OnVolumeNotification += OnVolumeChanged;
            monitor.OnDefaultDeviceChanged += OnDeviceChanged;
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
    }
}
