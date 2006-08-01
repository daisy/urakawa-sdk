using System;

namespace urakawa.core
{
	/// <summary>
	/// Default implementation of <see cref="ICoreNodeFactory"/>.
	/// Creates <see cref="CoreNode"/>s belonging to a specific <see cref="IPresentation"/>
	/// </summary>
	public class CoreNodeFactory : ICoreNodeFactory
	{
		/// <summary>
		/// The namespace uri of the XUK files
		/// </summary>
		public static string XUK_NS = PropertyFactory.XUK_NS;

		/// <summary>
		/// Gets or sets a <see cref="bool"/> indicating if the created
		/// <see cref="CoreNode"/>s should have <see cref="ChannelsProperty"/>
		/// added automatically
		/// </summary>
		public bool AutoAddChannelsProperty = true;

		/// <summary>
    /// The <see cref="Presentation"/> to which any created <see cref="ICoreNode"/>s belongs
    /// </summary>
    private Presentation mPresentation;

    /// <summary>
    /// Gets the <see cref="IPresentation"/> to which created nodes belong
    /// </summary>
    /// <returns>The <see cref="IPresentation"/></returns>
    public IPresentation getPresentation()
    {
      return mPresentation;
    }

    /// <summary>
    /// Constructs a <see cref="CoreNodeFactory"/> creating <see cref="CoreNode"/>s
    /// belonging to the given <see cref="IPresentation"/>
    /// </summary>
    /// <param name="presentation">The given <see cref="IPresentation"/></param>
		public CoreNodeFactory(Presentation presentation)
		{
      mPresentation = presentation;
    }

    #region ICoreNodeFactory Members
    ICoreNode ICoreNodeFactory.createNode()
    {
      return createNode();
    }
    #endregion


    /// <summary>
    /// Creates a new <see cref="ICoreNode"/>
    /// </summary>
    /// <returns>The new <see cref="ICoreNode"/></returns>
    public CoreNode createNode()
    {
      CoreNode node = new CoreNode(mPresentation);
			if (AutoAddChannelsProperty)
			{
				node.setProperty(mPresentation.getPropertyFactory().createChannelsProperty());
			}
      return node;
    }

  }
}
