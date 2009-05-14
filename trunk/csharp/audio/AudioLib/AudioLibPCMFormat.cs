using System;
using System.Collections.Generic;
using System.Text;

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

        public AudioLibPCMFormat ( int channels, int samplingRate, int bitDepth )
            {

            if (samplingRate != 11025
    && samplingRate != 22050
   && samplingRate != 44100
   && samplingRate != 88200
    && !(samplingRate >= 8000 && samplingRate <= 96000 && samplingRate % 8000 == 0))//Todo: check this line
                {
                throw new System.Exception ( "Invalid sampling rate" );
                }

            if (!(bitDepth >= 8 && bitDepth <= 32 && bitDepth % 8 == 0))
                {
                throw new System.Exception ( "Invalid bit depth" );
                }

            m_Channels = channels;
            m_SamplingRate = samplingRate;
            m_BitDepth = bitDepth;
            }

        public AudioLibPCMFormat ( ushort channels, uint samplingRate, ushort bitDepth )
            : this ( (int)channels, (int)samplingRate, (int)bitDepth )
            {
            }

        public int NumberOfChannels { get { return m_Channels; } }
        public int SampleRate { get { return m_SamplingRate; } }
        public int BitDepth { get { return m_BitDepth; } }
        public int BlockAlign { get { return (m_BitDepth / 8) * m_Channels; } }

        public long GetLengthInBytes ( double time )
            {
            long length = CalculationFunctions.ConvertTimeToByte ( time, SampleRate, BlockAlign );
            return CalculationFunctions.AdaptToFrame ( length, BlockAlign );
            }

        public double GetDurationInMilliseconds ( long length )
            {
            return CalculationFunctions.ConvertByteToTime ( length, SampleRate, BlockAlign );
            }

        }
    }
