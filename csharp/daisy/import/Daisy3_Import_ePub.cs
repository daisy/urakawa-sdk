using System;
using System.IO;
using System.Xml;
using AudioLib;
using ICSharpCode.SharpZipLib.Zip;
using urakawa.xuk;

namespace urakawa.daisy.import
{
    /// <summary>
    /// This Class takes care of creating  XUK files of EPUB files.
    /// </summary>
    public partial class Daisy3_Import
    {
        private void unzipEPubAndParseOpf()
        {
            if (RequestCancellation) return;

            ZipInputStream zipInputStream = new ZipInputStream(File.OpenRead(m_Book_FilePath));

            /*string directoryName = Path.GetTempPath();
            if (!directoryName.EndsWith("" + Path.DirectorySeparatorChar))
            {
                directoryName += Path.DirectorySeparatorChar;
            }*/

            string unzipDirectory = Path.Combine(m_outDirectory, m_Book_FilePath.Replace(".", "_"));
            if (Directory.Exists(unzipDirectory))
            {
                Directory.Delete(unzipDirectory, true);
            }

            ZipEntry zipEntry;
            while ((zipEntry = zipInputStream.GetNextEntry()) != null)
            {
                if (RequestCancellation) return;

                string zipEntryName = Path.GetFileName(zipEntry.Name);
                if (!String.IsNullOrEmpty(zipEntryName)) // || zipEntryName.IndexOf(".ini") >= 0
                {
                    // string unzippedFilePath = Path.Combine(unzipDirectory, zipEntryName);
                    string unzippedFilePath = unzipDirectory + Path.DirectorySeparatorChar + zipEntry.Name;
                    string unzippedFileDir = Path.GetDirectoryName(unzippedFilePath);
                    if (!Directory.Exists(unzippedFileDir))
                    {
                        Directory.CreateDirectory(unzippedFileDir);
                    }

                    FileStream fileStream = File.Create(unzippedFilePath);

                    //byte[] data = new byte[2 * 1024]; // 2 KB buffer
                    //int bytesRead = 0;
                    try
                    {
                        const uint BUFFER_SIZE = 1024 * 2; // 2 KB MAX BUFFER
                        StreamUtils.Copy(zipInputStream, 0, fileStream, BUFFER_SIZE);

                        //while ((bytesRead = zipInputStream.Read(data, 0, data.Length)) > 0)
                        //{
                        //    fileStream.Write(data, 0, bytesRead);
                        //}
                    }
                    finally
                    {
                        fileStream.Close();
                    }
                }
            }
            zipInputStream.Close();

            var dirInfo = new DirectoryInfo(unzipDirectory);
            FileInfo[] opfFiles = dirInfo.GetFiles("*.opf ", SearchOption.AllDirectories);

            foreach (FileInfo fileInfo in opfFiles)
            {
                if (RequestCancellation) return;

                m_Book_FilePath = Path.Combine(unzipDirectory, fileInfo.FullName);
                XmlDocument opfXmlDoc = OpenXukAction.ParseXmlDocument(m_Book_FilePath, false);

                if (RequestCancellation) return;
                reportProgress(-1, String.Format(UrakawaSDK_daisy_Lang.ParsingOPF, fileInfo.FullName));            
                parseOpf(opfXmlDoc);

                break;
            }
        }
    }
}