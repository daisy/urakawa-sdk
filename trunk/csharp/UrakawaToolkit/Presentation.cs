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
using urakawa.metadata;
using urakawa.undo;

namespace urakawa
{
	/// <summary>
	/// The primary container for a document tree consisting of <see cref="TreeNode"/>s,
	/// includes factories and managers for:
	/// <list type="bullet">
	/// <item><see cref="Property"/>s</item>
	/// <item><see cref="Channel"/>s</item>
	/// <item><see cref="IMedia"/></item>
	/// <item><see cref="MediaData"/></item>
	/// <item><see cref="IDataProvider"/>s</item>
	/// <item><see cref="Metadata"/></item>
	/// </list>
	/// </summary>
	public class Presentation : ITreePresentation, IMediaDataPresentation, IChannelPresentation, IXmlPresentation, IValueEquatable<Presentation>
	{
		/// <summary>
		/// Constructor - initializes the presentation with a given base <see cref="Uri"/> and default factories and managers.
		/// The constructed has an empty <see cref="TreeNode"/> as root
		/// </summary>
		/// <param name="bUri">The given base uri</param>
		public Presentation(Uri bUri) 
			: this(bUri, null, null, null, null, null, null, null, null, null, null, null)
		{
		}

		/// <summary>
		/// Constructor setting given factories and managers.
		/// The constructed has an empty <see cref="TreeNode"/> as root
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
		/// <param name="mediaDataFact">
		/// The media data facotry of the presentation - 
		/// if <c>null</c> a newly created <see cref="MediaDataFactory"/> is used
		/// </param>
		///	<param name="dataProvMngr">
		///	The data provider manager of the presentation - 
		///	if <c>null</c> a newly created <see cref="FileDataProviderManager"/> is used
		/// </param>
		///	<param name="undoRedoMngr">
		///	The undo/redo manager of the presentation - 
		///	if <c>null</c> a newly created <see cref="UndoRedoManager"/> is used
		/// </param>
		///	<param name="cmdFact">
		///	The command factory of the presentation - 
		///	if <c>null</c> a newly created <see cref="CommandFactory"/> is used
		/// </param>
		/// <param name="metaFact">
		/// The <see cref="Metadata"/> factory of the presentation
		/// </param>
		public Presentation(
			Uri bUri,
			TreeNodeFactory treeNodeFact, PropertyFactory propFact, 
			ChannelFactory chFact, ChannelsManager chMgr, IMediaFactory mediaFact,
			MediaDataManager mediaDataMngr, MediaDataFactory mediaDataFact, IDataProviderManager dataProvMngr,
			UndoRedoManager undoRedoMngr, CommandFactory cmdFact,
			metadata.MetadataFactory metaFact
			)
		{
			setBaseUri(bUri);
			//Replace nulls with defaults
			if (treeNodeFact == null) treeNodeFact = new TreeNodeFactory();
			if (propFact == null) propFact = new PropertyFactory();
			if (chFact == null) chFact = new ChannelFactory();
			if (chMgr == null) chMgr = new ChannelsManager();
			if (mediaFact == null) mediaFact = new MediaFactory();
			if (mediaDataMngr == null) mediaDataMngr = new MediaDataManager();
			if (mediaDataFact == null) mediaDataFact = new MediaDataFactory();
			if (dataProvMngr == null) dataProvMngr = new FileDataProviderManager("Data");
			if (undoRedoMngr == null) undoRedoMngr = new UndoRedoManager();
			if (cmdFact == null) cmdFact = new CommandFactory();
			if (metaFact == null) metaFact = new urakawa.metadata.MetadataFactory();


			//Setup member vars
			mTreeNodeFactory = treeNodeFact;
			mPropertyFactory = propFact;
			mChannelFactory = chFact;
			mChanelsManager = chMgr;
			mMediaFactory = mediaFact;
			mMediaDataManager = mediaDataMngr;
			mMediaDataFactory = mediaDataFact;
			mDataProviderManager = dataProvMngr;
			mUndoRedoManager = undoRedoMngr;
			mCommandFactory = cmdFact;
			mMetadataFactory = metaFact;
			mMetadata = new List<Metadata>();

			//Linkup members to this
			treeNodeFact.setPresentation(this);
			mChannelFactory.setPresentation(this);
			mChanelsManager.setPresentation(this);
			propFact.setPresentation(this);
			mMediaFactory.setPresentation(this);
			mMediaDataManager.setPresentation(this);
			mMediaDataFactory.setPresentation(this);
			mDataProviderManager.setPresentation(this);
			mUndoRedoManager.setPresentation(this);
			mCommandFactory.setPresentation(this);
			mMetadataFactory.setPresentation(this);

			setRootNode(getTreeNodeFactory().createNode());

			mLanguage = null;
		}

