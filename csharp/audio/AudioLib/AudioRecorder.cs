using System;
using System.Collections.Generic;
using System.IO;
using System.Security;
using System.Threading;

#if USE_SLIMDX
using SlimDX.DirectSound;
using SlimDX.Multimedia;
#else
using Microsoft.DirectX.DirectSound;
using Buffer = System.Buffer;

#endif

namespace AudioLib
{
    //#if NET40
    //    [SecuritySafeCritical]
    //#endif
    public class AudioRecorder
    {
        private const int NOTIFICATIONS = 16;

        private byte[] m_PcmDataBuffer;
        private int m_PcmDataBufferLength;

        private CaptureBuffer m_CircularBuffer;
        private int m_CircularBufferReadPositon;
        private AutoResetEvent m_CircularBufferNotificationEvent;


        private readonly Object LOCK_THREAD_INSTANCE = new object();
        private Thread m_CircularBufferRefreshThread;

#if !USE_SLIMDX
        private Notify m_Notify;
#endif
        private long m_TotalRecordedBytes;


        public AudioRecorder()
        {
            CurrentState = State.NotReady;

            //m_CircularBufferNotificationEventCheckTimer.Enabled = false;
            //m_CircularBufferNotificationEventCheckTimer.Interval = 200;
            //m_CircularBufferNotificationEventCheckTimer.Tick += new EventHandler(onCircularBufferNotificationEventCheckTimerTick);
        }

        /// <summary>
        /// The three states of the audio recorder.
        /// NotReady: the recorder is not ready to record, for whatever reason.
        /// Stopped: the recorder is stopped and ready to record or monitor.
        /// Monitoring: the recording is listening but not writing any data.
        /// Recording: sound is currently being recorded.
        /// </summary>
        public enum State { NotReady, Stopped, Monitoring, Recording };

        private State m_PreviousState;
        private State m_State;
        public State CurrentState
        {
            get
            {
                return m_State;
            }
            private set
            {
                if (m_State == value)
                {
                    return;
                }

                m_PreviousState = m_State;
                m_State = value;
                StateChangedHandler del = StateChanged;
                if (del != null) del(this, new StateChangedEventArgs(m_PreviousState));
                //var del = StateChanged;
                //if (del != null)
                //del(this, new StateChangedEventArgs(m_PreviousState));
            }
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

        private string m_RecordedFilePath;
        public string RecordedFilePath { get { return m_RecordedFilePath; } }

        public event AudioRecordingFinishHandler AudioRecordingFinished;
        public delegate void AudioRecordingFinishHandler(object sender, AudioRecordingFinishEventArgs e);
        public class AudioRecordingFinishEventArgs : EventArgs
        {
            private string m_RecordedFilePath;
            public string RecordedFilePath
            {
                get { return m_RecordedFilePath; }
            }

            public AudioRecordingFinishEventArgs(string recordedFilePath)
            {
                m_RecordedFilePath = recordedFilePath;
            }
        }

        private readonly PcmDataBufferAvailableEventArgs m_PcmDataBufferAvailableEventArgs = new PcmDataBufferAvailableEventArgs(new byte[] { 0, 0, 0, 0 }, 4);
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

            private int m_PcmDataBufferLength;
            public int PcmDataBufferLength
            {
                get
                {
                    return m_PcmDataBufferLength;
                }
                set
                {
                    m_PcmDataBufferLength = value;

                    if (m_PcmDataBuffer != null)
                    {
                        DebugFix.Assert(m_PcmDataBufferLength <= m_PcmDataBuffer.Length);
                    }
                }
            }

            public PcmDataBufferAvailableEventArgs(byte[] pcmDataBuffer, int length)
            {
                m_PcmDataBuffer = pcmDataBuffer;
                m_PcmDataBufferLength = length;
            }
        }

        private readonly Object LOCK_DEVICES = new object();
        public void ClearDeviceCache()
        {
            lock (LOCK_DEVICES)
            {
                m_CachedInputDevices = null;
            }
        }

