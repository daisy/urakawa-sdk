using System;
using System.Xml;
using urakawa.command;
using urakawa.progress;
using urakawa.xuk;
using urakawa.metadata;

namespace urakawa.commands
{
    public class MetadataSetUidCommand : Command
    {
        public override bool ValueEquals(WithPresentation other)
        {
            if (!base.ValueEquals(other))
            {
                return false;
            }

            MetadataSetUidCommand otherz = other as MetadataSetUidCommand;
            if (otherz == null)
            {
                return false;
            }

            // TODO: test local equality

            return true;
        }

        public override string GetTypeNameFormatted()
        {
            return XukStrings.MetadataSetUidCommand;
        }

        private string m_OriginalId;
        private string m_NewId;

        public string Id
        {
            private set { m_NewId = value; }
            get { return m_NewId; }
        }
        private Metadata m_Metadata;
        public Metadata Metadata
        {
            private set { m_Metadata = value; }
            get { return m_Metadata; }
        }



        public void Init(Metadata metadata, string id)
        {
            if (metadata == null)
            {
                throw new ArgumentNullException("metadata");
            }
            if (id == null)
            {
                throw new ArgumentNullException("id");
            }

            Metadata = metadata;
            m_OriginalId = Metadata.NameContentAttribute.Id;
            Id = id;

            ShortDescription = "Set metadata ID";
            LongDescription = "Set the ID string of the Metadata object";
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
            Metadata.Id = m_NewId;
        }

        public override void UnExecute()
        {
            Metadata.Id = m_OriginalId;
        }


        protected override void XukInAttributes(XmlReader source)
        {
            //nothing new here
            base.XukInAttributes(source);
        }

        protected override void XukInChild(XmlReader source, ProgressHandler handler)
        {
            //nothing new here
            base.XukInChild(source, handler);
        }

        protected override void XukOutAttributes(XmlWriter destination, Uri baseUri)
        {
            //nothing new here
            base.XukOutAttributes(destination, baseUri);
        }

        protected override void XukOutChildren(XmlWriter destination, Uri baseUri, ProgressHandler handler)
        {
            //nothing new here
            base.XukOutChildren(destination, baseUri, handler);
        }
    }
}
