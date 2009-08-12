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
            XmlNodeList listOfMetadataContainers = xmlDoc.GetElementsByTagName("metadata");
            if (listOfMetadataContainers.Count != 0)
            {
                parseMetadata_ElementInnerText(listOfMetadataContainers);
            }

            listOfMetadataContainers = xmlDoc.GetElementsByTagName("dc-metadata");
            if (listOfMetadataContainers.Count != 0)
            {
                parseMetadata_ElementInnerText(listOfMetadataContainers);
            }

            listOfMetadataContainers = xmlDoc.GetElementsByTagName("x-metadata");
            if (listOfMetadataContainers.Count != 0)
            {
                parseMetadata_ElementInnerText(listOfMetadataContainers);
            }
        }

        private void parseMetadata_NameContentAll(XmlDocument xmlDoc)
        {
            XmlNodeList listOfMetaNodes = xmlDoc.GetElementsByTagName("meta");
            if (listOfMetaNodes.Count == 0)
            {
                return;
            }
            parseMetadata_NameContent(listOfMetaNodes);
        }

        private void parseMetadata_ElementInnerText(XmlNodeList listOfMetadataContainers)
        {
            if (listOfMetadataContainers == null || listOfMetadataContainers.Count == 0)
            {
                return;
            }

            foreach (XmlNode mdNodeRoot in listOfMetadataContainers)
            {
                foreach (XmlNode mdNode in mdNodeRoot.ChildNodes)
                {
                    if (mdNode.NodeType == XmlNodeType.Element
                        && mdNode.Name != "meta"
                        && mdNode.Name != "dc-metadata"
                        && mdNode.Name != "x-metadata"
                        && !String.IsNullOrEmpty(mdNode.InnerText))
                    {
                        XmlNode mdIdentifier = mdNode.Attributes.GetNamedItem("id");

                        handleMetaData(mdNode.Name, mdNode.InnerText, (mdIdentifier == null ? null : mdIdentifier.Value));
                    }
                }
            }
        }

        private void handleMetaData(string name, string content, string id)
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

                    Presentation presentation = m_Project.Presentations.Get(0);
                    foreach (Metadata md in presentation.Metadatas.ContentsAs_ListCopy)
                    {
                        if (isUniqueIdName(md.Name) && md.Content == m_PublicationUniqueIdentifier)
                        {
                            presentation.Metadatas.Remove(md);
                        }
                    }

                }
                else if (!metadataUidValueAlreadyExists(content)
                    && (String.IsNullOrEmpty(m_PublicationUniqueIdentifier) || content != m_PublicationUniqueIdentifier))
                {
                    addMetadata(name, content);
                }
            }
            else
            {
                MetadataDefinition md = SupportedMetadata_Z39862005.GetMetadataDefinition(name);
                if (md == null
                    || md.IsRepeatable
                    || !metadataNameAlreadyExists(name))
                {
                    addMetadata(name, content);
                }
            }
        }

        private static bool isUniqueIdName(string name)
        {
            if ("dc:Identifier" == name)
            {
                return true;
            }

            MetadataDefinition md = SupportedMetadata_Z39862005.GetMetadataDefinition("dc:Identifier");
            return md != null && md.Synonyms.Contains(name);
        }

        private void parseMetadata_NameContent(XmlNodeList listOfMetaDataNodes)
        {
            if (listOfMetaDataNodes == null || listOfMetaDataNodes.Count == 0)
            {
                return;
            }

            foreach (XmlNode mdNode in listOfMetaDataNodes)
            {
                if (mdNode.NodeType != XmlNodeType.Element || mdNode.Name != "meta")
                {
                    continue;
                }

                XmlAttributeCollection mdAttributes = mdNode.Attributes;

                if (mdAttributes == null || mdAttributes.Count <= 0)
                {
                    continue;
                }

                XmlNode attrName = mdAttributes.GetNamedItem("name");
                XmlNode attrContent = mdAttributes.GetNamedItem("content");

                if (attrName != null && !String.IsNullOrEmpty(attrName.Value)
                    && attrContent != null && !String.IsNullOrEmpty(attrContent.Value))
                {
                    XmlNode mdIdentifier = mdAttributes.GetNamedItem("id");

                    handleMetaData(attrName.Value, attrContent.Value, (mdIdentifier == null ? null : mdIdentifier.Value));
                }
            }
        }

        private void addMetadata(string name, string content)
        {
            Presentation presentation = m_Project.Presentations.Get(0);
            Metadata md = presentation.MetadataFactory.CreateMetadata();
            md.Name = name;
            md.Content = content;
            presentation.Metadatas.Insert(presentation.Metadatas.Count, md);
        }

        private bool metadataNameAlreadyExists(string metaDataName)
        {
            Presentation presentation = m_Project.Presentations.Get(0);
            foreach (Metadata md in presentation.Metadatas.ContentsAs_YieldEnumerable)
            {
                if (md.Name == metaDataName)
                {
                    return true;
                }
            }
            return false;
            //List<Metadata> metadataList = presentation.GetMetadata(metaDataName);
            //return (metadataList != null && metadataList.Count > 0);
        }

        private bool metadataUidValueAlreadyExists(string uid)
        {
            Presentation presentation = m_Project.Presentations.Get(0);
            foreach (Metadata md in presentation.Metadatas.ContentsAs_YieldEnumerable)
            {
                if (isUniqueIdName(md.Name) && md.Content == uid)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
