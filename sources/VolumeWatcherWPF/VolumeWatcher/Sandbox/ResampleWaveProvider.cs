using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Audio.Wave;

namespace VolumeWatcher.Sandbox
{
    /// <summary>
    /// 超簡易的にリサンプリングするWaveProvider。
    /// 聞ければよいって程度のシロモノ。補間は行わずNearestな処理。
    /// </summary>
    class ResampleWaveProvider : IWaveProvider
    {
        #region Member Fields.

        private byte[] TempBuffer;
        private double ratio;
        private IWaveProvider InputProvider;

        #endregion

        #region Properties.

        public WaveFormat WaveFormat { get; private set; }
        public WaveFormat InWaveFormat => InputProvider.WaveFormat;
        
        #endregion

        /// <summary>
        /// Creates a new buffered WaveProvider
        /// </summary>
        /// <param name="waveFormat">WaveFormat</param>
        public ResampleWaveProvider(IWaveProvider inProvider, WaveFormat outWaveFormat)
        {
            InputProvider = inProvider;
            WaveFormat    = outWaveFormat;

            SetOutWaveFormat(outWaveFormat);
        }

        private void SetOutWaveFormat(WaveFormat outWaveFormat)
        {
            ratio = (double)outWaveFormat.SampleRate / InWaveFormat.SampleRate;
            TempBuffer = new byte[outWaveFormat.AverageBytesPerSecond * 5];
        }

        /*
        public void AddSamples(byte[] buffer, int offset, int count)
        {
            int BytesPerSample = WaveFormat.BitsPerSample / 8;  // 共有モードは32bit固定っぽい
            int nChannels      = WaveFormat.Channels;           // 
            int dst_id         = 0;
            int dst_count      = (int)(count * ratio);
            double rraito      = 1.0 / ratio;
            double src_frame   = 0;

            while(dst_id < dst_count)
            {
                int sr_id_tmp = offset + (int)src_frame * BytesPerSample * nChannels;

                for (var j = 0; j < nChannels; ++j)
                {
                    Array.Copy(buffer, sr_id_tmp, TempBuffer, dst_id, BytesPerSample);
                    dst_id    += BytesPerSample;
                    sr_id_tmp += BytesPerSample;
                }
                src_frame += rraito;
            }
            BufferedProvider.AddSamples(TempBuffer, 0, dst_count);
        }
        */

        /// <summary>
        /// リサンプリングしながらbufferに書き込む
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns>num read bytes.</returns>
        public int Read(byte[] buffer, int offset, int count)
        {
            double rratio      = 1.0 / ratio;
            double src_frame   = 0;
            byte[] src         = TempBuffer;
            int BytesPerSample = WaveFormat.BitsPerSample / 8;  // 共有モードは32bit固定っぽい
            int BlockAlign     = WaveFormat.BlockAlign;
            int dst_id         = offset;
            int src_count_tmp  = (int)Math.Ceiling(count * rratio) / BytesPerSample * BytesPerSample;
            int src_count      = InputProvider.Read(src, 0, src_count_tmp);
            int dst_count      = count * src_count / src_count_tmp + offset;

            while (dst_id < dst_count)
            {
                int src_id_tmp = offset + (int)src_frame * BlockAlign;

                Array.Copy(src, src_id_tmp, buffer, dst_id, BlockAlign);

                dst_id    += BlockAlign;
                src_frame += rratio;
            }

            return dst_count;
        }
    }
}
