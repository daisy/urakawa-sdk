using System;
using System.Xml;
using System.Collections.Generic;
using urakawa.core;
using urakawa.core.events;
using urakawa.core.property;
using urakawa.properties.channel;
using urakawa.properties.xml;
using urakawa.media;
using urakawa.media.data;

namespace urakawa
{
	/// <summary>
	/// Default implementation of interface <see cref="Presentation"/>
	/// </summary>
	public class Presentation : ICorePresentation, IMediaDataPresentation, IChannelPresentation, IXmlPresentation, IValueEquatable<Presentation>
	{
		/// <summary>
		/// Constructor - initializes the presentation with a given base <see cref="Uri"/>
		/// </summary>
		/// <param name="bUri">The given base uri</param>
		public Presentation(Uri bUri) 
			: this(bUri, null, null, null, null, null, null, null)
		{
		}

		/// <summary>
		/// Constructor setting given factories and managers
		/// </summary>
		/// <param name="bUri">The base uri of the presentation</param>
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
			Uri bUri,
			CoreNodeFactory coreNodeFact, PropertyFactory propFact, 
			ChannelFactory chFact, ChannelsManager chMgr, IMediaFactory mediaFact,
			IMediaDataManager mediaDataMngr, IDataProviderManager dataProvMngr
			)
		{
			setBaseUri(bUri);
			//Replace nulls with defaults
			if (coreNodeFact == null) coreNodeFact = new CoreNodeFactory();
			if (propFact == null) propFact = new PropertyFactory();
			if (chFact == null) chFact = new ChannelFactory();
			if (chMgr == null) chMgr = new ChannelsManager();
			if (mediaFact == null) mediaFact = new urakawa.media.MediaFactory();
			if (mediaDataMngr == null) mediaDataMngr = new urakawa.media.data.MediaDataManager();
			if (dataProvMngr == null) dataProvMngr = new urakawa.media.data.FileDataProviderManager("Data");

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

		private CoreNodeFactory mCoreNodeFactory;
		private PropertyFactory mPropertyFactory;
		private ChannelFactory mChannelFactory;
		private ChannelsManager mChanelsManager;
		private IMediaFactory mMediaFactory;
		private IMediaDataManager mMediaDataManager;
		private IDataProviderManager mDataProviderManager;
		private CoreNode mRootNode;
		private Uri mBaseUri;

		
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
		/// Reads the root <see cref="CoreNode"/> of <c>this</c> from a <c>mRootNode</c> xuk xml element
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
						CoreNode newRoot = getCoreNodeFactory().createNode(source.LocalName, source.NamespaceURI);
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
			destination.WriteStartElement("mMediaDataManager", ToolkitSettings.XUK_NS);
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
		/// Gets the root <see cref="CoreNode"/> of <c>this</c>
		/// </summary>
		/// <returns>The root</returns>
		public CoreNode getRootNode()
		{
			return mRootNode;
		}

		/// <summary>
		/// Sets the root <see cref="CoreNode"/> of <c>this</c>
		/// </summary>
		/// <param name="newRoot">The new root - a <c>null</c> value is allowed</param>
		/// <remarks>If the new root <see cref="CoreNode"/> has a parent it is detached</remarks>
		public void setRootNode(CoreNode newRoot)
		{
			if (newRoot != null)
			{
				if (newRoot.getParent() != null) newRoot.detach();
			}
			mRootNode = newRoot;
		}

		/// <summary>
		/// Gets the <see cref="CoreNodeFactory"/> of <c>this</c>
		/// </summary>
		/// <returns>The factory</returns>
		public CoreNodeFactory getCoreNodeFactory()
		{
			return mCoreNodeFactory;
		}

		ICorePropertyFactory ICorePresentation.getPropertyFactory()
		{
			return getPropertyFactory();
		}

		#endregion

		#region Presentation members

		/// <summary>
		/// Gets the <see cref="ICorePropertyFactory"/> of <c>this</c>, 
		/// which is in fact always a <see cref="PropertyFactory"/> instance
		/// </summary>
		/// <returns>The <see cref="PropertyFactory"/></returns>
		public PropertyFactory getPropertyFactory()
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

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public Uri getBaseUri()
		{
			return mBaseUri;
		}

		/// <summary>
		/// Sets the 
		/// </summary>
		/// <param name="newBase">The new base uri</param>
		/// <exception cref="exception.MethodParameterIsNullException">Thrown when the new uri is <c>null</c></exception>
		/// <exception cref="exception.InvalidUriException">Thrown when the given uri is not absolute</exception>
		public void setBaseUri(Uri newBase)
		{
			if (newBase == null)
			{
				throw new exception.MethodParameterIsNullException("The base Uri can not be null");
			}
			if (!newBase.IsAbsoluteUri)
			{
				throw new exception.InvalidUriException("The base uri must be absolute");
			}
			mBaseUri = newBase;
		}

		#endregion

		#region IChannelPresentation Members

		/// <summary>
		/// Gets the <see cref="ChannelFactory"/> of <c>this</c>
		/// </summary>
		/// <returns>The <see cref="ChannelFactory"/></returns>
		public ChannelFactory getChannelFactory()
		{
			return mChannelFactory;
		}

		/// <summary>
		/// Gets the <see cref="ChannelsManager"/> of <c>this</c>
		/// </summary>
		/// <returns>The <see cref="ChannelsManager"/></returns>
		public ChannelsManager getChannelsManager()
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
		/// Gets the factory creating <see cref="urakawa.properties.xml.XmlProperty"/>s 
		/// and <see cref="urakawa.properties.xml.XmlAttribute"/>s used by theese
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

		#region IValueEquatable<Presentation> Members


		/// <summary>
		/// Determines of <c>this</c> has the same value as a given other instance
		/// </summary>
		/// <param name="other">The other instance</param>
		/// <returns>A <see cref="bool"/> indicating the result</returns>
		public bool ValueEquals(Presentation other)
		{
			if (!getChannelsManager().ValueEquals(other.getChannelsManager())) return false;
			if (!getDataProviderManager().ValueEquals(other.getDataProviderManager())) return false;
			if (!getMediaDataManager().ValueEquals(other.getMediaDataManager())) return false;
			if (!getRootNode().ValueEquals(other.getRootNode())) return false;
			return true;
		}

		#endregion

		#region ICoreNodeChangedEventManager Members

		/// <summary>
		/// Event fired whenever a <see cref="CoreNode"/> is changed, i.e. added or removed 
		/// as the child of another <see cref="CoreNode"/>
		/// </summary>
		public event CoreNodeChangedEventHandler coreNodeChanged;

		/// <summary>
		/// Fires the <see cref="coreNodeChanged"/> event
		/// </summary>
		/// <param name="changedNode">The node that changed</param>
		public void notifyCoreNodeChanged(CoreNode changedNode)
		{
			CoreNodeChangedEventHandler d = coreNodeChanged;//Copy to local variable to make thread safe
			if (d != null) d(this, new CoreNodeChangedEventArgs(changedNode));
		}

		/// <summary>
		/// Event fired whenever a <see cref="CoreNode"/> is added as a child of another <see cref="CoreNode"/>
		/// </summary>
		public event CoreNodeAddedEventHandler coreNodeAdded;

		/// <summary>
		/// Fires the <see cref="coreNodeAdded"/> and <see cref="coreNodeChanged"/> events (in that order)
		/// </summary>
		/// <param name="addedNode">The node that has been added</param>
		public void notifyCoreNodeAdded(CoreNode addedNode)
		{
			CoreNodeAddedEventHandler d = coreNodeAdded;//Copy to local variable to make thread safe
			if (d != null) d(this, new CoreNodeAddedEventArgs(addedNode));
		}

		/// <summary>
		/// Event fired whenever a <see cref="CoreNode"/> is added as a child of another <see cref="CoreNode"/>
		/// </summary>
		public event CoreNodeRemovedEventHandler coreNodeRemoved;

		/// <summary>
		/// Fires the <see cref="coreNodeRemoved"/> and <see cref="coreNodeChanged"/> events (in that order)
		/// </summary>
		/// <param name="removedNode">The node that has been removed</param>
		/// <param name="formerParent">The parent node from which the node was removed as a child of</param>
		/// <param name="formerPosition">The position the node previously had of the list of children of it's former parent</param>
		public void notifyCoreNodeRemoved(CoreNode removedNode, CoreNode formerParent, int formerPosition)
		{
			CoreNodeRemovedEventHandler d = coreNodeRemoved;
			if (d != null) d(this, new CoreNodeRemovedEventArgs(removedNode, formerParent, formerPosition));
			notifyCoreNodeChanged(removedNode);
		}

		#endregion


		#region IMediaPresentation Members

		/// <summary>
		/// Gets a list of the <see cref="IMedia"/> used by a given <see cref="CoreNode"/>. 
		/// </summary>
		/// <param name="node">The node</param>
		/// <returns>The list</returns>
		/// <remarks>
		/// An <see cref="IMedia"/> is considered to be used by a <see cref="CoreNode"/> if the media
		/// is linked to the node via. a <see cref="ChannelsProperty"/>
		/// </remarks>
		protected virtual List<IMedia> getListOfMediaUsedByCoreNode(CoreNode node)
		{
			List<IMedia> res = new List<IMedia>();
			foreach (Type t in node.getListOfUsedPropertyTypes())
			{
				Property prop = node.getProperty(t);
				if (prop is ChannelsProperty)
				{
					ChannelsProperty chProp = (ChannelsProperty)prop;
					foreach (Channel ch in chProp.getListOfUsedChannels())
					{
						res.Add(chProp.getMedia(ch));
					}
				}
			}
			return res;
		}

		/// <summary>
		/// Gets the list of <see cref="IMedia"/> used by the <see cref="CoreNode"/> tree of the presentation. 
		/// Remark that a 
		/// </summary>
		/// <returns>The list</returns>
		public List<IMedia> getListOfUsedMedia()
		{
			List<IMedia> res = new List<IMedia>();
			if (getRootNode() != null)
			{
				collectUsedMedia(getRootNode(), res);
			}
			return res;
		}

		private void collectUsedMedia(CoreNode node, List<IMedia> collectedMedia)
		{
			foreach (IMedia m in getListOfMediaUsedByCoreNode(node))
			{
				if (!collectedMedia.Contains(m)) collectedMedia.Add(m);
			}
			for (int i = 0; i < node.getChildCount(); i++)
			{
				collectUsedMedia(node.getChild(i), collectedMedia);
			}
		}

		#endregion
	}
}
