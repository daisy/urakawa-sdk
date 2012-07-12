using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml;
using AudioLib;
using urakawa.data;
using urakawa.metadata;
using urakawa.metadata.daisy;
using urakawa.xuk;

namespace urakawa.daisy.import
{
    public partial class Daisy3_Import
    {
        private List<string> m_MetadataItemsToExclude = new List<string>();
        protected virtual List<string> MetadataItemsToExclude { get { return m_MetadataItemsToExclude; } }

        private void parseMetadata(string book_FilePath, Project project, XmlDocument xmlDoc)
        {
            parseMetadata_NameContentAll(book_FilePath, project, xmlDoc);
            parseMetadata_ElementInnerTextAll(book_FilePath, project, xmlDoc);
            RemoveMetadataItemsToBeExcluded(project);
        }

        private void parseMetadata_ElementInnerTextAll(string book_FilePath, Project project, XmlDocument xmlDoc)
        {
            if (RequestCancellation) return;

            foreach (XmlNode node in XmlDocumentHelper.GetChildrenElementsOrSelfWithName(xmlDoc, true, "metadata", null, false))
            {
                if (RequestCancellation) return;
                parseMetadata_ElementInnerText(book_FilePath, project, node);
            }

            if (RequestCancellation) return;
            foreach (XmlNode node in XmlDocumentHelper.GetChildrenElementsOrSelfWithName(xmlDoc, true, "dc-metadata", null, false))
            {
                if (RequestCancellation) return;
                parseMetadata_ElementInnerText(book_FilePath, project, node);
            }

            if (RequestCancellation) return;
            foreach (XmlNode node in XmlDocumentHelper.GetChildrenElementsOrSelfWithName(xmlDoc, true, "x-metadata", null, false))
            {
                if (RequestCancellation) return;
                parseMetadata_ElementInnerText(book_FilePath, project, node);
            }
        }

        private void parseMetadata_NameContentAll(string book_FilePath, Project project, XmlDocument xmlDoc)
        {
            if (RequestCancellation) return;
            foreach (XmlNode node in XmlDocumentHelper.GetChildrenElementsOrSelfWithName(xmlDoc, true, "meta", null, false))
            {
                if (RequestCancellation) return;
                parseMetadata_NameContent(book_FilePath, project, node);
            }
        }

        private void parseMetadata_ElementInnerText(string book_FilePath, Project project, XmlNode metadataContainer)
        {
            foreach (XmlNode mdNode in metadataContainer.ChildNodes)
            {
                if (RequestCancellation) return;

                //string lowerCaseName = mdNode.Name.ToLower();
                //string lowerCaseLocalName = mdNode.LocalName.ToLower();

                if (mdNode.NodeType == XmlNodeType.Element
                    && !mdNode.LocalName.Equals("meta", StringComparison.OrdinalIgnoreCase)
                    && !mdNode.LocalName.Equals("dc-metadata", StringComparison.OrdinalIgnoreCase)
                    && !mdNode.LocalName.Equals("x-metadata", StringComparison.OrdinalIgnoreCase)
                    && !String.IsNullOrEmpty(mdNode.InnerText))
                {
                    XmlNode mdIdentifier = mdNode.Attributes.GetNamedItem("id");

                    handleMetaData(book_FilePath, project, mdNode, mdNode.Name, mdNode.InnerText, (mdIdentifier == null ? null : mdIdentifier.Value));
                }
            }
        }

