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
        private PCMFormatInfo m_DefaultPCMInfo;

        /// <summary>
        ///  initializes instance with PCM format info 
        /// </summary>
        /// <param name="PCMInfo"></param>
        public WavFormatConverter(PCMFormatInfo PCMInfo)
        {
            m_DefaultPCMInfo = PCMInfo;
        }

        /// <summary>
        /// initializes instance without dependency on PCNFormatInfo of urakawa sdk.
        /// </summary>
        /// <param name="defaultNoOfChhanels"></param>
        /// <param name="defaultSamplingRate"></param>
        /// <param name="defaultBitDepth"></param>
        public WavFormatConverter(int defaultNoOfChhanels, int defaultSamplingRate, int defaultBitDepth)
        {
            m_DefaultPCMInfo = new PCMFormatInfo((ushort)defaultNoOfChhanels, (uint)defaultSamplingRate, (ushort)defaultBitDepth);
        }

        public PCMFormatInfo DefaultPCMFormat
        {
            get { return m_DefaultPCMInfo; }
        }

        public int ProgressInfo
        {
            get { return 0; }
        }

        /// <summary>
        /// create a new wav file by resampling an uncompressed wav file to another standard wav format
        /// </summary>
        /// <param name="sourceFilePath"></param>
        /// <param name="destinationPCMInfo"></param>
        /// <param name="destinationDirectory"></param>
        /// <returns>  file path of the new wave file </returns>
        public string ConvertUnCompressedWavFile(string sourceFilePath, PCMFormatInfo destinationPCMInfo, string destinationDirectory)
        {
            if (!File.Exists(sourceFilePath))
                throw new FileNotFoundException("Invalid source file path");

            if (!Directory.Exists(destinationDirectory))
                throw new FileNotFoundException("Invalid destination directory");

            NAudio.Wave.WaveFormat DestFormat = new WaveFormat((int)destinationPCMInfo.SampleRate, destinationPCMInfo.BitDepth, destinationPCMInfo.NumberOfChannels);

            WaveStream sourceStream = new WaveFileReader(sourceFilePath);
            NAudio.Wave.WaveFormatConversionStream conversionStream = new WaveFormatConversionStream(DestFormat, sourceStream);

            string destinationFilePath = GenerateOutputFileFullname(sourceFilePath, destinationDirectory);
            NAudio.Wave.WaveFileWriter.CreateWaveFile(destinationFilePath, conversionStream);

            conversionStream.Close();
            conversionStream = null;
            sourceStream.Close();
            sourceStream = null;

            return destinationFilePath;
        }

        private string GenerateOutputFileFullname(string sourcePath, string outputDirectory)
        {
            FileInfo inputFileInfo = new FileInfo(sourcePath);
            //string inputFilename = inputFileInfo.Name.Replace(inputFileInfo.Extension, "");

            Random random = new Random();

            string destFilePath = Path.Combine(outputDirectory, random.Next(100000).ToString() + inputFileInfo.Extension);
            int precautionCounter = 0;
            while (File.Exists(destFilePath))
            {
                destFilePath = Path.Combine(outputDirectory, random.Next(100000).ToString() + inputFileInfo.Extension);
                precautionCounter++;
                if (precautionCounter > 10000) throw new System.Exception("Not able to generate destination file name");
            }
            return destFilePath;
        }

        /// <summary>
        /// Creates a new wave file by uncompressing a wav file. It takes compressed wave file as source and do not take compressed formats like mp3 as input.
        /// </summary>
        /// <param name="sourceFilePath"></param>
        /// <param name="destinationDirectory"></param>
        /// <param name="convertToDefaultFormat"></param>
        /// <returns>  file path of new file </returns>
        /// <exception cref="NotImplementedException">NOT IMPLEMENTED !</exception>
        public string UnCompressWavFile(string sourceFilePath, string destinationDirectory, bool convertToDefaultFormat)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Creates a new wave files with supplied PCM format from a compressed wav file. It takes only compressed wav filesas input and do not uncompress mp3.
        /// </summary>
        /// <param name="sourceFilePath"></param>
        /// <param name="destinationPCMInfo"></param>
        /// <param name="destinationDirectory"></param>
        /// <returns> file path of new file </returns>
        /// <exception cref="NotImplementedException">NOT IMPLEMENTED !</exception>
        public string UnCompressWavFile(string sourceFilePath, PCMFormatInfo destinationPCMInfo, string destinationDirectory)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Create a wave file of default PCM format by uncompressing source mp3 file. 
        /// </summary>
        /// <param name="sourceFilePath"></param>
        /// <param name="destinationDirectory"></param>
        /// <returns> file path of new wav file </returns>
        /// <exception cref="NotImplementedException">NOT IMPLEMENTED !</exception>
        public string ImportMp3FileAsWavFile(string sourceFilePath, string destinationDirectory)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///  Create a wave file of supplied PCM format by uncompressing source mp3 file. 
        /// </summary>
        /// <param name="sourceFilePath"></param>
        /// <param name="destinationFormatInfo"></param>
        /// <param name="destinationDirectory"></param>
        /// <returns>file path of new wave file </returns>
        /// <exception cref="NotImplementedException">NOT IMPLEMENTED !</exception>
        public string ImportMp3FileAsWavFile(string sourceFilePath, PCMFormatInfo destinationFormatInfo, string destinationDirectory)
        {
            throw new NotImplementedException();
        }
    }
}
