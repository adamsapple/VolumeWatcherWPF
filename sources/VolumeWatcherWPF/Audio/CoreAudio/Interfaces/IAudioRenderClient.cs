using System;
using System.Runtime.InteropServices;

namespace Audio.CoreAudio.Interfaces
{
    [Guid(ComIIds.IID_AUDIO_RENDER_CLIENT),
        InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
        ComImport]
    interface IAudioRenderClient
    {
        int GetBuffer(uint numFramesRequested, out IntPtr dataBufferPointer);
        int ReleaseBuffer(uint numFramesWritten, EAudioClientBufferFlags bufferFlags);
    }


}