        private void handleMetaDataOptionalAttrs(Metadata meta, XmlNode node)
        {
            if (RequestCancellation) return;

            if (meta == null)
            {
                return;
            }

            for (int i = 0; i < node.Attributes.Count; i++)
            {
                XmlAttribute attribute = node.Attributes[i];

                //string lowerCaseName = attribute.Name.ToLower();
                //string lowerCaseLocalName = attribute.LocalName.ToLower();

                if (attribute.LocalName.Equals("name", StringComparison.OrdinalIgnoreCase)
                    || attribute.LocalName.Equals("property", StringComparison.OrdinalIgnoreCase)
                    || attribute.LocalName.Equals("content", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                // We keep IDS in case they are referred to by "refines"
                //if (attribute.LocalName.Equals("id", StringComparison.OrdinalIgnoreCase)
                //    && node != m_PublicationUniqueIdentifierNode)
                //{
                //    continue;
                //}

                if (attribute.Name.StartsWith(XmlReaderWriterHelper.NS_PREFIX_XMLNS + ":", StringComparison.OrdinalIgnoreCase))
                {
                    //meta.NameContentAttribute.NamespaceUri = attribute.Value;
                }
                else if (attribute.Name.Equals(XmlReaderWriterHelper.NS_PREFIX_XMLNS, StringComparison.OrdinalIgnoreCase))
                {
                    //meta.OtherAttributes.NamespaceUri = attribute.Value;
                }
                else
                {
                    MetadataAttribute xmlAttr = new MetadataAttribute();

                    xmlAttr.Name = attribute.Name;

                    if (attribute.Name.IndexOf(':') >= 0) // attribute.Name.Contains(":")
                    {
                        xmlAttr.NamespaceUri = attribute.NamespaceURI;
                    }

                    xmlAttr.Value = attribute.Value;

                    meta.OtherAttributes.Insert(meta.OtherAttributes.Count, xmlAttr);
                }
            }
        }

        private void handleMetaData(string book_FilePath, Project project, XmlNode mdNode, string name, string content, string id)
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

                    Presentation presentation = project.Presentations.Get(0);
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
                else if (!metadataUidValueAlreadyExists(project, content)
                    && (String.IsNullOrEmpty(m_PublicationUniqueIdentifier) || content != m_PublicationUniqueIdentifier))
                {
                    Metadata meta = addMetadata(book_FilePath, project, name, content, mdNode);
                }
            }
            else
            {
                MetadataDefinition md = SupportedMetadata_Z39862005.DefinitionSet.GetMetadataDefinition(name);
                if (
                    (md == null && !metadataNameContentAlreadyExists(project, name, content))
                    || (md != null && md.IsRepeatable && !metadataNameContentAlreadyExists(project, name, content))
                    || (md != null && !md.IsRepeatable && !metadataNameAlreadyExists(project, name))
                    )
                {
                    Metadata meta = addMetadata(book_FilePath, project, name, content, mdNode);
                }
            }
        }

