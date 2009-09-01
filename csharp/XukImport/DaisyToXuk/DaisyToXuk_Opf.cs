using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace XukImport
{
    public partial class DaisyToXuk
    {
        private XmlNode m_PackageUniqueIdAttr;
        private string m_PublicationUniqueIdentifier;
        private XmlNode m_PublicationUniqueIdentifierNode;

        private void parseOpf(XmlDocument opfXmlDoc)
        {
            foreach (XmlNode packageNode in getChildrenElementsWithName(opfXmlDoc, true, "package", null, true))
            {
                XmlAttributeCollection packageNodeAttrs = packageNode.Attributes;
                if (packageNodeAttrs != null && packageNodeAttrs.Count > 0)
                {
                    m_PackageUniqueIdAttr = packageNodeAttrs.GetNamedItem("unique-identifier");
                    break;
                }
            }

            parseMetadata(opfXmlDoc);

            List<string> spine;
            string spineMimeType;
            string dtbookPath;
            string ncxPath;

            parseOpfManifest(opfXmlDoc, out spine, out spineMimeType, out dtbookPath, out ncxPath);

            if (dtbookPath != null)
            {
                string fullDtbookPath = Path.Combine(Path.GetDirectoryName(m_Book_FilePath), dtbookPath);
                XmlDocument dtbookXmlDoc = readXmlDocument(fullDtbookPath);
                parseMetadata(dtbookXmlDoc);
                parseContentDocument(dtbookXmlDoc, null, fullDtbookPath);
            }

            if (false && ncxPath != null) //we skip NCX metadata parsing (we get publication metadata only from OPF and DTBOOK/XHTMLs)
            {
                string fullNcxPath = Path.Combine(Path.GetDirectoryName(m_Book_FilePath), ncxPath);
                XmlDocument ncxXmlDoc = readXmlDocument(fullNcxPath);
                parseMetadata(ncxXmlDoc);
            }

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

            XmlNode spineNodeRoot = null;
            foreach (XmlNode node in getChildrenElementsWithName(opfXmlDoc, true, "spine", null, true))
            {
                spineNodeRoot = node;
                break;
            }

            if (spineNodeRoot != null)
            {
                XmlNodeList listOfSpineItemNodes = spineNodeRoot.ChildNodes;
                foreach (XmlNode spineItemNode in listOfSpineItemNodes)
                {
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

            XmlNode manifNodeRoot = null;
            foreach (XmlNode node in getChildrenElementsWithName(opfXmlDoc, true, "manifest", null, true))
            {
                manifNodeRoot = node;
                break;
            }

            if (manifNodeRoot == null)
            {
                return;
            }

            XmlNodeList listOfManifestItemNodes = manifNodeRoot.ChildNodes;

            foreach (XmlNode manifItemNode in listOfManifestItemNodes)
            {
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
    }
}
