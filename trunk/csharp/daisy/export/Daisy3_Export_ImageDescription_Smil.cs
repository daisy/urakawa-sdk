using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using urakawa.core;
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
        public class Description
        {
            public readonly Dictionary<string, AlternateContent> Map_DiagramElementName_TO_AltContent = new Dictionary<string, AlternateContent>();
        }
        private Dictionary<AlternateContentProperty, Description> m_Map_AltProperty_TO_Description = new Dictionary<AlternateContentProperty, Description>();



        private Dictionary<AlternateContent, string> m_Map_AltContentAudio_TO_RelativeExportedFilePath =
            new Dictionary<AlternateContent, string>();
        public void CreateSmilNodesForImageDescription(TreeNode levelNodeDescendant, XmlDocument smilDocument, XmlNode smilBodySeq, Time durationOfCurrentSmil, AlternateContentProperty altProperty, string smilFileName)
        {
            //try
            //{
            int counter = 0;
            foreach (string diagramDescriptionElementName in m_Map_AltProperty_TO_Description[altProperty].Map_DiagramElementName_TO_AltContent.Keys)
            {
                AlternateContent altContent = m_Map_AltProperty_TO_Description[altProperty].Map_DiagramElementName_TO_AltContent[diagramDescriptionElementName];
                if (altContent.Text == null) continue;
                counter++;
                if (m_Image_ProdNoteMap[levelNodeDescendant].Count <= counter)
                {
                    break;
                }
                XmlNode seqNode = smilDocument.CreateElement("seq", smilBodySeq.NamespaceURI);
                smilBodySeq.AppendChild(seqNode);
                XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, seqNode, "class", "prodnote");
                string strSeqID = GetNextID(ID_SmilPrefix);
                //System.Windows.Forms.MessageBox.Show(counter.ToString ()  + " : " + m_Image_ProdNoteMap[n].Count.ToString());
                string dtbookID = m_Image_ProdNoteMap[levelNodeDescendant][counter].Attributes.GetNamedItem("id").Value;
                string par_id = GetNextID(ID_SmilPrefix);
                XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, seqNode, "id", strSeqID);
                XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, seqNode, "class", "prodnote");
                XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, seqNode, "customTest", "prodnote");
                XmlDocumentHelper.CreateAppendXmlAttribute(m_DTBDocument, m_Image_ProdNoteMap[levelNodeDescendant][counter], "smilref",
                    FileDataProvider.UriEncode(smilFileName + "#" + strSeqID));

                XmlNode parNode = smilDocument.CreateElement(null, "par", smilBodySeq.NamespaceURI);
                seqNode.AppendChild(parNode);
                XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, parNode, "id", par_id);
                XmlNode SmilTextNode = smilDocument.CreateElement(null, "text", smilBodySeq.NamespaceURI);
                XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, SmilTextNode, "id", GetNextID(ID_SmilPrefix));
                XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, SmilTextNode, "src",
                                                                           FileDataProvider.UriEncode(m_Filename_Content + "#" + dtbookID));
                parNode.AppendChild(SmilTextNode);


                if (altContent.Audio != null)
                {
                    media.data.audio.ManagedAudioMedia managedAudio = altContent.Audio;
                    string srcPath = m_Map_AltContentAudio_TO_RelativeExportedFilePath[altContent];

                    DebugFix.Assert(!string.IsNullOrEmpty(srcPath));

                    XmlNode audioNode = smilDocument.CreateElement(null, "audio", smilBodySeq.NamespaceURI);
                    XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, audioNode, "clipBegin",
                    "00:00:00");
                    XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, audioNode, "clipEnd",
                    FormatTimeString(managedAudio.Duration));
                    XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, audioNode, "src",
                                                               FileDataProvider.UriEncode(srcPath));
                    parNode.AppendChild(audioNode);

                    if (!m_FilesList_SmilAudio.Contains(srcPath))
                    {
                        m_FilesList_SmilAudio.Add(srcPath);
                    }

                    // add to duration 
                    durationOfCurrentSmil.Add(managedAudio.Duration);
                }
            }
            //}
            //catch (System.Exception ex)
            //{
            //    System.Windows.Forms.MessageBox.Show(ex.ToString());
            //}
        }


        //private Dictionary<AlternateContentProperty, XmlDocument> m_AltProperrty_DiagramDocument = new Dictionary<AlternateContentProperty, XmlDocument>();
        //private void CreateSmilNodesForAltProperty(urakawa.core.TreeNode n, XmlDocument smilDocument, XmlNode mainSeq)
        //{
        //    urakawa.property.alt.AlternateContentProperty altProperty = n.GetAlternateContentProperty();
        //    XmlDocument descriptionDocument = m_AltProperrty_DiagramDocument[altProperty];
        //    if (descriptionDocument != null)
        //    {
        //        XmlNode bodyNode = descriptionDocument.GetElementsByTagName("d:body")[0];
        //        foreach (XmlNode xn in bodyNode.ChildNodes)
        //        {
        //            XmlNode parNode = smilDocument.CreateElement(null, "par", mainSeq.NamespaceURI);
        //            mainSeq.AppendChild(parNode);
        //            XmlNode SmilTextNode = smilDocument.CreateElement(null, "text", mainSeq.NamespaceURI);
        //            XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, SmilTextNode, "id",
        //                                                       GetNextID(ID_SmilPrefix));
        //            string dtbookID = null;
        //            dtbookID = xn.Attributes.GetNamedItem(XmlReaderWriterHelper.XmlId).Value;

        //            //dtbookID = m_TreeNode_XmlNodeMap[n].Attributes != null
        //            //? m_TreeNode_XmlNodeMap[n].Attributes.GetNamedItem("id").Value
        //            //: m_TreeNode_XmlNodeMap[n.Parent].Attributes.GetNamedItem("id").Value;
        //            XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, SmilTextNode, "src",
        //                                                       m_Filename_Content + "#" + dtbookID);
        //            parNode.AppendChild(SmilTextNode);
        //            /*
        //            if (externalAudio != null)
        //            {
        //                XmlNode audioNode = smilDocument.CreateElement(null, "audio", mainSeq.NamespaceURI);
        //                XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, audioNode, "clipBegin",
        //                                                           FormatTimeString(externalAudio.ClipBegin));
        //                XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, audioNode, "clipEnd",
        //                                                           FormatTimeString(externalAudio.ClipEnd));
        // string extAudioSrc = AdjustAudioFileName(externalAudio, levelNodeDescendant);
        //                XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, audioNode, "src",
        //                                                           Path.GetFileName(extAudioSrc));
        //                parNode.AppendChild(audioNode);

        //                // add audio file name in audio files list for use in opf creation 
        //                string audioFileName = Path.GetFileName(extAudioSrc);
        //                if (!m_FilesList_Audio.Contains(audioFileName)) m_FilesList_Audio.Add(audioFileName);

        //                // add to duration 
        //                durationOfCurrentSmil.Add(externalAudio.Duration);
        //            }
        //        }
        //            */
        //            //System.Windows.Forms.MessageBox.Show(xn.Name);
        //        }
        //    }
        //}

    }
}