        private static bool isUniqueIdName(string name)
        {
            if (SupportedMetadata_Z39862005.DC_Identifier.Equals(name, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            MetadataDefinition md = SupportedMetadata_Z39862005.DefinitionSet.GetMetadataDefinition(SupportedMetadata_Z39862005.DC_Identifier);
            return md != null && md.Synonyms.Find(
                                delegate(string s)
                                {
                                    return s.Equals(name, StringComparison.OrdinalIgnoreCase);
                                }) != null;
        }

        private void parseMetadata_NameContent(string book_FilePath, Project project, XmlNode metaDataNode)
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
            XmlNode attrProperty = mdAttributes.GetNamedItem("property");

            XmlNode attrContent = mdAttributes.GetNamedItem("content");

            if ((
                attrName != null && !String.IsNullOrEmpty(attrName.Value)
                ||
                attrProperty != null && !String.IsNullOrEmpty(attrProperty.Value)
                )
                &&
                (
                attrContent != null && !String.IsNullOrEmpty(attrContent.Value)
                ||
                !String.IsNullOrEmpty(metaDataNode.InnerText)
                ))
            {
                XmlNode mdIdentifier = mdAttributes.GetNamedItem("id");

                handleMetaData(book_FilePath, project, metaDataNode,
                    attrName != null ? attrName.Value : attrProperty.Value,
                    attrContent != null ? attrContent.Value : metaDataNode.InnerText,
                    (mdIdentifier == null ? null : mdIdentifier.Value));
            }
        }

        private Metadata addMetadata(string book_FilePath, Project project, string name, string content, XmlNode node)
        {
            Presentation presentation = project.Presentations.Get(0);

            Metadata md = presentation.MetadataFactory.CreateMetadata();
            md.NameContentAttribute = new MetadataAttribute();
            md.NameContentAttribute.Name = name; //.ToLower();
            md.NameContentAttribute.Value = content;

            if (md.NameContentAttribute.Name.IndexOf(':') >= 0 // md.NameContentAttribute.Name.Contains(":")
                && node.Name.Equals(md.NameContentAttribute.Name, StringComparison.OrdinalIgnoreCase))
            {
                md.NameContentAttribute.NamespaceUri = node.NamespaceURI;
            }

            presentation.Metadatas.Insert(presentation.Metadatas.Count, md);

            handleMetaDataOptionalAttrs(md, node);

            if (name == SupportedMetadata_Z39862005.MATHML_XSLT_METADATA)
            {
                string styleSheetPath = Path.Combine(
                    Path.GetDirectoryName(book_FilePath),
                    content);

                if (File.Exists(styleSheetPath))
                {
                    string ext = Path.GetExtension(content);
                    if (DataProviderFactory.XSLT_EXTENSION.Equals(ext, StringComparison.OrdinalIgnoreCase)
                    || DataProviderFactory.XSL_EXTENSION.Equals(ext, StringComparison.OrdinalIgnoreCase))
                    {
                        ExternalFiles.ExternalFileData efd = presentation.ExternalFilesDataFactory.Create<ExternalFiles.XSLTExternalFileData>();
                        efd.InitializeWithData(styleSheetPath, SupportedMetadata_Z39862005.MATHML_XSLT_METADATA + content, true);
                    }
                }
            }
            return md;
        }

        private bool metadataNameContentAlreadyExists(Project project, string metaDataName, string metaDataContent)
        {
            //string lower = metaDataName.ToLower();

            Presentation presentation = project.Presentations.Get(0);
            foreach (Metadata md in presentation.Metadatas.ContentsAs_Enumerable)
            {
                if (md.NameContentAttribute.Name.Equals(metaDataName, StringComparison.OrdinalIgnoreCase)
                    && md.NameContentAttribute.Value == metaDataContent)
                {
                    return true;
                }
            }
            return false;
        }

        private bool metadataNameAlreadyExists(Project project, string metaDataName)
        {
            //string lower = metaDataName.ToLower();

            Presentation presentation = project.Presentations.Get(0);
            foreach (Metadata md in presentation.Metadatas.ContentsAs_Enumerable)
            {
                if (md.NameContentAttribute.Name.Equals(metaDataName, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        private bool metadataUidValueAlreadyExists(Project project, string uid)
        {
            Presentation presentation = project.Presentations.Get(0);

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
        private void metadataPostProcessing(string book_FilePath, Project project)
        {
            ensureCorrectMetadataIdentifier(book_FilePath, project);
            checkAllMetadataSynonyms(project);
            addMissingRequiredMetadata(project);
        }

        private void checkAllMetadataSynonyms(Project project)
        {
            foreach (MetadataDefinition metadataDefinition in SupportedMetadata_Z39862005.DefinitionSet.Definitions)
            {
                checkMetadataSynonyms(project, metadataDefinition);
            }
        }

        private void checkMetadataSynonyms(Project project, MetadataDefinition metadataDefinition)
        {
            //does this item exist?
            List<Metadata> primaryNameMetadata = findMetadataByName(project, metadataDefinition.Name);

            List<Metadata> allSynonyms = new List<Metadata>();

            if (metadataDefinition.Synonyms == null) return;
            foreach (string synonym in metadataDefinition.Synonyms)
            {
                List<Metadata> matchesForOneSynonym = findMetadataByName(project, synonym);
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
                        if (synonymMetadata.NameContentAttribute.Value.Equals(primaryMetadata.NameContentAttribute.Value, StringComparison.OrdinalIgnoreCase))
                        {
                            project.Presentations.Get(0).Metadatas.Remove(synonymMetadata);
                            break;
                        }
                    }
                }
            }

        }

        //find all metadata items with the given name (case-insensitive)
        //returns an empty list if not found
        private List<Metadata> findMetadataByName(Project project, string name)
        {
            IEnumerable<Metadata> metadatas =
                project.Presentations.Get(0).Metadatas.ContentsAs_Enumerable;

            IEnumerator<Metadata> enumerator = metadatas.GetEnumerator();
            List<Metadata> found = new List<Metadata>();
            while (enumerator.MoveNext())
            {
                if (enumerator.Current.NameContentAttribute.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    found.Add(enumerator.Current);
                }
            }
            return found;
        }

        private void ensureCorrectMetadataIdentifier(string book_FilePath, Project project)
        {
            if (!String.IsNullOrEmpty(m_PublicationUniqueIdentifier))
            {
                Metadata meta = addMetadata(book_FilePath, project, SupportedMetadata_Z39862005.DC_Identifier, m_PublicationUniqueIdentifier, m_PublicationUniqueIdentifierNode);
                meta.IsMarkedAsPrimaryIdentifier = true;
            }
            //if no unique publication identifier could be determined, see how many identifiers there are
            //if there is only one, then make that the unique publication identifier
            else
            {
                if (project.Presentations.Count > 0)
                {
                    List<Metadata> identifiers = new List<Metadata>();

                    foreach (Metadata md in project.Presentations.Get(0).Metadatas.ContentsAs_Enumerable)
                    {
                        //get this metadata's definition (and search synonyms too)
                        MetadataDefinition definition = SupportedMetadata_Z39862005.DefinitionSet.GetMetadataDefinition(
                            md.NameContentAttribute.Name, true);

                        //if this is a dc:identifier, then add it to our list
                        if (SupportedMetadata_Z39862005.DC_Identifier.Equals(definition.Name, StringComparison.OrdinalIgnoreCase))
                        {
                            identifiers.Add(md);
                        }
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
        private void addMissingRequiredMetadata(Project project)
        {
            Presentation presentation = project.Presentations.Get(0);
            //add any missing required metadata entries
            IEnumerable<Metadata> metadatas = presentation.Metadatas.ContentsAs_Enumerable;
            foreach (MetadataDefinition metadataDefinition in SupportedMetadata_Z39862005.DefinitionSet.Definitions)
            {
                if (!metadataDefinition.IsReadOnly && metadataDefinition.Occurrence == MetadataOccurrence.Required)
                {
                    bool found = false;
                    foreach (Metadata m in metadatas)
                    {
                        if (m.NameContentAttribute.Name.Equals(metadataDefinition.Name, StringComparison.OrdinalIgnoreCase))
                        {
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                    {
                        Metadata metadata = presentation.MetadataFactory.CreateMetadata();
                        metadata.NameContentAttribute = new MetadataAttribute();
                        metadata.NameContentAttribute.Name = metadataDefinition.Name;
                        metadata.NameContentAttribute.Value = SupportedMetadata_Z39862005.MagicStringEmpty;
                        presentation.Metadatas.Insert(presentation.Metadatas.Count, metadata);
                    }

                }
            }
        }

        protected virtual void RemoveMetadataItemsToBeExcluded(Project project)
        {
            if (MetadataItemsToExclude == null || MetadataItemsToExclude.Count == 0) return;

            Presentation pres = project.Presentations.Get(0);
            foreach (metadata.Metadata m in pres.Metadatas.ContentsAs_ListCopy)
            {
                if (m != null && MetadataItemsToExclude.Contains(m.NameContentAttribute.Name))
                {
                    //Console.WriteLine("removing metadata " + m.NameContentAttribute.Name);
                    pres.Metadatas.Remove(m);
                }
            }
        }


    }
}
