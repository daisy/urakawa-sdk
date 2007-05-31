using System;

namespace urakawa.core
{
	/// <summary>
	/// Interface for a <see cref="CoreNode"/> factory
	/// </summary>
	public interface ICoreNodeFactory
	{
		/// <summary>
		/// Creates a new <see cref="CoreNode"/> instance of <see cref="Type"/> 
		/// that is default to the <see cref="ICoreNodeFactory"/>instance
		/// </summary>
		/// <returns>The new <see cref="CoreNode"/></returns>
		CoreNode createNode();

    /// <summary>
    /// Creates a new <see cref="CoreNode"/> instance of <see cref="Type"/> matching a given QName
    /// </summary>
		/// <param name="localName">The local localName part of the QName</param>
		/// <param name="namespaceUri">The namespace uri part of the QName</param>
    /// <returns>The new <see cref="CoreNode"/></returns>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when parameters <paramref localName="localName"/> or <paramref localName="namespaceUri"/> 
		/// are <c>null</c>
		/// </exception>
    CoreNode createNode(string localName, string namespaceUri);

		/// <summary>
		/// Gets the <see cref="ICorePresentation"/> owns <see cref="CoreNode"/>s created by 
		/// the <see cref="ICoreNodeFactory"/> instance
		/// </summary>
		/// <returns>The <see cref="ICorePresentation"/></returns>
		ICorePresentation getPresentation();

		/// <summary>
		/// Sets the see cref="ICorePresentation"/> owns <see cref="CoreNode"/>s created by 
		/// the <see cref="ICoreNodeFactory"/> instance. This method should only be used during initialization
		/// </summary>
		/// <param name="pres">The <see cref="ICorePresentation"/></param>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when parameter <paramref localName="pres"/> is <c>null</c>
		/// </exception>
		void setPresentation(ICorePresentation pres);

	}
}
