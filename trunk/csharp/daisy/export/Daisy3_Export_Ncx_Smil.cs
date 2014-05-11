using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml;
using urakawa.core;
using urakawa.media;
using urakawa.media.timing;
using urakawa.media.data.audio;
using urakawa.metadata;
using urakawa.metadata.daisy;
using urakawa.xuk;
using urakawa.property.alt;
using urakawa.media.data.audio.codec;
using urakawa.data;

using AudioLib;
using System.Text.RegularExpressions;

namespace urakawa.daisy.export
{
    public partial class Daisy3_Export
    {
        //DAISY3Export_Ncx

        protected string FormatTimeString(Time time)
        {
            return time.Format_StandardExpanded();
        }

        protected uint m_SmilFileNameCounter;

        private bool IsHeadingNode(TreeNode node)
        {
            if (node.HasXmlProperty)
            {
                string localName = node.GetXmlElementLocalName();

                if (localName == "levelhd"
                    || localName == "hd"
                    || localName == "h1"
                    || localName == "h2"
                    || localName == "h3"
                    || localName == "h4"
                    || localName == "h5"
                    || localName == "h6"
                    || localName == "doctitle")
                {
                    return true;
                }
            }

            return false;
        }

        private bool IsEscapableNode(TreeNode node)
        {
            if (node.HasXmlProperty)
            {
                string localName = node.GetXmlElementLocalName();

                if (localName == "list"
                    || localName == "table"
                    || localName == "tr"
                    || localName == "note"
                    || localName == "annotation"
                    || localName == "sidebar"
                    || localName == "prodnote"
                    || localName == "endnote"
                    || localName == "footnote"
                    || localName == "rearnote"
                    || localName == DiagramContentModelHelper.Math
                    )
                {
                    return true;
                }
            }
            return false;
        }

        private bool IsSkippableNode(TreeNode node)
        {
            if (node.HasXmlProperty)
            {
                string localName = node.GetXmlElementLocalName();

                if (localName == "pagenum"
                    || localName == "linenum"
                    || localName == "noteref"
                    || localName == "note"
                    || localName == "annoref"
                    || localName == "annotation"
                    || IsOptionalSidebarOrProducerNote(node))
                {
                    return true;
                }
            }
            return false;
        }

        private bool IsOptionalSidebarOrProducerNote(TreeNode node)
        {
            if (node.HasXmlProperty)
            {
                string localName = node.GetXmlElementLocalName();

                if (localName == "sidebar" || localName == "prodnote")
                {
                    foreach (urakawa.property.xml.XmlAttribute xmlAttr in node.GetXmlProperty().Attributes.ContentsAs_Enumerable)
                    {
                        string prefix = xmlAttr.Prefix;
                        string nameWithoutPrefix = xmlAttr.PrefixedLocalName != null ? xmlAttr.PrefixedLocalName : xmlAttr.LocalName;

                        if (nameWithoutPrefix == "render" && xmlAttr.Value == "optional")
                        {
                            return true;
                        }
                    } // foreach loop ends

                } // end check for sidebar and producer notes local name
            }
            return false;
        }

        private string prepareNcxLabelText(TreeNode n)
        {
            string str = n.GetTextFlattened();
            if (!string.IsNullOrEmpty(str))
            {
                str = Regex.Replace(str, @"\s+", " ");
                return str.Trim();
            }
            return str;
        }

        private XmlNode CreateDocTitle(XmlDocument ncxDocument, XmlNode ncxRootNode, TreeNode n)
        {
            XmlNode docNode = ncxDocument.CreateElement(null,
                "docTitle",
                 ncxRootNode.NamespaceURI);

            XmlNode navMapNode = XmlDocumentHelper.GetFirstChildElementOrSelfWithName(ncxDocument, true, "navMap", null);

            ncxRootNode.InsertBefore(docNode, navMapNode);

            XmlNode docTxtNode = ncxDocument.CreateElement(null, "text", docNode.NamespaceURI);
            docNode.AppendChild(docTxtNode);
            docTxtNode.AppendChild(
            ncxDocument.CreateTextNode(prepareNcxLabelText(n)));


            ExternalAudioMedia externalAudio = GetExternalAudioMedia(n);

            if (externalAudio != null)
            {
                // create audio node
                XmlNode docAudioNode = ncxDocument.CreateElement(null, "audio", docNode.NamespaceURI);
                docNode.AppendChild(docAudioNode);
                XmlDocumentHelper.CreateAppendXmlAttribute(ncxDocument, docAudioNode, "clipBegin", FormatTimeString(externalAudio.ClipBegin));
                XmlDocumentHelper.CreateAppendXmlAttribute(ncxDocument, docAudioNode, "clipEnd", FormatTimeString(externalAudio.ClipEnd));
                XmlDocumentHelper.CreateAppendXmlAttribute(ncxDocument, docAudioNode, "src", FileDataProvider.UriEncode(Path.GetFileName(externalAudio.Src)));
            }
            return docNode;
        }


