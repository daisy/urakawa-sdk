using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Windows.Forms;
using System.Threading;
using System.Collections.Generic;

#if USE_SHARPDX
using SharpDX.DirectSound;
using SharpDX.Multimedia;
#else
using Microsoft.DirectX.DirectSound;
#endif

#if USE_SOUNDTOUCH

using SoundTouch;
using SoundTouch.Utility;
using Buffer = System.Buffer;
#if true || SOUNDTOUCH_INTEGER_SAMPLES
using TSampleType = System.Int16; // short (System.Int32 == int)
using TLongSampleType = System.Int64; // long
#else
using TSampleType = System.Single;
using TLongSampleType = System.Double;
#endif

#endif //USE_SOUNDTOUCH

namespace AudioLib
{
    public partial class AudioPlayer
    {

#if USE_SHARPDX
        private byte[] SharpDX_IntermediaryTransferBuffer;
#endif

        private readonly Object LOCK = new object();
        private Thread m_CircularBufferRefreshThread;

        //private bool m_CircularBufferRefreshThreadIsAlive = false;

#if USE_SHARPDX
        private SecondarySoundBuffer m_CircularBuffer;
#else
        private SecondaryBuffer m_CircularBuffer;
#endif
        //private int m_CircularBufferRefreshChunkSize;
        private int m_CircularBufferWritePosition;

        private byte[] m_PcmDataBuffer;
        private int m_PcmDataBufferLength;

#if USE_SOUNDTOUCH
        private byte[] m_SoundTouch_ByteBuffer = null;

#if !USE_SHARPDX
        private MemoryStream m_SoundTouch_ByteBuffer_Stream = null;
#endif

        private TSampleType[] m_SoundTouch_SampleBuffer = null;
#endif //USE_SOUNDTOUCH

        private const int REFRESH_INTERVAL_MS = 50; //ms interval for refreshing PCM data
        public int RefreshInterval
        {
            get { return REFRESH_INTERVAL_MS; }
        }


