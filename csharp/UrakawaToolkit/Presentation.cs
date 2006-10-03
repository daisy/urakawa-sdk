using System;
using System.Xml;

namespace urakawa.core
{
	/// <summary>
	/// Default implementation of interface <see cref="IPresentation"/>
	/// </summary>
	public class Presentation : IPresentation
	{
		/// <summary>
		/// Default constructor - initializes the
		/// </summary>
		public Presentation() : this(null, null, null, null, null)
		{
		}

		/// <summary>
		/// Constructor setting given factories and managers
		/// </summary>
		/// <param name="coreNodeFact">
		/// The <see cref="CoreNodeFactory"/> of the <see cref="Presentation"/> -
		/// if <c>null</c> a newly created <see cref="CoreNodeFactory"/> is used
		/// </param>
		/// <param name="chFact">
		/// The <see cref="ChannelFactory"/> of the <see cref="Presentation"/> -
		/// if <c>null</c> a newly created <see cref="ChannelFactory"/> is used
		/// </param>
		/// <param name="chMgr">
		/// The <see cref="ChannelsManager"/> of the <see cref="Presentation"/> -
		/// if <c>null</c> a newly created <see cref="ChannelsManager"/> is used
		/// </param>
		/// <param name="propFact">
		/// The <see cref="PropertyFactory"/> of the <see cref="Presentation"/> -
		/// if <c>null</c> a newly created <see cref="PropertyFactory"/> is used
		/// </param>
		/// <param name="mediaFact">
		/// The <see cref="urakawa.media.MediaFactory"/> of the <see cref="Presentation"/> -
		/// if <c>null</c> a newly created <see cref="urakawa.media.MediaFactory"/> is used
		/// </param>
		public Presentation(CoreNodeFactory coreNodeFact, ChannelFactory chFact, ChannelsManager chMgr, PropertyFactory propFact, urakawa.media.MediaFactory mediaFact)
		{
			if (coreNodeFact == null) coreNodeFact = new CoreNodeFactory();
			mCoreNodeFactory = coreNodeFact;
			mCoreNodeFactory.setPresentation(this);
			if (chFact == null) chFact = new ChannelFactory();
			mChannelFactory = chFact;
			if (chMgr == null) chMgr = new ChannelsManager();
			mChannelsManager = chMgr;
			mChannelsManager.setPresentation(this);
			mChannelsManager.setChannelFactory(mChannelFactory);
			if (propFact == null) propFact = new PropertyFactory();
			mPropertyFactory = propFact;
			mPropertyFactory.setPresentation(this);
			if (mediaFact == null) mediaFact = new urakawa.media.MediaFactory();
			mMediaFactory = mediaFact;
			mRootNode = new CoreNode(this);
		}


		private CoreNode mRootNode;
		private CoreNodeFactory mCoreNodeFactory;
		private ChannelFactory mChannelFactory;
		private ChannelsManager mChannelsManager;
		private PropertyFactory mPropertyFactory;
		private urakawa.media.MediaFactory mMediaFactory;

		//storage of the loaded and parsed DTD, if any has been given.
		internal System.Xml.XmlParserContext mDtdRules;
    
		/// <summary>
		/// Gets the <see cref="ChannelsManager"/> managing the list of <see cref="IChannel"/>s
		/// in the <see cref="Presentation"/>
		/// </summary>
		/// <returns>The <see cref="ChannelsManager"/></returns>
		public ChannelsManager getChannelsManager()
		{
			return mChannelsManager;
		}

		/// <summary>
		/// Gets the <see cref="CoreNodeFactory"/>
		/// creating <see cref="CoreNode"/>s for the <see cref="Presentation"/>
		/// </summary>
		/// <returns>The <see cref="CoreNodeFactory"/></returns>
		public CoreNodeFactory getCoreNodeFactory()
		{
			return mCoreNodeFactory;
		}

		/// <summary>
		/// Gets the root <see cref="CoreNode"/> of the <see cref="Presentation"/>
		/// </summary>
		/// <returns>The root <see cref="CoreNode"/></returns>
		public CoreNode getRootNode()
		{
			return mRootNode;
		}

