using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Audio.CoreAudio;
using Audio.CoreAudio.Interfaces;

namespace VolumeWatcher.Sandbox
{
    class RecorderTest
    {

        public RecorderTest()
        {
            var deviceEnumerator = new MMDeviceEnumerator();

            // get default device.
            var device = deviceEnumerator.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia);

            //PutDeviceInfo(device);
            //device.AudioEndpointVolume.Mute = !device.AudioEndpointVolume.Mute;

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

            var audioClient = device.AudioClient;
            var formatTag   = audioClient.MixFormat;
            Console.WriteLine(formatTag);
            var result      = audioClient.IsFormatSupported(DeviceShareMode.Shared);

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
    }
}
