using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
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
    public partial class AudioPlayer
    {
        private void initPreviewTimer()
        {
            //mPreviewTimer.Enabled = false;
            mPreviewTimer.Stop();
            mPreviewTimer.Interval = REFRESH_INTERVAL_MS;
            mPreviewTimer.Tick += new EventHandler(PreviewTimer_Tick);
            //mPreviewTimer.Elapsed += new System.Timers.ElapsedEventHandler(PreviewTimer_Tick);

            //mPreviewTimer.AutoReset = true;
        }

        // timer for playing chunks at interval during Forward/Rewind 
        System.Windows.Forms.Timer mPreviewTimer = new System.Windows.Forms.Timer();
        //System.Timers.Timer mPreviewTimer = new System.Timers.Timer();

        private int m_FwdRwdRate; // holds skip time multiplier for forward / rewind mode , value is 0 for normal playback,  positive  for FastForward and negetive  for Rewind

        /// <summary>
        /// Forward / Rewind rate.
        /// 0 for normal playback
        /// negative integer for Rewind
        /// positive integer for FastForward
        /// </summary>
        public int PlaybackFwdRwdRate
        {
            get { return m_FwdRwdRate; }
            set { SetPlaybackMode(value); }
        }


        /// <summary>
        /// Set a new playback mode i.e. one of Normal, FastForward, Rewind 
        /// </summary>
        /// <param name="mode">The new mode.</param>
        private void SetPlaybackMode(int rate)
        {
            if (rate != m_FwdRwdRate)
            {
                if (CurrentState == State.Playing)
                {
                    //            if (mFwdRwdRate != 0 || mPreviewTimer.Enabled)
                    //            {
                    //                //mPreviewTimer.Enabled = false;
                    //                mPreviewTimer.Stop();
                    //                //m_FwdRwdRate = 0 ;
                    //                m_lChunkStartPosition = 0;
                    ////                mIsFwdRwd = false;
                    //                mEventsEnabled = true;
                    //            }

                    if (rate == 0)
                    {
                        Pause();
                        //FastPlayFactor = 1;
                        m_FwdRwdRate = rate;
                        Thread.Sleep(10);
                        Resume();
                    }
                    else
                    {
                        long restartPos = CurrentBytePosition;

                        m_ResumeStartPosition = restartPos;

                        CurrentState = State.Paused; // before stopPlayback(), doesn't kill the stream provider
                        stopPlayback();

                        m_FwdRwdRate = rate;

                        //InitPlay(mCurrentAudio, restartPos, 0);
                        //startPlayback(restartPos, m_CurrentAudioStream.Length );
                        //startPlayback(restartPos, m_CurrentAudioDataLength);
                        if (m_FwdRwdRate > 0)
                        {
                            FastForward(restartPos);
                        }
                        else if (m_FwdRwdRate < 0)
                        {
                            if (restartPos == 0) restartPos = m_CurrentAudioStream.Length;
                            Rewind(restartPos);
                        }
                    }
                }
                else if (CurrentState == State.Paused || CurrentState == State.Stopped)
                {
                    m_FwdRwdRate = rate;
                }
            }
        }
        //private bool mEventsEnabled = true;
        private long m_lChunkStartPosition = 0; // position for starting chunk play in forward/Rewind
        //private bool mIsFwdRwd ;                // flag indicating forward or rewind playback is going on

        //  FastForward , Rewind playback modes
        /// <summary>
        ///  Starts playing small chunks of audio while jumping backward in audio asset
        /// <see cref=""/>
        /// </summary>
        /// <param name="lStartPosition"></param>
        private void Rewind(long lStartPosition)
        {
            // let's play backward!
            if (m_FwdRwdRate != 0)
            {

                m_lChunkStartPosition = lStartPosition;
                //mEventsEnabled = false;

                //                mIsFwdRwd = true;
                mPreviewTimer.Interval = REFRESH_INTERVAL_MS;
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
            if (m_FwdRwdRate != 0)
            {

                m_lChunkStartPosition = lStartPosition;
                //mEventsEnabled = false;

                //                mIsFwdRwd = true;
                mPreviewTimer.Interval = REFRESH_INTERVAL_MS;
                mPreviewTimer.Start();
            }
        }



        ///Preview timer tick function
        private void PreviewTimer_Tick(object sender, EventArgs e)
        { //1

            if (m_CurrentAudioPCMFormat == null)
                return;

            double StepInMs = Math.Abs(4000 * m_FwdRwdRate);
            //long lStepInBytes = CalculationFunctions.ConvertTimeToByte(StepInMs, (int)mCurrentAudio.getPCMFormat().getSampleRate(), mCurrentAudio.getPCMFormat().getBlockAlign());
            long lStepInBytes = m_CurrentAudioPCMFormat.ConvertTimeToBytes(Convert.ToInt64(StepInMs * AudioLibPCMFormat.TIME_UNIT));
            int PlayChunkLength = 1200;
            //long lPlayChunkLength = CalculationFunctions.ConvertTimeToByte(PlayChunkLength, (int)mCurrentAudio.getPCMFormat().getSampleRate(), mCurrentAudio.getPCMFormat().getBlockAlign());
            long lPlayChunkLength = m_CurrentAudioPCMFormat.ConvertTimeToBytes(Convert.ToInt64(PlayChunkLength * AudioLibPCMFormat.TIME_UNIT));
            if (m_CurrentAudioDataLength < lPlayChunkLength)
            {
                lPlayChunkLength = m_CurrentAudioDataLength;
                PlayChunkLength = Convert.ToInt32(m_CurrentAudioPCMFormat.ConvertBytesToTime(lPlayChunkLength) / AudioLibPCMFormat.TIME_UNIT);

            }

            //Console.WriteLine("play chunk length " + PlayChunkLength + " : " + lPlayChunkLength);

            //lPlayChunkLength = m_CurrentAudioPCMFormat.AdjustByteToBlockAlignFrameSize (PlayChunkLength) ;
            mPreviewTimer.Interval = PlayChunkLength + REFRESH_INTERVAL_MS;

            //Console.WriteLine("mPreviewTimer.Interval " + mPreviewTimer.Interval);

            //System.Media.SystemSounds.Asterisk.Play();
            long PlayStartPos = 0;
            long PlayEndPos = 0;
            if (m_FwdRwdRate > 0)
            { //2
                //Console.WriteLine("rate is above 0");
                //if ((mCurrentAudio.getPCMLength() - (lStepInBytes + m_lChunkStartPosition)) > lPlayChunkLength)
                //Console.WriteLine("m_CurrentAudioStream.Length " + m_CurrentAudioStream.Length + ", lStepInBytes :" + lStepInBytes + ",  m_lChunkStartPosition :" + m_lChunkStartPosition + ", lPlayChunkLength :" + lPlayChunkLength);

                if ((m_CurrentAudioDataLength - (m_lChunkStartPosition)) >= lPlayChunkLength)
                { //3
                    if (m_lChunkStartPosition > 0)
                    {
                        Console.WriteLine("m_lChunkStartPosition  , step in bytes : " + m_lChunkStartPosition + " : " + lStepInBytes);
                        m_lChunkStartPosition += lStepInBytes;

                    }
                    else
                        //m_lChunkStartPosition = mFrameSize;
                        m_lChunkStartPosition = m_CurrentAudioPCMFormat.BlockAlign;

                    PlayStartPos = m_lChunkStartPosition;
                    PlayEndPos = m_lChunkStartPosition + lPlayChunkLength;
                    //PlayAssetStream(PlayStartPos, PlayEndPos);
                    //Console.WriteLine("play pos " + PlayStartPos + " : " + PlayEndPos);
                    //mEventsEnabled = true;
                    if (CurrentState == State.Playing)
                    {
                        //stopPlayback();
                        //m_State = State.Paused;
                    }
                    if (CurrentState != State.Playing) startPlayback(PlayStartPos, PlayEndPos);

                    //mEventsEnabled = false;
                    //Console.WriteLine(CurrentState);
                    if (m_lChunkStartPosition > m_CurrentAudioStream.Length)
                        m_lChunkStartPosition = m_CurrentAudioStream.Length;
                } //-3
                else
                { //3
                    Stop();
                    AudioPlaybackFinishHandler delFinished = AudioPlaybackFinished;
                    if (!mPreviewTimer.Enabled
                        && delFinished != null)
                    {
                        delFinished(this, new AudioPlaybackFinishEventArgs());
                        //EndOfAudioAsset(this, new Events.Audio.Player.EndOfAudioAssetEventArgs());
                    }

                } //-3
            } //-2
            else if (m_FwdRwdRate < 0)
            { //2
                //if (m_lChunkStartPosition > (lStepInBytes ) && lPlayChunkLength <= m_Asset.getPCMLength () )
                //Console.WriteLine("rewind " + m_lChunkStartPosition);
                if (m_lChunkStartPosition > 0)
                { //3
                    if (m_lChunkStartPosition < m_CurrentAudioStream.Length)
                    {
                        m_lChunkStartPosition -= lStepInBytes;
                        if (m_lChunkStartPosition < lPlayChunkLength) m_lChunkStartPosition = 0;
                    }
                    else
                        m_lChunkStartPosition = m_CurrentAudioStream.Length - lPlayChunkLength;

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
                    if (!mPreviewTimer.Enabled
                        && delFinished != null)
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
            if (//mFwdRwdRate != 0 ||
                mPreviewTimer.Enabled)
            {
                //mPreviewTimer.Enabled = false;
                mPreviewTimer.Stop();
                //m_FwdRwdRate = 0 ;
                m_lChunkStartPosition = 0;
                //                mIsFwdRwd = false;
                //mEventsEnabled = true;
            }
        }


        //private System.Windows.Forms.Timer m_MonitoringTimer;


        //private bool m_FinishedPlayingCurrentStream = false;
        //private void m_MonitoringTimer_Tick(object sender, EventArgs e)
        //{
        //    //Console.WriteLine("monitoring ");
        //    if (AllowBackToBackPlayback &&  m_FinishedPlayingCurrentStream)
        //    {
        //        if (m_MonitoringTimer != null) m_MonitoringTimer.Stop();
        //        m_FinishedPlayingCurrentStream = false;
        //        AudioPlaybackFinishHandler delFinished = AudioPlaybackFinished;
        //        if (delFinished != null && mEventsEnabled)
        //            delFinished(this, new AudioPlaybackFinishEventArgs());

        //    }
        //}



        //public bool AllowBackToBackPlayback
        //{
        //    get { return m_AllowBackToBackPlayback; }
        //    set
        //    {
        //        if (value)
        //        {
        //            if (m_MonitoringTimer == null) m_MonitoringTimer = new System.Windows.Forms.Timer();
        //            m_MonitoringTimer.Tick += new EventHandler(m_MonitoringTimer_Tick);
        //            m_MonitoringTimer.Interval = 250;
        //            m_MonitoringTimer.Enabled = false;
        //        }
        //        else if ( m_MonitoringTimer != null )
        //        {
        //            m_MonitoringTimer.Enabled = false;
        //            m_MonitoringTimer.Tick -= new EventHandler(m_MonitoringTimer_Tick);
        //            m_MonitoringTimer.Dispose();
        //            m_MonitoringTimer = null;

        //        }
        //        m_AllowBackToBackPlayback = value;
        //    }
        //}
    }
}
