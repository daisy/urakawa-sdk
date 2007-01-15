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

		/// <summary>
		/// Default constructor
		/// </summary>
		public CoreNodeFactory()
		{
		}

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
			return new CoreNode(getPresentation());
		}

		/// <summary>
		/// Creates a new <see cref="ICoreNode"/> instance of <see cref="Type"/> matching a given QName
		/// </summary>
		/// <param localName="localName">The local localName part of the QName</param>
		/// <param localName="namespaceUri">The namespace uri part of the QName</param>
		/// <returns>The created <see cref="CoreNode"/> or <c>null</c> if the QN</returns>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when one of the parameters <paramref localName="localName"/> <paramref name="namespaceUri"/> and  is <c>null</c>
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
		/// <exception cref="exception.IsNotInitializedException">
		/// When no <see cref="ICorePresentation"/> has yet been associated with <c>this</c>
		/// </exception>
		public ICorePresentation getPresentation()
		{
			if (mPresentation == null)
			{
				throw new exception.IsNotInitializedException(
					"No core presentation has yet been associated with the core node factory");
			}
			return mPresentation;
		}


		/// <summary>
		/// Sets the <see cref="ICorePresentation"/> to which <see cref="CoreNode"/>s created by the factory belongs
		/// </summary>
		/// <param localName="pres">The <see cref="ICorePresentation"/></param>
		/// <remarks>This method should only be used during initialization</remarks>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when parameter <paramref localName="pres"/> is <c>null</c>
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