		private TreeNodeFactory mTreeNodeFactory;
		private PropertyFactory mPropertyFactory;
		private ChannelFactory mChannelFactory;
		private ChannelsManager mChanelsManager;
		private IMediaFactory mMediaFactory;
		private MediaDataManager mMediaDataManager;
		private MediaDataFactory mMediaDataFactory;
		private IDataProviderManager mDataProviderManager;
		private undo.UndoRedoManager mUndoRedoManager;
		private undo.CommandFactory mCommandFactory;
		private TreeNode mRootNode;
		private Uri mBaseUri;
		private string mLanguage;

		/// <summary>
		/// Sets the language of the presentation
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
		/// Gets the language of the presentation
		/// </summary>
		/// <returns>The language</returns>
		public string getLanguage()
		{
			return mLanguage;
		}

		/// <summary>
		/// Removes any <see cref="MediaData"/> and <see cref="DataProvider"/>s that are not used by any <see cref="TreeNode"/> in the document tree
		/// or by any <see cref="ICommand"/> in the <see cref="UndoRedoManager"/> stacks (undo/redo/transaction).
		/// </summary>
		public void cleanup()
		{
			urakawa.media.data.utilities.CollectManagedMediaTreeNodeVisitor collectorVisitor
				= new urakawa.media.data.utilities.CollectManagedMediaTreeNodeVisitor();
			if (getRootNode() != null)
			{
				getRootNode().acceptDepthFirst(collectorVisitor);
			}
			List<MediaData> usedMediaData = getUndoRedoManager().getListOfUsedMediaData();
			foreach (IManagedMedia mm in collectorVisitor.getListOfCollectedMedia())
			{
				if (!usedMediaData.Contains(mm.getMediaData())) usedMediaData.Add(mm.getMediaData());
			}
			List<IDataProvider> usedDataProviders = new List<IDataProvider>();
			foreach (MediaData md in getMediaDataManager().getListOfMediaData())
			{
				if (usedMediaData.Contains(md))
				{
					if (md is urakawa.media.data.audio.codec.WavAudioMediaData)
					{
						((urakawa.media.data.audio.codec.WavAudioMediaData)md).forceSingleDataProvider();
					}
					foreach (IDataProvider dp in md.getListOfUsedDataProviders())
					{
						if (!usedDataProviders.Contains(dp)) usedDataProviders.Add(dp);
					}
				}
				else
				{
					md.delete();
				}
			}
			foreach (IDataProvider dp in getDataProviderManager().getListOfDataProviders())
			{
				if (!usedDataProviders.Contains(dp)) dp.delete();
			}
		}

		
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
				setRootNode(null);
				getChannelsManager().clearChannels();
				getMediaDataManager().deleteUnusedMediaData();
				getDataProviderManager().removeUnusedDataProviders(false);
				mMetadata.Clear();
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
			string lang = source.GetAttribute("language");
			if (lang != null) lang = lang.Trim();
			if (lang == "") lang = null;
			setLanguage(lang);
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

