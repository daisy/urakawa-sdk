using System;

namespace urakawa.core
{
	/// <summary>
	/// Default implementation of 
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
    }

    private CoreNode mRootNode;
    private CoreNodeFactory mCoreNodeFactory;
    private ChannelFactory mChannelFactory;
    private ChannelsManager mChannelsManager;

    /// <summary>
    /// Gets the <see cref="ICoreNodeFactory"/>
    /// creating <see cref="ICoreNode"/>s for the <see cref="Presentation"/>
    /// </summary>
    /// <returns></returns>
    public ICoreNodeFactory getCoreNodeFactory()
    {
      return mCoreNodeFactory;
    }

    #region IPresentation Members

    /// <summary>
    /// Gets the root <see cref="ICoreNode"/> of the <see cref="Presentation"/>
    /// </summary>
    /// <returns>The root <see cref="ICoreNode"/></returns>
    public ICoreNode getRootNode()
    {
      return mRootNode;
    }

    /// <summary>
    /// Gets the <see cref="IChannelFactory"/> that creates <see cref="IChannel"/>s 
    /// for the <see cref="Presentation"/>
    /// </summary>
    /// <returns>The <see cref="IChannelFactory"/></returns>
    public IChannelFactory getChannelFactory()
    {
      return mChannelFactory;
    }

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
    /// Gets the <see cref="IChannelsManager"/> managing the list of <see cref="IChannel"/>s
    /// in the <see cref="Presentation"/>
    /// </summary>
    /// <returns>The <see cref="IChannelsManager"/></returns>
    IChannelsManager IPresentation.getChannelsManager()
    {
      return mChannelsManager;
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
