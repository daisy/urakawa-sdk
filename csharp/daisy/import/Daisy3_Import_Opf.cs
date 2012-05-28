using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using urakawa.xuk;

namespace urakawa.daisy.import
{
    public partial class Daisy3_Import
    {
        private XmlNode m_PackageUniqueIdAttr;
        private string m_PublicationUniqueIdentifier;
        private XmlNode m_PublicationUniqueIdentifierNode;

        private void parseOpf(XmlDocument opfXmlDoc)
        {
            XmlNode packageNode = XmlDocumentHelper.GetFirstChildElementOrSelfWithName(opfXmlDoc, true, "package", null);
            if (packageNode != null)
            {
                XmlAttributeCollection packageNodeAttrs = packageNode.Attributes;
                if (packageNodeAttrs != null && packageNodeAttrs.Count > 0)
                {
                    m_PackageUniqueIdAttr = packageNodeAttrs.GetNamedItem("unique-identifier");
                }
            }

            if (RequestCancellation) return;
            parseMetadata(opfXmlDoc);

            List<string> spine;
            string spineMimeType;
            string dtbookPath;
            string ncxPath;

            if (RequestCancellation) return;
            parseOpfManifest(opfXmlDoc, out spine, out spineMimeType, out dtbookPath, out ncxPath);

            if (dtbookPath != null && !AudioNCXImport)
            {
                if (RequestCancellation) return;
                string fullDtbookPath = Path.Combine(Path.GetDirectoryName(m_Book_FilePath), dtbookPath);
                XmlDocument dtbookXmlDoc = XmlReaderWriterHelper.ParseXmlDocument(fullDtbookPath, true);

                if (RequestCancellation) return;
                reportProgress(-1, String.Format(UrakawaSDK_daisy_Lang.ParsingMetadata, dtbookPath));
                parseMetadata(dtbookXmlDoc);

                if (RequestCancellation) return;
                reportProgress(-1, String.Format(UrakawaSDK_daisy_Lang.ParsingContent, dtbookPath));
                parseContentDocument(dtbookXmlDoc, null, fullDtbookPath);
            }

            //if (false && ncxPath != null) //we skip NCX metadata parsing (we get publication metadata only from OPF and DTBOOK/XHTMLs)
            if (
                //(string.IsNullOrEmpty(dtbookPath) && !string.IsNullOrEmpty(ncxPath))  ||
                AudioNCXImport)
            {
                //m_IsAudioNCX = true;
                if (RequestCancellation) return;
                string fullNcxPath = Path.Combine(Path.GetDirectoryName(m_Book_FilePath), ncxPath);
                XmlDocument ncxXmlDoc = XmlReaderWriterHelper.ParseXmlDocument(fullNcxPath, false);

                if (RequestCancellation) return;
                reportProgress(-1, "Parsing metadata: [" + ncxPath + "]");
                parseMetadata(ncxXmlDoc);

                if (AudioNCXImport)
                {
                    ParseNCXDocument(ncxXmlDoc);
                }
            }

            if (RequestCancellation) return;
            switch (spineMimeType)
            {
                case "application/smil":
                    {
                        parseSmiles(spine);
                        break;
                    }
                case "application/xhtml+xml":
                    {
                        parseContentDocuments(spine);
                        break;
                    }
            }
        }

