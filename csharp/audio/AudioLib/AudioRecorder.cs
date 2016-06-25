//#define SIMULATE_BUFFER_NOTIFICATION_SKIP

#define FORCE_SINGLE_NOTIFICATION_EVENT

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security;
using System.Threading;

#if USE_SHARPDX
using SharpDX.DirectSound;
using SharpDX.Multimedia;
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

#if FORCE_SINGLE_NOTIFICATION_EVENT

        private System.Windows.Forms.Timer m_CircularBufferTimer = new System.Windows.Forms.Timer();

        private ulong m_PreviousTotalRecordedBytes;


        public event CircularBufferNotificationTimerMessageHandler CircularBufferNotificationTimerMessage;
        public delegate void CircularBufferNotificationTimerMessageHandler(object sender, CircularBufferNotificationTimerMessageEventArgs e);
        public class CircularBufferNotificationTimerMessageEventArgs : EventArgs
        {
            private String m_msg;
            public String Msg
            {
                get
                {
                    return m_msg;
                }
            }

            public CircularBufferNotificationTimerMessageEventArgs(String msg)
            {
                m_msg = msg;
            }
        }

        private bool m_DO_LOG_CircularBufferNotificationTimerMessage = false;

        private void onCircularBufferTimerTick(object sender, EventArgs e)
        {
//#if DEBUG
//            Console.WriteLine("TICK");
//#endif

            if (m_PreviousTotalRecordedBytes == m_TotalRecordedBytes // Skipped notifications?
                && CurrentState == State.Recording)
            {
                m_DO_LOG_CircularBufferNotificationTimerMessage = true;

                String msg = "DO_LOG_CircularBufferNotificationTimerMessage:: onCircularBufferTimerTick EVENT SET (skipped notifications?) " +
                             m_PreviousTotalRecordedBytes + " / " + m_TotalRecordedBytes;
#if DEBUG
                Console.WriteLine(msg);
                System.Media.SystemSounds.Asterisk.Play();
#endif
                CircularBufferNotificationTimerMessageHandler del = CircularBufferNotificationTimerMessage;
                if (del != null)
                {
                    del(this, new CircularBufferNotificationTimerMessageEventArgs(msg));
                }

                // See relevant legacy old code diffs:
                //http://daisy-trac.cvsdude.com/urakawa-sdk/changeset?reponame=&new=1411%40trunk%2Fcsharp%2Faudio%2FAudioLib%2FAudioRecorder.cs&old=1405%40trunk%2Fcsharp%2Faudio%2FAudioLib%2FAudioRecorder.cs
                //http://daisy-trac.cvsdude.com/urakawa-sdk/changeset?reponame=&new=1490%40trunk%2Fcsharp%2Faudio%2FAudioLib%2FAudioRecorder.cs&old=1486%40trunk%2Fcsharp%2Faudio%2FAudioLib%2FAudioRecorder.cs
                //http://daisy-trac.cvsdude.com/urakawa-sdk/changeset?reponame=&new=1494%40trunk%2Fcsharp%2Faudio%2FAudioLib%2FAudioRecorder.cs&old=1491%40trunk%2Fcsharp%2Faudio%2FAudioLib%2FAudioRecorder.cs
                //m_CircularBufferNotificationEvent.WaitOne(1);
                m_CircularBufferNotificationEvent.Set();
            }
            else
            {
                m_DO_LOG_CircularBufferNotificationTimerMessage = false;
            }

            m_PreviousTotalRecordedBytes = m_TotalRecordedBytes;
        }

        private AutoResetEvent m_CircularBufferNotificationEvent;
#else
        private AutoResetEvent[] m_CircularBufferNotificationEvents;
#endif

#if !USE_SHARPDX
        private Notify m_Notify;
