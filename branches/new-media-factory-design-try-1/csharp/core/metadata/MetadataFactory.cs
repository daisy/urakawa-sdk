using System;
using System.Xml;
using urakawa.xuk;

namespace urakawa.metadata
{
    /// <summary>
    /// Default <see cref="Metadata"/> factory - supports creation of <see cref="Metadata"/> instances
    /// </summary>
    public class MetadataFactory : WithPresentation, IXukAble
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        protected internal MetadataFactory()
        {
        }

        /// <summary>
        /// Creates an <see cref="Metadata"/> matching a given QName
        /// </summary>
        /// <param name="localName">The local part of the QName</param>
        /// <param name="namespaceUri">The namespace uri part of the QName</param>
        /// <returns>The created <see cref="Metadata"/> instance or <c>null</c> if the given QName is not supported</returns>
        public Metadata CreateMetadata(string localName, string namespaceUri)
        {
            if (namespaceUri == XukAble.XUK_NS)
            {
                switch (localName)
                {
                    case "Metadata":
                        return CreateMetadata();
                }
            }
            return null;
        }

        /// <summary>
        /// Creates an <see cref="Metadata"/> instance
        /// </summary>
        /// <returns>The created instance</returns>
        public Metadata CreateMetadata()
        {
            return new Metadata();
        }
    }
}