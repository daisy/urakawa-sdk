using System;
using System.IO;
using System.Threading;
using System.Xml;
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

        private void ParseHeadLinks(XmlDocument contentDoc)
        {
            XmlNode headXmlNode = XmlDocumentHelper.GetFirstChildElementOrSelfWithName(contentDoc.DocumentElement, true, "head", null);
            Presentation presentation = m_Project.Presentations.Get(0) ;
            TreeNode headTreeNode = presentation.HeadNode;
            
            foreach ( XmlNode linkNode in XmlDocumentHelper.GetChildrenElementsOrSelfWithName (headXmlNode, true,"link",headXmlNode.NamespaceURI,false ))
            {
                TreeNode treeNode = presentation.TreeNodeFactory.Create();
                headTreeNode.AppendChild(treeNode);
                XmlProperty xmlProp = presentation.PropertyFactory.CreateXmlProperty();
                treeNode.AddProperty(xmlProp);
                xmlProp.SetQName(linkNode.LocalName,
                    headXmlNode.NamespaceURI == linkNode.NamespaceURI ? "" : linkNode.NamespaceURI);
                Console.WriteLine("XmlProperty: " + xmlProp.LocalName);
                foreach (System.Xml.XmlAttribute xAttr in linkNode.Attributes)
                {
                    xmlProp.SetAttribute(xAttr.LocalName,
                        linkNode.NamespaceURI == xAttr.NamespaceURI ? "" : xAttr.NamespaceURI,
                        xAttr.Value);
                    //if (xmlProp.Attributes.Count > 0)
                    //Console.WriteLine("Link attribute: " + xmlProp.Attributes.ContentsAs_ListAsReadOnly[xmlProp.Attributes.Count - 1]);
                }
                if ( !string.IsNullOrEmpty (linkNode.InnerText ))
                {
                     urakawa.media.TextMedia textMedia = presentation.MediaFactory.CreateTextMedia();
                        textMedia.Text = linkNode.InnerText;
                        ChannelsProperty cProp = presentation.PropertyFactory.CreateChannelsProperty();
                        cProp.SetMedia(m_textChannel, textMedia);
                    treeNode.AddProperty (cProp) ;
                    Console.WriteLine("Link inner text: " + textMedia.Text);
                }
            }
            
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

            string unzipDirectory = Path.Combine(m_outDirectory, m_Book_FilePath.Replace('.', '_'));
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
                XmlDocument opfXmlDoc = XmlReaderWriterHelper.ParseXmlDocument(m_Book_FilePath, false);

                if (RequestCancellation) return;
                reportProgress(-1, String.Format(UrakawaSDK_daisy_Lang.ParsingOPF, fileInfo.FullName));            
                parseOpf(opfXmlDoc);

                break;
            }
        }
    }
}