using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

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

        public WaveFormatExtensible MixFormat => mixFormat;
        private void GetMixFormat()
        {
            Marshal.ThrowExceptionForHR( _realAudioClient.GetMixFormat(out mixFormat) );
        }

        public bool IsFormatSupported(DeviceShareMode shareMode) {
            WaveFormatExtensible closestMatchFormat;
            Marshal.ThrowExceptionForHR(_realAudioClient.IsFormatSupported(shareMode, mixFormat, out closestMatchFormat));
            return true;
        }

        public EAudioClientReturnCode Start()
        {
            return (EAudioClientReturnCode)_realAudioClient.Start();
        }

        public EAudioClientReturnCode Stop()
        {
            return (EAudioClientReturnCode)_realAudioClient.Stop();
        }

        public EAudioClientReturnCode Reset()
        {
            return (EAudioClientReturnCode)_realAudioClient.Reset();
        }

        
    }
}
