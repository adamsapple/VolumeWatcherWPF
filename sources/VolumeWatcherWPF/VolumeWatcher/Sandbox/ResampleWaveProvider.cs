using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Audio.Wave;

namespace VolumeWatcher.Sandbox
{
    /// <summary>
    /// 超簡易的にリサンプリングするWaveProvider。聞ければよいって程度のシロモノ。
    /// </summary>
    class ResampleWaveProvider : IWaveProvider
    {
        public WaveFormat WaveFormat => BufferedProvider.WaveFormat;
        public WaveFormat InWaveFormat { get; }

        private BufferedWaveProvider BufferedProvider;
        private byte[] TempBuffer;
        private double ratio;
        /// <summary>
        /// Creates a new buffered WaveProvider
        /// </summary>
        /// <param name="waveFormat">WaveFormat</param>
        public ResampleWaveProvider(WaveFormat inWaveFormat, WaveFormat outWaveFormat)
        {
            InWaveFormat     = inWaveFormat;

            SetOutWaveFormat(outWaveFormat);
        }

        public void SetOutWaveFormat(WaveFormat outWaveFormat)
        {
            BufferedProvider = new BufferedWaveProvider(outWaveFormat);
            ratio = (double)outWaveFormat.SampleRate / InWaveFormat.SampleRate;

            TempBuffer = new byte[(int)(BufferedProvider.BufferLength * ratio)];
        }

        public void AddSamples(byte[] buffer, int offset, int count)
        {
            int BytesPerSample = WaveFormat.BitsPerSample / 8;  //共有モードは32bit固定っぽい
            int i2 = 0;
            int iRaito = (int)Math.Ceiling(ratio);

            for(var i=offset; i<offset+count; i+=BytesPerSample)
            {
                for(var j=0; j< iRaito; ++j)
                {
                    Array.Copy(buffer, i, TempBuffer, i2, BytesPerSample);
                    i2 += BytesPerSample;
                }
            }
            BufferedProvider.AddSamples(TempBuffer, 0, i2);
            //BufferedProvider.AddSamples(buffer, offset, count);
        }

        public int Read(byte[] buffer, int offset, int count)
        {
            return BufferedProvider.Read(buffer, offset, count);
        }
    }
}
