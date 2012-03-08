using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using AudioLib;
using urakawa.data;
using urakawa.ExternalFiles;
using urakawa.media.data.audio;

namespace urakawa.daisy.import
{
    /// <summary>
    ///  class to maintain session for importing files of format different from default format of project
    /// will reduce re conversion of files already converted in the same session
    /// </summary>
    public class AudioFormatConvertorSession : DualCancellableProgressReporter
    {
        public override void DoWork()
        {
        }

        public static readonly string TEMP_AUDIO_DIRECTORY = Path.Combine(ExternalFilesDataManager.STORAGE_FOLDER_PATH,
                                                                          "Temporary-Audio");
        static AudioFormatConvertorSession()
        {
            if (!Directory.Exists(TEMP_AUDIO_DIRECTORY))
            {
                Directory.CreateDirectory(TEMP_AUDIO_DIRECTORY);
            }
        }

        private readonly Dictionary<string, string> m_FilePathsMap; // maps source files to converted files
        private readonly string m_destinationDirectory;
        private readonly PCMFormatInfo m_destinationFormatInfo;

        private readonly bool m_SkipACM;
        public AudioFormatConvertorSession(string destinationDirectory, PCMFormatInfo destinationFormatInfo, bool skipACM)
        {
            if (destinationDirectory == null) throw new ArgumentNullException("destinationDirectory");
            m_destinationDirectory = destinationDirectory;

            m_destinationFormatInfo = destinationFormatInfo;

            m_SkipACM = skipACM;

            m_FilePathsMap = new Dictionary<string, string>();
        }

        public void RelocateDestinationFilePath(string originalPath, string newPath)
        {
            List<string> keys = new List<string>(m_FilePathsMap.Count);
            foreach (string key in m_FilePathsMap.Keys)
            {
                keys.Add(key);
            }
            foreach (string key in keys)
            {
                if (m_FilePathsMap[key] == originalPath)
                {
                    m_FilePathsMap[key] = newPath;
                }
            }
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

            string str;
            m_FilePathsMap.TryGetValue(sourceFilePath, out str);

            if (!string.IsNullOrEmpty(str)) //m_FilePathsMap.ContainsKey(sourceFilePath))
            {
                string fullPath = str; // m_FilePathsMap[sourceFilePath];
                if (File.Exists(fullPath))
                    return fullPath; // this should always be the case, unless the files have been removed from "outside" (i.e. user deleting the audio temporary directory while a session is alive)
#if DEBUG
                Debugger.Break();
#endif //DEBUG
                m_FilePathsMap.Remove(sourceFilePath);
            }

            string convertedFilePath = ConvertToDefaultFormat(sourceFilePath,
                m_destinationDirectory,
                m_destinationFormatInfo == null ? null : m_destinationFormatInfo.Copy(),
                m_SkipACM);

            if (RequestCancellation) return null;

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
        //public void DeleteSessionAudioFiles()
        //{
        //    List<string> presentationFilePaths = new List<string>();

        //    foreach (FileDataProvider f in m_Presentation.DataProviderManager.ManagedFileDataProviders)
        //    {
        //        presentationFilePaths.Add(f.DataFileFullPath);
        //    }

        //    string[] keysArray = new string[m_FilePathsMap.Keys.Count];
        //    m_FilePathsMap.Keys.CopyTo(keysArray, 0);

        //    for (int i = 0; i < keysArray.Length; i++)
        //    {
        //        string path = m_FilePathsMap[keysArray[i]];
        //        if (!presentationFilePaths.Contains(path))
        //        {
        //            if (File.Exists(path))
        //            {
        //                File.Delete(path);
        //            }
        //            m_FilePathsMap.Remove(keysArray[i]);
        //        }
        //    }
        //}



        /// <summary>
        /// takes wav file / mp3 file as input and converts it to wav file with audio format info supplied as parameter
        /// </summary>
        /// <param name="SourceFilePath"></param>
        /// <param name="destinationDirectory"></param>
        /// <param name="destinationFormatInfo"></param>
        /// <returns> full file path of converted file  </returns>
        private string ConvertToDefaultFormat(string SourceFilePath, string destinationDirectory, PCMFormatInfo destinationFormatInfo, bool skipACM)
        {
            if (!File.Exists(SourceFilePath))
                throw new FileNotFoundException(SourceFilePath);

            if (!Directory.Exists(destinationDirectory))
            {
                Directory.CreateDirectory(destinationDirectory);
            }

            AudioFileType sourceFileType = GetAudioFileType(SourceFilePath);
            switch (sourceFileType)
            {
                case AudioFileType.WavUncompressed:
                case AudioFileType.WavCompressed:
                    {
                        WavFormatConverter formatConverter1 = new WavFormatConverter(true, skipACM);

                        AddSubCancellable(formatConverter1);

                        string result = null;
                        try
                        {
                            result = formatConverter1.ConvertSampleRate(SourceFilePath, destinationDirectory,
                                                                        destinationFormatInfo != null
                                                                            ? destinationFormatInfo.Data
                                                                            : new AudioLibPCMFormat());
                        }
                        finally
                        {
                            RemoveSubCancellable(formatConverter1);
                        }

                        return result;
                    }
                case AudioFileType.Mp3:
                    {
                        WavFormatConverter formatConverter2 = new WavFormatConverter(true, skipACM);
                        
                        AddSubCancellable(formatConverter2);

                        string result = null;
                        try
                        {
                            result =  formatConverter2.UnCompressMp3File(SourceFilePath, destinationDirectory,
                            destinationFormatInfo != null ? destinationFormatInfo.Data : null);
                        }
                        finally
                        {
                            RemoveSubCancellable(formatConverter2);
                        }

                        return result;
                    }

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
            string ext = Path.GetExtension(filePath);
            if (string.Equals(ext, DataProviderFactory.AUDIO_MP3_EXTENSION, StringComparison.OrdinalIgnoreCase))
            {
                sourceFileType = AudioFileType.Mp3;
            }
            if (string.Equals(ext, DataProviderFactory.AUDIO_WAV_EXTENSION, StringComparison.OrdinalIgnoreCase))
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
