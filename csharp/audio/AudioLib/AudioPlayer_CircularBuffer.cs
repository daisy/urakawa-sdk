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

#if USE_SLIMDX
        private byte[] SlimDX_IntermediaryTransferBuffer;
#endif

        private readonly Object LOCK = new object();
        private Thread m_CircularBufferRefreshThread;

        //private bool m_CircularBufferRefreshThreadIsAlive = false;

#if USE_SLIMDX
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
        private MemoryStream m_SoundTouch_ByteBuffer_Stream = null;

        private TSampleType[] m_SoundTouch_SampleBuffer = null;
#endif //USE_SOUNDTOUCH

        private const int REFRESH_INTERVAL_MS = 50; //ms interval for refreshing PCM data
        public int RefreshInterval
        {
            get { return REFRESH_INTERVAL_MS; }
        }


        private int transferBytesFromWavStreamToCircularBuffer(int nbytes)
        {
            int circularBufferLength =
#if USE_SLIMDX
 m_CircularBuffer.Capabilities.BufferSize
#else
 m_CircularBuffer.Caps.BufferBytes
#endif
;
#if USE_SOUNDTOUCH

            DebugFix.Assert(nbytes <= circularBufferLength);

            if (UseSoundTouch && NotNormalPlayFactor())
            {
#if DEBUG
                int size = m_CurrentAudioPCMFormat.BitDepth / 8;
                int sizeOfTypeInBytes = Marshal.SizeOf(typeof(TSampleType));
                DebugFix.Assert(sizeOfTypeInBytes == size);
                sizeOfTypeInBytes = sizeof(TSampleType);
                DebugFix.Assert(sizeOfTypeInBytes == size);
#endif // DEBUG

                int nBytesPerSample = m_CurrentAudioPCMFormat.BitDepth / 8; // 16bits => 2 bytes per sample

                if (m_SoundTouch_ByteBuffer == null)
                {
                    m_SoundTouch_ByteBuffer = new byte[nbytes];
                    m_SoundTouch_ByteBuffer_Stream = new MemoryStream(m_SoundTouch_ByteBuffer);
                }
                else if (m_SoundTouch_ByteBuffer.Length < nbytes)
                {
                    Console.WriteLine("m_SoundTouch_ByteBuffer.resize");
                    Array.Resize(ref m_SoundTouch_ByteBuffer, nbytes);

                    //m_SoundTouch_ByteBuffer_Stream.Capacity = nbytes;
                    //m_SoundTouch_ByteBuffer_Stream.SetLength(nbytes);

                    m_SoundTouch_ByteBuffer_Stream = new MemoryStream(m_SoundTouch_ByteBuffer);
                }

                int bytesRead = m_CurrentAudioStream.Read(m_SoundTouch_ByteBuffer, 0, nbytes);
                DebugFix.Assert(bytesRead == nbytes);

                int nSamplesInBuffer = (bytesRead * 8) / m_CurrentAudioPCMFormat.BitDepth; // 16


                if (m_SoundTouch_SampleBuffer == null)
                {
                    m_SoundTouch_SampleBuffer = new TSampleType[nSamplesInBuffer];
                }
                else if (m_SoundTouch_SampleBuffer.Length < nSamplesInBuffer)
                {
                    Console.WriteLine("m_SoundTouch_SampleBuffer.resize");
                    Array.Resize(ref m_SoundTouch_SampleBuffer, nSamplesInBuffer);
                }

                int sampleBufferIndex = 0;
                int byteStep = nBytesPerSample * m_CurrentAudioPCMFormat.NumberOfChannels;
                for (int i = 0; i < nbytes; i += byteStep)
                {
                    for (int channel = 0; channel < m_CurrentAudioPCMFormat.NumberOfChannels; channel++)
                    {
                        byte byte1 = m_SoundTouch_ByteBuffer[i + channel];
                        byte byte2 = m_SoundTouch_ByteBuffer[i + channel + 1];
                        var sample =
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

                int soundTouchSampleBufferLength = nSamplesInBuffer / m_CurrentAudioPCMFormat.NumberOfChannels;

                m_SoundTouch.PutSamples((ArrayPtr<TSampleType>)m_SoundTouch_SampleBuffer, soundTouchSampleBufferLength);


                int totalBytesReceived = 0;
                int samplesReceived = -1;
                while (totalBytesReceived < nbytes
                    &&
                    (samplesReceived = m_SoundTouch.ReceiveSamples(
                    (ArrayPtr<TSampleType>)m_SoundTouch_SampleBuffer,
                    soundTouchSampleBufferLength)) > 0)
                {
                    int actualSamples = samplesReceived * m_CurrentAudioPCMFormat.NumberOfChannels;

                    int bytesReceived = nBytesPerSample * actualSamples;

                    if (bytesReceived > (nbytes - totalBytesReceived))
                    {
#if DEBUG
                        // The breakpoint should never hit,
                        // because the output audio data is smaller than the source
                        // due to the time stretching making the playback faster.
                        // (the chunk of audio of a given duration was able to fit inside the circular buffer,
                        // but now is "compressed" to fit within a shorter rendering time,
                        // so it should require less data than what the buffer offers).
                        DebugFix.Assert(m_FastPlayFactor < 1);
                        if (m_FastPlayFactor >= 1)
                        {
                            Debugger.Break();
                        }
#endif // DEBUG
                        break;
                    }

                    if (BitConverter.IsLittleEndian)
                    {
                        Buffer.BlockCopy(m_SoundTouch_SampleBuffer, 0,
                        m_SoundTouch_ByteBuffer, totalBytesReceived,
                        bytesReceived);


                        //unsafe
                        //{
                        //    fixed (short* pShorts = m_SoundTouch_SampleBuffer)
                        //        Marshal.Copy((IntPtr)pShorts, 0, m_SoundTouch_ByteBuffer, actualSamples);
                        //}
                    }
                    else
                    {
#if DEBUG
                        Debugger.Break();
#endif // DEBUG
                    }

                    totalBytesReceived += bytesReceived;

#if DEBUG
                    // this may not happen with FastPlayFactor < 1 !!
                    DebugFix.Assert(totalBytesReceived <= nbytes);
#endif // DEBUG
                }

                m_SoundTouch_ByteBuffer_Stream.Position = 0;

                if (totalBytesReceived > 0)
                {
                    DebugFix.Assert(totalBytesReceived <= circularBufferLength);

                    m_CircularBuffer.Write(m_CircularBufferWritePosition, m_SoundTouch_ByteBuffer_Stream,
                                           totalBytesReceived, LockFlag.None);

                    m_CircularBufferWritePosition += totalBytesReceived;

                    m_CircularBufferWritePosition %= circularBufferLength;
                    //if (m_CircularBufferWritePosition >= m_CircularBuffer.Caps.BufferBytes)
                    //{
                    //    m_CircularBufferWritePosition -= m_CircularBuffer.Caps.BufferBytes;
                    //}
                }

                return totalBytesReceived;
            }
            else
#endif //USE_SOUNDTOUCH

            {
#if USE_SLIMDX
            if (SlimDX_IntermediaryTransferBuffer == null)
            {
                SlimDX_IntermediaryTransferBuffer = new byte[nbytes];
            }
            else if (SlimDX_IntermediaryTransferBuffer.Length != nbytes)
            {
                Array.Resize(ref SlimDX_IntermediaryTransferBuffer, nbytes);
            }
            int read = m_CurrentAudioStream.Read(SlimDX_IntermediaryTransferBuffer, 0, nbytes);
            DebugFix.Assert(nbytes == read);
            m_CircularBuffer.Write(SlimDX_IntermediaryTransferBuffer, 0, nbytes, m_CircularBufferWritePosition, LockFlags.None);
#else
                m_CircularBuffer.Write(m_CircularBufferWritePosition, m_CurrentAudioStream, nbytes, LockFlag.None);
#endif

                m_CircularBufferWritePosition += nbytes;

                m_CircularBufferWritePosition %= circularBufferLength;
                //if (m_CircularBufferWritePosition >= m_CircularBuffer.Caps.BufferBytes)
                //{
                //    m_CircularBufferWritePosition -= m_CircularBuffer.Caps.BufferBytes;
                //}

                return nbytes;
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


            WaveFormat waveFormat = new WaveFormat();
            waveFormat.FormatTag = WaveFormatTag.Pcm;

            waveFormat.Channels = (short)m_CurrentAudioPCMFormat.NumberOfChannels;
            waveFormat.SamplesPerSecond = (int)m_CurrentAudioPCMFormat.SampleRate;
            waveFormat.BitsPerSample = (short)m_CurrentAudioPCMFormat.BitDepth;
            waveFormat.AverageBytesPerSecond = (int)byteRate;
#if USE_SLIMDX
            waveFormat.BlockAlignment = (short)m_CurrentAudioPCMFormat.BlockAlign;
#else
            waveFormat.BlockAlign = (short)m_CurrentAudioPCMFormat.BlockAlign;
#endif

#if USE_SLIMDX
            SoundBufferDescription bufferDescription = new SoundBufferDescription();
            bufferDescription.Flags = BufferFlags.ControlVolume | BufferFlags.ControlFrequency | BufferFlags.GlobalFocus;
            bufferDescription.SizeInBytes = dxBufferSize;
#else
            BufferDescription bufferDescription = new BufferDescription();

            bufferDescription.ControlVolume = true;
            bufferDescription.ControlFrequency = true;
            bufferDescription.GlobalFocus = true;
            bufferDescription.BufferBytes = dxBufferSize;
#endif
            bufferDescription.Format = waveFormat;

#if USE_SLIMDX
            m_CircularBuffer = new SecondarySoundBuffer(OutputDevice.Device, bufferDescription);
#else
            m_CircularBuffer = new SecondaryBuffer(bufferDescription, OutputDevice.Device);
#endif

#if USE_SOUNDTOUCH
            if (m_SoundTouch == null)
            {
                m_SoundTouch = new SoundTouch<TSampleType, TLongSampleType>();


                m_SoundTouch.SetSetting(SettingId.UseQuickseek, 0);
                m_SoundTouch.SetSetting(SettingId.UseAntiAliasFilter, 0);

                //TODO: restore this? speech processing
                //m_SoundTouch.SetSetting(SettingId.SequenceDurationMs, 40);
                //m_SoundTouch.SetSetting(SettingId.SeekwindowDurationMs, 15);
                //m_SoundTouch.SetSetting(SettingId.OverlapDurationMs, 8);
            }

            //m_SoundTouch.Flush();
            m_SoundTouch.Clear();

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

            long predictedByteIncrement = -1;
            int circularBufferTotalBytesPlayed = -1;
            int previousCircularBufferPlayPosition = -1;
            
            int totalWriteSkips = 0;

            int sleepTime = REFRESH_INTERVAL_MS;
            
            //bool endOfAudioStream = false;
            while (true)
            {
#if USE_SLIMDX
                if (m_CircularBuffer.Status == BufferStatus.BufferLost)
                {
                    m_CircularBuffer.Restore();
                }
#else
                if (m_CircularBuffer.Status.BufferLost)
                {
                    m_CircularBuffer.Restore();
                }
#endif

#if USE_SLIMDX
                if (m_CircularBuffer.Status == BufferStatus.BufferTerminated)
                {
                    return;
                }
#else
                if (m_CircularBuffer.Status.Terminated
                    || !m_CircularBuffer.Status.Playing
                    || !m_CircularBuffer.Status.Looping
                    )
                {
                    return false;
                }
#endif




                long pcmDataRemainingPlayableFromStream = m_PlaybackEndPositionInCurrentAudioStream - m_CurrentAudioStream.Position;

                long pcmDataTotalPlayableFromStream = m_PlaybackEndPositionInCurrentAudioStream - m_PlaybackStartPositionInCurrentAudioStream;

                //long pcmDataAlreadyReadFromStream = pcmDataTotalPlayableFromStream - pcmDataAvailableFromStream;

                Thread.Sleep(sleepTime);


                if (predictedByteIncrement < 0
                    || true //m_FastPlayFactor != previousFastPlayFactor
                    )
                {
                    //previousFastPlayFactor = m_FastPlayFactor;


                    int fastPlaySamplesPerSecond = (int)Math.Round(m_CurrentAudioPCMFormat.SampleRate * m_FastPlayFactor);
#if DEBUG
                    DebugFix.Assert(m_CurrentAudioPCMFormat.SampleRate == m_CircularBuffer.Format.SamplesPerSecond);

                    if (
#if USE_SOUNDTOUCH
!UseSoundTouch &&
#endif //USE_SOUNDTOUCH
#if USE_SLIMDX
 m_CircularBuffer.Capabilities.ControlFrequency
#else
 m_CircularBuffer.Caps.ControlFrequency
#endif
)
                    {
                        DebugFix.Assert(m_CircularBuffer.Frequency == fastPlaySamplesPerSecond);

                    }
#endif //DEBUG
                    int byteRate = fastPlaySamplesPerSecond * m_CurrentAudioPCMFormat.BlockAlign; // (m_CurrentAudioPCMFormat.BitDepth / 8) * m_CurrentAudioPCMFormat.NumberOfChannels;


                    // TODO: determine time elapsed since last iteration by comparing DateTime.UtcNow.Ticks or a more efficient system timer like StopWatch
                    predictedByteIncrement = (long)(byteRate * (REFRESH_INTERVAL_MS + 15) / 1000.0);
                    predictedByteIncrement -= predictedByteIncrement % m_CurrentAudioPCMFormat.BlockAlign;
                }

#if USE_SLIMDX
                int circularBufferPlayPosition = m_CircularBuffer.CurrentPlayPosition;
#else
                int circularBufferPlayPosition = m_CircularBuffer.PlayPosition;
#endif

                int circularBufferLength =
#if USE_SLIMDX
 m_CircularBuffer.Capabilities.BufferSize
#else
 m_CircularBuffer.Caps.BufferBytes
#endif
;
                if (circularBufferTotalBytesPlayed < 0)
                {
                    circularBufferTotalBytesPlayed = circularBufferPlayPosition;
                }
                else
                {
                    if (circularBufferPlayPosition >= previousCircularBufferPlayPosition)
                    {
                        circularBufferTotalBytesPlayed += circularBufferPlayPosition - previousCircularBufferPlayPosition;
                    }
                    else
                    {
                        circularBufferTotalBytesPlayed += (circularBufferLength - previousCircularBufferPlayPosition) + circularBufferPlayPosition;
                    }
                }

                previousCircularBufferPlayPosition = circularBufferPlayPosition;


                int totalBytesPlayed_AdjustedPlaybackRate = circularBufferTotalBytesPlayed;

#if USE_SOUNDTOUCH
                if (UseSoundTouch && NotNormalPlayFactor())
                {
                    //m_CurrentAudioPCMFormat
                    totalBytesPlayed_AdjustedPlaybackRate = (int)Math.Round(totalBytesPlayed_AdjustedPlaybackRate * m_FastPlayFactor);
                    totalBytesPlayed_AdjustedPlaybackRate -= totalBytesPlayed_AdjustedPlaybackRate % m_CurrentAudioPCMFormat.BlockAlign;
                }
#endif //USE_SOUNDTOUCH

                int circularBufferBytesAvailableForWriting = (circularBufferPlayPosition == m_CircularBufferWritePosition ? 0
                                        : (circularBufferPlayPosition < m_CircularBufferWritePosition
                                  ? circularBufferPlayPosition + (circularBufferLength - m_CircularBufferWritePosition)
                                  : circularBufferPlayPosition - m_CircularBufferWritePosition));

                int circularBufferBytesAvailableForPlaying = circularBufferLength - circularBufferBytesAvailableForWriting;

                //realTimePlaybackPosition -= realTimePlaybackPosition % m_CurrentAudioPCMFormat.BlockAlign;

                //Console.WriteLine(String.Format("bytesAvailableForWriting: [{0} / {1}]", bytesAvailableForWriting, m_CircularBuffer.Caps.BufferBytes));

                //Console.WriteLine("dataAvailableFromStream: " + dataAvailableFromStream);

                int circularBufferBytesAvailableForPlaying_AdjustedPlaybackRate = circularBufferBytesAvailableForPlaying;

#if USE_SOUNDTOUCH
                if (UseSoundTouch && NotNormalPlayFactor())
                {
                    circularBufferBytesAvailableForPlaying_AdjustedPlaybackRate = (int)Math.Round(circularBufferBytesAvailableForPlaying_AdjustedPlaybackRate * m_FastPlayFactor);
                    circularBufferBytesAvailableForPlaying_AdjustedPlaybackRate -= circularBufferBytesAvailableForPlaying_AdjustedPlaybackRate % m_CurrentAudioPCMFormat.BlockAlign;
                }
#endif //USE_SOUNDTOUCH

                long remainingBytesToPlay = pcmDataTotalPlayableFromStream - totalBytesPlayed_AdjustedPlaybackRate;

                long realTimePlaybackPosition = Math.Max(m_PlaybackStartPositionInCurrentAudioStream,
                    m_CurrentAudioStream.Position - Math.Min(
                    circularBufferBytesAvailableForPlaying_AdjustedPlaybackRate, remainingBytesToPlay));

                realTimePlaybackPosition = Math.Min(realTimePlaybackPosition,
                    m_PlaybackStartPositionInCurrentAudioStream + totalBytesPlayed_AdjustedPlaybackRate);

                if (m_CurrentBytePosition == m_PlaybackStartPositionInCurrentAudioStream)
                {
                    //Console.WriteLine(string.Format("m_CurrentBytePosition ASSIGNED: realTimePlaybackPosition [{0}]", realTimePlaybackPosition));

                    m_CurrentBytePosition += totalBytesPlayed_AdjustedPlaybackRate;
                }
                else if (realTimePlaybackPosition < m_CurrentBytePosition)
                {
                    Console.WriteLine(string.Format("realTimePlaybackPosition [{0}] < m_CurrentBytePosition [{1}]", realTimePlaybackPosition, m_CurrentBytePosition));

                    m_CurrentBytePosition = Math.Min(m_PlaybackEndPositionInCurrentAudioStream, m_CurrentBytePosition + predictedByteIncrement);
                }
                else if (realTimePlaybackPosition > m_CurrentBytePosition + predictedByteIncrement)
                {
                    Console.WriteLine(string.Format("realTimePlaybackPosition [{0}] > m_CurrentBytePosition [{1}] + m_PredictedByteIncrement: [{2}] (diff: [{3}])",
                        realTimePlaybackPosition, m_CurrentBytePosition, predictedByteIncrement, realTimePlaybackPosition - m_CurrentBytePosition));

                    m_CurrentBytePosition = Math.Min(m_PlaybackEndPositionInCurrentAudioStream, m_CurrentBytePosition + predictedByteIncrement);
                }
                else
                {
                    //Console.WriteLine(string.Format("m_CurrentBytePosition OK: realTimePlaybackPosition [{0}]", realTimePlaybackPosition));

                    m_CurrentBytePosition = m_PlaybackStartPositionInCurrentAudioStream + totalBytesPlayed_AdjustedPlaybackRate;
                }



#if DEBUG
                DebugFix.Assert(m_CurrentBytePosition >= m_PlaybackStartPositionInCurrentAudioStream);
                DebugFix.Assert(m_CurrentBytePosition <= m_PlaybackEndPositionInCurrentAudioStream);
#endif // DEBUG
                if (m_CurrentBytePosition < m_PlaybackStartPositionInCurrentAudioStream)
                {
                    m_CurrentBytePosition = m_PlaybackStartPositionInCurrentAudioStream;
                }
                else if (m_CurrentBytePosition > m_PlaybackEndPositionInCurrentAudioStream)
                {
                    m_CurrentBytePosition = m_PlaybackEndPositionInCurrentAudioStream;
                }



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

#if USE_SLIMDX
                    if (SlimDX_IntermediaryTransferBuffer != null)
                    {
                        Array.Copy(SlimDX_IntermediaryTransferBuffer, m_PcmDataBuffer, Math.Min(m_PcmDataBuffer.Length, SlimDX_IntermediaryTransferBuffer.Length));
                        m_PcmDataBufferAvailableEventArgs.PcmDataBuffer = m_PcmDataBuffer;
                        del  (this, m_PcmDataBufferAvailableEventArgs);
                    }
#else

#if FETCH_PCM_FROM_CIRCULAR_BUFFER
                    byte[] array = (byte[])m_CircularBuffer.Read(
                        circularBufferPlayPosition, typeof(byte), LockFlag.None, min);
                    //Array.Copy(array, m_PcmDataBuffer, min);
                    Buffer.BlockCopy(array, 0,
                            m_PcmDataBuffer, 0,
                            min);
#else
                    long pos = m_CurrentAudioStream.Position;

                    m_CurrentAudioStream.Position = m_CurrentBytePosition;

                    m_CurrentAudioStream.Read(m_PcmDataBuffer, 0, min);

                    m_CurrentAudioStream.Position = pos;
#endif // FETCH_PCM_FROM_CIRCULAR_BUFFER

                    m_PcmDataBufferAvailableEventArgs.PcmDataBuffer = m_PcmDataBuffer;
                    m_PcmDataBufferAvailableEventArgs.PcmDataBufferLength = min;
                    del(this, m_PcmDataBufferAvailableEventArgs);
#endif
                }
                //var del_ = PcmDataBufferAvailable;
                //if (del_ != null
                //&& m_PcmDataBuffer.Length <= circularBufferBytesAvailableForPlaying)
                //{
                //#if USE_SLIMDX
                //if (SlimDX_IntermediaryTransferBuffer != null)
                //{
                //Array.Copy(SlimDX_IntermediaryTransferBuffer, m_PcmDataBuffer, Math.Min(m_PcmDataBuffer.Length, SlimDX_IntermediaryTransferBuffer.Length));
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

                if (circularBufferBytesAvailableForWriting <= 0)
                {
                    if (pcmDataRemainingPlayableFromStream > 0)
                    {
                        Console.WriteLine("circularBufferBytesAvailableForWriting <= 0, pcmDataAvailableFromStream > 0 ... continue...");
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
                    if (remainingBytesToPlay > predictedByteIncrement)
                    {
                        Console.WriteLine(string.Format("remainingBytesToPlay [{0}] [{1}] [{2}] [{3}]",
                            pcmDataTotalPlayableFromStream, totalBytesPlayed_AdjustedPlaybackRate, remainingBytesToPlay, predictedByteIncrement));
                        continue;
                    }
                    else
                    {
                        m_CircularBuffer.Stop();

                        //Console.WriteLine("Time to break, all bytes gone.");
                        //break;

                        return true;
                    }


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
                    int circularBufferBytesAvailableForWriting_AdjustedPlaybackRate =
                        circularBufferBytesAvailableForWriting;

#if USE_SOUNDTOUCH
                    if (UseSoundTouch && NotNormalPlayFactor())
                    {
                        circularBufferBytesAvailableForWriting_AdjustedPlaybackRate =
                            (int)
                            Math.Round(circularBufferBytesAvailableForWriting_AdjustedPlaybackRate * m_FastPlayFactor);
                        circularBufferBytesAvailableForWriting_AdjustedPlaybackRate -=
                            circularBufferBytesAvailableForWriting_AdjustedPlaybackRate %
                            m_CurrentAudioPCMFormat.BlockAlign;
                    }
#endif //USE_SOUNDTOUCH


                    long bytesToTransferToCircularBuffer =
                        Math.Min(circularBufferBytesAvailableForWriting_AdjustedPlaybackRate,
                                 pcmDataRemainingPlayableFromStream);
                    //bytesToTransferToCircularBuffer = Math.Min(bytesToTransferToCircularBuffer, m_CircularBufferRefreshChunkSize);
                    bytesToTransferToCircularBuffer -= bytesToTransferToCircularBuffer %
                                                       m_CurrentAudioPCMFormat.BlockAlign;

                    if (bytesToTransferToCircularBuffer <= 0)
                    {
                        Debug.Fail("bytesToTransferToCircularBuffer <= 0 !!?");
                        continue;
                    }

                    //Console.WriteLine(String.Format("m_CircularBufferWritePosition: [{0} / toCopy {1}]", m_CircularBufferWritePosition, toCopy));

#if DEBUG
                    DebugFix.Assert(m_CurrentAudioStream.Position + bytesToTransferToCircularBuffer <=
                                    m_PlaybackEndPositionInCurrentAudioStream);
#endif//DEBUG
                    //bool mustFlush = pcmDataRemainingPlayableFromStream <
                    //                 circularBufferBytesAvailableForWriting_AdjustedPlaybackRate;
                    if (circularBufferBytesAvailableForWriting < (int)Math.Round(circularBufferLength * 2 / 3.0)) // 2 thirds minimum must be available
                    {
                        totalWriteSkips++;
                    }
                    else
                    {
                        //Console.WriteLine("totalWriteSkips: " + totalWriteSkips + " ms: " + totalWriteSkips*REFRESH_INTERVAL_MS);
                        totalWriteSkips = 0;

                        int bytesWrittenToCirularBuffer =
                            transferBytesFromWavStreamToCircularBuffer((int)bytesToTransferToCircularBuffer);

#if DEBUG
                        if (
#if USE_SOUNDTOUCH
!UseSoundTouch ||
#endif
                            //USE_SOUNDTOUCH
                            !NotNormalPlayFactor())
                        {
                            DebugFix.Assert(bytesWrittenToCirularBuffer == bytesToTransferToCircularBuffer);
                        }
#endif//DEBUG
                    }
                }
            }

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
