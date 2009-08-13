using System;

namespace AudioLib
{
    /// <summary>
    /// Class for holding information about no. of channels, sampling rate and bit depth for PCM file or stream.
    /// </summary> 
    public class AudioLibPCMFormat
    {
        private int m_Channels;
        private int m_SamplingRate;
        private int m_BitDepth;

        public AudioLibPCMFormat(int channels, int samplingRate, int bitDepth)
        {
            NumberOfChannels = channels;
            SampleRate = samplingRate;
            BitDepth = bitDepth;
        }

        public int NumberOfChannels // 1 = mono, 2 = stereo
        {
            get { return m_Channels; }
            set { m_Channels = value; }
        }

        public int SampleRate
        {
            get { return m_SamplingRate; }
            set
            {
                if (value != 11025
                  && value != 22050
                  && value != 44100
                  && value != 88200
                  && !(value >= 8000
                          && value <= 96000
                          && value % 8000 == 0)
                  )//Todo: check this line
                {
                    throw new System.Exception("Invalid sampling rate: " + value);
                }
                m_SamplingRate = value;
            }
        }

        public int BitDepth // number of bits per sample
        {
            get { return m_BitDepth; }
            set
            {
                if (!(value >= 8 && value <= 32 && value % 8 == 0))
                {
                    throw new System.Exception("Invalid bit depth: " + value);
                }
                m_BitDepth = value;
            }
        }

        public int BlockAlign // Otherwise known as "frame size", in bytes. A frame is a sequence of samples, one for each channel of the digitized audio signal.
        {
            get { return (m_BitDepth / 8) * m_Channels; }
        }

        public void CopyValues(AudioLibPCMFormat pcmFormat)
        {
            NumberOfChannels = pcmFormat.NumberOfChannels;
            SampleRate = pcmFormat.SampleRate;
            BitDepth = pcmFormat.BitDepth;
        }

        public long ConvertTimeToBytes(double time)
        {
            return ConvertTimeToBytes(time, m_SamplingRate, BlockAlign);
        }

        public static long ConvertTimeToBytes(double time, int samplingRate, int frameSize)
        {
            return Convert.ToInt64((time * samplingRate * frameSize) / 1000.0);
        }

        public double ConvertBytesToTime(long bytes)
        {
            return ConvertBytesToTime(bytes, m_SamplingRate, BlockAlign);
        }

        public static double ConvertBytesToTime(long bytes, int samplingRate, int frameSize)
        {
            return (1000.0 * bytes) / (samplingRate * frameSize); //Convert.ToDouble
        }
    }
}
