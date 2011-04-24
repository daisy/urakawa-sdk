using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;
using AudioLib;
using urakawa.metadata;
using urakawa.metadata.daisy;

namespace urakawa.daisy.import
{
    public partial class Daisy3_Import
    {
        private List<string> m_MetadataItemsToExclude = new List<string>();
        protected virtual List<string> MetadataItemsToExclude { get { return m_MetadataItemsToExclude; } }

        private void parseMetadata(XmlDocument xmlDoc)
        {
            parseMetadata_NameContentAll(xmlDoc);
            parseMetadata_ElementInnerTextAll(xmlDoc);
            RemoveMetadataItemsToBeExcluded();
        }

        private void parseMetadata_ElementInnerTextAll(XmlDocument xmlDoc)
        {
            if (RequestCancellation) return;

            foreach (XmlNode node in XmlDocumentHelper.GetChildrenElementsWithName(xmlDoc, true, "metadata", null, false))
            {
                if (RequestCancellation) return;
                parseMetadata_ElementInnerText(node);
            }

            if (RequestCancellation) return;
            foreach (XmlNode node in XmlDocumentHelper.GetChildrenElementsWithName(xmlDoc, true, "dc-metadata", null, false))
            {
                if (RequestCancellation) return;
                parseMetadata_ElementInnerText(node);
            }

            if (RequestCancellation) return;
            foreach (XmlNode node in XmlDocumentHelper.GetChildrenElementsWithName(xmlDoc, true, "x-metadata", null, false))
            {
                if (RequestCancellation) return;
                parseMetadata_ElementInnerText(node);
            }
        }

        private void parseMetadata_NameContentAll(XmlDocument xmlDoc)
        {
            if (RequestCancellation) return;
            foreach (XmlNode node in XmlDocumentHelper.GetChildrenElementsWithName(xmlDoc, true, "meta", null, false))
            {
                if (RequestCancellation) return;
                parseMetadata_NameContent(node);
            }
        }

