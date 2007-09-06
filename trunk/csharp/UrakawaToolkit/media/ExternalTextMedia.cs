using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Xml;

namespace urakawa.media
{
	/// <summary>
	/// An implementation of <see cref="ITextMedia"/> based on text storage in an external file/uri
	/// </summary>
	public class ExternalTextMedia : ITextMedia, ILocated
	{
		/// <summary>
		/// Constructor setting the <see cref="IMediaFactory"/> that created the instance
		/// </summary>
		/// <param name="fact">The creating instance</param>
		protected internal ExternalTextMedia(IMediaFactory fact)
		{
			if (fact == null)
			{
				throw new exception.MethodParameterIsNullException(
					"The MediaFactory of the PlainTextMedia can not be null");
			}
			mFactory = fact;
			mSrc = "";
		}


		private IMediaFactory mFactory;
		private string mSrc;
		private string mLanguage;

		#region IMedia Members

		/// <summary>
		/// Sets the language of the external text media
		/// </summary>
		/// <param name="lang">The new language, can be null but not an empty string</param>
		public void setLanguage(string lang)
		{
			if (lang == "")
			{
				throw new exception.MethodParameterIsEmptyStringException(
					"The language can not be an empty string");
			}
			mLanguage = lang;
		}

		/// <summary>
		/// Gets the language of the external text medi
		/// </summary>
		/// <returns>The language</returns>
		public string getLanguage()
		{
			return mLanguage;
		}


		/// <summary>
		/// Gets the <see cref="IMediaFactory"/> associated with <c>this</c>
		/// </summary>
		/// <returns>The factory</returns>
		public IMediaFactory getMediaFactory()
		{
			return mFactory;
		}

		/// <summary>
		/// Determines if <c>this</c> is a continuous media (wich it is not)
		/// </summary>
		/// <returns><c>false</c></returns>
		public bool isContinuous()
		{
			return false;
		}

		/// <summary>
		/// Determines if <c>this</c> is a descrete media (which it is)
		/// </summary>
		/// <returns><c>true</c></returns>
		public bool isDiscrete()
		{
			return true;
		}

		/// <summary>
		/// Determines if <c>this</c>is a sequence media (which it is not)
		/// </summary>
		/// <returns><c>false</c></returns>
		public bool isSequence()
		{
			return false;
		}

		IMedia IMedia.copy()
		{
			return copy();
		}


		/// <summary>
		/// Creates a copy of <c>this</c>
		/// </summary>
		/// <returns>The copy</returns>
		public ExternalTextMedia copy()
		{
			return export(getMediaFactory().getPresentation());
		}

		IMedia IMedia.export(Presentation destPres)
		{
			return export(destPres);
		}

		/// <summary>
		/// Exports the external text media to a destination <see cref="Presentation"/>
		/// </summary>
		/// <param name="destPres">The destination presentation</param>
		/// <returns>The exported external text media</returns>
		public ExternalTextMedia export(Presentation destPres)
		{
			ExternalTextMedia exported = destPres.getMediaFactory().createMedia(
				getXukLocalName(), getXukNamespaceUri()) as ExternalTextMedia;
			if (exported == null)
			{
				throw new exception.FactoryCannotCreateTypeException(String.Format(
					"The MediaFactory cannot create a ExternalTextMedia matching QName {1}:{0}",
					getXukLocalName(), getXukNamespaceUri()));
			}

			exported.setSrc(destPres.getBaseUri().MakeRelativeUri(getUri()).ToString());
			return exported;
		}


		#endregion
	
		#region IXUKAble members

		/// <summary>
		/// Reads the <see cref="ExternalTextMedia"/> from a PlainTextMedia xuk element
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
		/// <exception cref="exception.XukException">Thrown when an Xuk error occurs</exception>
		public void XukIn(XmlReader source)
		{
			if (source == null)
			{
				throw new exception.MethodParameterIsNullException("Can not XukIn from an null source XmlReader");
			}
			if (source.NodeType != XmlNodeType.Element)
			{
				throw new exception.XukException("Can not XukIn an ExternalTextMedia from a non-element node");
			}
			try
			{
				XukInAttributes(source);
				if (!source.IsEmptyElement)
				{
					while (source.Read())
					{
						if (source.NodeType == XmlNodeType.Element)
						{
							XukInChild(source);
						}
						else if (source.NodeType == XmlNodeType.EndElement)
						{
							break;
						}
						if (source.EOF) throw new exception.XukException("Unexpectedly reached EOF");
					}
				}
			}
			catch (exception.XukException e)
			{
				throw e;
			}
			catch (Exception e)
			{
				throw new exception.XukException(
					String.Format("An exception occured during XukIn: {0}", e.Message),
					e);
			}
		}

		/// <summary>
		/// Reads the attributes of a PlainTextMedia xuk element.
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
		protected virtual void XukInAttributes(XmlReader source)
		{
			string lang = source.GetAttribute("Language");
			if (lang != null) lang = lang.Trim();
			if (lang != "") lang = null;
			setLanguage(lang);
		}

		/// <summary>
		/// Reads a child of a PlainTextMedia xuk element. 
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
		protected virtual void XukInChild(XmlReader source)
		{
			bool readItem = false;
			// Read known children, when read set readItem to true


			if (!(readItem || source.IsEmptyElement))
			{
				source.ReadSubtree().Close();//Read past unknown child 
			}
		}

