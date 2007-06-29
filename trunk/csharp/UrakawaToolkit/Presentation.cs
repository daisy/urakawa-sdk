using System;
using System.Xml;
using System.Collections.Generic;
using urakawa.core;
using urakawa.core.events;
using urakawa.property;
using urakawa.property.channel;
using urakawa.property.xml;
using urakawa.media;
using urakawa.media.data;

namespace urakawa
{
	/// <summary>
	/// Default implementation of interface <see cref="Presentation"/>
	/// </summary>
	public class Presentation : ITreePresentation, IMediaDataPresentation, IChannelPresentation, IXmlPresentation, IValueEquatable<Presentation>
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
		/// <param name="treeNodeFact">
		/// The core node factory of the presentation -
		/// if <c>null</c> a newly created <see cref="TreeNodeFactory"/> is used
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
			TreeNodeFactory treeNodeFact, PropertyFactory propFact, 
			ChannelFactory chFact, ChannelsManager chMgr, IMediaFactory mediaFact,
			MediaDataManager mediaDataMngr, IDataProviderManager dataProvMngr
			)
		{
			setBaseUri(bUri);
			//Replace nulls with defaults
			if (treeNodeFact == null) treeNodeFact = new TreeNodeFactory();
			if (propFact == null) propFact = new PropertyFactory();
			if (chFact == null) chFact = new ChannelFactory();
			if (chMgr == null) chMgr = new ChannelsManager();
			if (mediaFact == null) mediaFact = new urakawa.media.MediaFactory();
			if (mediaDataMngr == null) mediaDataMngr = new urakawa.media.data.MediaDataManager();
			if (dataProvMngr == null) dataProvMngr = new urakawa.media.data.FileDataProviderManager("Data");

			//Setup member vars
			mTreeNodeFactory = treeNodeFact;
			mPropertyFactory = propFact;
			mChannelFactory = chFact;
			mChanelsManager = chMgr;
			mMediaFactory = mediaFact;
			mMediaDataManager = mediaDataMngr;
			mDataProviderManager = dataProvMngr;

			//Linkup members to this
			treeNodeFact.setPresentation(this);
			mChannelFactory.setPresentation(this);
			mChanelsManager.setPresentation(this);
			propFact.setPresentation(this);
			mMediaFactory.setPresentation(this);
			mMediaDataManager.setPresentation(this);
			mDataProviderManager.setPresentation(this);

			setRootNode(getTreeNodeFactory().createNode());
		}

		private TreeNodeFactory mTreeNodeFactory;
		private PropertyFactory mPropertyFactory;
		private ChannelFactory mChannelFactory;
		private ChannelsManager mChanelsManager;
		private IMediaFactory mMediaFactory;
		private MediaDataManager mMediaDataManager;
		private IDataProviderManager mDataProviderManager;
		private TreeNode mRootNode;
		private Uri mBaseUri;

		
		#region IXUKAble members

		/// <summary>
		/// Reads the <see cref="Presentation"/> from a Presentation xuk element
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
				throw new exception.XukException("Can not read Presentation from a non-element node");
			}
			try
			{
				if (source.BaseURI != null && source.BaseURI != "")
				{
					setBaseUri(new Uri(source.BaseURI));
				}
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
					String.Format("An exception occured during XukIn of Presentation: {0}", e.Message),
					e);
			}
		}

		/// <summary>
		/// Reads the attributes of a Presentation xuk element.
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
		protected virtual void XukInAttributes(XmlReader source)
		{
			// Read known attributes
		}

		/// <summary>
		/// Reads an <see cref="xuk.IXukAble"/> instance from one of the children of a xuk element,
		/// more specifically the one with matching Xuk QName
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
		/// <param name="xukAble">The instance to read</param>
		protected void XukInXukAbleFromChild(XmlReader source, xuk.IXukAble xukAble)
		{
			if (!source.IsEmptyElement)
			{
				while (source.Read())
				{
					if (source.NodeType == XmlNodeType.Element)
					{
						if (source.LocalName == xukAble.getXukLocalName() && source.NamespaceURI == xukAble.getXukNamespaceUri())
						{
							xukAble.XukIn(source);
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
					if (source.EOF) throw new exception.XukException("Unexpectedly reached EOF");
				}
			}
		}

		/// <summary>
		/// Reads the root <see cref="TreeNode"/> of <c>this</c> from a <c>mRootNode</c> xuk xml element
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
		/// <remarks>The read is considered succesful even if no valid root node is found</remarks>
		protected void XukInRootNode(XmlReader source)
		{
			setRootNode(null);
			if (!source.IsEmptyElement)
			{
				while (source.Read())
				{
					if (source.NodeType == XmlNodeType.Element)
					{
						TreeNode newRoot = getTreeNodeFactory().createNode(source.LocalName, source.NamespaceURI);
						if (newRoot != null)
						{
							newRoot.XukIn(source);
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
					if (source.EOF) throw new exception.XukException("Unexpectedly reached EOF");
				}
			}
		}

		/// <summary>
		/// Reads a child of a Presentation xuk element. 
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
		protected virtual void XukInChild(XmlReader source)
		{
			bool readItem = false;
			if (source.NamespaceURI == ToolkitSettings.XUK_NS)
			{
				readItem = true;
				switch (source.LocalName)
				{
					case "mChannelsManager":
						XukInXukAbleFromChild(source, getChannelsManager());
						break;
					case "mDataProviderManager":
						XukInXukAbleFromChild(source, getDataProviderManager());
						break;
					case "mMediaDataManager":
						XukInXukAbleFromChild(source, getMediaDataManager());
						break;
					case "mRootNode":
						XukInRootNode(source);
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
		}

		/// <summary>
		/// Write a Presentation element to a XUK file representing the <see cref="Presentation"/> instance
		/// </summary>
		/// <param localName="destination">The destination <see cref="XmlWriter"/></param>
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
					String.Format("An exception occured during XukOut of Presentation: {0}", e.Message),
					e);
			}
		}

		/// <summary>
		/// Writes the attributes of a Presentation element
		/// </summary>
		/// <param name="destination">The destination <see cref="XmlWriter"/></param>
		/// <returns>A <see cref="bool"/> indicating if the write was succesful</returns>
		protected virtual void XukOutAttributes(XmlWriter destination)
		{
		}

		/// <summary>
		/// Write the child elements of a Presentation element.
		/// </summary>
		/// <param name="destination">The destination <see cref="XmlWriter"/></param>
		protected virtual void XukOutChildren(XmlWriter destination)
		{
			destination.WriteStartElement("mChannelsManager", ToolkitSettings.XUK_NS);
			getChannelsManager().XukOut(destination);
			destination.WriteEndElement();
			destination.WriteStartElement("mDataProviderManager", ToolkitSettings.XUK_NS);
			getDataProviderManager().XukOut(destination);
			destination.WriteEndElement();
			destination.WriteStartElement("mMediaDataManager", ToolkitSettings.XUK_NS);
			getMediaDataManager().XukOut(destination);
			destination.WriteEndElement();
			destination.WriteStartElement("mRootNode", ToolkitSettings.XUK_NS);
			getRootNode().XukOut(destination);
			destination.WriteEndElement();
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

		#region ITreePresentation Members

		/// <summary>
		/// Gets the root <see cref="TreeNode"/> of <c>this</c>
		/// </summary>
		/// <returns>The root</returns>
		public TreeNode getRootNode()
		{
			return mRootNode;
		}

		/// <summary>
		/// Sets the root <see cref="TreeNode"/> of <c>this</c>
		/// </summary>
		/// <param name="newRoot">The new root - a <c>null</c> value is allowed</param>
		/// <remarks>If the new root <see cref="TreeNode"/> has a parent it is detached</remarks>
		public void setRootNode(TreeNode newRoot)
		{
			if (newRoot != null)
			{
				if (newRoot.getParent() != null) newRoot.detach();
			}
			mRootNode = newRoot;
		}

		/// <summary>
		/// Gets the <see cref="TreeNodeFactory"/> of <c>this</c>
		/// </summary>
		/// <returns>The factory</returns>
		public TreeNodeFactory getTreeNodeFactory()
		{
			return mTreeNodeFactory;
		}

		IGenericPropertyFactory ITreePresentation.getPropertyFactory()
		{
			return getPropertyFactory();
		}

		#endregion

		#region Presentation members

		/// <summary>
		/// Gets the <see cref="IGenericPropertyFactory"/> of <c>this</c>, 
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
			Uri prev = mBaseUri;
			mBaseUri = newBase;
			if (prev == null)
			{
				notifyBaseUriChanged(null);
			}
			else if (prev.AbsoluteUri != mBaseUri.AbsoluteUri)
			{
				notifyBaseUriChanged(prev);
			}
		}

		/// <summary>
		/// Fired when the base <see cref="Uri"/> has changed
		/// </summary>
		public event BaseUriChangedEventHandler BaseUriChanged;

		private void notifyBaseUriChanged(Uri prevUri)
		{
			BaseUriChangedEventHandler d = BaseUriChanged;
			if (d!=null) d(this, new BaseUriChangedEventArgs(prevUri));
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
		/// Gets the factory creating <see cref="urakawa.property.xml.XmlProperty"/>s 
		/// and <see cref="urakawa.property.xml.XmlAttribute"/>s used by theese
		/// </summary>
		/// <returns>The factory</returns>
		IXmlPropertyFactory IXmlPresentation.getPropertyFactory()
		{
			return mPropertyFactory;
		}

		#endregion

		#region MediaDataPresentation Members

		/// <summary>
		/// Gets the manager for <see cref="urakawa.media.data.MediaData"/>
		/// </summary>
		/// <returns>The media data manager</returns>
		public urakawa.media.data.MediaDataManager getMediaDataManager()
		{
			return mMediaDataManager;
		}

		/// <summary> 
		/// Gets the factory for <see cref="urakawa.media.data.MediaData"/>.
		/// Convenience for <c>getMediaDataManager().getMediaDataFactory()</c>
		/// </summary>
		/// <returns>The media data factory</returns>
		public urakawa.media.data.MediaDataFactory getMediaDataFactory()
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

		#region ITreeNodeChangedEventManager Members

		/// <summary>
		/// Event fired whenever a <see cref="TreeNode"/> is changed, i.e. added or removed 
		/// as the child of another <see cref="TreeNode"/>
		/// </summary>
		public event TreeNodeChangedEventHandler coreNodeChanged;

		/// <summary>
		/// Fires the <see cref="coreNodeChanged"/> event
		/// </summary>
		/// <param name="changedNode">The node that changed</param>
		public void notifyTreeNodeChanged(TreeNode changedNode)
		{
			TreeNodeChangedEventHandler d = coreNodeChanged;//Copy to local variable to make thread safe
			if (d != null) d(this, new TreeNodeChangedEventArgs(changedNode));
		}

		/// <summary>
		/// Event fired whenever a <see cref="TreeNode"/> is added as a child of another <see cref="TreeNode"/>
		/// </summary>
		public event TreeNodeAddedEventHandler treeNodeAdded;

		/// <summary>
		/// Fires the <see cref="treeNodeAdded"/> and <see cref="coreNodeChanged"/> events (in that order)
		/// </summary>
		/// <param name="addedNode">The node that has been added</param>
		public void notifyTreeNodeAdded(TreeNode addedNode)
		{
			TreeNodeAddedEventHandler d = treeNodeAdded;//Copy to local variable to make thread safe
			if (d != null) d(this, new TreeNodeAddedEventArgs(addedNode));
		}

		/// <summary>
		/// Event fired whenever a <see cref="TreeNode"/> is added as a child of another <see cref="TreeNode"/>
		/// </summary>
		public event TreeNodeRemovedEventHandler treeNodeRemoved;

		/// <summary>
		/// Fires the <see cref="treeNodeRemoved"/> and <see cref="coreNodeChanged"/> events (in that order)
		/// </summary>
		/// <param name="removedNode">The node that has been removed</param>
		/// <param name="formerParent">The parent node from which the node was removed as a child of</param>
		/// <param name="formerPosition">The position the node previously had of the list of children of it's former parent</param>
		public void notifyTreeNodeRemoved(TreeNode removedNode, TreeNode formerParent, int formerPosition)
		{
			TreeNodeRemovedEventHandler d = treeNodeRemoved;
			if (d != null) d(this, new TreeNodeRemovedEventArgs(removedNode, formerParent, formerPosition));
			notifyTreeNodeChanged(removedNode);
		}

		#endregion


		#region IMediaPresentation Members

		/// <summary>
		/// Gets a list of the <see cref="IMedia"/> used by a given <see cref="TreeNode"/>. 
		/// </summary>
		/// <param name="node">The node</param>
		/// <returns>The list</returns>
		/// <remarks>
		/// An <see cref="IMedia"/> is considered to be used by a <see cref="TreeNode"/> if the media
		/// is linked to the node via. a <see cref="ChannelsProperty"/>
		/// </remarks>
		protected virtual List<IMedia> getListOfMediaUsedByTreeNode(TreeNode node)
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
		/// Gets the list of <see cref="IMedia"/> used by the <see cref="TreeNode"/> tree of the presentation. 
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

		private void collectUsedMedia(TreeNode node, List<IMedia> collectedMedia)
		{
			foreach (IMedia m in getListOfMediaUsedByTreeNode(node))
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
