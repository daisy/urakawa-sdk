using System;
using System.Diagnostics;
using System.IO;
using System.Security;
using System.Windows.Forms;
using System.Threading;
using System.Collections.Generic;

#if USE_SLIMDX
using SlimDX.DirectSound;
using SlimDX.Multimedia;
#else
using Microsoft.DirectX.DirectSound;
#endif

namespace AudioLib
{
    // This class underwent a major cleanup and simplification at revision 1488.
    // See:
    // http://daisy.trac.cvsdude.com/urakawa-sdk/changeset/1488#file0
    // Just in case we need to restore some functionality:
    // http://daisy.trac.cvsdude.com/urakawa-sdk/browser/trunk/csharp/audio/AudioLib/AudioPlayer.cs?rev=1487
    //#if NET40
    //    [SecuritySafeCritical]
    //#endif
    public class AudioPlayer
    {
        
        private readonly bool m_KeepStreamAlive;
        public AudioPlayer(bool keepStreamAlive)
        {
            m_KeepStreamAlive = keepStreamAlive;

            CurrentState = State.NotReady;
            m_AllowBackToBackPlayback = false;
            mPreviewTimer.Tick += new EventHandler(PreviewTimer_Tick);
            mPreviewTimer.Enabled = false;
        }

        /// <summary>
        /// The four states of the audio player.
        /// NotReady: the player has no output device set yet.
        /// Playing: sound is currently playing.
        /// Paused: playback was paused and can be resumed.
        /// Stopped: player is idle.
        /// </summary>
        public enum State
        {
            NotReady, Stopped,
            Playing
#if PAUSE_FEATURE_ENABLED
, Paused 
#endif //PAUSE_FEATURE_ENABLED
        };

        private State m_State;
        public State CurrentState
        {
            get
            {
                //if (mIsFwdRwd) return State.Playing; else
                return m_State;
            }
            private set
            {
                if (m_State == value)
                {
                    return;
                }

                State oldState = m_State;
                m_State = value;

                StateChangedHandler del = StateChanged;
                if (del != null && mEventsEnabled)
                    del (this, new StateChangedEventArgs(oldState));
                //var del = StateChanged;
                //if (del != null)
                    //del(this, new StateChangedEventArgs(oldState));
            }
        }

        private float m_FastPlayFactor = 1;
        /// <summary>
        /// Gets and sets fast play multiplying factor,suggested value is between 1.0 and 2.0
        /// </summary>
        public float FastPlayFactor
        {
            get
            {
                return m_FastPlayFactor;
            }
            set
            {
                m_FastPlayFactor = value;

                if (m_CircularBuffer != null &&
#if USE_SLIMDX
 m_CircularBuffer.Capabilities.ControlFrequency
#else
 m_CircularBuffer.Caps.ControlFrequency
#endif
)
                {
                    m_CircularBuffer.Frequency = (int)(m_CircularBuffer.Format.SamplesPerSecond * m_FastPlayFactor);
                }
            }
        }

        private bool m_AllowBackToBackPlayback;
        private System.Windows.Forms.Timer m_MonitoringTimer;
        public bool AllowBackToBackPlayback
        {
            get { return m_AllowBackToBackPlayback; }
            set
            {
                if (value)
                {
                    if (m_MonitoringTimer == null) m_MonitoringTimer = new System.Windows.Forms.Timer();
                    m_MonitoringTimer.Tick += new EventHandler(m_MonitoringTimer_Tick);
                    m_MonitoringTimer.Interval = 250;
                    m_MonitoringTimer.Enabled = false;
                }
                else if ( m_MonitoringTimer != null )
                {
                    m_MonitoringTimer.Enabled = false;
                    m_MonitoringTimer.Tick -= new EventHandler(m_MonitoringTimer_Tick);
                    m_MonitoringTimer.Dispose();
                    m_MonitoringTimer = null;
                    
                }
                m_AllowBackToBackPlayback = value;
            }
        }



        public delegate Stream StreamProviderDelegate();
        private StreamProviderDelegate m_CurrentAudioStreamProvider;
        private Stream m_CurrentAudioStream;
        private long m_CurrentAudioDataLength;

        public bool EnsurePlaybackStreamIsDead()
        {
            if (CurrentState == State.NotReady)
            {
                return false;
            }

            bool I_Closed_The_Stream = m_CurrentAudioStream != null;

            if (I_Closed_The_Stream)
            {
                m_CurrentAudioStream.Close();
                m_CurrentAudioStream = null;
            }

            m_CurrentAudioPCMFormat = null;
            m_CurrentAudioDataLength = 0;
            m_CurrentAudioStreamProvider = null;

            return I_Closed_The_Stream;
        }

        //private bool m_CircularBufferRefreshThreadIsAlive = false;

        private readonly Object LOCK = new object();
        private Thread m_CircularBufferRefreshThread;

#if USE_SLIMDX
        private SecondarySoundBuffer m_CircularBuffer;
#else
        private SecondaryBuffer m_CircularBuffer;
#endif
        private int m_CircularBufferRefreshChunkSize;
        private int m_CircularBufferWritePosition;

        private byte[] m_PcmDataBuffer;

        private long m_PlaybackStartPosition;
        private long m_PlaybackEndPosition;

        private readonly Object LOCK_DEVICES = new object();
        public void ClearDeviceCache()
        {
            lock (LOCK_DEVICES)
            {
                m_CachedOutputDevices = null;
            }
        }

