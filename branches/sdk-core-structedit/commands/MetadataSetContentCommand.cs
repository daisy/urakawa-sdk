using System;
using System.Xml;
using urakawa.command;
using urakawa.progress;
using urakawa.xuk;
using urakawa.metadata;

namespace urakawa.commands
{
    [XukNameUglyPrettyAttribute("metaSetContentCmd", "MetadataSetContentCommand")]
    public class MetadataSetContentCommand : MetadataCommand
    {
        public override bool ValueEquals(WithPresentation other)
        {
            if (!base.ValueEquals(other))
            {
                return false;
            }

            MetadataSetContentCommand otherz = other as MetadataSetContentCommand;
            if (otherz == null)
            {
                return false;
            }

            // TODO: test local equality

            return true;
        }
        

        private string m_OriginalContent;
        private string m_NewContent;
        public string Content
        {
            private set { m_NewContent = value; }
            get { return m_NewContent; }
        }

        private Metadata m_Metadata;
        public override Metadata Metadata
        {
            protected set { m_Metadata = value; }
            get { return m_Metadata; }
        }


        public void Init(Metadata metadata, string content)
        {
            if (metadata == null)
            {
                throw new ArgumentNullException("metadata");
            }
            if (content == null)
            {
                throw new ArgumentNullException("content");
            }

            Metadata = metadata;
            m_OriginalContent = Metadata.NameContentAttribute.Value;
            Content = content;

            ShortDescription = "Set metadata content";
            LongDescription = "Set the Content of the Metadata object";
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
            Metadata.NameContentAttribute.Value = m_NewContent;
        }

        public override void UnExecute()
        {
            Metadata.NameContentAttribute.Value = m_OriginalContent;
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
