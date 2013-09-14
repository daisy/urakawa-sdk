using System;
using System.Xml;
using urakawa.metadata;
using urakawa.xuk;
using urakawa.progress;

namespace urakawa.property.alt
{
    [XukNameUglyPrettyAttribute("acP", "AlternateContentProperty")]
    public class AlternateContentProperty : Property
    {
        public new AlternateContentProperty Copy()
        {
            return CopyProtected() as AlternateContentProperty;
        }

        protected virtual Property CopyProtected()
        {
            AlternateContentProperty theCopy = base.Copy() as AlternateContentProperty;
            if (theCopy == null)
            {
                throw new exception.OperationNotValidException("The CopyProtected method of the base class unexpectedly did not return a AlternateContentProperty");
            }

            foreach (Metadata md in m_Metadata.ContentsAs_Enumerable)
            {
                theCopy.m_Metadata.Insert(theCopy.m_Metadata.Count, md.Copy());
            }

            foreach (AlternateContent ac in m_AlternateContents.ContentsAs_Enumerable)
            {
                theCopy.m_AlternateContents.Insert(theCopy.m_AlternateContents.Count, ac.Copy());
            }

            return theCopy;
        }

        public new AlternateContentProperty Export(Presentation destPres)
        {
            return ExportProtected(destPres) as AlternateContentProperty;
        }

        protected override Property ExportProtected(Presentation destPres)
        {
            AlternateContentProperty exported = base.ExportProtected(destPres) as AlternateContentProperty;
            if (exported == null)
            {
                throw new exception.OperationNotValidException(
                    "The ExportProtected method of the base class unexpectedly did not return a AlternateContentProperty");
            }
            
            foreach (Metadata md in m_Metadata.ContentsAs_Enumerable)
            {
                exported.m_Metadata.Insert(exported.m_Metadata.Count, md.Export(destPres));
            }

            foreach (AlternateContent ac in m_AlternateContents.ContentsAs_Enumerable)
            {
                exported.m_AlternateContents.Insert(exported.m_AlternateContents.Count, ac.Export(destPres));
            }


            return exported;
        }

        public AlternateContentProperty()
        {
            m_AlternateContents = new ObjectListProvider<AlternateContent>(this, true);
            m_Metadata = new ObjectListProvider<Metadata>(this, true);
        }

        public bool IsEmpty
        {
            get
            {
                if (m_Metadata.Count == 0)
                {
                    if (m_AlternateContents.Count == 0) return true;

                    foreach (AlternateContent altContent in m_AlternateContents.ContentsAs_Enumerable)
                    {
                        if (!altContent.IsEmpty) return false;
                    }

                    return true;
                }

                return false;
            }
        }

        private ObjectListProvider<Metadata> m_Metadata;
        public ObjectListProvider<Metadata> Metadatas
        {
            get
            {
                return m_Metadata;
            }
        }

        private ObjectListProvider<AlternateContent> m_AlternateContents;
        public ObjectListProvider<AlternateContent> AlternateContents
        {
            get
            {
                return m_AlternateContents;
            }
        }

        protected override void XukInAttributes(XmlReader source)
        {
            //nothing new here
            base.XukInAttributes(source);
        }

        protected override void XukOutAttributes(XmlWriter destination, Uri baseUri)
        {
            //nothing new here
            base.XukOutAttributes(destination, baseUri);
        }

        protected override void XukOutChildren(XmlWriter destination, Uri baseUri, IProgressHandler handler)
        {
            base.XukOutChildren(destination, baseUri, handler);

            destination.WriteStartElement(XukStrings.Metadatas, XukAble.XUK_NS);
            foreach (Metadata md in m_Metadata.ContentsAs_Enumerable)
            {
                md.XukOut(destination, baseUri, handler);
            }
            destination.WriteEndElement();

            destination.WriteStartElement(XukStrings.AlternateContents, XukAble.XUK_NS);
            foreach (AlternateContent ac in m_AlternateContents.ContentsAs_Enumerable)
            {
                ac.XukOut(destination, baseUri, handler);
            }
            destination.WriteEndElement();
        }

        private void XukInMetadata(XmlReader source, IProgressHandler handler)
        {
            if (source.IsEmptyElement) return;
            while (source.Read())
            {
                if (source.NodeType == XmlNodeType.Element)
                {
                    if (source.NamespaceURI == XukAble.XUK_NS
                        && XukAble.GetXukName(typeof(Metadata)).Match(source.LocalName))
                    {
                        Metadata md = Presentation.MetadataFactory.CreateMetadata();
                        md.XukIn(source, handler);
                        m_Metadata.Insert(m_Metadata.Count, md);
                    }
                }
                else if (source.NodeType == XmlNodeType.EndElement)
                {
                    break;
                }
                if (source.EOF)
                {
                    throw new exception.XukException("Unexpectedly reached EOF");
                }
            }
        }

        private void XukInAlternateContent(XmlReader source, IProgressHandler handler)
        {
            if (source.IsEmptyElement) return;
            while (source.Read())
            {
                if (source.NodeType == XmlNodeType.Element)
                {
                    if (source.NamespaceURI == XukAble.XUK_NS
                        && GetXukName(typeof(AlternateContent)).Match(source.LocalName))
                    {
                        AlternateContent ac = Presentation.AlternateContentFactory.CreateAlternateContent();
                        ac.XukIn(source, handler);
                        m_AlternateContents.Insert(m_AlternateContents.Count, ac);
                    }
                }
                else if (source.NodeType == XmlNodeType.EndElement)
                {
                    break;
                }
                if (source.EOF)
                {
                    throw new exception.XukException("Unexpectedly reached EOF");
                }
            }
        }
        protected override void XukInChild(XmlReader source, IProgressHandler handler)
        {
            bool readItem = false;
            if (source.NamespaceURI == XukAble.XUK_NS)
            {
                readItem = true;
                if (source.LocalName == XukStrings.Metadatas)
                {
                    XukInMetadata(source, handler);
                }
                else if (source.LocalName == XukStrings.AlternateContents)
                {
                    XukInAlternateContent(source, handler);
                }
                else
                {
                    readItem = false;
                }
            }
            if (!(readItem || source.IsEmptyElement))
            {
                source.ReadSubtree().Close(); //Read past unknown child 
            }
        }
    }
}
