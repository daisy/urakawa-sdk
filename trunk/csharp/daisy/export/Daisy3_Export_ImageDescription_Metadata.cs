using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;
using System.IO;
using AudioLib;
using urakawa.data;
using urakawa.media.data.audio.codec;
using urakawa.metadata.daisy;
using urakawa.property.alt;
using urakawa.metadata;
using urakawa.xuk;

namespace urakawa.daisy.export
{
    public partial class Daisy3_Export
    {
        private static XmlNode addFlatDiagramHeadMetadata(
            MetadataAttribute metaAttr, IEnumerable<MetadataAttribute> metaAttrs,
            XmlNode parentNode, XmlDocument descriptionDocument, XmlNode descriptionNode)
        {
            XmlNode metaNode = descriptionDocument.CreateElement(
                    DiagramContentModelHelper.StripNSPrefix(DiagramContentModelHelper.Meta),
                    DiagramContentModelHelper.NS_URL_ZAI);
            parentNode.AppendChild(metaNode);

            if (metaAttr != null)
            {
                if (metaAttr.Name != DiagramContentModelHelper.NA)
                {
                    XmlDocumentHelper.CreateAppendXmlAttribute(descriptionDocument, metaNode,
                                                           DiagramContentModelHelper.Property,
                                                           metaAttr.Name,
                                                           DiagramContentModelHelper.NS_URL_ZAI);
                }

                if (metaAttr.Value != DiagramContentModelHelper.NA)
                {
                    // TODO: INNER_TEXT vs CONTENT_ATTR => is this specified anywhere?
                    if (
                        string.Equals(metaAttr.Name, DiagramContentModelHelper.DIAGRAM_Purpose, StringComparison.OrdinalIgnoreCase)
                        || string.Equals(metaAttr.Name, DiagramContentModelHelper.DIAGRAM_Credentials, StringComparison.OrdinalIgnoreCase)
                        || string.Equals(metaAttr.Name, SupportedMetadata_Z39862005.DC_AccessRights, StringComparison.OrdinalIgnoreCase)
                        //
                        || string.Equals(metaAttr.Name, SupportedMetadata_Z39862005.DC_Creator, StringComparison.OrdinalIgnoreCase)
                        || string.Equals(metaAttr.Name, SupportedMetadata_Z39862005.DC_Description, StringComparison.OrdinalIgnoreCase)
                        || string.Equals(metaAttr.Name, SupportedMetadata_Z39862005.DC_Title, StringComparison.OrdinalIgnoreCase)
                        || string.Equals(metaAttr.Name, SupportedMetadata_Z39862005.DC_Subject, StringComparison.OrdinalIgnoreCase)
                        )
                    {
                        metaNode.InnerText = metaAttr.Value;
                    }
                    else
                    {
                        XmlDocumentHelper.CreateAppendXmlAttribute(descriptionDocument, metaNode,
                                                           DiagramContentModelHelper.Content,
                                                           metaAttr.Value,
                                                           DiagramContentModelHelper.NS_URL_ZAI);
                    }
                }
            }

            if (metaAttrs != null)
            {
                foreach (MetadataAttribute metaAttribute in metaAttrs)
                {
                    if (metaAttribute.Name.StartsWith(XmlReaderWriterHelper.NS_PREFIX_XML + ":"))
                    {
                        XmlDocumentHelper.CreateAppendXmlAttribute(descriptionDocument, metaNode,
                            metaAttribute.Name,
                            metaAttribute.Value,
                            XmlReaderWriterHelper.NS_URL_XML);
                    }
                    else if (metaAttribute.Name.StartsWith(DiagramContentModelHelper.NS_PREFIX_ZAI + ":"))
                    {
                        XmlDocumentHelper.CreateAppendXmlAttribute(descriptionDocument, metaNode,
                            metaAttribute.Name,
                            metaAttribute.Value,
                            DiagramContentModelHelper.NS_URL_ZAI);
                    }
                    else if (metaAttribute.Name.StartsWith(DiagramContentModelHelper.NS_PREFIX_DIAGRAM + ":"))
                    {
                        XmlDocumentHelper.CreateAppendXmlAttribute(descriptionDocument, metaNode,
                            metaAttribute.Name,
                            metaAttribute.Value,
                            DiagramContentModelHelper.NS_URL_DIAGRAM);
                    }
                    else if (string.Equals(metaAttribute.Name, DiagramContentModelHelper.Rel, StringComparison.OrdinalIgnoreCase)
                        || string.Equals(metaAttribute.Name, DiagramContentModelHelper.Resource, StringComparison.OrdinalIgnoreCase)
                        || string.Equals(metaAttribute.Name, DiagramContentModelHelper.About, StringComparison.OrdinalIgnoreCase))
                    {
                        XmlDocumentHelper.CreateAppendXmlAttribute(descriptionDocument, metaNode,
                            metaAttribute.Name.ToLower(),
                            metaAttribute.Value,
                            DiagramContentModelHelper.NS_URL_ZAI);
                    }
                    else
                    {
                        XmlDocumentHelper.CreateAppendXmlAttribute(descriptionDocument, metaNode,
                            DiagramContentModelHelper.StripNSPrefix(metaAttribute.Name),
                            metaAttribute.Value,
                            descriptionNode.NamespaceURI);
                    }
                }
            }

            return metaNode;
        }


