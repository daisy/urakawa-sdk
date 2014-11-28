using System;
using System.Xml;
using urakawa.command;
using urakawa.progress;
using urakawa.xuk;
using urakawa.metadata;

namespace urakawa.commands
{
    public abstract class MetadataCommand : Command
    {
        public abstract Metadata Metadata { protected set; get; }
    }

    [XukNameUglyPrettyAttribute("metaAddCmd", "MetadataAddCommand")]
    public class MetadataAddCommand : MetadataCommand
    {
        public override bool ValueEquals(WithPresentation other)
        {
            if (!base.ValueEquals(other))
            {
                return false;
            }

            MetadataAddCommand otherz = other as MetadataAddCommand;
            if (otherz == null)
            {
                return false;
            }

            // TODO: test local equality

            return true;
        }
        
        private Metadata m_Metadata;
        public override Metadata Metadata
        {
            protected set { m_Metadata = value; }
            get { return m_Metadata; }
        }

        public void Init(Metadata metadata)
        {
            if (metadata == null)
            {
                throw new ArgumentNullException("metadata");
            }
            if (Presentation.Metadatas == null)
            {
                throw new ArgumentException("Presentation has null metadata");
            }
            Metadata = metadata;

            ShortDescription = "Add metadata";
            LongDescription = "Add the Metadata object to the Presentation";
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
            Presentation.Metadatas.Insert(Presentation.Metadatas.Count, Metadata);
        }

        public override void UnExecute()
        {
            Presentation.Metadatas.Remove(Metadata);
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
