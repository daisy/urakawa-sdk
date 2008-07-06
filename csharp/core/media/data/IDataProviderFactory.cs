using System;
using System.Collections.Generic;
using System.Text;
using urakawa.xuk;

namespace urakawa.media.data
{
    /// <summary>
    /// Interface for a factory creating <see cref="IDataProvider"/>s
    /// </summary>
    public interface DataProviderFactory : IWithPresentation, IXukAble
    {
        /// <summary>
        /// Gets the <see cref="IDataProviderManager"/> associated with the <see cref="IDataProviderFactory"/>
        /// </summary>
        /// <returns>The <see cref="IDataProviderManager"/></returns>
        IDataProviderManager DataProviderManager { get; }

        /// <summary>
        /// Creates a <see cref="IDataProvider"/> instance of default type for a given MIME type
        /// </summary>
        /// <param name="mimeType">The given MIME type</param>
        /// <returns>The created instance</returns>
        IDataProvider CreateDataProvider(string mimeType);

        /// <summary>
        /// Creates a <see cref="IDataProvider"/> instance of type matching a given XUK QName
        /// for a given MIME type
        /// </summary>
        /// <param name="mimeType">The given MIME type</param>
        /// <param name="xukLocalName">The local name part of the given XUK QName</param>
        /// <param name="xukNamespaceUri">The namespace uri part of the given XUK QName</param>
        /// <returns>The created instance</returns>
        IDataProvider CreateDataProvider(string mimeType, string xukLocalName, string xukNamespaceUri);
    }
}