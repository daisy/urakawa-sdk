using System;
using System.Collections.Generic;
using System.Text;
using urakawa.media.data.audio;

namespace AudioLib
    {
    class WavFormatConverter : IWavFormatConverter 
        {

        private PCMFormatInfo m_DefaultPCMInfo;

        public WavFormatConverter ( PCMFormatInfo PCMInfo )
            {
            m_DefaultPCMInfo = PCMInfo;
            }

        public PCMFormatInfo DefaultPCMFormat
            {
            get { return m_DefaultPCMInfo; }
            }


        /// <summary>
        /// create a new wav file by resampling an uncompressed wav file to another standard wav format
        /// </summary>
        /// <param name="sourceFilePath"></param>
        /// <param name="destinationPCMInfo"></param>
        /// <param name="destinationDirectory"></param>
        /// <returns>  file path of the new wave file </returns>
        public string ConvertUnpressedWavFile ( string sourceFilePath, PCMFormatInfo destinationPCMInfo, string destinationDirectory )
            {
            return  null;
            }


        /// <summary>
        /// Creates a new wave file by uncompressing a wav file. It takes compressed wave file as source and do not take compressed formats like mp3 as input.
        /// </summary>
        /// <param name="sourceFilePath"></param>
        /// <param name="destinationDirectory"></param>
        /// <param name="convertToDefaultFormat"></param>
        /// <returns>  file path of new file </returns>
        public string UnCompressWavFile ( string sourceFilePath, string destinationDirectory, bool convertToDefaultFormat )
            {
            return null;
            }



        /// <summary>
        /// Creates a new wave files with supplied PCM format from a compressed wav file. It takes only compressed wav filesas input and do not uncompress mp3.
        /// </summary>
        /// <param name="sourceFilePath"></param>
        /// <param name="destinationPCMInfo"></param>
        /// <param name="destinationDirectory"></param>
        /// <returns> file path of new file </returns>
        public string UnCompressWavFile ( string sourceFilePath, PCMFormatInfo destinationPCMInfo, string destinationDirectory )
            {
            return null;
            }

        /// <summary>
        /// Create a wave file of default PCM format by uncompressing source mp3 file. 
        /// </summary>
        /// <param name="sourceFilePath"></param>
        /// <param name="destinationDirectory"></param>
        /// <returns> file path of new wav file </returns>
        public string ImportMp3FileAsWavFile ( string sourceFilePath, string destinationDirectory )
            {
            return null;
            }

        /// <summary>
        ///  Create a wave file of supplied PCM format by uncompressing source mp3 file. 
        /// </summary>
        /// <param name="sourceFilePath"></param>
        /// <param name="destinationFormatInfo"></param>
        /// <param name="destinationDirectory"></param>
        /// <returns>file path of new wave file </returns>
        public string ImportMp3FileAsWavFile ( string sourceFilePath, PCMFormatInfo destinationFormatInfo, string destinationDirectory )
            {

            return null;
            }





        }
    }
