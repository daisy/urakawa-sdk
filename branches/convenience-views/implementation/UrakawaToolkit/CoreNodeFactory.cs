using System;

namespace urakawa.core
{
	/// <summary>
	/// Default implementation of <see cref="ICoreNodeFactory"/>.
	/// Creates <see cref="CoreNode"/>s belonging to a specific <see cref="ICorePresentation"/>
	/// </summary>
	public class CoreNodeFactory : ICoreNodeFactory
	{
		/// <summary>
		/// The <see cref="ICorePresentation"/> to which any created <see cref="ICoreNode"/>s belongs
    /// </summary>
    private ICorePresentation mPresentation;

    /// <summary>
    /// Constructs a <see cref="CoreNodeFactory"/> creating <see cref="CoreNode"/>s
    /// </summary>
		public CoreNodeFactory()
		{
    }

		#region ICoreNodeFactory Members

		/// <summary>
		/// Creates a new <see cref="CoreNode"/>
		/// </summary>
		/// <returns>The new <see cref="ICoreNode"/></returns>
		public ICoreNode createNode()
		{
			return new CoreNode(getPresentation());
		}

		/// <summary>
		/// Creates a new <see cref="ICoreNode"/> instance of <see cref="Type"/> matching a given QName
		/// </summary>
		/// <param name="localName">The local name part of the QName</param>
		/// <param name="namespaceUri">The namespace uri part of the QName</param>
		/// <returns>The created <see cref="CoreNode"/> or <c>null</c> if the QN</returns>
		public virtual ICoreNode createNode(string localName, string namespaceUri)
		{
			if (namespaceUri == urakawa.ToolkitSettings.XUK_NS)
			{
				switch (localName)
				{
					case "CoreNode":
						return createNode();
				}
			}
			return null;
		}


		/// <summary>
		/// Gets the <see cref="ICorePresentation"/> to which created nodes belong
		/// </summary>
		/// <returns>The <see cref="ICorePresentation"/></returns>
		public ICorePresentation getPresentation()
		{
			return mPresentation;
		}


		/// <summary>
		/// Sets the <see cref="ICorePresentation"/> to which <see cref="CoreNode"/>s created by the factory belongs
		/// </summary>
		/// <param name="newPres">The <see cref="ICorePresentation"/></param>
		/// <remarks>This method should only be used during initialization</remarks>
		public void setPresentation(ICorePresentation newPres)
		{
			//TODO: Change visibility to internal?
			mPresentation = newPres;
		}

		#endregion
	}
}
