using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

using urakawa.media;
using urakawa.progress;
using urakawa.xuk;

namespace urakawa.property.alt
{
    public class AlternateContents : WithPresentation
    {
        

        public AlternateContents()
        {
            m_AlternateContentItems = new ObjectListProvider<AlternateContent>(this, true);
        }

        private ObjectListProvider<AlternateContent> m_AlternateContentItems;
        public ObjectListProvider<AlternateContent> AlternateContentItems
        {
            get { return m_AlternateContentItems; } 
        }


        public override string GetTypeNameFormatted()
        {
            return XukStrings.AlternateContents;
        }

        public void AddAlternateContentItem(AlternateContent content)
        {
            if (content == null) throw new exception.MethodParameterIsNullException ("Null AlternateContent cannot be added");

            m_AlternateContentItems.Insert(m_AlternateContentItems.Count, content);
        }

        public void RemoveAlternateContentItem(AlternateContent content)
        {
            if (content == null) throw new exception.MethodParameterIsNullException("AlternateContent to be removed has null value");

            if (!m_AlternateContentItems.ContentsAs_ReadOnlyCollectionWrapper.Contains(content)) throw new exception.CannotExecuteException("Alternate content to be removed is not in collection");

            m_AlternateContentItems.Remove(content);
        }

        protected override void XukInChild(XmlReader source, IProgressHandler handler)
        {
            if (source.LocalName == XukStrings.AlternateContentItems)
            {
                XukInAlternateContent(source, handler);
            }
            else
            {
                base.XukInChild(source, handler);
            }

        }

        private void XukInAlternateContent(XmlReader source, IProgressHandler handler)
        {
            if (source.IsEmptyElement) return;
            while (source.Read())
            {
                if (source.NodeType == XmlNodeType.Element)
                {
                    AlternateContent newAltContent = Presentation.AlternateContentFactory.Create();
                    if (newAltContent != null)
                    {
                        newAltContent.XukIn(source, handler);
                        m_AlternateContentItems.Insert(m_AlternateContentItems.Count, newAltContent);
                    }
                    else if (!source.IsEmptyElement)
                    {
                        //Read past unidentified element
                        source.ReadSubtree().Close();
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

        protected override void XukOutChildren(XmlWriter destination, Uri baseUri, IProgressHandler handler)
        {
            base.XukOutChildren(destination, baseUri, handler);

            destination.WriteStartElement(XukStrings.AlternateContentItems, XukAble.XUK_NS);
            foreach (AlternateContent ac in m_AlternateContentItems.ContentsAs_Enumerable)
            {
                ac.XukOut(destination, baseUri, handler);
            }
            destination.WriteEndElement();
        }
        


    }
}
