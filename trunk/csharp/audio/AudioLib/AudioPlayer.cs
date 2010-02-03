using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Threading;
using System.Collections.Generic;
using Microsoft.DirectX.DirectSound;

namespace AudioLib
{
    // This class underwent a major cleanup and simplification at revision 1488.
    // See:
    // http://daisy.trac.cvsdude.com/urakawa-sdk/changeset/1488#file0
    // Just in case we need to restore some functionality:
    // http://daisy.trac.cvsdude.com/urakawa-sdk/browser/trunk/csharp/audio/AudioLib/AudioPlayer.cs?rev=1487
    public class AudioPlayer
    {
        private readonly bool m_KeepStreamAlive;
        public AudioPlayer(bool keepStreamAlive)
        {
            m_KeepStreamAlive = keepStreamAlive;

            CurrentState = State.NotReady;
        }

        /// <summary>
        /// The four states of the audio player.
        /// NotReady: the player has no output device set yet.
        /// Playing: sound is currently playing.
        /// Paused: playback was paused and can be resumed.
        /// Stopped: player is idle.
        /// </summary>
        public enum State { NotReady, Stopped, Playing, Paused };

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

                if (StateChanged != null)
                    StateChanged(this, new StateChangedEventArgs(oldState));
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

                if (m_CircularBuffer != null && m_CircularBuffer.Caps.ControlFrequency)
                {
                    m_CircularBuffer.Frequency = (int)(m_CircularBuffer.Format.SamplesPerSecond * m_FastPlayFactor);
                }
            }
        }

        public delegate Stream StreamProviderDelegate();
        private StreamProviderDelegate m_CurrentAudioStreamProvider;
        private Stream m_CurrentAudioStream;
        private double m_CurrentAudioDuration;

        public void EnsurePlaybackStreamIsDead()
        {
            if (CurrentState == State.NotReady)
            {
                return;
            }

            if (m_CurrentAudioStream != null)
            {
                m_CurrentAudioStream.Close();
                m_CurrentAudioStream = null;
            }

            m_CurrentAudioPCMFormat = null;
            m_CurrentAudioDuration = 0;
            m_CurrentAudioStreamProvider = null;
        }

        private Thread m_CircularBufferRefreshThread;

        private SecondaryBuffer m_CircularBuffer;
        private int m_CircularBufferRefreshChunkSize;
        private int m_CircularBufferWritePosition;

        private byte[] m_PcmDataBuffer;

        private long m_PlaybackStartPosition;
        private long m_PlaybackEndPosition;

        private long m_ResumeStartPosition;

        public void SetOutputDevice(Control handle, OutputDevice device)
        {
            if (handle != null)
            {
                device.Device.SetCooperativeLevel(handle, CooperativeLevel.Priority);
            }

            OutputDevice = device;
        }

        public void SetOutputDevice(Control handle, string name)
        {
            List<OutputDevice> devices = OutputDevices;
            OutputDevice found = devices.Find(delegate(OutputDevice d) { return d.Name == name; });
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

        private List<OutputDevice> m_OutputDevices;
        public List<OutputDevice> OutputDevices
        {
            get
            {
                if (m_OutputDevices != null)
                {
                    return m_OutputDevices;
                }

                DevicesCollection devices = new DevicesCollection();
                m_OutputDevices = new List<OutputDevice>(devices.Count);
                foreach (DeviceInformation info in devices)
                {
                    m_OutputDevices.Add(new OutputDevice(info));
                }

                return m_OutputDevices;
            }
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


        private void Play(StreamProviderDelegate currentAudioStreamProvider,
                            double duration, AudioLibPCMFormat pcmInfo,
                            long from, long to)
        {
            if (pcmInfo == null)
            {
                throw new ArgumentNullException("PCM format cannot be null !");
            }

            if (currentAudioStreamProvider == null)
            {
                throw new ArgumentNullException("Stream cannot be null !");
            }
            if (duration <= 0)
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
            m_CurrentAudioDuration = duration;

            long startPosition = 0;
            if (from > 0)
            {
                startPosition = from;
                startPosition -= startPosition % m_CurrentAudioPCMFormat.BlockAlign;
            }

            long endPosition = 0;
            if (to > 0)
            {
                endPosition = to;
                endPosition -= endPosition % m_CurrentAudioPCMFormat.BlockAlign;
            }

            long max = pcmInfo.ConvertTimeToBytes(duration);

            if (startPosition >= 0 &&
                (endPosition == 0 || startPosition < endPosition) &&
                endPosition <= max)
            {
                startPlayback(startPosition, endPosition);
            }
            else
            {
                throw new Exception("Start/end positions out of bounds of audio asset.");
            }
        }

        public void PlayBytes(StreamProviderDelegate currentAudioStreamProvider,
                            long duration, AudioLibPCMFormat pcmInfo,
                            long from, long to)
        {
            if (pcmInfo == null)
            {
                throw new ArgumentNullException("PCM format cannot be null !");
            }

            Play(currentAudioStreamProvider, pcmInfo.ConvertBytesToTime(duration), pcmInfo, from, to);
        }

        public void PlayTime(StreamProviderDelegate currentAudioStreamProvider,
                            double duration, AudioLibPCMFormat pcmInfo,
                            double from, double to)
        {
            if (pcmInfo == null)
            {
                throw new ArgumentNullException("PCM format cannot be null !");
            }

            Play(currentAudioStreamProvider, duration, pcmInfo, pcmInfo.ConvertTimeToBytes(from), pcmInfo.ConvertTimeToBytes(to));
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

        public double CurrentTime
        {
            get
            {
                if (CurrentState == State.NotReady)
                {
                    return -1.0;
                }

                if (m_CurrentAudioPCMFormat == null)
                {
                    return 0; // Play was never called, or the stream was closed completely
                }

                return m_CurrentAudioPCMFormat.ConvertBytesToTime(getCurrentBytePosition());
            }
        }

        private long m_CurrentBytePosition = -1;
        private long getCurrentBytePosition()
        {
            if (m_CurrentAudioPCMFormat == null)
            {
                return 0;
            }

            if (CurrentState == State.Paused)
            {
                return m_ResumeStartPosition;
            }

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
            waveFormat.BlockAlign = (short)m_CurrentAudioPCMFormat.BlockAlign;

            BufferDescription bufferDescription = new BufferDescription();
            bufferDescription.BufferBytes = dxBufferSize;
            bufferDescription.Format = waveFormat;
            bufferDescription.ControlVolume = true;
            bufferDescription.ControlFrequency = true;
            bufferDescription.GlobalFocus = true;

            m_CircularBuffer = new SecondaryBuffer(bufferDescription, OutputDevice.Device);
        }


        private void startPlayback(long startPosition, long endPosition)
        {
            initializeBuffers();

            m_PlaybackStartPosition = startPosition;
            m_PlaybackEndPosition = endPosition == 0 ? m_CurrentAudioPCMFormat.ConvertTimeToBytes(m_CurrentAudioDuration) : endPosition;

            m_CircularBufferWritePosition = 0;

            //m_CircularBufferFlushTolerance = -1;
            m_PredictedByteIncrement = -1;

            m_PreviousCircularBufferPlayPosition = -1;
            m_TotalBytesPlayed = -1;

            m_CurrentAudioStream = m_CurrentAudioStreamProvider();
            m_CurrentAudioStream.Position = m_PlaybackStartPosition;

            long length = m_PlaybackEndPosition - m_PlaybackStartPosition;
            length -= length % m_CurrentAudioPCMFormat.BlockAlign;
            int initialFullBufferBytes = (int)Math.Min(length,
                                                        m_CircularBuffer.Caps.BufferBytes);

            m_CircularBuffer.Write(0, m_CurrentAudioStream, initialFullBufferBytes, LockFlag.None);
            m_CircularBufferWritePosition += initialFullBufferBytes;

            m_CircularBufferWritePosition %= m_CircularBuffer.Caps.BufferBytes;
            //if (m_CircularBufferWritePosition >= m_CircularBuffer.Caps.BufferBytes) m_CircularBufferWritePosition -= m_CircularBuffer.Caps.BufferBytes;

            m_CurrentBytePosition = m_PlaybackStartPosition;

            CurrentState = State.Playing;

            try
            {
                m_CircularBuffer.Play(0, BufferPlayFlags.Looping);
            }
            catch (Exception)
            {
                Debug.Fail("EmergencyStopForSoundBufferProblem !");

                CurrentState = State.Stopped;

                stopPlayback();

                return;
            }

            m_CircularBufferRefreshThread = new Thread(new ThreadStart(circularBufferRefreshThreadMethod));
            m_CircularBufferRefreshThread.Name = "Player Refresh Thread";
            m_CircularBufferRefreshThread.Priority = ThreadPriority.Highest;
            m_CircularBufferRefreshThread.IsBackground = true;
            m_CircularBufferRefreshThread.Start();


            //Console.WriteLine("Player refresh thread start.");
        }

        //private long m_CircularBufferFlushTolerance;
        //private int m_CircularBufferPreviousBytesAvailableForWriting;
        private long m_PredictedByteIncrement;

        private int m_PreviousCircularBufferPlayPosition;
        private int m_TotalBytesPlayed;

        private void circularBufferRefreshThreadMethod()
        {
            while (true)
            {
                Thread.Sleep(REFRESH_INTERVAL_MS);

                if (m_CircularBuffer.Status.BufferLost)
                {
                    m_CircularBuffer.Restore();
                }



                if (m_PredictedByteIncrement < 0)
                {
                    uint byteRate = m_CurrentAudioPCMFormat.SampleRate * m_CurrentAudioPCMFormat.BlockAlign;
                    m_PredictedByteIncrement = (long)(byteRate * (REFRESH_INTERVAL_MS + 15) / 1000.0); // TODO: determine time experied since last iteration by comparing DateTime.Now.Ticks or a more efficient system timer
                    m_PredictedByteIncrement -= m_PredictedByteIncrement % m_CurrentAudioPCMFormat.BlockAlign;
                }

                long pcmDataAvailableFromStream = m_PlaybackEndPosition - m_CurrentAudioStream.Position;

                long pcmDataTotalPlayableFromStream = m_PlaybackEndPosition - m_PlaybackStartPosition;

                long pcmDataAlreadyReadFromStream = pcmDataTotalPlayableFromStream - pcmDataAvailableFromStream;

                int circularBufferPlayPosition = m_CircularBuffer.PlayPosition;

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
                        m_TotalBytesPlayed += (m_CircularBuffer.Caps.BufferBytes - m_PreviousCircularBufferPlayPosition) + circularBufferPlayPosition;
                    }
                }

                m_PreviousCircularBufferPlayPosition = circularBufferPlayPosition;

                long remainingBytesToPlay = pcmDataTotalPlayableFromStream - m_TotalBytesPlayed;

                int circularBufferBytesAvailableForWriting = (circularBufferPlayPosition == m_CircularBufferWritePosition ? 0
                                        : (circularBufferPlayPosition < m_CircularBufferWritePosition
                                  ? circularBufferPlayPosition + (m_CircularBuffer.Caps.BufferBytes - m_CircularBufferWritePosition)
                                  : circularBufferPlayPosition - m_CircularBufferWritePosition));

                int circularBufferBytesAvailableForPlaying = m_CircularBuffer.Caps.BufferBytes - circularBufferBytesAvailableForWriting;

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


                if (PcmDataBufferAvailable != null
                    && m_PcmDataBuffer.Length <= circularBufferBytesAvailableForPlaying)
                {
                    Array array = m_CircularBuffer.Read(circularBufferPlayPosition, typeof(byte), LockFlag.None, m_PcmDataBuffer.Length);
                    Array.Copy(array, m_PcmDataBuffer, m_PcmDataBuffer.Length);

                    m_PcmDataBufferAvailableEventArgs.PcmDataBuffer = m_PcmDataBuffer;
                    PcmDataBufferAvailable(this, m_PcmDataBufferAvailableEventArgs);
                }

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

                    Debug.Assert(m_CurrentAudioStream.Position + bytesToTransferToCircularBuffer <= m_PlaybackEndPosition);

                    m_CircularBuffer.Write(m_CircularBufferWritePosition, m_CurrentAudioStream, (int)bytesToTransferToCircularBuffer, LockFlag.None);

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
                    m_CircularBufferWritePosition %= m_CircularBuffer.Caps.BufferBytes;
                    //if (m_CircularBufferWritePosition >= m_CircularBuffer.Caps.BufferBytes) m_CircularBufferWritePosition -= m_CircularBuffer.Caps.BufferBytes;
                }
            }

            //Console.WriteLine("Player refresh thread exiting....");

            CurrentState = State.Stopped;

            m_CircularBufferRefreshThread = null;
            stopPlayback();

            if (AudioPlaybackFinished != null)
                AudioPlaybackFinished(this, new AudioPlaybackFinishEventArgs());

            //Console.WriteLine("Player refresh thread exit.");
        }


        private void stopPlayback()
        {
            m_CircularBuffer.Stop();

            if (m_CircularBufferRefreshThread != null && m_CircularBufferRefreshThread.IsAlive)
            {
                m_CircularBufferRefreshThread.Abort();
                m_CircularBufferRefreshThread = null;
                //Console.WriteLine("Player refresh thread abort.");
            }

            if (!m_KeepStreamAlive)
            {
                EnsurePlaybackStreamIsDead();
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