        public void SetOutputDevice(Control handle, string name)
        {
            lock (LOCK_DEVICES)
            {
                if (m_CachedOutputDevices != null
                    && OutputDevice != null && OutputDevice.Name == name && !OutputDevice.Device.Disposed)
                {
                    return;
                }

                if (m_CachedOutputDevices != null)
                {
                    OutputDevice foundCached =
                        m_CachedOutputDevices.Find(delegate(OutputDevice d) { return d.Name == name; });
                    if (foundCached != null && !foundCached.Device.Disposed)
                    {
                        SetOutputDevice(handle, foundCached);
                        return;
                    }
                }
            }

            List<OutputDevice> devices = OutputDevices;
            OutputDevice found = null;
            lock (LOCK_DEVICES)
            {
                found = devices.Find(delegate(OutputDevice d) { return d.Name == name; });
            }
            if (found != null)
            {
                SetOutputDevice(handle, found);
            }
            else if (devices.Count > 0)
            {
                SetOutputDevice(handle, devices[0]);
            }
            else
            {
                CurrentState = State.NotReady;
                throw new Exception("No output device available.");
            }
        }

        private List<OutputDevice> m_CachedOutputDevices;
        public List<OutputDevice> OutputDevices
        {
            get
            {
                lock (LOCK_DEVICES)
                {
#if USE_SLIMDX
                DeviceCollection devices = DirectSound.GetDevices();
                    // new DeviceCollection();
#else
                    DevicesCollection devices = new DevicesCollection();
#endif
                    List<OutputDevice> outputDevices = new List<OutputDevice>(devices.Count);
                    foreach (DeviceInformation info in devices)
                    {
                        Console.WriteLine(info.ModuleName);
                        Console.WriteLine(info.Description);
                        Console.WriteLine(info.DriverGuid);
                        try
                        {
                            outputDevices.Add(new OutputDevice(info));
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                            Console.WriteLine(ex.StackTrace);
                            continue;
                        }
                    }
                    m_CachedOutputDevices = outputDevices;
                    return outputDevices;
                }
            }
        }

        private void SetOutputDevice(Control handle, OutputDevice device)
        {
            if (handle != null)
            {
#if USE_SLIMDX
                device.Device.SetCooperativeLevel(handle.Handle, CooperativeLevel.Priority);
#else
                device.Device.SetCooperativeLevel(handle, CooperativeLevel.Priority);
#endif
            }

            OutputDevice = device;
        }

        private OutputDevice m_OutputDevice;
        public OutputDevice OutputDevice
        {
            get
            {
                return m_OutputDevice;
            }
            private set
            {
                m_OutputDevice = value;

                CurrentState = State.Stopped;
            }
        }


        private AudioLibPCMFormat m_CurrentAudioPCMFormat;
        public AudioLibPCMFormat CurrentAudioPCMFormat
        {
            get { return m_CurrentAudioPCMFormat; }
        }

        public void PlayBytes(StreamProviderDelegate currentAudioStreamProvider,
                            long dataLength, AudioLibPCMFormat pcmInfo,
                            long bytesFrom, long bytesTo)
        {
            if (pcmInfo == null)
            {
                throw new ArgumentNullException("PCM format cannot be null !");
            }

            if (currentAudioStreamProvider == null)
            {
                throw new ArgumentNullException("Stream cannot be null !");
            }
            if (dataLength <= 0)
            {
                throw new ArgumentOutOfRangeException("Duration cannot be <= 0 !");
            }

            if (CurrentState == State.NotReady)
            {
                return;
            }

            if (CurrentState != State.Stopped)
            {
                Debug.Fail("Attempting to play when not stopped ? " + CurrentState);
                return;
            }

            m_CurrentAudioStreamProvider = currentAudioStreamProvider;
            m_CurrentAudioStream = m_CurrentAudioStreamProvider();
            m_CurrentAudioPCMFormat = pcmInfo;
            m_CurrentAudioDataLength = dataLength;



            long startPosition = 0;
            if (bytesFrom > 0)
            {
                startPosition = m_CurrentAudioPCMFormat.AdjustByteToBlockAlignFrameSize(bytesFrom);
            }

            long endPosition = 0;
            if (bytesTo > 0)
            {
                endPosition = m_CurrentAudioPCMFormat.AdjustByteToBlockAlignFrameSize(bytesTo);
            }

            if (m_CurrentAudioPCMFormat.BytesAreEqualWithOneMillisecondTolerance(startPosition, 0))
            {
                startPosition = 0;
            }

            if (m_CurrentAudioPCMFormat.BytesAreEqualWithOneMillisecondTolerance(endPosition, dataLength))
            {
                endPosition = dataLength;
            }

            if (m_CurrentAudioPCMFormat.BytesAreEqualWithOneMillisecondTolerance(endPosition, 0))
            {
                endPosition = 0;
            }

            if (endPosition != 0
                && m_CurrentAudioPCMFormat.BytesAreEqualWithOneMillisecondTolerance(endPosition, startPosition))
            {
                return;
            }

            if (startPosition >= 0 &&
                (endPosition == 0 || startPosition < endPosition) &&
                endPosition <= dataLength)
            {
                startPlayback(startPosition, endPosition);
            }
            else
            {
                throw new Exception("Start/end positions out of bounds of audio asset.");
            }
        }

        //public void PlayTime(StreamProviderDelegate currentAudioStreamProvider,
        //                    double duration, AudioLibPCMFormat pcmInfo,
        //                    double from, double to)
        //{
        //    if (pcmInfo == null)
        //    {
        //        throw new ArgumentNullException("PCM format cannot be null !");
        //    }

        //    Play(currentAudioStreamProvider, duration, pcmInfo, pcmInfo.ConvertTimeToBytes(from), pcmInfo.ConvertTimeToBytes(to));
        //}

#if PAUSE_FEATURE_ENABLED
        private long m_ResumeStartPosition;

        public void Pause(long bytePos)
        {
            if (CurrentState == State.NotReady)
            {
                return;
            }

            if (CurrentState == State.Playing)
            {
                stopPlayback();
            }

            m_ResumeStartPosition = bytePos;

            CurrentState = State.Paused;
        }

        public void Pause()
        {
            if (CurrentState == State.NotReady)
            {
                return;
            }

            if (CurrentState != State.Playing)
            {
                return;
            }

            m_ResumeStartPosition = getCurrentBytePosition();

            CurrentState = State.Paused;

            stopPlayback();
        }

