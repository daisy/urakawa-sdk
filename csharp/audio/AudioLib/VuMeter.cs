//#define CLONE_PCM_BUFFER

using System;
using System.Diagnostics;
using System.Collections.Generic;

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
        public ResetHandler ResetEvent;

        private AudioPlayer m_Player;
        private AudioRecorder m_Recorder;

        private byte[] m_PcmDataBuffer;

        public VuMeter(AudioPlayer player, AudioRecorder recorder)
        {
            m_Player = player;
            m_Recorder = recorder;

            m_Player.PcmDataBufferAvailable += new AudioPlayer.PcmDataBufferAvailableHandler(OnPcmDataBufferAvailable_Player);
            m_Recorder.PcmDataBufferAvailable += new AudioRecorder.PcmDataBufferAvailableHandler(OnPcmDataBufferAvailable_Recorder);
            m_Player.StateChanged += new AudioPlayer.StateChangedHandler(StateChanged);
            m_Recorder.StateChanged += new AudioRecorder.StateChangedHandler(StateChanged);
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
            if (del != null
                && m_Player.CurrentState == AudioPlayer.State.Playing )
            {
                m_PeakMeterUpdateEventArgs.PeakDb = peakDb;
                del(this, m_PeakMeterUpdateEventArgs);
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

                    delOverload(this, m_PeakOverloadEventArgs);
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
            if (del != null
                && (m_Recorder.CurrentState == AudioRecorder.State.Monitoring || m_Recorder.CurrentState == AudioRecorder.State.Recording))
            {
                m_PeakMeterUpdateEventArgs.PeakDb = peakDb;
                del(this, m_PeakMeterUpdateEventArgs);
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

                if (PeakMeterOverloaded != null)
                {
                    m_PeakOverloadEventArgs.Channel = index;

                    PeakMeterOverloaded(this, m_PeakOverloadEventArgs);
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

        private void StateChanged(object sender,EventArgs e)
        {
            m_PeakDb  = null;
            if ( m_PcmDataBuffer != null )  m_PcmDataBuffer = new byte[m_PcmDataBuffer.Length];
            m_AverageValue = new double[2];
            ResetHandler del = ResetEvent;
                if (del != null) del ( this, new ResetEventArgs ()) ;
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
                        int arrayIndex = byteOffsetOfFrame + (channelIndex * bytesPerSample) + byteOffsetInSample;

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

        // text meter code
        public event LevelTooLowHandler LevelTooLowEvent;
        public event LevelGoodHandler LevelGoodEvent;


        private double[] m_AverageValue; // array to hold average or RMS value
        public double[] AverageAmplitudeDBValue { get { return m_AverageValue; } }

        double[] TempArray = new double[4];
        int tempCount = 0;
        void AnimationComputation()
        {
            int channels = AudioPCMFormat.NumberOfChannels;
            int blockAlign = AudioPCMFormat.BlockAlign;
            // create an local array and fill the amplitude value of both channels from function
            int[] TempAmpArray = new int[2];
            Array.Copy(AmplitudeValue1(), TempAmpArray, 2);

            //find value in db
            double MaxVal = (int)Math.Pow(2, 8 * (blockAlign / channels)) / 2;


            double left = Convert.ToDouble(TempAmpArray[0]) / MaxVal;
            left = 20 * Math.Log10(left);
            if (left < -90) left = -90;
            double right = Convert.ToDouble(TempAmpArray[1]) / MaxVal;
            right = 20 * Math.Log10(right);
            if (right < -90) right = -90;

            TempArray[tempCount] = left;
            tempCount++;
            if (tempCount >= 4) tempCount = 0;
            m_AverageValue[0] = (TempArray[0] + TempArray[1] + TempArray[2]) / 3;
            m_AverageValue[1] = right;

            DetectLowAmplitude();
        }

        int[] AmplitudeValue1()
        {
            int channels = (int)AudioPCMFormat.NumberOfChannels;
            int blockAlign = (int)AudioPCMFormat.BlockAlign;
            int samplingRate = (int)AudioPCMFormat.SampleRate;
            // average value to return
            int[] arAveragePeaks = new int[2];
            long Left = 0;
            long Right = 0;


            // number of samples from which peak is selected
            uint PeakSampleCount = Convert.ToUInt32(samplingRate / 2000);

            // number of blocks iterated
            uint Count = Convert.ToUInt32(m_PcmDataBuffer.Length / PeakSampleCount);
            if (Count * PeakSampleCount > m_PcmDataBuffer.Length) Count--;

            //System.IO.StreamWriter wr = System.IO.File.AppendText("c:\\2.txt");
            //wr.WriteLine("Count" +  Count.ToString() + "-"+ "PeakSampleCount" + PeakSampleCount.ToString());
            //wr.Close();

            int[] tempArray = new int[2];
            for (uint i = 0; i < Count; i++)
            {
                GetSpeechFragmentPeak(PeakSampleCount, i * PeakSampleCount).CopyTo(tempArray, 0);
                Left += tempArray[0];
                Right += tempArray[1];
            }
            arAveragePeaks[0] = Convert.ToInt32(Left / Count);
            arAveragePeaks[1] = Convert.ToInt32(Right / Count);

            return arAveragePeaks;
        }

        private int[] GetSpeechFragmentPeak(uint FragmentSize, uint StartIndex)
        {
            int channels = (int)AudioPCMFormat.NumberOfChannels;
            int blockAlign = (int)AudioPCMFormat.BlockAlign;
            int[] arPeakVal = new int[2];
            arPeakVal[0] = 0;
            arPeakVal[1] = 0;

            for (int i = 0; i < FragmentSize; i = i + blockAlign)
            {
                int SampleLeft = 0;
                int SampleRight = 0;
                SampleLeft = m_PcmDataBuffer[StartIndex + i];
                if (blockAlign / channels == 2)
                {
                    SampleLeft += m_PcmDataBuffer[StartIndex + i + 1] * 256;

                    if (SampleLeft > 32768)
                        SampleLeft = SampleLeft - 65536;
                }
                if (channels == 2)
                {
                    SampleRight = m_PcmDataBuffer[StartIndex + i + 2];
                    if (blockAlign / channels == 2)
                    {
                        SampleRight += m_PcmDataBuffer[StartIndex + i + 3] * 256;

                        if (SampleRight > 32768)
                            SampleRight = SampleRight - 65536;
                    }
                }

                // Update peak values from fragment
                if (SampleLeft > arPeakVal[0]) arPeakVal[0] = SampleLeft;
                if (SampleRight > arPeakVal[1]) arPeakVal[1] = SampleRight;
            }

            return arPeakVal;
        }

        // calculates the amplitude of both channels from input taken from DirectX buffers
        int[] AmplitudeValue()
        {
            int channels = AudioPCMFormat.NumberOfChannels;
            int blockAlign = (int)AudioPCMFormat.BlockAlign;

            int leftVal = 0;
            int rightVal = 0;
            int tmpLeftVal = 0;
            int tmpRightVal = 0;
            int[] last2LeftVals = new int[] { 0, 1 };
            int[] last2RightVals = new int[] { 0, 1 };
            System.Collections.Generic.Stack<int> leftPeaks = new System.Collections.Generic.Stack<int>();
            System.Collections.Generic.Stack<int> rightPeaks = new System.Collections.Generic.Stack<int>();

            for (int i = 0; i < m_PcmDataBuffer.Length; i += blockAlign)
            {
                switch (blockAlign)
                {
                    case 1:
                        //each byte is a simple sample

                        sbyte curVal = (sbyte)m_PcmDataBuffer[i];
                        tmpLeftVal = (int)curVal * (int.MaxValue / sbyte.MaxValue);
                        break;

                    case 2:
                        switch (channels)
                        {
                            case 1:
                                short sCurVal = BitConverter.ToInt16(m_PcmDataBuffer, i);
                                tmpLeftVal = (int)sCurVal * (int.MaxValue / short.MaxValue);

                                break;
                            case 2:
                                sbyte curValLeft = (sbyte)m_PcmDataBuffer[i];
                                sbyte curValRight = (sbyte)m_PcmDataBuffer[i + 1];

                                tmpLeftVal = (int)curValLeft * (int.MaxValue / sbyte.MaxValue);
                                tmpRightVal = (int)curValRight * (int.MaxValue / sbyte.MaxValue);
                                break;
                            default:
                                break;
                        }
                        break;

                    case 4:
                        short sCurValLeft = BitConverter.ToInt16(m_PcmDataBuffer, i);
                        short sCurValRight = BitConverter.ToInt16(m_PcmDataBuffer, i + 2);
                        tmpLeftVal = (int)sCurValLeft * (int.MaxValue / short.MaxValue);
                        tmpRightVal = (int)sCurValRight * (int.MaxValue / short.MaxValue);
                        break;

                    default:
                        break;
                }
                if (tmpLeftVal == int.MinValue)
                    tmpLeftVal = int.MaxValue;
                if (tmpRightVal == int.MinValue)
                    tmpRightVal = int.MaxValue;

                tmpLeftVal = Math.Abs(tmpLeftVal);
                tmpRightVal = Math.Abs(tmpRightVal);

                //Test if this is a peak?
                if (tmpLeftVal < last2LeftVals[0] && last2LeftVals[0] > last2LeftVals[1])
                    leftPeaks.Push(tmpLeftVal);

                if (tmpLeftVal < last2LeftVals[0] && last2RightVals[0] > last2RightVals[1])
                    rightPeaks.Push(tmpRightVal);

                if (tmpLeftVal != last2LeftVals[0])
                {
                    last2LeftVals[1] = last2LeftVals[0];
                    last2LeftVals[0] = tmpLeftVal;
                }
                if (tmpRightVal != last2RightVals[0])
                {
                    last2RightVals[1] = last2RightVals[0];
                    last2RightVals[0] = tmpRightVal;
                }

            }

            long peakCount = 0;
            long peakSum = 0;
            for (; leftPeaks.Count > 0; peakCount++)
            {
                peakSum += leftPeaks.Pop();
            }
            if (peakCount > 0)
                leftVal = (int)(peakSum / peakCount);

            peakCount = 0;
            peakSum = 0;
            for (; rightPeaks.Count > 0; peakCount++)
            {
                peakSum += rightPeaks.Pop();
            }
            if (peakCount > 0)
                rightVal = (int)(peakSum / peakCount);

            int[] arSum = new int[2];
            arSum[0] = leftVal / (int.MaxValue / 256); // division done to reduce the returned value to be in the range of 0-255
            arSum[1] = rightVal / (int.MaxValue / 256);



            return arSum;
        }

        int LowThresholdCount = 0;
        List<double> LowAmpList = new List<double>();
        bool GoneHigh = false;
        double m_UBound = -28.8;

        private AudioLib.AudioLibPCMFormat AudioPCMFormat
        {
            get
            {
                if (m_Recorder != null && m_Recorder.CurrentState == AudioRecorder.State.Monitoring || m_Recorder.CurrentState == AudioRecorder.State.Recording)
                {
                    return m_Recorder.RecordingPCMFormat;
                }
                else if (m_Player != null && m_Player.CurrentState == AudioPlayer.State.Paused || m_Player.CurrentState == AudioPlayer.State.Playing)
                {
                    return m_Player.CurrentAudioPCMFormat;
                }
                else
                {
                    return null;
                }
            }
        }

        private bool m_IsLowAmplitude;
        public bool IsLevelTooLow { get { return m_IsLowAmplitude; } }

        private void DetectLowAmplitude()
        {
            int channels = AudioPCMFormat.NumberOfChannels;


            double AmplitudeValue;

            if (channels == 1) AmplitudeValue = m_AverageValue[0];
            else AmplitudeValue = (m_AverageValue[0] + m_AverageValue[1]) / 2;

            if (AmplitudeValue < m_LBound)
                LowThresholdCount++;
            else
            {
                GoneHigh = true;
                LowAmpList.Add(AmplitudeValue);
                LowThresholdCount = 0;
            }

            if ((LowThresholdCount >= 4 || LowAmpList.Count > 40)
                && GoneHigh)
            {
                if (LowAmpList.Count > 4)
                {
                    GoneHigh = false;
                    double avg = 0;
                    foreach (double d in LowAmpList)
                        avg += d;

                    avg = avg / LowAmpList.Count;
                    if (avg < m_UBound)
                    {
                        if (LevelTooLowEvent != null) LevelTooLowEvent(this, new LevelTooLowEventArgs(this, avg, 0, 0));
                        m_IsLowAmplitude = true;

                        //Debug_WriteToTextFile("low: " + avg.ToString());
                    }
                    else if (avg > m_UBound)
                    {
                        m_IsLowAmplitude = false;
                        if (LevelGoodEvent != null) LevelGoodEvent(this, new LevelGoodArgs(avg));
                    }
                    LowAmpList.Clear();
                    LowThresholdCount = 0;
                    //Debug_WriteToTextFile(" next ");
                }
                else
                {
                    LowAmpList.Clear();
                    LowThresholdCount = 0;
                }
            }
        }

        void Debug_WriteToTextFile(string s)
        {
            if (System.IO.File.Exists("c:\\222111.txt"))
            {
                try
                {
                    System.IO.StreamWriter wr = System.IO.File.AppendText("c:\\222111.txt");
                    wr.WriteLine(s);
                    wr.Close();
                }
                catch (System.Exception)
                {
                    return;
                }
            }
        }

        // text meter code ends

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

        public delegate void LevelGoodHandler(object sender, LevelGoodArgs e);

        public class LevelGoodArgs : EventArgs
        {
            private double m_AmplitudeLevel;

            public LevelGoodArgs(double Amplitude)
            {
                m_AmplitudeLevel = Amplitude;
            }

            public double AmplitudeValue
            {
                get { return m_AmplitudeLevel; }
            }
        }

        public delegate void LevelTooLowHandler(object sender, LevelTooLowEventArgs e);

        public class LevelTooLowEventArgs : EventArgs
        {
            private object mMeasureInfo;
            private double m_LowLevelValue;
            private double mBytePositionStartOfRange;
            private double mBytePositionEndOfRange;

            public LevelTooLowEventArgs(object measureInfo, double LowLevelValue, double startOfRange, double endOfRange)
            {
                mMeasureInfo = measureInfo;
                m_LowLevelValue = LowLevelValue;
                mBytePositionStartOfRange = startOfRange;
                mBytePositionEndOfRange = endOfRange;
            }

            public object MeasureInfo
            {
                get
                {
                    return mMeasureInfo;
                }
            }

            public double LowLevelValue
            {
                get
                {
                    return m_LowLevelValue;
                }
            }

            public double BytePositionStartOfRange
            {
                get
                {
                    return mBytePositionStartOfRange;
                }
            }

            public double BytePositionEndOfRange
            {
                get
                {
                    return mBytePositionEndOfRange;
                }
            }
        }

        public delegate void ResetHandler(object sender, ResetEventArgs e);
        public class ResetEventArgs : EventArgs
        {
        }

    }
}