#endif


        private const int REFRESH_INTERVAL_MS = 75; //ms interval for refreshing PCM data

        private byte[] m_PcmDataBuffer;
        private int m_PcmDataBufferLength;

        private CaptureBuffer m_CircularBuffer;
        private int m_CircularBufferReadPositon;




        private readonly Object LOCK_THREAD_INSTANCE = new object();
        private Thread m_CircularBufferRefreshThread;

        private ulong m_TotalRecordedBytes;


        //private void On_CircularBufferNotificationTimerMessage(object sender, CircularBufferNotificationTimerMessageEventArgs e)
        //{
        //    Console.WriteLine(e.Msg);
        //}

        public AudioRecorder()
        {
            //CircularBufferNotificationTimerMessage += On_CircularBufferNotificationTimerMessage;

            CurrentState = State.NotReady;

#if FORCE_SINGLE_NOTIFICATION_EVENT
            m_CircularBufferTimer.Enabled = false;
            m_CircularBufferTimer.Interval = REFRESH_INTERVAL_MS * 2 + 50; //  150 + 50 = 200ms
            m_CircularBufferTimer.Tick += new EventHandler(onCircularBufferTimerTick);
#endif
        }

        private void AudioRecorder_CircularBufferNotificationTimerMessage(object sender, CircularBufferNotificationTimerMessageEventArgs e)
        {
            throw new NotImplementedException();
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
                    && InputDevice != null && InputDevice.Name == name && !InputDevice.Capture.
#if USE_SHARPDX
IsDisposed
#else
 Disposed
#endif
)
                {
                    return;
                }

                if (m_CachedInputDevices != null)
                {
                    InputDevice foundCached =
                        m_CachedInputDevices.Find(delegate (InputDevice d) { return d.Name == name; });
                    if (foundCached != null && !foundCached.Capture.
#if USE_SHARPDX
IsDisposed
#else
 Disposed
#endif
)
                    {
                        InputDevice = foundCached;
                        return;
                    }
                }
            }

            List<InputDevice> devices = InputDevices;
            InputDevice found = devices.Find(delegate (InputDevice d) { return d.Name == name; });
            if (found != null)
            {
                InputDevice = found;
            }
            else if (devices.Count > 0)
            {
                InputDevice = devices[0]; //devices.Count-1

                Console.WriteLine("InputDevice name not found, defaulting: [[" + name + "]] ==> [[" + InputDevice.Name + "]]");
            }
            else
            {
                //throw new Exception("No input device available.");
                Console.WriteLine("ERROR: InputDevices empty!!");
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
                Console.WriteLine("=== InputDevices");

                lock (LOCK_DEVICES)
                {
#if USE_SHARPDX
                    List<DeviceInformation> devices = DirectSoundCapture.GetDevices();
#else
                    CaptureDevicesCollection devices = new CaptureDevicesCollection();
#endif
                    List<InputDevice> inputDevices = new List<InputDevice>(devices.Count);
                    foreach (DeviceInformation info in devices)
                    {
                        Console.WriteLine("InputDevice ModuleName:");
                        Console.WriteLine(info.ModuleName);

                        Console.WriteLine("InputDevice Description:");
                        Console.WriteLine(info.Description);

                        Console.WriteLine("InputDevice DriverGuid:");
                        Console.WriteLine(info.DriverGuid);
                        try
                        {
                            inputDevices.Add(new InputDevice(info));
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("InputDevice FAILED:");
                            Console.WriteLine(ex.Message);
                            Console.WriteLine(ex.StackTrace);
                            continue;
                        }
                    }
                    m_CachedInputDevices = inputDevices;

                    Console.WriteLine(">>> InputDevices: " + inputDevices.Count);
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

        public ulong CurrentDurationBytePosition_BufferLookAhead
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

                int circularBufferBytesAvailableForReading = 0;

                Monitor.Enter(LOCK_CIRCULAR_BUFFER);
                try
                {

#if USE_SHARPDX
                    int circularBufferCapturePosition = m_CircularBuffer.CurrentCapturePosition;
                    //int readPosition = m_CircularBuffer.CurrentRealPosition;
#else
                    int circularBufferCapturePosition;
                    int readPosition; // UNUSED
                    m_CircularBuffer.GetCurrentPosition(out circularBufferCapturePosition, out readPosition);
#endif
                    int circularBufferBytes = m_CircularBuffer.
#if USE_SHARPDX
Capabilities
#else
Caps
#endif
.BufferBytes
        ;
                    circularBufferBytesAvailableForReading = (circularBufferCapturePosition == m_CircularBufferReadPositon ? 0
                                            : (circularBufferCapturePosition < m_CircularBufferReadPositon
                                      ? circularBufferCapturePosition + (circularBufferBytes - m_CircularBufferReadPositon)
                                      : circularBufferCapturePosition - m_CircularBufferReadPositon));


#if DEBUG
                    if (circularBufferBytesAvailableForReading > 0)
                    {
                        Console.WriteLine(string.Format("REMAINING buffer bytes (STOP RECORD AND CONTINUE): {0}", circularBufferBytesAvailableForReading));
                    }
#endif

                }
                finally
                {
                    Monitor.Exit(LOCK_CIRCULAR_BUFFER);
                }

                return m_TotalRecordedBytes + (ulong)circularBufferBytesAvailableForReading;
            }
        }

        public ulong CurrentDurationBytePosition
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

        private void startRecordingOrMonitoring(AudioLibPCMFormat pcmFormat, bool recordingToFile)
        {
            RecordingPCMFormat = pcmFormat;

#if USE_SHARPDX
            WaveFormat waveFormat = new WaveFormat((int)RecordingPCMFormat.SampleRate, 16, (int)RecordingPCMFormat.NumberOfChannels);
#else
            WaveFormat waveFormat = new WaveFormat();
            waveFormat.FormatTag = WaveFormatTag.Pcm;
            waveFormat.Channels = (short)RecordingPCMFormat.NumberOfChannels;
            waveFormat.SamplesPerSecond = (int)RecordingPCMFormat.SampleRate;
            waveFormat.BitsPerSample = (short)RecordingPCMFormat.BitDepth;
            waveFormat.AverageBytesPerSecond = (int)RecordingPCMFormat.SampleRate * RecordingPCMFormat.BlockAlign;
            waveFormat.BlockAlign = (short)RecordingPCMFormat.BlockAlign;
#endif

            uint byteRate = RecordingPCMFormat.SampleRate * RecordingPCMFormat.BlockAlign;

            int pcmDataBufferSize = (int)(byteRate * REFRESH_INTERVAL_MS / 1000.0);
            pcmDataBufferSize -= pcmDataBufferSize % RecordingPCMFormat.BlockAlign;

            m_PcmDataBufferLength = pcmDataBufferSize;
            if (m_PcmDataBuffer == null)
            {
                Console.WriteLine("ALLOCATING m_PcmDataBuffer");
                m_PcmDataBuffer = new byte[m_PcmDataBufferLength];
            }
            else if (m_PcmDataBuffer.Length < m_PcmDataBufferLength)
            {
                Console.WriteLine("m_PcmDataBuffer.resize");
                Array.Resize(ref m_PcmDataBuffer, m_PcmDataBufferLength);
            }

            int circularBufferSize = m_PcmDataBufferLength * NOTIFICATIONS;

            CaptureBufferDescription bufferDescription = new CaptureBufferDescription();
            bufferDescription.BufferBytes = circularBufferSize;
            bufferDescription.Format = waveFormat;
#if USE_SHARPDX
            bufferDescription.Flags = CaptureBufferCapabilitiesFlags.WaveMapped; //CaptureBufferCapabilitiesFlags.ControlEffects

            if (InputDevice == null)
            {
                Console.WriteLine("/// InputDevice NULL, attempting reset...");

                List<InputDevice> inputDevices = InputDevices;

                Console.WriteLine("/// InputDevices: " + inputDevices.Count);

                SetInputDevice("dummy");
            }

            m_CircularBuffer = new CaptureBuffer(InputDevice.Capture, bufferDescription);
#else
            m_CircularBuffer = new CaptureBuffer(bufferDescription, InputDevice.Capture);
#endif

#if FORCE_SINGLE_NOTIFICATION_EVENT
            m_CircularBufferNotificationEvent = new AutoResetEvent(false);
#else
            m_CircularBufferNotificationEvents = new AutoResetEvent[NOTIFICATIONS];
#endif

#if USE_SHARPDX
            NotificationPosition[] m_BufferPositionNotify = new NotificationPosition[NOTIFICATIONS];
#else
            BufferPositionNotify[] m_BufferPositionNotify = new BufferPositionNotify[NOTIFICATIONS];
#endif
            

            for (int i = 0; i < NOTIFICATIONS; i++)
            {

#if USE_SHARPDX
                m_BufferPositionNotify[i] = new NotificationPosition();
#endif

                m_BufferPositionNotify[i].Offset = (m_PcmDataBufferLength * i) + m_PcmDataBufferLength - 1;

#if FORCE_SINGLE_NOTIFICATION_EVENT

#if USE_SHARPDX
                m_BufferPositionNotify[i].WaitHandle = m_CircularBufferNotificationEvent;
                //m_BufferPositionNotify[i].EventNotifyHandlePointer = m_CircularBufferNotificationEvent.SafeWaitHandle.DangerousGetHandle();
#else
                m_BufferPositionNotify[i].EventNotifyHandle = m_CircularBufferNotificationEvent.SafeWaitHandle.DangerousGetHandle();
#endif

#else
                m_CircularBufferNotificationEvents[i] = new AutoResetEvent(false);

#if USE_SHARPDX
                m_BufferPositionNotify[i].WaitHandle = m_CircularBufferNotificationEvents[i];
                //m_BufferPositionNotify[i].EventNotifyHandlePointer = m_CircularBufferNotificationEvents[i].SafeWaitHandle.DangerousGetHandle();
#else
                m_BufferPositionNotify[i].EventNotifyHandle = m_CircularBufferNotificationEvents[i].SafeWaitHandle.DangerousGetHandle();
#endif

#endif
            }

#if USE_SHARPDX
            m_CircularBuffer.SetNotificationPositions(m_BufferPositionNotify);
#else
            m_Notify = new Notify(m_CircularBuffer);
            m_Notify.SetNotificationPositions(m_BufferPositionNotify, NOTIFICATIONS);
#endif

            m_CircularBufferReadPositon = 0;
            m_RecordingFileWriter = null;

            m_TotalRecordedBytes = 0;
#if FORCE_SINGLE_NOTIFICATION_EVENT
            m_PreviousTotalRecordedBytes = 0;
#endif

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

            ThreadStart threadDelegate = delegate ()
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

#if FORCE_SINGLE_NOTIFICATION_EVENT
                m_CircularBufferTimer.Start();
#endif
            }


            //Console.WriteLine("Recorder notify thread start.");
        }

        private BinaryWriter m_RecordingFileWriter;

        private readonly Object LOCK_CIRCULAR_BUFFER = new object();
        private void circularBufferRefreshThreadMethod()
        {
            while (true)
            {

#if FORCE_SINGLE_NOTIFICATION_EVENT
                m_CircularBufferNotificationEvent.WaitOne(Timeout.Infinite, true);
#else
                int eventIndex = WaitHandle.WaitAny(m_CircularBufferNotificationEvents);
#endif

                Monitor.Enter(LOCK_CIRCULAR_BUFFER);
                try
                {
                    if (m_CircularBuffer == null || !m_CircularBuffer.Capturing || CurrentState == State.Stopped)
                    {
                        //Console.WriteLine("circularBufferRefreshThreadMethod EXIT while");
                        break;
                    }
                    else
                    {
                        circularBufferTransferData(
#if !FORCE_SINGLE_NOTIFICATION_EVENT
eventIndex,
false,
#endif
false
);
                    }
                }
                catch (Exception ex)
                {
#if DEBUG
                    Debugger.Break();
#endif
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.StackTrace);
                    //Console.WriteLine(@"circularBufferRefreshThreadMethod EXCEPTION: " + ex.Message);
                    break;
                }
                finally
                {
                    Monitor.Exit(LOCK_CIRCULAR_BUFFER);
                }
            }

            //Console.WriteLine("Recorder notify thread exit.");
        }

