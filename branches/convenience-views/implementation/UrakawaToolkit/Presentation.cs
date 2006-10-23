using System;
using System.Xml;
using urakawa.core;
using urakawa.core.property;
using urakawa.properties.channel;

namespace urakawa
{
	/// <summary>
	/// Default implementation of interface <see cref="IPresentation"/>
	/// </summary>
	public class Presentation : CorePresentation, IPresentation
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
		/// The <see cref="ICoreNodeFactory"/> of the <see cref="Presentation"/> -
		/// if <c>null</c> a newly created <see cref="CoreNodeFactory"/> is used
		/// </param>
		/// <param name="propFact">
		/// The <see cref="IPropertyFactory"/> of the <see cref="Presentation"/> -
		/// if <c>null</c> a newly created <see cref="PropertyFactory"/> is used
		/// </param>
		/// <param name="chFact">
		/// The <see cref="IChannelFactory"/> of the <see cref="Presentation"/> -
		/// if <c>null</c> a newly created <see cref="ChannelFactory"/> is used
		/// </param>
		/// <param name="chMgr">
		/// The <see cref="IChannelsManager"/> of the <see cref="Presentation"/> -
		/// if <c>null</c> a newly created <see cref="ChannelsManager"/> is used
		/// </param>
		/// <param name="mediaFact">
		/// The <see cref="urakawa.media.IMediaFactory"/> of the <see cref="Presentation"/> -
		/// if <c>null</c> a newly created <see cref="urakawa.media.MediaFactory"/> is used
		/// </param>
		public Presentation(CoreNodeFactory coreNodeFact, PropertyFactory propFact, ChannelFactory chFact, ChannelsManager chMgr, urakawa.media.MediaFactory mediaFact)
			: base(coreNodeFact, propFact)
		{
			if (chFact == null) chFact = new ChannelFactory();
			mChannelFactory = chFact;
			if (chMgr == null) chMgr = new ChannelsManager();
			mChannelsManager = chMgr;
			mChannelsManager.setPresentation(this);
			mChannelsManager.setChannelFactory(mChannelFactory);
			if (mediaFact == null) mediaFact = new urakawa.media.MediaFactory();
			mMediaFactory = mediaFact;
		}


		private IChannelFactory mChannelFactory;
		private IChannelsManager mChannelsManager;
		private urakawa.media.IMediaFactory mMediaFactory;

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
		/// <param name="newMediaFactory">
		/// The new <see cref="urakawa.media.MediaFactory"/>
		/// </param>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when <paramref name="exception.MethodParameterIsNullException"/> 
		/// is <c>null</c>
		/// </exception>
		public void setMediaFactory(urakawa.media.MediaFactory newMediaFactory)
		{
			if (newMediaFactory==null)
			{
				throw new exception.MethodParameterIsNullException(
					"The new media factory can not be null");
			}
			mMediaFactory = newMediaFactory;
		}

		#region IPresentation Members

		ICoreNode ICorePresentation.getRootNode()
		{
			return getRootNode();
		}

		IChannelFactory IChannelPresentation.getChannelFactory()
		{
			return getChannelFactory();
		}

		IChannelsManager IChannelPresentation.getChannelsManager()
		{
			return getChannelsManager();
		}

		ICoreNodeFactory ICorePresentation.getCoreNodeFactory()
		{
			return getCoreNodeFactory();
		}

		IPropertyFactory ICorePresentation.getPropertyFactory()
		{
			return getPropertyFactory();
		}

		urakawa.media.IMediaFactory urakawa.media.IMediaPresentation.getMediaFactory()
		{
			return getMediaFactory();
		}
		#endregion

		#region IXUKAble members 


		public bool XukIn(System.Xml.XmlReader source)
		{
			return bProcessedChannelsManager && bProcessedRootNode;
		}

		protected bool XUKInChannelsManager(System.Xml.XmlReader source)
		{
			if (source.IsEmptyElement) return false;
			bool bFoundChMgr = false;
			while (source.Read())
			{
				if (source.NodeType == XmlNodeType.Element)
				{
					if (!mChannelsManager.XukIn(source)) return false;
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
