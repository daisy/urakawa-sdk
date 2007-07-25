using System;
using urakawa.core;

namespace urakawa.property
{
	/// <summary>
	/// Interface for factories creating <see cref="Property"/>s
	/// </summary>
	public interface IGenericPropertyFactory
	{
		/// <summary>
		/// Creates a <see cref="Property"/> matching a given QName
		/// </summary>
		/// <param name="localName">The local part of the QName</param>
		/// <param name="namespaceUri">The namespace uri part of the QName</param>
		/// <returns>The created <see cref="Property"/> or <c>null</c> if the given QName is not supported</returns>
		Property createProperty(string localName, string namespaceUri);

		/// <summary>
		/// Gets the <see cref="Presentation"/> associated with <c>this</c>
		/// </summary>
		/// <returns>The <see cref="Presentation"/></returns>
		/// <exception cref="exception.IsNotInitializedException">
		/// Thrown when the property factory has not been initialized with a presentation
		/// </exception>
		Presentation getPresentation();

		/// <summary>
		/// Sets the see <cref="Presentation"/> owns <see cref="TreeNode"/>s created by 
		/// the <c>this</c>. This method should only be used during initialization
		/// </summary>
		/// <param name="pres">The <see cref="Presentation"/></param>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when parameter <paramref localName="pres"/> is <c>null</c>
		/// </exception>
		void setPresentation(Presentation pres);
	}
}