        private static void createDiagramHeadMetadata(XmlDocument descriptionDocument, XmlNode descriptionNode, AlternateContentProperty altProperty)
        {
            XmlNode headNode = descriptionDocument.CreateElement(
                DiagramContentModelHelper.NS_PREFIX_DIAGRAM,
                DiagramContentModelHelper.StripNSPrefix(DiagramContentModelHelper.D_Head),
                DiagramContentModelHelper.NS_URL_DIAGRAM);
            descriptionNode.AppendChild(headNode);

            //TODO: ALWAYS DISABLE THE DEBUG CODE BELOW UNLESS NEEDED FOR TESTING!!
#if false && DEBUG
            foreach (Metadata md in altProperty.Metadatas.ContentsAs_Enumerable)
            {
                addFlatDiagramHeadMetadata(
                    md.NameContentAttribute, md.OtherAttributes.ContentsAs_Enumerable,
                    headNode, descriptionDocument, descriptionNode);
            }
#endif // DEBUG

            List<Metadata> flatMetadatas = new List<Metadata>();
            Dictionary<string, List<Metadata>> groupedMetadata_Id = new Dictionary<string, List<Metadata>>();
            Dictionary<string, List<Metadata>> groupedMetadata_RelResource = new Dictionary<string, List<Metadata>>();

            foreach (Metadata md in altProperty.Metadatas.ContentsAs_Enumerable)
            {
                if (md.NameContentAttribute == null)
                {
#if DEBUG
                    Debugger.Break();
#endif // DEBUG
                    continue;
                }

                if (
                    md.NameContentAttribute.Name.StartsWith(XmlReaderWriterHelper.NS_PREFIX_XML + ":")

                    //&& (md.OtherAttributes == null || md.OtherAttributes.Count == 0)

                    && (descriptionNode.Attributes == null || descriptionNode.Attributes.GetNamedItem(md.NameContentAttribute.Name) == null))
                {
                    XmlDocumentHelper.CreateAppendXmlAttribute(descriptionDocument, descriptionNode,
                                                            md.NameContentAttribute.Name,
                                                            md.NameContentAttribute.Value,
                                                            XmlReaderWriterHelper.NS_URL_XML);
                }
                else
                {
                    if (md.OtherAttributes != null && md.OtherAttributes.Count > 0)
                    {
                        MetadataAttribute mdAttr_Rel = null;
                        MetadataAttribute mdAttr_Resource = null;
                        MetadataAttribute mdAttr_Id = null;
                        bool hasOtherAttrs = false;
                        foreach (MetadataAttribute mdAttr in md.OtherAttributes.ContentsAs_Enumerable)
                        {
                            if (mdAttr.Name == DiagramContentModelHelper.Rel)
                            {
                                mdAttr_Rel = mdAttr;
                                continue;
                            }
                            if (mdAttr.Name == DiagramContentModelHelper.Resource)
                            {
                                mdAttr_Resource = mdAttr;
                                continue;
                            }
                            if (mdAttr.Name == XmlReaderWriterHelper.XmlId)
                            {
                                mdAttr_Id = mdAttr;
                                continue;
                            }

                            hasOtherAttrs = true;
                        }

                        if (mdAttr_Id != null)
                        {
                            addDic_(groupedMetadata_Id, mdAttr_Id.Value, md);

                            continue;
                        }
                        else if (mdAttr_Rel != null || mdAttr_Resource != null)
                        {
                            string key = (mdAttr_Rel != null ? mdAttr_Rel.Value : "")
                                         + "_-_"
                                         + (mdAttr_Resource != null ? mdAttr_Resource.Value : "");
                            addDic_(groupedMetadata_RelResource, key, md);

                            continue;
                        }
                    }

                    //md.NameContentAttribute.Name != DiagramContentModelHelper.NA
                    //    && md.NameContentAttribute.Value != DiagramContentModelHelper.NA

                    flatMetadatas.Add(md);
                }
            }

            foreach (Metadata md in flatMetadatas)
            {
                addFlatDiagramHeadMetadata(
                    md.NameContentAttribute, md.OtherAttributes.ContentsAs_Enumerable,
                    headNode, descriptionDocument, descriptionNode);
            }

            handleMetadataGroup(headNode, descriptionDocument, descriptionNode, groupedMetadata_Id);
            handleMetadataGroup(headNode, descriptionDocument, descriptionNode, groupedMetadata_RelResource);
        }

