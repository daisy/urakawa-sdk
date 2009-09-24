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

        private string m_OriginalUid;
        private string m_NewUid;

        public string Uid
        {
            private set { m_NewUid = value; }
            get { return m_NewUid; }
        }
        private Metadata m_Metadata;
        public Metadata Metadata
        {
            private set { m_Metadata = value; }
            get { return m_Metadata; }
        }



        public void Init(Metadata metadata, string uid)
        {
            if (metadata == null)
            {
                throw new ArgumentNullException("metadata");
            }
            if (uid == null)
            {
                throw new ArgumentNullException("uid");
            }

            Metadata = metadata;
            m_OriginalUid = Metadata.NameContentAttribute.Uid;
            Uid = uid;

            ShortDescription = "Set metadata UID";
            LongDescription = "Set the UID of the Metadata object";
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
            Metadata.Uid = m_NewUid;
        }

        public override void UnExecute()
        {
            Metadata.Uid = m_OriginalUid;
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
