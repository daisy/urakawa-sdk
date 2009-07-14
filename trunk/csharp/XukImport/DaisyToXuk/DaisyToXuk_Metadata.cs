using System;
using System.Xml;
using urakawa;
using urakawa.metadata;
using System.Collections.Generic;

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
                        && mdNode.Name != "meta" && !String.IsNullOrEmpty(mdNode.InnerText))
                    {
                        if (m_MetadataDictionary.ContainsKey(mdNode.Name) && !m_MetadataDictionary[mdNode.Name].IsRepeatable)
                        {
                            if (checkDuplicacy(mdNode.Name))
                            {
                                //ignore
                            }
                            else
                            {
                              addMetadata(mdNode.Name, mdNode.InnerText);
                            }      
                        }
                        else 
                        {
                           addMetadata(mdNode.Name, mdNode.InnerText);
                        }                    
                    }
                }
            }
        }

        private void addMetadata(string nodeName, string nodeContent)
        {
            Presentation presentation = m_Project.GetPresentation(0);
            Metadata md = presentation.MetadataFactory.CreateMetadata();
            md.Name = nodeName;
            md.Content = nodeContent;
            presentation.AddMetadata(md);
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
                        if (m_MetadataDictionary.ContainsKey(attrName.Value) && !m_MetadataDictionary[attrName.Value].IsRepeatable)
                        {
                            if (checkDuplicacy(attrName.Value))
                            {
                                // ignore
                            }
                            else
                            {
                                addNameContent(attrName.Value, attrContent.Value);
                            }
                        }
                        else
                        {
                            addNameContent(attrName.Value, attrContent.Value);
                        }
                     }  
            }
        }

        private void addNameContent(string nodeName, string contentName)
        {
            Presentation presentation = m_Project.GetPresentation(0);
            Metadata md = presentation.MetadataFactory.CreateMetadata();
            md.Name = nodeName;
            md.Content = contentName;
            presentation.AddMetadata(md);
        }

        private bool IsDuplicateMetadata(string metaDataName)
        {
            Presentation presentation = m_Project.ListOfPresentations[0];
            List<Metadata> metadataList = presentation.GetMetadata(metaDataName);

            if (metadataList != null && metadataList.Count > 0)
               return true;
            else
               return false;
        }

        public bool checkDuplicacy(string metadataName)
        {
            if (metadataName == "dtb:uid")
            {
               if (IsDuplicateMetadata("dc:identifier") || IsDuplicateMetadata(metadataName))
                    {
                       return true;
                    }
                else 
                return false;
            }
            else
                return IsDuplicateMetadata(metadataName);
        }   
                        
         
        /*    private void parseMetadata_X(XmlDocument xmlDoc)
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
