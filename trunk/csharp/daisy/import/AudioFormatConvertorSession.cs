using System;
using System.Collections.Generic;
using System.IO;
using AudioLib;
using urakawa.data;
using urakawa.media.data.audio;

namespace urakawa.daisy.import
{
    /// <summary>
    ///  class to maintain session for importing files of format different from default format of project
    /// will reduce re conversion of files already converted in the same session
    /// </summary>
    public class AudioFormatConvertorSession
    {
        private Dictionary<string, string> m_FilePathsMap; // maps source files to converted files
        Presentation m_Presentation;


        public AudioFormatConvertorSession(Presentation presentation)
        {
            m_FilePathsMap = new Dictionary<string, string>();
            m_Presentation = presentation;
        }

        /// <summary>
        /// checks if wav file or mp3 file has already been converted in the same session, if it is already converted, returns file path of converted file
        /// else convert to wav file with default project format and stores it in data directory of project.
        /// </summary>
        /// <param name="sourceFilePath"></param>
        /// <returns> full path of converted file if conversion is successful </returns>
        public string ConvertAudioFileFormat(string sourceFilePath)
        {
            if (!File.Exists(sourceFilePath))
                throw new FileNotFoundException();

            if (m_FilePathsMap.ContainsKey(sourceFilePath))
            {
                return m_FilePathsMap[sourceFilePath];
            }

            string convertedFilePath = ConvertToDefaultFormat(sourceFilePath, m_Presentation.DataProviderManager.DataFileDirectoryFullPath, m_Presentation.MediaDataManager.DefaultPCMFormat.Copy());

            if (File.Exists(convertedFilePath))
            {
                m_FilePathsMap.Add(sourceFilePath, convertedFilePath);
                return convertedFilePath;
            }

            throw new System.Exception("Operation failed");
        }

        /// <summary>
        /// Deletes audio files created during this session, do not deletes the file which is being referenced in presentation
        /// </summary>
        public void DeleteSessionAudioFiles()
        {
            List<string> presentationFilePaths = new List<string>();

            foreach (FileDataProvider f in m_Presentation.DataProviderManager.ManagedFileDataProviders)
            {
                presentationFilePaths.Add(f.DataFileFullPath);
            }

            string[] keysArray = new string[m_FilePathsMap.Keys.Count];
            m_FilePathsMap.Keys.CopyTo(keysArray, 0);

            for (int i = 0; i < keysArray.Length; i++)
            {
                string path = m_FilePathsMap[keysArray[i]];
                if (!presentationFilePaths.Contains(path))
                {
                    if (File.Exists(path))
                    {
                        File.Delete(path);
                    }
                    m_FilePathsMap.Remove(keysArray[i]);
                }
            }
        }

        public void RelocateDestinationFilePath(string originalPath, string newPath)
        {
            foreach (string srcFilePath in m_FilePathsMap.Keys)
            {
                if (m_FilePathsMap[srcFilePath] == originalPath)
                {
                    m_FilePathsMap[srcFilePath] = newPath;
                }
            }
        }


        /// <summary>
        /// takes wav file / mp3 file as input and converts it to wav file with audio format info supplied as parameter
        /// </summary>
        /// <param name="SourceFilePath"></param>
        /// <param name="destinationDirectory"></param>
        /// <param name="destinationFormatInfo"></param>
        /// <returns> full file path of converted file  </returns>
        private static string ConvertToDefaultFormat(string SourceFilePath, string destinationDirectory, PCMFormatInfo destinationFormatInfo)
        {
            if (!File.Exists(SourceFilePath))
                throw new FileNotFoundException(SourceFilePath);

            if (destinationFormatInfo == null)
                throw new ArgumentNullException("PCM format");

            if (!Directory.Exists(destinationDirectory))
            {
                Directory.CreateDirectory(destinationDirectory);
            }

            AudioFileType sourceFileType = GetAudioFileType(SourceFilePath);
            switch (sourceFileType)
            {
                case AudioFileType.WavUncompressed:
                case AudioFileType.WavCompressed:
                    IWavFormatConverter formatConverter1 = new WavFormatConverter(true);
                    return formatConverter1.ConvertSampleRate(SourceFilePath, destinationDirectory, destinationFormatInfo.Data.NumberOfChannels, destinationFormatInfo.Data.SampleRate, destinationFormatInfo.Data.BitDepth);
                case AudioFileType.Mp3:
                    IWavFormatConverter formatConverter2 = new WavFormatConverter(true);
                    return formatConverter2.UnCompressMp3File(SourceFilePath, destinationDirectory, destinationFormatInfo.Data.NumberOfChannels, destinationFormatInfo.Data.SampleRate, destinationFormatInfo.Data.BitDepth);
                default:
                    throw new Exception("Source file format not supported");
            }
        }


        private enum AudioFileType { WavUncompressed, WavCompressed, Mp3, NotSupported } ;


        ///// <summary>
        ///// Determines audio file type
        ///// </summary>
        ///// <param name="filePath"></param>
        ///// <returns> file type as enum </returns>
        private static AudioFileType GetAudioFileType(string filePath)
        {
            AudioFileType sourceFileType = AudioFileType.NotSupported;
            if (filePath.ToLower().EndsWith(".mp3"))
            {
                sourceFileType = AudioFileType.Mp3;
            }
            else if (filePath.ToLower().EndsWith(".wav"))
            {
                sourceFileType = AudioFileType.WavUncompressed;
            }
            return sourceFileType;

            // TODO: introspect audio file content instead of checking file extension
            //if (!File.Exists(filePath))
            //    throw new FileNotFoundException();

            //// first check if it is wav file
            //FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            //try
            //{
            //    uint dataLength;
            //    AudioLibPCMFormat format = AudioLibPCMFormat.RiffHeaderParse(fs, out dataLength);

            //    fs.Close();

            //    if (format != null)
            //    {
            //        return format.IsCompressed ? AudioFileTypes.UncompressedWav : AudioFileTypes.UncompressedWav;
            //    }
            //}
            //catch (System.Exception)
            //{
            //    if (fs != null) fs.Close();
            //}

            //// now check for mp3 header
            //FileStream mp3Filestream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            //try
            //{
            //    Mp3Frame frame = new Mp3Frame(mp3Filestream);
            //    mp3Filestream.Close();
            //    if (frame != null) return AudioFileTypes.mp3;
            //}
            //catch (System.Exception)
            //{
            //    if (mp3Filestream != null) mp3Filestream.Close();
            //}
            //return AudioFileTypes.NotSupported;
        }
    }
}
