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
    public class MetadataSetNameCommand : Command
    {
        public override bool ValueEquals(WithPresentation other)
        {
            if (!base.ValueEquals(other))
            {
                return false;
            }

            MetadataSetNameCommand otherz = other as MetadataSetNameCommand;
            if (otherz == null)
            {
                return false;
            }

            // TODO: test local equality

            return true;
        }

        public override string GetTypeNameFormatted()
        {
            return XukStrings.MetadataSetNameCommand;
        }

        private string m_OriginalName;
        private string m_NewName;
        public string Name
        {
            private set { m_NewName = value; }
            get { return m_NewName; }
        }
        private Metadata m_Metadata;
        public Metadata Metadata
        {
            private set { m_Metadata = value; }
            get { return m_Metadata; }
        }

        

        public void Init(Metadata metadata, string name)
        {
            if (metadata == null)
            {
                throw new ArgumentNullException("metadata");
            }
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            Metadata = metadata;
            m_OriginalName = Metadata.Name;
            Name = name;

            ShortDescription = "Set metadata name";
            LongDescription = "Set the Name of the Metadata object";
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
            Metadata.Name = m_NewName;
        }

        public override void UnExecute()
        {
            Metadata.Name = m_OriginalName;
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
