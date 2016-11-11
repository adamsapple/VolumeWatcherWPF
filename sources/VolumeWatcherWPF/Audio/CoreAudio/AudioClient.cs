using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

using Audio.Wave;
using Audio.CoreAudio.Interfaces;

namespace Audio.CoreAudio
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// https://github.com/SjB/NAudio/tree/master/NAudio/CoreAudioApi
    /// </remarks>
    public class AudioClient
    {
        private IAudioClient _realAudioClient;
        WaveFormatExtensible mixFormat;

        
        public uint BufferSize => GetBufferSize();

        private AudioRenderClient _AudioRenderClient;
        public AudioRenderClient AudioRenderClient
        {
            get
            {
                if(_AudioRenderClient == null) GetAudioRenderClient();
                return _AudioRenderClient;
            }
        }
        

        internal AudioClient(IAudioClient realAudioClient)
        {
            _realAudioClient = realAudioClient;
            GetMixFormat();
        }

        public void Dispose(bool disposing)
        {
            _realAudioClient = null;
            mixFormat = null;
        }


        public void  Initialize(DeviceShareMode shareMode,
            EAudioClientStreamFlags streamFlags,
            long bufferDuration,
            long periodicity,
            WaveFormat format,
            Guid audioSessionGuid)
        {
            var hresult = _realAudioClient.Initialize(shareMode, streamFlags, bufferDuration, periodicity, format, ref audioSessionGuid);
            Marshal.ThrowExceptionForHR(hresult);
            // may have changed the mix format so reset it
            mixFormat = null;

        }


        public WaveFormatExtensible MixFormat => mixFormat;
        private void GetMixFormat()
        {
            Marshal.ThrowExceptionForHR( _realAudioClient.GetMixFormat(out mixFormat) );
        }

        public bool IsFormatSupported(DeviceShareMode shareMode, out WaveFormatExtensible closestMatchFormat) {
            Marshal.ThrowExceptionForHR(_realAudioClient.IsFormatSupported(shareMode, mixFormat, out closestMatchFormat));
            return true;
        }

        private uint GetBufferSize()
        {
            uint result;
            Marshal.ThrowExceptionForHR(_realAudioClient.GetBufferSize(out result));

            return result;
        }

        private void GetAudioRenderClient()
        {
            object result;
            var IID_AUDIO_RENDER_CLIENT = typeof(IAudioRenderClient).GUID;
            Marshal.ThrowExceptionForHR(_realAudioClient.GetService(IID_AUDIO_RENDER_CLIENT, out result));
            _AudioRenderClient = new AudioRenderClient(result as IAudioRenderClient);
        }


        /*
        [PreserveSig]
        int GetService(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid interfaceId,
            [Out, MarshalAs(UnmanagedType.IUnknown)] out object interfacePointer);
        

        [PreserveSig]
        int GetStreamLatency([Out] [MarshalAs(UnmanagedType.I8)] out long bufferSize);

        [PreserveSig]
        int GetCurrentPadding(out int currentPadding);

        [PreserveSig]
        int IsFormatSupported(
            DeviceShareMode shareMode,
            [In] WaveFormat pFormat,
            [Out] out WaveFormatExtensible closestMatchFormat);

        [PreserveSig]
        int GetMixFormat([Out] out WaveFormatExtensible format);

        [PreserveSig]
        int GetDevicePeriod(
            [Out] [MarshalAs(UnmanagedType.I8)] out long defaultDevicePeriod,
            [Out] [MarshalAs(UnmanagedType.I8)]out long minimumDevicePeriod);*/

        public void Start()
        {
            _realAudioClient.Start();
        }

        public void Stop()
        {
            _realAudioClient.Stop();
        }

        public void Reset()
        {
            _realAudioClient.Reset();
        }

        
    }
}
