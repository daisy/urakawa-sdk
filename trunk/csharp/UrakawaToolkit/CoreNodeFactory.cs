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
    /// Gets the <see cref="Presentation"/> to which created nodes belong
    /// </summary>
    /// <returns>The <see cref="IPresentation"/></returns>
    public Presentation getPresentation()
    {
      return mPresentation;
    }

		/// <summary>
		/// Sets the <see cref="Presentation"/> to which <see cref="CoreNode"/>s created by the factory belongs
		/// </summary>
		/// <param name="newPres"></param>
		public void setPresentation(Presentation newPres)
		{
			mPresentation = newPres;
		}

    /// <summary>
    /// Constructs a <see cref="CoreNodeFactory"/> creating <see cref="CoreNode"/>s
    /// </summary>
		public CoreNodeFactory()
		{
    }


		/// <summary>
		/// Creates a new <see cref="ICoreNode"/>
		/// </summary>
		/// <returns>The new <see cref="ICoreNode"/></returns>
		public virtual CoreNode createNode()
		{
			return createNode("CoreNode", urakawa.ToolkitSettings.XUK_NS);
		}


		#region ICoreNodeFactory Members

		ICoreNode ICoreNodeFactory.createNode(string localName, string namespaceUri)
		{
			return createNode(localName, namespaceUri);
		}

		/// <summary>
		/// Creates a new <see cref="ICoreNode"/> instance of <see cref="Type"/> matching a given QName
		/// </summary>
		/// <param name="localName">The local name part of the QName</param>
		/// <param name="namespaceUri">The namespace uri part of the QName</param>
		/// <returns>The created <see cref="CoreNode"/></returns>
		public virtual CoreNode createNode(string localName, string namespaceUri)
		{
			if (namespaceUri == urakawa.ToolkitSettings.XUK_NS)
			{
				switch (localName)
				{
					case "CoreNode":
						return new CoreNode(getPresentation());
				}
			}
			return null;
		}

		#endregion
	}
}
