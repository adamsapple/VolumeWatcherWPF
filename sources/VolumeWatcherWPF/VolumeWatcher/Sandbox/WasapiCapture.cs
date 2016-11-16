using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.Runtime.InteropServices;

using Audio.CoreAudio;
using Audio.CoreAudio.Interfaces;
using Audio.Wave;


namespace VolumeWatcher.Sandbox
{
    class WasapiCapture
    {

        /*/
        // 基本時間
        private const long REFTIMES_PER_SEC      = 10000000;
        private const long REFTIMES_PER_MILLISEC = 10000;
        //*/
        private const long REFTIMES_PER_SEC      = 10000000;
        private const long REFTIMES_PER_MILLISEC = REFTIMES_PER_SEC/1000;
        //*/

        private MMDevice        device;
        private AudioClient     audioClient;
        private Thread          captureThread;

        private bool            isInitialized;
        private volatile bool   stop;
        internal byte[]          recordBuffer;
        private byte[]          tempBuffer;
        private int             bytesPerFrame;

        private BufferedWaveProvider waveProvider;
        public IWaveProvider WaveProvider => waveProvider;

        /// <summary>
        /// Recording wave format
        /// </summary>
        public virtual WaveFormat WaveFormat { get; set; }


        /// <summary>
        /// Indicates recorded data is available 
        /// </summary>
        //public event EventHandler<WaveInEventArgs> DataAvailable;

        /// <summary>
        /// Indicates that all recorded data has now been received.
        /// </summary>
        //public event EventHandler<StoppedEventArgs> RecordingStopped;

        /// <summary>
        /// Initialises a new instance of the WASAPI capture class
        /// </summary>
        public WasapiCapture() :
            this(GetDefaultCaptureDevice())
        {
        }

        /// <summary>
        /// Initialises a new instance of the WASAPI capture class
        /// </summary>
        /// <param name="captureDevice">Capture device to use</param>
        public WasapiCapture(MMDevice captureDevice)
        {
            this.device         = captureDevice;
            this.audioClient    = device.AudioClient;
            this.WaveFormat     = audioClient.MixFormat;
            this.isInitialized  = false;
        }

        /// <summary>
        /// Gets the default audio capture device
        /// </summary>
        /// <returns>The default audio capture device</returns>
        public static MMDevice GetDefaultCaptureDevice()
        {
            MMDeviceEnumerator devices = new MMDeviceEnumerator();
            return devices.GetDefaultAudioEndpoint(EDataFlow.eCapture, ERole.eConsole);
        }

        private void InitializeCaptureDevice()
        {
            if (isInitialized)
                return;

            long requestedDuration = REFTIMES_PER_MILLISEC * 100;
            var streamFlags        = GetAudioClientStreamFlags();
            var shareMode          = EAudioClientShareMode.Shared;

            var approFormat = audioClient.CheckSupportFormat(shareMode, WaveFormat);        // 規定フォーマットが対応しているかどうかのCheck.
            if (approFormat != null)
            {
                // 対応していない場合は、システムから提示のあった近似フォーマットで再度確認する。
                if (!audioClient.IsFormatSupported(shareMode, approFormat))
                {
                    throw new ArgumentException("Unsupported Wave Format");
                }
                // 近似フォーマットを採用して続行。
                WaveFormat = approFormat;
            }

            audioClient.Initialize(
                shareMode,
                streamFlags,
                requestedDuration,
                0,
                WaveFormat,
                Guid.Empty);
            Debug.WriteLine(string.Format("CaptureAudioClient: {0}", audioClient.ToString()));

            var bufferFrameCount = audioClient.BufferSize;
            this.bytesPerFrame   = this.WaveFormat.Channels * this.WaveFormat.BitsPerSample / 8;
            this.recordBuffer    = new byte[bufferFrameCount * bytesPerFrame];
            Debug.WriteLine(string.Format("record buffer size = {0}", this.recordBuffer.Length));

            //WaveProvider = new InnerWaveProvider(this);
            waveProvider = new BufferedWaveProvider(WaveFormat);

            isInitialized = true;
        }

        /// <summary>
        /// To allow overrides to specify different flags (e.g. loopback)
        /// </summary>
        protected virtual EAudioClientStreamFlags GetAudioClientStreamFlags()
        {
            return EAudioClientStreamFlags.None;
        }

        /// <summary>
        /// Start Recording
        /// </summary>
        public void StartRecording()
        {
            InitializeCaptureDevice();
            ThreadStart start = delegate { this.CaptureThread(this.audioClient); };
            this.captureThread = new Thread(start);

            Debug.WriteLine("Thread starting...");
            this.stop = false;
            this.captureThread.Start();
        }