        public void SetInputDevice(string name)
        {
            lock (LOCK_DEVICES)
            {
                if (m_CachedInputDevices != null
                    && InputDevice != null && InputDevice.Name == name && !InputDevice.Capture.Disposed)
                {
                    return;
                }

                if (m_CachedInputDevices != null)
                {
                    InputDevice foundCached =
                        m_CachedInputDevices.Find(delegate(InputDevice d) { return d.Name == name; });
                    if (foundCached != null && !foundCached.Capture.Disposed)
                    {
                        InputDevice = foundCached;
                        return;
                    }
                }
            }

            List<InputDevice> devices = InputDevices;
            InputDevice found = devices.Find(delegate(InputDevice d) { return d.Name == name; });
            if (found != null)
            {
                InputDevice = found;
            }
            else if (devices.Count > 0)
            {
                InputDevice = devices[0]; //devices.Count-1
            }
            else
            {
                throw new Exception("No input device available.");
            }
        }

        InputDevice m_InputDevice;
        public InputDevice InputDevice
        {
            get { return m_InputDevice; }
            set
            {
                m_InputDevice = value;

                CurrentState = State.Stopped;
            }
        }

        private List<InputDevice> m_CachedInputDevices;
        public List<InputDevice> InputDevices
        {
            get
            {
                lock (LOCK_DEVICES)
                {
#if USE_SLIMDX
                DeviceCollection devices = DirectSoundCapture.GetDevices();
                    // new DeviceCollection();
#else
                    CaptureDevicesCollection devices = new CaptureDevicesCollection();
#endif
                    List<InputDevice> inputDevices = new List<InputDevice>(devices.Count);
                    foreach (DeviceInformation info in devices)
                    {
                        Console.WriteLine(info.ModuleName);
                        Console.WriteLine(info.Description);
                        Console.WriteLine(info.DriverGuid);
                        try
                        {
                            inputDevices.Add(new InputDevice(info));
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                            Console.WriteLine(ex.StackTrace);
                            continue;
                        }
                    }
                    m_CachedInputDevices = inputDevices;
                    return inputDevices;
                }
            }
        }


        private AudioLibPCMFormat m_RecordingPCMFormat;
        public AudioLibPCMFormat RecordingPCMFormat
        {
            get { return m_RecordingPCMFormat; }
            private set { m_RecordingPCMFormat = value; }
        }

        private ulong m_RecordedFileRiffHeaderSize;


        private string m_RecordingDirectory;
        public string RecordingDirectory
        {
            get
            {
                return m_RecordingDirectory;
            }
            set
            {
                m_RecordingDirectory = value;
                if (!Directory.Exists(m_RecordingDirectory))
                {
                    Directory.CreateDirectory(m_RecordingDirectory);
                }
            }
        }

        public long CurrentDurationBytePosition_BufferLookAhead
        {
            get
            {
                if (CurrentState == State.NotReady)
                {
                    return 0;
                }

                if (CurrentState != State.Monitoring && CurrentState != State.Recording)
                {
                    return 0;
                }

                Monitor.Enter(LOCK);
                try
                {
                    m_CircularBuffer.Stop();

                    int remainingBytesToRead = 0;
                    do
                    {
                        try
                        {
                            remainingBytesToRead = circularBufferTransferData();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                            Console.WriteLine(ex.StackTrace);
                            break;
                        }

                        //Console.WriteLine(string.Format("circularBufferTransferData: fetched remaining bytes: {0}", remainingBytesToRead));
                    } while (remainingBytesToRead > 0);

                    m_CircularBuffer.Start(true);
                }
                finally
                {
                    Monitor.Exit(LOCK);
                }

                return m_TotalRecordedBytes;
            }
        }

        public long CurrentDurationBytePosition
        {
            get
            {
                if (CurrentState == State.NotReady)
                {
                    return 0;
                }

                if (CurrentState != State.Monitoring && CurrentState != State.Recording)
                {
                    return 0;
                }

                return m_TotalRecordedBytes;
            }
        }

        public void StartMonitoring(AudioLibPCMFormat pcmFormat)
        {
            if (CurrentState == State.NotReady)
            {
                return;
            }

            if (CurrentState != State.Stopped)
            {
                return;
            }

            startRecordingOrMonitoring(pcmFormat, false);
        }


