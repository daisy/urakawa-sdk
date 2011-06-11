using System;
using System.Collections.Generic;
using System.Xml;
using urakawa.command;
using urakawa.media.data;
using urakawa.progress;
using urakawa.property.alt;
using urakawa.xuk;

namespace urakawa.commands
{
    public class AlternateContentSetRoleCommand : Command
    {
        public override bool ValueEquals(WithPresentation other)
        {
            if (!base.ValueEquals(other))
            {
                return false;
            }

            AlternateContentSetRoleCommand otherz = other as AlternateContentSetRoleCommand;
            if (otherz == null)
            {
                return false;
            }

            // TODO: test local equality

            return true;
        }
        public override string GetTypeNameFormatted()
        {
            return XukStrings.AlternateContentSetRoleCommand;
        }

        private AlternateContent m_AlternateContent;
        public AlternateContent AlternateContent
        {
            private set { m_AlternateContent = value; }
            get { return m_AlternateContent; }
        }

        private string m_Role;
        public string Role
        {
            private set { m_Role = value; }
            get { return m_Role; }
        }
        private string m_OldRole;
        public string OldRole
        {
            private set { m_OldRole = value; }
            get { return m_OldRole; }
        }

        public void Init(AlternateContent altContent, string role)
        {
            if (altContent == null)
            {
                throw new ArgumentNullException("altContent");
            }

            if (string.IsNullOrEmpty(role))
            {
                throw new ArgumentNullException("role");
            }

            m_AlternateContent = altContent;
            Role = role;
            OldRole = m_AlternateContent.Role;

            ShortDescription = "Set role";
            LongDescription = "Set the role of an AlternateContent";
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
            m_AlternateContent.Role = Role;
        }

        public override void UnExecute()
        {
            m_AlternateContent.Role = OldRole;
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
