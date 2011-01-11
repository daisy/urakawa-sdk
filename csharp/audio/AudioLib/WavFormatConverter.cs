using System;
using System.IO;
using javazoom.jl.decoder;
using NAudio.Wave;
using System.Diagnostics;

namespace AudioLib
{
    public class WavFormatConverter : DualCancellableProgressReporter
    {
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

        public string ConvertSampleRate(string sourceFile, string destinationDirectory, AudioLibPCMFormat pcmFormat)
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

                //WaveFileWriter.CreateWaveFile(destinationFilePath, conversionStream);
                using (WaveFileWriter writer = new WaveFileWriter(destinationFilePath, conversionStream.WaveFormat))
                {
                    //const int BUFFER_SIZE = 1024 * 8; // 8 KB MAX BUFFER
                    int BUFFER_SIZE = (int)pcmFormat.ConvertTimeToBytes(1500 * AudioLibPCMFormat.TIME_UNIT);
                    //int debug = pcmStream.GetReadSize(4000);
                    byte[] buffer = new byte[BUFFER_SIZE];
                    int byteRead;
                    writer.Flush();

                    string msg = Path.GetFileName(sourceFile) + " / " + Path.GetFileName(destinationFilePath); //"Resampling WAV audio...";
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
                                    mp3FilePath + ".wav",
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
                                            mp3FilePath + ".wav",
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

        private bool m_SkipACM;
        public string UnCompressMp3File(string sourceFile, string destinationDirectory, AudioLibPCMFormat pcmFormat)
        {
            if (!File.Exists(sourceFile))
                throw new FileNotFoundException("Invalid source file path");

            if (!Directory.Exists(destinationDirectory))
                throw new FileNotFoundException("Invalid destination directory");

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

            //Debug.Assert(acmMS < csMS);
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
                    if (ex.GetType().FullName.ToLower().Contains("acm")
                        || ex.Message.ToLower().Contains("acm")
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

            Debug.Assert(File.Exists(destinationFile));

            if (pcmFormat != null
                &&
                !mp3PcmFormat.IsCompatibleWith(pcmFormat))
            {
                string newDestinationFilePath = ConvertSampleRate(destinationFile, destinationDirectory, pcmFormat);

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
            if (!File.Exists(sourceFile))
                throw new FileNotFoundException("Invalid source file path " + sourceFile);



            string LameWorkingDir = Path.GetDirectoryName(
                System.Reflection.Assembly.GetExecutingAssembly().Location);
            //string outputFilePath = Path.Combine (
            //Path.GetDirectoryName ( sourceFile ),
            //Path.GetFileNameWithoutExtension ( sourceFile ) + ".mp3" );

            if (pcmFormat.SampleRate < 22050)
            {
                bitRate_mp3Output = 32;
            }

            string sampleRate = String.Format("{0}", pcmFormat.SampleRate); //Math.Round(pcmFormat.SampleRate / 1000.0, 3));
            sampleRate = sampleRate.Substring(0, 2) + '.' + sampleRate.Substring(2, 3);

            string channelsArg = pcmFormat.NumberOfChannels == 1 ? "m" : "s";
            //string argumentString = "-b " + bitRate_mp3Output.ToString ()  + " --cbr --resample default -m m \"" + sourceFile + "\" \"" + destinationFile + "\"";
            string argumentString = "-b " + bitRate_mp3Output.ToString()
                + " --cbr"
                + " --resample " + sampleRate
                + " -m " + channelsArg
                + " \"" + sourceFile + "\" \"" + destinationFile + "\"";

            Process mp3encodeProcess = new Process () ;
            
                mp3encodeProcess.StartInfo.FileName =Path.Combine(LameWorkingDir, "lame.exe") ;
                mp3encodeProcess.StartInfo.RedirectStandardOutput = false;
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

            string channels = (pcmFormat.NumberOfChannels == 1 ? "Mono" : (pcmFormat.NumberOfChannels == 2 ? "Stereo" : pcmFormat.NumberOfChannels.ToString()));

            string destFile = null;

            if (OverwriteOutputFiles)
            {
                destFile = Path.Combine(destinationDirectory,
                                           sourceFileName
                                           + "_"
                                           + pcmFormat.BitDepth
                                           + "-"
                                           + channels
                                           + "-"
                                           + pcmFormat.SampleRate
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
                                        + "_"
                                        + pcmFormat.BitDepth
                                        + "-"
                                        + channels
                                        + "-"
                                        + pcmFormat.SampleRate
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
