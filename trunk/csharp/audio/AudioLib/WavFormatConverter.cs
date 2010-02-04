using System;
using System.IO;
using javazoom.jl.decoder;
using NAudio.Wave;
using System.Diagnostics;

namespace AudioLib
{
    public class WavFormatConverter 
    {
        private bool m_OverwriteOutputFiles;
        public bool OverwriteOutputFiles
        {
            get { return m_OverwriteOutputFiles; }
            set { m_OverwriteOutputFiles = value; }
        }

        public WavFormatConverter(bool overwriteOutputFiles)
        {
            OverwriteOutputFiles = overwriteOutputFiles;
        }

        public string ConvertSampleRate(string sourceFile, string destinationDirectory, ushort destChannels, uint destSamplingRate, ushort destBitDepth)
        {
            if (!File.Exists(sourceFile))
                throw new FileNotFoundException("Invalid source file path");

            if (!Directory.Exists(destinationDirectory))
                throw new FileNotFoundException("Invalid destination directory");

            string destinationFilePath = null;
            WaveStream sourceStream = null;
            WaveFormatConversionStream conversionStream = null;
            try
            {
                WaveFormat destFormat = new WaveFormat((int)destSamplingRate,
                                                                   destBitDepth,
                                                                   destChannels);
                sourceStream = new WaveFileReader(sourceFile);

                conversionStream = new WaveFormatConversionStream(destFormat, sourceStream);

                destinationFilePath = GenerateOutputFileFullname(sourceFile, destinationDirectory, destChannels, destSamplingRate, destBitDepth);
                WaveFileWriter.CreateWaveFile(destinationFilePath, conversionStream);
            }
            finally
            {
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

        public string UnCompressMp3File(string sourceFile, string destinationDirectory, ushort destChannels, uint destSamplingRate, ushort destBitDepth)
        {
            if (!File.Exists(sourceFile))
                throw new FileNotFoundException("Invalid source file path");

            if (!Directory.Exists(destinationDirectory))
                throw new FileNotFoundException("Invalid destination directory");
            
            Stream mp3Stream = File.Open(sourceFile, FileMode.Open, FileAccess.Read, FileShare.Read);

            Bitstream mp3BitStream = new Bitstream(new BackStream(mp3Stream, 1024 * 10));
            Header mp3Frame = mp3BitStream.readFrame();

            AudioLibPCMFormat mp3PcmFormat = new AudioLibPCMFormat(
                (ushort)(mp3Frame.mode() == Header.SINGLE_CHANNEL ? 1 : 2),
                (uint)mp3Frame.frequency(),
                16);

            string destinationFilePath = GenerateOutputFileFullname(
                sourceFile + ".wav",
                destinationDirectory,
                mp3PcmFormat.NumberOfChannels,
                mp3PcmFormat.SampleRate,
                mp3PcmFormat.BitDepth);

            FileStream wavFileStream = new FileStream(destinationFilePath, FileMode.Create, FileAccess.Write, FileShare.Read);

            ulong wavFilRiffHeaderLength = mp3PcmFormat.RiffHeaderWrite(wavFileStream, 0);

            long wavFileStreamStart = wavFileStream.Position;

            Decoder decoder = new Decoder();
            ObufferStreamWrapper outputBuffer = new ObufferStreamWrapper(wavFileStream, mp3PcmFormat.NumberOfChannels);
            decoder.OutputBuffer = outputBuffer;

            while (true)
            {
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

            mp3Stream.Close();

            long wavFileStreamEnd = wavFileStream.Position;

            if ((ulong)wavFileStreamEnd == wavFilRiffHeaderLength)
            {
                wavFileStream.Close();
                File.Delete(destinationFilePath);
                return null;
            }

            wavFileStream.Position = 0;
            mp3PcmFormat.RiffHeaderWrite(wavFileStream, (uint)(wavFileStreamEnd - wavFileStreamStart));

            wavFileStream.Close();

            if (mp3PcmFormat.NumberOfChannels != destChannels
                || mp3PcmFormat.SampleRate != destSamplingRate
                || mp3PcmFormat.BitDepth != destBitDepth)
            {
                string newDestinationFilePath = ConvertSampleRate(destinationFilePath, destinationDirectory, destChannels, destSamplingRate, destBitDepth);
                if (File.Exists(destinationFilePath))
                {
                    File.Delete(destinationFilePath);
                }
                return newDestinationFilePath;
            }
            return destinationFilePath;

            /*
            bool exceptionError = false;
            using (Mp3FileReader mp3Reader = new Mp3FileReader(sourceFile))
            {
                using (WaveStream pcmStream = WaveFormatConversionStream.CreatePcmStream(mp3Reader))
                {
                    //pcmFormat = new PCMFormatInfo((ushort)pcmStream.WaveFormat.Channels,
                                                            //(uint)pcmStream.WaveFormat.SampleRate,
                                                            //(ushort)pcmStream.WaveFormat.BitsPerSample);
                channels = pcmStream.WaveFormat.Channels;
                sampleRate = pcmStream.WaveFormat.SampleRate;
                bitDepth = pcmStream.WaveFormat.BitsPerSample;
                    destinationFilePath = GenerateOutputFileFullname ( sourceFile + ".wav", destinationDirectory, pcmStream.WaveFormat.Channels, pcmStream.WaveFormat.SampleRate, pcmStream.WaveFormat.BitsPerSample );
                    using (WaveFileWriter writer = new WaveFileWriter(destinationFilePath, pcmStream.WaveFormat))
                    {
                        const int BUFFER_SIZE = 1024 * 8; // 8 KB MAX BUFFER  
                        byte[] buffer = new byte[BUFFER_SIZE];
                        int byteRead;
                        try
                        {
                            writer.Flush();
                            while ((byteRead = pcmStream.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                writer.WriteData(buffer, 0, byteRead);
                            }
                        }
                        catch (Exception ex)
                        {
                            pcmStream.Close();
                            writer.Close();
                            //pcmFormat = null;
                            exceptionError = true;
                        }
                    }
                }
            }

            //if (pcmFormat == null)
            if ( exceptionError )
            {
                // in case of exception, delete incomplete file just created
            if (File.Exists ( destinationFilePath ))
                {
                File.Delete ( destinationFilePath );
                }
*/
        }

        public string UnCompressWavFile(string sourceFile, string destinationDirectory, ushort destChannels, uint destSamplingRate, ushort destBitDepth)
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

        public bool CompressWavToMp3 ( string sourceFile, string destinationFile, ushort destChannels, uint destSamplingRate, ushort destBitDepth, ushort bitRate_mp3Output )
            {
            if (!File.Exists ( sourceFile ))
                throw new FileNotFoundException ( "Invalid source file path" );

            

            string LameWorkingDir = Path.GetDirectoryName (
                System.Reflection.Assembly.GetExecutingAssembly ().Location);
            //string outputFilePath = Path.Combine (
                //Path.GetDirectoryName ( sourceFile ),
                //Path.GetFileNameWithoutExtension ( sourceFile ) + ".mp3" );

            string channelsArg = destChannels == 1 ? "m" : "s";
            //string argumentString = "-b " + bitRate_mp3Output.ToString ()  + " --cbr --resample default -m m \"" + sourceFile + "\" \"" + destinationFile + "\"";
            string argumentString = "-b " + bitRate_mp3Output.ToString () + " --cbr -m " + channelsArg +  " \"" + sourceFile + "\" \"" + destinationFile + "\"";

            System.Diagnostics.ProcessStartInfo process_Lame = new System.Diagnostics.ProcessStartInfo ();

            process_Lame.FileName = Path.Combine ( LameWorkingDir, "lame.exe" );

            process_Lame.Arguments = argumentString;

            process_Lame.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            //System.Windows.Forms.MessageBox.Show ( process_Lame.FileName );
            //System.Windows.Forms.MessageBox.Show ( process_Lame.Arguments );
            System.Diagnostics.Process p = System.Diagnostics.Process.Start ( process_Lame );


            p.WaitForExit ();

            if ( File.Exists (destinationFile ))
                {
                return true;
                }
            else
                {
                return false;
                }
            }



        private string GenerateOutputFileFullname(string sourceFile, string destinationDirectory, ushort destChannels, uint destSamplingRate, ushort destBitDepth)
        {
            //FileInfo sourceFileInfo = new FileInfo(sourceFile);
            //string sourceFileName = sourceFileInfo.Name.Replace(sourceFileInfo.Extension, "");

            string sourceFileName = Path.GetFileNameWithoutExtension(sourceFile);
            string sourceFileExt = Path.GetExtension(sourceFile);

            string channels = (destChannels == 1 ? "Mono" : (destChannels == 2 ? "Stereo" : destChannels.ToString()));

            string destFile = null;

            if (OverwriteOutputFiles)
            {
                destFile = Path.Combine(destinationDirectory,
                                           sourceFileName
                                           + "_"
                                           + destBitDepth
                                           + "-"
                                           + channels
                                           + "-"
                                           + destSamplingRate
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
                                        + destBitDepth
                                        + "-"
                                        + channels
                                        + "-"
                                        + destSamplingRate
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
