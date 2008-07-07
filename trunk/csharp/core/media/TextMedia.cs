using System;
using System.Xml;
using urakawa.progress;

namespace urakawa.media
{
    /// <summary>
    /// TextMedia represents a text string
    /// </summary>
    public class TextMedia : AbstractMedia, ITextMedia
    {
        #region Event related members

        /// <summary>
        /// Event fired after the text of the <see cref="TextMedia"/> has changed
        /// </summary>
        public event EventHandler<urakawa.events.media.TextChangedEventArgs> TextChanged;

        /// <summary>
        /// Fires the <see cref="TextChanged"/> event
        /// </summary>
        /// <param name="source">The source, that is the <see cref="TextMedia"/> whoose text was changed</param>
        /// <param name="newText">The new text value</param>
        /// <param name="prevText">Thye text value prior to the change</param>
        protected void NotifyTextChanged(TextMedia source, string newText, string prevText)
        {
            EventHandler<urakawa.events.media.TextChangedEventArgs> d = TextChanged;
            if (d != null) d(this, new urakawa.events.media.TextChangedEventArgs(source, newText, prevText));
        }

        private void this_textChanged(object sender, urakawa.events.media.TextChangedEventArgs e)
        {
            NotifyChanged(e);
        }

        #endregion

        /// <summary>
        /// Constructor setting the associated <see cref="IMediaFactory"/>
        /// </summary>
        /// <exception cref="exception.MethodParameterIsNullException">
        /// Thrown when <paramref localName="fact"/> is <c>null</c>
        /// </exception>
        protected internal TextMedia()
        {
            mText = "";
            this.TextChanged += new EventHandler<urakawa.events.media.TextChangedEventArgs>(this_textChanged);
        }


        private string mText;


        /// <summary>
        /// This override is useful while debugging
        /// </summary>
        /// <returns>The textual content of the <see cref="ITextMedia"/></returns>
        public override string ToString()
        {
            return mText;
        }

        #region ITextMedia Members

        /// <summary>
        /// Return the text string
        /// </summary>
        /// <returns></returns>
        public string Text
        {
            get { return mText; }
            set
            {
                if (value == null)
                {
                    throw new exception.MethodParameterIsNullException("The text of a TextMedia cannot be null");
                }
                string prevText = mText;
                mText = value;
                NotifyTextChanged(this, value, prevText);
            }
        }

        #endregion

        #region IMedia Members

        /// <summary>
        /// This always returns false, because
        /// text media is never considered continuous
        /// </summary>
        /// <returns></returns>
        public override bool IsContinuous
        {
            get { return false; }
        }

        /// <summary>
        /// This always returns true, because
        /// text media is always considered discrete
        /// </summary>
        /// <returns></returns>
        public override bool IsDiscrete
        {
            get { return true; }
        }


        /// <summary>
        /// This always returns false, because
        /// a single media object is never considered to be a sequence
        /// </summary>
        /// <returns></returns>
        public override bool IsSequence
        {
            get { return false; }
        }

        /// <summary>
        /// Make a copy of this text object
        /// </summary>
        /// <returns>The copy</returns>
        public new TextMedia Copy()
        {
            return CopyProtected() as TextMedia;
        }

        /// <summary>
        /// Make a copy of this text object
        /// </summary>
        /// <returns>The copy</returns>
        protected override IMedia CopyProtected()
        {
            return Export(MediaFactory.Presentation);
        }

        /// <summary>
        /// Exports the text media to a destination <see cref="Presentation"/>
        /// </summary>
        /// <param name="destPres">The destination presentation</param>
        /// <returns>The exported external text media</returns>
        public new TextMedia Export(Presentation destPres)
        {
            return ExportProtected(destPres) as TextMedia;
        }

        /// <summary>
        /// Exports the text media to a destination <see cref="Presentation"/>
        /// </summary>
        /// <param name="destPres">The destination presentation</param>
        /// <returns>The exported external text media</returns>
        protected override IMedia ExportProtected(Presentation destPres)
        {
            TextMedia exported = destPres.MediaFactory.CreateMedia(
                                     XukLocalName, XukNamespaceUri) as TextMedia;
            if (exported == null)
            {
                throw new exception.FactoryCannotCreateTypeException(String.Format(
                                                                         "The MediaFactory cannot create a TextMedia matching QName {1}:{0}",
                                                                         XukLocalName, XukNamespaceUri));
            }
            exported.Text = this.Text;
            return exported;
        }

        #endregion

        #region IXukAble members

        /// <summary>
        /// Clears the <see cref="TextMedia"/> setting the text to <c>""</c>
        /// </summary>
        protected override void Clear()
        {
            mText = "";
            base.Clear();
        }

        /// <summary>
        /// Reads a child of a TextMedia xuk element. 
        /// </summary>
        /// <param name="source">The source <see cref="XmlReader"/></param>
        /// <param name="handler">The handler for progress</param>
        protected override void XukInChild(XmlReader source, ProgressHandler handler)
        {
            if (source.LocalName == "mText" && source.NamespaceURI == ToolkitSettings.XUK_NS)
            {
                if (!source.IsEmptyElement)
                {
                    XmlReader subtreeReader = source.ReadSubtree();
                    subtreeReader.Read();
                    try
                    {
                        Text = subtreeReader.ReadElementContentAsString();
                    }
                    finally
                    {
                        subtreeReader.Close();
                    }
                }
                return;
            }
            base.XukInChild(source, handler);
        }

        /// <summary>
        /// Write the child elements of a TextMedia element.
        /// </summary>
        /// <param name="destination">The destination <see cref="XmlWriter"/></param>
        /// <param name="baseUri">
        /// The base <see cref="Uri"/> used to make written <see cref="Uri"/>s relative, 
        /// if <c>null</c> absolute <see cref="Uri"/>s are written
        /// </param>
        /// <param name="handler">The handler for progress</param>
        protected override void XukOutChildren(XmlWriter destination, Uri baseUri, ProgressHandler handler)
        {
            destination.WriteStartElement("mText", ToolkitSettings.XUK_NS);
            destination.WriteString(Text);
            destination.WriteEndElement();
            base.XukOutChildren(destination, baseUri, handler);
        }

        #endregion

        #region IValueEquatable<IMedia> Members

        /// <summary>
        /// Compares <c>this</c> with a given other <see cref="IMedia"/> for equality
        /// </summary>
        /// <param name="other">The other <see cref="IMedia"/></param>
        /// <returns><c>true</c> if equal, otherwise <c>false</c></returns>
        public override bool ValueEquals(IMedia other)
        {
            if (!base.ValueEquals(other)) return false;
            if (Text != ((TextMedia) other).Text) return false;
            return true;
        }

        #endregion
    }
}