		/// <summary>
		/// Sets the root <see cref="CoreNode"/> of the <see cref="Presentation"/>
		/// </summary>
		/// <param name="newRoot">The new root <see cref="CoreNode"/></param>
		public void setRootNode(CoreNode newRoot)
		{
			mRootNode = newRoot;
		}

		/// <summary>
		/// Gets the <see cref="PropertyFactory"/> associated with the <see cref="Presentation"/>
		/// </summary>
		/// <returns>The <see cref="PropertyFactory"/></returns>
		public PropertyFactory getPropertyFactory()
		{
			return mPropertyFactory;
		}

		/// <summary>
		/// Set the <see cref="PropertyFactory"/> associated with the <see cref="Presentation"/>
		/// </summary>
		/// <param name="propFact"></param>
		public void setPropertyFactory(PropertyFactory propFact)
		{
			propFact.setPresentation(this);
			mPropertyFactory = propFact;
		}


		/// <summary>
		/// Gets the <see cref="IChannelFactory"/> that creates <see cref="IChannel"/>s 
		/// for the <see cref="Presentation"/>
		/// </summary>
		/// <returns>The <see cref="IChannelFactory"/></returns>
		public ChannelFactory getChannelFactory()
		{
			return mChannelFactory;
		}
		// TODO: Add setChannelfactory or equivalent

		/// <summary>
		/// Gets the <see cref="urakawa.media.MediaFactory"/> creating <see cref="urakawa.media.IMedia"/>
		/// for the <see cref="Presentation"/>
		/// </summary>
		/// <returns>The <see cref="urakawa.media.MediaFactory"/></returns>
		public urakawa.media.MediaFactory getMediaFactory()
		{
			return mMediaFactory;
		}

		/// <summary>
		/// Sets the <see cref="urakawa.media.MediaFactory"/>
		/// of the <see cref="Presentation"/>
		/// </summary>
		/// <param name="newMediafactory">
		/// The new <see cref="urakawa.media.MediaFactory"/>
		/// </param>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when <paramref name="exception.MethodParameterIsNullException"/> 
		/// is <c>null</c>
		/// </exception>
		public void setMediaFactory(urakawa.media.MediaFactory newMediafactory)
		{
			if (newMediafactory==null)
			{
				throw new exception.MethodParameterIsNullException(
					"The new media factory can not be null");
			}
			mMediaFactory = newMediafactory;
		}

		#region IPresentation Members

		ICoreNode IPresentation.getRootNode()
		{
			return getRootNode();
		}

		IChannelFactory IPresentation.getChannelFactory()
		{
			return getChannelFactory();
		}

		IChannelsManager IPresentation.getChannelsManager()
		{
			return getChannelsManager();
		}

		ICoreNodeFactory IPresentation.getCoreNodeFactory()
		{
			return getCoreNodeFactory();
		}

		IPropertyFactory IPresentation.getPropertyFactory()
		{
			return getPropertyFactory();
		}

		urakawa.media.IMediaFactory IPresentation.getMediaFactory()
		{
			return getMediaFactory();
		}
		#endregion

		#region IXUKAble members 

		/// <summary>
		/// Reads the <see cref="Presentation"/> instance from a Presentation element in a XUK file.
		/// <list type="table">
		/// <item>
		/// <term>Entry state</term>
		/// <description>
		/// The cursor of <paramref name="source"/> must be at the start of the Presentation element
		/// </description>
		/// </item>
		/// <item>
		/// <term>Exit state</term>
		/// </item>
		/// <description>
		/// The cursor of  <paramref name="source"/> must be at the end of the Presentation element
		/// </description>
		/// </list>
		/// </summary>
		/// <param name="source">The <see cref="XmlReader"/> from which to read the Presentation element</param>
		/// <returns>A <see cref="bool"/> indicating if the instance was succesfully read</returns>
		public bool XUKIn(System.Xml.XmlReader source)
		{
			if (source == null)
			{
				throw new exception.MethodParameterIsNullException("XML Reader is null");
			}
			if (source.NodeType != XmlNodeType.Element) return false;
			if (source.LocalName != "Presentation") return false;
			if (source.NamespaceURI != urakawa.ToolkitSettings.XUK_NS) return false;

			bool bProcessedChannelsManager = false;
			bool bProcessedRootNode = false;

			while (source.Read())
			{
				if (source.NodeType==XmlNodeType.Element)
				{
					if (source.NamespaceURI != urakawa.ToolkitSettings.XUK_NS) return false;
					switch (source.LocalName)
					{
						case "mChannelsManager":
							if (bProcessedChannelsManager) return false;
							if (!XUKInChannelsManager(source)) return false;
							bProcessedChannelsManager = true;
							break;
						case "mRootNode":
							if (bProcessedRootNode) return false;
							if (!XUKInRootNode(source)) return false;
							bProcessedRootNode = true;
							break;
						default:
							return false;
					}
				}
				else if (source.NodeType==XmlNodeType.EndElement)
				{
					break;
				}
				if (source.EOF) break;
			}
			return bProcessedChannelsManager && bProcessedRootNode;
		}

