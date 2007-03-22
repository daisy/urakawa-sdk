using System;
using System.Xml;
using urakawa.core;
using urakawa.core.property;
using urakawa.properties.channel;
using urakawa.properties.xml;
using urakawa.media;
using urakawa.media.data;

namespace urakawa
{
	/// <summary>
	/// Default implementation of interface <see cref="IPresentation"/>
	/// </summary>
	public class Presentation : IPresentation
	{
		/// <summary>
		/// Default constructor - initializes the
		/// </summary>
		public Presentation(string basePath, string dataDir) 
			: this(null, null, null, null, null, null, new FileDataProviderManager(basePath, dataDir))
		{
		}

		/// <summary>
		/// Constructor setting given factories and managers
		/// </summary>
		/// <param name="coreNodeFact">
		/// The core node factory of the presentation -
		/// if <c>null</c> a newly created <see cref="CoreNodeFactory"/> is used
		/// </param>
		/// <param name="propFact">
		/// The property factory of the presentation -
		/// if <c>null</c> a newly created <see cref="PropertyFactory"/> is used
		/// </param>
		/// <param name="chFact">
		/// The channel factory of the presentation -
		/// if <c>null</c> a newly created <see cref="ChannelFactory"/> is used
		/// </param>
		/// <param name="chMgr">
		/// The channels manager> of the presentation -
		/// if <c>null</c> a newly created <see cref="ChannelsManager"/> is used
		/// </param>
		/// <param name="mediaFact">
		/// The media factory of the presentation -
		/// if <c>null</c> a newly created <see cref="MediaFactory"/> is used
		/// </param>
		/// <param name="mediaDataMngr">
		/// The media data manager of the presentation -
		/// if <c>null</c> a newly created <see cref="MediaDataManager"/> is used
		///	</param>
		///	<param name="dataProvMngr">
		///	The data provider manager of the presentation - 
		///	if <c>null</c> a newly created <see cref="FileDataProviderManager"/> is used</param>
		public Presentation(
			ICoreNodeFactory coreNodeFact, IPropertyFactory propFact, 
			IChannelFactory chFact, IChannelsManager chMgr, IMediaFactory mediaFact,
			IMediaDataManager mediaDataMngr, IDataProviderManager dataProvMngr
			)
		{
			//Replace nulls with defaults
			if (coreNodeFact == null) coreNodeFact = new CoreNodeFactory();
			if (propFact == null) propFact = new PropertyFactory();
			if (chFact == null) chFact = new ChannelFactory();
			if (chMgr == null) chMgr = new ChannelsManager();
			if (mediaFact == null) mediaFact = new urakawa.media.MediaFactory();
			if (mediaDataMngr == null) mediaDataMngr = new urakawa.media.data.MediaDataManager();
			if (dataProvMngr == null) 
			{
				dataProvMngr = new urakawa.media.data.FileDataProviderManager(System.IO.Directory.GetCurrentDirectory(), null);
			}

			//Setup member vars
			mCoreNodeFactory = coreNodeFact;
			mPropertyFactory = propFact;
			mChannelFactory = chFact;
			mChanelsManager = chMgr;
			mMediaFactory = mediaFact;
			mMediaDataManager = mediaDataMngr;
			mDataProviderManager = dataProvMngr;

			//Linkup members to this
			coreNodeFact.setPresentation(this);
			mChannelFactory.setPresentation(this);
			mChanelsManager.setPresentation(this);
			propFact.setPresentation(this);
			mMediaFactory.setPresentation(this);
			mMediaDataManager.setPresentation(this);
			mDataProviderManager.setPresentation(this);

			setRootNode(getCoreNodeFactory().createNode());
		}

		private ICoreNodeFactory mCoreNodeFactory;
		private IPropertyFactory mPropertyFactory;
		private IChannelFactory mChannelFactory;
		private IChannelsManager mChanelsManager;
		private IMediaFactory mMediaFactory;
		private IMediaDataManager mMediaDataManager;
		private IDataProviderManager mDataProviderManager;
		private ICoreNode mRootNode;

		
		#region IXUKAble members

		/// <summary>
		/// Reads the <see cref="Presentation"/> from a Presentation xuk element
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
		/// Reads the attributes of a Presentation xuk element.
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
		/// <returns>A <see cref="bool"/> indicating if the attributes was succefully read</returns>
		protected virtual bool XukInAttributes(XmlReader source)
		{
			// Read known attributes


			return true;
		}

