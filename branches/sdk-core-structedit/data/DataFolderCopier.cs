using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using AudioLib;

namespace urakawa.data
{
    public class DataFolderCopier : DualCancellableProgressReporter
    {
        private readonly Presentation m_Presentation;

        //private readonly string m_parentFolderPath;
        //private readonly string m_prefixforSubDataFolder;

        private readonly string m_destinationFolderPath;

        public DataFolderCopier(Presentation presentation,
            //string parentFolderPath, string prefixforSubDataFolder
            string destinationFolderPath
            )
        {
            m_Presentation = presentation;

            //m_parentFolderPath = parentFolderPath;
            //m_prefixforSubDataFolder = prefixforSubDataFolder;

            m_destinationFolderPath = destinationFolderPath;
        }

        public override void DoWork()
        {
            CopyFileDataProvidersToDataFolderWithPrefix(m_destinationFolderPath
                //m_parentFolderPath, m_prefixforSubDataFolder
            );
        }

        public string CopyFileDataProvidersToDataFolderWithPrefix(string destinationFolderPath
            //string parentFolderPath, string prefixforSubDataFolder
            )
        {
            string fullDataFolderPath = destinationFolderPath;
            //Path.Combine(parentFolderPath,
            //                                          prefixforSubDataFolder +
            //                                          DataProviderManager.DefaultDataFileDirectorySeparator + DataProviderManager.DefaultDataFileDirectory);

            CopyFileDataProvidersToDataFolder(fullDataFolderPath);

            return fullDataFolderPath;
        }


        public void CopyFileDataProvidersToDataFolder(string fullDataFolderPath)
        {
            if (!Directory.Exists(fullDataFolderPath))
            {
                FileDataProvider.CreateDirectory(fullDataFolderPath);
            }

            reportProgress(-1, Path.GetFileName(fullDataFolderPath));

            int index = 0;
            const int progressStep = 10;
            int progress = progressStep;

            List<FileDataProvider> list = new List<FileDataProvider>(m_Presentation.DataProviderManager.ManagedFileDataProviders);
            foreach (FileDataProvider fdp in list)
            {
                index++;
                progress = 100 * index / list.Count;

                reportProgress_Throttle(progress, string.Format("{1}/{2} ({0})", fdp.DataFileRelativePath, index, list.Count));


                if (RequestCancellation) return;

                string pathSource = Path.Combine(m_Presentation.DataProviderManager.DataFileDirectoryFullPath, fdp.DataFileRelativePath);
                if (!File.Exists(pathSource))
                {
                    throw new exception.DataMissingException(String.Format("File does not exist: {0}", pathSource));
                }
                string pathDest = Path.Combine(fullDataFolderPath, fdp.DataFileRelativePath);
                if (!File.Exists(pathDest))
                {
                    File.Copy(pathSource, pathDest);
                    try
                    {
                        File.SetAttributes(pathDest, FileAttributes.Normal);
                    }
                    catch
                    {
                    }
                }
            }
        }
    }
}
