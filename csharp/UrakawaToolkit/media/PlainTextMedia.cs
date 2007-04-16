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
	public class PlainTextMedia : ITextMedia, ILocated
	{
		/// <summary>
		/// Constructor setting the <see cref="IMediaFactory"/> that created the instance
		/// </summary>
		/// <param name="fact">The creating instance</param>
		protected internal PlainTextMedia(IMediaFactory fact)
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

		#region IMedia Members


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
		/// Determines if <see cref="this"/> is a sequence media (which it is not)
		/// </summary>
		/// <returns><c>false</c></returns>
		public bool isSequence()
		{
			return false;
		}

		/// <summary>
		/// Gets the <see cref="MediaType"/> of <c>this</c> (which is <see cref="MediaType.TEXT"/>)
		/// </summary>
		/// <returns><see cref="MediaType.TEXT"/></returns>
		public MediaType getMediaType()
		{
			return MediaType.TEXT;
		}

		IMedia IMedia.copy()
		{
			return copy();
		}

		/// <summary>
		/// Creates a copy of <c>this</c>
		/// </summary>
		/// <returns>The copy</returns>
		public PlainTextMedia copy()
		{
			IMedia oCopy = getMediaFactory().createMedia(getXukLocalName(), getXukNamespaceUri());
			if (!(oCopy is PlainTextMedia))
			{
				throw new exception.FactoryCanNotCreateTypeException(
					"The Mediafactory of the PlainTextMedia can not create a PlainTextMedia");
			}
			PlainTextMedia theCopy = (PlainTextMedia)oCopy;
			theCopy.setSrc(getSrc());
			return theCopy;
		}

		#endregion
	
		#region IXUKAble members

		/// <summary>
		/// Reads the <see cref="PlainTextMedia"/> from a PlainTextMedia xuk element
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
		/// <returns>A <see cref="bool"/> indicating if the read was succesful</returns>
		public bool XukIn(XmlReader source)
		{
			if (source == null)
			{
				throw new exception.MethodParameterIsNullException("Can not XukIn from an null source XmlReader");
			}
			if (source.NodeType != XmlNodeType.Element) return false;
			if (!XukInAttributes(source)) return false;
			if (!source.IsEmptyElement)
			{
				while (source.Read())
				{
					if (source.NodeType == XmlNodeType.Element)
					{
						if (!XukInChild(source)) return false;
					}
					else if (source.NodeType == XmlNodeType.EndElement)
					{
						break;
					}
					if (source.EOF) break;
				}
			}
			return true;
		}

		/// <summary>
		/// Reads the attributes of a PlainTextMedia xuk element.
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
		/// <returns>A <see cref="bool"/> indicating if the attributes was succefully read</returns>
		protected virtual bool XukInAttributes(XmlReader source)
		{
			// Read known attributes


			return true;
		}

		/// <summary>
		/// Reads a child of a PlainTextMedia xuk element. 
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
		/// <returns>A <see cref="bool"/> indicating if the child was succefully read</returns>
		protected virtual bool XukInChild(XmlReader source)
		{
			bool readItem = false;
			// Read known children, when read set readItem to true


			if (!(readItem || source.IsEmptyElement))
			{
				source.ReadSubtree().Close();//Read past unknown child 
			}
			return true;
		}

		/// <summary>
		/// Write a PlainTextMedia element to a XUK file representing the <see cref="PlainTextMedia"/> instance
		/// </summary>
		/// <param localName="destination">The destination <see cref="XmlWriter"/></param>
		/// <returns>A <see cref="bool"/> indicating if the write was succesful</returns>
		public bool XukOut(XmlWriter destination)
		{
			if (destination == null)
			{
				throw new exception.MethodParameterIsNullException(
					"Can not XukOut to a null XmlWriter");
			}
			destination.WriteStartElement(getXukLocalName(), getXukNamespaceUri());
			if (!XukOutAttributes(destination)) return false;
			if (!XukOutChildren(destination)) return false;
			destination.WriteEndElement();
			return true;
		}

		/// <summary>
		/// Writes the attributes of a PlainTextMedia element
		/// </summary>
		/// <param name="destination">The destination <see cref="XmlWriter"/></param>
		/// <returns>A <see cref="bool"/> indicating if the write was succesful</returns>
		protected virtual bool XukOutAttributes(XmlWriter destination)
		{
			return true;
		}

		/// <summary>
		/// Write the child elements of a PlainTextMedia element.
		/// </summary>
		/// <param name="destination">The destination <see cref="XmlWriter"/></param>
		/// <returns>A <see cref="bool"/> indicating if the write was succesful</returns>
		protected virtual bool XukOutChildren(XmlWriter destination)
		{
			// Write children
			return true;
		}

		/// <summary>
		/// Gets the local name part of the QName representing a <see cref="PlainTextMedia"/> in Xuk
		/// </summary>
		/// <returns>The local name part</returns>
		public virtual string getXukLocalName()
		{
			return this.GetType().Name;
		}

		/// <summary>
		/// Gets the namespace uri part of the QName representing a <see cref="PlainTextMedia"/> in Xuk
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
		public bool ValueEquals(IMedia other)
		{
			if (!(other is PlainTextMedia)) return false;
			PlainTextMedia oPTM = (PlainTextMedia)other;
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

		/// <summary>
		/// Gets the text of the <c>this</c>
		/// </summary>
		/// <returns>The text - if the plaintext file could not be found, <see cref="String.Empty"/> is returned</returns>
		public string getText()
		{
			try
			{
				Uri src = new Uri(getMediaFactory().getPresentation().getBaseUri(), getSrc());
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
