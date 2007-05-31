using System;

namespace urakawa.core
{
	/// <summary>
	/// Default implementation of <see cref="CoreNodeFactory"/>.
	/// Creates <see cref="CoreNode"/>s belonging to a specific <see cref="ICorePresentation"/>
	/// </summary>
	/// <remarks>
	/// A <see cref="CoreNodeFactory"/> can not create <see cref="CoreNode"/>s
	/// until it has been associated with a <see cref="Presentation"/> using the
	/// <see cref="setPresentation"/> method
	/// </remarks>
	public class CoreNodeFactory
	{
		/// <summary>
		/// The <see cref="Presentation"/> to which any created <see cref="CoreNode"/>s belongs
    /// </summary>
    private Presentation mPresentation;

		/// <summary>
		/// Default constructor
		/// </summary>
		public CoreNodeFactory()
		{
		}

		#region CoreNodeFactory Members

		/// <summary>
		/// Creates a new <see cref="CoreNode"/>
		/// </summary>
		/// <returns>The new <see cref="CoreNode"/></returns>
		/// <exception cref="exception.IsNotInitializedException">
		/// Thrown when the <see cref="ICorePresentation"/> of the 
		/// </exception>
		public CoreNode createNode()
		{
			return new CoreNode(getPresentation());
		}

		/// <summary>
		/// Creates a new <see cref="CoreNode"/> instance of <see cref="Type"/> matching a given QName
		/// </summary>
		/// <param name="localName">The local localName part of the QName</param>
		/// <param name="namespaceUri">The namespace uri part of the QName</param>
		/// <returns>The created <see cref="CoreNode"/> or <c>null</c> if the QN</returns>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when one of the parameters <paramref localName="localName"/> <paramref name="namespaceUri"/> and  is <c>null</c>
		/// </exception>
		public virtual CoreNode createNode(string localName, string namespaceUri)
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
		public Presentation getPresentation()
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
		/// <param name="pres">The presentation</param>
		/// <remarks>This method should only be used during initialization</remarks>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when parameter <paramref localName="pres"/> is <c>null</c>
		/// </exception>
		/// <exception cref="exception.IsAlreadyInitializedException">
		/// Thrown when the <see cref="Presentation"/> has already been set
		/// </exception>
		public void setPresentation(Presentation pres)
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
