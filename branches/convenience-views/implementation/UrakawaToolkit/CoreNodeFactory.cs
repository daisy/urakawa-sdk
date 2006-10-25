using System;

namespace urakawa.core
{
	/// <summary>
	/// Default implementation of <see cref="ICoreNodeFactory"/>.
	/// Creates <see cref="CoreNode"/>s belonging to a specific <see cref="ICorePresentation"/>
	/// </summary>
	/// <remarks>
	/// A <see cref="CoreNodeFactory"/> can not create <see cref="CoreNode"/>s
	/// until it has been associated with a <see cref="ICorePresentation"/> using the
	/// <see cref="setPresentation"/> method
	/// </remarks>
	public class CoreNodeFactory : ICoreNodeFactory
	{
		/// <summary>
		/// The <see cref="ICorePresentation"/> to which any created <see cref="ICoreNode"/>s belongs
    /// </summary>
    private ICorePresentation mPresentation;

		#region ICoreNodeFactory Members

		/// <summary>
		/// Creates a new <see cref="CoreNode"/>
		/// </summary>
		/// <returns>The new <see cref="ICoreNode"/></returns>
		/// <exception cref="exception.IsNotInitializedException">
		/// Thrown when the <see cref="ICorePresentation"/> of the 
		/// </exception>
		public ICoreNode createNode()
		{
			ICorePresentation pres = getPresentation();
			if (pres == null)
			{
				throw new exception.IsNotInitializedException(
					"The core node factory has not yet been associated with a presentation");
			}
			return new CoreNode(getPresentation());
		}

		/// <summary>
		/// Creates a new <see cref="ICoreNode"/> instance of <see cref="Type"/> matching a given QName
		/// </summary>
		/// <param name="localName">The local name part of the QName</param>
		/// <param name="namespaceUri">The namespace uri part of the QName</param>
		/// <returns>The created <see cref="CoreNode"/> or <c>null</c> if the QN</returns>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when parameter <paramref name="pres"/> is <c>null</c>
		/// </exception>
		public virtual ICoreNode createNode(string localName, string namespaceUri)
		{
			if (localName == null || namespaceUri == null)
			{
				throw new exception.MethodParameterIsNullException(
					"No part of the QName can be null");
			}
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
		/// <param name="pres">The <see cref="ICorePresentation"/></param>
		/// <remarks>This method should only be used during initialization</remarks>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when parameter <paramref name="pres"/> is <c>null</c>
		/// </exception>
		/// <exception cref="exception.IsAlreadyInitializedException">
		/// Thrown when the <see cref="ICorePresentation"/> has already been set
		/// </exception>
		public void setPresentation(ICorePresentation pres)
		{
			if (pres == null)
			{
				throw new exception.MethodParameterIsNullException(
					"The presentation associated with a core node factory can not be null");
			}
			if (mPresentation != null)
			{
				throw new exception.IsAlreadyInitializedException(
					"The presentation has already been set");
			}
			mPresentation = pres;
		}

		#endregion
	}
}
