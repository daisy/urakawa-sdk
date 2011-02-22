//#define CLONE_PCM_BUFFER

using System;
using System.Diagnostics;

namespace AudioLib
{
    // This class underwent a major cleanup and simplification at revision 1486.
    // See:
    // http://daisy.trac.cvsdude.com/urakawa-sdk/changeset/1486#file6
    // Just in case we need to restore some functionality:
    // http://daisy.trac.cvsdude.com/urakawa-sdk/browser/trunk/csharp/audio/AudioLib/VuMeter.cs?rev=1485
    public class VuMeter
    {
        //TODO: implement low/high boundaries !!
        private double m_LBound = -36;
        public enum NoiseLevelSelection { High, Low, Medium } ;
        private NoiseLevelSelection m_NoiseLevel;
        public NoiseLevelSelection NoiseLevel
        {
            get { return m_NoiseLevel; }
            set
            {
                m_NoiseLevel = value;

                if (value == NoiseLevelSelection.Low)
                {
                    m_LBound = -37.5;
                }
                else if (value == NoiseLevelSelection.Medium)
                {
                    m_LBound = -36;
                }
                else if (value == NoiseLevelSelection.High)
                {
                    m_LBound = -33.5;
                }
            }
        }

        private readonly PeakOverloadEventArgs m_PeakOverloadEventArgs = new PeakOverloadEventArgs(1); //, 0);
        public event PeakOverloadHandler PeakMeterOverloaded;

        private readonly PeakMeterUpdateEventArgs m_PeakMeterUpdateEventArgs = new PeakMeterUpdateEventArgs(new double[] { -1, -1 });
        public event PeakMeterUpdateHandler PeakMeterUpdated;

        private AudioPlayer m_Player;
        private AudioRecorder m_Recorder;

        private byte[] m_PcmDataBuffer;

        public VuMeter(AudioPlayer player, AudioRecorder recorder)
        {
            m_Player = player;
            m_Recorder = recorder;

            m_Player.PcmDataBufferAvailable += new AudioPlayer.PcmDataBufferAvailableHandler(OnPcmDataBufferAvailable_Player);
            m_Recorder.PcmDataBufferAvailable += new AudioRecorder.PcmDataBufferAvailableHandler(OnPcmDataBufferAvailable_Recorder);
        }

        public void OnPcmDataBufferAvailable_Player(object sender, AudioPlayer.PcmDataBufferAvailableEventArgs e)
        {
            AudioPlayer ob_AudioPlayer = sender as AudioPlayer;
            if (ob_AudioPlayer == null || ob_AudioPlayer != m_Player)
            {
                Debug.Fail("This should never happen !!!?");
                return;
            }
#if CLONE_PCM_BUFFER
            if (m_PcmDataBuffer == null || m_PcmDataBuffer.Length != e.PcmDataBuffer.Length)
            {
                Console.WriteLine("*** creating m_Player buffer");
                m_PcmDataBuffer = new byte[e.PcmDataBuffer.Length];
            }
            Array.Copy(e.PcmDataBuffer, m_PcmDataBuffer, e.PcmDataBuffer.Length);
#else
            m_PcmDataBuffer = e.PcmDataBuffer;
#endif

            double[] peakDb = computePeakDb(m_Player.CurrentAudioPCMFormat);

            PeakMeterUpdateHandler del = PeakMeterUpdated;
            if (del != null)
            {
                m_PeakMeterUpdateEventArgs.PeakDb = peakDb;
                del (this, m_PeakMeterUpdateEventArgs);
            }
            //var del_ = PeakMeterUpdated;
            //if (del_ != null)
            //{
                //m_PeakMeterUpdateEventArgs.PeakDb = peakDb;
                //del_(this, m_PeakMeterUpdateEventArgs);
            //}

            int index = 0;
            foreach (double peak in peakDb)
            {
                index++;
                if (peak < 0)
                {
                    continue;
                }

                PeakOverloadHandler delOverload = PeakMeterOverloaded;
                if (del != null)
                {
                    m_PeakOverloadEventArgs.Channel = index;

                    delOverload (this, m_PeakOverloadEventArgs);
                }
                //var del = PeakMeterOverloaded;
                //if (del != null)
                //{
                    //m_PeakOverloadEventArgs.Channel = index;
                    //m_PeakOverloadEventArgs.Time = m_Player.CurrentTimeInLocalUnit;
                    //del(this, m_PeakOverloadEventArgs);
                //}
            }
        }

