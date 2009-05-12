using System;
using System.Xml;
using urakawa;
using urakawa.metadata;

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
            Presentation presentation = m_Project.GetPresentation(0);

            foreach (XmlNode mdNodeRoot in listOfMetadataContainers)
            {
                foreach (XmlNode mdNode in mdNodeRoot.ChildNodes)
                {
                    if (mdNode.NodeType == XmlNodeType.Element
                        && mdNode.Name != "meta" && !String.IsNullOrEmpty(mdNode.InnerText))
                    {
                        Metadata md = presentation.MetadataFactory.CreateMetadata();
                        md.Name = mdNode.Name;
                        md.Content = mdNode.InnerText;
                        presentation.AddMetadata(md);
                    }
                }
            }
        }

        private void parseMetadata_NameContent(XmlNodeList listOfMetaDataNodes)
        {
            if (listOfMetaDataNodes == null || listOfMetaDataNodes.Count == 0)
            {
                return;
            }

            Presentation presentation = m_Project.GetPresentation(0);

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
                    Metadata md = presentation.MetadataFactory.CreateMetadata();
                    md.Name = attrName.Value;
                    md.Content = attrContent.Value;
                    presentation.AddMetadata(md);
                }
            }
        }
        /*
         * 

        private void parseMetadata_X(XmlDocument xmlDoc)
        {
            XmlNodeList listOfMetaDataRootNodes = xmlDoc.GetElementsByTagName("x-metadata");
            if (listOfMetaDataRootNodes.Count == 0)
            {
                return;
            }
            foreach (XmlNode mdNodeRoot in listOfMetaDataRootNodes)
            {
                XmlNodeList listOfMetaDataNodes = mdNodeRoot.ChildNodes;

                parseMetadata_NameContent(listOfMetaDataNodes);
            }
        }

         */
    }
}