		/// <summary>
		/// Reads an <see cref="xuk.IXukAble"/> instance from one of the children of a xuk element,
		/// more specifically the one with matching Xuk QName
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
		/// <param name="xukAble">The instance to read</param>
		/// <returns>
		/// A <see cref="bool"/> if the instance was successfully read, 
		/// where the read is considered succesfull if no child is found with matching xuk QName
		/// </returns>
		protected bool XukInXukAbleFromChild(XmlReader source, xuk.IXukAble xukAble)
		{
			if (!source.IsEmptyElement)
			{
				while (source.Read())
				{
					if (source.NodeType == XmlNodeType.Element)
					{
						if (source.LocalName == xukAble.getXukLocalName() && source.NamespaceURI == xukAble.getXukNamespaceUri())
						{
							if (!xukAble.XukIn(source)) return false;
						}
						else if (!source.IsEmptyElement)
						{
							source.ReadSubtree().Close();
						}
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
		/// Reads the root <see cref="ICoreNode"/> of <c>this</c> from a <c>mRootNode</c> xuk xml element
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
		/// <returns>A <see cref="bool"/> indicating if the read was succesful</returns>
		/// <remarks>The read is considered succesful even if no valid root node is found</remarks>
		protected bool XukInRootNode(XmlReader source)
		{
			setRootNode(null);
			if (!source.IsEmptyElement)
			{
				while (source.Read())
				{
					if (source.NodeType == XmlNodeType.Element)
					{
						ICoreNode newRoot = getCoreNodeFactory().createNode(source.LocalName, source.NamespaceURI);
						if (newRoot != null)
						{
							if (!newRoot.XukIn(source)) return false;
							setRootNode(newRoot);
						}
						else if (!source.IsEmptyElement)
						{
							source.ReadSubtree().Close();
						}
					}
					if (source.EOF) break;
				}
			}
			return true;
		}

		/// <summary>
		/// Reads a child of a Presentation xuk element. 
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
		/// <returns>A <see cref="bool"/> indicating if the child was succefully read</returns>
		protected virtual bool XukInChild(XmlReader source)
		{
			bool readItem = false;
			if (source.NamespaceURI == ToolkitSettings.XUK_NS)
			{
				readItem = true;
				switch (source.LocalName)
				{
					case "mChannelsManager":
						if (!XukInXukAbleFromChild(source, getChannelsManager())) return false;
						break;
					case "mDataProviderManager":
						if (!XukInXukAbleFromChild(source, getDataProviderManager())) return false;
						break;
					case "mMediaDataManager":
						if (!XukInXukAbleFromChild(source, getMediaDataManager())) return false;
						break;
					case "mRootNode":
						if (!XukInRootNode(source)) return false;
						break;
					default:
						readItem = false;
						break;
				}
			}
			if (!(readItem || source.IsEmptyElement))
			{
				source.ReadSubtree().Close();//Read past unknown child 
			}
			return true;
		}

		/// <summary>
		/// Write a Presentation element to a XUK file representing the <see cref="Presentation"/> instance
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
		/// Writes the attributes of a Presentation element
		/// </summary>
		/// <param name="destination">The destination <see cref="XmlWriter"/></param>
		/// <returns>A <see cref="bool"/> indicating if the write was succesful</returns>
		protected virtual bool XukOutAttributes(XmlWriter destination)
		{
			return true;
		}

		/// <summary>
		/// Write the child elements of a Presentation element.
		/// </summary>
		/// <param name="destination">The destination <see cref="XmlWriter"/></param>
		/// <returns>A <see cref="bool"/> indicating if the write was succesful</returns>
		protected virtual bool XukOutChildren(XmlWriter destination)
		{
			destination.WriteStartElement("mChannelsManager", ToolkitSettings.XUK_NS);
			if (!getChannelsManager().XukOut(destination)) return false;
			destination.WriteEndElement();
			destination.WriteStartElement("mDataProviderManager", ToolkitSettings.XUK_NS);
			if (!getDataProviderManager().XukOut(destination)) return false;
			destination.WriteEndElement();
			destination.WriteStartElement("", ToolkitSettings.XUK_NS);
			if (!getMediaDataManager().XukOut(destination)) return false;
			destination.WriteEndElement();
			destination.WriteStartElement("mRootNode", ToolkitSettings.XUK_NS);
			if (!getRootNode().XukOut(destination)) return false;
			destination.WriteEndElement();
			return true;
		}

		/// <summary>
		/// Gets the local name part of the QName representing a <see cref="Presentation"/> in Xuk
		/// </summary>
		/// <returns>The local name part</returns>
		public virtual string getXukLocalName()
		{
			return this.GetType().Name;
		}

		/// <summary>
		/// Gets the namespace uri part of the QName representing a <see cref="Presentation"/> in Xuk
		/// </summary>
		/// <returns>The namespace uri part</returns>
		public virtual string getXukNamespaceUri()
		{
			return urakawa.ToolkitSettings.XUK_NS;
		}

		#endregion

		#region Old IXUKAble members (commented out)

		///// <summary>
		///// Reads the <see cref="Presentation"/> from a Presentation xuk element
		///// </summary>
		///// <param name="source">The source <see cref="XmlReader"/></param>
		///// <returns>A <see cref="bool"/> indicating if the read was succesful</returns>
		//public bool XukIn(System.Xml.XmlReader source)
		//{
		//  if (source == null)
		//  {
		//    throw new exception.MethodParameterIsNullException("The source xml reader is null");
		//  }
		//  bool bProcessedChannelsManager = false;
		//  if (source.NodeType != XmlNodeType.Element)
		//  {
		//    return false;
		//  }
		//  if (!source.IsEmptyElement)
		//  {
		//    while (source.Read())
		//    {
		//      if (source.NodeType == XmlNodeType.Element)
		//      {
		//        bool handledElement = false;
		//        if (source.NamespaceURI == ToolkitSettings.XUK_NS)
		//        {
		//          switch (source.LocalName)
		//          {
		//            case "mChannelsManager":
		//              bProcessedChannelsManager = true;
		//              handledElement = true;
		//              if (!XukInChannelsManager(source)) return false;
		//              break;
		//            case "mRootNode":
		//              handledElement = true;
		//              if (!XukInRootNode(source)) return false;
		//              break;
		//          }
		//        }
		//        if (!handledElement)
		//        {
		//          if (!source.IsEmptyElement)
		//          {
		//            //Read past subtree
		//            source.ReadSubtree().Close();
		//          }
		//        }
		//      }
		//      else if (source.NodeType == XmlNodeType.EndElement)
		//      {
		//        break;
		//      }
		//      if (source.EOF) break;
		//    }
		//  }
		//  return bProcessedChannelsManager;
		//}

		///// <summary>
		///// Write a Presentation element to a XUK file representing the <see cref="Presentation"/> instance
		///// </summary>
		///// <param name="destination">The destination <see cref="XmlWriter"/></param>
		///// <returns>A <see cref="bool"/> indicating if the write was succesful</returns>
		//public bool XukOut(System.Xml.XmlWriter destination)
		//{
		//  if (destination == null)
		//  {
		//    throw new exception.MethodParameterIsNullException("Xml Writer is null");
		//  }
		//  destination.WriteStartElement("Presentation", urakawa.ToolkitSettings.XUK_NS);
		//  destination.WriteStartElement("mChannelsManager", urakawa.ToolkitSettings.XUK_NS);
		//  if (!getChannelsManager().XukOut(destination)) return false;
		//  destination.WriteEndElement();
		//  destination.WriteStartElement("mRootNode", urakawa.ToolkitSettings.XUK_NS);
		//  if (!getRootNode().XukOut(destination)) return false;
		//  destination.WriteEndElement();
		//  destination.WriteEndElement();
		//  return true;
		//}

		
		///// <summary>
		///// Gets the local localName part of the QName representing a <see cref="Presentation"/> in Xuk
		///// </summary>
		///// <returns>The local localName part</returns>
		//public string getXukLocalName()
		//{
		//  return this.GetType().Name;
		//}

		///// <summary>
		///// Gets the namespace uri part of the QName representing a <see cref="Presentation"/> in Xuk
		///// </summary>
		///// <returns>The namespace uri part</returns>
		//public string getXukNamespaceUri()
		//{
		//  return urakawa.ToolkitSettings.XUK_NS;
		//}

		#endregion

		#region ICorePresentation Members

		/// <summary>
		/// Gets the root <see cref="ICoreNode"/> of <c>this</c>
		/// </summary>
		/// <returns>The root</returns>
		public ICoreNode getRootNode()
		{
			return mRootNode;
		}

		/// <summary>
		/// Sets the root <see cref="ICoreNode"/> of <c>this</c>
		/// </summary>
		/// <param name="newRoot">The new root - a <c>null</c> value is allowed</param>
		/// <remarks>If the new root <see cref="ICoreNode"/> has a parent it is detached</remarks>
		public void setRootNode(ICoreNode newRoot)
		{
			if (newRoot != null)
			{
				if (newRoot.getParent() != null) newRoot.detach();
			}
			mRootNode = newRoot;
		}

		/// <summary>
		/// Gets the <see cref="ICoreNodeFactory"/> of <c>this</c>
		/// </summary>
		/// <returns>The factory</returns>
		public ICoreNodeFactory getCoreNodeFactory()
		{
			return mCoreNodeFactory;
		}

		ICorePropertyFactory ICorePresentation.getPropertyFactory()
		{
			return getPropertyFactory();
		}

		#endregion

		#region IPresentation members

		/// <summary>
		/// Gets the <see cref="ICorePropertyFactory"/> of <c>this</c>, 
		/// which is in fact always a <see cref="IPropertyFactory"/> instance
		/// </summary>
		/// <returns>The <see cref="IPropertyFactory"/></returns>
		public IPropertyFactory getPropertyFactory()
		{
			return mPropertyFactory;
		}

		#endregion

		#region IMediaPresentation Members

		/// <summary>
		/// Gets the <see cref="urakawa.media.IMediaFactory"/> of <c>this</c>
		/// </summary>
		/// <returns>The <see cref="urakawa.media.IMediaFactory"/></returns>
		public urakawa.media.IMediaFactory getMediaFactory()
		{
			return mMediaFactory;
		}

		#endregion

		#region IChannelPresentation Members

		/// <summary>
		/// Gets the <see cref="IChannelFactory"/> of <c>this</c>
		/// </summary>
		/// <returns>The <see cref="IChannelFactory"/></returns>
		public IChannelFactory getChannelFactory()
		{
			return mChannelFactory;
		}

		/// <summary>
		/// Gets the <see cref="IChannelsManager"/> of <c>this</c>
		/// </summary>
		/// <returns>The <see cref="IChannelsManager"/></returns>
		public IChannelsManager getChannelsManager()
		{
			return mChanelsManager;
		}

		/// <summary>
		/// Gets the <see cref="IChannelsPropertyFactory"/> of <c>this</c>
		/// </summary>
		/// <returns>The <see cref="IChannelsPropertyFactory"/></returns>
		IChannelsPropertyFactory IChannelPresentation.getPropertyFactory()
		{
			return mPropertyFactory;
		}

		#endregion

		#region IXmlPresentation Members

		/// <summary>
		/// Gets the factory creating <see cref="urakawa.properties.xml.IXmlProperty"/>s 
		/// and <see cref="urakawa.properties.xml.IXmlAttribute"/>s used by theese
		/// </summary>
		/// <returns>The factory</returns>
		IXmlPropertyFactory IXmlPresentation.getPropertyFactory()
		{
			return mPropertyFactory;
		}

		#endregion

		#region IMediaDataPresentation Members

		/// <summary>
		/// Gets the manager for <see cref="urakawa.media.data.MediaData"/>
		/// </summary>
		/// <returns>The media data manager</returns>
		public urakawa.media.data.IMediaDataManager getMediaDataManager()
		{
			return mMediaDataManager;
		}

		/// <summary> 
		/// Gets the factory for <see cref="urakawa.media.data.MediaData"/>.
		/// Convenience for <c>getMediaDataManager().getMediaDataFactory()</c>
		/// </summary>
		/// <returns>The media data factory</returns>
		public urakawa.media.data.IMediaDataFactory getMediaDataFactory()
		{
			return mMediaDataManager.getMediaDataFactory();
		}

		/// <summary>
		/// Gets the manager for <see cref="IDataProvider"/>s
		/// </summary>
		/// <returns>The data provider manager</returns>
		public urakawa.media.data.IDataProviderManager getDataProviderManager()
		{
			return mDataProviderManager;
		}

		#endregion
	}
}
