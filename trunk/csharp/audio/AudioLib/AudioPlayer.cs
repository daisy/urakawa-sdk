using System;
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

        private readonly PcmDataBufferAvailableEventArgs m_PcmDataBufferAvailableEventArgs = new PcmDataBufferAvailableEventArgs(new byte[] {0,0,0,0});
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

        private State mState;
        public State CurrentState
        {
            get
            {
                //if (mIsFwdRwd) return State.Playing; else
                return mState;
            }
            set
            {
                if (mState == value)
                {
                    return;
                }

                State oldState = mState;
                mState = value;

                if (StateChanged != null)//mEventsEnabled && 
                    StateChanged(this, new StateChangedEventArgs(oldState));
            }
        }

        public delegate Stream StreamProviderDelegate();

        #region private members


        private StreamProviderDelegate mCurrentAudioStreamProvider;
        private Stream mCurrentAudioStream;
        private double mCurrentAudioDuration;



        long mStartPosition;
        private bool m_KeepStreamAlive;


        private SecondaryBuffer mSoundBuffer;  // DX playback buffer
        private int m_SizeBuffer; // Size of buffer created for playing
        private int m_RefreshLength;         // length of buffer to be refreshed during playing which is half of buffer size
        private long m_lPlayEnd;         // Total length of audio asset being played
        private Thread RefreshThread; // thread for refreshing buffer while playing 


        private byte[] arUpdateVM; // array for update current amplitude to VuMeter
        private int m_UpdateVMArrayLength; // length of VuMeter update array ( may be removed )


        private int m_BufferCheck; // integer to indicate which part of buffer is to be refreshed front or rear, value is odd for refreshing front part and even for refreshing rear
        private long m_lPlayed;         // Length of audio asset in bytes which had been played ( loadded to SoundBuffer )
        private long mPausePosition; // holds pause position in bytes to allow play resume playback from there
        private long m_lResumeToPosition; // In case play ( from, to ) function is used, holds the end position i.e. "to"  for resuming playback


        // The player sometimes reports a bogus position before the current position while playing,
        // so remember where we were last to avoid going backward randomly.
        long mPrevBytePosition;

        private int mBufferStopPosition;       // used by refresh thread for stop position in buffer, value is negetive till refreshing of buffer is going on


        #endregion



        /// <summary>
        /// Create a new player. It doesn't have an output device yet.
        /// </summary>
        public AudioPlayer(bool keepStreamAlive)
        {
            m_KeepStreamAlive = keepStreamAlive;

            CurrentState = State.NotReady;

            m_lResumeToPosition = 0;
            mBufferStopPosition = -1;

            //MoniteringTimer = new System.Windows.Forms.Timer();
            //MoniteringTimer.Tick += new System.EventHandler(this.MoniteringTimer_Tick);
            //MoniteringTimer.Interval = 200;

            //mIsFwdRwd = false;

            //m_IsEndOfAsset = false;

            //mPreviewTimer = new System.Windows.Forms.Timer();
            //mPreviewTimer.Tick += new System.EventHandler(this.PreviewTimer_Tick);
            //mPreviewTimer.Interval = 100;

            //mPlaybackMode = PlaybackMode.Normal;
            //mFwdRwdRate = 0;
            //m_fFastPlayFactor = 1;
            //mEventsEnabled = true;
            //m_IsPreviewing = false;
        }

        /// <summary>
        /// Set the device to be used by the player.
        /// </summary>
        public void SetDevice(Control handle, OutputDevice device)
        {
            if (handle != null)
            {
                device.Device.SetCooperativeLevel(handle, CooperativeLevel.Priority);
            }

            OutputDevice = device;
        }

        /// <summary>
        /// Set the device that matches this name; if it could not be found, default to the first one.
        /// Throw an exception if no devices were found.
        /// </summary>
        public void SetDevice(Control FormHandle, string name)
        {
            List<OutputDevice> devices = OutputDevices;
            OutputDevice found = devices.Find(delegate(OutputDevice d) { return d.Name == name; });
            if (found != null)
            {
                SetDevice(FormHandle, found);
            }
            else if (devices.Count > 0)
            {
                SetDevice(FormHandle, devices[0]);
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

        private OutputDevice mDevice;
        public OutputDevice OutputDevice
        {
            get { return mDevice; }
            set
            {
                mDevice = value;

                CurrentState = State.Stopped;
            }
        }


        private AudioLibPCMFormat mCurrentAudioPCMFormat;
        public AudioLibPCMFormat CurrentAudioPCMFormat
        {
            get { return mCurrentAudioPCMFormat; }
        }


        public void Pause()
        {
            if (!CurrentState.Equals(State.Playing))
            {
                return;
            }

            //if (m_IsPreviewing)
            //    m_IsPreviewing = false;

            mPausePosition = GetCurrentBytePosition();

            m_lResumeToPosition = m_lPlayEnd;

            //if (!mIsFwdRwd)
            //    m_lResumeToPosition = m_lPlayEnd;
            //else
            //    m_lResumeToPosition = 0;

            StopPlayback();

            CurrentState = State.Paused;
               
        }

        public void Resume()
        {
            if (CurrentState.Equals(State.Paused))
            {
                return;
            }

            long lPosition = CalculationFunctions.AdaptToFrame(mPausePosition, mCurrentAudioPCMFormat.BlockAlign);
            long lEndPosition = CalculationFunctions.AdaptToFrame(m_lResumeToPosition, mCurrentAudioPCMFormat.BlockAlign);

            if (lPosition >= 0 && lPosition < mCurrentAudioPCMFormat.GetLengthInBytes(mCurrentAudioDuration))
            {
                mStartPosition = lPosition;
                InitPlay(lPosition, lEndPosition);
            }
            else
                throw new Exception("Start Position is out of bounds of Audio Asset");
        }

        public void Stop()
        {
            if (CurrentState != State.Stopped)
            {
                //if (m_IsPreviewing)
                //    m_IsPreviewing = false;

                StopPlayback();

                mCurrentAudioPCMFormat = null;
                mCurrentAudioDuration = 0;
                mCurrentAudioStreamProvider = null;
            }

            mPausePosition = 0;

            CurrentState = State.Stopped;
        }


        private void StopPlayback()
        {
            //StopForwardRewind();

            mSoundBuffer.Stop();
            if (RefreshThread != null && RefreshThread.IsAlive)
            {
                RefreshThread.Abort();
                Console.WriteLine("Player refresh thread abort.");
            }
            mBufferStopPosition = -1;

            //if (ResetVuMeter != null)
            //    ResetVuMeter(this, new AudioLib.Events.Player.UpdateVuMeterEventArgs());


            if (!m_KeepStreamAlive && mCurrentAudioStream != null)
            {
                mCurrentAudioStream.Close();
                mCurrentAudioStream = null;

                //Reset of the following values only performed in actual Stop() method
                //mCurrentAudioPCMFormat = null;
                //mCurrentAudioDuration = 0;
                //mCurrentAudioStreamProvider = null;
            }
        }

        // position in bytes.
        private long GetCurrentBytePosition()
        {
            int PlayPosition = 0;
            long lCurrentPosition = 0;
            if (mCurrentAudioStream != null &&
                mCurrentAudioPCMFormat.GetLengthInBytes(mCurrentAudioDuration) > 0)
            {//1
                if (CurrentState == State.Playing)
                {//2
                    PlayPosition = mSoundBuffer.PlayPosition;
                    // if refreshing of buffer has finished and player is near end of asset
                    if (mBufferStopPosition != -1)
                    {//3
                        int subtractor = 0;
                        if (mBufferStopPosition >= PlayPosition)
                            subtractor = (mBufferStopPosition - PlayPosition);
                        else
                            subtractor = mBufferStopPosition + (mSoundBuffer.Caps.BufferBytes - PlayPosition);

                        lCurrentPosition = m_lPlayEnd - subtractor;
                    }//-3
                    else if (m_BufferCheck % 2 == 1)
                    {//3
                        // takes the lPlayed position and subtract the part of buffer played from it
                        int subtractor = (2 * m_RefreshLength) - PlayPosition;
                        lCurrentPosition = m_lPlayed - subtractor;
                    }//-3
                    else
                    {//3
                        int subtractor = (3 * m_RefreshLength) - PlayPosition;
                        lCurrentPosition = m_lPlayed - subtractor;
                    }//-3
                    if (lCurrentPosition >= mCurrentAudioPCMFormat.GetLengthInBytes(mCurrentAudioDuration))
                    {//3
                        lCurrentPosition = mCurrentAudioPCMFormat.GetLengthInBytes(mCurrentAudioDuration) -
                            Convert.ToInt32(CalculationFunctions.ConvertTimeToByte(100, (int)mCurrentAudioPCMFormat.SampleRate, mCurrentAudioPCMFormat.BlockAlign));
                    }//-3
                    if (mPrevBytePosition > lCurrentPosition) return mPrevBytePosition;
                    //if (mPrevBytePosition > lCurrentPosition && mFwdRwdRate >= 0) return mPrevBytePosition;

                    mPrevBytePosition = lCurrentPosition;
                }//-2
                else if (CurrentState == State.Paused)
                {//2
                    lCurrentPosition = mPausePosition;
                }//-2

                //if (mFwdRwdRate != 0) lCurrentPosition = m_lChunkStartPosition;

                lCurrentPosition = CalculationFunctions.AdaptToFrame(lCurrentPosition, mCurrentAudioPCMFormat.BlockAlign);
            }//-1
            return lCurrentPosition;
        }



        /// <summary>
        /// When playing, current playback position.
        /// 0 when not playing.
        /// </summary>
        public double CurrentTimePosition
        {
            get
            {
                if (CurrentState == State.Stopped)
                {
                    return 0;
                }

                if (CurrentState == State.Paused)
                {
                    return CalculationFunctions.ConvertByteToTime(mPausePosition,
                                                       (int)mCurrentAudioPCMFormat.SampleRate,
                                                       mCurrentAudioPCMFormat.BlockAlign);
                }

                if (mCurrentAudioStream == null)
                {
                    return 0;
                }
                return CalculationFunctions.ConvertByteToTime(GetCurrentBytePosition(),
                                                       (int)mCurrentAudioPCMFormat.SampleRate,
                                                       mCurrentAudioPCMFormat.BlockAlign);
            }
            set
            {
                if (!(CurrentState != State.Stopped || mCurrentAudioStream != null))
                {
                    return;
                }
                double position = value;

                if (position < 0) position = 0;

                double duration = mCurrentAudioDuration;
                if (position > duration)
                {
                    position = duration;
                }
                //mEventsEnabled = false;
                if (CurrentState == State.Playing)
                {

                    StreamProviderDelegate spd = mCurrentAudioStreamProvider;
                    double dur = mCurrentAudioDuration;
                    AudioLibPCMFormat fmt = mCurrentAudioPCMFormat;

                    Stop();
                    if (!m_KeepStreamAlive)
                    {
                        Thread.Sleep(30);
                    }

                    mCurrentAudioStream = spd();
                    mStartPosition = CalculationFunctions.ConvertTimeToByte(position, (int)fmt.SampleRate, fmt.BlockAlign);
                    //InitPlay (position, 0 );
                    Play(spd, dur, fmt, position);
                }
                else if (CurrentState.Equals(State.Paused))
                {
                    mStartPosition = CalculationFunctions.ConvertTimeToByte(position, (int)mCurrentAudioPCMFormat.SampleRate, mCurrentAudioPCMFormat.BlockAlign);
                    mPausePosition = mStartPosition;
                }
                //mEventsEnabled = true;
            }
        }






        /// <summary>
        ///  Plays an asset from beginning to end
        /// </summary>
        public void Play(StreamProviderDelegate currentAudioStreamProvider, double duration, AudioLibPCMFormat pcmInfo)
        {
            Play(currentAudioStreamProvider, duration, pcmInfo, 0);
        }



        /// <summary>
        ///  Plays an asset from a specified time position its to ends
        /// </summary>
        /// <param name="timeFrom"></param>
        public void Play(StreamProviderDelegate currentAudioStreamProvider, double duration, AudioLibPCMFormat pcmInfo, double timeFrom)
        {
            Play(currentAudioStreamProvider, duration, pcmInfo, timeFrom, 0);
        }


        /// <summary>
        /// Play an asset from a specified time position upto another specified time position
        /// </summary>
        public void Play(StreamProviderDelegate currentAudioStreamProvider, double duration, AudioLibPCMFormat pcmInfo, double from, double to)
        {
            if (currentAudioStreamProvider == null)
            {
                throw new ArgumentNullException("Stream cannot be null !");
            }
            if (duration <= 0)
            {
                throw new ArgumentOutOfRangeException("Duration cannot be <= 0 !");
            }

            mCurrentAudioStreamProvider = currentAudioStreamProvider;
            mCurrentAudioStream = mCurrentAudioStreamProvider();
            mCurrentAudioDuration = duration;
            mCurrentAudioPCMFormat = pcmInfo;

            System.Diagnostics.Debug.Assert(CurrentState == State.Stopped || CurrentState == State.Paused, "Already playing?!");
            if (CurrentState == State.Stopped
                || CurrentState == State.Paused)
            {
                long startPosition = 0;
                if (from > 0)
                {
                    startPosition = CalculationFunctions.ConvertTimeToByte(from, (int)pcmInfo.SampleRate, pcmInfo.BlockAlign);
                    startPosition = CalculationFunctions.AdaptToFrame(startPosition, pcmInfo.BlockAlign);
                }
                long endPosition = 0;
                if (to > 0)
                {
                    endPosition = CalculationFunctions.ConvertTimeToByte(to, (int)pcmInfo.SampleRate, pcmInfo.BlockAlign);
                    endPosition = CalculationFunctions.AdaptToFrame(endPosition, pcmInfo.BlockAlign);
                }
                if (startPosition >= 0 &&
                    (endPosition == 0 || startPosition < endPosition) &&
                    endPosition <= pcmInfo.GetLengthInBytes(duration))
                {
                    InitPlay(startPosition, endPosition);
                }
                else
                {
                    throw new Exception("Start/end positions out of bounds of audio asset.");
                }
            }

        }


        /// <summary>
        ///  convenience function to start playback of an asset
        ///  first initialise player with asset followed by starting playback using PlayAssetStream function
        /// </summary>
        /// <param name="lStartPosition"></param>
        /// <param name="lEndPosition"></param>
        private void InitPlay(long lStartPosition, long lEndPosition)
        {
            if (CurrentState != State.Playing)
            {
                WaveFormat newFormat = new WaveFormat();
                BufferDescription BufferDesc = new BufferDescription();

                newFormat.AverageBytesPerSecond = (int)mCurrentAudioPCMFormat.SampleRate * mCurrentAudioPCMFormat.BlockAlign;
                newFormat.BitsPerSample = Convert.ToInt16(mCurrentAudioPCMFormat.BitDepth);
                newFormat.BlockAlign = Convert.ToInt16(mCurrentAudioPCMFormat.BlockAlign);
                newFormat.Channels = Convert.ToInt16(mCurrentAudioPCMFormat.NumberOfChannels);

                newFormat.FormatTag = WaveFormatTag.Pcm;

                newFormat.SamplesPerSecond = (int)mCurrentAudioPCMFormat.SampleRate;

                // loads  format to buffer description
                BufferDesc.Format = newFormat;

                // enable buffer description properties
                BufferDesc.ControlVolume = true;
                BufferDesc.ControlFrequency = true;

                // calculate size of buffer so as to contain 1 second of audio
                m_SizeBuffer = (int)mCurrentAudioPCMFormat.SampleRate * mCurrentAudioPCMFormat.BlockAlign;
                m_RefreshLength = (int)(mCurrentAudioPCMFormat.SampleRate / 2) * mCurrentAudioPCMFormat.BlockAlign;

                // calculate the size of VuMeter Update array length
                m_UpdateVMArrayLength = m_SizeBuffer / 20; //50ms
                m_UpdateVMArrayLength = Convert.ToInt32(CalculationFunctions.AdaptToFrame(Convert.ToInt32(m_UpdateVMArrayLength), mCurrentAudioPCMFormat.BlockAlign));
                arUpdateVM = new byte[m_UpdateVMArrayLength];
                

                //if (ResetVuMeter != null)
                //    ResetVuMeter(this, new AudioLib.Events.Player.UpdateVuMeterEventArgs());

                // sets the calculated size of buffer
                BufferDesc.BufferBytes = m_SizeBuffer;

                // Global focus is set to true so that the sound can be played in background also
                BufferDesc.GlobalFocus = true;

                // initialising secondary buffer
                // m_SoundBuffer = new SecondaryBuffer(BufferDesc, SndDevice);
                mSoundBuffer = new SecondaryBuffer(BufferDesc, OutputDevice.Device);

                //SetPlayFrequency(m_fFastPlayFactor);

            }// end of state check


            PlayAssetStream(lStartPosition, lEndPosition);

            //if (mFwdRwdRate == 0)
            //{
            //    PlayAssetStream(lStartPosition, lEndPosition);
            //}
            //else if (mFwdRwdRate > 0)
            //{
            //    FastForward(lStartPosition);
            //}
            //else if (mFwdRwdRate < 0)
            //{
            //    if (lStartPosition == 0) lStartPosition = mCurrentAudioPCMFormat.GetLengthInBytes (mCurrentAudioDuration);
            //    Rewind(lStartPosition);
            //}
        }

        /// <summary>
        ///  Called to start playback when player is already initialised with an asset
        ///  Initialises all member variables dependent on asset stream and fill play buffers with data
        /// <see cref=""/>
        /// </summary>
        /// <param name="lStartPosition"></param>
        /// <param name="lEndPosition"></param>
        private void PlayAssetStream(long lStartPosition, long lEndPosition)
        {
            if (CurrentState != State.Playing)
            {
                // Adjust the start and end position according to frame size
                lStartPosition = CalculationFunctions.AdaptToFrame(lStartPosition, mCurrentAudioPCMFormat.BlockAlign);
                lEndPosition = CalculationFunctions.AdaptToFrame(lEndPosition, mCurrentAudioPCMFormat.BlockAlign);

                // lEndPosition = 0 means that file is played to end
                if (lEndPosition != 0)
                {
                    m_lPlayEnd = (lEndPosition); // -lStartPosition;
                }
                else
                {
                    // folowing one line is modified on 2 Aug 2006
                    //m_lPlayEnd = (m_Asset .SizeInBytes  - lStartPosition ) ;
                    m_lPlayEnd = (mCurrentAudioPCMFormat.GetLengthInBytes(mCurrentAudioDuration));
                }

                mPrevBytePosition = lStartPosition;
                // initialize M_lPlayed for this asset
                m_lPlayed = lStartPosition;

                //m_IsEndOfAsset = false;

                mCurrentAudioStream = mCurrentAudioStreamProvider();

                mCurrentAudioStream.Position = lStartPosition;

                mSoundBuffer.Write(0, mCurrentAudioStream, m_SizeBuffer, 0);

                // Adds the length (count) of file played into a variable
                m_lPlayed += m_SizeBuffer;

                CurrentState = State.Playing;
               

                //MoniteringTimer.Enabled = true;

                // starts playing
                try
                {
                    mSoundBuffer.Play(0, BufferPlayFlags.Looping);
                }
                catch (System.Exception ex)
                {
                    EmergencyStopForSoundBufferProblem();
                    //TODO System.Windows.Forms.MessageBox.Show ( string.Format( Localizer.Message ( "Player_TryAgain" ) , "\n\n" , ex.ToString ()) ) ;
                    return;
                }
                m_BufferCheck = 1;

                //initialise and start thread for refreshing buffer
                RefreshThread = new Thread(new ThreadStart(RefreshBuffer));
                RefreshThread.Name = "Player Refresh Thread";
                RefreshThread.Start();

                Console.WriteLine("Player refresh thread start.");

            }
        } // function ends

        /// <summary>
        ///  Thread function which is responsible for refreshing half of sound buffer after every 0.5 second and also for stopping play at end of asset
        /// <see cref=""/>
        /// </summary>
        private void RefreshBuffer()
        {

            int ReadPosition;

            // variable to prevent least count errors in clip end time
            long SafeMargin = CalculationFunctions.ConvertTimeToByte(1, (int)mCurrentAudioPCMFormat.SampleRate, mCurrentAudioPCMFormat.BlockAlign);


            while (m_lPlayed < (m_lPlayEnd - SafeMargin))
            {//1
                if (mSoundBuffer.Status.BufferLost)
                    mSoundBuffer.Restore();


                Thread.Sleep(50);

                if (PcmDataBufferAvailable != null)
                {
                    ReadPosition = mSoundBuffer.PlayPosition;

                    if (ReadPosition < ((m_SizeBuffer) - m_UpdateVMArrayLength))
                    {
                        Array.Copy(mSoundBuffer.Read(ReadPosition, typeof(byte), LockFlag.None, m_UpdateVMArrayLength), arUpdateVM, m_UpdateVMArrayLength);

                        if (PcmDataBufferAvailable != null) //mEventsEnabled == true && 
                        {
                            m_PcmDataBufferAvailableEventArgs.PcmDataBuffer = arUpdateVM;
                            PcmDataBufferAvailable(this, m_PcmDataBufferAvailableEventArgs);
                        }
                    }
                }
                // check if play cursor is in second half , then refresh first half else second
                // refresh front part for odd count
                if ((m_BufferCheck % 2) == 1 && mSoundBuffer.PlayPosition > m_RefreshLength)
                {//2
                    mSoundBuffer.Write(0, mCurrentAudioStream, m_RefreshLength, 0);
                    m_lPlayed = m_lPlayed + m_RefreshLength;
                    m_BufferCheck++;
                }//-1
                // refresh Rear half of buffer for even count
                else if ((m_BufferCheck % 2 == 0) && mSoundBuffer.PlayPosition < m_RefreshLength)
                {//1
                    mSoundBuffer.Write(m_RefreshLength, mCurrentAudioStream, m_RefreshLength, 0);
                    m_lPlayed = m_lPlayed + m_RefreshLength;
                    m_BufferCheck++;
                    // end of even/ odd part of buffer;
                }//-1

                // end of while
            }

            //m_IsEndOfAsset = false;

            int LengthDifference = (int)(m_lPlayed - m_lPlayEnd);
            mBufferStopPosition = -1;
            // if there is no refresh after first load thenrefresh maps directly  
            if (m_BufferCheck == 1)
            {
                mBufferStopPosition = Convert.ToInt32(m_SizeBuffer - LengthDifference);
            }

            // if last refresh is to Front, BufferCheck is even and stop position is at front of buffer.
            else if ((m_BufferCheck % 2) == 0)
            {
                mBufferStopPosition = Convert.ToInt32(m_RefreshLength - LengthDifference);
            }
            else if ((m_BufferCheck > 1) && (m_BufferCheck % 2) == 1)
            {
                mBufferStopPosition = Convert.ToInt32(m_SizeBuffer - LengthDifference);
            }

            int CurrentPlayPosition;
            CurrentPlayPosition = mSoundBuffer.PlayPosition;
            int StopMargin = Convert.ToInt32(CalculationFunctions.ConvertTimeToByte(70, (int)mCurrentAudioPCMFormat.SampleRate, mCurrentAudioPCMFormat.BlockAlign));

            //StopMargin = (int)(StopMargin * m_fFastPlayFactor);

            if (mBufferStopPosition < StopMargin)
                mBufferStopPosition = StopMargin;

            while (CurrentPlayPosition < (mBufferStopPosition - StopMargin) || CurrentPlayPosition > (mBufferStopPosition))
            {
                Thread.Sleep(50);
                CurrentPlayPosition = mSoundBuffer.PlayPosition;
                if (PcmDataBufferAvailable != null)
                {
                    // trigger VuMeter events in this trailing part. Need cleanup, should be placed in another function to avoid duplicacy. But first it should work.
                    if (CurrentPlayPosition < ((m_SizeBuffer) - m_UpdateVMArrayLength))
                    {
                        Array.Copy(mSoundBuffer.Read(CurrentPlayPosition, typeof(byte), LockFlag.None, m_UpdateVMArrayLength), arUpdateVM, m_UpdateVMArrayLength);

                        if (PcmDataBufferAvailable != null) //mEventsEnabled == true && 
                        {
                            m_PcmDataBufferAvailableEventArgs.PcmDataBuffer = arUpdateVM;
                            PcmDataBufferAvailable(this, m_PcmDataBufferAvailableEventArgs);
                        }
                    }
                }
            }


            // Stopping process begins
            mBufferStopPosition = -1;
            mPausePosition = 0;
            mSoundBuffer.Stop();

            //if (ResetVuMeter != null)
            //    ResetVuMeter(this, new AudioLib.Events.Player.UpdateVuMeterEventArgs());

            if (!m_KeepStreamAlive && mCurrentAudioStream != null)
            {
                mCurrentAudioStream.Close();
                mCurrentAudioStream = null;

                //Reset of the following values only performed in actual Stop() method
                //mCurrentAudioPCMFormat = null;
                //mCurrentAudioDuration = 0;
                //mCurrentAudioStreamProvider = null;
            }

            CurrentState = State.Stopped;

            //if (mEventsEnabled)
            //    m_IsEventEnabledDelayedTillTimer = true;
            //else
            //    m_IsEventEnabledDelayedTillTimer = false;

            //m_IsEndOfAsset = true;

            if (AudioPlaybackFinished != null)
                AudioPlaybackFinished(this, new AudioPlaybackFinishEventArgs());

            //PreviewPlaybackStop();
            // RefreshBuffer ends


            Console.WriteLine("Player refresh thread exit.");
        }



        private void EmergencyStopForSoundBufferProblem()
        {
            //if (m_IsPreviewing)
            //    m_IsPreviewing = false;
            ////

            //StopForwardRewind();

            mSoundBuffer.Stop();
            if (RefreshThread != null && RefreshThread.IsAlive)
            {
                RefreshThread.Abort();
                Console.WriteLine("Player refresh thread abort.");
            }
            mBufferStopPosition = -1;

            //if (ResetVuMeter != null)
            //    ResetVuMeter(this, new AudioLib.Events.Player.UpdateVuMeterEventArgs());

            EnsurePlaybackStreamIsDead();

            mPausePosition = 0;

            CurrentState = State.Stopped;


        }


        public void EnsurePlaybackStreamIsDead()
        {
            if (mCurrentAudioStream != null)
            {
                mCurrentAudioStream.Close();
                mCurrentAudioStream = null;
            }

            mCurrentAudioPCMFormat = null;
            mCurrentAudioDuration = 0;
            mCurrentAudioStreamProvider = null;
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

        /// <summary>
        /// Pause from stopped state, in order to reset the pause position after preview.
        /// </summary>
        //public void PauseFromStopped(double time)
        //{
        //    if (State == State.Stopped)
        //    {
        //        m_lResumeToPosition = 0;

        //        State oldState = mState;
        //        mState = State.Paused;
        //        CurrentTimePosition = time;

        //        if (StateChanged != null)//mEventsEnabled && 
        //            StateChanged(this, new StateChangedEventArgs(oldState));
        //    }
        //}

        //private State m_StateBeforePreview;
        //private long m_PreviewStartPosition;


        //// Get a byte position from a time in ms. for the given PCM format info.
        //private long BytePositionFromTime(double time, AudioLibPCMFormat info)
        //{
        //    ushort align = (ushort)info.BlockAlign;
        //    return CalculationFunctions.AdaptToFrame(CalculationFunctions.ConvertTimeToByte(time, (int)info.SampleRate, align), align);
        //}

        //private bool mIsFwdRwd;                // flag indicating forward or rewind playback is going on

        //private void MoniteringTimer_Tick(object sender, EventArgs e)
        //{
        //    if (m_IsEndOfAsset == true)
        //    {
        //        m_IsEndOfAsset = false;
        //        MoniteringTimer.Enabled = false;

        //        if (m_IsEventEnabledDelayedTillTimer)
        //        {
        //            if (AudioPlaybackFinished != null)
        //                AudioPlaybackFinished(this, new Events.Player.EndOfAudioAssetEventArgs());

        //        }
        //        if (mEventsEnabled == true)
        //            m_IsEventEnabledDelayedTillTimer = true;
        //        else
        //            m_IsEventEnabledDelayedTillTimer = false;
        //    }

        //}

        /*
        /// <summary>
        /// Starts a preview playback
        /// playback time returns back to restore time after previewing
        /// end of asset is not triggered after previewing
        /// pause and stop functions work as same as that during normal playback
        /// </summary>
        public void PlayPreview(AudioMediaData asset, double from, double timeTo, double RestoreTime)
        {
            // it is public function so API state will be used
            if (State == State.Stopped || State == State.Paused)
            {
                if (   asset != null  &&    asset.getAudioDuration().TimeDeltaAsMillisecondFloat > 0)
                {
                    long lStartPosition = CalculationFunctions.ConvertTimeToByte(from, (int)pcmInfo.SampleRate, pcmInfo.BlockAlign);
                    lStartPosition = CalculationFunctions.AdaptToFrame(lStartPosition, pcmInfo.BlockAlign);
                    long lEndPosition = CalculationFunctions.ConvertTimeToByte(timeTo, (int)pcmInfo.SampleRate, pcmInfo.BlockAlign);
                    lEndPosition = CalculationFunctions.AdaptToFrame(lEndPosition, pcmInfo.BlockAlign);
                    // check for valid arguments
                    if (lStartPosition < 0) lStartPosition = 0;

                    if (lEndPosition > pcmInfo.GetLengthInBytes(asset.getAudioDuration()))
                        lEndPosition = pcmInfo.GetLengthInBytes(asset.getAudioDuration());

                    if ( mFwdRwdRate == 0  )
                    {
                        m_IsPreviewing = true ;
                        m_StateBeforePreview = State;
                            
                        InitPlay(asset, lStartPosition, lEndPosition);

                        if (RestoreTime >= 0 && RestoreTime < asset.getAudioDuration().TimeDeltaAsMillisecondFloat)
                            m_PreviewStartPosition = CalculationFunctions.ConvertTimeToByte(RestoreTime, (int)mSampleRate, mFrameSize);
                        else
                            m_PreviewStartPosition = 0;

                            mEventsEnabled = false;

                    }
                    else
                        throw new Exception("Start Position is out of bounds of Audio Asset");
                }
                else
                {
                    SimulateEmptyAssetPlaying();
                }
            }
        }
        */


        //private System.Windows.Forms.Timer MoniteringTimer; // monitoring timer to trigger events independent of refresh thread
        //private bool m_IsEventEnabledDelayedTillTimer = true;
        //private int mFrameSize;                       
        //private int mSampleRate;
        //private long m_lChunkStartPosition = 0; // position for starting chunk play in forward/Rewind
        //private bool m_IsEndOfAsset; // variable required to signal monitoring timer to trigger end of asset event, flag is set for a moment and again reset

        //public bool mEventsEnabled;            // flag to temporarily enable or disable events
        //System.Windows.Forms.Timer mPreviewTimer; // timer for playing chunks at interval during Forward/Rewind

        /// <summary>
        /// Set a new playback mode i.e. one of Normal, FastForward, Rewind 
        /// </summary>
        //private void SetPlaybackMode(int rate)
        //{
        //    if (rate != mFwdRwdRate)
        //    {
        //        if (State == State.Playing)
        //        {
        //            long restartPos = GetCurrentBytePosition();
        //            StopPlayback();
        //            mState = State.Paused;
        //            mFwdRwdRate = rate;

        //            InitPlay(restartPos, 0);
        //        }
        //        else if (mState == State.Paused || mState == State.Stopped)
        //        {
        //            mFwdRwdRate = rate;
        //        }
        //    }
        //}
        /// <summary>
        /// Forward / Rewind rate.
        /// 0 for normal playback
        /// negative integer for Rewind
        /// positive integer for FastForward
        /// </summary>
        //public int PlaybackFwdRwdRate
        //{
        //    get { return mFwdRwdRate; }
        //    set { SetPlaybackMode(value); }
        //}


        //  FastForward , Rewind playback modes
        /// <summary>
        ///  Starts playing small chunks of audio while jumping backward in audio assets
        /// </summary>
        /// <param name="lStartPosition"></param>
        //private void Rewind(long lStartPosition)
        //{
        //    // let's play backward!
        //    if (mFwdRwdRate != 0)
        //    {
        //        m_lChunkStartPosition = lStartPosition;
        //        mEventsEnabled = false;
        //        mIsFwdRwd = true;
        //        mPreviewTimer.Interval = 50;
        //        mPreviewTimer.Start();

        //    }
        //}


        /// <summary>
        ///  Starts playing small chunks while jumping forward in audio asset
        /// <see cref=""/>
        /// </summary>
        /// <param name="lStartPosition"></param>
        //private void FastForward(long lStartPosition)
        //{

        //    // let's play forward!
        //    if (mFwdRwdRate != 0)
        //    {
        //        m_lChunkStartPosition = lStartPosition;
        //        mEventsEnabled = false;
        //        mIsFwdRwd = true;
        //        mPreviewTimer.Interval = 50;
        //        mPreviewTimer.Start();
        //    }
        //}



        ///Preview timer tick function
        //private void PreviewTimer_Tick(object sender, EventArgs e)
        //{ //1

        //    double StepInMs = Math.Abs(4000 * mFwdRwdRate);
        //    long lStepInBytes = CalculationFunctions.ConvertTimeToByte(StepInMs, (int)mCurrentAudioPCMFormat.SampleRate, mCurrentAudioPCMFormat.BlockAlign);
        //    int PlayChunkLength = 1200;
        //    long lPlayChunkLength = CalculationFunctions.ConvertTimeToByte(PlayChunkLength, (int)mCurrentAudioPCMFormat.SampleRate, mCurrentAudioPCMFormat.BlockAlign);
        //    mPreviewTimer.Interval = PlayChunkLength + 50;

        //long PlayStartPos = 0;
        //long PlayEndPos = 0;
        //if (mFwdRwdRate > 0)
        //{ //2
        //    if ((mCurrentAudioPCMFormat.GetLengthInBytes (mCurrentAudioDuration) - (lStepInBytes + m_lChunkStartPosition)) > lPlayChunkLength)
        //    { //3
        //        if (m_lChunkStartPosition > 0)
        //        {
        //            m_lChunkStartPosition += lStepInBytes;
        //        }
        //        else
        //            m_lChunkStartPosition = mCurrentAudioPCMFormat.BlockAlign;

        //        PlayStartPos = m_lChunkStartPosition;
        //        PlayEndPos = m_lChunkStartPosition + lPlayChunkLength;
        //        PlayAssetStream(PlayStartPos, PlayEndPos);

        //        if (m_lChunkStartPosition > mCurrentAudioPCMFormat.GetLengthInBytes(mCurrentAudioDuration))
        //            m_lChunkStartPosition = mCurrentAudioPCMFormat.GetLengthInBytes(mCurrentAudioDuration);
        //    } //-3
        //    else
        //    { //3
        //        Stop();
        //        if (mEventsEnabled && AudioPlaybackFinished != null)
        //            AudioPlaybackFinished(this, new Events.Player.EndOfAudioAssetEventArgs());
        //    } //-3
        //} //-2
        //else if (mFwdRwdRate < 0)
        //{ //2
        //    //if (m_lChunkStartPosition > (lStepInBytes ) && lPlayChunkLength <= m_Asset.getPCMLength () )
        //    if (m_lChunkStartPosition > 0)
        //    { //3
        //        if (m_lChunkStartPosition < mCurrentAudioPCMFormat.GetLengthInBytes(mCurrentAudioDuration))
        //            m_lChunkStartPosition -= lStepInBytes;
        //        else
        //            m_lChunkStartPosition = mCurrentAudioPCMFormat.GetLengthInBytes(mCurrentAudioDuration) - lPlayChunkLength;

        //        PlayStartPos = m_lChunkStartPosition;
        //        PlayEndPos = m_lChunkStartPosition + lPlayChunkLength;
        //        PlayAssetStream(PlayStartPos, PlayEndPos);

        //        if (m_lChunkStartPosition < 0)
        //            m_lChunkStartPosition = 0;
        //    } //-3
        //    else
        //    {
        //        Stop();
        //        if (mEventsEnabled && AudioPlaybackFinished != null)
        //            AudioPlaybackFinished(this, new Events.Player.EndOfAudioAssetEventArgs());
        //    }
        //} //-2
        //} //-1




        /// <summary>
        /// Stop rewinding or forwarding, including the preview timer.
        /// </summary>
        //private void StopForwardRewind()
        //{
        //    if (mFwdRwdRate != 0 || mPreviewTimer.Enabled)
        //    {
        //        mPreviewTimer.Enabled = false;
        //        //m_FwdRwdRate = 0 ;
        //        m_lChunkStartPosition = 0;
        //        mIsFwdRwd = false;
        //        mEventsEnabled = true;
        //    }
        //}

        // Member variables changed by user 
        //private int m_VolumeLevel;
        //private int mFwdRwdRate; // holds skip time multiplier for forward / rewind mode , value is 0 for normal playback,  positive  for FastForward and negetive  for Rewind


        //private bool m_IsPreviewing; // Is true when playback is used for previewing a  selection or marking.

        /// <summary>
        /// Indicate if playback is previewing
        /// </summary>
        //public bool IsPreviewing { get { return m_IsPreviewing; } }

        //private void PreviewPlaybackStop()
        //{
        //    if (m_IsPreviewing)
        //    {
        //        m_IsPreviewing = false;
        //        m_IsEndOfAsset = false;
        //        mEventsEnabled = true;

        //        if (m_StateBeforePreview == State.Paused)
        //        {
        //            StateChangedEventArgs e = new AudioLib.StateChangedEventArgs(State.Playing);
        //            mState = State.Paused;
        //            mPausePosition = m_PreviewStartPosition;
        //            TriggerStateChangedEvent(e);
        //        }
        //        else if (m_StateBeforePreview == State.Stopped)
        //        {
        //            StateChangedEventArgs e = new AudioLib.StateChangedEventArgs(State.Playing);
        //            TriggerStateChangedEvent(e);
        //        }
        //    }
        //}

        //public void PlaySimulateEmpty()
        //{
        //    StateChangedEventArgs e = new StateChangedEventArgs(mState);
        //    mState = State.Playing;
        //    TriggerStateChangedEvent(e);


        //    Thread.Sleep(50);

        //    e = new StateChangedEventArgs(mState);
        //    mState = State.Stopped;
        //    TriggerStateChangedEvent(e);

        //    // trigger end of asset event
        //    if (mEventsEnabled == true && AudioPlaybackFinished != null)
        //        AudioPlaybackFinished(this, new Events.Player.EndOfAudioAssetEventArgs());
        //    //            System.Media.SystemSounds.Asterisk.Play();
        //}

        //private float m_fFastPlayFactor;
        //public float FastPlayFactor
        //{
        //    get
        //    {
        //        return m_fFastPlayFactor;
        //    }
        //    set
        //    {
        //        SetPlayFrequency(value);
        //    }
        //}

        //void SetPlayFrequency(float l_frequency)
        //{
        //    if (mSoundBuffer != null
        //        && mFwdRwdRate == 0)
        //    {
        //        try
        //        {
        //            mSoundBuffer.Frequency = (int)(mSoundBuffer.Format.SamplesPerSecond * l_frequency);
        //            m_fFastPlayFactor = l_frequency;
        //        }
        //        catch (System.Exception Ex)
        //        {
        //            MessageBox.Show("Unable to change fastplay rate " + Ex.ToString());
        //        }
        //    }
        //    else
        //        m_fFastPlayFactor = l_frequency;
        //}


        //public int OutputVolume
        //{
        //    get
        //    {
        //        return m_VolumeLevel;
        //    }
        //    set
        //    {
        //        SetVolumeLevel(value);
        //    }
        //}
        //void SetVolumeLevel(int VolLevel)
        //{
        //    m_VolumeLevel = VolLevel;

        //    if (mSoundBuffer != null)
        //        mSoundBuffer.Volume = m_VolumeLevel;

        //}

    }
}
