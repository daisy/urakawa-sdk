using System;
using System.Collections.Generic;
using System.Xml;
using urakawa.command;
using urakawa.core;
using urakawa.media.data;
using urakawa.progress;
using urakawa.xuk;

namespace urakawa.commands
{
    public class TreeNodeAudioStreamDeleteCommand : Command
    {
        public override bool ValueEquals(WithPresentation other)
        {
            if (!base.ValueEquals(other))
            {
                return false;
            }

            TreeNodeAudioStreamDeleteCommand otherz = other as TreeNodeAudioStreamDeleteCommand;
            if (otherz == null)
            {
                return false;
            }

            // TODO: test local equality

            return true;
        }

        public override string GetTypeNameFormatted()
        {
            return XukStrings.TreeNodeAudioStreamDeleteCommand;
        }

        private TreeNodeAndStreamSelection m_SelectionData;
        public TreeNodeAndStreamSelection SelectionData
        {
            private set { m_SelectionData = value; }
            get { return m_SelectionData; }
        }

        public void Init(TreeNodeAndStreamSelection selection)
        {
            if (selection.m_TreeNode == null)
            {
                throw new ArgumentNullException("selection.m_TreeNode");
            }

            SelectionData = selection;

            ShortDescription = "Delete audio portion";
            LongDescription = "Delete a portion of audio for a given treenode";
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
            //TODO
        }

        public override void UnExecute()
        {
            //TODO
        }

        private List<MediaData> m_UsedMediaData = new List<MediaData>();
        public override IEnumerable<MediaData> UsedMediaData
        {
            get
            {
                return m_UsedMediaData;
            }
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
