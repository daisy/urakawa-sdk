using System;
using System.IO;
using System.Threading;
using System.Xml;
using System.Collections.Generic;
using AudioLib;
using ICSharpCode.SharpZipLib.Zip;

using urakawa.core;
using urakawa.data;
using urakawa.xuk;
using urakawa.property.xml;
using urakawa.property.channel;

namespace urakawa.daisy.import
{
    /// <summary>
    /// This Class takes care of creating  XUK files of EPUB files.
    /// </summary>
    public partial class Daisy3_Import
    {
        private void ParseHeadLinks(string book_FilePath, Project project, XmlDocument contentDoc)
        {
            Presentation presentation = project.Presentations.Get(0);

            List<string> externalFileRelativePaths = new List<string>();
            foreach (ExternalFiles.ExternalFileData extData in presentation.ExternalFilesDataManager.ManagedObjects.ContentsAs_Enumerable)
            {
                if (!string.IsNullOrEmpty(extData.OriginalRelativePath))
                {
                    string relPath = Path.GetFullPath(extData.OriginalRelativePath);
                    if (!externalFileRelativePaths.Contains(relPath))
                    {
                        externalFileRelativePaths.Add(relPath);
                    }
                }
            }

            XmlNode headXmlNode = XmlDocumentHelper.GetFirstChildElementOrSelfWithName(contentDoc.DocumentElement, true, "head", null);

            List<XmlNode> externalFilesLinks = new List<XmlNode>();
            externalFilesLinks.AddRange(XmlDocumentHelper.GetChildrenElementsOrSelfWithName(headXmlNode, true, "link", headXmlNode.NamespaceURI, false));
            externalFilesLinks.AddRange(XmlDocumentHelper.GetChildrenElementsOrSelfWithName(headXmlNode, true, "script", headXmlNode.NamespaceURI, false));
            externalFilesLinks.AddRange(XmlDocumentHelper.GetChildrenElementsOrSelfWithName(headXmlNode, true, "style", headXmlNode.NamespaceURI, false));

            foreach (XmlNode linkNode in externalFilesLinks)
            {
                TreeNode treeNode = presentation.TreeNodeFactory.Create();
                presentation.HeadNode.AppendChild(treeNode);
                XmlProperty xmlProp = presentation.PropertyFactory.CreateXmlProperty();
                treeNode.AddProperty(xmlProp);
                xmlProp.SetQName(linkNode.LocalName,
                    headXmlNode.NamespaceURI == linkNode.NamespaceURI ? "" : linkNode.NamespaceURI);
                //Console.WriteLine("XmlProperty: " + xmlProp.LocalName);

                foreach (System.Xml.XmlAttribute xAttr in linkNode.Attributes)
                {
                    xmlProp.SetAttribute(xAttr.LocalName,
                        linkNode.NamespaceURI == xAttr.NamespaceURI ? "" : xAttr.NamespaceURI,
                        xAttr.Value);

                    if ((xAttr.Name.Equals("href", StringComparison.OrdinalIgnoreCase)
                        || xAttr.Name.Equals("src", StringComparison.OrdinalIgnoreCase))
                        && !string.IsNullOrEmpty(xAttr.Value)
                        && !externalFileRelativePaths.Contains(Path.GetFullPath(xAttr.Value)))
                    {
                        ExternalFiles.ExternalFileData extData = CreateAndAddExternalFileData(book_FilePath, project, xAttr.Value);
                        if (extData != null)
                        {
                            externalFileRelativePaths.Add(Path.GetFullPath(extData.OriginalRelativePath));
                        }
                    }
                }
                string innerText = linkNode.InnerText; // TODO: what about CDATA?;

                if (!string.IsNullOrEmpty(innerText))
                {
                    urakawa.media.TextMedia textMedia = presentation.MediaFactory.CreateTextMedia();
                    textMedia.Text = innerText;
                    ChannelsProperty cProp = presentation.PropertyFactory.CreateChannelsProperty();
                    cProp.SetMedia(presentation.ChannelsManager.GetOrCreateTextChannel(), textMedia);
                    treeNode.AddProperty(cProp);
                    //Console.WriteLine("Link inner text: " + textMedia.Text);
                }
            }
        }