		private bool XUKInChannelsManager(System.Xml.XmlReader source)
		{
			if (source.IsEmptyElement) return false;
			bool bFoundChMgr = false;
			while (source.Read())
			{
				if (source.NodeType == XmlNodeType.Element)
				{
					if (!mChannelsManager.XUKIn(source)) return false;
					bFoundChMgr = true;
				}
				else if (source.NodeType == XmlNodeType.EndElement)
				{
					break;
				}
				if (source.EOF) break;
			}
			return bFoundChMgr;
		}

		private bool XUKInRootNode(System.Xml.XmlReader source)
		{
			if (source.IsEmptyElement) return false;
			bool bFoundRootNode = false;
			while (source.Read())
			{
				if (source.NodeType == XmlNodeType.Element)
				{
					mRootNode = getCoreNodeFactory().createNode(source.LocalName, source.NamespaceURI);
					if (mRootNode == null) return false;
					if (!mRootNode.XUKIn(source)) return false;
					bFoundRootNode = true;
				}
				else if (source.NodeType == XmlNodeType.EndElement)
				{
					break;
				}
				if (source.EOF) break;
			}
			return bFoundRootNode;
		}

		/// <summary>
		/// Write a Presentation element to a XUK file representing the <see cref="Presentation"/> instance
		/// </summary>
		/// <param name="destination">The destination <see cref="XmlWriter"/></param>
		/// <returns>A <see cref="bool"/> indicating if the write was succesful</returns>
		public bool XUKOut(System.Xml.XmlWriter destination)
		{
			if (destination == null)
			{
				throw new exception.MethodParameterIsNullException("Xml Writer is null");
			}
			destination.WriteStartElement("Presentation", urakawa.ToolkitSettings.XUK_NS);
			destination.WriteStartElement("mChannelsManager", urakawa.ToolkitSettings.XUK_NS);
			if (!mChannelsManager.XUKOut(destination)) return false;
			destination.WriteEndElement();
			destination.WriteStartElement("mRootNode", urakawa.ToolkitSettings.XUK_NS);
			if (!mRootNode.XUKOut(destination)) return false;
			destination.WriteEndElement();
			destination.WriteEndElement();
			return true;
		}
		#endregion

		/// <summary>
		/// Sets the DTD of the presentation
		/// </summary>
		/// <param name="dtdContent">The content of the DTD</param>
		/// <returns>A <see cref="bool"/> indicating if the DTD content was succesfully set</returns>
		public bool setDtdContent(string dtdContent)
		{
			bool dtdWasLoaded = false;
			if(dtdContent == "")
			{
				//discarding any rules that might previously have been set
				mDtdRules = null;
				return true;
			}

			string strDtdContent = dtdContent;
			if(strDtdContent.IndexOf("?>",0)>-1)
				strDtdContent = strDtdContent.Substring(strDtdContent.IndexOf("?>",0)+2);

			try
			{
				System.Xml.XmlParserContext mDtdRules 
					= new System.Xml.XmlParserContext(
					null,
					new System.Xml.XmlNamespaceManager(new System.Xml.NameTable()),
					/*this value will be set to something else every time the object is used*/ "",
					/*pubId*/null,
					/*sysId*/null,
					strDtdContent,
					".",
					"",
					System.Xml.XmlSpace.Default,
					System.Text.Encoding.UTF8
					);
				dtdWasLoaded = true;
			}
			catch(Exception)
			{
				mDtdRules = null;
				dtdWasLoaded = false;
			}

			return dtdWasLoaded;
		}
	}
}
