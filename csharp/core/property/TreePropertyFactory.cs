using System;
using urakawa.core;

namespace urakawa.property
{
	/// <summary>
	/// Default implementation of <see cref="IGenericPropertyFactory"/> can not create any property.
	/// Use the built-in sub-class of <see cref="urakawa.property.PropertyFactory"/> that support creation of 
	/// <see cref="urakawa.property.channel.ChannelsProperty"/>s 
	/// and <see cref="urakawa.property.xml.XmlProperty"/>s.
	/// Alternatively the user should create their own sub-class of GenericPropertyFactory.
	/// </summary>
	public class GenericPropertyFactory : WithPresentation, IGenericPropertyFactory
	{
		/// <summary>
		/// Default constructor
		/// </summary>
		protected internal GenericPropertyFactory()
		{
		}

    #region IGenericPropertyFactory Members

    /// <summary>
    /// Creates a <see cref="Property"/> matching a given QName
    /// </summary>
		/// <param name="localName">The local part of the QName</param>
		/// <param name="namespaceUri">The namespace uri part of the QName</param>
		/// <returns>The created <see cref="Property"/> or <c>null</c> if the given QName is not supported
		/// (which is always hte case)
		/// </returns>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when <paramref name="localName"/> or <paramref name="namespaceUri"/> are <c>null</c>
		/// </exception>
    public virtual Property createProperty(string localName, string namespaceUri)
    {
			if (localName == null || namespaceUri == null)
			{
				throw new exception.MethodParameterIsNullException(
					"No part of the given QName can be null");
			}
			if (namespaceUri == ToolkitSettings.XUK_NS)
			{
				switch (localName)
				{
					case "Property":
						Property newProp = new Property();
						newProp.Presentation = Presentation;
						return newProp;
				}
			}
			return null;
    }

		#endregion
	}
}
