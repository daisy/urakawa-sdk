using System;
using System.Collections.Generic;
using System.Text;

namespace urakawa.media.data
{
	/// <summary>
	/// Interface for a factory creating <see cref="IDataProvider"/>s
	/// </summary>
	public interface IDataProviderFactory
	{
		/// <summary>
		/// Gets the <see cref="IDataProviderManager"/> associated with the <see cref="IDataProviderFactory"/>
		/// </summary>
		/// <returns>The <see cref="IDataProviderManager"/></returns>
		IDataProviderManager getDataProviderManager();

		/// <summary>
		/// Creates a <see cref="IDataProvider"/> instance of default <see cref="Type"/>
		/// </summary>
		/// <returns>The created instance</returns>
		IDataProvider createDataProvider();

		/// <summary>
		/// Creates a <see cref="IDataProvider"/> instance of <see cref="Type"/> matching a given XUK QName
		/// </summary>
		/// <param name="xukLocalName">The local name part of the given XUK QName</param>
		/// <param name="xukNamespaceUri">The namespace uri part of the given XUK QName</param>
		/// <returns>The created instance</returns>
		IDataProvider createDataProvider(string xukLocalName, string xukNamespaceUri);
	}
}