        public void Resume()
        {
            if (CurrentState == State.NotReady)
            {
                return;
            }

            if (CurrentState != State.Paused)
            {
                return;
            }

            startPlayback(m_ResumeStartPosition, m_PlaybackEndPosition);
        }
#endif //PAUSE_FEATURE_ENABLED

        public void Stop()
        {
            if (CurrentState == State.NotReady)
            {
                return;
            }

            if (CurrentState == State.Stopped)
            {
                return;
            }
            CurrentState = State.Stopped;
            stopPlayback();
        }

        public long CurrentBytePosition
        {
            get
            {
                if (CurrentState == State.NotReady)
                {
                    return -1;
                }

                if (m_CurrentAudioPCMFormat == null)
                {
                    return 0; // Play was never called, or the stream was closed completely
                }

                return m_CurrentAudioPCMFormat.AdjustByteToBlockAlignFrameSize(getCurrentBytePosition());
            }
        }

        private long m_CurrentBytePosition = -1;
        private long getCurrentBytePosition()
        {
            if (m_CurrentAudioPCMFormat == null)
            {
                return 0;
            }

#if PAUSE_FEATURE_ENABLED
            if (CurrentState == State.Paused)
            {
                return m_ResumeStartPosition;
            }   
#endif //PAUSE_FEATURE_ENABLED

            if (CurrentState == State.Stopped)
            {
                return 0;
            }

            if (CurrentState == State.Playing)
            {
                if (m_CurrentBytePosition < 0)
                {
                    //Console.WriteLine(String.Format("????? m_CurrentBytePosition < 0 [{0}]", m_CurrentBytePosition));
                }

                return m_CurrentBytePosition; //refreshed in the loop thread, so depends on the refresh rate.
            }

            return 0;
        }

        private const int REFRESH_INTERVAL_MS = 50; //ms interval for refreshing PCM data
        public int RefreshInterval
        {
            get { return REFRESH_INTERVAL_MS; }
        }

        private void initializeBuffers()
        {
            // example: 44.1 kHz (44,100 samples per second) * 16 bits per sample / 8 bits per byte * 2 channels (stereo)
            // blockAlign is number of bytes per frame (samples required for all channels)
            uint byteRate = m_CurrentAudioPCMFormat.SampleRate * m_CurrentAudioPCMFormat.BlockAlign;

            int pcmDataBufferSize = (int)(byteRate * REFRESH_INTERVAL_MS / 1000.0);
            pcmDataBufferSize -= pcmDataBufferSize % m_CurrentAudioPCMFormat.BlockAlign;
            m_PcmDataBuffer = new byte[pcmDataBufferSize];

            int dxBufferSize = (int)(byteRate * 0.500); // 500ms
            dxBufferSize -= dxBufferSize % m_CurrentAudioPCMFormat.BlockAlign;

            m_CircularBufferRefreshChunkSize = (int)(byteRate * 0.200); //200ms
            m_CircularBufferRefreshChunkSize -= m_CircularBufferRefreshChunkSize % m_CurrentAudioPCMFormat.BlockAlign;


            WaveFormat waveFormat = new WaveFormat();
            waveFormat.FormatTag = WaveFormatTag.Pcm;

            waveFormat.Channels = (short)m_CurrentAudioPCMFormat.NumberOfChannels;
            waveFormat.SamplesPerSecond = (int)m_CurrentAudioPCMFormat.SampleRate;
            waveFormat.BitsPerSample = (short)m_CurrentAudioPCMFormat.BitDepth;
            waveFormat.AverageBytesPerSecond = (int)byteRate;
#if USE_SLIMDX
            waveFormat.BlockAlignment = (short)m_CurrentAudioPCMFormat.BlockAlign;
#else
            waveFormat.BlockAlign = (short)m_CurrentAudioPCMFormat.BlockAlign;
#endif

#if USE_SLIMDX
            SoundBufferDescription bufferDescription = new SoundBufferDescription();
            bufferDescription.Flags = BufferFlags.ControlVolume | BufferFlags.ControlFrequency | BufferFlags.GlobalFocus;
            bufferDescription.SizeInBytes = dxBufferSize;
#else
            BufferDescription bufferDescription = new BufferDescription();

            bufferDescription.ControlVolume = true;
            bufferDescription.ControlFrequency = true;
            bufferDescription.GlobalFocus = true;
            bufferDescription.BufferBytes = dxBufferSize;
#endif
            bufferDescription.Format = waveFormat;

#if USE_SLIMDX
            m_CircularBuffer = new SecondarySoundBuffer(OutputDevice.Device, bufferDescription);
#else
            m_CircularBuffer = new SecondaryBuffer(bufferDescription, OutputDevice.Device);
#endif

            FastPlayFactor = FastPlayFactor; // reset
        }

#if USE_SLIMDX
        private byte[] SlimDX_IntermediaryTransferBuffer;
#endif

