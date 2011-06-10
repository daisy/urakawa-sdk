using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

using urakawa.xuk;
using urakawa.progress;
namespace urakawa.property.alt
{
    public class AlternateContentProperty:Property
    {
        public override string GetTypeNameFormatted()
        {
            return XukStrings.AlternateContentProperty;
        }

        private AlternateContents m_AlternateContents;

        public AlternateContentProperty()
        {
            m_AlternateContents = null;
        }
        
        public void SetAlternateContents(AlternateContents contents)
        {
            if (contents == null) throw new exception.MethodParameterIsNullException ("Contents instance is null");

            m_AlternateContents = contents;
        }

        public AlternateContents AlternateContents 
        { 
            get 
            {
                if (m_AlternateContents == null) m_AlternateContents = new AlternateContents();
                    
                return m_AlternateContents; 
            } 
        }

        protected override void XukInChild(XmlReader source, IProgressHandler handler)
        {
            if (source.LocalName == XukStrings.AlternateContents)
            {
                AlternateContents.XukIn(source, handler);
            }
            else
            {
                base.XukInChild(source, handler);
            }
        }

        protected override void XukOutChildren(XmlWriter destination, Uri baseUri, IProgressHandler handler)
        {
            base.XukOutChildren(destination, baseUri, handler);
            AlternateContents.XukOut(destination, baseUri, handler);
        }

    }
}
