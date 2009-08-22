﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using urakawa.command;
using urakawa.core;
using urakawa.exception;
using urakawa.progress;
using urakawa.xuk;
using urakawa.metadata;

namespace urakawa.commands
{

    public class MetadataAddCommand : Command
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

        public override string GetTypeNameFormatted()
        {
            return XukStrings.MetadataAddCommand;
        }

        private Metadata m_Metadata;
        public Metadata Metadata
        {
            private set { m_Metadata = value; }
            get { return m_Metadata; }
        }

        private Presentation m_Presentation;
        public Presentation Presentation
        {
            private set { m_Presentation = value;}
            get { return m_Presentation; }
        }

        public void Init(Metadata metadata, Presentation presentation)
        {
            if (metadata == null)
            {
                throw new ArgumentNullException("metadata");
            }
            if (presentation == null)
            {
                throw new ArgumentNullException("presentation");
            }
            if (presentation.Metadatas == null)
            {
                throw new ArgumentException("Presentation has null metadata");
            }
            Metadata = metadata;
            Presentation = presentation;

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