        private void startPlayback(long startPosition, long endPosition)
        {
            initializeBuffers();

            m_PlaybackStartPosition = startPosition;
            m_PlaybackEndPosition = endPosition == 0
                ? m_CurrentAudioDataLength
                : endPosition;

            m_CircularBufferWritePosition = 0;

            //m_CircularBufferFlushTolerance = -1;
            m_PredictedByteIncrement = -1;

            m_PreviousCircularBufferPlayPosition = -1;
            m_TotalBytesPlayed = -1;

            m_CurrentAudioStream = m_CurrentAudioStreamProvider();
            m_CurrentAudioStream.Position = m_PlaybackStartPosition;

            int sizeBytes =
#if USE_SLIMDX
 m_CircularBuffer.Capabilities.BufferSize
#else
 m_CircularBuffer.Caps.BufferBytes
#endif
;
            long length = m_PlaybackEndPosition - m_PlaybackStartPosition;
            length -= length % m_CurrentAudioPCMFormat.BlockAlign;
            int initialFullBufferBytes = (int)Math.Min(length, sizeBytes);

#if USE_SLIMDX
            if (SlimDX_IntermediaryTransferBuffer == null)
            {
                SlimDX_IntermediaryTransferBuffer = new byte[initialFullBufferBytes];
            }
            else if (SlimDX_IntermediaryTransferBuffer.Length != initialFullBufferBytes)
            {
                Array.Resize(ref SlimDX_IntermediaryTransferBuffer, initialFullBufferBytes);
            }
            int read = m_CurrentAudioStream.Read(SlimDX_IntermediaryTransferBuffer, 0, initialFullBufferBytes);
            DebugFix.Assert(initialFullBufferBytes == read);
            m_CircularBuffer.Write(SlimDX_IntermediaryTransferBuffer, 0, initialFullBufferBytes, m_CircularBufferWritePosition, LockFlags.None);
#else
            m_CircularBuffer.Write(0, m_CurrentAudioStream, initialFullBufferBytes, LockFlag.None);
#endif
            m_CircularBufferWritePosition += initialFullBufferBytes;

            m_CircularBufferWritePosition %= sizeBytes;
            //if (m_CircularBufferWritePosition >= m_CircularBuffer.Caps.BufferBytes) m_CircularBufferWritePosition -= m_CircularBuffer.Caps.BufferBytes;

            m_CurrentBytePosition = m_PlaybackStartPosition;

            CurrentState = State.Playing;
            if (AllowBackToBackPlayback && m_MonitoringTimer != null) m_MonitoringTimer.Start();
            try
            {
                m_CircularBuffer.Play(0,
#if USE_SLIMDX
 PlayFlags.Looping
#else
 BufferPlayFlags.Looping
#endif
);
            }
            catch (Exception)
            {
                Debug.Fail("EmergencyStopForSoundBufferProblem !");

                CurrentState = State.Stopped;

                stopPlayback();

                return;
            }

            ThreadStart threadDelegate = delegate()
            {
                try
                {
                    circularBufferRefreshThreadMethod();
                }
                catch (ThreadAbortException ex)
                {
                    //
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.StackTrace);
                }
                finally
                {
                    if ( CurrentState != State.Paused )  CurrentState = State.Stopped;

                    lock (LOCK)
                    {
                        m_CircularBufferRefreshThread = null;
                    }

                    stopPlayback();
                }

                //Console.WriteLine("Player refresh thread exiting....");

                //CurrentState = State.Stopped;

                //lock (LOCK)
                //{
                //    //m_CircularBufferRefreshThreadIsAlive = false;
                //    m_CircularBufferRefreshThread = null;
                //}

                //Console.WriteLine("Player refresh thread exit.");
            };
            DebugFix.Assert(m_CircularBufferRefreshThread == null);
            lock (LOCK)
            {
                m_CircularBufferRefreshThread = new Thread(threadDelegate);
                m_CircularBufferRefreshThread.Name = "Player Refresh Thread";
                m_CircularBufferRefreshThread.Priority = ThreadPriority.Normal;
                m_CircularBufferRefreshThread.IsBackground = true;
                m_CircularBufferRefreshThread.Start();
            }


            //Console.WriteLine("Player refresh thread start.");
        }

        //private long m_CircularBufferFlushTolerance;
        //private int m_CircularBufferPreviousBytesAvailableForWriting;
        private long m_PredictedByteIncrement;

        private int m_PreviousCircularBufferPlayPosition;
        private int m_TotalBytesPlayed;

