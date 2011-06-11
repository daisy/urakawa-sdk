using System;
using System.Xml;
using urakawa.command;
using urakawa.progress;
using urakawa.xuk;
using urakawa.metadata;

using urakawa.property.alt;

namespace urakawa.commands
{
    public class AlternateContentMetadataRemoveCommand : Command
    {
        
        public override bool ValueEquals(WithPresentation other)
        {
            if (!base.ValueEquals(other))
            {
                return false;
            }

            AlternateContentMetadataRemoveCommand otherz = other as AlternateContentMetadataRemoveCommand;
            if (otherz == null)
            {
                return false;
            }

            // TODO: test local equality

            return true;
        }

        public override string GetTypeNameFormatted()
        {
            return XukStrings.AlternateContentMetadataRemoveCommand;
        }

        private int m_Index;
        private Metadata m_Metadata;
        public Metadata Metadata
        {
            private set { m_Metadata = value; }
            get { return m_Metadata; }
        }

        private AlternateContentProperty m_AlternateContent;
        public AlternateContentProperty AlternateContent { get { return m_AlternateContent; } }

        public void Init(AlternateContentProperty altContent, Metadata metadata)
        {
            if (metadata == null)
            {
                throw new ArgumentNullException("metadata");
            }
            if (altContent == null)
            {
                throw new ArgumentNullException("altContent");
            }
            if (altContent.Metadatas == null)
            {
                throw new ArgumentException("AlternateContent has null metadata");
            }
            Metadata = metadata;
            m_AlternateContent = altContent;

            ShortDescription = "Remove metadata";
            LongDescription = "Remove the Metadata object from the AlternateContent";
        }

        public override bool CanExecute
        {
            get { return true; }
        }

        public override bool CanUnExecute
        {
            get { return true; }
        }

        public override void Execute()
        {
            m_Index = m_AlternateContent.Metadatas.IndexOf(Metadata);
            m_AlternateContent.Metadatas.Remove(Metadata);
        }

        public override void UnExecute()
        {
            m_AlternateContent.Metadatas.Insert(m_Index, Metadata);
        }


        protected override void XukInAttributes(XmlReader source)
        {
            //nothing new here
            base.XukInAttributes(source);
        }

        protected override void XukInChild(XmlReader source, IProgressHandler handler)
        {
            //nothing new here
            base.XukInChild(source, handler);
        }

        protected override void XukOutAttributes(XmlWriter destination, Uri baseUri)
        {
            //nothing new here
            base.XukOutAttributes(destination, baseUri);
        }

        protected override void XukOutChildren(XmlWriter destination, Uri baseUri, IProgressHandler handler)
        {
            //nothing new here
            base.XukOutChildren(destination, baseUri, handler);
        }
    }
}
