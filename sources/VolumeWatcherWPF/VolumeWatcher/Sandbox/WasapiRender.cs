using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Runtime.InteropServices;
using System.Diagnostics;

using Audio.CoreAudio;
using Audio.CoreAudio.Interfaces;
using Audio.Wave;

namespace VolumeWatcher.Sandbox
{
    class WasapiRender
    {
        private AudioClient audioClient;
        private EAudioClientShareMode shareMode;
        private AudioRenderClient renderClient;
        private IWaveProvider sourceProvider;
        private int latencyMilliseconds;
        
        private bool isUsingEventSync;
        private EventWaitHandle frameEventWaitHandle;
        private byte[] readBuffer;
        
        private Thread playThread;
        private WaveFormat outputFormat;
        public WaveFormat WaveFormat => outputFormat;
        private SynchronizationContext syncContext;

        public EPlaybackState PlaybackState { get; private set; }
        //public event EventHandler<StoppedEventArgs> PlaybackStopped;

        /// <summary>
        /// WASAPI Out using default audio endpoint
        /// </summary>
        /// <param name="shareMode">ShareMode - shared or exclusive</param>
        /// <param name="latency">Desired latency in milliseconds</param>
        public WasapiRender(EAudioClientShareMode shareMode, int latency) :
                this(GetDefaultAudioEndpoint(), shareMode, true, latency)
        {
        }

        /// <summary>
        /// WASAPI Out using default audio endpoint
        /// </summary>
        /// <param name="shareMode">ShareMode - shared or exclusive</param>
        /// <param name="useEventSync">true if sync is done with event. false use sleep.</param>
        /// <param name="latency">Desired latency in milliseconds</param>
        public WasapiRender(EAudioClientShareMode shareMode, bool useEventSync, int latency) :
                this(GetDefaultAudioEndpoint(), shareMode, useEventSync, latency)
        {
        }

        /// <summary>
        /// Creates a new WASAPI Output
        /// </summary>
        /// <param name="device">Device to use</param>
        /// <param name="shareMode"></param>
        /// <param name="useEventSync">true if sync is done with event. false use sleep.</param>
        /// <param name="latency"></param>
        public WasapiRender(MMDevice device, EAudioClientShareMode shareMode, bool useEventSync, int latency)
        {
            this.audioClient         = device.AudioClient;
            this.shareMode           = shareMode;
            this.isUsingEventSync    = useEventSync;
            this.latencyMilliseconds = latency;
            this.syncContext         = SynchronizationContext.Current;
        }

        static MMDevice GetDefaultAudioEndpoint()
        {
            if (Environment.OSVersion.Version.Major < 6)
            {
                throw new NotSupportedException("WASAPI supported only on Windows Vista and above");
            }
            MMDeviceEnumerator enumerator = new MMDeviceEnumerator();
            return enumerator.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eConsole);
        }

        private void PlayThread()
        {
            IWaveProvider playbackProvider = this.sourceProvider;
            Exception     exception        = null;
            try
            {
                // fill a whole buffer
                var bufferFrameCount = audioClient.BufferSize;
                //var bytesPerFrame    = outputFormat.Channels * outputFormat.BitsPerSample / 8;
                
                readBuffer       = new byte[bufferFrameCount * outputFormat.BlockAlign];
                FillBuffer(playbackProvider, bufferFrameCount);

                // Create WaitHandle for sync
                //WaitHandle[] waitHandles = new WaitHandle[] { frameEventWaitHandle };

                audioClient.Start();

                while (PlaybackState != EPlaybackState.Stopped)
                {
                    // If using Event Sync, Wait for notification from AudioClient or Sleep half latency
                    int indexHandle = 0;
                    if (isUsingEventSync)
                    {
                        //indexHandle = WaitHandle.WaitAny(waitHandles, 3 * latencyMilliseconds, false);
                        frameEventWaitHandle.WaitOne(3 * latencyMilliseconds);
                    }
                    else
                    {
                        Thread.Sleep(latencyMilliseconds / 2);
                    }

                    // If still playing and notification is ok
                    if (PlaybackState == EPlaybackState.Playing && indexHandle != WaitHandle.WaitTimeout)
                    {
                        // See how much buffer space is available.
                        int numFramesPadding = 0;
                        if (isUsingEventSync)
                        {
                            // In exclusive mode, always ask the max = bufferFrameCount = audioClient.BufferSize
                            numFramesPadding = (shareMode == EAudioClientShareMode.Shared) ? audioClient.CurrentPadding : 0;
                        }
                        else
                        {
                            numFramesPadding = audioClient.CurrentPadding;
                        }
                        int numFramesAvailable = bufferFrameCount - numFramesPadding;
                        if (numFramesAvailable > 0)
                        {
                            FillBuffer(playbackProvider, numFramesAvailable);
                        }
                    }
                }
                Thread.Sleep(latencyMilliseconds / 2);
                audioClient.Stop();
                if (PlaybackState == EPlaybackState.Stopped)
                {
                    audioClient.Reset();
                }
            }
            catch (Exception e)
            {
                exception = e;
            }
            finally
            {
                //RaisePlaybackStopped(exception);
            }
        }