        private void circularBufferRefreshThreadMethod()
        {
            //m_CircularBufferRefreshThreadIsAlive = true;

            int previousCircularBufferFrequence = m_CircularBuffer.Frequency;

            while (true)
            {
                Thread.Sleep(REFRESH_INTERVAL_MS);

#if USE_SLIMDX
                if (m_CircularBuffer.Status == BufferStatus.BufferLost)
                {
                    m_CircularBuffer.Restore();
                }
#else
                if (m_CircularBuffer.Status.BufferLost)
                {
                    m_CircularBuffer.Restore();
                }
#endif

#if USE_SLIMDX
                if (m_CircularBuffer.Status == BufferStatus.BufferTerminated)
                {
                    return;
                }
#else
                if (m_CircularBuffer.Status.Terminated
                    || !m_CircularBuffer.Status.Playing
                    || !m_CircularBuffer.Status.Looping)
                {
                    return;
                }
#endif
                if (m_PredictedByteIncrement < 0
                    || m_CircularBuffer.Frequency != previousCircularBufferFrequence)
                {
                    previousCircularBufferFrequence = m_CircularBuffer.Frequency;

                    int byteRate = m_CurrentAudioPCMFormat.BlockAlign * m_CircularBuffer.Frequency; // m_CurrentAudioPCMFormat.SampleRate;
                    m_PredictedByteIncrement = (long)(byteRate * (REFRESH_INTERVAL_MS + 15) / 1000.0); // TODO: determine time experied since last iteration by comparing DateTime.Now.Ticks or a more efficient system timer
                    m_PredictedByteIncrement -= m_PredictedByteIncrement % m_CurrentAudioPCMFormat.BlockAlign;
                }

                long pcmDataAvailableFromStream = m_PlaybackEndPosition - m_CurrentAudioStream.Position;

                long pcmDataTotalPlayableFromStream = m_PlaybackEndPosition - m_PlaybackStartPosition;

                long pcmDataAlreadyReadFromStream = pcmDataTotalPlayableFromStream - pcmDataAvailableFromStream;
#if USE_SLIMDX
                int circularBufferPlayPosition = m_CircularBuffer.CurrentPlayPosition;
#else
                int circularBufferPlayPosition = m_CircularBuffer.PlayPosition;
#endif
                int sizeBytes =
#if USE_SLIMDX
 m_CircularBuffer.Capabilities.BufferSize
#else
 m_CircularBuffer.Caps.BufferBytes
#endif
;
                if (m_TotalBytesPlayed < 0)
                {
                    m_TotalBytesPlayed = circularBufferPlayPosition;
                }
                else
                {
                    if (circularBufferPlayPosition >= m_PreviousCircularBufferPlayPosition)
                    {
                        m_TotalBytesPlayed += circularBufferPlayPosition - m_PreviousCircularBufferPlayPosition;
                    }
                    else
                    {
                        m_TotalBytesPlayed += (sizeBytes - m_PreviousCircularBufferPlayPosition) + circularBufferPlayPosition;
                    }
                }

                m_PreviousCircularBufferPlayPosition = circularBufferPlayPosition;

                long remainingBytesToPlay = pcmDataTotalPlayableFromStream - m_TotalBytesPlayed;

                int circularBufferBytesAvailableForWriting = (circularBufferPlayPosition == m_CircularBufferWritePosition ? 0
                                        : (circularBufferPlayPosition < m_CircularBufferWritePosition
                                  ? circularBufferPlayPosition + (sizeBytes - m_CircularBufferWritePosition)
                                  : circularBufferPlayPosition - m_CircularBufferWritePosition));

                int circularBufferBytesAvailableForPlaying = sizeBytes - circularBufferBytesAvailableForWriting;

                //realTimePlaybackPosition -= realTimePlaybackPosition % m_CurrentAudioPCMFormat.BlockAlign;

                //Console.WriteLine(String.Format("bytesAvailableForWriting: [{0} / {1}]", bytesAvailableForWriting, m_CircularBuffer.Caps.BufferBytes));

                //Console.WriteLine("dataAvailableFromStream: " + dataAvailableFromStream);


                long realTimePlaybackPosition = Math.Max(m_PlaybackStartPosition,
                    m_CurrentAudioStream.Position - Math.Min(circularBufferBytesAvailableForPlaying, remainingBytesToPlay));

                realTimePlaybackPosition = Math.Min(realTimePlaybackPosition, m_PlaybackStartPosition + m_TotalBytesPlayed);

                if (m_CurrentBytePosition == m_PlaybackStartPosition)
                {
                    //Console.WriteLine(string.Format("m_CurrentBytePosition ASSIGNED: realTimePlaybackPosition [{0}]", realTimePlaybackPosition));

                    m_CurrentBytePosition += m_TotalBytesPlayed;
                }
                else if (realTimePlaybackPosition < m_CurrentBytePosition)
                {
                    //Console.WriteLine(string.Format("realTimePlaybackPosition [{0}] < m_CurrentBytePosition [{1}]", realTimePlaybackPosition, m_CurrentBytePosition));

                    m_CurrentBytePosition = Math.Min(m_PlaybackEndPosition, m_CurrentBytePosition + m_PredictedByteIncrement);
                }
                else if (realTimePlaybackPosition > m_CurrentBytePosition + m_PredictedByteIncrement)
                {
                    //Console.WriteLine(string.Format("realTimePlaybackPosition [{0}] > m_CurrentBytePosition [{1}] + m_PredictedByteIncrement: [{2}] (diff: [{3}])",
                    //    realTimePlaybackPosition, m_CurrentBytePosition, m_PredictedByteIncrement, realTimePlaybackPosition - m_CurrentBytePosition));

                    m_CurrentBytePosition = Math.Min(m_PlaybackEndPosition, m_CurrentBytePosition + m_PredictedByteIncrement);
                }
                else
                {
                    //Console.WriteLine(string.Format("m_CurrentBytePosition OK: realTimePlaybackPosition [{0}]", realTimePlaybackPosition));

                    m_CurrentBytePosition = m_PlaybackStartPosition + m_TotalBytesPlayed;
                }


                PcmDataBufferAvailableHandler del = PcmDataBufferAvailable;
                if (del != null
                    && m_PcmDataBuffer.Length <= circularBufferBytesAvailableForPlaying)
                {
#if USE_SLIMDX
                    if (SlimDX_IntermediaryTransferBuffer != null)
                    {
                        Array.Copy(SlimDX_IntermediaryTransferBuffer, m_PcmDataBuffer, Math.Min(m_PcmDataBuffer.Length, SlimDX_IntermediaryTransferBuffer.Length));
                        m_PcmDataBufferAvailableEventArgs.PcmDataBuffer = m_PcmDataBuffer;
                        del  (this, m_PcmDataBufferAvailableEventArgs);
                    }
#else
                    Array array = m_CircularBuffer.Read(circularBufferPlayPosition, typeof(byte), LockFlag.None, m_PcmDataBuffer.Length);
                    Array.Copy(array, m_PcmDataBuffer, m_PcmDataBuffer.Length);
                    m_PcmDataBufferAvailableEventArgs.PcmDataBuffer = m_PcmDataBuffer;
                    del (this, m_PcmDataBufferAvailableEventArgs);
#endif
                }
                //var del_ = PcmDataBufferAvailable;
                //if (del_ != null
                    //&& m_PcmDataBuffer.Length <= circularBufferBytesAvailableForPlaying)
                //{
//#if USE_SLIMDX
                    //if (SlimDX_IntermediaryTransferBuffer != null)
                    //{
                        //Array.Copy(SlimDX_IntermediaryTransferBuffer, m_PcmDataBuffer, Math.Min(m_PcmDataBuffer.Length, SlimDX_IntermediaryTransferBuffer.Length));
                        //m_PcmDataBufferAvailableEventArgs.PcmDataBuffer = m_PcmDataBuffer;
                        //PcmDataBufferAvailable(this, m_PcmDataBufferAvailableEventArgs);
                    //}
//#else
                    //Array array = m_CircularBuffer.Read(circularBufferPlayPosition, typeof(byte), LockFlag.None, m_PcmDataBuffer.Length);
                    //Array.Copy(array, m_PcmDataBuffer, m_PcmDataBuffer.Length);
                    //m_PcmDataBufferAvailableEventArgs.PcmDataBuffer = m_PcmDataBuffer;
                    //del_(this, m_PcmDataBufferAvailableEventArgs);
//#endif
                //}

                if (circularBufferBytesAvailableForWriting <= 0)
                {
                    if (pcmDataAvailableFromStream > 0)
                    {
                        //Console.WriteLine("circularBufferBytesAvailableForWriting <= 0, pcmDataAvailableFromStream > 0 ... continue...");
                        continue;
                    }
                    else
                    {
                        //Console.WriteLine("circularBufferBytesAvailableForWriting <= 0, pcmDataAvailableFromStream <= 0 ... BREAK...");
                        break;
                    }
                }

                //m_CircularBufferPreviousBytesAvailableForWriting = circularBufferBytesAvailableForWriting;

                if (pcmDataAvailableFromStream <= 0)
                {
                    if (remainingBytesToPlay > m_PredictedByteIncrement)
                    {
                        //Console.WriteLine(string.Format("remainingBytesToPlay [{0}]", remainingBytesToPlay));
                        continue;
                    }
                    else
                    {
                        m_CircularBuffer.Stop(); // the earlier the better ?

                        //Console.WriteLine("Time to break, all bytes gone.");
                        break;
                    }
                    //if (m_CircularBufferFlushTolerance < 0)
                    //{
                    //    m_CircularBufferFlushTolerance = m_CurrentAudioPCMFormat.ConvertTimeToBytes(REFRESH_INTERVAL_MS*1.5);
                    //}

                    //Console.WriteLine(string.Format("pcmDataTotalPlayableFromStream [{0}]", pcmDataTotalPlayableFromStream));

                    //Console.WriteLine(String.Format("pcmDataAvailableFromStream <= 0 // circularBufferBytesAvailableForWriting [{0}], m_CircularBufferFlushTolerance [{1}], m_CircularBuffer.Caps.BufferBytes [{2}], m_CircularBufferPreviousBytesAvailableForWriting [{3}]",
                    //    circularBufferBytesAvailableForWriting, m_CircularBufferFlushTolerance, m_CircularBuffer.Caps.BufferBytes, m_CircularBufferPreviousBytesAvailableForWriting));

                    //if ((circularBufferBytesAvailableForWriting + m_CircularBufferFlushTolerance) >= m_CircularBuffer.Caps.BufferBytes
                    //    || m_CircularBufferPreviousBytesAvailableForWriting > circularBufferBytesAvailableForWriting)
                    //{
                    //    m_CircularBuffer.Stop(); // the earlier the better ?

                    //    Console.WriteLine("Forcing closing-up.");

                    //    circularBufferBytesAvailableForWriting = 0; // will enter the IF test below
                    //}
                }
                else
                {
                    long bytesToTransferToCircularBuffer = Math.Min(circularBufferBytesAvailableForWriting, m_CircularBufferRefreshChunkSize);
                    bytesToTransferToCircularBuffer = Math.Min(bytesToTransferToCircularBuffer, pcmDataAvailableFromStream);
                    bytesToTransferToCircularBuffer -= bytesToTransferToCircularBuffer % m_CurrentAudioPCMFormat.BlockAlign;

                    if (bytesToTransferToCircularBuffer <= 0)
                    {
                        Debug.Fail("bytesToTransferToCircularBuffer <= 0 !!");
                        continue;
                    }

                    //Console.WriteLine(String.Format("m_CircularBufferWritePosition: [{0} / toCopy {1}]", m_CircularBufferWritePosition, toCopy));

                    DebugFix.Assert(m_CurrentAudioStream.Position + bytesToTransferToCircularBuffer <= m_PlaybackEndPosition);

#if USE_SLIMDX
                    if (SlimDX_IntermediaryTransferBuffer == null)
                    {
                        SlimDX_IntermediaryTransferBuffer = new byte[bytesToTransferToCircularBuffer];
                    }
                    else if (SlimDX_IntermediaryTransferBuffer.Length != bytesToTransferToCircularBuffer)
                    {
                        Array.Resize(ref SlimDX_IntermediaryTransferBuffer, (int)bytesToTransferToCircularBuffer);
                    }
                    int read = m_CurrentAudioStream.Read(SlimDX_IntermediaryTransferBuffer, 0, (int)bytesToTransferToCircularBuffer);
                    DebugFix.Assert(bytesToTransferToCircularBuffer == read);
                    m_CircularBuffer.Write(SlimDX_IntermediaryTransferBuffer, 0, (int)bytesToTransferToCircularBuffer, m_CircularBufferWritePosition, LockFlags.None);
#else
                    m_CircularBuffer.Write(m_CircularBufferWritePosition, m_CurrentAudioStream, (int)bytesToTransferToCircularBuffer, LockFlag.None);
#endif

                    //int afterWriteCursor = m_CircularBuffer.Caps.BufferBytes - m_CircularBufferWritePosition;
                    //if (toCopy <= afterWriteCursor)
                    //{
                    //    m_CircularBuffer.Write(m_CircularBufferWritePosition, m_CurrentAudioStream, toCopy, LockFlag.None);
                    //}
                    //else
                    //{
                    //    m_CircularBuffer.Write(m_CircularBufferWritePosition, m_CurrentAudioStream, afterWriteCursor, LockFlag.None);
                    //    m_CircularBuffer.Write(0, m_CurrentAudioStream, toCopy - afterWriteCursor, LockFlag.None);
                    //}

                    m_CircularBufferWritePosition += (int)bytesToTransferToCircularBuffer;
                    m_CircularBufferWritePosition %= sizeBytes;
                    //if (m_CircularBufferWritePosition >= m_CircularBuffer.Caps.BufferBytes) m_CircularBufferWritePosition -= m_CircularBuffer.Caps.BufferBytes;
                }
            }

            CurrentState = State.Stopped;

            if (!m_AllowBackToBackPlayback)
            {
                AudioPlaybackFinishHandler delFinished = AudioPlaybackFinished;
                if (delFinished != null && mEventsEnabled)
                    delFinished(this, new AudioPlaybackFinishEventArgs());
            }
            else
            {
                m_FinishedPlayingCurrentStream = true;
            }
            //var del = AudioPlaybackFinished;
            //if (del != null)
                //del(this, new AudioPlaybackFinishEventArgs());
        }