        /// <summary>
        /// Stop Recording
        /// </summary>
        public void StopRecording()
        {
            if (this.captureThread != null)
            {
                this.stop = true;

                Debug.WriteLine("Thread ending...");

                // wait for thread to end
                this.captureThread.Join();
                this.captureThread = null;

                Debug.WriteLine("Done.");

                this.stop = false;
            }
        }

        private void CaptureThread(AudioClient client)
        {
            Exception exception = null;
            try
            {
                DoRecording(client);
            }
            catch (Exception e)
            {
                exception = e;
            }
            finally
            {
                client.Stop();
                // don't dispose - the AudioClient only gets disposed when WasapiCapture is disposed
            }

            RaiseRecordingStopped(exception);
            System.Diagnostics.Debug.WriteLine("stop wasapi");
        }

        private void DoRecording(AudioClient client)
        {
            Debug.WriteLine(string.Format("num Buffer Frames: {0}",client.BufferSize));
            int bufferFrameCount = audioClient.BufferSize;

            // Calculate the actual duration of the allocated buffer.
            long actualDuration = (long)((double)REFTIMES_PER_SEC *
                             bufferFrameCount / WaveFormat.SampleRate);
            int sleepMilliseconds = (int)(actualDuration / REFTIMES_PER_MILLISEC / 2);

            AudioCaptureClient capture = client.AudioCaptureClient;
            client.Start();
            Debug.WriteLine(string.Format("sleep: {0} ms", sleepMilliseconds));
            while (!this.stop)
            {
                Thread.Sleep(sleepMilliseconds);
                ReadNextPacket(capture);
            }
        }

        private void RaiseRecordingStopped(Exception exception)
        {
            //var handler = RecordingStopped;
            //if (handler != null)
            //{
            //    handler(this, new StoppedEventArgs(exception));
            //}
        }

        private void ReadNextPacket(AudioCaptureClient capture)
        {
            EAudioClientBufferFlags flags;
            int framesAvailable    = 0;
            int recordBufferOffset = 0;
            int packetSize         = capture.GetNextPacketSize();
            //Debug.WriteLine(string.Format("packet size: {0} samples", packetSize / 4));

            while (packetSize != 0)
            {
                var buffer         = capture.GetBuffer(out framesAvailable, out flags);
                int bytesAvailable = framesAvailable * bytesPerFrame;
                // apparently it is sometimes possible to read more frames than we were expecting?
                // fix suggested by Michael Feld:
                int spaceRemaining = Math.Max(0, recordBuffer.Length - recordBufferOffset);

                if (spaceRemaining < bytesAvailable && recordBufferOffset > 0)
                {
                    //if (DataAvailable != null) DataAvailable(this, new WaveInEventArgs(recordBuffer, recordBufferOffset));
                    recordBufferOffset = 0;
                }

                // if not silence...
                if ((flags & EAudioClientBufferFlags.Silent) != EAudioClientBufferFlags.Silent)
                {
                    Marshal.Copy(buffer, recordBuffer, recordBufferOffset, bytesAvailable);
                    waveProvider.AddSamples(recordBuffer, recordBufferOffset, bytesAvailable);
                }
                else
                {
                    Array.Clear(recordBuffer, recordBufferOffset, bytesAvailable);
                }
                recordBufferOffset += bytesAvailable;
                capture.ReleaseBuffer(framesAvailable);
                packetSize = capture.GetNextPacketSize();
            }
            //if (DataAvailable != null)
            //{
            //    DataAvailable(this, new WaveInEventArgs(recordBuffer, recordBufferOffset));
            //}
        }

        #region Dispose Members.

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            StopRecording();
            if (audioClient != null)
            {
                audioClient.Dispose();
                audioClient = null;
            }
        }

        #endregion


        public class InnerWaveProvider : IWaveProvider
        {
            WasapiCapture parent;
            internal int Offset { get; set; }
            internal int Length { get; set; }

            public InnerWaveProvider(WasapiCapture parent)
            {
                this.parent = parent;
                WaveFormat  = parent.WaveFormat;
            }

            public WaveFormat WaveFormat { get; }
            public int Read(byte[] buffer, int offset, int count)
            {
                int srcoffs = Offset;
                int length = Math.Min(Length, count);
                byte[] src  = parent.recordBuffer;

                Array.Copy(src, srcoffs, buffer, offset, length);
                int result = length;
                if(src.Length-srcoffs< result)
                {
                    result = src.Length - srcoffs;
                }
                return result;
            }
        }
    }
}