        private XmlNode CreateNavPointWithoutContentNode(XmlDocument ncxDocument, TreeNode urakawaNode, TreeNode currentHeadingTreeNode, TreeNode n, Dictionary<TreeNode, XmlNode> treeNode_NavNodeMap)
        {
            XmlNode navMapNode = ncxDocument.GetElementsByTagName("navMap")[0];

            ExternalAudioMedia externalAudio = GetExternalAudioMedia(n);

            // first create navPoints
            XmlNode navPointNode = ncxDocument.CreateElement(null, "navPoint", navMapNode.NamespaceURI);
            if (currentHeadingTreeNode != null) XmlDocumentHelper.CreateAppendXmlAttribute(ncxDocument, navPointNode, "class", currentHeadingTreeNode.GetXmlProperty().LocalName);
            XmlDocumentHelper.CreateAppendXmlAttribute(ncxDocument, navPointNode, "id", GetNextID(ID_NcxPrefix));
            XmlDocumentHelper.CreateAppendXmlAttribute(ncxDocument, navPointNode, "playOrder", "");

            TreeNode parentNode = GetParentLevelNode(urakawaNode);

            if (parentNode == null)
            {
                navMapNode.AppendChild(navPointNode);
            }
            else
            {
                XmlNode obj;
                treeNode_NavNodeMap.TryGetValue(parentNode, out obj);

                if (obj != null) //treeNode_NavNodeMap.ContainsKey(parentNode))
                {
                    obj.AppendChild(navPointNode); //treeNode_NavNodeMap[parentNode]
                }
                else // surch up for node
                {
                    int counter = 0;
                    while (parentNode != null && counter <= 6)
                    {
                        parentNode = GetParentLevelNode(parentNode);

                        if (parentNode != null
                            //&& treeNode_NavNodeMap.ContainsKey(parentNode)
                            )
                        {
                            treeNode_NavNodeMap.TryGetValue(parentNode, out obj);
                            if (obj != null)
                            {
                                obj.AppendChild(navPointNode); //treeNode_NavNodeMap[parentNode]
                                break;
                            }
                        }
                        counter++;
                    }

                    if (parentNode == null || counter > 7)
                    {
                        navMapNode.AppendChild(navPointNode);
                    }
                }
            }


            treeNode_NavNodeMap.Add(urakawaNode, navPointNode);

            // create navLabel
            XmlNode navLabel = ncxDocument.CreateElement(null, "navLabel", navPointNode.NamespaceURI);
            navPointNode.AppendChild(navLabel);

            // create text node
            XmlNode txtNode = ncxDocument.CreateElement(null, "text", navMapNode.NamespaceURI);
            navLabel.AppendChild(txtNode);
            if (currentHeadingTreeNode != null)
                txtNode.AppendChild(
                ncxDocument.CreateTextNode(prepareNcxLabelText(n)));

            if (externalAudio != null)
            {
                // create audio node
                XmlNode audioNode = ncxDocument.CreateElement(null, "audio", navMapNode.NamespaceURI);
                navLabel.AppendChild(audioNode);
                XmlDocumentHelper.CreateAppendXmlAttribute(ncxDocument, audioNode, "clipBegin",
                                                           FormatTimeString(externalAudio.ClipBegin));
                XmlDocumentHelper.CreateAppendXmlAttribute(ncxDocument, audioNode, "clipEnd",
                                                           FormatTimeString(externalAudio.ClipEnd));
                XmlDocumentHelper.CreateAppendXmlAttribute(ncxDocument, audioNode, "src",
                                                           FileDataProvider.UriEncode(Path.GetFileName(externalAudio.Src)));
            }
            return navPointNode;
        }

        private TreeNode GetParentLevelNode(TreeNode node)
        {
            TreeNode parentNode = node.Parent;

            while (parentNode != null)
            {
                DebugFix.Assert(parentNode.HasXmlProperty);

                if (parentNode.GetXmlElementLocalName().StartsWith("level"))
                {
                    return parentNode;
                }
                parentNode = parentNode.Parent;
            }

            return null;
        }

