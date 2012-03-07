using System;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using urakawa.data;
using urakawa.media.data.audio.codec;
using urakawa.property.alt;
using urakawa.metadata;
using urakawa.xuk;

namespace urakawa.daisy.export
{
    public partial class Daisy3_Export
    {
        private void createDiagramHeadMetadata(XmlNode headNode, XmlDocument descriptionDocument, XmlNode descriptionNode, AlternateContentProperty altProperty)
        {
            foreach (Metadata md in altProperty.Metadatas.ContentsAs_Enumerable)
            {
                if (md.NameContentAttribute != null && md.NameContentAttribute.Name.StartsWith(DiagramContentModelHelper.NS_PREFIX_XML + ":")
                    && (md.OtherAttributes == null || md.OtherAttributes.Count == 0)
                    && (descriptionNode.Attributes == null || descriptionNode.Attributes.GetNamedItem(md.NameContentAttribute.Name) == null))
                {
                    XmlDocumentHelper.CreateAppendXmlAttribute(descriptionDocument, descriptionNode,
                md.NameContentAttribute.Name,
                md.NameContentAttribute.Value,
                DiagramContentModelHelper.NS_URL_XML);
                }
                else
                {
                    XmlNode metaNode = descriptionDocument.CreateElement(
                DiagramContentModelHelper.StripNSPrefix(DiagramContentModelHelper.Meta),
                        DiagramContentModelHelper.NS_URL_ZAI);
                    headNode.AppendChild(metaNode);

                    if (md.NameContentAttribute != null)
                    {
                        handleMetadataAttr(false, md.NameContentAttribute, descriptionDocument, metaNode, false);
                    }

                    if (md.OtherAttributes != null)
                    {
                        foreach (MetadataAttribute metaAttr in md.OtherAttributes.ContentsAs_Enumerable)
                        {
                            if (metaAttr.Name.StartsWith(DiagramContentModelHelper.NS_PREFIX_XML + ":"))
                            {
                                handleMetadataAttr(true, metaAttr, descriptionDocument, metaNode, false);
                            }
                            else if (metaAttr.Name == DiagramContentModelHelper.Rel
                                     || metaAttr.Name == DiagramContentModelHelper.Resource
                                     || metaAttr.Name == DiagramContentModelHelper.About)
                            {
                                handleMetadataAttr(true, metaAttr, descriptionDocument, metaNode, true);
                            }
                            else
                            {
                                XmlNode metaSubNode = descriptionDocument.CreateElement(
                                    DiagramContentModelHelper.StripNSPrefix(DiagramContentModelHelper.Meta),
                                    DiagramContentModelHelper.NS_URL_ZAI);
                                metaNode.AppendChild(metaSubNode);
                                handleMetadataAttr(true, metaAttr, descriptionDocument, metaSubNode, false);
                            }
                        }
                    }
                }
            }
        }

        private void handleMetadataAttr(bool optionalAttr, MetadataAttribute mdAttr, XmlDocument descDocument, XmlNode metaNode, bool checkSpecialAttributesNames)
        {
            if (mdAttr.Name.StartsWith(DiagramContentModelHelper.NS_PREFIX_XML + ":"))
            {
                XmlDocumentHelper.CreateAppendXmlAttribute(descDocument, metaNode,
                    mdAttr.Name,
                    mdAttr.Value,
                    DiagramContentModelHelper.NS_URL_XML);
            }
            else
            {
                if (checkSpecialAttributesNames
                    && (mdAttr.Name == DiagramContentModelHelper.Rel
                              || mdAttr.Name == DiagramContentModelHelper.Resource
                              || mdAttr.Name == DiagramContentModelHelper.About))
                {
                    XmlDocumentHelper.CreateAppendXmlAttribute(descDocument, metaNode,
                        mdAttr.Name,
                        mdAttr.Value,
                        DiagramContentModelHelper.NS_URL_ZAI);
                }
                else
                {
                    XmlDocumentHelper.CreateAppendXmlAttribute(descDocument, metaNode,
                                                               DiagramContentModelHelper.Property,
                                                               mdAttr.Name,
                                                               DiagramContentModelHelper.NS_URL_ZAI);

                    // TODO: sometimes we should append a text child instead of an attribute
                    // (diagram:purpose, dc:creator, diagram:credentials, dc:accessRights, dc:description, diagram:queryConcept)
                    XmlDocumentHelper.CreateAppendXmlAttribute(descDocument, metaNode,
                                                               DiagramContentModelHelper.Content,
                                                               mdAttr.Value,
                                                               DiagramContentModelHelper.NS_URL_ZAI);
                }
            }
        }
    }
}
