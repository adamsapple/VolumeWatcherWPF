using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using Audio.CoreAudio.Interfaces;

namespace Audio.CoreAudio
{
    public class MMDevice
    {
        private readonly IMMDevice _RealDevice;
        private PropertyStore _PropertyStore;
        private AudioMeterInformation _AudioMeterInformation;
        //private AudioEndpointVolume _AudioEndpointVolume;

        private AudioEndpointVolume _AudioEndpointVolume;
        //private AudioSessionManager _AudioSessionManager;


        private static Guid IID_IAudioMeterInformation = typeof(IAudioMeterInformation).GUID;
        private static Guid IID_IAudioEndpointVolume = typeof(IAudioEndpointVolume).GUID;

        internal MMDevice(IMMDevice realDevice)
        {
            _RealDevice = realDevice;
        }

        
        private PropertyStore GetProperty()
        {
            IPropertyStore propstore;
            Marshal.ThrowExceptionForHR(_RealDevice.OpenPropertyStore(EStgmAccess.STGM_READ, out propstore));
            return new PropertyStore(propstore);
        }

        
        private void GetAudioMeterInformation()
        {
            object result;
            Marshal.ThrowExceptionForHR(_RealDevice.Activate(ref IID_IAudioMeterInformation, ClsCtx.All, IntPtr.Zero, out result));
            _AudioMeterInformation = new AudioMeterInformation(result as IAudioMeterInformation);
        }
        
        private void GetAudioEndpointVolume()
        {
            object result;
            Marshal.ThrowExceptionForHR(_RealDevice.Activate(ref IID_IAudioEndpointVolume, ClsCtx.All, IntPtr.Zero, out result));
            //_AudioEndpointVolume = new AudioEndpointVolume(result as IAudioEndpointVolume);
            _AudioEndpointVolume = new AudioEndpointVolume( result as IAudioEndpointVolume );
        }

        public AudioEndpointVolume AudioEndpointVolume
        {
            get
            {
                if (_AudioEndpointVolume == null)
                {
                    GetAudioEndpointVolume();
                }

                return _AudioEndpointVolume;
            }
        }


        public AudioMeterInformation AudioMeterInformation
        {
            get
            {
                if (_AudioMeterInformation == null)
                    GetAudioMeterInformation();

                return _AudioMeterInformation;
            }
        }
        
        public string FriendlyName
        {
            get
            {
                if (_PropertyStore == null)
                {
                    _PropertyStore = GetProperty();
                }
                //var nameGuid = Program.settings.ShowHardwareName
                //    ? PKEY.PKEY_Device_FriendlyName
                //    : PKEY.PKEY_Device_DeviceDesc;

                //if (_PropertyStore.Contains(nameGuid))
                return (string)_PropertyStore[PropertyKeys.PKEY_DEVICE_INTERFACE_FRIENDLY_NAME].Value;
                //return "Unknown";
            }
        }
        
        public string IconPath
        {
            get
            {
                if (_PropertyStore == null)
                {
                    _PropertyStore = GetProperty();
                }
                // if (_PropertyStore.Contains(PKEY.PKEY_DeviceClass_IconPath))
                return (string)_PropertyStore[PropertyKeys.PKEY_DEVICE_ICON].Value;
                //return "Unknown";
            }
        }

        public object GetProperty(PropertyKey key)
        {
            if (_PropertyStore == null)
            {
                _PropertyStore = GetProperty();
            }

            return _PropertyStore[key].Value;
        }

        public string ID
        {
            get
            {
                string Result;
                Marshal.ThrowExceptionForHR(_RealDevice.GetId(out Result));
                return Result;
            }
        }


        internal void FireOnDeviceStateChanged(string deviceId, EDeviceState newState)
        {
        }

        internal void FireOnDeviceAdded(string deviceId)
        {

        }

        internal void FireOnDeviceRemoved(string deviceId)
        {

        }

        internal void FireOnDefaultDeviceChanged(EDataFlow dataFlow, ERole deviceRole, string defaultDeviceId)
        {

        }

        internal void FireOnPropertyValueChanged(string deviceId, PropertyKey propertyKey)
        {

        }
    }
}
