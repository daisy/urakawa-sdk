using System;

namespace urakawa.core
{
	/// <summary>
	/// Default implementation of interface <see cref="IPresentation"/>
	/// </summary>
	public class Presentation : IPresentation
	{
    /// <summary>
    /// Default constructor
    /// </summary>
		public Presentation()
		{
      mCoreNodeFactory = new CoreNodeFactory(this);
      mRootNode = mCoreNodeFactory.createNode();
      mChannelFactory = new ChannelFactory();
      mChannelsManager = new ChannelsManager();
      mPropertyFactory = new PropertyFactory(this);
    }

    private CoreNode mRootNode;
    private CoreNodeFactory mCoreNodeFactory;
    private ChannelFactory mChannelFactory;
    private ChannelsManager mChannelsManager;
    private PropertyFactory mPropertyFactory;

    
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
    /// Gets the <see cref="ICoreNodeFactory"/>
    /// creating <see cref="ICoreNode"/>s for the <see cref="Presentation"/>
    /// </summary>
    /// <returns></returns>
    public CoreNodeFactory getCoreNodeFactory()
    {
      return mCoreNodeFactory;
    }

    /// <summary>
    /// Gets the root <see cref="ICoreNode"/> of the <see cref="Presentation"/>
    /// </summary>
    /// <returns>The root <see cref="ICoreNode"/></returns>
    public CoreNode getRootNode()
    {
      return mRootNode;
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
    /// Gets the <see cref="IChannelFactory"/> that creates <see cref="IChannel"/>s 
    /// for the <see cref="Presentation"/>
    /// </summary>
    /// <returns>The <see cref="IChannelFactory"/></returns>
    public ChannelFactory getChannelFactory()
    {
      return mChannelFactory;
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
    #endregion

		#region IXUKable members 

		public bool XUKin(System.Xml.XmlReader source)
		{
			//TODO: actual implementation, for now we return false as default, signifying that all was not done
			return false;
		}

		public bool XUKout(System.Xml.XmlWriter destination)
		{
			//TODO: actual implementation, for now we return false as default, signifying that all was not done
			return false;
		}
		#endregion


  }
}