        private int transferBytesFromWavStreamToCircularBuffer(int circularBufferBytesAvailableForWriting)
        {
            int circularBufferLength =
                m_CircularBuffer.
#if USE_SHARPDX
Capabilities
#else
 Caps
#endif
.BufferBytes
;

            DebugFix.Assert(circularBufferBytesAvailableForWriting <= circularBufferLength);


            long pcmDataRemainingPlayableFromStream = m_PlaybackEndPositionInCurrentAudioStream - m_CurrentAudioStream.Position;

            int bytesToTransferToCircularBuffer =
                Math.Min(circularBufferBytesAvailableForWriting,
                         (int)pcmDataRemainingPlayableFromStream);
            //bytesToTransferToCircularBuffer = Math.Min(bytesToTransferToCircularBuffer, m_CircularBufferRefreshChunkSize);
            bytesToTransferToCircularBuffer -= bytesToTransferToCircularBuffer %
                                               m_CurrentAudioPCMFormat.BlockAlign;

            if (bytesToTransferToCircularBuffer <= 0)
            {
                Debug.Fail("bytesToTransferToCircularBuffer <= 0 !!?");
                return 0;
            }

            if (m_CurrentAudioStream.Position + bytesToTransferToCircularBuffer >
                            m_PlaybackEndPositionInCurrentAudioStream)
            {
                // safeguard
                bytesToTransferToCircularBuffer = (int)pcmDataRemainingPlayableFromStream;
            }



#if USE_SOUNDTOUCH

            if (UseSoundTouch && NotNormalPlayFactor())
            {
                int sampleSizePerChannel = m_CurrentAudioPCMFormat.BitDepth / 8;
                // == m_CurrentAudioPCMFormat.BlockAlign / m_CurrentAudioPCMFormat.NumberOfChannels;

#if DEBUG
                int sizeOfTypeInBytes = Marshal.SizeOf(typeof(TSampleType));
                DebugFix.Assert(sizeOfTypeInBytes == sampleSizePerChannel);

                sizeOfTypeInBytes = sizeof(TSampleType);
                DebugFix.Assert(sizeOfTypeInBytes == sampleSizePerChannel);
#endif // DEBUG

                if (m_SoundTouch_ByteBuffer == null)
                {
                    Console.WriteLine("ALLOCATING m_SoundTouch_ByteBuffer");
                    m_SoundTouch_ByteBuffer = new byte[bytesToTransferToCircularBuffer];

#if !USE_SHARPDX
        m_SoundTouch_ByteBuffer_Stream = new MemoryStream(m_SoundTouch_ByteBuffer);
#endif
                }
                else if (m_SoundTouch_ByteBuffer.Length < bytesToTransferToCircularBuffer)
                {
                    Console.WriteLine("m_SoundTouch_ByteBuffer.resize");
                    Array.Resize(ref m_SoundTouch_ByteBuffer, bytesToTransferToCircularBuffer);

                    //m_SoundTouch_ByteBuffer_Stream.Capacity = nbytes;
                    //m_SoundTouch_ByteBuffer_Stream.SetLength(nbytes);

#if !USE_SHARPDX
        m_SoundTouch_ByteBuffer_Stream = new MemoryStream(m_SoundTouch_ByteBuffer);
#endif
                }

                //int bytesToReadFromAudioStream = bytesToTransferToCircularBuffer;
                //DebugFix.Assert(bytesToReadFromAudioStream <= bytesToTransferToCircularBuffer);

                int bytesReadFromAudioStream = m_CurrentAudioStream.Read(m_SoundTouch_ByteBuffer, 0, bytesToTransferToCircularBuffer);
                DebugFix.Assert(bytesReadFromAudioStream == bytesToTransferToCircularBuffer);

                int soundTouch_SampleBufferLength = (bytesReadFromAudioStream * 8) / m_CurrentAudioPCMFormat.BitDepth; // 16


                if (m_SoundTouch_SampleBuffer == null)
                {
                    Console.WriteLine("ALLOCATING m_SoundTouch_SampleBuffer");
                    m_SoundTouch_SampleBuffer = new TSampleType[soundTouch_SampleBufferLength];
                }
                else if (m_SoundTouch_SampleBuffer.Length < soundTouch_SampleBufferLength)
                {
                    Console.WriteLine("m_SoundTouch_SampleBuffer.resize");
                    Array.Resize(ref m_SoundTouch_SampleBuffer, soundTouch_SampleBufferLength);
                }

                int sampleBufferIndex = 0;
                for (int i = 0; i < bytesReadFromAudioStream; i += m_CurrentAudioPCMFormat.BlockAlign)
                {
                    for (int channel = 0; channel < m_CurrentAudioPCMFormat.NumberOfChannels; channel++)
                    {
                        byte byte1 = m_SoundTouch_ByteBuffer[i + channel];
                        byte byte2 = m_SoundTouch_ByteBuffer[i + channel + 1];
                        short sample =
                            BitConverter.IsLittleEndian
                            ? (short)(byte1 | (byte2 << 8))
                            : (short)((byte1 << 8) | byte2);
                        m_SoundTouch_SampleBuffer[sampleBufferIndex] = sample;


#if DEBUG
                        //// Little Indian
                        //short s1 = (short)(byte1 | (byte2 << 8));
                        //short s2 = (short)(byte1 + byte2 * 256);

                        //// Big Indian
                        //short s3 = (short)((byte1 << 8) | byte2);
                        //short s4 = (short)(byte1 * 256 + byte2);

                        short checkedSample = BitConverter.ToInt16(m_SoundTouch_ByteBuffer, i + channel);
                        DebugFix.Assert(checkedSample == sample);

                        checkedSample = (short)(byte1 + byte2 * 256);
                        DebugFix.Assert(checkedSample == sample);
#endif //DEBUG
                        sampleBufferIndex++;
                    }
                }

                int soundTouch_SampleBufferLength_Channels = soundTouch_SampleBufferLength /
                                                             m_CurrentAudioPCMFormat.NumberOfChannels;

                int soundTouch_SampleBufferLength_Channels_FULL = m_SoundTouch_SampleBuffer.Length /
                                                             m_CurrentAudioPCMFormat.NumberOfChannels;

                //m_SoundTouch.Flush();

                int samplesReceived = -1;

                //while (
                //    (samplesReceived = m_SoundTouch.ReceiveSamples(
                //        (ArrayPtr<TSampleType>)m_SoundTouch_SampleBuffer,
                //        soundTouch_SampleBufferLength_Channels_FULL
                //        )) > 0)
                //{
                //    // Ignore.
                //    bool debug = true;
                //}

                m_SoundTouch.PutSamples((ArrayPtr<TSampleType>)m_SoundTouch_SampleBuffer,
                                        soundTouch_SampleBufferLength_Channels);

                int totalBytesReceivedFromSoundTouch = 0;
                while (
                    (samplesReceived = m_SoundTouch.ReceiveSamples(
                        (ArrayPtr<TSampleType>)m_SoundTouch_SampleBuffer,
                        soundTouch_SampleBufferLength_Channels_FULL
                        )) > 0)
                {
                    samplesReceived *= m_CurrentAudioPCMFormat.NumberOfChannels;

                    int bytesReceivedFromSoundTouch = samplesReceived * sampleSizePerChannel;

                    int predictedTotal = totalBytesReceivedFromSoundTouch + bytesReceivedFromSoundTouch;
                    if (//predictedTotal > bytesReadFromAudioStream ||
                        predictedTotal > m_SoundTouch_ByteBuffer.Length)
                    {
#if DEBUG
                        // The breakpoint should never hit,
                        // because the output audio data is smaller than the source
                        // due to the time stretching making the playback faster.
                        // (the ratio is about the same as m_FastPlayFactor)

                        //DebugFix.Assert(m_FastPlayFactor < 1);

                        if (m_FastPlayFactor >= 1)
                        {
                            Debugger.Break();
                        }
#endif
                        break;
                    }

                    //unsafe
                    //{
                    //    fixed (short* pShorts = m_SoundTouch_SampleBuffer)
                    //        Marshal.Copy((IntPtr)pShorts, 0, m_SoundTouch_ByteBuffer, actualSamples);
                    //}

                    if (false)
                    {
                        // TODO: check little / big endian
                        Buffer.BlockCopy(m_SoundTouch_SampleBuffer,
                                         0,
                                         m_SoundTouch_ByteBuffer,
                                         totalBytesReceivedFromSoundTouch,
                                         bytesReceivedFromSoundTouch);
                    }
                    else
                    {
                        //try
                        //{
                        //}
                        //catch (Exception ex)
                        //{
                        //    Debugger.Break();
                        //}

                        int checkTotalBytes = 0;

                        for (int s = 0; s < samplesReceived; s += m_CurrentAudioPCMFormat.NumberOfChannels)
                        {
                            if (false)
                            {
                                int sampleSizePerChannels = sampleSizePerChannel * m_CurrentAudioPCMFormat.NumberOfChannels;

                                checkTotalBytes += sampleSizePerChannels;

                                // TODO: check little / big endian
                                Buffer.BlockCopy(m_SoundTouch_SampleBuffer,
                                                 s * sampleSizePerChannel,
                                                 m_SoundTouch_ByteBuffer,
                                                 totalBytesReceivedFromSoundTouch
                                                 + s * sampleSizePerChannel,
                                                 sampleSizePerChannels);
                            }
                            else
                            {
                                for (int channel = 0; channel < m_CurrentAudioPCMFormat.NumberOfChannels; channel++)
                                {
                                    if (true)
                                    {
                                        checkTotalBytes += sampleSizePerChannel;

                                        // TODO: check little / big endian
                                        Buffer.BlockCopy(m_SoundTouch_SampleBuffer,

                                                         // TODO: weird! only first sample (left channel) is correct :(
                                                         //(s + channel) * sampleSizePerChannel,
                                                         (s + 0) * sampleSizePerChannel,

                                                         m_SoundTouch_ByteBuffer,
                                                         totalBytesReceivedFromSoundTouch
                                                         + (s + channel) * sampleSizePerChannel,
                                                         sampleSizePerChannel);
                                    }
                                    else
                                    {
                                        TSampleType sample = m_SoundTouch_SampleBuffer[s + channel];

                                        byte[] sampleBytes;
                                        if (BitConverter.IsLittleEndian)
                                        {
                                            sampleBytes = BitConverter.GetBytes(sample);
                                        }
                                        else
                                        {
                                            sampleBytes =
                                                BitConverter.GetBytes(
                                                    (short)((sample & 0xFF) << 8 | (sample & 0xFF00) >> 8));
                                        }

                                        DebugFix.Assert(sampleSizePerChannel == sampleBytes.Length);

                                        checkTotalBytes += sampleSizePerChannel;

                                        Buffer.BlockCopy(sampleBytes,
                                                         0,
                                                         m_SoundTouch_ByteBuffer,
                                                         totalBytesReceivedFromSoundTouch
                                                         + (s + channel) * sampleSizePerChannel,
                                                         sampleSizePerChannel);
                                    }
                                }
                            }
                        }

                        DebugFix.Assert(checkTotalBytes == bytesReceivedFromSoundTouch);
                    }

                    totalBytesReceivedFromSoundTouch += bytesReceivedFromSoundTouch;

                    if (//totalBytesReceivedFromSoundTouch >= bytesReadFromAudioStream ||
                        totalBytesReceivedFromSoundTouch >= circularBufferBytesAvailableForWriting)
                    {
                        break;
                    }
                }


#if !USE_SHARPDX
        m_SoundTouch_ByteBuffer_Stream.Position = 0;
#endif

                if (totalBytesReceivedFromSoundTouch > 0
                    && totalBytesReceivedFromSoundTouch <= circularBufferBytesAvailableForWriting)
                {
#if DEBUG
                    //double ratio = bytesReadFromAudioStream / (double)totalBytesReceivedFromSoundTouch;
                    //Console.WriteLine("FAST: " + ratio + " -- " + m_FastPlayFactor);

                    DebugFix.Assert(totalBytesReceivedFromSoundTouch <= circularBufferLength);
                    //DebugFix.Assert(totalBytesReceivedFromSoundTouch <= bytesReadFromAudioStream);
#endif // DEBUG

#if USE_SHARPDX
                    m_CircularBuffer.Write<
                        //TSampleType
                        byte
                        >(
                        //m_SoundTouch_SampleBuffer,
                        m_SoundTouch_ByteBuffer,
                        0,
                        totalBytesReceivedFromSoundTouch,
                        m_CircularBufferWritePosition,
                        LockFlags.None);
#else
                    m_CircularBuffer.Write(m_CircularBufferWritePosition,
                            m_SoundTouch_ByteBuffer_Stream,
                                           totalBytesReceivedFromSoundTouch,
                    LockFlag.None);
#endif

                    m_CircularBufferWritePosition += totalBytesReceivedFromSoundTouch;

                    m_CircularBufferWritePosition %= circularBufferLength;
                    //if (m_CircularBufferWritePosition >= m_CircularBuffer.Caps.BufferBytes)
                    //{
                    //    m_CircularBufferWritePosition -= m_CircularBuffer.Caps.BufferBytes;
                    //}
                }

                //#if DEBUG
                //                if (
                //#if USE_SOUNDTOUCH
                //!UseSoundTouch ||
                //#endif
                //                    !NotNormalPlayFactor())
                //                {
                //                    DebugFix.Assert(totalBytesReceivedFromSoundTouch == bytesToTransferToCircularBuffer);
                //                }
                //#endif//DEBUG
                return totalBytesReceivedFromSoundTouch;
            }
            else
#endif //USE_SOUNDTOUCH

            {
#if USE_SHARPDX
                if (SharpDX_IntermediaryTransferBuffer == null)
                {
                    Console.WriteLine("ALLOCATING SharpDX_IntermediaryTransferBuffer");
                    SharpDX_IntermediaryTransferBuffer = new byte[bytesToTransferToCircularBuffer];
                }
                else if (SharpDX_IntermediaryTransferBuffer.Length < bytesToTransferToCircularBuffer)
                {
                    Console.WriteLine("SharpDX_IntermediaryTransferBuffer.resize");
                    Array.Resize(ref SharpDX_IntermediaryTransferBuffer, bytesToTransferToCircularBuffer);
                }
                int read = m_CurrentAudioStream.Read(SharpDX_IntermediaryTransferBuffer, 0, bytesToTransferToCircularBuffer);
                DebugFix.Assert(bytesToTransferToCircularBuffer == read);
                m_CircularBuffer.Write<byte>(SharpDX_IntermediaryTransferBuffer, 0, bytesToTransferToCircularBuffer, m_CircularBufferWritePosition, LockFlags.None);
#else
                m_CircularBuffer.Write(m_CircularBufferWritePosition, m_CurrentAudioStream, bytesToTransferToCircularBuffer, LockFlag.None);
#endif

                m_CircularBufferWritePosition += bytesToTransferToCircularBuffer;

                m_CircularBufferWritePosition %= circularBufferLength;
                //if (m_CircularBufferWritePosition >= m_CircularBuffer.Caps.BufferBytes)
                //{
                //    m_CircularBufferWritePosition -= m_CircularBuffer.Caps.BufferBytes;
                //}

                return bytesToTransferToCircularBuffer;
            }

            //int afterWriteCursor = m_CircularBuffer.Caps.BufferBytes - m_CircularBufferWritePosition;
            //if (toCopy <= afterWriteCursor)
            //{
            //    m_CircularBuffer.Write(m_CircularBufferWritePosition, m_CurrentAudioStream, toCopy, LockFlag.None);
            //}
            //else
            //{
            //    m_CircularBuffer.Write(m_CircularBufferWritePosition, m_CurrentAudioStream, afterWriteCursor, LockFlag.None);
            //    m_CircularBuffer.Write(0, m_CurrentAudioStream, toCopy - afterWriteCursor, LockFlag.None);
            //}
        }