		private void XukInMetadata(XmlReader source)
		{
			if (source.IsEmptyElement) return;
			while (source.Read())
			{
				if (source.NodeType == XmlNodeType.Element)
				{
					Metadata newMeta = mMetadataFactory.createMetadata(source.LocalName, source.NamespaceURI);
					if (newMeta == null)
					{
						if (!source.IsEmptyElement)
						{
							//Read past unidentified element
							source.ReadSubtree().Close();
						}
					}
					else
					{
						newMeta.XukIn(source);
						mMetadata.Add(newMeta);
					}
				}
				else if (source.NodeType == XmlNodeType.EndElement)
				{
					break;
				}
				if (source.EOF)
				{
					throw new exception.XukException("Unexpectedly reached EOF");
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
					case "mMetadata":
						XukInMetadata(source);
						break;
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
			if (getLanguage() != null)
			{
				destination.WriteAttributeString("language", getLanguage());
			}
		}

		/// <summary>
		/// Write the child elements of a Presentation element.
		/// </summary>
		/// <param name="destination">The destination <see cref="XmlWriter"/></param>
		protected virtual void XukOutChildren(XmlWriter destination)
		{
			destination.WriteStartElement("mMetadata", urakawa.ToolkitSettings.XUK_NS);
			foreach (Metadata md in mMetadata)
			{
				md.XukOut(destination);
			}
			destination.WriteEndElement();
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

		/// <summary>
		/// Gets the <see cref="UndoRedoManager"/> of <c>this</c>
		/// </summary>
		/// <returns>The <see cref="UndoRedoManager"/></returns>
		public UndoRedoManager getUndoRedoManager()
		{
			return mUndoRedoManager;
		}

		/// <summary>
		/// Gets the <see cref="CommandFactory"/> of <c>this</c>
		/// </summary>
		/// <returns>The <see cref="CommandFactory"/></returns>
		public CommandFactory getCommandFactory()
		{
			return mCommandFactory;
		}

		#endregion

		#region IMediaPresentation Members

		/// <summary>
		/// Gets the <see cref="IMediaFactory"/> of <c>this</c>
		/// </summary>
		/// <returns>The <see cref="IMediaFactory"/></returns>
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
		/// Gets the manager for <see cref="MediaData"/>
		/// </summary>
		/// <returns>The media data manager</returns>
		public MediaDataManager getMediaDataManager()
		{
			return mMediaDataManager;
		}

		/// <summary> 
		/// Gets the factory for <see cref="MediaData"/>.
		/// </summary>
		/// <returns>The media data factory</returns>
		public MediaDataFactory getMediaDataFactory()
		{
			return mMediaDataFactory;
		}

		/// <summary>
		/// Gets the manager for <see cref="IDataProvider"/>s
		/// </summary>
		/// <returns>The data provider manager</returns>
		public IDataProviderManager getDataProviderManager()
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
		public bool valueEquals(Presentation other)
		{
			if (other == null) return false;
			if (!getChannelsManager().valueEquals(other.getChannelsManager())) return false;
			if (!getDataProviderManager().valueEquals(other.getDataProviderManager())) return false;
			if (!getMediaDataManager().valueEquals(other.getMediaDataManager())) return false;
			if (!getRootNode().valueEquals(other.getRootNode())) return false;
			List<Metadata> thisMetadata = getMetadataList();
			List<Metadata> otherMetadata = other.getMetadataList();
			if (thisMetadata.Count != otherMetadata.Count) return false;
			foreach (Metadata m in thisMetadata)
			{
				bool found = false;
				foreach (Metadata om in other.getMetadataList(m.getName()))
				{
					if (m.ValueEquals(om)) found = true;
				}
				if (!found) return false;
			}
			if (getLanguage() != other.getLanguage()) return false;
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
			foreach (Property prop in node.getListOfProperties())
			{
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

		#region Metadata
		private List<Metadata> mMetadata;
		private MetadataFactory mMetadataFactory;


		/// <summary>
		/// Retrieves the <see cref="MetadataFactory"/> creating <see cref="Metadata"/> 
		/// for the <see cref="Project"/> instance
		/// </summary>
		/// <returns></returns>
		public MetadataFactory getMetadataFactory()
		{
			return mMetadataFactory;
		}


		/// <summary>
		/// Appends a <see cref="Metadata"/> to the <see cref="Project"/>
		/// </summary>
		/// <param name="metadata">The <see cref="Metadata"/> to add</param>
		public void appendMetadata(Metadata metadata)
		{
			mMetadata.Add(metadata);
		}

		/// <summary>
		/// Gets a <see cref="List{Metadata}"/> of all <see cref="Metadata"/>
		/// in the <see cref="Project"/>
		/// </summary>
		/// <returns>The <see cref="List{Metadata}"/> of metadata <see cref="Metadata"/></returns>
		public List<Metadata> getMetadataList()
		{
			return new List<Metadata>(mMetadata);
		}

		/// <summary>
		/// Gets a <see cref="List{Metadata}"/> of all <see cref="Metadata"/>
		/// in the <see cref="Project"/> with a given name
		/// </summary>
		/// <param name="name">The given name</param>
		/// <returns>The <see cref="List{Metadata}"/> of <see cref="Metadata"/></returns>
		public List<Metadata> getMetadataList(string name)
		{
			List<Metadata> list = new List<Metadata>();
			foreach (Metadata md in mMetadata)
			{
				if (md.getName() == name) list.Add(md);
			}
			return list;
		}

		/// <summary>
		/// Deletes all <see cref="Metadata"/>s with a given name
		/// </summary>
		/// <param name="name">The given name</param>
		public void deleteMetadata(string name)
		{
			foreach (Metadata md in getMetadataList(name))
			{
				deleteMetadata(md);
			}
		}

		/// <summary>
		/// Deletes a given <see cref="Metadata"/>
		/// </summary>
		/// <param name="metadata">The given <see cref="Metadata"/></param>
		public void deleteMetadata(Metadata metadata)
		{
			mMetadata.Remove(metadata);
		}

		#endregion

	}
}
