using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Xml;

using urakawa.core;
using urakawa.xuk;
using urakawa.progress;
using urakawa.metadata;
using urakawa.property.alt;

namespace urakawa.commands
{
    public class AlternateContentMetadataAttributeAddCommand:command.Command
    {
                
        public override bool ValueEquals(WithPresentation other)
        {
            if (!base.ValueEquals(other))
            {
                return false;
            }

            AlternateContentMetadataAttributeAddCommand otherz = other as AlternateContentMetadataAttributeAddCommand;
            if (otherz == null)
            {
                return false;
            }

            // TODO: test local equality

            return true;
        }

        public override string GetTypeNameFormatted()
        {
            return XukStrings.AlternateContentMetadataAttributeAddCommand;
        }

        private MetadataAttribute m_MetadataAttribute;
        public MetadataAttribute MetadataAttribute
        {
            private set { m_MetadataAttribute= value; }
            get { return m_MetadataAttribute; }
        }

        private Metadata m_Metadata;
        public Metadata Metadata
        {
            private set { m_Metadata = value; }
            get { return m_Metadata; }
        }


        public void Init(Metadata metadata, MetadataAttribute metadataAttribute)
        {
            
            if (metadata == null)
            {
                throw new ArgumentNullException("metadata");
            }

            if (MetadataAttribute == null)
            {
                throw new ArgumentNullException("MetadataAttribute");
            }


            Metadata = metadata;
            MetadataAttribute = metadataAttribute ;
            

            ShortDescription = "Add metadata attribute";
            LongDescription = "Add the Metadata attribute object to the metadata";
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
            m_Metadata.OtherAttributes.Insert (m_Metadata.OtherAttributes.Count ,m_MetadataAttribute ) ;
        }

        public override void UnExecute()
        {
            m_Metadata.OtherAttributes.Remove (m_MetadataAttribute ) ;
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
