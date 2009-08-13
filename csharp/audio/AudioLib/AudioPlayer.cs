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
        public event StateChangedHandler StateChanged;

        public event AudioPlaybackFinishHandler AudioPlaybackFinished;

        private readonly PcmDataBufferAvailableEventArgs m_PcmDataBufferAvailableEventArgs = new PcmDataBufferAvailableEventArgs(new byte[] { 0, 0, 0, 0 });
        public event PcmDataBufferAvailableHandler PcmDataBufferAvailable;


        //public event Events.Player.ResetVuMeterHandler ResetVuMeter;

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
            set
            {
                if (m_State == value)
                {
                    return;
                }

                State oldState = m_State;
                m_State = value;

                if (StateChanged != null)//mEventsEnabled && 
                    StateChanged(this, new StateChangedEventArgs(oldState));
            }
        }

        public delegate Stream StreamProviderDelegate();
        private StreamProviderDelegate m_CurrentAudioStreamProvider;
        private Stream m_CurrentAudioStream;
        private double m_CurrentAudioDuration;

        public void EnsurePlaybackStreamIsDead()
        {
            if (m_CurrentAudioStream != null)
            {
                m_CurrentAudioStream.Close();
                m_CurrentAudioStream = null;
            }

            m_CurrentAudioPCMFormat = null;
            m_CurrentAudioDuration = 0;
            m_CurrentAudioStreamProvider = null;
        }


        private Thread m_PcmDataBufferRefreshThread;

        private SecondaryBuffer m_DxBuffer;
        private int m_DxBufferRefreshSize;
        private int m_DxBufferNextWritePosition;


        private byte[] m_PcmDataBuffer;

        private long m_PlaybackStartPosition;
        private long m_PlaybackEndPosition;

        private long m_ResumeStartPosition;
        private long m_ResumeEndPosition;


        private readonly bool m_KeepStreamAlive;
        public AudioPlayer(bool keepStreamAlive)
        {
            m_KeepStreamAlive = keepStreamAlive;

            CurrentState = State.NotReady;
        }

        public void SetDevice(Control handle, OutputDevice device)
        {
            if (handle != null)
            {
                device.Device.SetCooperativeLevel(handle, CooperativeLevel.Priority);
            }

            OutputDevice = device;
        }

        public void SetDevice(Control handle, string name)
        {
            List<OutputDevice> devices = OutputDevices;
            OutputDevice found = devices.Find(delegate(OutputDevice d) { return d.Name == name; });
            if (found != null)
            {
                SetDevice(handle, found);
            }
            else if (devices.Count > 0)
            {
                SetDevice(handle, devices[0]);
            }
            else
            {
                CurrentState = State.NotReady;
                throw new Exception("No output device available.");
            }
        }

        private List<OutputDevice> m_DevicesList;
        public List<OutputDevice> OutputDevices
        {
            get
            {
                if (m_DevicesList != null)
                {
                    return m_DevicesList;
                }

                DevicesCollection devices = new DevicesCollection();
                m_DevicesList = new List<OutputDevice>(devices.Count);
                foreach (DeviceInformation info in devices)
                {
                    m_DevicesList.Add(new OutputDevice(info));
                }

                return m_DevicesList;
            }
        }

        private OutputDevice m_Device;
        public OutputDevice OutputDevice
        {
            get { return m_Device; }
            private set
            {
                m_Device = value;

                CurrentState = State.Stopped;
            }
        }


        private AudioLibPCMFormat m_CurrentAudioPCMFormat;
        public AudioLibPCMFormat CurrentAudioPCMFormat
        {
            get { return m_CurrentAudioPCMFormat; }
        }


        public void PlayBytes(StreamProviderDelegate currentAudioStreamProvider, long duration, AudioLibPCMFormat pcmInfo,
                            long from, long to)
        {
            if (currentAudioStreamProvider == null)
            {
                throw new ArgumentNullException("Stream cannot be null !");
            }
            if (duration <= 0)
            {
                throw new ArgumentOutOfRangeException("Duration cannot be <= 0 !");
            }

            m_CurrentAudioStreamProvider = currentAudioStreamProvider;
            m_CurrentAudioStream = m_CurrentAudioStreamProvider();
            m_CurrentAudioPCMFormat = pcmInfo;
            m_CurrentAudioDuration = m_CurrentAudioPCMFormat.ConvertBytesToTime(duration);
            
            Debug.Assert(CurrentState == State.Stopped || CurrentState == State.Paused, "Already playing?!");

            if (CurrentState == State.Stopped
                || CurrentState == State.Paused)
            {
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

                if (startPosition >= 0 &&
                    (endPosition == 0 || startPosition < endPosition) &&
                    endPosition <= duration)
                {
                    startPlayback(startPosition, endPosition);
                }
                else
                {
                    throw new Exception("Start/end positions out of bounds of audio asset.");
                }
            }
        }


        public void PlayTime(StreamProviderDelegate currentAudioStreamProvider, double duration, AudioLibPCMFormat pcmInfo, double from, double to)
        {
            if (currentAudioStreamProvider == null)
            {
                throw new ArgumentNullException("Stream cannot be null !");
            }
            if (duration <= 0)
            {
                throw new ArgumentOutOfRangeException("Duration cannot be <= 0 !");
            }

            m_CurrentAudioStreamProvider = currentAudioStreamProvider;
            m_CurrentAudioStream = m_CurrentAudioStreamProvider();
            m_CurrentAudioDuration = duration;
            m_CurrentAudioPCMFormat = pcmInfo;

            Debug.Assert(CurrentState == State.Stopped || CurrentState == State.Paused, "Already playing?!");

            if (CurrentState == State.Stopped
                || CurrentState == State.Paused)
            {
                long startPosition = 0;
                if (from > 0)
                {
                    startPosition = m_CurrentAudioPCMFormat.ConvertTimeToBytes(from);

                    startPosition -= startPosition % m_CurrentAudioPCMFormat.BlockAlign;
                }

                long endPosition = 0;
                if (to > 0)
                {
                    endPosition = m_CurrentAudioPCMFormat.ConvertTimeToBytes(to);

                    endPosition -= endPosition % m_CurrentAudioPCMFormat.BlockAlign;
                }

                if (startPosition >= 0 &&
                    (endPosition == 0 || startPosition < endPosition) &&
                    endPosition <= m_CurrentAudioPCMFormat.ConvertTimeToBytes(m_CurrentAudioDuration))
                {
                    startPlayback(startPosition, endPosition);
                }
                else
                {
                    throw new Exception("Start/end positions out of bounds of audio asset.");
                }
            }
        }

        public void Pause()
        {
            if (CurrentState == State.Paused)
            {
                return;
            }

            m_ResumeStartPosition = getCurrentBytePosition();
            m_ResumeEndPosition = m_PlaybackEndPosition;

            CurrentState = State.Paused;

            stopPlayback();
        }

        public void Resume()
        {
            if (CurrentState == State.Playing)
            {
                return;
            }

            startPlayback(m_ResumeStartPosition, m_ResumeEndPosition);
        }

        public void Stop()
        {
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
                return m_CurrentAudioPCMFormat.ConvertBytesToTime(getCurrentBytePosition());
            }
        }

        //private long m_LastKnownCurrentBytePosition;

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
                int dxPlayPos = m_DxBuffer.PlayPosition;

                //int dxWritePos = m_DxBuffer.WritePosition;
                int dxWritePos = m_DxBufferNextWritePosition;

                int bytesUnplayed = (dxPlayPos > dxWritePos ?
                                            m_DxBuffer.Caps.BufferBytes - dxPlayPos + dxWritePos :
                                            dxWritePos - dxPlayPos);

                long newPos = m_CurrentAudioStream.Position;

                if (bytesUnplayed > 0)
                {
                    //Console.WriteLine("bytesUnplayed adjustment: " + bytesUnplayed);

                    newPos -= bytesUnplayed;
                    newPos -= newPos % m_CurrentAudioPCMFormat.BlockAlign;
                }
                else if (bytesUnplayed < 0)
                {
                    Debug.Fail("bytesUnplayed < 0 ???");
                }

                return newPos;

                //if (newPos < m_LastKnownCurrentBytePosition)
                //{
                //    Console.WriteLine(String.Format("newPos [{0}] < m_LastKnownCurrentBytePosition [{1}]", newPos, m_LastKnownCurrentBytePosition));
                //    newPos = m_LastKnownCurrentBytePosition;
                //}

                //m_LastKnownCurrentBytePosition = newPos;
                //return m_LastKnownCurrentBytePosition;
            }

            return 0;
        }

        private const int REFRESH_INTERVAL_MS = 100; //ms interval for refreshing PCM data
        private void initializeBuffers()
        {
            // example: 44.1 kHz (44,100 samples per second) * 16 bits per sample / 8 bits per byte * 2 channels (stereo)
            // blockAlign is number of bytes per frame (samples required for all channels)
            int byteRate = m_CurrentAudioPCMFormat.SampleRate * m_CurrentAudioPCMFormat.BlockAlign;

            int pcmDataBufferSize = (int)(byteRate * REFRESH_INTERVAL_MS / 1000.0);
            pcmDataBufferSize -= pcmDataBufferSize % m_CurrentAudioPCMFormat.BlockAlign;
            m_PcmDataBuffer = new byte[pcmDataBufferSize];

            int dxBufferSize = (int)(byteRate * 1.0); // 1000ms
            dxBufferSize -= dxBufferSize % m_CurrentAudioPCMFormat.BlockAlign;

            m_DxBufferRefreshSize = (int)(byteRate * 0.250); //250ms
            m_DxBufferRefreshSize -= m_DxBufferRefreshSize % m_CurrentAudioPCMFormat.BlockAlign;


            WaveFormat waveFormat = new WaveFormat();
            waveFormat.FormatTag = WaveFormatTag.Pcm;
            waveFormat.BitsPerSample = Convert.ToInt16(m_CurrentAudioPCMFormat.BitDepth);
            waveFormat.Channels = Convert.ToInt16(m_CurrentAudioPCMFormat.NumberOfChannels);
            waveFormat.SamplesPerSecond = m_CurrentAudioPCMFormat.SampleRate;
            waveFormat.BlockAlign = Convert.ToInt16(m_CurrentAudioPCMFormat.BlockAlign);
            waveFormat.AverageBytesPerSecond = byteRate;

            BufferDescription bufferDescription = new BufferDescription();
            bufferDescription.BufferBytes = dxBufferSize;
            bufferDescription.Format = waveFormat;
            bufferDescription.ControlVolume = true;
            bufferDescription.ControlFrequency = true;
            bufferDescription.GlobalFocus = true;

            m_DxBuffer = new SecondaryBuffer(bufferDescription, OutputDevice.Device);
        }


        private void startPlayback(long startPosition, long endPosition)
        {
            if (CurrentState == State.Playing)
            {
                return;
            }

            initializeBuffers();

            m_PlaybackStartPosition = startPosition;
            m_PlaybackStartPosition -= m_PlaybackStartPosition % m_CurrentAudioPCMFormat.BlockAlign;

            //m_LastKnownCurrentBytePosition = m_PlaybackStartPosition;
            m_DxBufferNextWritePosition = 0;

            m_PlaybackEndPosition = endPosition == 0 ? m_CurrentAudioPCMFormat.ConvertTimeToBytes(m_CurrentAudioDuration) : endPosition;
            m_PlaybackEndPosition -= m_PlaybackEndPosition % m_CurrentAudioPCMFormat.BlockAlign;

            m_CurrentAudioStream = m_CurrentAudioStreamProvider();
            m_CurrentAudioStream.Position = m_PlaybackStartPosition;

            int initialFullBufferBytes = (int)Math.Min(m_PlaybackEndPosition - m_PlaybackStartPosition,
                                                m_DxBuffer.Caps.BufferBytes);
            
            m_DxBuffer.Write(0, m_CurrentAudioStream, initialFullBufferBytes, LockFlag.None);
            m_DxBufferNextWritePosition += initialFullBufferBytes;

            m_DxBufferNextWritePosition %= m_DxBuffer.Caps.BufferBytes;
            //if (m_DxBufferNextWritePosition >= m_DxBuffer.Caps.BufferBytes) m_DxBufferNextWritePosition -= m_DxBuffer.Caps.BufferBytes;

            //m_LastKnownCurrentBytePosition = m_CurrentAudioStream.Position;

            CurrentState = State.Playing;

            try
            {
                m_DxBuffer.Play(0, BufferPlayFlags.Looping);
            }
            catch (Exception)
            {
                Debug.Fail("EmergencyStopForSoundBufferProblem !");

                CurrentState = State.Stopped;

                stopPlayback();

                return;
            }

            m_PcmDataBufferRefreshThread = new Thread(new ThreadStart(pcmDataBufferRefreshLoop));
            m_PcmDataBufferRefreshThread.Name = "Player Refresh Thread";
            m_PcmDataBufferRefreshThread.Start();

            Console.WriteLine("Player refresh thread start.");
        }

        private int m_previousBytesAvailableForWriting;

        private void pcmDataBufferRefreshLoop()
        {
            while (true)
            {
                long dataAvailableFromStream = m_PlaybackEndPosition - m_CurrentAudioStream.Position;

                Thread.Sleep(REFRESH_INTERVAL_MS);
                if (m_DxBuffer.Status.BufferLost)
                {
                    m_DxBuffer.Restore();
                }

                int dxPlayPos = m_DxBuffer.PlayPosition;

                int bytesAvailableForWriting = (dxPlayPos == m_DxBufferNextWritePosition ? 0
                                        : (dxPlayPos < m_DxBufferNextWritePosition
                                  ? dxPlayPos + (m_DxBuffer.Caps.BufferBytes - m_DxBufferNextWritePosition)
                                  : dxPlayPos - m_DxBufferNextWritePosition));

                //Console.WriteLine(String.Format("bytesAvailableForWriting: [{0} / {1}]", bytesAvailableForWriting, m_DxBuffer.Caps.BufferBytes));

                //Console.WriteLine("dataAvailableFromStream: " + dataAvailableFromStream);

                if (dataAvailableFromStream <= 0)
                {
                    //Console.WriteLine(String.Format("NO  dataAvailableFromStream // m_PlaybackEndPosition [{0}], m_CurrentAudioStream.Position [{1}], dxPlayPos [{2}], m_DxBufferNextWritePosition [{3}]",
                    //    m_PlaybackEndPosition, m_CurrentAudioStream.Position, dxPlayPos, m_DxBufferNextWritePosition));

                    long toleranceMargin = m_CurrentAudioPCMFormat.ConvertTimeToBytes(REFRESH_INTERVAL_MS * 1.5);

                    if ((bytesAvailableForWriting + toleranceMargin) >= m_DxBuffer.Caps.BufferBytes
                        || m_previousBytesAvailableForWriting > bytesAvailableForWriting)
                    {
                        m_DxBuffer.Stop(); // the earlier the better ?

                        //Console.WriteLine("Forcing closing-up.");

                        bytesAvailableForWriting = 0;
                    }
                }

                m_previousBytesAvailableForWriting = bytesAvailableForWriting;

                if (bytesAvailableForWriting <= 0)
                {
                    if (dataAvailableFromStream > 0)
                    {
                        //Console.WriteLine("bytesAvailableForWriting <= 0, dataAvailableFromStream > 0 ... continue...");
                        continue;
                    }
                    else
                    {
                        //Console.WriteLine("bytesAvailableForWriting <= 0, dataAvailableFromStream <= 0 ... BREAK...");
                        break;
                    }
                }

                if (dataAvailableFromStream > 0)
                {
                    int toCopy = Math.Min(bytesAvailableForWriting, m_DxBufferRefreshSize);
                    toCopy -= toCopy % m_CurrentAudioPCMFormat.BlockAlign;

                    if (toCopy <= 0)
                    {
                        Debug.Fail("toCopy <= 0 !!");
                        continue;
                    }

                    if (toCopy > dataAvailableFromStream)
                    {
                        //Console.WriteLine("---- Using-up remaining bytes: " + dataAvailableFromStream);

                        toCopy = (int)dataAvailableFromStream;
                    }

                    //Console.WriteLine(String.Format("m_DxBufferNextWritePosition: [{0} / toCopy {1}]", m_DxBufferNextWritePosition, toCopy));

                    Debug.Assert(m_CurrentAudioStream.Position + toCopy <= m_PlaybackEndPosition);

                    m_DxBuffer.Write(m_DxBufferNextWritePosition, m_CurrentAudioStream, toCopy, LockFlag.None);

                    //int afterWriteCursor = m_DxBuffer.Caps.BufferBytes - m_DxBufferNextWritePosition;
                    //if (toCopy <= afterWriteCursor)
                    //{
                    //    m_DxBuffer.Write(m_DxBufferNextWritePosition, m_CurrentAudioStream, toCopy, LockFlag.None);
                    //}
                    //else
                    //{
                    //    m_DxBuffer.Write(m_DxBufferNextWritePosition, m_CurrentAudioStream, afterWriteCursor, LockFlag.None);
                    //    m_DxBuffer.Write(0, m_CurrentAudioStream, toCopy - afterWriteCursor, LockFlag.None);
                    //}

                    m_DxBufferNextWritePosition += toCopy;
                    m_DxBufferNextWritePosition %= m_DxBuffer.Caps.BufferBytes;
                    //if (m_DxBufferNextWritePosition >= m_DxBuffer.Caps.BufferBytes) m_DxBufferNextWritePosition -= m_DxBuffer.Caps.BufferBytes;
                }

                if (PcmDataBufferAvailable != null
                && m_PcmDataBuffer.Length <= m_DxBuffer.Caps.BufferBytes - bytesAvailableForWriting)
                {
                    Array array = m_DxBuffer.Read(dxPlayPos, typeof(byte), LockFlag.None, m_PcmDataBuffer.Length);
                    Array.Copy(array, m_PcmDataBuffer, m_PcmDataBuffer.Length);

                    m_PcmDataBufferAvailableEventArgs.PcmDataBuffer = m_PcmDataBuffer;
                    PcmDataBufferAvailable(this, m_PcmDataBufferAvailableEventArgs);
                }
            }

            CurrentState = State.Stopped;

            m_PcmDataBufferRefreshThread = null;
            stopPlayback();

            if (AudioPlaybackFinished != null)
                AudioPlaybackFinished(this, new AudioPlaybackFinishEventArgs());

            Console.WriteLine("Player refresh thread exit.");
        }


        private void stopPlayback()
        {
            m_DxBuffer.Stop();

            if (m_PcmDataBufferRefreshThread != null && m_PcmDataBufferRefreshThread.IsAlive)
            {
                m_PcmDataBufferRefreshThread.Abort();
                m_PcmDataBufferRefreshThread = null;
                Console.WriteLine("Player refresh thread abort.");
            }

            if (!m_KeepStreamAlive)
            {
                EnsurePlaybackStreamIsDead();
            }
        }



        public delegate void AudioPlaybackFinishHandler(object sender, AudioPlaybackFinishEventArgs e);

        public class AudioPlaybackFinishEventArgs : EventArgs
        {
        }

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
