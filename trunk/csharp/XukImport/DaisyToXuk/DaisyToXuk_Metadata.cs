using System;
using System.Diagnostics;
using System.Xml;
using urakawa;
using urakawa.metadata;
using urakawa.metadata.daisy;

namespace XukImport
{
    public partial class DaisyToXuk
    {
        private void parseMetadata(XmlDocument xmlDoc)
        {
            parseMetadata_NameContentAll(xmlDoc);
            parseMetadata_ElementInnerTextAll(xmlDoc);
        }

        private void parseMetadata_ElementInnerTextAll(XmlDocument xmlDoc)
        {
            foreach (XmlNode node in getChildrenElementsWithName(xmlDoc, true, "metadata", null, false))
            {
                parseMetadata_ElementInnerText(node);
            }

            foreach (XmlNode node in getChildrenElementsWithName(xmlDoc, true, "dc-metadata", null, false))
            {
                parseMetadata_ElementInnerText(node);
            }

            foreach (XmlNode node in getChildrenElementsWithName(xmlDoc, true, "x-metadata", null, false))
            {
                parseMetadata_ElementInnerText(node);
            }
        }

        private void parseMetadata_NameContentAll(XmlDocument xmlDoc)
        {
            foreach (XmlNode node in getChildrenElementsWithName(xmlDoc, true, "meta", null, false))
            {
                parseMetadata_NameContent(node);
            }
        }

        private void parseMetadata_ElementInnerText(XmlNode metadataContainer)
        {
            foreach (XmlNode mdNode in metadataContainer.ChildNodes)
            {
                //string lowerCaseName = mdNode.Name.ToLower();
                string lowerCaseLocalName = mdNode.LocalName.ToLower();

                if (mdNode.NodeType == XmlNodeType.Element
                    && lowerCaseLocalName != "meta"
                    && lowerCaseLocalName != "dc-metadata"
                    && lowerCaseLocalName != "x-metadata"
                    && !String.IsNullOrEmpty(mdNode.InnerText))
                {
                    XmlNode mdIdentifier = mdNode.Attributes.GetNamedItem("id");

                    handleMetaData(mdNode, mdNode.Name, mdNode.InnerText, (mdIdentifier == null ? null : mdIdentifier.Value));
                }
            }
        }

        private void handleMetaDataOptionalAttrs(Metadata meta, XmlNode node)
        {
            if (meta != null)
            {
                for (int i = 0; i < node.Attributes.Count; i++)
                {
                    XmlAttribute attribute = node.Attributes[i];

                    string lowerCaseName = attribute.Name.ToLower();
                    string lowerCaseLocalName = attribute.LocalName.ToLower();

                    if (lowerCaseLocalName == "name"
                        || lowerCaseLocalName == "content")
                    {
                        continue;
                    }
                    if (lowerCaseLocalName == "id"
                        && node != m_PublicationUniqueIdentifierNode)
                    {
                        continue;
                    }

                    if (lowerCaseName.StartsWith("xmlns:"))
                    {
                        //meta.NameContentAttribute.NamespaceUri = attribute.Value;
                    }
                    else if (lowerCaseName == "xmlns")
                    {
                        //meta.OtherAttributes.NamespaceUri = attribute.Value;
                    }
                    else
                    {
                        MetadataAttribute xmlAttr = new MetadataAttribute();

                        xmlAttr.Name = attribute.Name;

                        if (lowerCaseName.Contains(":"))
                        {
                            xmlAttr.NamespaceUri = attribute.NamespaceURI;
                        }

                        xmlAttr.Value = attribute.Value;

                        meta.OtherAttributes.Insert(meta.OtherAttributes.Count, xmlAttr);
                    }
                }
            }
        }