		/// <summary>
		/// Write a PlainTextMedia element to a XUK file representing the <see cref="ExternalTextMedia"/> instance
		/// </summary>
		/// <param localName="destination">The destination <see cref="XmlWriter"/></param>
		/// <exception cref="exception.XukException">Thrown when an Xuk error occurs</exception>
		public void XukOut(XmlWriter destination)
		{
			if (destination == null)
			{
				throw new exception.MethodParameterIsNullException(
					"Can not XukOut to a null XmlWriter");
			}
			try
			{
				destination.WriteStartElement(getXukLocalName(), getXukNamespaceUri());
				XukOutAttributes(destination);
				XukOutChildren(destination);
				destination.WriteEndElement();
			}
			catch (exception.XukException e)
			{
				throw e;
			}
			catch (Exception e)
			{
				throw new exception.XukException(
					String.Format("An exception occured during XukOut: {0}", e.Message),
					e);
			}
		}

		/// <summary>
		/// Writes the attributes of a PlainTextMedia element
		/// </summary>
		/// <param name="destination">The destination <see cref="XmlWriter"/></param>
		protected virtual void XukOutAttributes(XmlWriter destination)
		{
			if (getLanguage() != null) destination.WriteAttributeString("Language", getLanguage());
		}

		/// <summary>
		/// Write the child elements of a PlainTextMedia element.
		/// </summary>
		/// <param name="destination">The destination <see cref="XmlWriter"/></param>
		protected virtual void XukOutChildren(XmlWriter destination)
		{
		}

		/// <summary>
		/// Gets the local name part of the QName representing a <see cref="ExternalTextMedia"/> in Xuk
		/// </summary>
		/// <returns>The local name part</returns>
		public virtual string getXukLocalName()
		{
			return this.GetType().Name;
		}

		/// <summary>
		/// Gets the namespace uri part of the QName representing a <see cref="ExternalTextMedia"/> in Xuk
		/// </summary>
		/// <returns>The namespace uri part</returns>
		public virtual string getXukNamespaceUri()
		{
			return urakawa.ToolkitSettings.XUK_NS;
		}

		#endregion

		#region IValueEquatable<IMedia> Members

		/// <summary>
		/// Determines if <c>this</c> has the same value as a given other <see cref="IMedia"/>
		/// </summary>
		/// <param name="other">The other media</param>
		/// <returns>A <see cref="bool"/> indicating if the values are equal</returns>
		public bool valueEquals(IMedia other)
		{
			if (other == null) return false;
			if (getLanguage() != other.getLanguage()) return false;
			if (GetType() != other.GetType()) return false;
			ExternalTextMedia oPTM = (ExternalTextMedia)other;
			if (getSrc()!=oPTM.getSrc()) return false;
			return true;
		}

		#endregion

		#region ILocated Members

		/// <summary>
		/// Get the src location of the plaintext file
		/// </summary>
		/// <returns>The src location</returns>
		public string getSrc()
		{
			return mSrc;
		}

		/// <summary>
		/// Set the src location of the plaintext file
		/// </summary>
		/// <param name="newSrc">The new src location</param>
		public void setSrc(string newSrc)
		{
			if (newSrc == null)
			{
				throw new exception.MethodParameterIsNullException("The src of an PlainTextMedia can not be null");
			}
			mSrc = newSrc;
		}

		#endregion

		#region ITextMedia Members

		private Uri getUri()
		{
			return new Uri(getMediaFactory().getPresentation().getBaseUri(), getSrc());
		}

		/// <summary>
		/// Gets the text of the <c>this</c>
		/// </summary>
		/// <returns>The text - if the plaintext file could not be found, <see cref="String.Empty"/> is returned</returns>
		public string getText()
		{
			try
			{
				Uri src = getUri();
				WebClient client = new WebClient();
				client.UseDefaultCredentials = true;
				StreamReader rd = new StreamReader(client.OpenRead(src));
				string res = rd.ReadToEnd();
				rd.Close();
				return res;
			}
			catch (Exception e)
			{
				throw new exception.OperationNotValidException(
					String.Format("Could read the text from plaintext file {0}: {1}", getSrc(), e.Message),
					e);
			}
		}

		/// <summary>
		/// Sets the text of <c>this</c>
		/// </summary>
		/// <param name="text">The new text</param>
		/// <exception cref="exception.OperationNotValidException">
		/// Thrown when the text could not be written to the <see cref="Uri"/> (as returned by <see cref="getSrc"/>
		/// using the <see cref="WebClient.UploadData(Uri, byte[])"/> method.
		/// </exception>
		public void setText(string text)
		{
			WebClient client = new WebClient();
			client.UseDefaultCredentials = true;
			setText(text, client);
		}

		/// <summary>
		/// Sets the text of <c>this</c> using given <see cref="ICredentials"/>
		/// </summary>
		/// <param name="text">The new text</param>
		/// <param name="credits">The given credentisals</param>
		public void setText(string text, ICredentials credits)
		{
			WebClient client = new WebClient();
			client.Credentials = credits;
			setText(text, client);
		}

		private void setText(string text, WebClient client)
		{
			try
			{
				Uri src = new Uri(getMediaFactory().getPresentation().getBaseUri(), getSrc());
				byte[] utf8Data = Encoding.UTF8.GetBytes(text);
				client.UploadData(src, utf8Data);
			}
			catch (Exception e)
			{
				throw new exception.OperationNotValidException(
					String.Format("Could not write the text to plaintext file {0}: {1}", getSrc(), e.Message),
					e);
			}
		}

		#endregion
	}
}
