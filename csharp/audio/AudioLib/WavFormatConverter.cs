using System;
using System.IO;
using System.Runtime.InteropServices;
using javazoom.jl.decoder;
using NAudio.CoreAudioApi;
using NAudio.Wave;
using System.Diagnostics;

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

namespace AudioLib
{

    public class WavSoundTouch
    {
        SoundTouch<TSampleType, TLongSampleType> m_SoundTouch;

        private byte[] m_SoundTouch_ByteBuffer = null;
        private TSampleType[] m_SoundTouch_SampleBuffer = null;

        public WavSoundTouch(string fullpath, float factor
            //, AudioLibPCMFormat audioPCMFormat
            )
        {
            AudioLibPCMFormat audioPCMFormat = null;

            string fullpath_ = fullpath + "_.wav";

            m_SoundTouch = new SoundTouch<TSampleType, TLongSampleType>();

            m_SoundTouch.SetSetting(SettingId.UseAntiAliasFilter, 1);

            // Speech optimised
            m_SoundTouch.SetSetting(SettingId.SequenceDurationMs, 40);
            m_SoundTouch.SetSetting(SettingId.SeekwindowDurationMs, 15);
            m_SoundTouch.SetSetting(SettingId.OverlapDurationMs, 8);

            ////m_SoundTouch.Flush();
            //m_SoundTouch.Clear();

            m_SoundTouch.SetSetting(SettingId.UseQuickseek, 0);


            m_SoundTouch.SetTempo(factor);
            //m_SoundTouch.SetTempoChange(factor * 100);

            m_SoundTouch.SetPitchSemiTones(0);
            m_SoundTouch.SetRateChange(0);

            Stream audioStream = null;
            ulong audioStreamRiffOffset = 0;
            Stream destStream = null;

            bool okay = true;
            try
            {
                audioStream = File.Open(fullpath, FileMode.Open, FileAccess.Read, FileShare.Read);

                uint dataLength;
                AudioLibPCMFormat pcmInfo = null;

                pcmInfo = AudioLibPCMFormat.RiffHeaderParse(audioStream, out dataLength);

                //audioPCMFormat = new PCMFormatInfo(pcmInfo);
                audioPCMFormat = new AudioLibPCMFormat();
                audioPCMFormat.CopyFrom(pcmInfo);

                //AudioLib.SampleRate.Hz22050
                if (audioPCMFormat.SampleRate > 22050)
                {
                    m_SoundTouch.SetSetting(SettingId.UseQuickseek, 1);
                }
                m_SoundTouch.SetSampleRate((int) audioPCMFormat.SampleRate);
                m_SoundTouch.SetChannels((int) audioPCMFormat.NumberOfChannels);

                //destStream = File.Open(fullpath_, FileMode.CreateNew, FileAccess.Write, FileShare.Write);
                destStream = new FileStream(fullpath_, FileMode.Create, FileAccess.Write, FileShare.None);
                audioStreamRiffOffset = audioPCMFormat.RiffHeaderWrite(destStream, 0);
                DebugFix.Assert((long) audioStreamRiffOffset == destStream.Position);


                int sampleSizePerChannel = audioPCMFormat.BitDepth/8;
                // == audioPCMFormat.BlockAlign / audioPCMFormat.NumberOfChannels;

#if DEBUG
                int sizeOfTypeInBytes = Marshal.SizeOf(typeof (TSampleType));
                DebugFix.Assert(sizeOfTypeInBytes == sampleSizePerChannel);

                sizeOfTypeInBytes = sizeof (TSampleType);
                DebugFix.Assert(sizeOfTypeInBytes == sampleSizePerChannel);
#endif // DEBUG

                int bytesToTransfer =
                    (int)
                        Math.Min(audioStream.Length, audioPCMFormat.ConvertTimeToBytes(1000*AudioLibPCMFormat.TIME_UNIT));

                if (m_SoundTouch_ByteBuffer == null)
                {
                    Console.WriteLine("ALLOCATING m_SoundTouch_ByteBuffer");
                    m_SoundTouch_ByteBuffer = new byte[bytesToTransfer];
                }
                //else if (m_SoundTouch_ByteBuffer.Length < bytesToTransfer)
                //{
                //    Console.WriteLine("m_SoundTouch_ByteBuffer.resize");
                //    Array.Resize(ref m_SoundTouch_ByteBuffer, bytesToTransfer);
                //}

                //int bytesToReadFromAudioStream = bytesToTransferToCircularBuffer;
                //DebugFix.Assert(bytesToReadFromAudioStream <= bytesToTransferToCircularBuffer);

                int bytesReadFromAudioStream = 0;

                while ((bytesReadFromAudioStream = audioStream.Read(m_SoundTouch_ByteBuffer, 0, bytesToTransfer)) > 0)
                {
                    DebugFix.Assert(bytesReadFromAudioStream <= bytesToTransfer);

                    int soundTouch_SampleBufferLength = (bytesReadFromAudioStream*8)/audioPCMFormat.BitDepth; // 16


                    if (m_SoundTouch_SampleBuffer == null)
                    {
                        Console.WriteLine("ALLOCATING m_SoundTouch_SampleBuffer");
                        m_SoundTouch_SampleBuffer = new TSampleType[soundTouch_SampleBufferLength];
                    }
                    //else if (m_SoundTouch_SampleBuffer.Length < soundTouch_SampleBufferLength)
                    //{
                    //    Console.WriteLine("m_SoundTouch_SampleBuffer.resize");
                    //    Array.Resize(ref m_SoundTouch_SampleBuffer, soundTouch_SampleBufferLength);
                    //}

                    int sampleBufferIndex = 0;
                    for (int i = 0; i < bytesReadFromAudioStream;) //i += m_CurrentAudioPCMFormat.BlockAlign)
                    {
                        //short sampleLeft = 0;
                        //short sampleRight = 0;

                        for (int channel = 0; channel < audioPCMFormat.NumberOfChannels; channel++)
                        {
                            byte byte1 = m_SoundTouch_ByteBuffer[i++];
                            byte byte2 = m_SoundTouch_ByteBuffer[i++];

                            short sample =
                                BitConverter.IsLittleEndian
                                    ? (short) (byte1 | (byte2 << 8))
                                    : (short) ((byte1 << 8) | byte2);

#if DEBUG
                            //// Little Indian
                            //short s1 = (short)(byte1 | (byte2 << 8));
                            //short s2 = (short)(byte1 + byte2 * 256);

                            //// Big Indian
                            //short s3 = (short)((byte1 << 8) | byte2);
                            //short s4 = (short)(byte1 * 256 + byte2);

                            short checkedSample = BitConverter.ToInt16(m_SoundTouch_ByteBuffer, i - 2);
                            DebugFix.Assert(checkedSample == sample);

                            checkedSample = (short) (byte1 + byte2*256);
                            DebugFix.Assert(checkedSample == sample);
#endif //DEBUG

                            //if (channel == 0)
                            //{
                            //    sampleLeft = sample;
                            //}
                            //else
                            //{
                            //    sampleRight = sample;
                            //}

                            m_SoundTouch_SampleBuffer[sampleBufferIndex++] = sample;
                        }

                        //m_SoundTouch_SampleBuffer[sampleBufferIndex - 2] = sampleRight; // sampleLeft;
                        //m_SoundTouch_SampleBuffer[sampleBufferIndex - 1] = 0; // sampleRight;
                    }


                    int soundTouch_SampleBufferLength_Channels = soundTouch_SampleBufferLength/
                                                                 audioPCMFormat.NumberOfChannels;

                    int soundTouch_SampleBufferLength_Channels_FULL = m_SoundTouch_SampleBuffer.Length/
                                                                      audioPCMFormat.NumberOfChannels;

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

                    m_SoundTouch.PutSamples((ArrayPtr<TSampleType>) m_SoundTouch_SampleBuffer,
                        soundTouch_SampleBufferLength_Channels);

                    int totalBytesReceivedFromSoundTouch = 0;

                    //totalBytesReceivedFromSoundTouch = soundTouch_SampleBufferLength * sampleSizePerChannel;

                    while (
                        (samplesReceived = m_SoundTouch.ReceiveSamples(
                            (ArrayPtr<TSampleType>) m_SoundTouch_SampleBuffer,
                            soundTouch_SampleBufferLength_Channels_FULL
                            )) > 0)
                    {
                        samplesReceived *= audioPCMFormat.NumberOfChannels;

                        int bytesReceivedFromSoundTouch = samplesReceived*sampleSizePerChannel;

                        int predictedTotal = totalBytesReceivedFromSoundTouch + bytesReceivedFromSoundTouch;
                        if ( //predictedTotal > bytesReadFromAudioStream ||
                            predictedTotal > m_SoundTouch_ByteBuffer.Length)
                        {
#if DEBUG
                            // The breakpoint should never hit,
                            // because the output audio data is smaller than the source
                            // due to the time stretching making the playback faster.
                            // (the ratio is about the same as factor)

                            //DebugFix.Assert(factor < 1);

                            if (factor >= 1)
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

                            for (int s = 0; s < samplesReceived; s += audioPCMFormat.NumberOfChannels)
                            {
                                if (false)
                                {
                                    int sampleSizePerChannels = sampleSizePerChannel*audioPCMFormat.NumberOfChannels;

                                    checkTotalBytes += sampleSizePerChannels;

                                    // TODO: check little / big endian
                                    Buffer.BlockCopy(m_SoundTouch_SampleBuffer,
                                        s*sampleSizePerChannel,
                                        m_SoundTouch_ByteBuffer,
                                        totalBytesReceivedFromSoundTouch
                                        + s*sampleSizePerChannel,
                                        sampleSizePerChannels);
                                }
                                else
                                {
                                    for (int channel = 0; channel < audioPCMFormat.NumberOfChannels; channel++)
                                    {
                                        if (true)
                                        {
                                            checkTotalBytes += sampleSizePerChannel;

                                            sampleBufferIndex = (s + channel);
                                            //TSampleType sample = m_SoundTouch_SampleBuffer[sampleBufferIndex];

                                            int byteIndex = sampleBufferIndex*sampleSizePerChannel;

                                            // TODO: check little / big endian
                                            Buffer.BlockCopy(m_SoundTouch_SampleBuffer,
                                                byteIndex,
                                                m_SoundTouch_ByteBuffer,
                                                totalBytesReceivedFromSoundTouch + byteIndex,
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
                                                        (short) ((sample & 0xFF) << 8 | (sample & 0xFF00) >> 8));
                                            }

                                            DebugFix.Assert(sampleSizePerChannel == sampleBytes.Length);

                                            checkTotalBytes += sampleSizePerChannel;

                                            Buffer.BlockCopy(sampleBytes,
                                                0,
                                                m_SoundTouch_ByteBuffer,
                                                totalBytesReceivedFromSoundTouch
                                                + (s + channel)*sampleSizePerChannel,
                                                sampleSizePerChannel);
                                        }
                                    }
                                }
                            }

                            DebugFix.Assert(checkTotalBytes == bytesReceivedFromSoundTouch);


                            //if (bytesReceivedFromSoundTouch > 0
                            //    //&& totalBytesReceivedFromSoundTouch <= circularBufferBytesAvailableForWriting
                            //    )
                            //{
                            //    destStream.Write(m_SoundTouch_ByteBuffer, totalBytesReceivedFromSoundTouch, bytesReceivedFromSoundTouch);
                            //}
                        }

                        totalBytesReceivedFromSoundTouch += bytesReceivedFromSoundTouch;

                        //if (//totalBytesReceivedFromSoundTouch >= bytesReadFromAudioStream ||
                        //    totalBytesReceivedFromSoundTouch >= circularBufferBytesAvailableForWriting)
                        //{
                        //    break;
                        //}
                    }

                    if (totalBytesReceivedFromSoundTouch > 0
                        //&& totalBytesReceivedFromSoundTouch <= circularBufferBytesAvailableForWriting
                        )
                    {
                        destStream.Write(m_SoundTouch_ByteBuffer, 0, totalBytesReceivedFromSoundTouch);
                    }
                }
            }
            catch (Exception ex)
            {
                okay = false;
            }
            finally
            {
                if (audioStream != null)
                {
                    audioStream.Close();
                }
                if (destStream != null)
                {
                    destStream.Position = 0;
                    audioStreamRiffOffset = audioPCMFormat.RiffHeaderWrite(
                        destStream,
                        (uint)
                            (destStream.Length -
                             (long)
                                 audioStreamRiffOffset));
                    destStream.Close();
                }
            }

