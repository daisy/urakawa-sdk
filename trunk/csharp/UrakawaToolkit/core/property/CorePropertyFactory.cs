using System;
using urakawa.core;

namespace urakawa.core.property
{
	/// <summary>
	/// Default implementation of <see cref="ICorePropertyFactory"/> can not create any properties.
	/// Use the built-in sub-class of <see cref="urakawa.PropertyFactory"/> that support creation of 
	/// <see cref="urakawa.properties.channel.ChannelsProperty"/>s 
	/// and <see cref="urakawa.properties.xml.XmlProperty"/>s.
	/// Alternatively the user should create their own sub-class of CorePropertyFactory.
	/// </summary>
	public class CorePropertyFactory : ICorePropertyFactory
	{

    #region ICorePropertyFactory Members

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
						return new Property();
				}
			}
			return null;
    }

		private ICorePresentation mPresentation;

		/// <summary>
		/// Gets the <see cref="ICorePresentation"/> associated with this
		/// </summary>
		/// <returns>The <see cref="ICorePresentation"/></returns>
		/// <exception cref="exception.IsNotInitializedException">
		/// Thrown when the property factory has not been initialized with a presentation
		/// </exception>
		public ICorePresentation getPresentation()
		{
			if (mPresentation == null)
			{
				throw new exception.IsNotInitializedException(
					"The CorePropertyFactory has not been initialized with an associated ICorePresentation");
			}
			return mPresentation;
		}

		/// <summary>
		/// Initializer - sets the <see cref="ICorePresentation"/> that owns <see cref="ICoreNode"/>s created by 
		/// the <c>this</c>.
		/// </summary>
		/// <param name="pres">The <see cref="ICorePresentation"/></param>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when parameter <paramref name="pres"/> is <c>null</c>
		/// </exception>
		public virtual void setPresentation(ICorePresentation pres)
		{
			if (pres == null)
			{
				throw new exception.MethodParameterIsNullException("The associated ICorePresentation can not be null");
			}
			if (mPresentation != null)
			{
				throw new exception.IsAlreadyInitializedException(
					"The CorePropertyFactory has already been initialized with a ICorePresentation");
			}
			mPresentation = pres;
		}

		#endregion
	}
}