        public void StartRecording(AudioLibPCMFormat pcmFormat)
        {
            if (CurrentState == State.NotReady)
            {
                return;
            }

            if (CurrentState != State.Stopped)
            {
                return;
            }

            startRecordingOrMonitoring(pcmFormat, true);
        }

        private const int REFRESH_INTERVAL_MS = 75; //ms interval for refreshing PCM data
        private void startRecordingOrMonitoring(AudioLibPCMFormat pcmFormat, bool recordingToFile)
        {
            RecordingPCMFormat = pcmFormat;

            WaveFormat waveFormat = new WaveFormat();
            waveFormat.FormatTag = WaveFormatTag.Pcm;
            waveFormat.Channels = (short)RecordingPCMFormat.NumberOfChannels;
            waveFormat.SamplesPerSecond = (int)RecordingPCMFormat.SampleRate;
            waveFormat.BitsPerSample = (short)RecordingPCMFormat.BitDepth;
            waveFormat.AverageBytesPerSecond = (int)RecordingPCMFormat.SampleRate * RecordingPCMFormat.BlockAlign;
#if USE_SLIMDX
            waveFormat.BlockAlignment = (short)RecordingPCMFormat.BlockAlign;
#else
            waveFormat.BlockAlign = (short)RecordingPCMFormat.BlockAlign;
#endif
            uint byteRate = RecordingPCMFormat.SampleRate * RecordingPCMFormat.BlockAlign;

            int pcmDataBufferSize = (int)(byteRate * REFRESH_INTERVAL_MS / 1000.0);
            pcmDataBufferSize -= pcmDataBufferSize % RecordingPCMFormat.BlockAlign;

            m_PcmDataBufferLength = pcmDataBufferSize;
            if (m_PcmDataBuffer == null)
            {
                m_PcmDataBuffer = new byte[m_PcmDataBufferLength];
            }
            else if (m_PcmDataBuffer.Length < m_PcmDataBufferLength)
            {
                Array.Resize(ref m_PcmDataBuffer, m_PcmDataBufferLength);
            }

            int circularBufferSize = m_PcmDataBufferLength * NOTIFICATIONS;

            CaptureBufferDescription bufferDescription = new CaptureBufferDescription();
            bufferDescription.BufferBytes = circularBufferSize;
            bufferDescription.Format = waveFormat;
#if USE_SLIMDX
            bufferDescription.WaveMapped = false;
            m_CircularBuffer = new CaptureBuffer(InputDevice.Capture, bufferDescription);
#else
            m_CircularBuffer = new CaptureBuffer(bufferDescription, InputDevice.Capture);
#endif

            m_CircularBufferNotificationEvent = new AutoResetEvent(false);

#if USE_SLIMDX
            NotificationPosition[] m_BufferPositionNotify = new NotificationPosition[NOTIFICATIONS];
#else
            BufferPositionNotify[] m_BufferPositionNotify = new BufferPositionNotify[NOTIFICATIONS];
#endif
            for (int i = 0; i < NOTIFICATIONS; i++)
            {
                m_BufferPositionNotify[i].Offset = (m_PcmDataBufferLength * i) + m_PcmDataBufferLength - 1;
#if USE_SLIMDX
                m_BufferPositionNotify[i].Event = m_CircularBufferNotificationEvent;
#else
                m_BufferPositionNotify[i].EventNotifyHandle = m_CircularBufferNotificationEvent.SafeWaitHandle.DangerousGetHandle();
#endif
            }
#if USE_SLIMDX
            m_CircularBuffer.SetNotificationPositions(m_BufferPositionNotify);
#else
            m_Notify = new Notify(m_CircularBuffer);
            m_Notify.SetNotificationPositions(m_BufferPositionNotify, NOTIFICATIONS);
#endif

            m_CircularBufferReadPositon = 0;
            m_RecordingFileWriter = null;
            m_TotalRecordedBytes = 0;

            if (recordingToFile)
            {
                int i = -1;
                do
                {
                    i++;
                    m_RecordedFilePath = RecordingDirectory + Path.DirectorySeparatorChar + i.ToString() + WavFormatConverter.AUDIO_WAV_EXTENSION;

                } while (File.Exists(m_RecordedFilePath));

                Stream stream = File.Create(m_RecordedFilePath);
                try
                {
                    m_RecordedFileRiffHeaderSize = RecordingPCMFormat.RiffHeaderWrite(stream, 0);
                }
                finally
                {
                    stream.Close();
                }
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
                    lock (LOCK_THREAD_INSTANCE)
                    {
                        m_CircularBufferRefreshThread = null;
                    }
                }
                //lock (LOCK_THREAD_INSTANCE)
                //{
                //    m_CircularBufferRefreshThread = null;
                //}
            };