        private void parseMetadata_ElementInnerText(XmlNode metadataContainer)
        {
            foreach (XmlNode mdNode in metadataContainer.ChildNodes)
            {
                if (RequestCancellation) return;

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
            if (RequestCancellation) return;

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
            if (RequestCancellation) return;

            if (isUniqueIdName(name))
            {
                if (m_PackageUniqueIdAttr != null
                    && id != null
                    && id == m_PackageUniqueIdAttr.Value)
                {
                    DebugFix.Assert(String.IsNullOrEmpty(m_PublicationUniqueIdentifier));
                    //String.Format("The Publication's Unique Identifier is specified several times !! OLD: [{0}], NEW: [{1}]", m_PublicationUniqueIdentifier, content)

                    m_PublicationUniqueIdentifier = content;
                    m_PublicationUniqueIdentifierNode = mdNode;

                    Presentation presentation = m_Project.Presentations.Get(0);
                    foreach (Metadata md in presentation.Metadatas.ContentsAs_ListCopy)
                    {
                        if (RequestCancellation) return;

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
                MetadataDefinition md = SupportedMetadata_Z39862005.DefinitionSet.GetMetadataDefinition(name);
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

            MetadataDefinition md = SupportedMetadata_Z39862005.DefinitionSet.GetMetadataDefinition("dc:Identifier");
            return md != null && md.Synonyms.Find(
                                delegate(string s)
                                {
                                    return s.ToLower() == lower;
                                }) != null;
        }

        private void parseMetadata_NameContent(XmlNode metaDataNode)
        {
            if (RequestCancellation) return;

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
            foreach (Metadata md in presentation.Metadatas.ContentsAs_Enumerable)
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
            foreach (Metadata md in presentation.Metadatas.ContentsAs_Enumerable)
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

            foreach (Metadata md in presentation.Metadatas.ContentsAs_Enumerable)
            {
                if (isUniqueIdName(md.NameContentAttribute.Name)
                    && md.NameContentAttribute.Value == uid)
                {
                    return true;
                }
            }
            return false;
        }


        //after import is complete, process metadata to ensure lack of redundancy 
        //and also see that all required items are present
        private void metadataPostProcessing()
        {
            ensureCorrectMetadataIdentifier();
            checkAllMetadataSynonyms();
            addMissingRequiredMetadata();
        }

        private void checkAllMetadataSynonyms()
        {
            foreach (MetadataDefinition metadataDefinition in SupportedMetadata_Z39862005.DefinitionSet.Definitions)
            {
                checkMetadataSynonyms(metadataDefinition);
            }
        }

        private void checkMetadataSynonyms(MetadataDefinition metadataDefinition)
        {
            //does this item exist?
            List<Metadata> primaryNameMetadata = findMetadataByName(metadataDefinition.Name);
            
            List<Metadata> allSynonyms = new List<Metadata>();

            if (metadataDefinition.Synonyms == null) return;
            foreach (string synonym in metadataDefinition.Synonyms)
            {
                List<Metadata> matchesForOneSynonym = findMetadataByName(synonym);
                if (matchesForOneSynonym.Count > 0)
                    allSynonyms.AddRange(matchesForOneSynonym);

            }

            //if there is no direct match for metadataDefinition but at least one synonym exists?
            if (primaryNameMetadata.Count == 0)
            {
                if (allSynonyms.Count >= 1)
                {
                    //promote the first synonym
                    allSynonyms[0].NameContentAttribute.Name = metadataDefinition.Name;
                }
            }
            else
            {
                //delete synonyms with identical values to the primary metadata name
                foreach (Metadata synonymMetadata in allSynonyms)
                {
                    foreach (Metadata primaryMetadata in primaryNameMetadata)
                    {
                        if (synonymMetadata.NameContentAttribute.Value.ToLower() ==
                            primaryMetadata.NameContentAttribute.Value.ToLower())
                        {
                            m_Project.Presentations.Get(0).Metadatas.Remove(synonymMetadata);
                            break;
                        }
                    }
                }
            }
            
        }

        //find all metadata items with the given name (case-insensitive)
        //returns an empty list if not found
        private List<Metadata> findMetadataByName(string name)
        {
            IEnumerable<Metadata> metadatas =
                m_Project.Presentations.Get(0).Metadatas.ContentsAs_Enumerable;
            
            IEnumerator<Metadata> enumerator = metadatas.GetEnumerator();
            List<Metadata> found = new List<Metadata>();
            while(enumerator.MoveNext())
            {
                if (enumerator.Current.NameContentAttribute.Name.ToLower() == name.ToLower())
                {
                    found.Add(enumerator.Current);
                }
            }
            return found;
        }

        private void ensureCorrectMetadataIdentifier()
        {
            if (!String.IsNullOrEmpty(m_PublicationUniqueIdentifier))
            {
                Metadata meta = addMetadata("dc:Identifier", m_PublicationUniqueIdentifier, m_PublicationUniqueIdentifierNode);
                meta.IsMarkedAsPrimaryIdentifier = true;
            }
            //if no unique publication identifier could be determined, see how many identifiers there are
            //if there is only one, then make that the unique publication identifier
            else
            {
                if (m_Project.Presentations.Count > 0)
                {
                    List<Metadata> identifiers = new List<Metadata>();

                    foreach (Metadata md in m_Project.Presentations.Get(0).Metadatas.ContentsAs_Enumerable)
                    {
                        //get this metadata's definition (and search synonyms too)
                        MetadataDefinition definition = SupportedMetadata_Z39862005.DefinitionSet.GetMetadataDefinition(
                            md.NameContentAttribute.Name, true);

                        //if this is a dc:identifier, then add it to our list
                        if (definition.Name == "dc:Identifier") identifiers.Add(md);
                    }

                    //if there is only one identifier, then make it the publication UID
                    if (identifiers.Count == 1)
                    {
                        identifiers[0].IsMarkedAsPrimaryIdentifier = true;

                        //if dtb:uid is our only identifier, rename it dc:identifier
                        /*if (identifiers[0].NameContentAttribute.Name == "dtb:uid")
                            identifiers[0].NameContentAttribute.Name = "dc:Identifier";*/
                    }
                }
            }


        }
        private void addMissingRequiredMetadata()
        {
            //add any missing required metadata entries
            IEnumerable<Metadata> metadatas =
                m_Project.Presentations.Get(0).Metadatas.ContentsAs_Enumerable;
            foreach (MetadataDefinition metadataDefinition in SupportedMetadata_Z39862005.DefinitionSet.Definitions)
            {
                if (!metadataDefinition.IsReadOnly && metadataDefinition.Occurrence == MetadataOccurrence.Required)
                {
                    bool found = false;
                    foreach (Metadata m in metadatas)
                    {
                        if (m.NameContentAttribute.Name.ToLower() == metadataDefinition.Name.ToLower())
                        {
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                    {
                        Metadata metadata = m_Project.Presentations.Get(0).MetadataFactory.CreateMetadata();
                        metadata.NameContentAttribute = new MetadataAttribute();
                        metadata.NameContentAttribute.Name = metadataDefinition.Name;
                        metadata.NameContentAttribute.Value = SupportedMetadata_Z39862005.MagicStringEmpty;
                        m_Project.Presentations.Get(0).Metadatas.Insert
                            (m_Project.Presentations.Get(0).Metadatas.Count, metadata);
                    }

                }
            }
        }

        private void RemoveMetadataItemsToBeExcluded()
        {
            if (MetadataItemsToExclude == null || MetadataItemsToExclude.Count == 0) return;
            Presentation pres = m_Project.Presentations.Get(0);
            foreach (metadata.Metadata m in pres.Metadatas.ContentsAs_ListCopy)
            {
                if (m != null && MetadataItemsToExclude.Contains(m.NameContentAttribute.Name))
                {
                    Console.WriteLine("removing metadata " + m.NameContentAttribute.Name);
                    pres.Metadatas.Remove(m);
                }
            }
        }
    }
}