        private bool m_FinishedPlayingCurrentStream = false;
        private void m_MonitoringTimer_Tick(object sender, EventArgs e)
        {
            Console.WriteLine("monitoring ");
            if (AllowBackToBackPlayback &&  m_FinishedPlayingCurrentStream)
            {
                if (m_MonitoringTimer != null) m_MonitoringTimer.Stop();
                m_FinishedPlayingCurrentStream = false;
                AudioPlaybackFinishHandler delFinished = AudioPlaybackFinished;
                if (delFinished != null && mEventsEnabled)
                    delFinished(this, new AudioPlaybackFinishEventArgs());
                
            }
        }


        private void stopPlayback()
        {
            PcmDataBufferAvailableHandler del = PcmDataBufferAvailable;
            if (del != null)
            {
                for (int i = 0 ; i < m_PcmDataBuffer.Length; i++)
                {
                    m_PcmDataBuffer[i] = 0;
                }
                m_PcmDataBufferAvailableEventArgs.PcmDataBuffer = m_PcmDataBuffer;
                del(this, m_PcmDataBufferAvailableEventArgs);
            }


            m_CircularBuffer.Stop();

            //lock (LOCK)
            //{
            //    if (m_CircularBufferRefreshThread != null)
            //    {
            //        m_CircularBufferRefreshThread.Abort();
            //    }
            //}
            int count = 0;
            while (m_CircularBufferRefreshThread != null
                //&& (m_CircularBufferRefreshThread.IsAlive
                //// NO NEED FOR AN EXTRA CHECK, AS THE THREAD POINTER IS RESET TO NULL
                ////|| m_CircularBufferRefreshThreadIsAlive
                //)
                )
            {
                //if (count % 5 == 0)
                //{
                //    Console.WriteLine(@"///// PLAYER m_CircularBufferRefreshThread.Abort(): " + count++);
                //    lock (LOCK)
                //    {
                //        if (m_CircularBufferRefreshThread != null)
                //        {
                //            m_CircularBufferRefreshThread.Abort();
                //        }
                //    }
                //}
                Console.WriteLine(@"///// PLAYER m_CircularBufferRefreshThread != null: " + count++);
                Thread.Sleep(20);

                if (count > 15)
                {
                    //CurrentState = State.Stopped;

                    lock (LOCK)
                    {
                        if (m_CircularBufferRefreshThread != null)
                        {
                            m_CircularBufferRefreshThread.Join(100);
                        }
                        m_CircularBufferRefreshThread = null;
                    }
                    break;
                }
            }
            if (!m_KeepStreamAlive)
            {
                EnsurePlaybackStreamIsDead();
            }
        }