        protected int GetDepthOfNavPointNode(XmlDocument doc, XmlNode navPointNode)
        {
            XmlNode parent = navPointNode.ParentNode;

            int i = 1;
            for (i = 1; i <= 9; i++)
            {

                if (parent != null && parent.LocalName == "navMap")
                {
                    return i;
                }
                parent = parent.ParentNode;
            }
            return i;
        }

        protected Metadata AddMetadata_DtbUid(bool asInnerText, XmlDocument doc, XmlNode parentNode)
        {
            Metadata mdUid = null;
            XmlNode metaNodeUid = null;
            foreach (Metadata md in m_Presentation.Metadatas.ContentsAs_Enumerable)
            {
                if (!isUniqueIdName(md.NameContentAttribute.Name)) continue;

                foreach (MetadataAttribute mda in md.OtherAttributes.ContentsAs_Enumerable)
                {
                    if (mda.Name != "id") continue;

                    //AddMetadataAsAttributes(ncxDocument, headNode, "dtb:uid", md.NameContentAttribute.Value);

                    if (asInnerText)
                    {
                        metaNodeUid = AddMetadataAsInnerText(doc, parentNode, SupportedMetadata_Z39862005.NS_PREFIX_DUBLIN_CORE + ":Identifier", md.NameContentAttribute.Value);
                        XmlDocumentHelper.CreateAppendXmlAttribute(doc, metaNodeUid, "id", "uid");
                    }
                    else
                    {
                        metaNodeUid = doc.CreateElement(null, "meta", parentNode.NamespaceURI);
                        parentNode.AppendChild(metaNodeUid);

                        XmlDocumentHelper.CreateAppendXmlAttribute(doc, metaNodeUid, DiagramContentModelHelper.Name, SupportedMetadata_Z39862005.DTB_UID);
                        XmlDocumentHelper.CreateAppendXmlAttribute(doc, metaNodeUid, DiagramContentModelHelper.Content, md.NameContentAttribute.Value);
                    }

                    mdUid = md;
                    break;
                }

                if (mdUid != null)
                {
                    // add metadata optional attributes if any
                    foreach (MetadataAttribute ma in md.OtherAttributes.ContentsAs_Enumerable)
                    {
                        if (ma.Name == "id" || ma.Name == Metadata.PrimaryIdentifierMark)
                        {
                            continue;
                        }

                        XmlDocumentHelper.CreateAppendXmlAttribute(doc, metaNodeUid, ma.Name, ma.Value);
                    }

                    return mdUid;
                }
            }

            foreach (Metadata md in m_Presentation.Metadatas.ContentsAs_Enumerable)
            {
                if (!isUniqueIdName(md.NameContentAttribute.Name)) continue;

                //AddMetadataAsAttributes(ncxDocument, headNode, "dtb:uid", md.NameContentAttribute.Value);
                if (asInnerText)
                {
                    metaNodeUid = AddMetadataAsInnerText(doc, parentNode, SupportedMetadata_Z39862005.NS_PREFIX_DUBLIN_CORE + ":Identifier", md.NameContentAttribute.Value);
                    XmlDocumentHelper.CreateAppendXmlAttribute(doc, metaNodeUid, "id", "uid");
                }
                else
                {
                    metaNodeUid = doc.CreateElement(null, "meta", parentNode.NamespaceURI);
                    parentNode.AppendChild(metaNodeUid);

                    XmlDocumentHelper.CreateAppendXmlAttribute(doc, metaNodeUid, DiagramContentModelHelper.Name, SupportedMetadata_Z39862005.DTB_UID);
                    XmlDocumentHelper.CreateAppendXmlAttribute(doc, metaNodeUid, DiagramContentModelHelper.Content,
                                                             md.NameContentAttribute.Value);
                }

                // add metadata optional attributes if any
                foreach (MetadataAttribute ma in md.OtherAttributes.ContentsAs_Enumerable)
                {
                    if (ma.Name == "id" || ma.Name == Metadata.PrimaryIdentifierMark)
                    {
                        continue;
                    }

                    XmlDocumentHelper.CreateAppendXmlAttribute(doc, metaNodeUid, ma.Name, ma.Value);
                }
                return md;
            }

            return null;
        }

