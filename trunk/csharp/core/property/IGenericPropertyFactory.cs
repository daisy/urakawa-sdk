using System;
using urakawa.core;

namespace urakawa.property
{
    /// <summary>
    /// Interface for factories creating <see cref="Property"/>s
    /// </summary>
    public interface IGenericPropertyFactory : IWithPresentation
    {
        /// <summary>
        /// Creates a <see cref="Property"/> matching a given QName
        /// </summary>
        /// <param name="localName">The local part of the QName</param>
        /// <param name="namespaceUri">The namespace uri part of the QName</param>
        /// <returns>The created <see cref="Property"/> or <c>null</c> if the given QName is not supported</returns>
        Property CreateProperty(string localName, string namespaceUri);
    }
}