        private static void handleMetadataGroup(
            XmlNode headNode, XmlDocument descriptionDocument, XmlNode descriptionNode,
            Dictionary<string, List<Metadata>> groupedMetadata)
        {
            foreach (string key in groupedMetadata.Keys)
            {
                List<Metadata> metadatasForKey = groupedMetadata[key];
                if (metadatasForKey.Count == 1)
                {
                    Metadata md = metadatasForKey[0];

                    string hasId = null;
                    string hasRel = null;
                    string hasResource = null;
                    List<MetadataAttribute> mains = new List<MetadataAttribute>();

                    List<MetadataAttribute> hasOthers = new List<MetadataAttribute>();

                    foreach (MetadataAttribute metadataAttribute in md.OtherAttributes.ContentsAs_Enumerable)
                    {
                        if (metadataAttribute.Name == XmlReaderWriterHelper.XmlId)
                        {
                            hasId = metadataAttribute.Value;
                            mains.Add(metadataAttribute);
                        }
                        else if (metadataAttribute.Name == DiagramContentModelHelper.Rel)
                        {
                            hasRel = metadataAttribute.Value;
                            mains.Add(metadataAttribute);
                        }
                        else if (metadataAttribute.Name == DiagramContentModelHelper.Resource)
                        {
                            hasResource = metadataAttribute.Value;
                            mains.Add(metadataAttribute);
                        }
                        else
                        {
                            hasOthers.Add(metadataAttribute);
                        }
                    }

                    if (hasRel != null && hasResource != null
                        && (hasOthers.Count > 0
                        || (md.NameContentAttribute != null
                        && md.NameContentAttribute.Name != DiagramContentModelHelper.NA && md.NameContentAttribute.Value != DiagramContentModelHelper.NA)))
                    {
                        XmlNode metaNode = addFlatDiagramHeadMetadata(
                        null, mains,
                        headNode, descriptionDocument, descriptionNode);

                        addFlatDiagramHeadMetadata(
                        md.NameContentAttribute, hasOthers,
                        metaNode, descriptionDocument, descriptionNode);
                    }
                    else
                    {
                        addFlatDiagramHeadMetadata(
                        metadatasForKey[0].NameContentAttribute, metadatasForKey[0].OtherAttributes.ContentsAs_Enumerable,
                        headNode, descriptionDocument, descriptionNode);
                    }
                }
                else
                {
                    bool idAdded = false;
                    string relAdded = null;
                    string resourceAdded = null;

                    List<MetadataAttribute> listCommonMetaAttributes = new List<MetadataAttribute>();
                    Dictionary<Metadata, List<MetadataAttribute>> mapMetaUpdatedAttributes = new Dictionary<Metadata, List<MetadataAttribute>>();

                    foreach (Metadata md in metadatasForKey)
                    {
                        if (md.OtherAttributes == null || md.OtherAttributes.Count == 0)
                        {
#if DEBUG
                            Debugger.Break();
#endif
                            // DEBUG
                            continue;
                        }

                        foreach (MetadataAttribute metadataAttribute in md.OtherAttributes.ContentsAs_Enumerable)
                        {
                            if (metadataAttribute.Name == XmlReaderWriterHelper.XmlId)
                            {
                                DebugFix.Assert(metadataAttribute.Value == key);

                                if (!idAdded)
                                {
                                    listCommonMetaAttributes.Add(metadataAttribute);
                                    idAdded = true;
                                }
                            }
                            else if (metadataAttribute.Name == DiagramContentModelHelper.Rel)
                            {
                                if (relAdded == null)
                                {
                                    listCommonMetaAttributes.Add(metadataAttribute);
                                    relAdded = metadataAttribute.Value;
                                }
                                else
                                {
                                    if (metadataAttribute.Value != relAdded)
                                    {
#if DEBUG
                                        Debugger.Break();
#endif
                                        addDic(mapMetaUpdatedAttributes, md, metadataAttribute);
                                    }
                                }
                            }
                            else if (metadataAttribute.Name == DiagramContentModelHelper.Resource)
                            {
                                if (resourceAdded == null)
                                {
                                    listCommonMetaAttributes.Add(metadataAttribute);
                                    resourceAdded = metadataAttribute.Value;
                                }
                                else
                                {
                                    if (metadataAttribute.Value != resourceAdded)
                                    {
#if DEBUG
                                        Debugger.Break();
#endif
                                        addDic(mapMetaUpdatedAttributes, md, metadataAttribute);
                                    }
                                }
                            }
                            else
                            {
                                addDic(mapMetaUpdatedAttributes, md, metadataAttribute);
                            }
                        }
                    }

                    XmlNode metaNode = addFlatDiagramHeadMetadata(
                        null, listCommonMetaAttributes,
                        headNode, descriptionDocument, descriptionNode);


                    foreach (Metadata md in metadatasForKey)
                    {
                        if (mapMetaUpdatedAttributes.ContainsKey(md))
                        {
                            addFlatDiagramHeadMetadata(
                            md.NameContentAttribute, mapMetaUpdatedAttributes[md],
                            metaNode, descriptionDocument, descriptionNode);
                        }
                        else
                        {
                            addFlatDiagramHeadMetadata(
                            md.NameContentAttribute, null,
                            metaNode, descriptionDocument, descriptionNode);
                        }
                    }
                }
            }
        }

        private static void addDic(Dictionary<Metadata, List<MetadataAttribute>> dic, Metadata md, MetadataAttribute mdAttr)
        {
            List<MetadataAttribute> list;
            dic.TryGetValue(md, out list);
            if (list != null)
            {
                list.Add(mdAttr);
            }
            else
            {
                list = new List<MetadataAttribute>();
                list.Add(mdAttr);
                dic.Add(md, list);
            }
        }
        private static void addDic_(Dictionary<string, List<Metadata>> dic, string key, Metadata md)
        {
            List<Metadata> list;
            dic.TryGetValue(key, out list);
            if (list != null)
            {
                list.Add(md);
            }
            else
            {
                list = new List<Metadata>();
                list.Add(md);
                dic.Add(key, list);
            }
        }
    }
}
