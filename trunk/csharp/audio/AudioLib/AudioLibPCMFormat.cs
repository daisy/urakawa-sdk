using System;
using System.IO;
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
            long bytes = Convert.ToInt64((time * samplingRate * frameSize) / 1000.0);
            bytes -= bytes % frameSize;
            return bytes;
        }

        public double ConvertBytesToTime(long bytes)
        {
            return ConvertBytesToTime(bytes, m_SamplingRate, BlockAlign);
        }

        public static double ConvertBytesToTime(long bytes, int samplingRate, int frameSize)
        {
            bytes -= bytes % frameSize;
            return (1000.0 * bytes) / (samplingRate * frameSize); //Convert.ToDouble
        }


        public static ulong WriteWavePcmRiffHeader(Stream output,
            uint dataLength, ushort channels, uint sampleRate, ushort bitDepth)
        {
            ushort blockAlign = (ushort)(channels * bitDepth / 8);
            uint byteRate = channels * sampleRate * bitDepth / 8U;

            long initPos = output.Position;

            BinaryWriter wr = new BinaryWriter(output);
            wr.Write(Encoding.ASCII.GetBytes("RIFF")); //Chunk Uid
            uint chunkSize = 4 + 8 + 16 + 8 + dataLength;
            wr.Write(chunkSize); //Chunk Size
            wr.Write(Encoding.ASCII.GetBytes("WAVE")); //Format field
            wr.Write(Encoding.ASCII.GetBytes("fmt ")); //Format sub-chunk
            uint formatChunkSize = 16;
            wr.Write(formatChunkSize);
            ushort audioFormat = 1; //PCM format
            wr.Write(audioFormat);
            wr.Write(channels);
            wr.Write(sampleRate);
            wr.Write(byteRate);
            wr.Write(blockAlign);
            wr.Write(bitDepth);
            wr.Write(Encoding.ASCII.GetBytes("data"));
            wr.Write(dataLength);

            long endPos = output.Position;
            return (ulong)(endPos - initPos);
        }


        //public void CreateRIFF(BinaryWriter Writer)
        //{
        //    // Set up file with RIFF chunk info.
        //    char[] ChunkRiff = { 'R', 'I', 'F', 'F' };
        //    char[] ChunkType = { 'W', 'A', 'V', 'E' };
        //    char[] ChunkFmt = { 'f', 'm', 't', ' ' };
        //    char[] ChunkData = { 'd', 'a', 't', 'a' };
        //    short shPad = 1; // File padding
        //    int nFormatChunkLength = 0x10;
        //    // Format chunk length.
        //    short shBytesPerSample = 0;
        //    // Figure out how many bytes there will be per sample.
        //    if (8 == m_CaptureBuffer.Format.BitsPerSample && 1 == m_CaptureBuffer.Format.Channels)
        //        shBytesPerSample = 1;
        //    else if ((8 == m_CaptureBuffer.Format.BitsPerSample && 2 == m_CaptureBuffer.Format.Channels) || (16 == m_CaptureBuffer.Format.BitsPerSample && 1 == m_CaptureBuffer.Format.Channels))
        //        shBytesPerSample = 2;
        //    else if (16 == m_CaptureBuffer.Format.BitsPerSample && 2 == m_CaptureBuffer.Format.Channels)
        //        shBytesPerSample = 4;
        //    // Fill in the riff info for the wave file.
        //    Writer.Write(ChunkRiff);
        //    Writer.Write(1);
        //    Writer.Write(ChunkType);
        //    // Fill in the format info for the wave file.
        //    Writer.Write(ChunkFmt);
        //    Writer.Write(nFormatChunkLength);
        //    Writer.Write(shPad);
        //    Writer.Write(m_CaptureBuffer.Format.Channels);
        //    Writer.Write(m_CaptureBuffer.Format.SamplesPerSecond);
        //    Writer.Write(m_CaptureBuffer.Format.AverageBytesPerSecond);
        //    Writer.Write(shBytesPerSample);
        //    Writer.Write(m_CaptureBuffer.Format.BitsPerSample);
        //    // Now fill in the data chunk.
        //    Writer.BaseStream.Position = 36;
        //    Writer.Write(ChunkData);
        //    Writer.BaseStream.Position = 40;
        //    Writer.Write(0);	// The sample length will be written in later
        //    Writer.Close();
        //    //Writer = null;
        //}
    }
}
