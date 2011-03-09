using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace AudioLib
{
     public partial class VuMeter
    {

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

        
        private bool m_EnableAudioLevelAlerts = false;
        /// <summary>
        /// Enables computations and firing of level low, level good and high events.
        /// </summary>
        public bool EnableAudioLevelAlerts
        {
            get { return m_EnableAudioLevelAlerts; }
            set { m_EnableAudioLevelAlerts = value; }
        }

        public event LevelTooLowHandler LevelTooLowEvent;
        public event LevelGoodHandler LevelGoodEvent;


        private double[] m_AverageValue; // array to hold average or RMS value
        public double[] AverageAmplitudeDBValue { get { return m_AverageValue; } }

        double[] m_TempArray = new double[4];
        int m_TempCount = 0;
        private void ComputeAverageValues(AudioLibPCMFormat audioPCMFormat)
        {
            int channels = audioPCMFormat.NumberOfChannels;
            int blockAlign = audioPCMFormat.BlockAlign;
            // create an local array and fill the amplitude value of both channels from function
            int[] TempAmpArray = new int[2];
            Array.Copy(GetAverageOfPeaksInSpeechFragment(audioPCMFormat), TempAmpArray, 2);

            //find value in db
            double MaxVal = (int)Math.Pow(2, 8 * (blockAlign / channels)) / 2;

            double left = Convert.ToDouble(TempAmpArray[0]) / MaxVal;
            left = 20 * Math.Log10(left);
            if (left < -90) left = -90;
            double right = Convert.ToDouble(TempAmpArray[1]) / MaxVal;
            right = 20 * Math.Log10(right);
            if (right < -90) right = -90;

            m_TempArray[m_TempCount] = left;
            m_TempCount++;
            if (m_TempCount >= 4) m_TempCount = 0;
            m_AverageValue[0] = (m_TempArray[0] + m_TempArray[1] + m_TempArray[2]) / 3;
            m_AverageValue[1] = right;

            DetectLowAmplitude(audioPCMFormat);
        }

        int[] GetAverageOfPeaksInSpeechFragment(AudioLibPCMFormat audioPCMFormat)
        {
            int channels = (int)audioPCMFormat.NumberOfChannels;
            int blockAlign = (int)audioPCMFormat.BlockAlign;
            int samplingRate = (int)audioPCMFormat.SampleRate;
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
                GetSpeechFragmentPeak(PeakSampleCount, i * PeakSampleCount, audioPCMFormat).CopyTo(tempArray, 0);
                Left += tempArray[0];
                Right += tempArray[1];
            }
            arAveragePeaks[0] = Convert.ToInt32(Left / Count);
            arAveragePeaks[1] = Convert.ToInt32(Right / Count);

            return arAveragePeaks;
        }

        private int[] GetSpeechFragmentPeak(uint FragmentSize, uint StartIndex, AudioLibPCMFormat audioPCMFormat)
        {
            int channels = (int)audioPCMFormat.NumberOfChannels;
            int blockAlign = (int)audioPCMFormat.BlockAlign;
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
        int[] AmplitudeValue(AudioLibPCMFormat audioPCMFormat)
        {
            int channels = audioPCMFormat.NumberOfChannels;
            int blockAlign = (int)audioPCMFormat.BlockAlign;

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


        int m_LowThresholdCount = 0;
        List<double> m_LowAmpList = new List<double>();
        bool m_GoneHigh = false;
        double m_UBound = -28.8;
        private bool m_IsLowAmplitude;
        public bool IsLevelTooLow { get { return m_IsLowAmplitude; } }

        private void DetectLowAmplitude(AudioLibPCMFormat audioPCMFormat)
        {
            int channels = audioPCMFormat.NumberOfChannels;

            double AmplitudeValue;

            if (channels == 1) AmplitudeValue = m_AverageValue[0];
            else AmplitudeValue = (m_AverageValue[0] + m_AverageValue[1]) / 2;

            if (AmplitudeValue < m_LBound)
                m_LowThresholdCount++;
            else
            {
                m_GoneHigh = true;
                m_LowAmpList.Add(AmplitudeValue);
                m_LowThresholdCount = 0;
            }

            if ((m_LowThresholdCount >= 4 || m_LowAmpList.Count > 40)
                && m_GoneHigh)
            {
                if (m_LowAmpList.Count > 4)
                {
                    m_GoneHigh = false;
                    double avg = 0;
                    foreach (double d in m_LowAmpList)
                        avg += d;

                    avg = avg / m_LowAmpList.Count;
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
                    m_LowAmpList.Clear();
                    m_LowThresholdCount = 0;
                    //Debug_WriteToTextFile(" next ");
                }
                else
                {
                    m_LowAmpList.Clear();
                    m_LowThresholdCount = 0;
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

        

         // alert events
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

    }
}