            lock (LOCK_THREAD_INSTANCE)
            {
                m_CircularBufferRefreshThread = new Thread(threadDelegate);
                m_CircularBufferRefreshThread.Name = "Recorder Notify Thread";
                m_CircularBufferRefreshThread.Priority = ThreadPriority.AboveNormal;
                m_CircularBufferRefreshThread.IsBackground = true;


                CurrentState = (recordingToFile ? State.Recording : State.Monitoring);

                m_CircularBuffer.Start(true);

                m_CircularBufferRefreshThread.Start();
            }


            //Console.WriteLine("Recorder notify thread start.");

            //m_PreviousTotalRecordedBytes = 0;
            //m_CircularBufferNotificationEventCheckTimer.Start();
        }

        //private System.Windows.Forms.Timer m_CircularBufferNotificationEventCheckTimer = new System.Windows.Forms.Timer();
        //long m_PreviousTotalRecordedBytes;
        //private void onCircularBufferNotificationEventCheckTimerTick(object sender, EventArgs e)
        //{
        //    if (m_PreviousTotalRecordedBytes == m_TotalRecordedBytes
        //        && CurrentState == State.Recording)
        //    {
        //        m_CircularBufferNotificationEvent.WaitOne(1);
        //    }
        //    m_PreviousTotalRecordedBytes = m_TotalRecordedBytes;
        //}

        private BinaryWriter m_RecordingFileWriter;

        private readonly Object LOCK = new object();
        private void circularBufferRefreshThreadMethod()
        {
            while (true)
            {
                m_CircularBufferNotificationEvent.WaitOne(Timeout.Infinite, true);

                Monitor.Enter(LOCK);
                try
                {
                    if (m_CircularBuffer == null || !m_CircularBuffer.Capturing || CurrentState == State.Stopped)
                    {
                        //Console.WriteLine("circularBufferRefreshThreadMethod EXIT while");
                        break;
                    }
                    else
                    {
                        circularBufferTransferData();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.StackTrace);
                    //Console.WriteLine(@"circularBufferRefreshThreadMethod EXCEPTION: " + ex.Message);
                    break;
                }
                finally
                {
                    Monitor.Exit(LOCK);
                }
            }

            //Console.WriteLine("Recorder notify thread exit.");
        }

#if USE_SLIMDX
        private byte[] incomingPcmData;
#endif