        private ExternalFiles.ExternalFileData CreateAndAddExternalFileData(string book_FilePath, Project project, string relativePath)
        {
            Presentation presentation = project.Presentations.Get(0);
            string fullPath = Path.Combine(
                    Path.GetDirectoryName(book_FilePath),
                    relativePath);

            if (File.Exists(fullPath))
            {
                ExternalFiles.ExternalFileData efd = null;
                string ext = Path.GetExtension(relativePath);
                if (String.Equals(ext, DataProviderFactory.STYLE_CSS_EXTENSION, StringComparison.OrdinalIgnoreCase))
                {
                    efd = presentation.ExternalFilesDataFactory.Create<ExternalFiles.CSSExternalFileData>();
                }
                else if (String.Equals(ext, DataProviderFactory.STYLE_PLS_EXTENSION, StringComparison.OrdinalIgnoreCase))
                {
                    efd = presentation.ExternalFilesDataFactory.Create<ExternalFiles.PLSExternalFileData>();
                }
                else if (String.Equals(ext, DataProviderFactory.STYLE_JS_EXTENSION, StringComparison.OrdinalIgnoreCase))
                {
                    efd = presentation.ExternalFilesDataFactory.Create<ExternalFiles.JSExternalFileData>();
                }
                else if (String.Equals(ext, DataProviderFactory.STYLE_XSLT_EXTENSION, StringComparison.OrdinalIgnoreCase)
                || String.Equals(ext, DataProviderFactory.STYLE_XSL_EXTENSION, StringComparison.OrdinalIgnoreCase))
                {
                    efd = presentation.ExternalFilesDataFactory.Create<ExternalFiles.XSLTExternalFileData>();
                }
                if (efd != null)
                {
                    efd.InitializeWithData(fullPath, relativePath, true);
                }
                return efd;
            }

            return null;
        }

        private void unzipEPubAndParseOpf()
        {
            if (RequestCancellation) return;

            ZipInputStream zipInputStream = new ZipInputStream(File.OpenRead(m_Book_FilePath));

            /*string directoryName = Path.GetTempPath();
            if (!directoryName.EndsWith("" + Path.DirectorySeparatorChar))
            {
                directoryName += Path.DirectorySeparatorChar;
            }*/

            string unzipDirectory = Path.Combine(
                Path.GetDirectoryName(m_Book_FilePath),
                //m_outDirectory,
                //FileDataProvider.EliminateForbiddenFileNameCharacters(m_Book_FilePath)
                //m_Book_FilePath.Replace('.', '_')
                m_Book_FilePath + "_UNZIPPED"
            );
            if (Directory.Exists(unzipDirectory))
            {
                FileDataProvider.DeleteDirectory(unzipDirectory);
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
                        FileDataProvider.CreateDirectory(unzippedFileDir);
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

            DirectoryInfo dirInfo = new DirectoryInfo(unzipDirectory);
            FileInfo[] opfFiles = dirInfo.GetFiles("*.opf ", SearchOption.AllDirectories);

            foreach (FileInfo fileInfo in opfFiles)
            {
                if (RequestCancellation) return;

                m_Book_FilePath = Path.Combine(unzipDirectory, fileInfo.FullName);
                m_Xuk_FilePath = GetXukFilePath(m_outDirectory, m_Book_FilePath);
                initializeProject();

                XmlDocument opfXmlDoc = XmlReaderWriterHelper.ParseXmlDocument(m_Book_FilePath, false);

                if (RequestCancellation) return;
                reportProgress(-1, String.Format(UrakawaSDK_daisy_Lang.ParsingOPF, fileInfo.FullName));
                parseOpf(opfXmlDoc);

                break;
            }
        }
    }
}