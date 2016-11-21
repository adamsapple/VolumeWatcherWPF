using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

using Audio.Wave;
using Audio.CoreAudio;
using Audio.CoreAudio.Interfaces;

using Moral.Audio;

namespace VolumeWatcher.Audio
{
    class MicPlayer :IDisposable
    {
        
        public delegate void MicPlayerStateChangedDelegate(bool IsRunning);


        MMDeviceEnumerator deviceEnumerator;
        WasapiCapture   capture;
        WasapiRender    render;

        public MicPlayerStateChangedDelegate OnStateChanged;

        public bool Initialized { get; private set; } = false;
        public bool IsRunning => render?.IsRunning??false;

        public MicPlayer()
        {
            deviceEnumerator = MMDeviceEnumerator.GetInstance();
        }

        public void Initialize()
        {
            if (Initialized)
            {
                return;
            }

            // get default device.
            var deviceCapture = deviceEnumerator.GetDefaultAudioEndpoint(EDataFlow.eCapture, ERole.eConsole);
            var deviceRender = deviceEnumerator.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eConsole);
            var shareMode = EAudioClientShareMode.Shared;
            capture = new WasapiCapture(deviceCapture);                   // Captureデバイスの準備
            render = new WasapiRender(deviceRender, shareMode, true, 0);  // Renderデバイスの準備

            capture.Initialize();
            render.Initialize(capture.WaveProvider);
            Debug.WriteLine(string.Format("capture format:{0}", capture.WaveFormat));
            Debug.WriteLine(string.Format("render  format:{0}", render.WaveFormat));

            deviceEnumerator.OnDefaultDeviceChanged += DeviceChanged;
        }


        void DeviceChanged(
                [MarshalAs(UnmanagedType.I4)] EDataFlow dataFlow,
                [MarshalAs(UnmanagedType.I4)] ERole deviceRole,
                [MarshalAs(UnmanagedType.LPWStr)] string defaultDeviceId)
        {
            var newDevice = deviceEnumerator.GetDevice(defaultDeviceId);
            Stop();
            Dispose();
        }

        public void Start()
        {
            if (IsRunning)
            {
                return;
            }

            Initialize();

            capture.Start();
            render.Play();

            OnStateChanged?.Invoke(true);
        }

        public void Stop()
        {
            if (!IsRunning)
            {
                return;
            }

            render.Stop();
            capture.Stop();
            OnStateChanged?.Invoke(false);
        }

        public void Dispose()
        {
            Stop();
            capture?.Dispose();
            render?.Dispose();
            capture = null;
            render  = null;
        }
    }
}
