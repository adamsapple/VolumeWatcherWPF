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
        private AudioMeterInformation   _AudioMeterInformation;
        private AudioEndpointVolume     _AudioEndpointVolume;
        private AudioClient             _AudioClient;
        //private AudioSessionManager _AudioSessionManager;


        private static Guid IID_IAudioMeterInformation = typeof(IAudioMeterInformation).GUID;
        private static Guid IID_IAudioEndpointVolume   = typeof(IAudioEndpointVolume).GUID;
        private static Guid IID_IAudioClient           = typeof(IAudioClient).GUID;

        private readonly string _Id;

        internal MMDevice(IMMDevice realDevice)
        {
            Marshal.ThrowExceptionForHR(realDevice.GetId(out _Id));


            _RealDevice = realDevice;
        }


        private PropertyStore GetProperty()
        {
            IPropertyStore propstore;
            Marshal.ThrowExceptionForHR(_RealDevice.OpenPropertyStore(EStgmAccess.STGM_READ, out propstore));
            return new PropertyStore(propstore);
        }


        private EDeviceState GetState()
        {
            EDeviceState result;
            Marshal.ThrowExceptionForHR(_RealDevice.GetState(out result));

            return result;
        }
        
        private void GetAudioMeterInformation()
        {
            object result;
            Marshal.ThrowExceptionForHR(_RealDevice.Activate(ref IID_IAudioMeterInformation, ClsCtx.InprocServer, IntPtr.Zero, out result));
            _AudioMeterInformation = new AudioMeterInformation(result as IAudioMeterInformation);
        }
        
        private void GetAudioEndpointVolume()
        {
            object result;
            Marshal.ThrowExceptionForHR(_RealDevice.Activate(ref IID_IAudioEndpointVolume, ClsCtx.All, IntPtr.Zero, out result));
            //_AudioEndpointVolume = new AudioEndpointVolume(result as IAudioEndpointVolume);
            _AudioEndpointVolume = new AudioEndpointVolume( result as IAudioEndpointVolume );
        }

        private void GetAudioClient()
        {
            object result;
            Marshal.ThrowExceptionForHR(_RealDevice.Activate(ref IID_IAudioClient, ClsCtx.All, IntPtr.Zero, out result));
            _AudioClient = new CoreAudio.AudioClient(result as IAudioClient);
        }

        public EDeviceState State => GetState();
        public string Id => _Id;

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

        public AudioClient AudioClient
        {
            get
            {
                if (_AudioClient == null)
                    GetAudioClient();

                return _AudioClient;
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

        protected virtual void Dispose(bool disposing)
        {
            _AudioEndpointVolume?.Dispose(disposing);
            _AudioMeterInformation?.Dispose(disposing);
            _PropertyStore = null;
        }

        ~MMDevice()
        {
            Dispose(false);
        }
    }
}
