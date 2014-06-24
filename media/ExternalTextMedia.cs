using System;
using System.Diagnostics;
using System.Text;
using System.Net;
using System.IO;
using System.Xml;
using urakawa.events.media;
using urakawa.xuk;

namespace urakawa.media
{
    [XukNameUglyPrettyAttribute("exTxtMed", "ExternalTextMedia")]
    public class ExternalTextMedia : AbstractTextMedia, ILocated
    {
        public const string DEFAULT_SRC = "file.ext";
        
        private string mSrc;

        private void Reset()
        {
            mSrc = DEFAULT_SRC;
        }

        /// <summary>
        /// Default constructor - for system use only, 
        /// <see cref="ExternalTextMedia"/>s should only be created via. the <see cref="MediaFactory"/>
        /// </summary>
        public ExternalTextMedia()
        {
            Reset();
            SrcChanged += this_SrcChanged;
        }

        /// <summary>
        /// Gets the text of the <c>this</c> using given <see cref="ICredentials"/>
        /// </summary>
        /// <param name="credits">The given credentisals</param>
        /// <returns>The text - if the plaintext file could not be found, <see cref="String.Empty"/> is returned</returns>
        /// <exception cref="exception.CannotReadFromExternalFileException">
        /// Thrown if the file referenced by <see cref="Src"/> is not accessible
        /// </exception>
        public string GetText(ICredentials credits)
        {
            WebClient client = new WebClient();
            client.Credentials = credits;
            return GetText(client);
        }

        private string GetText(WebClient client)
        {
            try
            {
                Uri src = Uri;
                StreamReader rd = new StreamReader(client.OpenRead(src), Encoding.UTF8);
                string res = rd.ReadToEnd();
                rd.Close();
                return res;
            }
            catch (Exception e)
            {
                throw new exception.CannotReadFromExternalFileException(
                    String.Format("Could read the text from plaintext file {0}: {1}", Src, e.Message),
                    e);
            }
        }

        /// <summary>
        /// Sets the text of <c>this</c> using given <see cref="ICredentials"/>
        /// </summary>
        /// <param name="text">The new text</param>
        /// <param name="credits">The given credentisals</param>
        /// <exception cref="exception.CannotWriteToExternalFileException">
        /// Thrown when the text could not be written to the <see cref="Uri"/> (as returned by <see cref="Src"/>)
        /// using the <see cref="WebClient.UploadData(Uri,byte[])"/> method.
        /// </exception>
        public void SetText(string text, ICredentials credits)
        {
            WebClient client = new WebClient();
            client.Credentials = credits;
            SetText(text, client);
        }

        private void SetText(string text, WebClient client)
        {
            string prevText;
            try
            {
                prevText = GetText(client);
            }
            catch (exception.CannotReadFromExternalFileException)
            {
                prevText = "";
            }
            Uri uri = Uri;
            if (uri.Scheme != Uri.UriSchemeFile && uri.Scheme != Uri.UriSchemeFtp)
            {
                throw new exception.CannotWriteToExternalFileException(String.Format(
                                                                           "Could not write the text to plaintext file {0} - unsupported scheme {1}",
                                                                           Src, uri.Scheme));
            }
            try
            {
                byte[] utf8Data = Encoding.UTF8.GetBytes(text);
                client.UploadData(uri, utf8Data);
            }
            catch (Exception e)
            {
                throw new exception.CannotWriteToExternalFileException(
                    String.Format("Could not write the text to plaintext file {0}: {1}", Src, e.Message),
                    e);
            }
            NotifyTextChanged(text, prevText);
        }

        #region Media Members


        /// <summary>
        /// Exports the external text media to a destination <see cref="Presentation"/>
        /// </summary>
        /// <param name="destPres">The destination presentation</param>
        /// <returns>The exported external text media</returns>
        public new ExternalTextMedia Export(Presentation destPres)
        {
            return ExportProtected(destPres) as ExternalTextMedia;
        }

        ///<summary>
        ///
        ///</summary>
        ///<returns></returns>
        protected override Media CopyProtected()
        {
            ExternalTextMedia copy = (ExternalTextMedia)base.CopyProtected();
            copy.Src = Src;
            return copy;
        }

        /// <summary>
        /// Exports the media to a destination <see cref="Presentation"/>
        /// </summary>
        /// <param name="destPres">The destination presentation</param>
        /// <returns>The exported media</returns>
        protected override Media ExportProtected(Presentation destPres)
        {
            ExternalTextMedia cpETM = (ExternalTextMedia) base.ExportProtected(destPres);
            cpETM.Src = Src;
            return cpETM;
        }

        /// <summary>
        /// Gets a copy of the <see cref="Media"/>
        /// </summary>
        /// <returns></returns>
        public new ExternalTextMedia Copy()
        {
            return CopyProtected() as ExternalTextMedia;
        }

        #endregion

        #region AbstractTextMedia Members

