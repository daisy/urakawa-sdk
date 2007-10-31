using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using urakawa.xuk;

namespace urakawa.media.data
{
	/// <summary>
	/// Implementation of <see cref="IDataProviderFactory"/> that supports the creation of <see cref="FileDataProvider"/>s
	/// </summary>
	public class FileDataProviderFactory : WithPresentation, IDataProviderFactory
	{
		/// <summary>
		/// Default constructor
		/// </summary>
		internal protected FileDataProviderFactory() { }

		#region IDataProviderFactory Members

		IDataProviderManager IDataProviderFactory.getDataProviderManager()
		{
			return getDataProviderManager();
		}

		/// <summary>
		/// Gets the <see cref="FileDataProviderManager"/> that owns the factory 
		/// and manages the data providers created by the factory
		/// </summary>
		/// <returns>The manager</returns>
		/// <exception cref="exception.IncompatibleManagerOrFactoryException">
		/// Thrown when <c>getPresentation().getDataProviderManager()</c> is not a <see cref="FileDataProviderManager"/>
		/// </exception>
		public FileDataProviderManager getDataProviderManager()
		{
			FileDataProviderManager mngr = getPresentation().getDataProviderManager() as FileDataProviderManager;
			if (mngr == null)
			{
				throw new exception.IncompatibleManagerOrFactoryException(
					"The DataProviderManager of the Presentation owning a FileDataProviderFactory must be a FileDataProviderManager");
			}
			return mngr;
		}

		/// <summary>
		/// MIME type for MPEG-4 AAC audio
		/// </summary>
		public const string AUDIO_MP4_MIME_TYPE = "audio/mpeg-generic";
		/// <summary>
		/// MIME type for MPEG-1/2 Layer III audio (MP3)
		/// </summary>
		public const string AUDIO_MP3_MIME_TYPE = "audio/mpeg";
		/// <summary>
		/// MIME type for linear PCM RIFF WAVE format audio (wav)
		/// </summary>
		public const string AUDIO_WAV_MIME_TYPE = "audio/x-wav";
		/// <summary>
		/// MIME type for JPEG images
		/// </summary>
		public const string IMAGE_JPG_MIME_TYPE = "image/jpeg";
		/// <summary>
		/// MIME type for PNG images
		/// </summary>
		public const string IMAGE_PNG_MIME_TYPE =  "image/png";
		/// <summary>
		/// MIME type for Scalable Vector Graphics (SVG) images
		/// </summary>
		public const string IMAGE_SVG_MIME_TYPE =  "image/svg+xml";
		/// <summary>
		/// MIME type for Cascading Style Sheets (CSS)
		/// </summary>
		public const string STYLE_CSS_MIME_TYPE =  "text/css";
		/// <summary>
		/// MIME type for plain text
		/// </summary>
		public const string TEXT_PLAIN_MIME_TYPE = "text/plain";


		/// <summary>
		/// Gets the file extension for a given MIME type
		/// </summary>
		/// <param name="mimeType"></param>
		/// <returns>The extension</returns>
		public static string getExtensionFromMimeType(string mimeType)
		{
			string extension;
			switch (mimeType)
			{
				case AUDIO_MP4_MIME_TYPE:
					extension = ".mp4";
					break;
				case AUDIO_MP3_MIME_TYPE:
					extension = ".mp3";
					break;
				case AUDIO_WAV_MIME_TYPE:
					extension = ".wav";
					break;
				case IMAGE_JPG_MIME_TYPE:
					extension = ".jpg";
					break;
				case IMAGE_PNG_MIME_TYPE:
					extension = ".png";
					break;
				case IMAGE_SVG_MIME_TYPE:
					extension = ".svg";
					break;
				case STYLE_CSS_MIME_TYPE:
					extension = ".css";
					break;
				case TEXT_PLAIN_MIME_TYPE:
					extension = ".txt";
					break;
				default:
					extension = ".bin";
					break;
			}
			return extension;
		}

		/// <summary>
		/// Creates a <see cref="FileDataProvider"/> for the given MIME type
		/// </summary>
		/// <param name="mimeType">The given MIME type</param>
		/// <returns>The created data provider</returns>
		public virtual IDataProvider createDataProvider(string mimeType)
		{
			return createFileDataProvider(mimeType);
		}

		/// <summary>
		/// Creates a <see cref="FileDataProvider"/> for the given MIME type
		/// </summary>
		/// <param name="mimeType">The given MIME type</param>
		/// <returns>The created data provider</returns>
		public FileDataProvider createFileDataProvider(string mimeType)
		{
			if (mimeType == null)
			{
				throw new exception.MethodParameterIsNullException("Can not create a FileDataProvider for a null MIME type");
			}
			FileDataProvider newProv;
			newProv = new FileDataProvider(
				getDataProviderManager(),
				getDataProviderManager().getNewDataFileRelPath(getExtensionFromMimeType(mimeType)),
				mimeType);
			return newProv;
		}

