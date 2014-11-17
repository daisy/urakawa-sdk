using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;
using System.IO;
using AudioLib;
using urakawa.core;
using urakawa.data;
using urakawa.media.data.audio;
using urakawa.media.data.audio.codec;
using urakawa.property.alt;
using urakawa.metadata;
using urakawa.xuk;

namespace urakawa.daisy.export
{
    public partial class Daisy3_Export
    {
        public const string DIAGRAM_XML_PARSE_FAIL = "_DIAGRAM_XML_PARSE_FAIL_";

        private void generateImageDescriptionInDTBook(TreeNode n, XmlNode currentXmlNode, string exportImageName, XmlDocument DTBookDocument)
        {
            AlternateContentProperty altProp = n.GetAlternateContentProperty();

            if (currentXmlNode.LocalName == null
                || !currentXmlNode.LocalName.Equals("img", StringComparison.OrdinalIgnoreCase)
                || altProp == null || altProp.IsEmpty)
            {
                return;
            }

            m_Map_AltProperty_TO_Description.Add(altProp, new Description());

            PCMFormatInfo audioFormat = m_Presentation.MediaDataManager.DefaultPCMFormat;
            AudioLibPCMFormat pcmFormat = audioFormat.Data;
            pcmFormat.SampleRate = (ushort)m_sampleRate;
            pcmFormat.NumberOfChannels = (ushort)(m_audioStereo ? 2 : 1);

            Dictionary<string, List<string>> map_DiagramElementName_TO_TextualDescriptions = new Dictionary<string, List<string>>();

            string imageDescriptionDirectoryPath = GetAndCreateImageDescriptionDirectoryPath(true, exportImageName, m_OutputDirectory);
            string descriptionFile = CreateImageDescription(m_SkipACM, pcmFormat, m_encodeAudioFiles, m_BitRate_Encoding,
                imageDescriptionDirectoryPath, exportImageName,
                altProp,
                map_DiagramElementName_TO_TextualDescriptions,
                m_Map_AltProperty_TO_Description,
                m_Map_AltContentAudio_TO_RelativeExportedFilePath);

            if (m_includeImageDescriptions && !String.IsNullOrEmpty(descriptionFile))
            {
                //short term way for executing image description code: will be updated in later phase of implementation
                XmlNode prodNoteNode = DTBookDocument.CreateElement("prodnote", currentXmlNode.NamespaceURI);
                XmlDocumentHelper.CreateAppendXmlAttribute(DTBookDocument, prodNoteNode, "render", "optional");
                string id_Prodnote = GetNextID(ID_DTBPrefix);
                XmlDocumentHelper.CreateAppendXmlAttribute(DTBookDocument, prodNoteNode, "id", id_Prodnote);
                XmlDocumentHelper.CreateAppendXmlAttribute(DTBookDocument, prodNoteNode, "imgref", currentXmlNode.Attributes.GetNamedItem("id").Value);
                XmlDocumentHelper.CreateAppendXmlAttribute(DTBookDocument, prodNoteNode, "class", DiagramContentModelHelper.EPUB_DescribedAt);

                currentXmlNode.ParentNode.AppendChild(prodNoteNode);
                if (!m_Image_ProdNoteMap.ContainsKey(n))
                {
                    m_Image_ProdNoteMap.Add(n, new List<XmlNode>());
                }
                m_Image_ProdNoteMap[n].Add(prodNoteNode);
                XmlNode anchorNode = DTBookDocument.CreateElement("a", currentXmlNode.NamespaceURI);

                XmlNode pAnchor = DTBookDocument.CreateElement(
                    DiagramContentModelHelper.P , currentXmlNode.NamespaceURI
                    );
                pAnchor.AppendChild(anchorNode);
                prodNoteNode.AppendChild(pAnchor);
                string descriptionFileUrl = descriptionFile.Replace('\\', '/');

                XmlDocumentHelper.CreateAppendXmlAttribute(DTBookDocument, anchorNode, "href", FileDataProvider.UriEncode(descriptionFileUrl));
                XmlDocumentHelper.CreateAppendXmlAttribute(DTBookDocument, anchorNode, "external", "true");
                anchorNode.AppendChild(DTBookDocument.CreateTextNode("Image description (DIAGRAM XML)"));

                if (map_DiagramElementName_TO_TextualDescriptions.Count > 0)
                {
                    foreach (string diagramDescriptionElementName in map_DiagramElementName_TO_TextualDescriptions.Keys)
                    {
                        //System.Windows.Forms.MessageBox.Show(s + " : " + imageDescriptions[s]);

                        XmlNode prodNoteDesc = DTBookDocument.CreateElement("prodnote", currentXmlNode.NamespaceURI);
                        XmlDocumentHelper.CreateAppendXmlAttribute(DTBookDocument, prodNoteDesc, "render", "optional");
                        string id_ProdnoteDesc = GetNextID(ID_DTBPrefix);
                        XmlDocumentHelper.CreateAppendXmlAttribute(DTBookDocument, prodNoteDesc, "id", id_ProdnoteDesc);
                        XmlDocumentHelper.CreateAppendXmlAttribute(DTBookDocument, prodNoteDesc, "imgref", currentXmlNode.Attributes.GetNamedItem("id").Value);
                        XmlDocumentHelper.CreateAppendXmlAttribute(DTBookDocument, prodNoteDesc, "class", diagramDescriptionElementName);
                        currentXmlNode.ParentNode.AppendChild(prodNoteDesc);
                        m_Image_ProdNoteMap[n].Add(prodNoteDesc);

                        foreach (string txt in map_DiagramElementName_TO_TextualDescriptions[diagramDescriptionElementName])
                        {
                            string descText = txt;

                            bool xmlParseFail = descText.StartsWith(DIAGRAM_XML_PARSE_FAIL);

                            bool descriptionTextContainsMarkup = !xmlParseFail && descText.IndexOf('<') >= 0; // descText.Contains("<");
                            if (descriptionTextContainsMarkup)
                            {
                                try
                                {
                                    prodNoteDesc.InnerXml = descText;
                                }
                                catch (Exception ex)
                                {
#if DEBUG
                                    Debugger.Break();
#endif
                                    Console.WriteLine(@"Cannot set DIAGRAM XML: " + descText);

                                    XmlNode wrapperNode = DTBookDocument.CreateElement(DiagramContentModelHelper.CODE,
                                        currentXmlNode.NamespaceURI);
                                    prodNoteDesc.AppendChild(wrapperNode);
                                    wrapperNode.AppendChild(DTBookDocument.CreateTextNode(descText));
                                }
                            }
                            else if (xmlParseFail)
                            {
                                //descText = descText.Replace(DIAGRAM_XML_PARSE_FAIL, "");
                                descText = descText.Substring(DIAGRAM_XML_PARSE_FAIL.Length);

                                XmlNode wrapperNode = DTBookDocument.CreateElement(DiagramContentModelHelper.CODE, currentXmlNode.NamespaceURI);
                                prodNoteDesc.AppendChild(wrapperNode);
                                wrapperNode.AppendChild(DTBookDocument.CreateTextNode(descText));
                            }
                            else
                            {
                                string normalizedText = descText.Replace("\r\n", "\n");

                                string[] parasText = normalizedText.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                                //string[] parasText = System.Text.RegularExpressions.Regex.Split(normalizedText, "\n");

                                for (int i = 0; i < parasText.Length; i++)
                                {
                                    string paraText = parasText[i].Trim();
                                    if (string.IsNullOrEmpty(paraText))
                                    {
                                        continue;
                                    }

                                    XmlNode paragraph = DTBookDocument.CreateElement(
                                        //DiagramContentModelHelper.NS_PREFIX_ZAI,
                                        DiagramContentModelHelper.P
                                        , currentXmlNode.NamespaceURI
                                        //, DiagramContentModelHelper.NS_URL_ZAI
                                        );

                                    paragraph.InnerText = paraText;

                                    prodNoteDesc.AppendChild(paragraph);
                                }
                            }
                        }
                    }
                }

                /*
                if ( EXPORT_IMAGE_DESCRIPTION_IN_DTBOOK )
                {//1
                // to do copy the diagram nodes that descend directly from body
                    if (m_AltProperrty_DiagramDocument.ContainsKey(n.GetAlternateContentProperty()))
                    {//2

                        XmlDocumentHelper.CreateAppendXmlAttribute(DTBookDocument, DTBookDocument.GetElementsByTagName("dtbook")[0],
XmlReaderWriterHelper.NS_PREFIX_XMLNS+":" + DiagramContentModelHelper.NS_PREFIX_DIAGRAM,
DiagramContentModelHelper.NS_URL_DIAGRAM);
                        XmlDocument descriptionDocument = m_AltProperrty_DiagramDocument[n.GetAlternateContentProperty()];
                        XmlNodeList diagramNodesList = descriptionDocument.GetElementsByTagName("d:body")[0].ChildNodes;
                        foreach (XmlNode xn in diagramNodesList)
                        {//3
                            XmlNode newNode = DTBookDocument.ImportNode(xn, true);
                            prodNoteNode.AppendChild(newNode);
                            for (int i = 0; i < newNode.Attributes.Count; i++)
                            {//4
                                XmlAttribute attr = newNode.Attributes[i];
                                if (attr.Name == DiagramContentModelHelper.NS_PREFIX_XML + ":id")
                                {//-4
                                    XmlDocumentHelper.CreateAppendXmlAttribute(DTBookDocument, newNode, "id", attr.Value);
                                    newNode.Attributes.Remove(attr);
                                }//-3
                            }//-2
                                              
                        }//-1
                                             
                    }
                                              
                //XmlNode newNode = DTBookDocument.ImportNode(M_DescriptionDocument.GetElementsByTagName("d:description")[0], true);
                    //prodNoteNode.AppendChild(newNode);
                }
                */
            }
            //}
            //catch (System.Exception ex)
            //{
            //System.Windows.Forms.MessageBox.Show(ex.ToString());
            //}
            //}

            //}

        }
    }
}