        private void parseOpfManifest(XmlDocument opfXmlDoc,
                                out List<string> spine,
                                out string spineMimeType,
                                out string dtbookPath,
                                out string ncxPath)
        {
            spine = new List<string>();
            spineMimeType = null;
            ncxPath = null;
            dtbookPath = null;

            XmlNode spineNodeRoot = XmlDocumentHelper.GetFirstChildElementOrSelfWithName(opfXmlDoc, true, "spine", null);

            if (spineNodeRoot != null)
            {
                XmlNodeList listOfSpineItemNodes = spineNodeRoot.ChildNodes;
                foreach (XmlNode spineItemNode in listOfSpineItemNodes)
                {
                    if (RequestCancellation) return;

                    if (spineItemNode.NodeType != XmlNodeType.Element
                        || spineItemNode.LocalName != "itemref")
                    {
                        continue;
                    }
                    XmlAttributeCollection spineItemAttributes = spineItemNode.Attributes;

                    if (spineItemAttributes == null || spineItemAttributes.Count <= 0)
                    {
                        continue;
                    }

                    XmlNode attrIdRef = spineItemAttributes.GetNamedItem("idref");
                    if (attrIdRef != null && !string.IsNullOrEmpty(attrIdRef.Value))
                    {
                        spine.Add(attrIdRef.Value);
                    }
                }
            }

            XmlNode manifNodeRoot = XmlDocumentHelper.GetFirstChildElementOrSelfWithName(opfXmlDoc, true, "manifest", null);
            if (manifNodeRoot == null)
            {
                return;
            }

            XmlNodeList listOfManifestItemNodes = manifNodeRoot.ChildNodes;

            foreach (XmlNode manifItemNode in listOfManifestItemNodes)
            {
                if (RequestCancellation) return;

                if (manifItemNode.NodeType != XmlNodeType.Element
                    || manifItemNode.LocalName != "item")
                {
                    continue;
                }
                XmlAttributeCollection manifItemAttributes = manifItemNode.Attributes;

                if (manifItemAttributes == null)
                {
                    continue;
                }
                XmlNode attrId = manifItemAttributes.GetNamedItem("id");
                XmlNode attrHref = manifItemAttributes.GetNamedItem("href");
                XmlNode attrMediaType = manifItemAttributes.GetNamedItem("media-type");

                if (attrHref == null || String.IsNullOrEmpty(attrHref.Value)
                    || attrMediaType == null || String.IsNullOrEmpty(attrMediaType.Value))
                {
                    continue;
                }

                if (attrMediaType.Value == "application/smil"
                    || attrMediaType.Value == "application/xhtml+xml")
                {
                    if (attrId != null)
                    {
                        int i = spine.IndexOf(attrId.Value);
                        if (i >= 0)
                        {
                            spine[i] = attrHref.Value;
                            if (!string.IsNullOrEmpty(spineMimeType)
                                && spineMimeType != attrMediaType.Value)
                            {
                                System.Diagnostics.Debug.Fail(String.Format("Spine contains different mime-types ?! {0} vs {1}", attrMediaType.Value, spineMimeType));
                            }
                            spineMimeType = attrMediaType.Value;
                        }
                    }
                }
                else if (attrMediaType.Value == "application/x-dtbook+xml")
                {
                    dtbookPath = attrHref.Value;
                }
                else if (attrMediaType.Value == "application/x-dtbncx+xml"
                    || attrMediaType.Value == "text/xml" && attrHref.Value.EndsWith(".ncx"))
                {
                    ncxPath = attrHref.Value;
                }
                else if (attrMediaType.Value == "text/css"
                    || attrMediaType.Value == "application/pls+xml")
                {
                    AddExternalFilesToXuk(m_Project.Presentations.Get(0), attrHref.Value);
                }
                    
                else if (attrMediaType.Value == "image/jpeg"    
                || attrMediaType.Value == "audio/mpeg"
                || attrMediaType.Value == "text/css"
                || attrMediaType.Value == "text/xml"
                || attrMediaType.Value == "application/vnd.adobe.page-template+xml"
                || attrMediaType.Value == "application/oebps-page-map+xml"
                || attrMediaType.Value == "application/x-dtbresource+xml")
                {
                    // Ignore
                }
            }
        }

        private void AddExternalFilesToXuk(Presentation presentation, string relPath)
        {
            if (RequestCancellation) return;

            string fileFullPath = Path.Combine(Path.GetDirectoryName(m_Book_FilePath),
                relPath);

            ExternalFiles.ExternalFileData externalData = null;

            string ext = Path.GetExtension(fileFullPath);
            if (String.Equals(ext, ".dtd", StringComparison.OrdinalIgnoreCase))
            {
                externalData = presentation.ExternalFilesDataFactory.Create<ExternalFiles.DTDExternalFileData>();
            }
            if (String.Equals(ext, ".pls", StringComparison.OrdinalIgnoreCase))
            {
                externalData = presentation.ExternalFilesDataFactory.Create<ExternalFiles.PLSExternalFileData>();
            }
            if (externalData != null)
            {
                externalData.InitializeWithData(fileFullPath, relPath, true);
            }
        }
    }
}
