using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using Moral.Model;
using VolumeWatcher.Enumrate;
using Audio.CoreAudio;

namespace VolumeWatcher.Model
{
    class VolumeWatcherModel : ModelBase
    {

        #region アプリケーション設定
        public string StartupName
        {
            get { return _StartupName; }
            private set { _StartupName = value; SetProperty(ref _StartupName, value); }
        }

        public string _GitURL;
        public string GitURL
        {
            get { return _GitURL; }
            private set { _GitURL = value; SetProperty(ref _GitURL, value); }
        }
        #endregion

        #region 設定情報
        //
        // 設定情報
        //
        private float _Opacity = 0;
        public float Opacity
        {
            get { return _Opacity; }
            set { _Opacity = value; SetProperty(ref _Opacity, value); }
        }
        private EWindowPosition _WindowPosition = EWindowPosition.LEFT_TOP;
        public EWindowPosition WindowPosition
        {
            get { return _WindowPosition; }
            set { _WindowPosition = value; SetProperty(ref _WindowPosition, value); }
        }
        private bool _IsKeyHook = false;
        public bool IsKeyHook
        {
            get { return _IsKeyHook; }
            set { _IsKeyHook = value; SetProperty(ref _IsKeyHook, value); }
        }
        private string _StartupName = string.Empty;

        #endregion

        #region デバイスからの情報
        //
        // デバイスからの情報
        //
        private int _Volume = 0;
        public int Volume
        {
            get { return _Volume; }
            set { _Volume = value; SetProperty(ref _Volume, value); }
        }
        private bool _IsMute = false;
        public bool IsMute
        {
            get { return _IsMute; }
            set {
                _IsMute = value;
                SetProperty(ref _IsMute, value);
                var volume = this.RenderDevice?.AudioEndpointVolume;
                if (volume == null)
                {
                    return;
                }
                if(volume.Mute == _IsMute)
                {
                    return;
                }
                volume.Mute = _IsMute;
            }
        }

        private int _RecVolume;
        public int RecVolume
        {
            get { return _RecVolume; }
            set { _RecVolume = value; SetProperty(ref _RecVolume, value); }
        }

        private bool _IsRecMute = false;
        public bool IsRecMute
        {
            get { return _IsRecMute; }
            set
            {
                _IsRecMute = value;
                SetProperty(ref _IsRecMute, value);
                var volume = this.CaptureDevice?.AudioEndpointVolume;
                if (volume == null)
                {
                    return;
                }
                if (volume.Mute == _IsRecMute)
                {
                    return;
                }
                volume.Mute = _IsRecMute;
            }
        }
        /*
        private string _DeviceName = string.Empty;
        public string DeviceName
        {
            get { return _DeviceName; }
            private set { _DeviceName = value; SetProperty(ref _DeviceName, value); }
        }
        
        private string _IconPath = string.Empty;
        public string IconPath
        {
            get { return _IconPath; }
            set { _IconPath = value; SetProperty(ref _IconPath, value); }
        }
        */

        /*
        private AudioEndpointVolume _RenderVolume;
        public AudioEndpointVolume RenderVolume
        {
            get { return _RenderVolume; }
            set { _RenderVolume = value; SetProperty(ref _RenderVolume, value); }
        }
        */
        private MMDevice _RenderDevice;
        public MMDevice RenderDevice
        {
            get { return _RenderDevice; }
            set { _RenderDevice = value; SetProperty(ref _RenderDevice, value); }
        }

        private MMDevice _CaptureDevice;
        public MMDevice CaptureDevice
        {
            get { return _CaptureDevice; }
            set { _CaptureDevice = value; SetProperty(ref _CaptureDevice, value); }
        }

        #endregion

        #region About OptionWindowの表示位置関係
        //
        // About OptionWindowの表示位置関係
        //
        private double _OptionWindow_Left = -1.0;
        public double OptionWindow_Left
        {
            get { return _OptionWindow_Left; }
            set { _OptionWindow_Left = value; SetProperty(ref _OptionWindow_Left, value); }
        }
        private double _OptionWindow_Top = -1.0;
        public double OptionWindow_Top
        {
            get { return _OptionWindow_Top; }
            set { _OptionWindow_Top = value; SetProperty(ref _OptionWindow_Top, value); }
        }
        
        #endregion

        public void LoadSettings()
        {
            var setting = Properties.Settings.Default;

            //
            // Configのアップグレード(旧verのコンフィグから情報を引き継ぐ)
            //
            {
                // Configに書き込まれたAssemblyVersionを比較し、
                //  今回起動したApplicationを差異があれば、コンフィグのUpgradeを行う。
                var assemblyVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
                if (!setting.Version.Equals(assemblyVersion))
                {
                    setting.Upgrade();
                    setting.Version = assemblyVersion;
                    setting.Save();
                }
            }

            //
            // save情報を設定に反映
            //
            {
                this.StartupName = setting.startup_name;
                this.GitURL = setting.GitURL;
                this.Opacity = setting.window_opacity;
                this.WindowPosition = setting.window_position2;
                this.IsKeyHook = setting.enable_volume_key;

                this.OptionWindow_Left = setting.OptionWindow_Left;
                this.OptionWindow_Top = setting.OptionWindow_Top;
            }
        }

        public void SaveSettings()
        {
            var setting = Properties.Settings.Default;

            //
            // save情報を更新
            //
            setting.window_opacity      = this.Opacity;
            setting.window_position2    = this.WindowPosition;
            setting.enable_volume_key   = this.IsKeyHook;

            setting.OptionWindow_Left   = this.OptionWindow_Left;
            setting.OptionWindow_Top    = this.OptionWindow_Top;

            setting.Save();
        }

        public void SetDeviceInfo(MMDevice device)
        {
            // デバイス情報を取得し格納する
            this.RenderDevice = device;
            
            // volumeとmuteは「OnVolumeChanged」を流用
            AudioEndpointVolume volume = device?.AudioEndpointVolume;
            this.Volume = (int)Math.Round((volume?.MasterVolumeLevelScalar??0) * 100);
            this.IsMute = volume?.Mute??false;
        }

        public void SetRecDeviceInfo(MMDevice device)
        {
            // デバイス情報を取得し格納する
            this.CaptureDevice = device;

            // volumeとmuteは「OnVolumeChanged」を流用
            AudioEndpointVolume volume = device?.AudioEndpointVolume;
            this.RecVolume = (int)Math.Round((volume?.MasterVolumeLevelScalar??0) * 100);
            this.IsRecMute = volume?.Mute??false;
        }
    }
}