        private void handleMetaData(XmlNode mdNode, string name, string content, string id)
        {
            if (isUniqueIdName(name))
            {
                if (m_PackageUniqueIdAttr != null
                    && id != null
                    && id == m_PackageUniqueIdAttr.Value)
                {
                    Debug.Assert(String.IsNullOrEmpty(m_PublicationUniqueIdentifier),
                        String.Format("The Publication's Unique Identifier is specified several times !! OLD: [{0}], NEW: [{1}]", m_PublicationUniqueIdentifier, content));

                    m_PublicationUniqueIdentifier = content;
                    m_PublicationUniqueIdentifierNode = mdNode;

                    Presentation presentation = m_Project.Presentations.Get(0);
                    foreach (Metadata md in presentation.Metadatas.ContentsAs_ListCopy)
                    {
                        if (isUniqueIdName(md.NameContentAttribute.Name)
                            && md.NameContentAttribute.Value == m_PublicationUniqueIdentifier)
                        {
                            presentation.Metadatas.Remove(md);
                        }
                    }
                }
                else if (!metadataUidValueAlreadyExists(content)
                    && (String.IsNullOrEmpty(m_PublicationUniqueIdentifier) || content != m_PublicationUniqueIdentifier))
                {
                    Metadata meta = addMetadata(name, content, mdNode);
                }
            }
            else
            {
                MetadataDefinition md = SupportedMetadata_Z39862005.GetMetadataDefinition(name);
                if ((md == null && !metadataNameContentAlreadyExists(name, content))
                    || (md != null && md.IsRepeatable && !metadataNameContentAlreadyExists(name, content))
                    || (md != null && !md.IsRepeatable && !metadataNameAlreadyExists(name)))
                {
                    Metadata meta = addMetadata(name, content, mdNode);
                }
            }
        }

        private static bool isUniqueIdName(string name)
        {
            string lower = name.ToLower();

            if ("dc:identifier" == lower)
            {
                return true;
            }

            MetadataDefinition md = SupportedMetadata_Z39862005.GetMetadataDefinition("dc:Identifier");
            return md != null && md.Synonyms.Find(
                                delegate(string s)
                                {
                                    return s.ToLower() == lower;
                                }) != null;
        }

        private void parseMetadata_NameContent(XmlNode metaDataNode)
        {
            if (metaDataNode.NodeType != XmlNodeType.Element || metaDataNode.LocalName != "meta")
            {
                return;
            }

            XmlAttributeCollection mdAttributes = metaDataNode.Attributes;

            if (mdAttributes == null || mdAttributes.Count <= 0)
            {
                return;
            }

            XmlNode attrName = mdAttributes.GetNamedItem("name");
            XmlNode attrContent = mdAttributes.GetNamedItem("content");

            if (attrName != null && !String.IsNullOrEmpty(attrName.Value)
                && attrContent != null && !String.IsNullOrEmpty(attrContent.Value))
            {
                XmlNode mdIdentifier = mdAttributes.GetNamedItem("id");

                handleMetaData(metaDataNode, attrName.Value, attrContent.Value, (mdIdentifier == null ? null : mdIdentifier.Value));
            }
        }

        private Metadata addMetadata(string name, string content, XmlNode node)
        {
            Presentation presentation = m_Project.Presentations.Get(0);

            Metadata md = presentation.MetadataFactory.CreateMetadata();
            md.NameContentAttribute = new MetadataAttribute();
            md.NameContentAttribute.Name = name; //.ToLower();
            md.NameContentAttribute.Value = content;

            if (md.NameContentAttribute.Name.Contains(":")
                && node.Name.ToLower() == md.NameContentAttribute.Name)
            {
                md.NameContentAttribute.NamespaceUri = node.NamespaceURI;
            }

            presentation.Metadatas.Insert(presentation.Metadatas.Count, md);

            handleMetaDataOptionalAttrs(md, node);

            return md;
        }

        private bool metadataNameContentAlreadyExists(string metaDataName, string metaDataContent)
        {
            string lower = metaDataName.ToLower();

            Presentation presentation = m_Project.Presentations.Get(0);
            foreach (Metadata md in presentation.Metadatas.ContentsAs_YieldEnumerable)
            {
                if (md.NameContentAttribute.Name.ToLower() == lower
                    && md.NameContentAttribute.Value == metaDataContent)
                {
                    return true;
                }
            }
            return false;
        }

        private bool metadataNameAlreadyExists(string metaDataName)
        {
            string lower = metaDataName.ToLower();

            Presentation presentation = m_Project.Presentations.Get(0);
            foreach (Metadata md in presentation.Metadatas.ContentsAs_YieldEnumerable)
            {
                if (md.NameContentAttribute.Name.ToLower() == lower)
                {
                    return true;
                }
            }
            return false;
        }

        private bool metadataUidValueAlreadyExists(string uid)
        {
            Presentation presentation = m_Project.Presentations.Get(0);

            foreach (Metadata md in presentation.Metadatas.ContentsAs_YieldEnumerable)
            {
                if (isUniqueIdName(md.NameContentAttribute.Name)
                    && md.NameContentAttribute.Value == uid)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
