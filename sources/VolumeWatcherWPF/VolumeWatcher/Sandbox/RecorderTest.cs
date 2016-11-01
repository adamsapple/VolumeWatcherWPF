using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Audio.CoreAudio;

namespace VolumeWatcher.Sandbox
{
    class RecorderTest
    {
        public RecorderTest()
        {
            // get the speakers (1st render + multimedia) device
            var deviceEnumerator = new MMDeviceEnumerator();

            // get default device.
            //device = deviceEnumerator.GetDefaultAudioEndpoint(EDataFlow.eCapture, ERole.eCommunications|ERole.eMultimedia);

            var collection = deviceEnumerator.EnumAudioEndpoints(EDataFlow.eCapture, EDeviceState.Active);

            var devices = new List<MMDevice>();
            


            for (int i=0,len=collection.Count; i<len; ++i)
            {
                devices.Add(collection[i]);
            }
            
            devices.ForEach( (i) => {
                var str = (string)i.GetProperty(PropertyKeys.PKEY_DEVICE_FRIENDLY_NAME);
                //var str = device.FriendlyName;
                Console.WriteLine(str);
            });

        }
    }
}