        private static bool isUniqueIdName(string name)
        {
            if (SupportedMetadata_Z39862005.DC_Identifier.Equals(name, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            MetadataDefinition md = SupportedMetadata_Z39862005.DefinitionSet.GetMetadataDefinition(SupportedMetadata_Z39862005.DC_Identifier);
            //SupportedMetadata_Z39862005.NS_PREFIX_DUBLIN_CORE + ":Identifier"

            return md != null && md.Synonyms.Find(
                                delegate(string s)
                                {
                                    return name.Equals(s, StringComparison.OrdinalIgnoreCase);
                                }) != null;
        }

        protected void AddMetadata_Ncx(XmlDocument ncxDocument, string strTotalPages, string strMaxNormalPage, string strDepth, List<string> ncxCustomTestList)
        {
            XmlNode headNode = XmlDocumentHelper.GetFirstChildElementOrSelfWithName(ncxDocument, true, "head", null); //ncxDocument.GetElementsByTagName("head")[0];

            AddMetadata_DtbUid(false, ncxDocument, headNode);

            AddMetadata_Generator(ncxDocument, headNode);

            AddMetadataAsAttributes(ncxDocument, headNode, SupportedMetadata_Z39862005.DTB_DEPTH, strDepth);
            AddMetadataAsAttributes(ncxDocument, headNode, SupportedMetadata_Z39862005.DTB_TOTAL_PAGE_COUNT, strTotalPages);
            AddMetadataAsAttributes(ncxDocument, headNode, SupportedMetadata_Z39862005.DTB_MAX_PAGE_NUMBER, strMaxNormalPage);


            // add custom test to headNode
            if (ncxCustomTestList != null && ncxCustomTestList.Count > 0)
            {
                // create dictionary for custom test
                Dictionary<string, string> bookStrucMap = new Dictionary<string, string>();
                bookStrucMap.Add("pagenum", "PAGE_NUMBER");
                bookStrucMap.Add("linenum", "LINE_NUMBER");
                bookStrucMap.Add("note", "NOTE");
                bookStrucMap.Add("noteref", "NOTE_REFERENCE");
                bookStrucMap.Add("annoref", "NOTE_REFERENCE");
                bookStrucMap.Add("annotation", "ANNOTATION");
                bookStrucMap.Add("prodnote", "OPTIONAL_PRODUCER_NOTE");
                bookStrucMap.Add("sidebar", "OPTIONAL_SIDEBAR");
                bookStrucMap.Add("footnote", "NOTE");
                bookStrucMap.Add("endnote", "NOTE");

                foreach (string customTestName in ncxCustomTestList)
                {
                    if (!bookStrucMap.ContainsKey(customTestName)) continue;
                    XmlNode customTestNode = ncxDocument.CreateElement(null, "smilCustomTest", headNode.NamespaceURI);
                    headNode.AppendChild(customTestNode);
                    XmlDocumentHelper.CreateAppendXmlAttribute(ncxDocument, customTestNode, "bookStruct", bookStrucMap[customTestName]);
                    XmlDocumentHelper.CreateAppendXmlAttribute(ncxDocument, customTestNode, "defaultState", "false");

                    XmlDocumentHelper.CreateAppendXmlAttribute(ncxDocument, customTestNode, "id", customTestName);
                    XmlDocumentHelper.CreateAppendXmlAttribute(ncxDocument, customTestNode, "override", "visible");
                }
            }

        }

        protected void AddMetadata_Smil(XmlDocument smilDocument, string strElapsedTime, List<string> currentSmilCustomTestList)
        {
            XmlNode headNode = XmlDocumentHelper.GetFirstChildElementOrSelfWithName(smilDocument, true, "head", null); //smilDocument.GetElementsByTagName("head")[0];

            AddMetadata_DtbUid(false, smilDocument, headNode);

            AddMetadata_Generator(smilDocument, headNode);

            AddMetadataAsAttributes(smilDocument, headNode, SupportedMetadata_Z39862005.DTB_TOTAL_ELAPSED_TIME, strElapsedTime);


            //if (isCustomTestRequired)
            //{
            
            if (currentSmilCustomTestList != null &&  currentSmilCustomTestList.Count > 0)
            {
                XmlNode customAttributesNode = smilDocument.CreateElement(null, "customAttributes", headNode.NamespaceURI);
                headNode.AppendChild(customAttributesNode);
                foreach (string customTestName in currentSmilCustomTestList)
                {

                    XmlNode customTestNode = smilDocument.CreateElement(null, "customTest", headNode.NamespaceURI);
                    customAttributesNode.AppendChild(customTestNode);
                    XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, customTestNode, "defaultState", "false");
                    XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, customTestNode, "id", customTestName);
                    XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, customTestNode, "override", "visible");

                    //<customTest defaultState="false" id="note" override="visible" />
                }
            }
            
        }