        /// <summary>
        /// Gets the text of the <c>this</c>
        /// </summary>
        /// <returns>The text - if the plaintext file could not be found, <see cref="String.Empty"/> is returned</returns>
        /// <exception cref="exception.CannotReadFromExternalFileException">
        /// Thrown if the file referenced by <see cref="Src"/> is not accessible
        /// </exception>
        public override string Text
        {
            get
            {
                WebClient client = new WebClient();
				try {
                client.UseDefaultCredentials = true;
				} catch (System.NotImplementedException e) {
					// Ignore (otherwise Mono does not pass the unit-tests)
					Console.WriteLine("WebClient.UseDefaultCredentials not implemented (using Mono ?)");
				}
                return GetText(client);
            }
            set
            {
                WebClient client = new WebClient();
                client.UseDefaultCredentials = true;
                SetText(value, client);
            }
        }

        #endregion

        #region IXUKAble overrides

        /// <summary>
        /// Clears the <see cref="ExternalTextMedia"/>, resetting the src value
        /// </summary>
        protected override void Clear()
        {
            Reset();
            base.Clear();
        }

        /// <summary>
        /// Reads the attributes of a ExternalMedia xuk element.
        /// </summary>
        /// <param name="source">The source <see cref="XmlReader"/></param>
        protected override void XukInAttributes(XmlReader source)
        {
            base.XukInAttributes(source);

            string val = source.GetAttribute(XukStrings.Src);
            if (string.IsNullOrEmpty(val)) val = DEFAULT_SRC;
            Src = val;
        }

        /// <summary>
        /// Writes the attributes of a ExternalMedia element
        /// </summary>
        /// <param name="destination">The destination <see cref="XmlWriter"/></param>
        /// <param name="baseUri">
        /// The base <see cref="Uri"/> used to make written <see cref="Uri"/>s relative, 
        /// if <c>null</c> absolute <see cref="Uri"/>s are written
        /// </param>
        protected override void XukOutAttributes(XmlWriter destination, Uri baseUri)
        {
            base.XukOutAttributes(destination, baseUri);

            if (Src != "")
            {
                Uri srcUri = new Uri(Presentation.RootUri, Src);
                if (baseUri == null)
                {
                    destination.WriteAttributeString(XukStrings.Src, srcUri.AbsoluteUri);
                }
                else
                {
                    destination.WriteAttributeString(XukStrings.Src, baseUri.MakeRelativeUri(srcUri).ToString());
                }
            }
        }

        #endregion

        #region ILocated Members

        /// <summary>
        /// Event fired after <see cref="Src"/> of the <see cref="ILocated"/> has changed
        /// </summary>
        public event EventHandler<SrcChangedEventArgs> SrcChanged;

        /// <summary>
        /// Fires the <see cref="SrcChanged"/> event
        /// </summary>
        /// <param name="newSrc">The new <see cref="Src"/> value</param>
        /// <param name="prevSrc">The <see cref="Src"/> value prior to the change</param>
        protected void NotifySrcChanged(string newSrc, string prevSrc)
        {
            EventHandler<SrcChangedEventArgs> d = SrcChanged;
            if (d != null) d(this, new SrcChangedEventArgs(this, newSrc, prevSrc));
        }

        private void this_SrcChanged(object sender, SrcChangedEventArgs e)
        {
            NotifyChanged(e);
        }


        /// <summary>
        /// Gets or sets the src value. The default value is DEFAULT_SRC
        /// </summary>
        /// <exception cref="exception.MethodParameterIsEmptyStringException">
        /// Thrown when trying to set the <see cref="Src"/> value to <c>null</c></exception>
        public string Src
        {
            get { return mSrc; }
            set
            {
                if (value == null)
                {
                    throw new exception.MethodParameterIsNullException("The src value cannot be null or empty");
                }
                if (value == "")
                {
                    throw new exception.MethodParameterIsEmptyStringException("The src value cannot be null or empty");
                }
                string prevSrc = mSrc;
                mSrc = value;
                if (mSrc != prevSrc) NotifySrcChanged(mSrc, prevSrc);
            }
        }

        /// <summary>
        /// Gets the <see cref="Uri"/> of the <see cref="ExternalAudioMedia"/> 
        /// - uses <c>getMediaFactory().getPresentation().getRootUri()</c> as base <see cref="Uri"/>
        /// </summary>
        /// <returns>The <see cref="Uri"/> - <c>null</c> if <see cref="Src"/> is <c>null</c></returns>
        /// <exception cref="exception.InvalidUriException">
        /// Thrown when the value <see cref="Src"/> is not a well-formed <see cref="Uri"/>
        /// </exception>
        public Uri Uri
        {
            get
            {
                if (Src == null) return null;
                if (!Uri.IsWellFormedUriString(Src, UriKind.RelativeOrAbsolute))
                {
                    throw new exception.InvalidUriException(String.Format(
                                                                "The src value '{0}' is not a well-formed Uri", Src));
                }
                return new Uri(Presentation.RootUri, Src);
            }
        }

        #endregion

        #region IValueEquatable<Media> Members


        public override bool ValueEquals(WithPresentation other)
        {
            if (!base.ValueEquals(other))
            {
                return false;
            }

            ExternalTextMedia otherz = other as ExternalTextMedia;
            if (otherz == null)
            {
                return false;
            }

            if (Src != otherz.Src)
            {
                return false;
            }

            return true;
        }

        #endregion


        
    }
}