#if USE_SHARPDX
        private byte[] incomingPcmData;
#endif

#if !FORCE_SINGLE_NOTIFICATION_EVENT
        private int m_previousEventIndex = -1;
        //private int m_circularBufferCapturePosition = -1;
#endif

        private int circularBufferTransferData(
#if !FORCE_SINGLE_NOTIFICATION_EVENT
int eventIndex,
bool catchingUp,
#endif
bool lastFlush
)
        {
#if !FORCE_SINGLE_NOTIFICATION_EVENT

#if SIMULATE_BUFFER_NOTIFICATION_SKIP
            Console.WriteLine(eventIndex);
#endif

            if (!catchingUp)
            {
#if SIMULATE_BUFFER_NOTIFICATION_SKIP

                // NOTIFICATIONS = 16, each REFRESH_INTERVAL_MS = 75
                bool beginning = eventIndex == 0 || eventIndex == 1 || eventIndex == 2;
                bool end = eventIndex == 13 || eventIndex == 14 || eventIndex == 15;
                bool middle = eventIndex == 6 || eventIndex == 7 || eventIndex == 8;
                bool middleTwo = eventIndex == 3 || eventIndex == 4 || eventIndex == 5 || eventIndex == 9 || eventIndex == 10 || eventIndex == 11;
                //if (beginning)
                //if (end)
                //if (beginning || end)
                //if (beginning || middle)
                //if (middle || end)
                if (beginning || middle || end)
                //if (middle)
                //if (middleTwo)
                {
                    Console.WriteLine("SKIPPED.");

                    return 0;
                }
#endif
                if ((m_previousEventIndex == -1 || m_previousEventIndex >= 0) && eventIndex >= 0)
                {
                    int nextEventIndex = m_previousEventIndex + 1;
                    if (nextEventIndex >= NOTIFICATIONS)
                    {
                        nextEventIndex = 0;
                    }

                    int toCatchUp = 0;
                    if (eventIndex == nextEventIndex)
                    {
                        // OK
                    }
                    else if (eventIndex > nextEventIndex)
                    {
                        // ahead, but not looped back in circular buffer

                        toCatchUp = eventIndex - nextEventIndex;
                    }
                    else if (eventIndex < nextEventIndex)
                    {
                        // ahead, but looped back in circular buffer

                        toCatchUp = (NOTIFICATIONS - nextEventIndex) + eventIndex;
                    }

                    if (toCatchUp > 0)
                    {
#if SIMULATE_BUFFER_NOTIFICATION_SKIP
                        Console.WriteLine("TO CATCHUP: {0}", toCatchUp);
#endif
                        for (int i = 0; i < toCatchUp; i++)
                        {
#if SIMULATE_BUFFER_NOTIFICATION_SKIP
                            Console.WriteLine("Audio recording event catch-up {0}, {1} => {2}", eventIndex, m_previousEventIndex,
                                nextEventIndex);
#endif
                            circularBufferTransferData(nextEventIndex, true, lastFlush);
                            nextEventIndex++;
                            if (nextEventIndex >= NOTIFICATIONS)
                            {
                                nextEventIndex = 0;
                            }
                        }

                        //DebugFix.Assert(m_previousEventIndex == (eventIndex-1));
                    }
                }
            }


            m_previousEventIndex = eventIndex;
#endif

#if USE_SHARPDX
            int circularBufferCapturePosition = m_CircularBuffer.CurrentCapturePosition;
            //int readPosition = m_CircularBuffer.CurrentRealPosition;
#else
            int circularBufferCapturePosition;
            int readPosition; // UNUSED
            m_CircularBuffer.GetCurrentPosition(out circularBufferCapturePosition, out readPosition);
#endif

            //#if !FORCE_SINGLE_NOTIFICATION_EVENT
            //            if (catchingUp)
            //            {
            //                circularBufferCapturePosition = m_circularBufferCapturePosition;
            //            }
            //            else
            //            {
            //                m_circularBufferCapturePosition = circularBufferCapturePosition;
            //            }
            //#endif


            int circularBufferBytes = m_CircularBuffer.
#if USE_SHARPDX
Capabilities
#else
Caps
#endif
.BufferBytes
;
            int notifyChunk = circularBufferBytes / NOTIFICATIONS;


            int circularBufferBytesAvailableForReading = (circularBufferCapturePosition == m_CircularBufferReadPositon ? 0
                                    : (circularBufferCapturePosition < m_CircularBufferReadPositon
                              ? circularBufferCapturePosition + (circularBufferBytes - m_CircularBufferReadPositon)
                              : circularBufferCapturePosition - m_CircularBufferReadPositon));

            if (lastFlush)
            {
#if !FORCE_SINGLE_NOTIFICATION_EVENT
                DebugFix.Assert(eventIndex == -1);
#endif
                //circularBufferBytesAvailableForReading -= (circularBufferBytesAvailableForReading % notifyChunk);
#if DEBUG
                Console.WriteLine("m_CircularBufferReadPositon: " + m_CircularBufferReadPositon);
                Console.WriteLine("circularBufferBytesAvailableForReading; " + circularBufferBytesAvailableForReading);
#endif
            }
            else
            {
#if FORCE_SINGLE_NOTIFICATION_EVENT
                if (m_DO_LOG_CircularBufferNotificationTimerMessage)
                {
                    String msg = "DO_LOG_CircularBufferNotificationTimerMessage:: circularBufferBytes @ m_CircularBufferReadPositon # circularBufferCapturePosition - circularBufferBytesAvailableForReading / notifyChunk $ incomingPcmData.Length: " +
                                 circularBufferBytes + " @ " + m_CircularBufferReadPositon + " # " + circularBufferCapturePosition + " - " + circularBufferBytesAvailableForReading + " / " + notifyChunk + " $ " + (incomingPcmData == null ? "NULL" : ("" + incomingPcmData.Length));
#if DEBUG
                    Console.WriteLine(msg);
#endif
                    CircularBufferNotificationTimerMessageHandler del_ = CircularBufferNotificationTimerMessage;
                    if (del_ != null)
                    {
                        del_(this, new CircularBufferNotificationTimerMessageEventArgs(msg));
                    }
                }
#else
                DebugFix.Assert(eventIndex >= 0);
                
                circularBufferBytesAvailableForReading = notifyChunk;
#endif
            }

            if (circularBufferBytesAvailableForReading <= 0)
            {
                //Console.WriteLine(string.Format("circularBufferTransferData: no more bytes to fetch {0}", circularBufferBytesAvailableForReading));
                return circularBufferBytesAvailableForReading;
            }

            DebugFix.Assert(circularBufferBytesAvailableForReading <= circularBufferBytes);
            if (circularBufferBytesAvailableForReading > circularBufferBytes)
            {
                circularBufferBytesAvailableForReading = circularBufferBytes;
            }

            int circularBufferBytesAvailableForCapturing = circularBufferBytes - circularBufferBytesAvailableForReading;
            
            //int toRead = readPosition - m_CircularBufferReadPositon;
            //if (toRead < 0)
            //    toRead += m_CircularBuffer.Caps.BufferBytes;

            //toRead -= (toRead % (m_CircularBuffer.Caps.BufferBytes / NOTIFICATIONS));
            //if (toRead <= 0)
            //{
            //    Console.WriteLine(string.Format("BAD toRead {0}", toRead));
            //    continue;
            //}

#if !FORCE_SINGLE_NOTIFICATION_EVENT
            if (eventIndex >= 0)
            {
                DebugFix.Assert(circularBufferBytesAvailableForReading == notifyChunk);

                int pos = eventIndex * notifyChunk;

                //DebugFix.Assert(pos == m_CircularBufferReadPositon);
                if (m_CircularBufferReadPositon != pos)
                {
#if DEBUG
                    Console.WriteLine("READ POS ADJUST: " + m_CircularBufferReadPositon + " != " + pos + " // " + notifyChunk + " -- " + (pos - m_CircularBufferReadPositon));
#endif
                    m_CircularBufferReadPositon = pos;
                }
            }
#endif

#if USE_SHARPDX
            if (incomingPcmData == null)
            {
                Console.WriteLine("ALLOCATING incomingPcmData");
                incomingPcmData = new byte[circularBufferBytesAvailableForReading];
            }
            else if (incomingPcmData.Length < circularBufferBytesAvailableForReading)
            {
                Console.WriteLine("incomingPcmData.resize");
                Array.Resize(ref incomingPcmData, circularBufferBytesAvailableForReading);
            }

            DebugFix.Assert(circularBufferBytesAvailableForReading <= incomingPcmData.Length);


            m_CircularBuffer.Read(incomingPcmData, 0, circularBufferBytesAvailableForReading, m_CircularBufferReadPositon, LockFlags.None);

#else //  !USE_SHARPDX

            byte[] incomingPcmData = (byte[]) m_CircularBuffer.Read(m_CircularBufferReadPositon, typeof(byte), LockFlag.None, circularBufferBytesAvailableForReading);

            DebugFix.Assert(circularBufferBytesAvailableForReading == incomingPcmData.Length);
#endif


            //if (m_CircularBuffer != null && m_CircularBuffer.Capturing)
            //{
            //    int capturePosition;
            //    int readPosition;
            //    m_CircularBuffer.GetCurrentPosition(out capturePosition, out readPosition);

            //    return
            //        RecordingPCMFormat.ConvertBytesToTime(m_TotalRecordedBytes + capturePosition -
            //                                                m_CircularBufferReadPositon);
            //}

            int length = circularBufferBytesAvailableForReading;

            DebugFix.Assert(length <= incomingPcmData.Length);
            if (length > incomingPcmData.Length)
            {
                length = incomingPcmData.Length;
            }

            if (m_TotalRecordedBytes >= (ulong.MaxValue - (ulong)length))
            {
#if DEBUG
                Debugger.Break();
#endif //DEBUG

                // Oh oh! :(
                if (CurrentState == State.Monitoring)
                {
                    m_TotalRecordedBytes = 0;
#if FORCE_SINGLE_NOTIFICATION_EVENT
                    m_PreviousTotalRecordedBytes = 0;
#endif
                }
            }
            else
            {
                if (CurrentState == State.Recording)
                {
                    if (m_RecordingFileWriter == null)
                    {
                        string parentDir = Path.GetDirectoryName(m_RecordedFilePath);
                        if (!Directory.Exists(parentDir))
                        {
                            Directory.CreateDirectory(parentDir);
                            //FileDataProvider.CreateDirectory(parentDir);
                        }

                        //FileInfo fi = new FileInfo(m_RecordedFilePath);
                        //fi.FullName
                        m_RecordingFileWriter = new BinaryWriter(File.OpenWrite(m_RecordedFilePath));
                    }

                    m_RecordingFileWriter.BaseStream.Position = (long)m_TotalRecordedBytes +
                                                                (long)m_RecordedFileRiffHeaderSize;
                    // m_RecordingFileWriter.BaseStream.Length;
                    m_RecordingFileWriter.Write(incomingPcmData, 0, length);
                }

                m_TotalRecordedBytes += (ulong)length;
            }

            m_CircularBufferReadPositon += length;
            m_CircularBufferReadPositon %= circularBufferBytes;

            PcmDataBufferAvailableHandler del = PcmDataBufferAvailable;
            if (del != null)
            {
                //if (m_PcmDataBuffer.Length != incomingPcmData.Length)
                //{
                //    //Console.WriteLine(string.Format(">>>>> Resizing buffer: m_PcmDataBuffer = {0}, incomingPcmData = {1}",m_PcmDataBuffer.Length, incomingPcmData.Length));

                //    Array.Resize(ref m_PcmDataBuffer, incomingPcmData.Length);
                //}
                //Array.Copy(incomingPcmData, m_PcmDataBuffer, m_PcmDataBuffer.Length);

                int min = Math.Min(m_PcmDataBufferLength, length);

                //Array.Copy(incomingPcmData, m_PcmDataBuffer, min);
                Buffer.BlockCopy(incomingPcmData, 0,
                    m_PcmDataBuffer, 0,
                    min);

                m_PcmDataBufferAvailableEventArgs.PcmDataBuffer = m_PcmDataBuffer;
                m_PcmDataBufferAvailableEventArgs.PcmDataBufferLength = min;
                del(this, m_PcmDataBufferAvailableEventArgs);
            }

            return length;
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

#if FORCE_SINGLE_NOTIFICATION_EVENT
            m_CircularBufferTimer.Stop();
#endif

            Monitor.Enter(LOCK_CIRCULAR_BUFFER);
            try
            {
                m_CircularBuffer.Stop();
            }
            finally
            {
                Monitor.Exit(LOCK_CIRCULAR_BUFFER);
            }

#if FORCE_SINGLE_NOTIFICATION_EVENT
            m_CircularBufferNotificationEvent.Set();
            m_CircularBufferNotificationEvent.Close();
#else
            for (int i = 0; i < NOTIFICATIONS; i++)
            {
                m_CircularBufferNotificationEvents[i].Set();
                m_CircularBufferNotificationEvents[i].Close();
            }
#endif


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

            int numberOfBytesThatHaveBeenRead = 0;
            do
            {
                try
                {
                    numberOfBytesThatHaveBeenRead = circularBufferTransferData(
#if !FORCE_SINGLE_NOTIFICATION_EVENT
-1,
false,
#endif
true
);
                }
                catch (Exception ex)
                {
#if DEBUG
                    Debugger.Break();
#endif
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.StackTrace);
                    break;
                }
#if DEBUG
                if (numberOfBytesThatHaveBeenRead > 0)
                {
                    Console.WriteLine(string.Format("READ buffer bytes (STOP RECORD): {0}", numberOfBytesThatHaveBeenRead));
                }
#endif
            } while (numberOfBytesThatHaveBeenRead > 0);

            if (m_RecordingFileWriter != null)
            {
                // overwrite the existing RIFF header, this time with correct data length
                long length = 0;
                Stream stream = null;
                try
                {
                    stream = m_RecordingFileWriter.BaseStream;
                    stream.Position = 0;
                }
                catch (Exception ex)
                {
#if DEBUG
                    Debugger.Break();
#endif
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.StackTrace);


                    try
                    {
                        m_RecordingFileWriter.Close();
                        m_RecordingFileWriter = null;

                        Thread.Sleep(100);

                        //FileInfo fileInfo = new FileInfo(m_RecordedFilePath);
                        stream = File.OpenWrite(m_RecordedFilePath);
                    }
                    catch (Exception ex2)
                    {
#if DEBUG
                        Debugger.Break();
#endif
                        Console.WriteLine("WTF?!");
                        Console.WriteLine(ex2.Message);
                        Console.WriteLine(ex2.StackTrace);

                        stream = null;
                    }
                }

                if (stream != null)
                {
                    try
                    {
                        length = stream.Length;

                        m_RecordedFileRiffHeaderSize = RecordingPCMFormat.RiffHeaderWrite(
                                                stream,
                                                (uint)
                                                (length -
                                                (long)
                                                m_RecordedFileRiffHeaderSize));
                    }
                    catch (Exception ex3)
                    {
#if DEBUG
                        Debugger.Break();
#endif
                        Console.WriteLine("Erm ?!");
                        Console.WriteLine(ex3.Message);
                        Console.WriteLine(ex3.StackTrace);
                    }
                    finally
                    {
                        if (m_RecordingFileWriter != null)
                        {
                            m_RecordingFileWriter.Close();
                        }
                        else if (stream != null)
                        {
                            stream.Close();
                        }
                    }

                    if (length <= (long)m_RecordedFileRiffHeaderSize // no PCM data, just RIFF header
                        && File.Exists(m_RecordedFilePath))
                    {
                        File.Delete(m_RecordedFilePath);
                        m_RecordedFilePath = null;
                    }
                }
                else
                {
                    try
                    {
                        m_RecordingFileWriter.Close();
                    }
                    catch (Exception ex4)
                    {
#if DEBUG
                        Debugger.Break();
#endif
                        Console.WriteLine("WHAT ?!");
                        Console.WriteLine(ex4.Message);
                        Console.WriteLine(ex4.StackTrace);
                    }
                }
            }

#if !USE_SHARPDX
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