        private int circularBufferTransferData()
        {
#if USE_SLIMDX
            int circularBufferCapturePosition = m_CircularBuffer.CurrentCapturePosition;
            int readPosition = m_CircularBuffer.CurrentReadPosition;

#else
            int circularBufferCapturePosition;
            int readPosition;
            m_CircularBuffer.GetCurrentPosition(out circularBufferCapturePosition, out readPosition);
#endif
            int sizeBytes =
#if USE_SLIMDX
 m_CircularBuffer.SizeInBytes
#else
 m_CircularBuffer.Caps.BufferBytes
#endif
;

            int circularBufferBytesAvailableForReading = (circularBufferCapturePosition == m_CircularBufferReadPositon ? 0
                                    : (circularBufferCapturePosition < m_CircularBufferReadPositon
                              ? circularBufferCapturePosition + (sizeBytes - m_CircularBufferReadPositon)
                              : circularBufferCapturePosition - m_CircularBufferReadPositon));

            circularBufferBytesAvailableForReading -= (circularBufferBytesAvailableForReading % (sizeBytes / NOTIFICATIONS));

            int circularBufferBytesAvailableForCapturing = sizeBytes - circularBufferBytesAvailableForReading;

            if (circularBufferBytesAvailableForReading <= 0)
            {
                //Console.WriteLine(string.Format("circularBufferTransferData: no more bytes to fetch {0}", circularBufferBytesAvailableForReading));
                return circularBufferBytesAvailableForReading;
            }

            //int toRead = readPosition - m_CircularBufferReadPositon;
            //if (toRead < 0)
            //    toRead += m_CircularBuffer.Caps.BufferBytes;

            //toRead -= (toRead % (m_CircularBuffer.Caps.BufferBytes / NOTIFICATIONS));
            //if (toRead <= 0)
            //{
            //    Console.WriteLine(string.Format("BAD toRead {0}", toRead));
            //    continue;
            //}

#if USE_SLIMDX
            if (incomingPcmData == null)
            {
                incomingPcmData = new byte[circularBufferBytesAvailableForReading];
            }
            else if (incomingPcmData.Length != circularBufferBytesAvailableForReading)
            {
                Array.Resize(ref incomingPcmData, circularBufferBytesAvailableForReading);
            }
            m_CircularBuffer.Read(incomingPcmData, 0, circularBufferBytesAvailableForReading, m_CircularBufferReadPositon);

            //byte[] incomingPcmData = SlimDX_IntermediaryTransferBuffer;
#else
            byte[] incomingPcmData =
                (byte[])
                m_CircularBuffer.Read(m_CircularBufferReadPositon, typeof(byte),
                LockFlag.None,
                circularBufferBytesAvailableForReading);
#endif

            /*
             * 
    if (m_CircularBuffer != null && m_CircularBuffer.Capturing)
    {
        int capturePosition;
        int readPosition;
        m_CircularBuffer.GetCurrentPosition(out capturePosition, out readPosition);

        return
            RecordingPCMFormat.ConvertBytesToTime(m_TotalRecordedBytes + capturePosition -
                                                    m_CircularBufferReadPositon);
    }
             */
            //DebugFix.Assert(circularBufferBytesAvailableForReading == incomingPcmData.Length);

            m_TotalRecordedBytes += incomingPcmData.Length;

            m_CircularBufferReadPositon += incomingPcmData.Length;
            m_CircularBufferReadPositon %= sizeBytes;

            if (CurrentState == State.Recording)
            {
                if (m_RecordingFileWriter == null)
                {
                    //FileInfo fi = new FileInfo(m_RecordedFilePath);
                    //fi.FullName
                    m_RecordingFileWriter = new BinaryWriter(File.OpenWrite(m_RecordedFilePath));
                }

                m_RecordingFileWriter.BaseStream.Position = m_TotalRecordedBytes +
                                                            (long)m_RecordedFileRiffHeaderSize;
                // m_RecordingFileWriter.BaseStream.Length;
                m_RecordingFileWriter.Write(incomingPcmData, 0, incomingPcmData.Length);
            }

            PcmDataBufferAvailableHandler del = PcmDataBufferAvailable;
            if (del != null)
            {
                //if (m_PcmDataBuffer.Length != incomingPcmData.Length)
                //{
                //    //Console.WriteLine(string.Format(">>>>> Resizing buffer: m_PcmDataBuffer = {0}, incomingPcmData = {1}",m_PcmDataBuffer.Length, incomingPcmData.Length));

                //    Array.Resize(ref m_PcmDataBuffer, incomingPcmData.Length);
                //}
                //Array.Copy(incomingPcmData, m_PcmDataBuffer, m_PcmDataBuffer.Length);

                int min = Math.Min(m_PcmDataBufferLength, incomingPcmData.Length);

                //Array.Copy(incomingPcmData, m_PcmDataBuffer, min);
                Buffer.BlockCopy(incomingPcmData, 0,
                    m_PcmDataBuffer, 0,
                    min);

                m_PcmDataBufferAvailableEventArgs.PcmDataBuffer = m_PcmDataBuffer;
                m_PcmDataBufferAvailableEventArgs.PcmDataBufferLength = min;
                del(this, m_PcmDataBufferAvailableEventArgs);
            }

            return circularBufferBytesAvailableForReading;
        }