		/// <summary>
		/// Creates a data provider for the given mime type of type mathcing the given xuk QName
		/// </summary>
		/// <param name="mimeType">The given MIME type</param>
		/// <param name="xukLocalName">The local name part of the given xuk QName</param>
		/// <param name="xukNamespaceUri">The namespace uri part of the given xuk QName</param>
		/// <returns>The created data provider</returns>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when <paramref name="xukLocalName"/> or <paramref name="xukNamespaceUri"/> is <c>null</c>
		/// </exception>
		public virtual IDataProvider createDataProvider(string mimeType, string xukLocalName, string xukNamespaceUri)
		{
			if (xukLocalName == null || xukNamespaceUri == null)
			{
				throw new exception.MethodParameterIsNullException("No part of the xuk QName can be null");
			}
			if (xukNamespaceUri == ToolkitSettings.XUK_NS)
			{
				switch (xukLocalName)
				{
					case "FileDataProvider":
						return createFileDataProvider(mimeType);
				}
			}
			return null;
		}

		#endregion

		
		#region IXUKAble members

		/// <summary>
		/// Reads the <see cref="FileDataProviderFactory"/> from a FileDataProviderFactory xuk element
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
		public void XukIn(XmlReader source)
		{
			if (source == null)
			{
				throw new exception.MethodParameterIsNullException("Can not XukIn from an null source XmlReader");
			}
			if (source.NodeType != XmlNodeType.Element)
			{
				throw new exception.XukException("Can not read FileDataProviderFactory from a non-element node");
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
					String.Format("An exception occured during XukIn of FileDataProviderFactory: {0}", e.Message),
					e);
			}
		}

		/// <summary>
		/// Reads the attributes of a FileDataProviderFactory xuk element.
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
		protected virtual void XukInAttributes(XmlReader source)
		{
		}

		/// <summary>
		/// Reads a child of a FileDataProviderFactory xuk element. 
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
		/// Write a FileDataProviderFactory element to a XUK file representing the <see cref="FileDataProviderFactory"/> instance
		/// </summary>
		/// <param name="destination">The destination <see cref="XmlWriter"/></param>
		/// <param name="baseUri">
		/// The base <see cref="Uri"/> used to make written <see cref="Uri"/>s relative, 
		/// if <c>null</c> absolute <see cref="Uri"/>s are written
		/// </param>
		public void XukOut(XmlWriter destination, Uri baseUri)
		{
			if (destination == null)
			{
				throw new exception.MethodParameterIsNullException(
					"Can not XukOut to a null XmlWriter");
			}

			try
			{
				destination.WriteStartElement(getXukLocalName(), getXukNamespaceUri());
				XukOutAttributes(destination, baseUri);
				XukOutChildren(destination, baseUri);
				destination.WriteEndElement();

			}
			catch (exception.XukException e)
			{
				throw e;
			}
			catch (Exception e)
			{
				throw new exception.XukException(
					String.Format("An exception occured during XukOut of FileDataProviderFactory: {0}", e.Message),
					e);
			}
		}

		/// <summary>
		/// Writes the attributes of a FileDataProviderFactory element
		/// </summary>
		/// <param name="destination">The destination <see cref="XmlWriter"/></param>
		/// <param name="baseUri">
		/// The base <see cref="Uri"/> used to make written <see cref="Uri"/>s relative, 
		/// if <c>null</c> absolute <see cref="Uri"/>s are written
		/// </param>
		protected virtual void XukOutAttributes(XmlWriter destination, Uri baseUri)
		{

		}

		/// <summary>
		/// Write the child elements of a FileDataProviderFactory element.
		/// </summary>
		/// <param name="destination">The destination <see cref="XmlWriter"/></param>
		/// <param name="baseUri">
		/// The base <see cref="Uri"/> used to make written <see cref="Uri"/>s relative, 
		/// if <c>null</c> absolute <see cref="Uri"/>s are written
		/// </param>
		protected virtual void XukOutChildren(XmlWriter destination, Uri baseUri)
		{

		}

		/// <summary>
		/// Gets the local name part of the QName representing a <see cref="FileDataProviderFactory"/> in Xuk
		/// </summary>
		/// <returns>The local name part</returns>
		public virtual string getXukLocalName()
		{
			return this.GetType().Name;
		}

		/// <summary>
		/// Gets the namespace uri part of the QName representing a <see cref="FileDataProviderFactory"/> in Xuk
		/// </summary>
		/// <returns>The namespace uri part</returns>
		public virtual string getXukNamespaceUri()
		{
			return urakawa.ToolkitSettings.XUK_NS;
		}

		#endregion

	}
}
