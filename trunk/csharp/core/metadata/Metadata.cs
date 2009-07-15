using System;
using System.Collections.Generic;
using System.Xml;
using urakawa.events;
using urakawa.events.metadata;
using urakawa.xuk;

namespace urakawa.metadata
{
    /// <summary>
    /// Represents <see cref="Metadata"/> of a <see cref="Presentation"/>
    /// </summary>
    public class Metadata : WithPresentation, IChangeNotifier
    {
        public override string GetTypeNameFormatted()
        {
            return XukStrings.Metadata;
        }
        #region IChangeNotifier members

        /// <summary>
        /// Event fired after the <see cref="Metadata"/> has changed. 
        /// The event fire before any change specific event 
        /// </summary>
        public event EventHandler<DataModelChangedEventArgs> Changed;

        /// <summary>
        /// Fires the <see cref="Changed"/> event 
        /// </summary>
        /// <param name="args">The arguments of the event</param>
        protected void NotifyChanged(DataModelChangedEventArgs args)
        {
            EventHandler<DataModelChangedEventArgs> d = Changed;
            if (d != null) d(this, args);
        }

        #endregion

        /// <summary>
        /// Event fired after the name of the <see cref="Metadata"/> has changed
        /// </summary>
        public event EventHandler<NameChangedEventArgs> NameChanged;

        /// <summary>
        /// Fires the <see cref="NameChanged"/> event
        /// </summary>
        /// <param name="newName">The new name</param>
        /// <param name="prevName">The name prior to the change</param>
        protected void NotifyNameChanged(string newName, string prevName)
        {
            EventHandler<NameChangedEventArgs> d = NameChanged;
            if (d != null) d(this, new NameChangedEventArgs(this, newName, prevName));
        }

        /// <summary>
        /// Event fired after the content of the <see cref="Metadata"/> has changed
        /// </summary>
        public event EventHandler<ContentChangedEventArgs> ContentChanged;

        /// <summary>
        /// Fires the <see cref="ContentChanged"/> event
        /// </summary>
        /// <param name="newContent">The new content</param>
        /// <param name="prevContent">The content prior to the change</param>
        protected void NotifyContentChanged(string newContent, string prevContent)
        {
            EventHandler<ContentChangedEventArgs> d = ContentChanged;
            if (d != null) d(this, new ContentChangedEventArgs(this, newContent, prevContent));
        }

        /// <summary>
        /// Event fired after the optional attribute of the <see cref="Metadata"/> has changed
        /// </summary>
        public event EventHandler<OptionalAttributeChangedEventArgs> OptionalAttributeChanged;

        /// <summary>
        /// Fires the <see cref="OptionalAttributeChanged"/> event
        /// </summary>
        /// <param name="name">The name of the optional attribute</param>
        /// <param name="newVal">The new value of the optional attribute</param>
        /// <param name="prevValue">The value of the optional attribute prior to the change</param>
        protected void NotifyOptionalAttributeChanged(string name, string newVal, string prevValue)
        {
            EventHandler<OptionalAttributeChangedEventArgs> d = OptionalAttributeChanged;
            if (d != null) d(this, new OptionalAttributeChangedEventArgs(this, name, newVal, prevValue));
        }

        private string mName;

        private Dictionary<string, string> mAttributes;

        /// <summary>
        /// Default constructor - for system use only, 
        /// <see cref="Metadata"/>s should only be created via. the <see cref="MetadataFactory"/>
        /// </summary>
        public Metadata()
        {
            mName = "";
            mAttributes = new Dictionary<string, string>();
            mAttributes.Add(XukStrings.MetaDataContent, "");
            NameChanged += this_NameChanged;
            ContentChanged += this_ContentChanged;
            OptionalAttributeChanged += this_OptionalAttributeChanged;
        }

        void this_OptionalAttributeChanged(object sender, OptionalAttributeChangedEventArgs e)
        {
            NotifyChanged(e);
        }

        void this_ContentChanged(object sender, ContentChangedEventArgs e)
        {
            NotifyChanged(e);
        }

        void this_NameChanged(object sender, NameChangedEventArgs e)
        {
            NotifyChanged(e);
        }

        /// <summary>
        /// Gets the name
        /// </summary>
        /// <returns>The name</returns>
        public string Name
        {
            get { return mName; }
            set
            {
                if (value == null)
                {
                    throw new exception.MethodParameterIsNullException(
                        "The name can no t be null");
                }
                string prevName = mName;
                mName = value;
                if (prevName != mName) NotifyNameChanged(value, prevName);
            }
        }