        private XmlNode AddMetadataAsAttributes(XmlDocument doc, XmlNode headNode, string name, string content)
        {
            XmlNode metaNode = doc.CreateElement(null, "meta", headNode.NamespaceURI);
            headNode.AppendChild(metaNode);
            XmlDocumentHelper.CreateAppendXmlAttribute(doc, metaNode, "name", name);
            XmlDocumentHelper.CreateAppendXmlAttribute(doc, metaNode, "content", content);

            return metaNode;
        }

        private TreeNode GetReferedTreeNode(TreeNode node)
        {
            string noteRefID = node.GetXmlProperty().GetAttribute("idref").Value;

            //System.Windows.Forms.MessageBox.Show ( "Attributes xml " + noteRefID );

            noteRefID = noteRefID.Replace("#", "");
            foreach (TreeNode n in m_NotesNodeList)
            {
                string id = n.GetXmlElementId();

                if (!String.IsNullOrEmpty(id) && id == noteRefID)
                {

                    return n;
                }
            }

            return null;
        }

        protected string GetNextSmilFileName
        {
            get
            {
                string strNumericFrag = (++m_SmilFileNameCounter).ToString();
                return strNumericFrag.PadLeft(4, '0') + ".smil";
            }
        }


        protected XmlDocument CreateStub_NcxDocument()
        {
            XmlDocument NcxDocument = new XmlDocument();
            NcxDocument.XmlResolver = null;

            NcxDocument.CreateXmlDeclaration("1.0", "utf-8", null);
            NcxDocument.AppendChild(NcxDocument.CreateDocumentType("ncx",
                "-//NISO//DTD ncx 2005-1//EN",
                "http://www.daisy.org/z3986/2005/ncx-2005-1.dtd",
                null));

            XmlNode rootNode = NcxDocument.CreateElement(null,
                "ncx",
                "http://www.daisy.org/z3986/2005/ncx/");

            NcxDocument.AppendChild(rootNode);


            XmlDocumentHelper.CreateAppendXmlAttribute(NcxDocument, rootNode, "version", "2005-1");
            XmlDocumentHelper.CreateAppendXmlAttribute(NcxDocument, rootNode,
                XmlReaderWriterHelper.XmlLang,
                (string.IsNullOrEmpty(m_Presentation.Language)
                ? "en-US"
                : m_Presentation.Language));


            XmlNode headNode = NcxDocument.CreateElement(null, "head", rootNode.NamespaceURI);
            rootNode.AppendChild(headNode);

            XmlNode navMapNode = NcxDocument.CreateElement(null, "navMap", rootNode.NamespaceURI);
            rootNode.AppendChild(navMapNode);

            return NcxDocument;
        }

        protected XmlDocument CreateStub_SmilDocument()
        {
            XmlDocument smilDocument = new XmlDocument();
            smilDocument.XmlResolver = null;

            smilDocument.AppendChild(smilDocument.CreateXmlDeclaration("1.0", "utf-8", null));
            smilDocument.AppendChild(smilDocument.CreateDocumentType("smil",
                "-//NISO//DTD dtbsmil 2005-2//EN",
                    "http://www.daisy.org/z3986/2005/dtbsmil-2005-2.dtd",
                null));
            XmlNode smilRootNode = smilDocument.CreateElement(null,
                "smil", "http://www.w3.org/2001/SMIL20/");

            smilDocument.AppendChild(smilRootNode);

            XmlNode headNode = smilDocument.CreateElement(null, "head", smilRootNode.NamespaceURI);
            smilRootNode.AppendChild(headNode);
            XmlNode bodyNode = smilDocument.CreateElement(null, "body", smilRootNode.NamespaceURI);
            smilRootNode.AppendChild(bodyNode);
            XmlNode mainSeqNode = smilDocument.CreateElement(null, "seq", smilRootNode.NamespaceURI);
            bodyNode.AppendChild(mainSeqNode);

            return smilDocument;
        }

        private static string[] m_RomansSTR = { "M", "CM", "D", "CD", "C", "XC", "L", "XL", "X", "IX", "V", "IV", "I" };
        private static int[] m_RomansINT = { 1000, 900, 500, 400, 100, 90, 50, 40, 10, 9, 5, 4, 1 };
        public static int ParseRomanToInt(string roman)
        {
            int j = 0;
            int romanInt = 0;
            for (int i = 0; i < m_RomansSTR.Length; i++)
            {
                while (roman.IndexOf(m_RomansSTR[i], j, StringComparison.InvariantCultureIgnoreCase) == j)
                {
                    romanInt += m_RomansINT[i];
                    j += m_RomansSTR[i].Length;
                }
            }
            return romanInt;
        }
    }
}