        public void OnPcmDataBufferAvailable_Recorder(object sender, AudioRecorder.PcmDataBufferAvailableEventArgs e)
        {
            AudioRecorder ob_AudioRecorder = sender as AudioRecorder;
            if (ob_AudioRecorder == null || ob_AudioRecorder != m_Recorder)
            {
                Debug.Fail("This should never happen !!!?");
                return;
            }
#if CLONE_PCM_BUFFER
            if (m_PcmDataBuffer == null || m_PcmDataBuffer.Length != e.PcmDataBuffer.Length)
            {
                Console.WriteLine("*** creating m_Recorder buffer");
                m_PcmDataBuffer = new byte[e.PcmDataBuffer.Length];
            }
            Array.Copy(e.PcmDataBuffer, m_PcmDataBuffer, e.PcmDataBuffer.Length);
#else
            m_PcmDataBuffer = e.PcmDataBuffer;
#endif

            double[] peakDb = computePeakDb(m_Recorder.RecordingPCMFormat);

            PeakMeterUpdateHandler del = PeakMeterUpdated;
            if (del != null)
            {
                m_PeakMeterUpdateEventArgs.PeakDb = peakDb;
                del (this, m_PeakMeterUpdateEventArgs);
            }
            //var del_ = PeakMeterUpdated;
            //if (del_ != null)
            //{
                //m_PeakMeterUpdateEventArgs.PeakDb = peakDb;
                //del_(this, m_PeakMeterUpdateEventArgs);
            //}

            int index = 0;
            foreach (double peak in peakDb)
            {
                index++;
                if (peak < 0)
                {
                    continue;
                }

                if (PeakMeterOverloaded  != null)
                {
                    m_PeakOverloadEventArgs.Channel = index;

                    PeakMeterOverloaded (this, m_PeakOverloadEventArgs);
                }
                //var del = PeakMeterOverloaded;
                //if (del != null)
                //{
                    //m_PeakOverloadEventArgs.Channel = index;
                    //m_PeakOverloadEventArgs.Time = m_Recorder.CurrentDurationInLocalUnits;
                    //del(this, m_PeakOverloadEventArgs);
                //}
            }
        }

        public double[] LastPeakDb
        {
            get { return m_PeakDb; }
        }

        private double[] m_PeakDb; //to avoid re-allocating the buffer when not necessary
        private double[] computePeakDb(AudioLibPCMFormat pcmFormat)
        {
            if (m_PeakDb == null || m_PeakDb.Length != pcmFormat.NumberOfChannels)
            {
                Console.WriteLine("*** creating PeakDbValue buffer");
                m_PeakDb = new double[pcmFormat.NumberOfChannels];
            }

            bool allZeros = true;
            for (int i = 0; i < m_PcmDataBuffer.Length; i++)
            {
                if (m_PcmDataBuffer[i] != 0)
                {
                    allZeros = false;
                    break;
                }
            }
            if (allZeros)
            {
                for (int i = 0; i < m_PeakDb.Length; i++)
                {
                    m_PeakDb[i] = Double.PositiveInfinity;
                }
                return m_PeakDb;
            }

            double full = Math.Pow(2, pcmFormat.BitDepth);
            double halfFull = full / 2;

            int bytesPerSample = pcmFormat.BitDepth / 8;

            for (int byteOffsetOfFrame = 0; byteOffsetOfFrame < m_PcmDataBuffer.Length; byteOffsetOfFrame += pcmFormat.BlockAlign)
            {
                for (int channelIndex = 0; channelIndex < pcmFormat.NumberOfChannels; channelIndex++)
                {
                    double val = 0;
                    for (int byteOffsetInSample = 0; byteOffsetInSample < bytesPerSample; byteOffsetInSample++)
                    {
                        int arrayIndex = byteOffsetOfFrame + (channelIndex*bytesPerSample) + byteOffsetInSample;
                        
                        val += Math.Pow(2, 8 * byteOffsetInSample) * m_PcmDataBuffer[arrayIndex];
                    }

                    if (val > halfFull)
                    {
                        val = full - val;
                    }

                    if (m_PeakDb[channelIndex] < val)
                    {
                        m_PeakDb[channelIndex] = val;
                    }
                }
            }

            for (int channelIndex = 0; channelIndex < pcmFormat.NumberOfChannels; channelIndex++)
            {
                m_PeakDb[channelIndex] = 20 * Math.Log10(m_PeakDb[channelIndex] / halfFull);
            }

            return m_PeakDb;
        }

        public delegate void PeakOverloadHandler(object sender, PeakOverloadEventArgs e);

        public class PeakOverloadEventArgs : EventArgs
        {
            private int m_Channel; //1, 2, etc.
            public int Channel
            {
                get
                {
                    return m_Channel;
                }
                set
                {
                    m_Channel = value;
                }
            }

            //private double m_Time;
            //public double Time
            //{
            //    get
            //    {
            //        return m_Time;
            //    }
            //    set
            //    {
            //        m_Time = value;
            //    }
            //}

            public PeakOverloadEventArgs(int channel) // double time
            {
                m_Channel = channel;
                //m_Time = time;
            }
        }

        public delegate void PeakMeterUpdateHandler(object sender, PeakMeterUpdateEventArgs e);

        public class PeakMeterUpdateEventArgs
        {
            private double[] m_PeakDb; // in decibels, one value per channel
            public double[] PeakDb
            {
                get
                {
                    return m_PeakDb;
                }
                set
                {
                    m_PeakDb = value;
                }
            }

            public PeakMeterUpdateEventArgs(double[] Values)
            {
                m_PeakDb = Values;
            }
        }
    }
}
