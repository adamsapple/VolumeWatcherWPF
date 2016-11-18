using System;
using System.Runtime.InteropServices;


namespace Audio.DMO.Interfaces
{
    [ComImport,
    System.Security.SuppressUnmanagedCodeSecurity,
    Guid(ComIds.IID_MEDIA_OBJECT),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    interface IMediaObject
    {
        [PreserveSig]
        int GetStreamCount(out int inputStreams, out int outputStreams);

        [PreserveSig]
        int GetInputStreamInfo(int inputStreamIndex, out EInputStreamInfoFlags flags);

        [PreserveSig]
        int GetOutputStreamInfo(int outputStreamIndex, out EOutputStreamInfoFlags flags);

        [PreserveSig]
        int GetInputType(int inputStreamIndex, int typeIndex, out DmoMediaType mediaType);

        [PreserveSig]
        int GetOutputType(int outputStreamIndex, int typeIndex, out DmoMediaType mediaType);

        [PreserveSig]
        int SetInputType(int inputStreamIndex, [In] ref DmoMediaType mediaType, EDmoSetTypeFlags flags);

        [PreserveSig]
        int SetOutputType(int outputStreamIndex, [In] ref DmoMediaType mediaType, EDmoSetTypeFlags flags);

        [PreserveSig]
        int GetInputCurrentType(int inputStreamIndex, out DmoMediaType mediaType);

        [PreserveSig]
        int GetOutputCurrentType(int outputStreamIndex, out DmoMediaType mediaType);

        [PreserveSig]
        int GetInputSizeInfo(int inputStreamIndex, out int size, out int maxLookahead, out int alignment);

        [PreserveSig]
        int GetOutputSizeInfo(int outputStreamIndex, out int size, out int alignment);

        [PreserveSig]
        int GetInputMaxLatency(int inputStreamIndex, out long referenceTimeMaxLatency);

        [PreserveSig]
        int SetInputMaxLatency(int inputStreamIndex, long referenceTimeMaxLatency);

        [PreserveSig]
        int Flush();

        [PreserveSig]
        int Discontinuity(int inputStreamIndex);

        [PreserveSig]
        int AllocateStreamingResources();

        [PreserveSig]
        int FreeStreamingResources();

        [PreserveSig]
        int GetInputStatus(int inputStreamIndex, out EDmoInputStatusFlags flags);

        [PreserveSig]
        int ProcessInput(int inputStreamIndex, [In] IMediaBuffer mediaBuffer, EDmoInputDataBufferFlags flags,
            long referenceTimeTimestamp, long referenceTimeDuration);

        [PreserveSig]
        int ProcessOutput(EDmoProcessOutputFlags flags,
            int outputBufferCount,
            [In, Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] DmoOutputDataBuffer[] outputBuffers,
            out int statusReserved);

        [PreserveSig]
        int Lock(bool acquireLock);
    }
}