        System.Windows.Forms.Timer mPreviewTimer = new System.Windows.Forms.Timer () ; // timer for playing chunks at interval during Forward/Rewind 

        private int mFwdRwdRate; // holds skip time multiplier for forward / rewind mode , value is 0 for normal playback,  positive  for FastForward and negetive  for Rewind

        /// <summary>
        /// Forward / Rewind rate.
        /// 0 for normal playback
        /// negative integer for Rewind
        /// positive integer for FastForward
        /// </summary>
        public int PlaybackFwdRwdRate
        {
            get { return mFwdRwdRate; }
            set { SetPlaybackMode(value); }
        }


        /// <summary>
        /// Set a new playback mode i.e. one of Normal, FastForward, Rewind 
        /// </summary>
        /// <param name="mode">The new mode.</param>
        private void SetPlaybackMode(int rate)
        {
            if (rate != mFwdRwdRate)
            {
                if (CurrentState == State.Playing )
                {
                    long restartPos = CurrentBytePosition;
                    stopPlayback();
                    m_State = State.Paused;
                    mFwdRwdRate = rate;

                    //InitPlay(mCurrentAudio, restartPos, 0);
                    //startPlayback(restartPos, m_CurrentAudioStream.Length );
                    if (mFwdRwdRate > 0)
                    {
                        FastForward(restartPos);
                    }
                    else if (mFwdRwdRate < 0)
                    {
                        if (restartPos == 0) restartPos = m_CurrentAudioStream.Length;
                        Rewind(restartPos);
                    }
                }
                else if (CurrentState == State.Paused || CurrentState == State.Stopped)
                {
                    mFwdRwdRate = rate;
                }
            }
        }
        private bool mEventsEnabled = true;
        private long m_lChunkStartPosition = 0; // position for starting chunk play in forward/Rewind
        private bool mIsFwdRwd ;                // flag indicating forward or rewind playback is going on

        //  FastForward , Rewind playback modes
        /// <summary>
        ///  Starts playing small chunks of audio while jumping backward in audio asset
        /// <see cref=""/>
        /// </summary>
        /// <param name="lStartPosition"></param>
        private void Rewind(long lStartPosition)
        {
            // let's play backward!
            if (mFwdRwdRate != 0)
            {
                CurrentState = State.Playing;
                m_lChunkStartPosition = lStartPosition;
                mEventsEnabled = false;
                mIsFwdRwd = true;
                mPreviewTimer.Interval = 50;
                mPreviewTimer.Start();

            }
        }


        /// <summary>
        ///  Starts playing small chunks while jumping forward in audio asset
        /// <see cref=""/>
        /// </summary>
        /// <param name="lStartPosition"></param>
        private void FastForward(long lStartPosition)
        {

            // let's play forward!
            if (mFwdRwdRate != 0)
            {
                CurrentState = State.Playing;
                m_lChunkStartPosition = lStartPosition;
                mEventsEnabled = false;
                mIsFwdRwd = true;
                mPreviewTimer.Interval = 50;
                mPreviewTimer.Start();
            }
        }



