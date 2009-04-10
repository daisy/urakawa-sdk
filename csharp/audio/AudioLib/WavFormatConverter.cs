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

                destinationFilePath = GenerateOutputFileFullname(sourceFile, destinationDirectory);
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

        private static string GenerateOutputFileFullname(string sourceFile, string destinationDirectory)
        {
            FileInfo inputFileInfo = new FileInfo(sourceFile);
            //string inputFilename = inputFileInfo.Name.Replace(inputFileInfo.Extension, "");

            Random random = new Random();

            string destFilePath = Path.Combine(destinationDirectory, random.Next(100000).ToString() + inputFileInfo.Extension);
            int precautionCounter = 0;
            while (File.Exists(destFilePath))
            {
                destFilePath = Path.Combine(destinationDirectory, random.Next(100000).ToString() + inputFileInfo.Extension);
                precautionCounter++;
                if (precautionCounter > 10000) throw new System.Exception("Not able to generate destination file name");
            }
            return destFilePath;
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
