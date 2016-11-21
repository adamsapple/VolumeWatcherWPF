using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VolumeWatcher.Audio
{
    public enum EMicState
    {
        NoInitialized,
        Initialized,
        InitializeFailed,
        Start,
        Stop,
    }
}