        ///Preview timer tick function
        private void PreviewTimer_Tick(object sender, EventArgs e)
        { //1

            double StepInMs = Math.Abs(4000 * mFwdRwdRate);
            //long lStepInBytes = CalculationFunctions.ConvertTimeToByte(StepInMs, (int)mCurrentAudio.getPCMFormat().getSampleRate(), mCurrentAudio.getPCMFormat().getBlockAlign());
            long lStepInBytes = m_CurrentAudioPCMFormat.ConvertTimeToBytes(Convert.ToInt64 (StepInMs*1000 ));
            int PlayChunkLength = 1200;
            //long lPlayChunkLength = CalculationFunctions.ConvertTimeToByte(PlayChunkLength, (int)mCurrentAudio.getPCMFormat().getSampleRate(), mCurrentAudio.getPCMFormat().getBlockAlign());
            long lPlayChunkLength = m_CurrentAudioPCMFormat.AdjustByteToBlockAlignFrameSize (PlayChunkLength) ;
            mPreviewTimer.Interval = PlayChunkLength + 50;

            long PlayStartPos = 0;
            long PlayEndPos = 0;
            if (mFwdRwdRate > 0)
            { //2
                //if ((mCurrentAudio.getPCMLength() - (lStepInBytes + m_lChunkStartPosition)) > lPlayChunkLength)
                if ((m_CurrentAudioStream.Length - (lStepInBytes + m_lChunkStartPosition)) > lPlayChunkLength)
                { //3
                    if (m_lChunkStartPosition > 0)
                    {
                        m_lChunkStartPosition += lStepInBytes;
                    }
                    else
                        //m_lChunkStartPosition = mFrameSize;
                        m_lChunkStartPosition = m_CurrentAudioPCMFormat.BlockAlign;

                    PlayStartPos = m_lChunkStartPosition;
                    PlayEndPos = m_lChunkStartPosition + lPlayChunkLength;
                    //PlayAssetStream(PlayStartPos, PlayEndPos);
                    startPlayback(PlayStartPos, PlayEndPos);

                    if (m_lChunkStartPosition > m_CurrentAudioStream.Length )
                        m_lChunkStartPosition = m_CurrentAudioStream.Length;
                } //-3
                else
                { //3
                    Stop();
                    AudioPlaybackFinishHandler delFinished = AudioPlaybackFinished;
                    if (mEventsEnabled
                        && AudioPlaybackFinished != null)
                    {   
                            delFinished(this, new AudioPlaybackFinishEventArgs());
                        //EndOfAudioAsset(this, new Events.Audio.Player.EndOfAudioAssetEventArgs());
                    }

                } //-3
            } //-2
            else if (mFwdRwdRate < 0)
            { //2
                //if (m_lChunkStartPosition > (lStepInBytes ) && lPlayChunkLength <= m_Asset.getPCMLength () )
                if (m_lChunkStartPosition > 0)
                { //3
                    if (m_lChunkStartPosition < m_CurrentAudioStream.Length)
                        m_lChunkStartPosition -= lStepInBytes;
                    else
                        m_lChunkStartPosition = m_CurrentAudioStream.Length- lPlayChunkLength;

                    PlayStartPos = m_lChunkStartPosition;
                    PlayEndPos = m_lChunkStartPosition + lPlayChunkLength;
                    //PlayAssetStream(PlayStartPos, PlayEndPos);
                    startPlayback(PlayStartPos, PlayEndPos);
                    if (m_lChunkStartPosition < 0)
                        m_lChunkStartPosition = 0;
                } //-3
                else
                {
                    Stop();
                    AudioPlaybackFinishHandler delFinished = AudioPlaybackFinished;
                    if (mEventsEnabled
                        && AudioPlaybackFinished != null)
                    {
                        delFinished(this, new AudioPlaybackFinishEventArgs());
                        //EndOfAudioAsset(this, new Events.Audio.Player.EndOfAudioAssetEventArgs());
                    }
                }
            } //-2
        } //-1


        /// <summary>
        /// Stop rewinding or forwarding, including the preview timer.
        /// </summary>
        private void StopForwardRewind()
        {
            if (mFwdRwdRate != 0 || mPreviewTimer.Enabled)
            {
                mPreviewTimer.Enabled = false;
                //m_FwdRwdRate = 0 ;
                m_lChunkStartPosition = 0;
                mIsFwdRwd = false;
                mEventsEnabled = true;
            }
        }



        public event AudioPlaybackFinishHandler AudioPlaybackFinished;
        public delegate void AudioPlaybackFinishHandler(object sender, AudioPlaybackFinishEventArgs e);
        public class AudioPlaybackFinishEventArgs : EventArgs
        {
        }

        public event StateChangedHandler StateChanged;
        public delegate void StateChangedHandler(object sender, StateChangedEventArgs e);
        public class StateChangedEventArgs : EventArgs
        {
            private State m_OldState;
            public State OldState
            {
                get
                {
                    return m_OldState;
                }
            }

            public StateChangedEventArgs(State oldState)
            {
                m_OldState = oldState;
            }
        }


        private readonly PcmDataBufferAvailableEventArgs m_PcmDataBufferAvailableEventArgs = new PcmDataBufferAvailableEventArgs(new byte[] { 0, 0, 0, 0 });
        public event PcmDataBufferAvailableHandler PcmDataBufferAvailable;
        public delegate void PcmDataBufferAvailableHandler(object sender, PcmDataBufferAvailableEventArgs e);
        public class PcmDataBufferAvailableEventArgs : EventArgs
        {
            private byte[] m_PcmDataBuffer;
            public byte[] PcmDataBuffer
            {
                get
                {
                    return m_PcmDataBuffer;
                }
                set
                {
                    m_PcmDataBuffer = value;
                }
            }

            public PcmDataBufferAvailableEventArgs(byte[] pcmDataBuffer)
            {
                m_PcmDataBuffer = pcmDataBuffer;
            }
        }
    }
}