        private void initializeBuffers()
        {
            // example: 44.1 kHz (44,100 samples per second) * 16 bits per sample / 8 bits per byte * 2 channels (stereo)
            // blockAlign is number of bytes per frame (samples required for all channels)

            uint fastPlaySamplesPerSecond = m_CurrentAudioPCMFormat.SampleRate; // (int)Math.Round(m_CurrentAudioPCMFormat.SampleRate * m_FastPlayFactor);
            uint byteRate = fastPlaySamplesPerSecond * m_CurrentAudioPCMFormat.BlockAlign; // (m_CurrentAudioPCMFormat.BitDepth / 8) * m_CurrentAudioPCMFormat.NumberOfChannels;

            int pcmDataBufferSize = (int)Math.Round(byteRate * REFRESH_INTERVAL_MS / 1000.0);
            pcmDataBufferSize -= pcmDataBufferSize % m_CurrentAudioPCMFormat.BlockAlign;

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

            int dxBufferSize = (int)Math.Round(byteRate * (20 * REFRESH_INTERVAL_MS) / 1000.0); // ONE SECOND when 50ms refresh internal // 0.500 == 500ms
            dxBufferSize -= dxBufferSize % m_CurrentAudioPCMFormat.BlockAlign;

            //m_CircularBufferRefreshChunkSize = (int)(byteRate * 0.200); //200ms
            //m_CircularBufferRefreshChunkSize -= m_CircularBufferRefreshChunkSize % m_CurrentAudioPCMFormat.BlockAlign;

#if USE_SHARPDX
            WaveFormat waveFormat = new WaveFormat((int)m_CurrentAudioPCMFormat.SampleRate, 16, m_CurrentAudioPCMFormat.NumberOfChannels);
#else
            WaveFormat waveFormat = new WaveFormat();
            waveFormat.FormatTag = WaveFormatTag.Pcm;

            waveFormat.Channels = (short)m_CurrentAudioPCMFormat.NumberOfChannels;
            waveFormat.SamplesPerSecond = (int)m_CurrentAudioPCMFormat.SampleRate;
            waveFormat.BitsPerSample = (short)m_CurrentAudioPCMFormat.BitDepth;
            
            waveFormat.BlockAlign = (short)m_CurrentAudioPCMFormat.BlockAlign;
            waveFormat.AverageBytesPerSecond = (int)byteRate;
#endif

#if USE_SHARPDX
            SoundBufferDescription bufferDescription = new SoundBufferDescription();
            bufferDescription.Flags =
                BufferFlags.GetCurrentPosition2 |
                //BufferFlags.ControlPositionNotify | 
                BufferFlags.ControlVolume | BufferFlags.ControlFrequency | BufferFlags.GlobalFocus;
#else
            BufferDescription bufferDescription = new BufferDescription();

            bufferDescription.ControlVolume = true;
            bufferDescription.ControlFrequency = true;
            bufferDescription.GlobalFocus = true;
#endif
            bufferDescription.BufferBytes = dxBufferSize;
            bufferDescription.Format = waveFormat;

#if USE_SHARPDX
            m_CircularBuffer = new SecondarySoundBuffer(OutputDevice.Device, bufferDescription);
#else
            m_CircularBuffer = new SecondaryBuffer(bufferDescription, OutputDevice.Device);
#endif

#if USE_SOUNDTOUCH
            if (m_SoundTouch == null)
            {
                m_SoundTouch = new SoundTouch<TSampleType, TLongSampleType>();

                m_SoundTouch.SetSetting(SettingId.UseAntiAliasFilter, 0);

                m_SoundTouch.SetSetting(SettingId.SequenceDurationMs, 40);
                m_SoundTouch.SetSetting(SettingId.SeekwindowDurationMs, 15);
                m_SoundTouch.SetSetting(SettingId.OverlapDurationMs, 8);
            }

            //m_SoundTouch.Flush();
            m_SoundTouch.Clear();

            //AudioLib.SampleRate.Hz22050
            if (m_CurrentAudioPCMFormat.SampleRate > 22050)
            {
                m_SoundTouch.SetSetting(SettingId.UseQuickseek, 1);
            }
            else
            {
                m_SoundTouch.SetSetting(SettingId.UseQuickseek, 0);
            }

            m_SoundTouch.SetSampleRate((int)m_CurrentAudioPCMFormat.SampleRate);
            m_SoundTouch.SetChannels((int)m_CurrentAudioPCMFormat.NumberOfChannels);

#endif //USE_SOUNDTOUCH


            FastPlayFactor = m_FastPlayFactor; // reset
        }