        public void StopRecording()
        {
            if (CurrentState == State.NotReady)
            {
                return;
            }

            PcmDataBufferAvailableHandler deleg = PcmDataBufferAvailable;
            if (deleg != null)
            {
                for (int i = 0; i < m_PcmDataBuffer.Length; i++)
                {
                    m_PcmDataBuffer[i] = 0;
                }
                m_PcmDataBufferAvailableEventArgs.PcmDataBuffer = m_PcmDataBuffer;
                m_PcmDataBufferAvailableEventArgs.PcmDataBufferLength = m_PcmDataBufferLength;
                deleg(this, m_PcmDataBufferAvailableEventArgs);
            }

            if (CurrentState != State.Recording && CurrentState != State.Monitoring)
            {
                return;
            }

            bool wasRecording = CurrentState == State.Recording;

            Monitor.Enter(LOCK);
            try
            {
                m_CircularBuffer.Stop();
            }
            finally
            {
                Monitor.Exit(LOCK);
            }

            m_CircularBufferNotificationEvent.Set();
            m_CircularBufferNotificationEvent.Close();

            //lock (LOCK_THREAD_INSTANCE)
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
                //    Console.WriteLine(@"///// RECORDER m_CircularBufferRefreshThread.Abort(): " + count++);
                //    lock (LOCK_THREAD_INSTANCE)
                //    {
                //        if (m_CircularBufferRefreshThread != null)
                //        {
                //            m_CircularBufferRefreshThread.Abort();
                //        }
                //    }
                //}

                Console.WriteLine(@"///// RECORDER m_CircularBufferRefreshThread != null: " + count++);
                Thread.Sleep(20);

                if (count > 15)
                {
                    lock (LOCK_THREAD_INSTANCE)
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

            int remainingBytesToRead = 0;
            do
            {
                try
                {
                    remainingBytesToRead = circularBufferTransferData();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.StackTrace);
                    break;
                }

                //Console.WriteLine(string.Format("circularBufferTransferData: fetched remaining bytes: {0}", remainingBytesToRead));
            } while (remainingBytesToRead > 0);

            if (m_RecordingFileWriter != null)
            {
                m_RecordingFileWriter.Close();

                //FileInfo fileInfo = new FileInfo(m_RecordedFilePath);
                Stream stream = File.OpenWrite(m_RecordedFilePath);
                long length = 0;
                try
                {
                    length = stream.Length;
                    // overriding the existing RIFF header, this time with correct data length
                    m_RecordedFileRiffHeaderSize = RecordingPCMFormat.RiffHeaderWrite(stream,
                                                            (uint)
                                                            (length -
                                                             (long)
                                                             m_RecordedFileRiffHeaderSize));
                }
                finally
                {
                    stream.Close();
                }

                if (length <= (long)m_RecordedFileRiffHeaderSize) // no PCM data, just RIFF header
                {
                    File.Delete(m_RecordedFilePath);
                    m_RecordedFilePath = null;
                }
            }

#if !USE_SLIMDX
            m_Notify.Dispose();
            m_Notify = null;
#endif

            m_CircularBuffer.Dispose();
            m_CircularBuffer = null;

            CurrentState = State.Stopped;

            if (wasRecording)
            {
                AudioRecordingFinishHandler del = AudioRecordingFinished;
                if (del != null) del(this, new AudioRecordingFinishEventArgs(m_RecordedFilePath));
                //var del = AudioRecordingFinished;
                //if (del != null)
                //del(this, new AudioRecordingFinishEventArgs(m_RecordedFilePath));
            }
        }
    }
}



//BinaryWriter Writer = new BinaryWriter(File.OpenWrite(RecordedFilePath));
//FileInfo RecordedFile = new FileInfo(RecordedFilePath);

//long Audiolength = RecordedFile.Length - 8;
//for (int i = 0; i < 4; i++)
//{
//    Writer.BaseStream.Position = i + 4;
//    Writer.Write(Convert.ToByte(CalculationFunctions.ConvertFromDecimal(Audiolength)[i]));
//}
//Audiolength = Audiolength - 36;
//for (int i = 0; i < 4; i++)
//{
//    Writer.BaseStream.Position = i + 40;
//    Writer.Write(Convert.ToByte(CalculationFunctions.ConvertFromDecimal(Audiolength)[i]));
//}
//Writer.Close();