using System;
using System.Xml;
using urakawa.xuk;

namespace urakawa.media
{
	/// <summary>
	/// The media factory will create any media object of MediaType.xxx
	/// </summary>
	public class MediaFactory : WithPresentation, IMediaFactory
	{

		/// <summary>
		/// Default constructor.
		/// </summary>
		internal protected MediaFactory()
		{
		}

		#region IMediaFactory Members

		/// <summary>
		/// Creates an <see cref="IMedia"/> matching a given QName
		/// </summary>
		/// <param name="localName">The local part of the QName</param>
		/// <param name="namespaceUri">The namespace uri part of the QName</param>
		/// <returns>The creates <see cref="IMedia"/> or <c>null</c> is the given QName is not supported</returns>
		public IMedia createMedia(string localName, string namespaceUri)
		{
			if (namespaceUri == urakawa.ToolkitSettings.XUK_NS)
			{
				switch (localName)
				{
					case "ManagedAudioMedia":
						return new data.audio.ManagedAudioMedia(this);
					case "ExternalAudioMedia":
						return new ExternalAudioMedia(this);
					case "ExternalImageMedia":
						return new ExternalImageMedia(this);
					case "ExternalVideoMedia":
						return new ExternalVideoMedia(this);
					case "TextMedia":
						return new TextMedia(this);
					case "SequenceMedia":
						return new SequenceMedia(this);
					case "ExternalTextMedia":
						return new ExternalTextMedia(this);

				}
			}
			return null;
		}

		/// <summary>
		/// Creates a <see cref="data.audio.ManagedAudioMedia"/> which is the default <see cref="IAudioMedia"/> of the factory
		/// </summary>
		/// <returns>The creation</returns>
		public virtual IAudioMedia createAudioMedia()
		{
			IMedia newMedia = createMedia("ManagedAudioMedia", ToolkitSettings.XUK_NS);
			if (newMedia is IAudioMedia) return (IAudioMedia)newMedia;
			throw new exception.FactoryCannotCreateTypeException(
				"The factory unexpectedly could not create a ManagedAudioMedia");
		}

		/// <summary>
		/// Creates a <see cref="TextMedia"/> which is the default <see cref="ITextMedia"/> of the factory
		/// </summary>
		/// <returns>The creation</returns>
		public virtual ITextMedia createTextMedia()
		{
			IMedia newMedia = createMedia("TextMedia", ToolkitSettings.XUK_NS);
			if (newMedia is ITextMedia) return (ITextMedia)newMedia;
			throw new exception.FactoryCannotCreateTypeException(
				"The factory unexpectedly could not create a TextMedia");

		}

		/// <summary>
		/// Creates a <see cref="ExternalImageMedia"/> which is the default <see cref="IImageMedia"/> of the factory
		/// </summary>
		/// <returns>The creation</returns>
		public virtual IImageMedia createImageMedia()
		{
			IMedia newMedia = createMedia("ExternalImageMedia", ToolkitSettings.XUK_NS);
			if (newMedia is IImageMedia) return (IImageMedia)newMedia;
			throw new exception.FactoryCannotCreateTypeException(
				"The factory unexpectedly could not create an ExternalImageMedia");
		}

		/// <summary>
		/// Creates a <see cref="ExternalVideoMedia"/> which is the default <see cref="IVideoMedia"/> of the factory
		/// </summary>
		/// <returns>The creation</returns>
		public virtual IVideoMedia createVideoMedia()
		{
			IMedia newMedia = createMedia("ExternalVideoMedia", ToolkitSettings.XUK_NS);
			if (newMedia is IVideoMedia) return (IVideoMedia)newMedia;
			throw new exception.FactoryCannotCreateTypeException(
				"The factory unexpectedly could not create an ExternalVideoMedia");
		}

		/// <summary>
		/// Creates a <see cref="SequenceMedia"/> which is the default <see cref="SequenceMedia"/> of the factory
		/// </summary>
		/// <returns>The creation</returns>
		public virtual SequenceMedia createSequenceMedia()
		{
			IMedia newMedia = createMedia("SequenceMedia", ToolkitSettings.XUK_NS);
			if (newMedia is SequenceMedia) return (SequenceMedia)newMedia;
			throw new exception.FactoryCannotCreateTypeException(
				"The factory unexpectedly could not create an SequenceMedia");
		}

