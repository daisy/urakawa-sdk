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
        private int m_SanplingRate;
        private int m_BitDepth;

        public AudioLibPCMFormat  ( int channels, int samplingRate, int bitDepth )
            {
            // to do:  need to add invalid format exceptions here

                        m_Channels = channels;
            m_SanplingRate = samplingRate;
            m_BitDepth = bitDepth;
            }

        public AudioLibPCMFormat ( ushort channels, uint samplingRate, ushort bitDepth )
            : this ( (int)channels, (int)samplingRate, (int)bitDepth )
            {
            }

        public int NumberOfChannels  { get { return m_Channels; } }
        public int SampleRate  { get { return m_SanplingRate; } }
        public int BitDepth { get { return m_BitDepth; } }
        public int BlockAlign { get { return (m_BitDepth / 8) * m_Channels; } }

        public long GetLengthInBytes ( double time )
            {
            long length = CalculationFunctions.ConvertTimeToByte ( time, SampleRate, BlockAlign );
            return CalculationFunctions.AdaptToFrame ( length, BlockAlign );
            }

        }
    }
