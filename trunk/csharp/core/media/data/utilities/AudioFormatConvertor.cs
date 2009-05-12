using System;
using System.IO;

using AudioLib;

namespace urakawa.media.data.utilities
{
    public enum AudioFileTypes { UncompressedWav, CompressedWav, mp3, NotSupported } ;

    public class AudioFormatConvertor
    {
        /// <summary>
        /// Determines audio file type on basis of header.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns> file type as enum </returns>
        public static AudioFileTypes GetAudioFileType(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException();

            // first check if it is wav file
            FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            try
            {
                audio.PCMFormatInfo formatInfo = audio.PCMDataInfo.ParseRiffWaveHeader(fs);
                fs.Close();
                if (formatInfo != null)
                {
                    return formatInfo.IsCompressed ? AudioFileTypes.UncompressedWav : AudioFileTypes.UncompressedWav;
                }
            }
            catch (System.Exception)
            {
                if (fs != null) fs.Close();
            }

            // now check for mp3 header
            FileStream mp3Filestream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            try
            {
                Mp3Frame frame = new Mp3Frame(mp3Filestream);
                mp3Filestream.Close();
                if (frame != null) return AudioFileTypes.mp3;
            }
            catch (System.Exception)
            {
                if (mp3Filestream != null) mp3Filestream.Close();
            }
            return AudioFileTypes.NotSupported;
        }

        public static bool WillConversionReduceQuality(string fileName)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// takes wav file / mp3 file as input and converts it to wav file with default audio format of project
        /// stores output file in data directory of project
        /// </summary>
        /// <param name="presentation"></param>
        /// <param name="SourceFilePath"></param>
        /// <returns> full file path of converted file </returns>
        public static string ConvertToDefaultFormat(Presentation presentation, string SourceFilePath)
        {
            if (!File.Exists(SourceFilePath))
                throw new System.IO.FileNotFoundException();

            string destDirectory = presentation.DataProviderManager.DataFileDirectoryFullPath;
            if (!Directory.Exists(destDirectory))
            {
                Directory.CreateDirectory(destDirectory);
            }

            audio.PCMFormatInfo defaultFormat = presentation.MediaDataManager.DefaultPCMFormat;

            return ConvertToDefaultFormat(SourceFilePath, destDirectory, defaultFormat);
        }


        /// <summary>
        /// takes wav file / mp3 file as input and converts it to wav file with audio format info supplied as parameter
        /// </summary>
        /// <param name="SourceFilePath"></param>
        /// <param name="destinationDirectory"></param>
        /// <param name="destinationFormatInfo"></param>
        /// <returns> full file path of converted file  </returns>
        public static string ConvertToDefaultFormat(string SourceFilePath, string destinationDirectory, audio.PCMFormatInfo destinationFormatInfo)
        {
            if (!File.Exists(SourceFilePath))
                throw new FileNotFoundException(SourceFilePath);

            if (destinationFormatInfo == null)
                throw new ArgumentNullException("PCM format");

            if (!Directory.Exists(destinationDirectory))
            {
                Directory.CreateDirectory(destinationDirectory);
            }

            // TODO: fix this !
            //AudioFileTypes sourceFileType = GetAudioFileType(SourceFilePath);
            AudioFileTypes sourceFileType = AudioFileTypes.NotSupported;
            if (SourceFilePath.EndsWith(".mp3"))
            {
                sourceFileType = AudioFileTypes.mp3;
            }
            else if (SourceFilePath.EndsWith(".wav") || SourceFilePath.EndsWith(".WAV"))
            {
                sourceFileType = AudioFileTypes.UncompressedWav;
            }


            IWavFormatConverter formatConverter = new WavFormatConverter(true);

            string convertedFile_FullPath = null;

            switch (sourceFileType)
            {
                case AudioFileTypes.UncompressedWav:
                case AudioFileTypes.CompressedWav:
                    convertedFile_FullPath = formatConverter.ConvertSampleRate(SourceFilePath, destinationDirectory, destinationFormatInfo);
                    break;

                case AudioFileTypes.mp3:
                    convertedFile_FullPath = formatConverter.UnCompressMp3File(SourceFilePath, destinationDirectory, destinationFormatInfo);
                    break;

                default:
                    throw new Exception("Source file format not supported");
            }

            return convertedFile_FullPath;
        }
    }
}
