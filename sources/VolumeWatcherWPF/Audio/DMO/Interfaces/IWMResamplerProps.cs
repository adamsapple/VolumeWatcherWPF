using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Audio.DMO.Interfaces
{
    /// <summary>
    /// Windows Media Resampler Props
    /// wmcodecdsp.h
    /// </summary>
    [Guid(ComIds.IID_WM_RESAMPLER_PROPS),
        InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    interface IWMResamplerProps
    {
        /// <summary>
        /// Range is 1 to 60
        /// </summary>
        int SetHalfFilterLength(int outputQuality);

        int SetUserChannelMtx([In] float[] channelConversionMatrix);
    }
}