        /// <summary>
        /// Gets the content
        /// </summary>
        /// <returns>The content, or null if none has been set yet.</returns>
        public string Content
        {
            get { return mAttributes.ContainsKey(XukStrings.MetaDataContent) ? mAttributes[XukStrings.MetaDataContent] : null; }
            set
            {
                if (value == null)
                {
                    throw new exception.MethodParameterIsNullException(
                        "Content can not be null");
                }
                string prevContent = Content;
                mAttributes[XukStrings.MetaDataContent] = value;
                if (value != prevContent) NotifyContentChanged(value, prevContent);
            }
        }

        /// <summary>
        /// Gets the value of a named attribute
        /// </summary>
        /// <param name="name">The name of the attribute</param>
        /// <returns>The value of the attribute - <see cref="String.Empty"/> if the attribute does not exist</returns>
        public string GetOptionalAttributeValue(string name)
        {
            if (mAttributes.ContainsKey(name))
            {
                return mAttributes[name];
            }
            return "";
        }

        /// <summary>
        /// Sets the value of a named attribute
        /// </summary>
        /// <param name="name">The name of the attribute</param>
        /// <param name="value">The new value for the attribute</param>
        public void SetOptionalAttributeValue(string name, string value)
        {
            if (value == null)
            {
                throw new exception.MethodParameterIsNullException(
                    "A metadata attribute can not have null value");
            }
            if (name == XukStrings.MetaDataName) Name = value;
            else if (name == XukStrings.MetaDataContent) Content = name;
            string prevValue = GetOptionalAttributeValue(name);
            if (mAttributes.ContainsKey(name))
            {
                mAttributes[name] = value;
            }
            else
            {
                mAttributes.Add(name, value);
            }
            if (prevValue != name) NotifyOptionalAttributeChanged(name, value, prevValue);
        }

        /// <summary>
        /// Gets the names of all attributes with non-empty names
        /// </summary>
        /// <returns>A <see cref="List{String}"/> containing the attribute names</returns>
        public List<string> ListOfOptionalAttributeNames
        {
            get
            {
                List<string> names = new List<string>(mAttributes.Keys);
                foreach (string name in new List<string>(names))
                {
                    if (mAttributes[name] == "") names.Remove(name);
                }
                return names;
            }
        }

        #region IXUKAble members

        /// <summary>
        /// Reads the attributes of a Metadata xuk element.
        /// </summary>
        /// <param name="source">The source <see cref="XmlReader"/></param>
        protected override void XukInAttributes(XmlReader source)
        {
            base.XukInAttributes(source);

            if (source.MoveToFirstAttribute())
            {
                bool moreAttrs = true;
                while (moreAttrs)
                {
                    SetOptionalAttributeValue(source.Name, source.Value);
                    moreAttrs = source.MoveToNextAttribute();
                }
                source.MoveToElement();
            }
        }

        /// <summary>
        /// Writes the attributes of a Metadata element
        /// </summary>
        /// <param name="destination">The destination <see cref="XmlWriter"/></param>
        /// <param name="baseUri">
        /// The base <see cref="Uri"/> used to make written <see cref="Uri"/>s relative, 
        /// if <c>null</c> absolute <see cref="Uri"/>s are written
        /// </param>
        protected override void XukOutAttributes(XmlWriter destination, Uri baseUri)
        {
            base.XukOutAttributes(destination, baseUri);

            destination.WriteAttributeString(XukStrings.MetaDataName, Name);
            foreach (string a in ListOfOptionalAttributeNames)
            {
                if (a != XukStrings.MetaDataName)
                {
                    destination.WriteAttributeString(a, GetOptionalAttributeValue(a));
                }
            }
        }

        #endregion

        #region IValueEquatable<Metadata> Members

        public override bool ValueEquals(WithPresentation other)
        {
            if (!base.ValueEquals(other))
            {
                return false;
            }

            Metadata otherz = other as Metadata;
            if (otherz == null)
            {
                return false;
            }

            if (Name != otherz.Name) return false;
            List<string> names = ListOfOptionalAttributeNames;
            List<string> otherNames = otherz.ListOfOptionalAttributeNames;
            if (names.Count != otherNames.Count) return false;
            foreach (string name in names)
            {
                if (!otherNames.Contains(name)) return false;
                if (GetOptionalAttributeValue(name) != otherz.GetOptionalAttributeValue(name)) return false;
            }


            return true;
        }

        #endregion
    }
}