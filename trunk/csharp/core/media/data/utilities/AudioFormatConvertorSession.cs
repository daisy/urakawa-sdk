using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace urakawa.media.data.utilities
    {
    /// <summary>
    ///  class to maintain session for importing files of format different from default format of project
    /// will reduce re conversion of files already converted in the same session
    /// </summary>
    public class AudioFormatConvertorSession
        {
        private Dictionary<string, string> m_FilePathsMap; // maps source files to converted files
        Presentation m_Presentation;


                public AudioFormatConvertorSession ( Presentation presentation)
            {
            m_FilePathsMap = new Dictionary<string, string> ();
            m_Presentation = presentation;
                                                    }

/// <summary>
/// checks if wav file or mp3 file has already been converted in the same session, if it is already converted, returns file path of converted file
/// else convert to wav file with default project format and stores it in data directory of project.
/// </summary>
/// <param name="sourceFilePath"></param>
/// <returns> full path of converted file if conversion is successful </returns>
        public string ConvertAudioFileFormat ( string sourceFilePath )
            {
            if ( !File.Exists( sourceFilePath ))
                throw new System.IO.FileNotFoundException () ;

            if (m_FilePathsMap.ContainsKey ( sourceFilePath ))
                {
                return m_FilePathsMap[sourceFilePath];
                }
            else
                {
                                string convertedFilePath =  AudioFormatConvertor.ConvertToDefaultFormat ( m_Presentation, sourceFilePath );

                                if (File.Exists ( convertedFilePath ))
                                    {
                                    m_FilePathsMap.Add ( sourceFilePath, convertedFilePath );
                                    return convertedFilePath;
                                    }
                                else
                                    {
                                    throw new System.Exception ( "Operation failed" );
                                    }
                }

}


/// <summary>
/// Deletes audio files created during this session, do not deletes the file which is being referenced in presentation
/// </summary>
        public void DeleteSessionAudioFiles ()
            {
            List<string> presentationFilePaths = new List<string> ();

            foreach (FileDataProvider f in m_Presentation.DataProviderManager.ListOfFileDataProviders)
                {
                                presentationFilePaths.Add ( f.DataFileFullPath );
                }
            
            foreach (string pathKey in m_FilePathsMap.Keys)
                {
                string path = m_FilePathsMap[pathKey] ;
                if  ( !presentationFilePaths.Contains ( path )   &&    File.Exists (path) ) 
                    {
                    File.Delete ( path ) ;
                    m_FilePathsMap.Remove ( pathKey );
                    }

                }
                        }


        }
    }
