using System;
using System.Xml;
using urakawa.progress;
using urakawa.xuk;

namespace urakawa.media
{
    /// <summary>
    /// TextMedia represents a text string
    /// </summary>
    public class TextMedia : AbstractTextMedia
    {

        public override string GetTypeNameFormatted()
        {
            return XukStrings.TextMedia;
        }

        private string mText;

        private void Reset()
        {
            mText = "";
        }

        /// <summary>
        /// Default constructor - for system use only, 
        /// <see cref="TextMedia"/>s should only be created via. the <see cref="MediaFactory"/>
        /// </summary>
        public TextMedia()
        {
            Reset();
        }

        /// <summary>
        /// This override is useful while debugging
        /// </summary>
        /// <returns>The textual content of the <see cref="AbstractTextMedia"/></returns>
        public override string ToString()
        {
            return mText;
        }

        #region AbstractTextMedia Members

        /// <summary>
        /// Return the text string
        /// </summary>
        /// <returns></returns>
        public override string Text
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
                NotifyTextChanged(value, prevText);
            }
        }

        #endregion

        #region Media Members


        /// <summary>
        /// Make a copy of this text object
        /// </summary>
        /// <returns>The copy</returns>
        public new TextMedia Copy()
        {
            return CopyProtected() as TextMedia;
        }

        ///<summary>
        ///
        ///</summary>
        ///<returns></returns>
        protected override Media CopyProtected()
        {
            TextMedia copy = (TextMedia)base.CopyProtected();
            copy.Text = Text;
            return copy;
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
        protected override Media ExportProtected(Presentation destPres)
        {
            TextMedia exported = (TextMedia) base.ExportProtected(destPres);
            exported.Text = Text;
            return exported;
        }

        #endregion

        #region IXukAble members

        /// <summary>
        /// Clears the <see cref="TextMedia"/> setting the text to <c>""</c>
        /// </summary>
        protected override void Clear()
        {
            Reset();
            base.Clear();
        }

        /// <summary>
        /// Reads a child of a TextMedia xuk element. 
        /// </summary>
        /// <param name="source">The source <see cref="XmlReader"/></param>
        /// <param name="handler">The handler for progress</param>
        protected override void XukInChild(XmlReader source, IProgressHandler handler)
        {
            if (IsPrettyFormat())
            {
                if (source.LocalName == XukStrings.Text && source.NamespaceURI == XukNamespaceUri)
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
            }
            else
            {
                Text = source.ReadString();
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
        protected override void XukOutChildren(XmlWriter destination, Uri baseUri, IProgressHandler handler)
        {
            if (IsPrettyFormat())
            {
                destination.WriteStartElement(XukStrings.Text, XukNamespaceUri);
            }
            destination.WriteString(Text);
            if (IsPrettyFormat())
            {
                destination.WriteEndElement();
            }
            base.XukOutChildren(destination, baseUri, handler);
        }

        #endregion

    }
}