        /*
        private void RaisePlaybackStopped(Exception e)
        {
            var handler = PlaybackStopped;
            if (handler != null)
            {
                if (this.syncContext == null)
                {
                    handler(this, new StoppedEventArgs(e));
                }
                else
                {
                    syncContext.Post(state => handler(this, new StoppedEventArgs(e)), null);
                }
            }
        }
        */

        private void FillBuffer(IWaveProvider playbackProvider, int frameCount)
        {
            IntPtr buffer  = renderClient.GetBuffer(frameCount);
            int readLength = frameCount * outputFormat.BlockAlign;
            int read       = playbackProvider.Read(readBuffer, 0, readLength);

            if (read == 0)
            {
                PlaybackState = EPlaybackState.Stopped;
            }
            Marshal.Copy(readBuffer, 0, buffer, read);
            int actualFrameCount = read / outputFormat.BlockAlign;
            /*if (actualFrameCount != frameCount)
            {
                Debug.WriteLine(String.Format("WASAPI wanted {0} frames, supplied {1}", frameCount, actualFrameCount ));
            }*/
            renderClient.ReleaseBuffer(actualFrameCount, EAudioClientBufferFlags.None);
        }

        /// <summary>
        /// Begin Playback
        /// </summary>
        public void Play()
        {
            if (PlaybackState != EPlaybackState.Playing)
            {
                if (PlaybackState == EPlaybackState.Stopped)
                {
                    playThread = new Thread(new ThreadStart(PlayThread));
                    playThread.Start();
                }

                PlaybackState = EPlaybackState.Playing;
            }
        }

        /// <summary>
        /// Stop playback and flush buffers
        /// </summary>
        public void Stop()
        {
            if (PlaybackState != EPlaybackState.Stopped)
            {
                PlaybackState = EPlaybackState.Stopped;
                playThread.Join();
                playThread = null;
            }
        }

        /// <summary>
        /// Stop playback without flushing buffers
        /// </summary>
        public void Pause()
        {
            if (PlaybackState == EPlaybackState.Playing)
            {
                PlaybackState = EPlaybackState.Paused;
            }
        }

        /// <summary>
        /// Initialize for playing the specified wave stream
        /// </summary>
        /// <param name="waveProvider">IWaveProvider to play</param>
        public void Initialize(IWaveProvider waveProvider)
        {
            long latencyRefTimes = latencyMilliseconds * 10000;

            outputFormat = waveProvider.WaveFormat;
            var approFormat = audioClient.CheckSupportFormat(shareMode, outputFormat);        // 規定フォーマットが対応しているかどうかのCheck.
            if (approFormat != null)
            {
                // 対応していない場合は、システムから提示のあった近似フォーマットで再度確認する。
                if (!audioClient.IsFormatSupported(shareMode, approFormat))
                {
                    throw new ArgumentException("Unsupported Wave Format");
                }
                outputFormat = approFormat;
            }
            //audioClient.Initialize(shareMode, EAudioClientStreamFlags.None, 1000000, 0, outputFormat, Guid.Empty);

            this.sourceProvider = waveProvider;

            // If using EventSync, setup is specific with shareMode
            if (isUsingEventSync)
            {
                // Init Shared or Exclusive
                if (shareMode == EAudioClientShareMode.Shared)
                {
                    // With EventCallBack and Shared, both latencies must be set to 0
                    audioClient.Initialize(shareMode, EAudioClientStreamFlags.EventCallback, 0, 0, outputFormat, Guid.Empty);
                    // Get back the effective latency from AudioClient
                    latencyMilliseconds = (int)(audioClient.StreamLatency / 10000);
                }
                else
                {
                    // With EventCallBack and Exclusive, both latencies must equals
                    audioClient.Initialize(shareMode, EAudioClientStreamFlags.EventCallback, latencyRefTimes, latencyRefTimes, outputFormat, Guid.Empty);
                }

                // Create the Wait Event Handle
                frameEventWaitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);
                audioClient.SetEventHandle(frameEventWaitHandle);
            }
            else
            {
                // Normal setup for both sharedMode
                audioClient.Initialize(shareMode, EAudioClientStreamFlags.None, latencyRefTimes, 0, outputFormat, Guid.Empty);
            }
            
            Debug.WriteLine(string.Format("RenderAudioClient: {0}", audioClient.ToString()));
            // Get the RenderClient
            renderClient = audioClient.AudioRenderClient;
        }

        #region IDisposable Members

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            if (audioClient != null)
            {
                Stop();

                audioClient.Dispose();
                audioClient = null;
                renderClient= null;
            }
        }

        #endregion
    }
}
