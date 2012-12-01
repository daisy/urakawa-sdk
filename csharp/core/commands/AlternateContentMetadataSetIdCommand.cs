﻿using System;
using System.Xml;
using urakawa.command;
using urakawa.metadata;
using urakawa.progress;
using urakawa.property.alt;
using urakawa.xuk;

namespace urakawa.commands
{
    [XukNameUglyPrettyAttribute("acMetaSetIdCmd", "AlternateContentMetadataSetIdCommand")]
    public class AlternateContentMetadataSetIdCommand : Command
    {
        public override bool ValueEquals(WithPresentation other)
        {
            if (!base.ValueEquals(other))
            {
                return false;
            }

            AlternateContentMetadataSetIdCommand otherz = other as AlternateContentMetadataSetIdCommand;
            if (otherz == null)
            {
                return false;
            }

            // TODO: test local equality

            return true;
        }
            

        private bool m_OriginalId;
        private bool m_NewId;
        private Metadata m_Metadata;

        private AlternateContentProperty m_AlternateContentProperty;
        public AlternateContentProperty AlternateContentProperty { get { return m_AlternateContentProperty; } }

        private AlternateContent m_AlternateContent;
        public AlternateContent AlternateContent { get { return m_AlternateContent; } }

        public void Init(AlternateContentProperty altContentProperty, AlternateContent altContent, Metadata metadata, bool id)
        {
            if (metadata == null)
            {
                throw new ArgumentNullException("metadata");
            }

            if (altContentProperty == null && altContent == null)
            {
                throw new ArgumentNullException("altContentProperty && altContent");
            }
            if (altContentProperty != null && altContent != null)
            {
                throw new ArgumentException("altContentProperty && altContent");
            }
            m_Metadata = metadata;
            m_OriginalId = m_Metadata.IsMarkedAsPrimaryIdentifier;
            m_NewId = id;
            m_AlternateContent = altContent;
            m_AlternateContentProperty = altContentProperty;

            ShortDescription = "Set metadata ID marker";
            LongDescription = "Set the marker that indicates whether a metadata is the unique identifier of the AlternateContent";
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
            m_Metadata.IsMarkedAsPrimaryIdentifier = m_NewId;
        }

        public override void UnExecute()
        {
            m_Metadata.IsMarkedAsPrimaryIdentifier = m_OriginalId;
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
