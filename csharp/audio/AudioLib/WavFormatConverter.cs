using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using urakawa.media.data.audio;

using NAudio.Wave;

namespace AudioLib
{
    public class WavFormatConverter : IWavFormatConverter
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

        public string ConvertSampleRate(string sourceFile, string destinationDirectory, PCMFormatInfo destinationPCMFormat)
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
                WaveFormat destFormat = new WaveFormat((int)destinationPCMFormat.SampleRate,
                                                                   destinationPCMFormat.BitDepth,
                                                                   destinationPCMFormat.NumberOfChannels);
                sourceStream = new WaveFileReader(sourceFile);

                conversionStream = new WaveFormatConversionStream(destFormat, sourceStream);

                destinationFilePath = GenerateOutputFileFullname(sourceFile, destinationDirectory, destinationPCMFormat);
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

        private string GenerateOutputFileFullname(string sourceFile, string destinationDirectory, PCMFormatInfo destinationPCMFormat)
        {
            //FileInfo sourceFileInfo = new FileInfo(sourceFile);
            //string sourceFileName = sourceFileInfo.Name.Replace(sourceFileInfo.Extension, "");

            string sourceFileName = Path.GetFileNameWithoutExtension(sourceFile);
            string sourceFileExt = Path.GetExtension(sourceFile);

            string channels = (destinationPCMFormat.NumberOfChannels == 1 ? "Mono" : (destinationPCMFormat.NumberOfChannels == 2 ? "Stereo" : destinationPCMFormat.NumberOfChannels.ToString()));

            string destFile = null;

            if (OverwriteOutputFiles)
            {
                destFile = Path.Combine(destinationDirectory,
                                           sourceFileName
                                           + "_"
                                           + destinationPCMFormat.BitDepth
                                           + "-"
                                           + channels
                                           + "-"
                                           + destinationPCMFormat.SampleRate
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
                                        + destinationPCMFormat.BitDepth
                                        + "-"
                                        + channels
                                        + "-"
                                        + destinationPCMFormat.SampleRate
                                        + randomStr
                                        + sourceFileExt);
                } while (File.Exists(destFile));
            }

            return destFile;
        }

        /// <exception cref="NotImplementedException">NOT IMPLEMENTED !</exception>
        public int ProgressInfo
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <exception cref="NotImplementedException">NOT IMPLEMENTED !</exception>
        public string UnCompressWavFile(string sourceFile, string destinationDirectory, PCMFormatInfo destinationPCMFormat)
        {
            throw new NotImplementedException();
        }

        /// <exception cref="NotImplementedException">NOT IMPLEMENTED !</exception>
        public string UnCompressMp3File(string sourceFile, string destinationDirectory, PCMFormatInfo destinationPCMFormat)
        {
            if (!File.Exists(sourceFile))
                throw new FileNotFoundException("Invalid source file path");

            if (!Directory.Exists(destinationDirectory))
                throw new FileNotFoundException("Invalid destination directory");

            string destinationFilePath = null;
            PCMFormatInfo pcmFormat = null;
            using (Mp3FileReader mp3Reader = new Mp3FileReader(sourceFile))
            {
                using (WaveStream pcmStream = WaveFormatConversionStream.CreatePcmStream(mp3Reader))
                {
                    pcmFormat = new PCMFormatInfo((ushort)pcmStream.WaveFormat.Channels,
                                                            (uint)pcmStream.WaveFormat.SampleRate,
                                                            (ushort)pcmStream.WaveFormat.BitsPerSample);

                    destinationFilePath = GenerateOutputFileFullname(sourceFile + ".wav", destinationDirectory, pcmFormat);
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
                            pcmFormat = null;
                        }
                    }
                }
            }

            if (pcmFormat == null)
            {
                Stream fileStream = File.Open(sourceFile, FileMode.Open, FileAccess.Read);
                if (fileStream != null)
                {
                    int totalBytesWritten = 0;
                    using (NLayerMp3Stream mp3Stream = new NLayerMp3Stream(fileStream))
                    {
                        pcmFormat = new PCMFormatInfo((ushort)mp3Stream.WaveFormat.Channels,
                                                      (uint)mp3Stream.WaveFormat.SampleRate,
                                                      (ushort)mp3Stream.WaveFormat.BitsPerSample);
                        destinationFilePath = GenerateOutputFileFullname(sourceFile + ".wav", destinationDirectory, pcmFormat);
                        using (WaveFileWriter writer = new WaveFileWriter(destinationFilePath, mp3Stream.WaveFormat))
                        {
                            int buffSize = mp3Stream.GetReadSize(4000);
                            //const int BUFFER_SIZE = 1024 * 8; // 8 KB MAX BUFFER  
                            byte[] buffer = new byte[buffSize];
                            try
                            {
                                int byteRead;
                                writer.Flush();
                                while ((byteRead = mp3Stream.Read(buffer, 0, buffer.Length)) > 0)
                                {
                                    writer.WriteData(buffer, 0, byteRead);
                                    totalBytesWritten += byteRead;
                                }
                            }
                            catch (Exception ex)
                            {
                                writer.Close();
                                return null;
                            }
                        }
                    }
                    if (totalBytesWritten == 0)
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }

            if (!pcmFormat.IsCompatibleWith(destinationPCMFormat))
            {
                return ConvertSampleRate(destinationFilePath, destinationDirectory, destinationPCMFormat);
            }
            return destinationFilePath;
        }
    }
}
