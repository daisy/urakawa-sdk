using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using urakawa.media.timing;
using urakawa.media.data.audio;
using urakawa.metadata;
using urakawa.metadata.daisy;
using urakawa.xuk;
using urakawa.property.alt;
using urakawa.media.data.audio.codec;
using urakawa.data;

using AudioLib;

namespace urakawa.daisy.export
{
    public partial class Daisy3_Export
    {
        public void CreateSmilNodesForImageDescription(urakawa.core.TreeNode n, XmlDocument smilDocument, XmlNode mainSeq, Time durationOfCurrentSmil, AlternateContentProperty altProperty, string smilFileName)
        {
            try
            {
                int counter = 0;
                foreach (string s in m_AltProperty_DescriptionMap[altProperty].ImageDescNodeToAltContentMap.Keys)
                {
                    AlternateContent altContent = m_AltProperty_DescriptionMap[altProperty].ImageDescNodeToAltContentMap[s];
                    if (altContent.Text == null) continue;
                    counter++;
                    if (m_Image_ProdNoteMap[n].Count <= counter) break;
                    XmlNode seqNode = smilDocument.CreateElement("seq", mainSeq.NamespaceURI);
                    mainSeq.AppendChild(seqNode);
                    XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, seqNode, "class", "prodnote");
                    string strSeqID = GetNextID(ID_SmilPrefix);
                    //System.Windows.Forms.MessageBox.Show(counter.ToString ()  + " : " + m_Image_ProdNoteMap[n].Count.ToString());
                    string dtbookID = m_Image_ProdNoteMap[n][counter].Attributes.GetNamedItem("id").Value;
                    string par_id = GetNextID(ID_SmilPrefix);
                    XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, seqNode, "id", strSeqID);
                    XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, seqNode, "class", "prodnote");
                    XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, seqNode, "customTest", "prodnote");
                    XmlDocumentHelper.CreateAppendXmlAttribute(m_DTBDocument, m_Image_ProdNoteMap[n][counter], "smilref", smilFileName + "#" + strSeqID);

                    XmlNode parNode = smilDocument.CreateElement(null, "par", mainSeq.NamespaceURI);
                    seqNode.AppendChild(parNode);
                    XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, parNode, "id", par_id);
                    XmlNode SmilTextNode = smilDocument.CreateElement(null, "text", mainSeq.NamespaceURI);
                    XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, SmilTextNode, "id", GetNextID(ID_SmilPrefix));
                    XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, SmilTextNode, "src",
                                                                               m_Filename_Content + "#" + dtbookID);
                    parNode.AppendChild(SmilTextNode);


                    if (altContent.Audio != null)
                    {
                        media.data.audio.ManagedAudioMedia managedAudio = altContent.Audio;
                        DataProvider dataProvider = ((WavAudioMediaData)managedAudio.AudioMediaData).ForceSingleDataProvider();

                        //string exportAudioName = ((FileDataProvider)dataProvider).DataFileRelativePath.Replace("" + Path.DirectorySeparatorChar, "_");
                        string exportAudioName = Path.GetFileNameWithoutExtension(smilFileName) + "_" + counter.ToString() + ".wav";
                        string imageSRC = m_FilesList_Image[m_FilesList_Image.Count - 1];
                        string imageDescriptionDirectoryPath = getAndCreateImageDescriptionDirectoryPath(imageSRC);
                        string destPath = Path.Combine(imageDescriptionDirectoryPath, exportAudioName);

                        if (!File.Exists(destPath))
                        {

                            dataProvider.ExportDataStreamToFile(destPath, false);

                            if (m_encodeToMp3)
                            {
                                string convertedFile = EncodeWavFileToMp3(destPath);
                                if (convertedFile != null) exportAudioName = Path.GetFileName(convertedFile);
                            }
                        }

                        //XmlDocumentHelper.CreateAppendXmlAttribute(descriptionDocument, contentXmlNode,
                        //DiagramContentModelHelper.TOBI_Audio, exportAudioName, DiagramContentModelHelper.NS_URL_TOBI);
                        DirectoryInfo d = new DirectoryInfo(imageDescriptionDirectoryPath);

                        string srcPath = d.Name + "/" + exportAudioName;

                        XmlNode audioNode = smilDocument.CreateElement(null, "audio", mainSeq.NamespaceURI);
                        XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, audioNode, "clipBegin",
                        "00:00:00");
                        XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, audioNode, "clipEnd",
                        FormatTimeString(managedAudio.Duration));
                        XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, audioNode, "src",
                                                                   srcPath);
                        parNode.AppendChild(audioNode);

                        if (!m_FilesList_Audio.Contains(srcPath)) m_FilesList_Audio.Add(srcPath);

                        // add to duration 
                        durationOfCurrentSmil.Add(managedAudio.Duration);
                    }
                }
            }
            catch (System.Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString());
            }
        }



        private Dictionary<AlternateContentProperty, XmlDocument> m_AltProperrty_DiagramDocument = new Dictionary<AlternateContentProperty, XmlDocument>();
        private void CreateSmilNodesForAltProperty(urakawa.core.TreeNode n, XmlDocument smilDocument, XmlNode mainSeq)
        {
            urakawa.property.alt.AlternateContentProperty altProperty = n.GetAlternateContentProperty();
            XmlDocument descriptionDocument = m_AltProperrty_DiagramDocument[altProperty];
            if (descriptionDocument != null)
            {
                XmlNode bodyNode = descriptionDocument.GetElementsByTagName("d:body")[0];
                foreach (XmlNode xn in bodyNode.ChildNodes)
                {
                    XmlNode parNode = smilDocument.CreateElement(null, "par", mainSeq.NamespaceURI);
                    mainSeq.AppendChild(parNode);
                    XmlNode SmilTextNode = smilDocument.CreateElement(null, "text", mainSeq.NamespaceURI);
                    XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, SmilTextNode, "id",
                                                               GetNextID(ID_SmilPrefix));
                    string dtbookID = null;
                    dtbookID = xn.Attributes.GetNamedItem("xml:id").Value;

                    //dtbookID = m_TreeNode_XmlNodeMap[n].Attributes != null
                    //? m_TreeNode_XmlNodeMap[n].Attributes.GetNamedItem("id").Value
                    //: m_TreeNode_XmlNodeMap[n.Parent].Attributes.GetNamedItem("id").Value;
                    XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, SmilTextNode, "src",
                                                               m_Filename_Content + "#" + dtbookID);
                    parNode.AppendChild(SmilTextNode);
                    /*
                    if (externalAudio != null)
                    {
                        XmlNode audioNode = smilDocument.CreateElement(null, "audio", mainSeq.NamespaceURI);
                        XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, audioNode, "clipBegin",
                                                                   FormatTimeString(externalAudio.ClipBegin));
                        XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, audioNode, "clipEnd",
                                                                   FormatTimeString(externalAudio.ClipEnd));
                        XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, audioNode, "src",
                                                                   Path.GetFileName(externalAudio.Src));
                        parNode.AppendChild(audioNode);

                        // add audio file name in audio files list for use in opf creation 
                        string audioFileName = Path.GetFileName(externalAudio.Src);
                        if (!m_FilesList_Audio.Contains(audioFileName)) m_FilesList_Audio.Add(audioFileName);

                        // add to duration 
                        durationOfCurrentSmil.Add(externalAudio.Duration);
                    }
                }
                    */
                    System.Windows.Forms.MessageBox.Show(xn.Name);
                }
            }
        }
    }
}
