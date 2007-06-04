using System;
//using urakawa.properties.channel;
//using urakawa.properties.xml;

namespace urakawa.core.property
{
	/// <summary>
	/// Interface for factories creating <see cref="Property"/>s
	/// </summary>
	public interface ICorePropertyFactory
	{
		/// <summary>
		/// Creates a <see cref="Property"/> matching a given QName
		/// </summary>
		/// <param name="localName">The local part of the QName</param>
		/// <param name="namespaceUri">The namespace uri part of the QName</param>
		/// <returns>The created <see cref="Property"/> or <c>null</c> if the given QName is not supported</returns>
		Property createProperty(string localName, string namespaceUri);

		/// <summary>
		/// Gets the <see cref="ICorePresentation"/> associated with <c>this</c>
		/// </summary>
		/// <returns>The <see cref="ICorePresentation"/></returns>
		/// <exception cref="exception.IsNotInitializedException">
		/// Thrown when the property factory has not been initialized with a presentation
		/// </exception>
		ICorePresentation getPresentation();

		/// <summary>
		/// Sets the see cref="ICorePresentation"/> owns <see cref="TreeNode"/>s created by 
		/// the <c>this</c>. This method should only be used during initialization
		/// </summary>
		/// <param name="pres">The <see cref="ICorePresentation"/></param>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when parameter <paramref localName="pres"/> is <c>null</c>
		/// </exception>
		void setPresentation(ICorePresentation pres);
	}
}
