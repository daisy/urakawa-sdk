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

        private static string GenerateOutputFileFullname(string sourceFile, string destinationDirectory, PCMFormatInfo destinationPCMFormat)
        {
            //FileInfo sourceFileInfo = new FileInfo(sourceFile);
            //string sourceFileName = sourceFileInfo.Name.Replace(sourceFileInfo.Extension, "");

            string sourceFileName = Path.GetFileNameWithoutExtension(sourceFile);
            string sourceFileExt = Path.GetExtension(sourceFile);

            string channels = (destinationPCMFormat.NumberOfChannels == 1 ? "Mono" : (destinationPCMFormat.NumberOfChannels == 2 ? "Stereo" : destinationPCMFormat.NumberOfChannels.ToString()));

            Random random = new Random();

            int loopCounter = 0;
            string destFile = null;
            do
            {
                loopCounter++;
                if (loopCounter > 10000)
                {
                    throw new Exception("Not able to generate destination file name");
                }

                destFile = Path.Combine(destinationDirectory,
                                    sourceFileName
                                    + "_"
                                    + destinationPCMFormat.BitDepth
                                    + "-"
                                    + channels
                                    + "-"
                                    + destinationPCMFormat.SampleRate
                                    + "_"
                                    + random.Next(100000).ToString()
                                    + sourceFileExt);
            } while (File.Exists(destFile));

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
            throw new NotImplementedException();
        }
    }
}