        //private long m_CircularBufferFlushTolerance;
        //private int m_CircularBufferPreviousBytesAvailableForWriting;



        private bool circularBufferRefreshThreadMethod()
        {
            //m_CircularBufferRefreshThreadIsAlive = true;

            //float previousFastPlayFactor = m_FastPlayFactor;

            //long predictedByteIncrement = -1;


            //int circularBufferTotalBytesPlayed = -1;
            //int previousCircularBufferPlayPosition = -1;


            Stopwatch transferBytesStopWatch = new Stopwatch();
            transferBytesStopWatch.Stop();


            int totalWriteSkips = 0;

            uint adjustedFastPlaySampleRate =
                    (uint)Math.Round(m_CurrentAudioPCMFormat.SampleRate * m_FastPlayFactor);

            long pcmDataTotalPlayableFromStream = m_PlaybackEndPositionInCurrentAudioStream - m_PlaybackStartPositionInCurrentAudioStream;
            long pcmDataTotalPlayableFromStream_DurationMS =
                    (
                    NotNormalPlayFactor()
                    ?
                    AudioLibPCMFormat.ConvertBytesToTime(
                    pcmDataTotalPlayableFromStream,
                    adjustedFastPlaySampleRate,
                    m_CurrentAudioPCMFormat.BlockAlign
#if DEBUG
, true
#endif
)
                    :
                    m_CurrentAudioPCMFormat.ConvertBytesToTime(pcmDataTotalPlayableFromStream)
                    )
                    / AudioLibPCMFormat.TIME_UNIT;

            int slowLoop = 0;

            int sleepTime = REFRESH_INTERVAL_MS;

            //bool endOfAudioStream = false;
            while (true)
            {
#if USE_SHARPDX
                DebugFix.Assert(
                    m_CircularBuffer.Status == (int)BufferStatus.BufferLost
                    || m_CircularBuffer.Status == (int)BufferStatus.Hardware
                    || m_CircularBuffer.Status == (int)BufferStatus.Looping
                    || m_CircularBuffer.Status == (int)BufferStatus.None
                    || m_CircularBuffer.Status == (int)BufferStatus.Playing
                    || m_CircularBuffer.Status == (int)BufferStatus.Software
                    || m_CircularBuffer.Status == (int)BufferStatus.Terminated
                    || m_CircularBuffer.Status == 5 //?!
                    );

                if (m_CircularBuffer.Status == (int)BufferStatus.BufferLost)
                {
                    m_CircularBuffer.Restore();
                }
#else
                if (m_CircularBuffer.Status.BufferLost)
                {
                    m_CircularBuffer.Restore();
                }
#endif

#if USE_SHARPDX
                if (m_CircularBuffer.Status == (int)BufferStatus.Terminated
                    || (m_CircularBuffer.Status != 5 //?!
                    && m_CircularBuffer.Status != (int)BufferStatus.Playing
                    && m_CircularBuffer.Status != (int)BufferStatus.Looping)
                    )
                {
                    return false;
                }
#else
                if (m_CircularBuffer.Status.Terminated
                    || (!m_CircularBuffer.Status.Playing
                    && !m_CircularBuffer.Status.Looping)
                    )
                {
                    return false;
                }
#endif

                Thread.Sleep(sleepTime);
                sleepTime = REFRESH_INTERVAL_MS; // reset after each loop


                //                if (predictedByteIncrement < 0
                //                    || true //m_FastPlayFactor != previousFastPlayFactor
                //                    )
                //                {
                //                    //previousFastPlayFactor = m_FastPlayFactor;


                //                    int fastPlaySamplesPerSecond = (int)Math.Round(m_CurrentAudioPCMFormat.SampleRate * m_FastPlayFactor);
                //#if DEBUG
                //                    DebugFix.Assert(m_CurrentAudioPCMFormat.SampleRate == m_CircularBuffer.Format.SamplesPerSecond);

                //                    if (
                //#if USE_SOUNDTOUCH
                //!UseSoundTouch &&
                //#endif //USE_SOUNDTOUCH
                //#if USE_SHARPDX
                // m_CircularBuffer.Capabilities.ControlFrequency
                //#else
                // m_CircularBuffer.Caps.ControlFrequency
                //#endif
                //)
                //                    {
                //                        DebugFix.Assert(m_CircularBuffer.Frequency == fastPlaySamplesPerSecond);

                //                    }
                //#endif //DEBUG
                //                    int byteRate = fastPlaySamplesPerSecond * m_CurrentAudioPCMFormat.BlockAlign; // (m_CurrentAudioPCMFormat.BitDepth / 8) * m_CurrentAudioPCMFormat.NumberOfChannels;


                //                    predictedByteIncrement = (long)(byteRate * (REFRESH_INTERVAL_MS + 15) / 1000.0);
                //                    predictedByteIncrement -= predictedByteIncrement % m_CurrentAudioPCMFormat.BlockAlign;
                //                }

#if USE_SHARPDX
                int circularBufferPlayPosition;
                int circularBufferWritePosition;
                m_CircularBuffer.GetCurrentPosition(out circularBufferPlayPosition, out circularBufferWritePosition);
#else
                int circularBufferPlayPosition = m_CircularBuffer.PlayPosition;
#endif

                int circularBufferLength = m_CircularBuffer.
#if USE_SHARPDX
Capabilities
#else
Caps
#endif
.BufferBytes
;
                //if (circularBufferTotalBytesPlayed < 0)
                //{
                //    circularBufferTotalBytesPlayed = circularBufferPlayPosition;
                //}
                //else
                //{
                //    if (circularBufferPlayPosition >= previousCircularBufferPlayPosition)
                //    {
                //        circularBufferTotalBytesPlayed += circularBufferPlayPosition - previousCircularBufferPlayPosition;
                //    }
                //    else
                //    {
                //        circularBufferTotalBytesPlayed += (circularBufferLength - previousCircularBufferPlayPosition) + circularBufferPlayPosition;
                //    }
                //}

                //previousCircularBufferPlayPosition = circularBufferPlayPosition;


                //                int totalBytesPlayed_AdjustedPlaybackRate = circularBufferTotalBytesPlayed;

                //#if USE_SOUNDTOUCH
                //                if (UseSoundTouch && NotNormalPlayFactor())
                //                {
                //                    //m_CurrentAudioPCMFormat
                //                    totalBytesPlayed_AdjustedPlaybackRate = (int)Math.Round(totalBytesPlayed_AdjustedPlaybackRate * m_FastPlayFactor);
                //                    totalBytesPlayed_AdjustedPlaybackRate -= totalBytesPlayed_AdjustedPlaybackRate % m_CurrentAudioPCMFormat.BlockAlign;
                //                }
                //#endif //USE_SOUNDTOUCH




                int circularBufferBytesAvailableForWriting = (circularBufferPlayPosition == m_CircularBufferWritePosition ? 0
                                        : (circularBufferPlayPosition < m_CircularBufferWritePosition
                                  ? circularBufferPlayPosition + (circularBufferLength - m_CircularBufferWritePosition)
                                  : circularBufferPlayPosition - m_CircularBufferWritePosition));

                //int circularBufferBytesAvailableForPlaying = circularBufferLength - circularBufferBytesAvailableForWriting;




                //realTimePlaybackPosition -= realTimePlaybackPosition % m_CurrentAudioPCMFormat.BlockAlign;

                //Console.WriteLine(String.Format("bytesAvailableForWriting: [{0} / {1}]", bytesAvailableForWriting, m_CircularBuffer.Caps.BufferBytes));

                //Console.WriteLine("dataAvailableFromStream: " + dataAvailableFromStream);

                //                int circularBufferBytesAvailableForPlaying_AdjustedPlaybackRate = circularBufferBytesAvailableForPlaying;

                //#if USE_SOUNDTOUCH
                //                if (UseSoundTouch && NotNormalPlayFactor())
                //                {
                //                    circularBufferBytesAvailableForPlaying_AdjustedPlaybackRate = (int)Math.Round(circularBufferBytesAvailableForPlaying_AdjustedPlaybackRate * m_FastPlayFactor);
                //                    circularBufferBytesAvailableForPlaying_AdjustedPlaybackRate -= circularBufferBytesAvailableForPlaying_AdjustedPlaybackRate % m_CurrentAudioPCMFormat.BlockAlign;
                //                }
                //#endif //USE_SOUNDTOUCH

                long totalPlayedMS = m_PlaybackStopWatch.ElapsedMilliseconds;
                long totalPlayedBytes =
                    (
                    NotNormalPlayFactor()
                    ?
                    AudioLibPCMFormat.ConvertTimeToBytes(
                    totalPlayedMS * AudioLibPCMFormat.TIME_UNIT,
                    (uint)adjustedFastPlaySampleRate,
                    (ushort)m_CurrentAudioPCMFormat.BlockAlign
#if DEBUG
, true
#endif
)
                    :
                    m_CurrentAudioPCMFormat.ConvertTimeToBytes(totalPlayedMS * AudioLibPCMFormat.TIME_UNIT)
                    );


                m_CurrentBytePosition = m_PlaybackStartPositionInCurrentAudioStream + totalPlayedBytes;

                //safeguard
                if (m_CurrentBytePosition < m_PlaybackStartPositionInCurrentAudioStream)
                {
                    m_CurrentBytePosition = m_PlaybackStartPositionInCurrentAudioStream;
                }
                else if (m_CurrentBytePosition > m_PlaybackEndPositionInCurrentAudioStream)
                {
                    m_CurrentBytePosition = m_PlaybackEndPositionInCurrentAudioStream;
                }

                //#if DEBUG
                //                DebugFix.Assert(m_CurrentBytePosition >= m_PlaybackStartPositionInCurrentAudioStream);
                //                DebugFix.Assert(m_CurrentBytePosition <= m_PlaybackEndPositionInCurrentAudioStream);
                //#endif // DEBUG



                //long remainingBytesToPlay = pcmDataTotalPlayableFromStream - totalBytesPlayed_AdjustedPlaybackRate;

                //long realTimePlaybackPosition = Math.Max(m_PlaybackStartPositionInCurrentAudioStream,
                //    m_CurrentAudioStream.Position - Math.Min(
                //    circularBufferBytesAvailableForPlaying_AdjustedPlaybackRate, remainingBytesToPlay));

                //realTimePlaybackPosition = Math.Min(realTimePlaybackPosition,
                //    m_PlaybackStartPositionInCurrentAudioStream + totalBytesPlayed_AdjustedPlaybackRate);

                //if (m_CurrentBytePosition == m_PlaybackStartPositionInCurrentAudioStream)
                //{
                //    //Console.WriteLine(string.Format("m_CurrentBytePosition ASSIGNED: realTimePlaybackPosition [{0}]", realTimePlaybackPosition));

                //    m_CurrentBytePosition += totalBytesPlayed_AdjustedPlaybackRate;
                //}
                ////else if (realTimePlaybackPosition < m_CurrentBytePosition)
                ////{
                ////    Console.WriteLine(string.Format("realTimePlaybackPosition [{0}] < m_CurrentBytePosition [{1}]", realTimePlaybackPosition, m_CurrentBytePosition));

                ////    m_CurrentBytePosition = Math.Min(m_PlaybackEndPositionInCurrentAudioStream, m_CurrentBytePosition + predictedByteIncrement);
                ////}
                ////else if (realTimePlaybackPosition > m_CurrentBytePosition + predictedByteIncrement)
                ////{
                ////    Console.WriteLine(string.Format("realTimePlaybackPosition [{0}] > m_CurrentBytePosition [{1}] + m_PredictedByteIncrement: [{2}] (diff: [{3}])",
                ////        realTimePlaybackPosition, m_CurrentBytePosition, predictedByteIncrement, realTimePlaybackPosition - m_CurrentBytePosition));

                ////    m_CurrentBytePosition = Math.Min(m_PlaybackEndPositionInCurrentAudioStream, m_CurrentBytePosition + predictedByteIncrement);
                ////}
                //else
                //{
                //    //Console.WriteLine(string.Format("m_CurrentBytePosition OK: realTimePlaybackPosition [{0}]", realTimePlaybackPosition));

                //    m_CurrentBytePosition = m_PlaybackStartPositionInCurrentAudioStream + totalBytesPlayed_AdjustedPlaybackRate;
                //}


                PcmDataBufferAvailableHandler del = PcmDataBufferAvailable;
                if (del != null)
                {
                    int min = Math.Min(m_PcmDataBufferLength,
#if FETCH_PCM_FROM_CIRCULAR_BUFFER
                        circularBufferBytesAvailableForPlaying
#else
 (int)(m_PlaybackEndPositionInCurrentAudioStream - m_CurrentBytePosition)
#endif // FETCH_PCM_FROM_CIRCULAR_BUFFER
);

#if DEBUG
                    DebugFix.Assert(min <= m_PcmDataBufferLength);
                    DebugFix.Assert(min <= m_PcmDataBuffer.Length);
#endif //DEBUG
                    if (min >= m_CurrentAudioPCMFormat.BlockAlign)
                    {

#if FETCH_PCM_FROM_CIRCULAR_BUFFER

#if USE_SHARPDX
                        if (SharpDX_IntermediaryTransferBuffer != null)
                        {
                            Array.Copy(SharpDX_IntermediaryTransferBuffer, m_PcmDataBuffer, Math.Min(m_PcmDataBuffer.Length, SharpDX_IntermediaryTransferBuffer.Length));
                        }
#else
                    byte[] array = (byte[])m_CircularBuffer.Read(
                        circularBufferPlayPosition, typeof(byte), LockFlag.None, min);
                    //Array.Copy(array, m_PcmDataBuffer, min);
                    Buffer.BlockCopy(array, 0,
                            m_PcmDataBuffer, 0,
                            min);
#endif
#else // !FETCH_PCM_FROM_CIRCULAR_BUFFER

                        long pos = m_CurrentAudioStream.Position;

                        m_CurrentAudioStream.Position = m_CurrentBytePosition;

                        m_CurrentAudioStream.Read(m_PcmDataBuffer, 0, min);

                        m_CurrentAudioStream.Position = pos;
#endif


                        m_PcmDataBufferAvailableEventArgs.PcmDataBuffer = m_PcmDataBuffer;
                        m_PcmDataBufferAvailableEventArgs.PcmDataBufferLength = min;
                        del(this, m_PcmDataBufferAvailableEventArgs);
                    }
                }

                //var del_ = PcmDataBufferAvailable;
                //if (del_ != null
                //&& m_PcmDataBuffer.Length <= circularBufferBytesAvailableForPlaying)
                //{
                //#if USE_SHARPDX
                //if (SharpDX_IntermediaryTransferBuffer != null)
                //{
                //Array.Copy(SharpDX_IntermediaryTransferBuffer, m_PcmDataBuffer, Math.Min(m_PcmDataBuffer.Length, SharpDX_IntermediaryTransferBuffer.Length));
                //m_PcmDataBufferAvailableEventArgs.PcmDataBuffer = m_PcmDataBuffer;
                //PcmDataBufferAvailable(this, m_PcmDataBufferAvailableEventArgs);
                //}
                //#else
                //Array array = m_CircularBuffer.Read(circularBufferPlayPosition, typeof(byte), LockFlag.None, m_PcmDataBuffer.Length);
                //Array.Copy(array, m_PcmDataBuffer, m_PcmDataBuffer.Length);
                //m_PcmDataBufferAvailableEventArgs.PcmDataBuffer = m_PcmDataBuffer;
                //del_(this, m_PcmDataBufferAvailableEventArgs);
                //#endif
                //}

                long pcmDataRemainingPlayableFromStream = m_PlaybackEndPositionInCurrentAudioStream - m_CurrentAudioStream.Position;
                //long pcmDataAlreadyReadFromStream = pcmDataTotalPlayableFromStream - pcmDataAvailableFromStream;

                if (circularBufferBytesAvailableForWriting <= 0)
                {
                    if (pcmDataRemainingPlayableFromStream > 0)
                    {
                        //Console.WriteLine("circularBufferBytesAvailableForWriting <= 0, pcmDataAvailableFromStream > 0 ... continue...");
                        continue;
                    }
                    else
                    {
                        Console.WriteLine("circularBufferBytesAvailableForWriting <= 0, pcmDataAvailableFromStream <= 0 ... BREAK...");
                        break;
                    }
                }

                //m_CircularBufferPreviousBytesAvailableForWriting = circularBufferBytesAvailableForWriting;

                // We have fed all of the available bytes from the audio stream to the circular secondary buffer.
                // Now we have to wait until the playback ends.
                if (pcmDataRemainingPlayableFromStream <= 0)
                {
                    if ((m_PlaybackStopWatch.ElapsedMilliseconds + REFRESH_INTERVAL_MS) >= pcmDataTotalPlayableFromStream_DurationMS)
                    {
                        m_CircularBuffer.Stop();
                        return true;
                    }
                    else
                    {
                        int newInterval = (int)Math.Round(REFRESH_INTERVAL_MS / 2.0);
                        sleepTime = newInterval;
                        continue;
                    }

                    //if (remainingBytesToPlay > predictedByteIncrement)
                    //{
                    //    Console.WriteLine(string.Format("remainingBytesToPlay [{0}] [{1}] [{2}] [{3}]",
                    //        pcmDataTotalPlayableFromStream, totalBytesPlayed_AdjustedPlaybackRate, remainingBytesToPlay, predictedByteIncrement));
                    //    continue;
                    //}
                    //else
                    //{
                    //    m_CircularBuffer.Stop();

                    //    //Console.WriteLine("Time to break, all bytes gone.");
                    //    //break;

                    //    return true;
                    //}


                    //if (m_CircularBufferFlushTolerance < 0)
                    //{
                    //    m_CircularBufferFlushTolerance = m_CurrentAudioPCMFormat.ConvertTimeToBytes(REFRESH_INTERVAL_MS*1.5);
                    //}

                    //Console.WriteLine(string.Format("pcmDataTotalPlayableFromStream [{0}]", pcmDataTotalPlayableFromStream));

                    //Console.WriteLine(String.Format("pcmDataAvailableFromStream <= 0 // circularBufferBytesAvailableForWriting [{0}], m_CircularBufferFlushTolerance [{1}], m_CircularBuffer.Caps.BufferBytes [{2}], m_CircularBufferPreviousBytesAvailableForWriting [{3}]",
                    //    circularBufferBytesAvailableForWriting, m_CircularBufferFlushTolerance, m_CircularBuffer.Caps.BufferBytes, m_CircularBufferPreviousBytesAvailableForWriting));

                    //if ((circularBufferBytesAvailableForWriting + m_CircularBufferFlushTolerance) >= m_CircularBuffer.Caps.BufferBytes
                    //    || m_CircularBufferPreviousBytesAvailableForWriting > circularBufferBytesAvailableForWriting)
                    //{
                    //    m_CircularBuffer.Stop(); // the earlier the better ?

                    //    Console.WriteLine("Forcing closing-up.");

                    //    circularBufferBytesAvailableForWriting = 0; // will enter the IF test below
                    //}
                }
                else
                {
                    // 2 thirds minimum of the circular buffer must be available,
                    //otherwise skip until next sleep loop
                    double ratio = 2 / 3.0;

#if USE_SOUNDTOUCH
                    if (UseSoundTouch && NotNormalPlayFactor())
                    {
                        ratio /= m_FastPlayFactor;
                    }
#endif // USE_SOUNDTOUCH

                    bool skip = circularBufferBytesAvailableForWriting <
                        (int)Math.Round(circularBufferLength * ratio);

                    if (false && skip)
                    {
                        totalWriteSkips++;
                    }
                    else
                    {
                        //                        int circularBufferBytesAvailableForWriting_AdjustedPlaybackRate =
                        //                            circularBufferBytesAvailableForWriting;

                        //#if USE_SOUNDTOUCH
                        //                        if (UseSoundTouch && NotNormalPlayFactor())
                        //                        {
                        //                            int bytesPerSample = (int)Math.Round(m_CurrentAudioPCMFormat.BitDepth / 8.0);
                        //                            int bytesPerFrame = bytesPerSample * m_CurrentAudioPCMFormat.NumberOfChannels;
                        //                            DebugFix.Assert(m_CurrentAudioPCMFormat.BlockAlign == bytesPerFrame);



                        //                            circularBufferBytesAvailableForWriting_AdjustedPlaybackRate =
                        //                                (int)
                        //                                Math.Round(circularBufferBytesAvailableForWriting_AdjustedPlaybackRate
                        //                                * m_FastPlayFactor
                        //                                * m_CurrentAudioPCMFormat.NumberOfChannels);

                        //                            circularBufferBytesAvailableForWriting_AdjustedPlaybackRate -=
                        //                                circularBufferBytesAvailableForWriting_AdjustedPlaybackRate %
                        //                                m_CurrentAudioPCMFormat.BlockAlign;
                        //                        }
                        //#endif //USE_SOUNDTOUCH


                        //Console.WriteLine("totalWriteSkips: " + totalWriteSkips + " ms: " + totalWriteSkips*REFRESH_INTERVAL_MS);
                        totalWriteSkips = 0;

#if NET4
                        transferBytesStopWatch.Restart();
#else
                        transferBytesStopWatch.Stop();
                        transferBytesStopWatch.Reset();
                        transferBytesStopWatch.Start();
#endif //NET4

                        int bytesWrittenToCirularBuffer =
                            transferBytesFromWavStreamToCircularBuffer(circularBufferBytesAvailableForWriting);

                        long timeMS = transferBytesStopWatch.ElapsedMilliseconds;
                        transferBytesStopWatch.Stop();

                        //Console.WriteLine("transferBytesStopWatch: " + timeMS);

                        sleepTime = Math.Max(10, REFRESH_INTERVAL_MS - (int)timeMS);
#if USE_SOUNDTOUCH
                        if (UseSoundTouch && NotNormalPlayFactor())
                        {
                            if (timeMS >= REFRESH_INTERVAL_MS)
                            {
                                slowLoop++;
                            }
                            if (slowLoop > 2)
                            {
                                slowLoop = 0;
                                Console.WriteLine("SOUNDTOUCH Enable SettingId.UseQuickseek");
                                m_SoundTouch.SetSetting(SettingId.UseQuickseek, 1);
                            }
                        }
#endif //USE_SOUNDTOUCH

                        //#if USE_SOUNDTOUCH
                        //                        if (UseSoundTouch && NotNormalPlayFactor())
                        //                        {
                        //                            int newInterval = (int)Math.Round(REFRESH_INTERVAL_MS / m_FastPlayFactor);
                        //                            //sleepTime = newInterval;
                        //                        }
                        //#endif // USE_SOUNDTOUCH
                    }
                }
            } // WHILE LOOP

            return true;

            //CurrentState = State.Stopped;
            //AudioPlaybackFinishHandler delFinished = AudioPlaybackFinished;
            //if (delFinished != null && !mPreviewTimer.Enabled)
            //    delFinished(this, new AudioPlaybackFinishEventArgs());

            //if (!m_AllowBackToBackPlayback)
            //{
            //    AudioPlaybackFinishHandler delFinished = AudioPlaybackFinished;
            //    if (delFinished != null && mEventsEnabled)
            //        delFinished(this, new AudioPlaybackFinishEventArgs());
            //}
            //else
            //{
            //    m_FinishedPlayingCurrentStream = true;
            //}
        }
    }
}