		#endregion

		
		#region IXUKAble members

		/// <summary>
		/// Reads the <see cref="MediaFactory"/> from a MediaFactory xuk element
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
		public void xukIn(XmlReader source)
		{
			if (source == null)
			{
				throw new exception.MethodParameterIsNullException("Can not xukIn from an null source XmlReader");
			}
			if (source.NodeType != XmlNodeType.Element)
			{
				throw new exception.XukException("Can not read MediaFactory from a non-element node");
			}
			try
			{
				xukInAttributes(source);
				if (!source.IsEmptyElement)
				{
					while (source.Read())
					{
						if (source.NodeType == XmlNodeType.Element)
						{
							xukInChild(source);
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
					String.Format("An exception occured during xukIn of MediaFactory: {0}", e.Message),
					e);
			}
		}

		/// <summary>
		/// Reads the attributes of a MediaFactory xuk element.
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
		protected virtual void xukInAttributes(XmlReader source)
		{
		}

		/// <summary>
		/// Reads a child of a MediaFactory xuk element. 
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
		protected virtual void xukInChild(XmlReader source)
		{
			bool readItem = false;
			// Read known children, when read set readItem to true


			if (!(readItem || source.IsEmptyElement))
			{
				source.ReadSubtree().Close();//Read past unknown child 
			}
		}

		/// <summary>
		/// Write a MediaFactory element to a XUK file representing the <see cref="MediaFactory"/> instance
		/// </summary>
		/// <param name="destination">The destination <see cref="XmlWriter"/></param>
		/// <param name="baseUri">
		/// The base <see cref="Uri"/> used to make written <see cref="Uri"/>s relative, 
		/// if <c>null</c> absolute <see cref="Uri"/>s are written
		/// </param>
		public void xukOut(XmlWriter destination, Uri baseUri)
		{
			if (destination == null)
			{
				throw new exception.MethodParameterIsNullException(
					"Can not xukOut to a null XmlWriter");
			}

			try
			{
				destination.WriteStartElement(getXukLocalName(), getXukNamespaceUri());
				xukOutAttributes(destination, baseUri);
				xukOutChildren(destination, baseUri);
				destination.WriteEndElement();

			}
			catch (exception.XukException e)
			{
				throw e;
			}
			catch (Exception e)
			{
				throw new exception.XukException(
					String.Format("An exception occured during xukOut of MediaFactory: {0}", e.Message),
					e);
			}
		}

		/// <summary>
		/// Writes the attributes of a MediaFactory element
		/// </summary>
		/// <param name="destination">The destination <see cref="XmlWriter"/></param>
		/// <param name="baseUri">
		/// The base <see cref="Uri"/> used to make written <see cref="Uri"/>s relative, 
		/// if <c>null</c> absolute <see cref="Uri"/>s are written
		/// </param>
		protected virtual void xukOutAttributes(XmlWriter destination, Uri baseUri)
		{

		}

		/// <summary>
		/// Write the child elements of a MediaFactory element.
		/// </summary>
		/// <param name="destination">The destination <see cref="XmlWriter"/></param>
		/// <param name="baseUri">
		/// The base <see cref="Uri"/> used to make written <see cref="Uri"/>s relative, 
		/// if <c>null</c> absolute <see cref="Uri"/>s are written
		/// </param>
		protected virtual void xukOutChildren(XmlWriter destination, Uri baseUri)
		{

		}

		/// <summary>
		/// Gets the local name part of the QName representing a <see cref="MediaFactory"/> in Xuk
		/// </summary>
		/// <returns>The local name part</returns>
		public virtual string getXukLocalName()
		{
			return this.GetType().Name;
		}

		/// <summary>
		/// Gets the namespace uri part of the QName representing a <see cref="MediaFactory"/> in Xuk
		/// </summary>
		/// <returns>The namespace uri part</returns>
		public virtual string getXukNamespaceUri()
		{
			return urakawa.ToolkitSettings.XUK_NS;
		}

		#endregion

	}
}
