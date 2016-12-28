using System;
using System.Collections.Generic;
using System.Windows.Input;
using System.Windows.Controls;

using VolumeWatcher.Component;

namespace VolumeWatcher.Command
{
    /// <summary>
    /// キーボードのショートカット機能をCommandに逃がそうとして失敗した、作りかけ。
    /// ちゃんと動きません。
    /// </summary>
    class KeyboardHookCommand : SimpleCommandBase
    {
        public float Step { get; set; } = 0.02f;
        private VolumeWatcherMain main;

        private KeyboardHookComponent KeyboardHooker;
        
        public KeyboardHookCommand(VolumeWatcherMain main)
        {
            this.main = main;
            this.KeyboardHooker = main.trayComponent.KeyboardHooker;

            var dict = new Dictionary<System.Windows.Input.Key, Action>()
            {
                {
                    Key.OemComma, () => {
                        // 音量-
                        var device = main.VolumeMonitor1.AudioDevice;
                        if(device == null || device.AudioEndpointVolume == null)
                        {
                            return;
                        }
                        device.AudioEndpointVolume.MasterVolumeLevelScalar -= Step;
                    }
                },
                {
                    Key.OemPeriod, () => {
                        // 音量+
                        var device = main.VolumeMonitor1.AudioDevice;
                        if(device == null || device.AudioEndpointVolume == null)
                        {
                            return;
                        }
                        device.AudioEndpointVolume.MasterVolumeLevelScalar += Step;
                    }
                },
                {
                    Key.M, () => {
                        // Mute
                        var device = main.VolumeMonitor1.AudioDevice;
                        if(device == null || device.AudioEndpointVolume == null)
                        {
                            return;
                        }
                        device.AudioEndpointVolume.Mute = !device.AudioEndpointVolume.Mute;
                    }
                },
                {
                    Key.K, () => {
                        // 音量-
                        var device = main.CaptureMonitor.AudioDevice;
                        if(device == null || device.AudioEndpointVolume == null)
                        {
                            return;
                        }
                        device.AudioEndpointVolume.MasterVolumeLevelScalar -= Step;
                    }
                },
                {
                    Key.L, () => {
                        // 音量+
                        var device = main.CaptureMonitor.AudioDevice;
                        if(device == null || device.AudioEndpointVolume == null)
                        {
                            return;
                        }
                        device.AudioEndpointVolume.MasterVolumeLevelScalar += Step;
                    }
                },
                {
                    Key.J, () => {
                        // Mute
                        var device = main.CaptureMonitor.AudioDevice;
                        if(device == null || device.AudioEndpointVolume == null)
                        {
                            return;
                        }
                        device.AudioEndpointVolume.Mute = !device.AudioEndpointVolume.Mute;
                    }
                }
            };

            foreach (var key in dict.Keys)
            {
                KeyboardHooker.AddShortcut(KeyboardHookComponent.GetMixedKeyFromKey(key, false, false, true), dict[key]);
            }
        }

        public override void Execute(object parameter)
        {
            var IsExecute = parameter as bool? ?? false;
            //var IsExecute = (parameter as CheckBox)?.IsChecked ?? false;

            KeyboardHooker.Enable = IsExecute;
        }
    }
}
