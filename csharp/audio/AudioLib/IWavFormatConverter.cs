using System.Collections.Generic;
using System.Text;
using urakawa.media.data.audio;


namespace AudioLib
    {
    public interface IWavFormatConverter
        {

        /// <summary>
        /// returns default wave format for the class
                /// </summary>
        /// <param name="formatInfo"></param>
        PCMFormatInfo DefaultPCMFormat
            {
            get ;
            }

        /// <summary>
        /// reports progress information from 0 to 100
                /// </summary>
        int ProgressInfo
            {
            get;
            }

        /// <summary>
        /// create a new wav file by resampling an uncompressed wav file to another standard wav format
                /// </summary>
        /// <param name="sourceFilePath"></param>
        /// <param name="destinationPCMInfo"></param>
        /// <param name="destinationDirectory"></param>
        /// <returns>  file path of the new wave file </returns>
        string ConvertUnCompressedWavFile  ( string sourceFilePath, PCMFormatInfo destinationPCMInfo, string destinationDirectory );


        /// <summary>
        /// Creates a new wave file by uncompressing a wav file. It takes compressed wave file as source and do not take compressed formats like mp3 as input.
                        /// </summary>
        /// <param name="sourceFilePath"></param>
        /// <param name="destinationDirectory"></param>
        /// <param name="convertToDefaultFormat"></param>
        /// <returns>  file path of new file </returns>
        string UnCompressWavFile ( string sourceFilePath, string destinationDirectory , bool convertToDefaultFormat);



        /// <summary>
        /// Creates a new wave files with supplied PCM format from a compressed wav file. It takes only compressed wav filesas input and do not uncompress mp3.
                /// </summary>
        /// <param name="sourceFilePath"></param>
        /// <param name="destinationPCMInfo"></param>
        /// <param name="destinationDirectory"></param>
        /// <returns> file path of new file </returns>
        string UnCompressWavFile ( string sourceFilePath, PCMFormatInfo destinationPCMInfo,  string destinationDirectory );


        /// <summary>
        /// Create a wave file of default PCM format by uncompressing source mp3 file. 
                /// </summary>
        /// <param name="sourceFilePath"></param>
        /// <param name="destinationDirectory"></param>
        /// <returns> file path of new wav file </returns>
        string ImportMp3FileAsWavFile ( string sourceFilePath, string destinationDirectory );

        /// <summary>
        ///  Create a wave file of supplied PCM format by uncompressing source mp3 file. 
                /// </summary>
        /// <param name="sourceFilePath"></param>
        /// <param name="destinationFormatInfo"></param>
        /// <param name="destinationDirectory"></param>
        /// <returns>file path of new wave file </returns>
        string ImportMp3FileAsWavFile  ( string sourceFilePath, PCMFormatInfo destinationFormatInfo, string destinationDirectory );
            

        }
    }
