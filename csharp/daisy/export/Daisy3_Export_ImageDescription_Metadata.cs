using System;
using System.Collections.Generic;
using System.Xml;
using System.IO;
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
        private void createDiagramHeadMetadata(XmlNode headNode, XmlDocument descriptionDocument, XmlNode descriptionNode, AlternateContentProperty altProperty)
        {
            //XmlNode metaSubNode = descriptionDocument.CreateElement(
            //    DiagramContentModelHelper.StripNSPrefix(DiagramContentModelHelper.Meta),
            //    DiagramContentModelHelper.NS_URL_ZAI);
            //metaNode.AppendChild(metaSubNode);

            foreach (Metadata md in altProperty.Metadatas.ContentsAs_Enumerable)
            {
                if (md.NameContentAttribute != null
                    && md.NameContentAttribute.Name.StartsWith(XmlReaderWriterHelper.NS_PREFIX_XML + ":")

                    && (md.OtherAttributes == null || md.OtherAttributes.Count == 0)

                    && (descriptionNode.Attributes == null || descriptionNode.Attributes.GetNamedItem(md.NameContentAttribute.Name) == null))
                {
                    XmlDocumentHelper.CreateAppendXmlAttribute(descriptionDocument, descriptionNode,
                                                            md.NameContentAttribute.Name,
                                                            md.NameContentAttribute.Value,
                                                            XmlReaderWriterHelper.NS_URL_XML);
                }
                else
                {
                    XmlNode metaNode = descriptionDocument.CreateElement(
                            DiagramContentModelHelper.StripNSPrefix(DiagramContentModelHelper.Meta),
                            DiagramContentModelHelper.NS_URL_ZAI);
                    headNode.AppendChild(metaNode);

                    if (md.NameContentAttribute != null)
                    {
                        if (md.NameContentAttribute.Name != DiagramContentModelHelper.NA)
                        {
                            XmlDocumentHelper.CreateAppendXmlAttribute(descriptionDocument, metaNode,
                                                                   DiagramContentModelHelper.Property,
                                                                   md.NameContentAttribute.Name,
                                                                   DiagramContentModelHelper.NS_URL_ZAI);
                        }

                        if (md.NameContentAttribute.Value != DiagramContentModelHelper.NA)
                        {
                            // TODO: INNER_TEXT vs CONTENT_ATTR => is this specified anywhere?
                            if ((md.OtherAttributes == null || md.OtherAttributes.Count == 0)
                                && (
                                string.Equals(md.NameContentAttribute.Name, DiagramContentModelHelper.DIAGRAM_Purpose, StringComparison.OrdinalIgnoreCase)
                                || string.Equals(md.NameContentAttribute.Name, DiagramContentModelHelper.DIAGRAM_Credentials, StringComparison.OrdinalIgnoreCase)
                                || string.Equals(md.NameContentAttribute.Name, SupportedMetadata_Z39862005.DC_AccessRights, StringComparison.OrdinalIgnoreCase)
                                //
                                || string.Equals(md.NameContentAttribute.Name, SupportedMetadata_Z39862005.DC_Creator, StringComparison.OrdinalIgnoreCase)
                                || string.Equals(md.NameContentAttribute.Name, SupportedMetadata_Z39862005.DC_Description, StringComparison.OrdinalIgnoreCase)
                                || string.Equals(md.NameContentAttribute.Name, SupportedMetadata_Z39862005.DC_Title, StringComparison.OrdinalIgnoreCase)
                                || string.Equals(md.NameContentAttribute.Name, SupportedMetadata_Z39862005.DC_Subject, StringComparison.OrdinalIgnoreCase)
                                ))
                            {
                                metaNode.InnerText = md.NameContentAttribute.Value;
                            }
                            else
                            {
                                XmlDocumentHelper.CreateAppendXmlAttribute(descriptionDocument, metaNode,
                                                                   DiagramContentModelHelper.Content,
                                                                   md.NameContentAttribute.Value,
                                                                   DiagramContentModelHelper.NS_URL_ZAI);
                            }
                        }
                    }

                    if (md.OtherAttributes != null)
                    {
                        foreach (MetadataAttribute metaAttr in md.OtherAttributes.ContentsAs_Enumerable)
                        {
                            if (metaAttr.Name.StartsWith(XmlReaderWriterHelper.NS_PREFIX_XML + ":"))
                            {
                                XmlDocumentHelper.CreateAppendXmlAttribute(descriptionDocument, metaNode,
                                    metaAttr.Name,
                                    metaAttr.Value,
                                    XmlReaderWriterHelper.NS_URL_XML);
                            }
                            else if (metaAttr.Name.StartsWith(DiagramContentModelHelper.NS_PREFIX_ZAI + ":"))
                            {
                                XmlDocumentHelper.CreateAppendXmlAttribute(descriptionDocument, metaNode,
                                    metaAttr.Name,
                                    metaAttr.Value,
                                    DiagramContentModelHelper.NS_URL_ZAI);
                            }
                            else if (metaAttr.Name.StartsWith(DiagramContentModelHelper.NS_PREFIX_DIAGRAM + ":"))
                            {
                                XmlDocumentHelper.CreateAppendXmlAttribute(descriptionDocument, metaNode,
                                    metaAttr.Name,
                                    metaAttr.Value,
                                    DiagramContentModelHelper.NS_URL_DIAGRAM);
                            }
                            else if (string.Equals(metaAttr.Name, DiagramContentModelHelper.Rel, StringComparison.OrdinalIgnoreCase)
                                || string.Equals(metaAttr.Name, DiagramContentModelHelper.Resource, StringComparison.OrdinalIgnoreCase)
                                || string.Equals(metaAttr.Name, DiagramContentModelHelper.About, StringComparison.OrdinalIgnoreCase))
                            {
                                XmlDocumentHelper.CreateAppendXmlAttribute(descriptionDocument, metaNode,
                                    metaAttr.Name.ToLower(),
                                    metaAttr.Value,
                                    DiagramContentModelHelper.NS_URL_ZAI);
                            }
                            else
                            {
                                XmlDocumentHelper.CreateAppendXmlAttribute(descriptionDocument, metaNode,
                                    DiagramContentModelHelper.StripNSPrefix(metaAttr.Name),
                                    metaAttr.Value,
                                    descriptionNode.NamespaceURI);
                            }
                        }
                    }
                }
            }
        }
    }
}