            if (okay && File.Exists(fullpath_))
            {
                File.Delete(fullpath);
                File.Move(fullpath_, fullpath);
            }
        }
    }

    public class WavFormatConverter : DualCancellableProgressReporter
    {
        static WavFormatConverter()
        {
#if DEBUG
            string workingDir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string lameExe = Path.Combine(workingDir, "lame.exe"); ;
            if (!File.Exists(lameExe))
            {
                Debugger.Break();
            }
            string faadExe = Path.Combine(workingDir, "faad.exe"); ;
            if (!File.Exists(faadExe))
            {
                Debugger.Break();
            }
#endif //DEBUG
        }

        public const string AUDIO_MP3_EXTENSION = ".mp3";
        public const string AUDIO_WAV_EXTENSION = ".wav";

        public override void DoWork()
        {
        }

        private bool m_OverwriteOutputFiles;
        public bool OverwriteOutputFiles
        {
            get { return m_OverwriteOutputFiles; }
            set { m_OverwriteOutputFiles = value; }
        }

        public WavFormatConverter(bool overwriteOutputFiles, bool skipACM)
        {
            OverwriteOutputFiles = overwriteOutputFiles;
            m_SkipACM = skipACM;
        }

        private const uint PROGRESS_INTERVAL_MS = 500;

        public string ConvertSampleRate(string sourceFile, string destinationDirectory, AudioLibPCMFormat pcmFormat, out AudioLibPCMFormat originalPcmFormat)
        {
            if (!File.Exists(sourceFile))
                throw new FileNotFoundException("Invalid source file path");

            if (!Directory.Exists(destinationDirectory))
                throw new FileNotFoundException("Invalid destination directory");

            string destinationFilePath = null;
            WaveStream sourceStream = null;
            WaveFormatConversionStream conversionStream = null;
            Stopwatch watch = new Stopwatch();
            try
            {
                WaveFormat destFormat = new WaveFormat((int)pcmFormat.SampleRate,
                                                                   pcmFormat.BitDepth,
                                                                   pcmFormat.NumberOfChannels);
                sourceStream = new WaveFileReader(sourceFile);

                conversionStream = new WaveFormatConversionStream(destFormat, sourceStream);

                destinationFilePath = GenerateOutputFileFullname(sourceFile, destinationDirectory, pcmFormat);

                WaveFormat sourceFormat = sourceStream.WaveFormat;
                originalPcmFormat = new AudioLibPCMFormat((ushort)sourceFormat.Channels, (uint)sourceFormat.SampleRate, (ushort)sourceFormat.BitsPerSample);
                bool formatsEqual1 = sourceFormat.Equals(destFormat);
                bool formatsEqual2 = originalPcmFormat.Equals(pcmFormat);
                DebugFix.Assert(formatsEqual1 && formatsEqual1 || !formatsEqual1 && !formatsEqual1);
                if (formatsEqual1 || formatsEqual2)
                {
#if DEBUG
                    Debugger.Break();
#endif //DEBUG
                    File.Copy(sourceFile, destinationFilePath);
                    try
                    {
                        File.SetAttributes(destinationFilePath, FileAttributes.Normal);
                    }
                    catch
                    {
                    }
                }
                else
                {

                    //WaveFileWriter.CreateWaveFile(destinationFilePath, conversionStream);
                    using (WaveFileWriter writer = new WaveFileWriter(destinationFilePath, conversionStream.WaveFormat))
                    {
                        //const int BUFFER_SIZE = 1024 * 8; // 8 KB MAX BUFFER
                        int BUFFER_SIZE = (int)pcmFormat.ConvertTimeToBytes(1500 * AudioLibPCMFormat.TIME_UNIT);
                        //int debug = pcmStream.GetReadSize(4000);
                        byte[] buffer = new byte[BUFFER_SIZE];
                        int byteRead;
                        writer.Flush();

                        string msg = Path.GetFileName(sourceFile) + " / " + Path.GetFileName(destinationFilePath);
                        //"Resampling WAV audio...";
                        reportProgress(-1, msg);
                        watch.Start();
                        while ((byteRead = conversionStream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            if (RequestCancellation)
                            {
                                writer.Close();
                                if (File.Exists(destinationFilePath))
                                {
                                    File.Delete(destinationFilePath);
                                }
                                return null;
                            }

                            if (watch.ElapsedMilliseconds >= PROGRESS_INTERVAL_MS)
                            {
                                watch.Stop();

                                int percent = (int)(100.0 * sourceStream.Position / sourceStream.Length);
                                reportProgress(percent, msg); // + sourceStream.Position + "/" + sourceStream.Length);

                                watch.Reset();
                                watch.Start();
                            }

                            writer.WriteData(buffer, 0, byteRead);
                        }
                    }
                }
            }
            finally
            {
                watch.Stop();

                if (conversionStream != null)
                {
                    conversionStream.Close();
                }
                if (sourceStream != null)
                {
                    sourceStream.Close();
                }
            }

            return destinationFilePath;
        }

        private AudioLibPCMFormat Mp3ToWav_Mp3Sharp(string mp3FilePath, string destinationDirectory, out string wavFilePath)
        {
            Stream mp3Stream = File.Open(mp3FilePath, FileMode.Open, FileAccess.Read, FileShare.Read);

            Bitstream mp3BitStream = new Bitstream(new BackStream(mp3Stream, 1024 * 10));
            Header mp3Frame = mp3BitStream.readFrame();

            AudioLibPCMFormat mp3PcmFormat = new AudioLibPCMFormat(
                (ushort)(mp3Frame.mode() == Header.SINGLE_CHANNEL ? 1 : 2),
                (uint)mp3Frame.frequency(),
                16);

            wavFilePath = GenerateOutputFileFullname(
                                    mp3FilePath + WavFormatConverter.AUDIO_WAV_EXTENSION,
                                    destinationDirectory,
                                    mp3PcmFormat);

            FileStream wavFileStream = new FileStream(wavFilePath, FileMode.Create, FileAccess.Write, FileShare.Read);

            ulong wavFilRiffHeaderLength = mp3PcmFormat.RiffHeaderWrite(wavFileStream, 0);

            long wavFileStreamStart = wavFileStream.Position;

            Decoder decoder = new Decoder();
            ObufferStreamWrapper outputBuffer = new ObufferStreamWrapper(wavFileStream, mp3PcmFormat.NumberOfChannels);
            decoder.OutputBuffer = outputBuffer;

            string msg = Path.GetFileName(mp3FilePath) + " / " + Path.GetFileName(wavFilePath); // "Decoding MP3 to WAV audio (CSharp Lib)..."
            reportProgress(-1, msg);
            Stopwatch watch = new Stopwatch();
            try
            {
                watch.Start();
                while (true)
                {
                    if (RequestCancellation)
                    {
                        mp3Stream.Close();
                        wavFileStream.Close();
                        if (File.Exists(wavFilePath))
                        {
                            File.Delete(wavFilePath);
                        }
                        return null;
                    }

                    if (watch.ElapsedMilliseconds >= PROGRESS_INTERVAL_MS)
                    {
                        watch.Stop();

                        int percent = (int)(100.0 * mp3Stream.Position / mp3Stream.Length);
                        reportProgress(percent, msg); // + mp3Stream.Position + "/" + mp3Stream.Length);

                        watch.Reset();
                        watch.Start();
                    }

                    Header header = mp3BitStream.readFrame();
                    if (header == null)
                    {
                        break;
                    }

                    try
                    {
                        Obuffer decoderOutput = decoder.decodeFrame(header, mp3BitStream);
                    }
                    catch
                    {
                        mp3BitStream.closeFrame();
                        continue;
                    }
                    mp3BitStream.closeFrame();
                }
            }
            finally
            {
                watch.Stop();
            }

            mp3Stream.Close();

            long wavFileStreamEnd = wavFileStream.Position;

            if ((ulong)wavFileStreamEnd == wavFilRiffHeaderLength)
            {
                wavFileStream.Close();
                if (File.Exists(wavFilePath))
                {
                    File.Delete(wavFilePath);
                }
                return null;
            }

            wavFileStream.Position = 0;
            mp3PcmFormat.RiffHeaderWrite(wavFileStream, (uint)(wavFileStreamEnd - wavFileStreamStart));

            wavFileStream.Close();

            return mp3PcmFormat;
        }

        private AudioLibPCMFormat Mp3ToWav_NAudio(string mp3FilePath, string destinationDirectory, out string wavFilePath)
        {
            AudioLibPCMFormat mp3PcmFormat = null;

            using (Mp3FileReader reader = new Mp3FileReader(mp3FilePath))
            {
                using (WaveStream pcmStream = WaveFormatConversionStream.CreatePcmStream(reader))
                {
                    mp3PcmFormat = new AudioLibPCMFormat(
                        (ushort)pcmStream.WaveFormat.Channels,
                        (uint)pcmStream.WaveFormat.SampleRate,
                        (ushort)pcmStream.WaveFormat.BitsPerSample);

                    wavFilePath = GenerateOutputFileFullname(
                                            mp3FilePath + WavFormatConverter.AUDIO_WAV_EXTENSION,
                                            destinationDirectory,
                                            mp3PcmFormat);

                    //WaveFileWriter.CreateWaveFile(wavFilePath, pcmStream);
                    using (WaveFileWriter writer = new WaveFileWriter(wavFilePath, pcmStream.WaveFormat))
                    {
                        //const int BUFFER_SIZE = 1024 * 8; // 8 KB MAX BUFFER
                        int BUFFER_SIZE = (int)mp3PcmFormat.ConvertTimeToBytes(1500 * AudioLibPCMFormat.TIME_UNIT);
                        //int debug = pcmStream.GetReadSize(4000);
                        byte[] buffer = new byte[BUFFER_SIZE];
                        int byteRead;
                        writer.Flush();

                        string msg = Path.GetFileName(mp3FilePath) + " / " + Path.GetFileName(wavFilePath); //"Decoding MP3 to WAV audio (ACM Codec) ..."
                        reportProgress(-1, msg);
                        Stopwatch watch = new Stopwatch();
                        try
                        {
                            watch.Start();
                            while ((byteRead = pcmStream.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                if (RequestCancellation)
                                {
                                    writer.Close();
                                    if (File.Exists(wavFilePath))
                                    {
                                        File.Delete(wavFilePath);
                                    }
                                    return null;
                                }

                                if (watch.ElapsedMilliseconds >= PROGRESS_INTERVAL_MS)
                                {
                                    watch.Stop();

                                    int percent = (int)(100.0 * reader.Position / reader.Length);
                                    reportProgress(percent, msg); // + reader.Position + "/" + reader.Length);

                                    watch.Reset();
                                    watch.Start();
                                }

                                writer.WriteData(buffer, 0, byteRead);
                            }
                        }
                        finally
                        {
                            watch.Stop();
                        }
                    }
                }
            }

            return mp3PcmFormat;
        }

        public string UnCompressMp4_AACFile(string sourceFile, string destinationDirectory, AudioLibPCMFormat pcmFormat, out AudioLibPCMFormat originalPcmFormat)
        {
            if (!File.Exists(sourceFile))
                throw new FileNotFoundException("Invalid source file path");

            if (!Directory.Exists(destinationDirectory))
                throw new FileNotFoundException("Invalid destination directory");

            originalPcmFormat = null;

            //        AudioLibPCMFormat mp4PcmFormat = new AudioLibPCMFormat(
            //            (ushort)pcmStream.WaveFormat.Channels,
            //            (uint)pcmStream.WaveFormat.SampleRate,
            //            (ushort)pcmStream.WaveFormat.BitsPerSample);

            //AudioLibPCMFormat mp4PcmFormat = new AudioLibPCMFormat(
            //    (ushort)(mp3Frame.mode() == Header.SINGLE_CHANNEL ? 1 : 2),
            //    (uint)mp3Frame.frequency(),
            //    16);


            string destinationFile = GenerateOutputFileFullname(
                                    sourceFile + WavFormatConverter.AUDIO_WAV_EXTENSION,
                                    destinationDirectory,
                                    null);

            AudioLibPCMFormat mp4PcmFormat = null;

            string workingDir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            Process mp4_AACDecodeProcess = new Process();

            mp4_AACDecodeProcess.StartInfo.FileName = Path.Combine(workingDir, "faad.exe");
            mp4_AACDecodeProcess.StartInfo.RedirectStandardOutput = false;
            mp4_AACDecodeProcess.StartInfo.RedirectStandardError = false;
            mp4_AACDecodeProcess.StartInfo.UseShellExecute = true;
            mp4_AACDecodeProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            mp4_AACDecodeProcess.StartInfo.Arguments = "-o \"" + destinationFile + "\" \"" + sourceFile + "\"";

            mp4_AACDecodeProcess.Start();
            mp4_AACDecodeProcess.WaitForExit();

            if (!mp4_AACDecodeProcess.StartInfo.UseShellExecute && mp4_AACDecodeProcess.ExitCode != 0)
            {
                StreamReader stdErr = mp4_AACDecodeProcess.StandardError;
                if (!stdErr.EndOfStream)
                {
                    string toLog = stdErr.ReadToEnd();
                    if (!string.IsNullOrEmpty(toLog))
                    {
                        Console.WriteLine(toLog);
                    }
                }
            }
            else if (!mp4_AACDecodeProcess.StartInfo.UseShellExecute)
            {
                StreamReader stdOut = mp4_AACDecodeProcess.StandardOutput;
                if (!stdOut.EndOfStream)
                {
                    string toLog = stdOut.ReadToEnd();
                    if (!string.IsNullOrEmpty(toLog))
                    {
                        Console.WriteLine(toLog);
                    }
                }
            }

            if (File.Exists(destinationFile))
            {
                uint dataLength;
                Stream stream = null;
                try
                {
                    stream = File.Open(destinationFile, FileMode.Open, FileAccess.Read, FileShare.Read);
                    if (stream != null)
                    {
                        mp4PcmFormat = AudioLibPCMFormat.RiffHeaderParse(stream, out dataLength);
                    }
                }
                finally
                {
                    if (stream != null)
                    {
                        stream.Close();
                    }
                }

                if (mp4PcmFormat != null)
                {
                    originalPcmFormat = new AudioLibPCMFormat();
                    originalPcmFormat.CopyFrom(mp4PcmFormat);
                }

                if (mp4PcmFormat != null)
                {
                    string destinationFileUPDATED = GenerateOutputFileFullname(
                        sourceFile + WavFormatConverter.AUDIO_WAV_EXTENSION,
                        destinationDirectory,
                        mp4PcmFormat);

                    try
                    {
                        File.Move(destinationFile, destinationFileUPDATED);
                        try
                        {
                            File.SetAttributes(destinationFileUPDATED, FileAttributes.Normal);
                        }
                        catch
                        {
                        }
                        destinationFile = destinationFileUPDATED;
                    }
                    catch
                    {
                    }
                }
            }

            if (mp4PcmFormat == null || RequestCancellation)
            {
                if (File.Exists(destinationFile))
                {
                    File.Delete(destinationFile);
                }
                return null;
            }

            DebugFix.Assert(File.Exists(destinationFile));

            if (pcmFormat == null) // auto detect
            {
                pcmFormat = mp4PcmFormat;
            }

            if (pcmFormat != null
                &&
                !mp4PcmFormat.IsCompatibleWith(pcmFormat))
            {
                AudioLibPCMFormat originalWavPcmFormat;
                string newDestinationFilePath = ConvertSampleRate(destinationFile, destinationDirectory, pcmFormat, out originalWavPcmFormat);
                if (originalWavPcmFormat != null)
                {
                    DebugFix.Assert(mp4PcmFormat.Equals(originalWavPcmFormat));
                }


                if (File.Exists(destinationFile))
                {
                    File.Delete(destinationFile);
                }
                if (RequestCancellation)
                {
                    return null;
                }
                return newDestinationFilePath;
            }

            return destinationFile;
        }

        private bool m_SkipACM;
        public string UnCompressMp3File(string sourceFile, string destinationDirectory, AudioLibPCMFormat pcmFormat, out AudioLibPCMFormat originalPcmFormat)
        {
            if (!File.Exists(sourceFile))
                throw new FileNotFoundException("Invalid source file path");

            if (!Directory.Exists(destinationDirectory))
                throw new FileNotFoundException("Invalid destination directory");

            originalPcmFormat = null;

            string destinationFile = null;
            AudioLibPCMFormat mp3PcmFormat = null;

            #region PERFORMANCE TESTING
            //Stopwatch watch = new Stopwatch();

            //watch.Start();
            //mp3PcmFormat = Mp3ToWav_Mp3Sharp(sourceFile, destinationFile);
            //watch.Stop();
            //long csMS = watch.ElapsedMilliseconds;

            //if (File.Exists(destinationFile))
            //{
            //    File.Delete(destinationFile);
            //}

            //watch.Reset();
            //watch.Start();
            //mp3PcmFormat = Mp3ToWav_NAudio(sourceFile, destinationFile);
            //watch.Stop();
            //long acmMS = watch.ElapsedMilliseconds;

            //DebugFix.Assert(acmMS < csMS);
            //Console.WriteLine("C# / ACM RATIO: " + csMS / acmMS);
            #endregion

            if (!m_SkipACM)
            {
                try
                {
                    mp3PcmFormat = Mp3ToWav_NAudio(sourceFile, destinationDirectory, out destinationFile);
                }
                catch (Exception ex)
                {
                    mp3PcmFormat = null;
                    if (ex.GetType().FullName.IndexOf("acm", StringComparison.OrdinalIgnoreCase) >= 0
                        || ex.Message.IndexOf("acm", StringComparison.OrdinalIgnoreCase) >= 0
                        ) // Hacky sucky !! Needs testing on machines that don't support ACM
                    {
#if DEBUG
                        Debugger.Break();
#endif
                        m_SkipACM = true;
                    }
                    if (destinationFile != null && File.Exists(destinationFile))
                    {
                        File.Delete(destinationFile);
                    }

                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.StackTrace);
                }
            }

            if (mp3PcmFormat != null)
            {
                originalPcmFormat = new AudioLibPCMFormat();
                originalPcmFormat.CopyFrom(mp3PcmFormat);
            }

            if (RequestCancellation)
            {
                return null;
            }
            if (mp3PcmFormat == null || m_SkipACM)
            {
                mp3PcmFormat = Mp3ToWav_Mp3Sharp(sourceFile, destinationDirectory, out destinationFile);
            }

            if (mp3PcmFormat == null || RequestCancellation)
            {
                if (File.Exists(destinationFile))
                {
                    File.Delete(destinationFile);
                }
                return null;
            }

            if (pcmFormat == null) // auto detect
            {
                pcmFormat = mp3PcmFormat;
            }

            DebugFix.Assert(File.Exists(destinationFile));

            if (pcmFormat != null
                &&
                !mp3PcmFormat.IsCompatibleWith(pcmFormat))
            {
                AudioLibPCMFormat originalWavPcmFormat;
                string newDestinationFilePath = ConvertSampleRate(destinationFile, destinationDirectory, pcmFormat, out originalWavPcmFormat);
                if (originalWavPcmFormat != null)
                {
                    DebugFix.Assert(mp3PcmFormat.Equals(originalWavPcmFormat));
                }

                if (File.Exists(destinationFile))
                {
                    File.Delete(destinationFile);
                }
                if (RequestCancellation)
                {
                    return null;
                }
                return newDestinationFilePath;
            }

            return destinationFile;
        }

        public string UnCompressWavFile(string sourceFile, string destinationDirectory, AudioLibPCMFormat pcmFormat)
        {
            throw new System.NotImplementedException();

            // following code fails at ACM codec, so commented for now.
            /*
            if (!File.Exists(sourceFile))
                throw new FileNotFoundException("Invalid source file path");

            if (!Directory.Exists(destinationDirectory))
                throw new FileNotFoundException("Invalid destination directory");

            string destinationFilePath = null;
            WaveStream sourceStream = null;
            WaveFormatConversionStream conversionStream = null;
            try
                {
                WaveFormat destFormat = new WaveFormat ( (int)destinationPCMFormat.SampleRate,
                                                                   destinationPCMFormat.BitDepth,
                                                                   destinationPCMFormat.NumberOfChannels );
                sourceStream = new WaveFileReader ( sourceFile );

                WaveStream intermediateStream = WaveFormatConversionStream.CreatePcmStream ( sourceStream );
                conversionStream = new WaveFormatConversionStream ( destFormat, intermediateStream);

                destinationFilePath = GenerateOutputFileFullname ( sourceFile, destinationDirectory, destinationPCMFormat );
                WaveFileWriter.CreateWaveFile ( destinationFilePath, conversionStream );
                }
                        finally
                {
                if (conversionStream != null)
                    {
                    conversionStream.Close ();
                    }
                if (sourceStream != null)
                    {
                    sourceStream.Close ();
                    }
                }
            return destinationFilePath;
             */
        }

        public bool CompressWavToMp3(string sourceFile, string destinationFile, AudioLibPCMFormat pcmFormat, ushort bitRate_mp3Output)
        {
            return CompressWavToMp3(sourceFile, destinationFile, pcmFormat, bitRate_mp3Output, null, true, null);
        }

        public bool CompressWavToMp3(string sourceFile, string destinationFile, AudioLibPCMFormat pcmFormat, ushort bitRate_mp3Output, string extraparamChannels, bool extraParamResample, string extraParamReplayGain)
        {
            if (!File.Exists(sourceFile))
                throw new FileNotFoundException("Invalid source file path " + sourceFile);



            string LameWorkingDir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            //string outputFilePath = Path.Combine (
            //Path.GetDirectoryName ( sourceFile ),
            //Path.GetFileNameWithoutExtension ( sourceFile ) + DataProviderFactory.AUDIO_MP3_EXTENSION );

            if (pcmFormat.SampleRate < 22050)
            {
                bitRate_mp3Output = 32;
            }

            string sampleRate = String.Format("{0}", pcmFormat.SampleRate); //Math.Round(pcmFormat.SampleRate / 1000.0, 3));
            sampleRate = sampleRate.Substring(0, 2) + '.' + sampleRate.Substring(2, 3);
            string sampleRateArg = " --resample " + sampleRate;
            if (!extraParamResample) sampleRateArg = "";

            string replayGainArg = !string.IsNullOrEmpty(extraParamReplayGain) ? (extraParamReplayGain.StartsWith(" ") ? extraParamReplayGain : " " + extraParamReplayGain) :
                "";

            string channelsArg = pcmFormat.NumberOfChannels == 1 ? "m" : "s";
            if (!string.IsNullOrEmpty(extraparamChannels)) channelsArg = extraparamChannels;
            //string argumentString = "-b " + bitRate_mp3Output.ToString ()  + " --cbr --resample default -m m \"" + sourceFile + "\" \"" + destinationFile + "\"";
            string argumentString = "-b " + bitRate_mp3Output.ToString()
                + " --cbr"
                + sampleRateArg
                + " -m " + channelsArg
                + replayGainArg
                + " \"" + sourceFile + "\" \"" + destinationFile + "\"";
            Console.WriteLine(argumentString);
            Process mp3encodeProcess = new Process();

            mp3encodeProcess.StartInfo.FileName = Path.Combine(LameWorkingDir, "lame.exe");
            mp3encodeProcess.StartInfo.RedirectStandardOutput = false;
            mp3encodeProcess.StartInfo.RedirectStandardError = false;
            mp3encodeProcess.StartInfo.UseShellExecute = true;
            mp3encodeProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            mp3encodeProcess.StartInfo.Arguments = argumentString;
            //{
            //FileName = Path.Combine(LameWorkingDir, "lame.exe"),

            //RedirectStandardError = false,
            //RedirectStandardOutput = false,
            //UseShellExecute = true,
            //WindowStyle = ProcessWindowStyle.Hidden,
            //Arguments = argumentString
            //}
            //};
            mp3encodeProcess.Start();
            mp3encodeProcess.WaitForExit();

            if (!mp3encodeProcess.StartInfo.UseShellExecute && mp3encodeProcess.ExitCode != 0)
            {
                StreamReader stdErr = mp3encodeProcess.StandardError;
                if (!stdErr.EndOfStream)
                {
                    string toLog = stdErr.ReadToEnd();
                    if (!string.IsNullOrEmpty(toLog))
                    {
                        Console.WriteLine(toLog);
                    }
                }
            }
            else if (!mp3encodeProcess.StartInfo.UseShellExecute)
            {
                StreamReader stdOut = mp3encodeProcess.StandardOutput;
                if (!stdOut.EndOfStream)
                {
                    string toLog = stdOut.ReadToEnd();
                    if (!string.IsNullOrEmpty(toLog))
                    {
                        Console.WriteLine(toLog);
                    }
                }
            }

            if (File.Exists(destinationFile))
            {
                return true;
            }

            return false;

            //ProcessStartInfo process_Lame = new ProcessStartInfo();

            //process_Lame.FileName = Path.Combine(LameWorkingDir, "lame.exe");

            //process_Lame.Arguments = argumentString;

            //process_Lame.WindowStyle = ProcessWindowStyle.Hidden;
            ////System.Windows.Forms.MessageBox.Show ( process_Lame.FileName );
            ////System.Windows.Forms.MessageBox.Show ( process_Lame.Arguments );
            //Process p = Process.Start(process_Lame);
            //p.WaitForExit();
        }



        private string GenerateOutputFileFullname(string sourceFile, string destinationDirectory, AudioLibPCMFormat pcmFormat)
        {
            //FileInfo sourceFileInfo = new FileInfo(sourceFile);
            //string sourceFileName = sourceFileInfo.Name.Replace(sourceFileInfo.Extension, "");

            string sourceFileName = Path.GetFileNameWithoutExtension(sourceFile);
            string sourceFileExt = Path.GetExtension(sourceFile);
            if (!string.IsNullOrEmpty(sourceFileExt))
            {
                sourceFileExt = sourceFileExt.ToLower();
            }

            string channels = pcmFormat == null ? null : (pcmFormat.NumberOfChannels == 1 ? "Mono" : (pcmFormat.NumberOfChannels == 2 ? "Stereo" : pcmFormat.NumberOfChannels.ToString()));

            string destFile = null;

            if (OverwriteOutputFiles)
            {
                destFile = Path.Combine(destinationDirectory,
                                           sourceFileName

                                           +
                                           (pcmFormat == null ? "" :
                                           ("_"
                                           + pcmFormat.BitDepth
                                           + "-"
                                           + channels
                                           + "-"
                                           + pcmFormat.SampleRate))

                                           + sourceFileExt);
            }
            else
            {
                Random random = new Random();

                int loopCounter = 0;
                do
                {
                    loopCounter++;
                    if (loopCounter > 10000)
                    {
                        throw new Exception("Not able to generate destination file name");
                    }
                    string randomStr = "_" + random.Next(100000).ToString();

                    destFile = Path.Combine(destinationDirectory,
                                        sourceFileName

                                           +
                                           (pcmFormat == null ? "" :
                                           ("_"
                                           + pcmFormat.BitDepth
                                           + "-"
                                           + channels
                                           + "-"
                                           + pcmFormat.SampleRate))

                                        + randomStr
                                        + sourceFileExt);
                } while (File.Exists(destFile));
            }

            return destFile;
        }
    }

    internal class ObufferStreamWrapper : Obuffer
    {
        BinaryWriter m_WrappedStreamBinaryWriter;
        private short[] m_SamplesBuffer;
        private short[] m_SamplesBufferPointer;
        private int m_NumberOfChannels;

        public ObufferStreamWrapper(Stream outputStream, int numberOfChannels)
        {
            m_WrappedStreamBinaryWriter = new BinaryWriter(outputStream);
            m_NumberOfChannels = numberOfChannels;

            m_SamplesBuffer = new short[OBUFFERSIZE];
            m_SamplesBufferPointer = new short[MAXCHANNELS];

            clear_buffer();
        }

        public override void append(int channel, short value)
        {
            m_SamplesBuffer[m_SamplesBufferPointer[channel]] = value;
            m_SamplesBufferPointer[channel] += (short)m_NumberOfChannels;
        }

        public override void write_buffer(int val)
        {
            for (int sample = 0; sample < m_SamplesBufferPointer[0]; sample++)
            {
                m_WrappedStreamBinaryWriter.Write(m_SamplesBuffer[sample]);
            }

            clear_buffer();
        }

        public override void close()
        {
            m_WrappedStreamBinaryWriter.Close();
        }

        public override void clear_buffer()
        {
            for (int i = 0; i < m_NumberOfChannels; ++i)
            {
                m_SamplesBufferPointer[i] = (short)i;
            }
        }

        public override void set_stop_flag() { }
    }
}
