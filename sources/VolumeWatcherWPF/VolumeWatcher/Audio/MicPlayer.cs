using System;
using System.Diagnostics;
using System.Windows;
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
        
        public delegate void MicPlayerStateChangedDelegate(EMicState state);


        MMDeviceEnumerator deviceEnumerator;
        WasapiCapture   capture;
        WasapiRender    render;

        public MicPlayerStateChangedDelegate OnStateChanged;

        public bool Initialized { get; private set; } = false;
        public bool IsRunning => (render?.IsRunning??false)|(capture?.IsRunning??false);
        public ERole Role => ERole.eConsole;
        public EAudioClientShareMode ShareMode => EAudioClientShareMode.Shared;
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
            var deviceCapture = deviceEnumerator.GetDefaultAudioEndpoint(EDataFlow.eCapture, Role);
            var deviceRender = deviceEnumerator.GetDefaultAudioEndpoint(EDataFlow.eRender, Role);
            
            if(deviceCapture==null || deviceRender == null)
            {
                OnStateChanged?.Invoke(EMicState.InitializeFailed);
                return;
            }

            capture = new WasapiCapture(deviceCapture);                   // Captureデバイスの準備
            render = new WasapiRender(deviceRender, ShareMode, true, 0);  // Renderデバイスの準備

            capture.Initialize();
            render.Initialize(capture.WaveProvider);

            capture.StoppedEvent += OnCaptureStopped;
            render.StoppedEvent  += OnCaptureStopped;

            Debug.WriteLine(string.Format("capture format:{0}", capture.WaveFormat));
            Debug.WriteLine(string.Format("render  format:{0}", render.WaveFormat));

            deviceEnumerator.OnDefaultDeviceChanged += DeviceChanged;

            Initialized = true;
            OnStateChanged?.Invoke(EMicState.Initialized);
        }

        private void Capture_StoppedEvent(object sender, WasapiStopEventArgs e)
        {
            throw new NotImplementedException();
        }

        void DeviceChanged(
                [MarshalAs(UnmanagedType.I4)] EDataFlow dataFlow,
                [MarshalAs(UnmanagedType.I4)] ERole deviceRole,
                [MarshalAs(UnmanagedType.LPWStr)] string defaultDeviceId)
        {
            if(deviceRole != this.Role)
            {
                return;
            }

            var dispatcher = Application.Current.Dispatcher;
            //dispatcher.Invoke(DispatcherPriority.Normal, (Action)delegate () {
            dispatcher.Invoke((Action)delegate ()
            {
                if (!Initialized)
                {
                    return;
                }

                Debug.WriteLine($"MicPlayter Stop(DeviceChanged) Role={deviceRole} Flow={dataFlow} ID={defaultDeviceId}");
                Stop();
                Dispose();
                //var newDevice = deviceEnumerator.GetDevice(defaultDeviceId);
            });
            
        }

        private void OnCaptureStopped(object sender, WasapiStopEventArgs e)
        {
            if(sender == capture)
            {
                Debug.WriteLine("OnCaptureStopped");
            }
            else if (sender == render)
            {
                Debug.WriteLine("OnRenderStopped");
            }
            
            if (e.Exception != null)
            {
                Debug.WriteLine("  Exception:" + e);
                Stop();
                Dispose();
            }
        }
        
        public void Start()
        {
            if (IsRunning)
            {
                return;
            }

            Initialize();

            if (!Initialized)
            {
                return;
            }

            capture.Start();
            render.Play();

            OnStateChanged?.Invoke(EMicState.Start);
        }

        public void Stop()
        {
            if (!IsRunning)
            {
                return;
            }

            render.Stop();
            capture.Stop();
            OnStateChanged?.Invoke(EMicState.Stop);
        }

        public void Dispose()
        {
            if (!Initialized)
            {
                return;
            }
            Stop();
            //capture?.Dispose();
            //render?.Dispose();
            capture = null;
            render  = null;
            Initialized = false;
        }

        ~MicPlayer()
        {
            Dispose();
        }
    }
}
