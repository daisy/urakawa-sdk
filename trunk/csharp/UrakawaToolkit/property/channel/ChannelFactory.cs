using System;
using System.Xml;
using urakawa.core;
using urakawa.xuk;

namespace urakawa.property.channel
{
	/// <summary>
	/// The actual implementation to be implemented by the implementation team ;)
	/// All method bodies must be completed for realizing the required business logic.
	/// -
	/// This is the DEFAULT implementation for the API/Toolkit:
	/// end-users should feel free to use this class as such,
	/// or they can sub-class it in order to specialize the instance creation process.
	/// -
	/// In addition, an end-user has the possibility to implement the
	/// singleton factory pattern, so that only one instance of the factory
	/// is used throughout the application life
	/// (by adding a method like "static Factory getFactory()").
	/// <seealso cref="Channel"/> 
	/// <seealso cref="Channel"/>
	/// <seealso cref="ChannelsManager"/>
	/// </summary>
	public class ChannelFactory : WithPresentation, IXukAble
	{

    /// <summary>
    /// Default constructor
    /// </summary>
		internal protected ChannelFactory()
		{
    }



    #region ChannelFactory Members
		/// <summary>
		/// Gets the <see cref="ChannelsManager"/> assigned the <see cref="Channel"/>s created
		/// by the <see cref="ChannelFactory"/>
		/// </summary>
		/// <returns>The <see cref="ChannelsManager"/></returns>
		public ChannelsManager getChannelsManager()
		{
			return getPresentation().getChannelsManager();
		}

		/// <summary>
		/// Creates a new <see cref="Channel"/> matching a given QName.
		/// </summary>
		/// <param name="localName">The local part of the QName</param>
		/// <param name="namespaceUri">The namespace uri part of the QName</param>
		/// <returns>The created <see cref="Channel"/> or <c>null</c> is the given QName is not supported</returns>
		/// <remarks>
		/// The only supported QName is <c><see cref="urakawa.ToolkitSettings.XUK_NS"/>:Channel</c> which matches <see cref="Channel"/>
		/// </remarks>
		public virtual Channel createChannel(string localName, string namespaceUri)
		{
			if (namespaceUri == urakawa.ToolkitSettings.XUK_NS)
			{
				if (localName==typeof(Channel).Name)
				{
					return new Channel(getChannelsManager());
				}
				else if (localName == typeof(AudioChannel).Name)
				{
					return new AudioChannel(getChannelsManager());
				}
				else if (localName == typeof(TextChannel).Name)
				{
					return new TextChannel(getChannelsManager());
				}
			}
			return null;
		}

		/// <summary>
		/// Creates a <see cref="Channel"/> instance
		/// </summary>
		/// <returns>The instance</returns>
		public virtual Channel createChannel()
		{
			return createChannel("Channel", urakawa.ToolkitSettings.XUK_NS); 
		}

		#endregion

		
		#region IXUKAble members

		/// <summary>
		/// Reads the <see cref="ChannelFactory"/> from a ChannelFactory xuk element
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
				throw new exception.XukException("Can not read ChannelFactory from a non-element node");
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
					String.Format("An exception occured during XukIn of ChannelFactory: {0}", e.Message),
					e);
			}
		}

		/// <summary>
		/// Reads the attributes of a ChannelFactory xuk element.
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
		protected virtual void XukInAttributes(XmlReader source)
		{
		}

		/// <summary>
		/// Reads a child of a ChannelFactory xuk element. 
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
		/// Write a ChannelFactory element to a XUK file representing the <see cref="ChannelFactory"/> instance
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
					String.Format("An exception occured during XukOut of ChannelFactory: {0}", e.Message),
					e);
			}
		}

		/// <summary>
		/// Writes the attributes of a ChannelFactory element
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
		/// Write the child elements of a ChannelFactory element.
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
		/// Gets the local name part of the QName representing a <see cref="ChannelFactory"/> in Xuk
		/// </summary>
		/// <returns>The local name part</returns>
		public virtual string getXukLocalName()
		{
			return this.GetType().Name;
		}

		/// <summary>
		/// Gets the namespace uri part of the QName representing a <see cref="ChannelFactory"/> in Xuk
		/// </summary>
		/// <returns>The namespace uri part</returns>
		public virtual string getXukNamespaceUri()
		{
			return urakawa.ToolkitSettings.XUK_NS;
		}

		#endregion

	}
}
