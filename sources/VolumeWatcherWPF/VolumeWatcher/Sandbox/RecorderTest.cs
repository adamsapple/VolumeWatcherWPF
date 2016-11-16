using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;


using Audio.Wave;
using Audio.CoreAudio;
using Audio.CoreAudio.Interfaces;

namespace VolumeWatcher.Sandbox
{
    class RecorderTest : IDisposable
    {
        WasapiCapture capture;
        WasapiRender  render;


        public RecorderTest()
        {
            var deviceEnumerator = new MMDeviceEnumerator();

            // get default device.
            var deviceCapture = deviceEnumerator.GetDefaultAudioEndpoint(EDataFlow.eCapture, ERole.eConsole);
            var deviceRender  = deviceEnumerator.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eConsole);

            PutDeviceInfo(deviceCapture);
            //device.AudioEndpointVolume.Mute = !device.AudioEndpointVolume.Mute;
            /*
            var collection = deviceEnumerator.EnumAudioEndpoints(EDataFlow.eCapture, EDeviceState.Active);

            var devices = new List<MMDevice>();

            for (int i = 0, len = collection.Count; i < len; ++i)
            {
                if (device == collection[i])
                {
                    Console.WriteLine("this is default;"+i);
                }

                devices.Add(collection[i]);
            }

            devices.ForEach((i) => {
                if (device.Equals(i))
               { 
                    Console.WriteLine("this is default;");
                }
                PutDeviceInfo(i);
            });

            if (device == null )
            {
                return;
            }
            */

            var shareMode    = EAudioClientShareMode.Shared;
            capture      = new WasapiCapture(deviceCapture);                    // Captureデバイスの準備
            capture.StartRecording();
            render       = new WasapiRender(deviceRender, shareMode, false, 0); // Renderデバイスの準備
            render.Init(capture.WaveProvider);
            render.Play();

            Console.WriteLine("capture:{0}",capture.WaveFormat);
            Console.WriteLine("render :{0}", render.WaveFormat);

            /*
            var audioClient = device.AudioClient;
            var formatTag = audioClient.MixFormat;
            Console.WriteLine("formatTag1:{0}", formatTag);
            //formatTag.BitsPerSample = 16;
            Console.WriteLine("formatTag2:{0}", formatTag);
            //WaveFormatExtensible altFormat;
            //var supported         = audioClient.IsFormatSupported(DeviceShareMode.Shared, altFormat);
            //if (altFormat != null)
            //{
            //    formatTag = altFormat;
            //}
            Console.WriteLine("altFormat:{0}", formatTag);
            // 再生レイテンシ
            const uint latency_ms_ = 50;
            const uint periods_per_buffer_ = 4; // バッファ中の区切り数（レイテンシ時間が何個あるか）
            uint buffer_period = latency_ms_ * 10000;
            uint buffer_duration = buffer_period * periods_per_buffer_;
            audioClient.Initialize(EAudioClientShareMode.Shared,
                EAudioClientStreamFlags.NoPersist,
                buffer_duration, buffer_period, formatTag, Guid.NewGuid());


            // 曲データ準備
            var musicdata = create_wave_data(formatTag, 10);

            //
            // 再生クライアント取得
            //
            var buffer_size = audioClient.BufferSize;
            var audioRenderClient = audioClient.AudioRenderClient;
            
            var buffer = audioRenderClient.GetBuffer(buffer_size);
            var size   =(int)(buffer_size / sizeof(short));
            Marshal.Copy(musicdata, 0, buffer, size);

            audioClient.Start();
            */

            //var buffer            = audioRenderClient.GetBuffer(buffer_size);
            //Marshal.FreeHGlobal(buffer);
        }

        /// <summary>
        /// MMDeviceの情報を標準出力へWriteLineする
        /// </summary>
        /// <param name="device"></param>
        private void PutDeviceInfo(MMDevice device)
        {
            Console.WriteLine("id={0}, State={1}", device.Id, device.State);
            Console.WriteLine("mute :" + device.AudioEndpointVolume.Mute);

            var keys = new List<PropertyKey>
            {
                PropertyKeys.PKEY_DEVICE_INTERFACE_FRIENDLY_NAME,
                PropertyKeys.PKEY_AUDIO_ENDPOINT_FORM_FACTOR,
                PropertyKeys.PKEY_AUDIO_ENDPOINT_CONTROL_PANEL_PAGE_PROVIDER,
                PropertyKeys.PKEY_AUDIO_ENDPOINT_ASSOCIATION,
                PropertyKeys.PKEY_AUDIO_ENDPOINT_PHYSICAL_SPEAKERS,
                PropertyKeys.PKEY_AUDIO_ENDPOINT_GUID,
                PropertyKeys.PKEY_AUDIO_ENDPOINT_DISABLE_SYS_FX,
                PropertyKeys.PKEY_AUDIO_ENDPOINT_FULL_RANGE_SPEAKERS,
                PropertyKeys.PKEY_AUDIO_ENDPOINT_SUPPORTS_EVENT_DRIVEN_MODE,
                PropertyKeys.PKEY_AUDIO_ENDPOINT_JACK_SUB_TYPE,
                PropertyKeys.PKEY_AUDIO_ENGINE_DEVICE_FORMAT,
                PropertyKeys.PKEY_AUDIO_ENGINE_OEM_FORMAT,
                PropertyKeys.PKEY_DEVICE_FRIENDLY_NAME,
                PropertyKeys.PKEY_DEVICE_DESCRIPTION,
                PropertyKeys.PKEY_DEVICE_ICON,
                PropertyKeys.PKEY_SYSTEM_NAME
            };

            keys.ForEach((i) => {
                var str = device.GetProperty(i);
                Console.WriteLine("\t:({0}) {1}", str.GetType(), str);
            });
            
            Console.WriteLine("level:"+device.AudioEndpointVolume.MasterVolumeLevelScalar);
            Console.WriteLine("mute :"+device.AudioEndpointVolume.Mute);
            Console.WriteLine("----");
        }

        short[] create_wave_data(WaveFormat format, double sec)
        {
            var length = (int)(format.SampleRate * sec /* 秒 */ * format.BlockAlign / sizeof(short));
            var result = new short[length];
            // サイン波の生成
            //var size_of_bites = buffer_size_ * format.BlockAlign;
            var id = 0; 
            //tone_buffer_.reserve(render_data_length);
            var sampleIncrement = (440 /* Hz */ * (Math.PI * 2.0)) / (double)format.SampleRate;
            var theta = 0.0;
            for (var i = 0; i < length; i += format.Channels)
            {
                double sinValue = Math.Sin(theta);
                short  val      = (short)(sinValue * short.MaxValue);
                for (var j = 0; j < format.Channels; j++)
                {
                    result[id++] = val;
                }
                theta += sampleIncrement;
            }

            return result;
        }

        #region

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            capture.StopRecording();
            render.Stop();

            capture.Dispose();
            render.Dispose();
        }

        #endregion
    }
}
