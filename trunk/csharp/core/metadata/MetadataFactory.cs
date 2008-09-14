using System;
using System.Xml;
using urakawa.xuk;

namespace urakawa.metadata
{
    /// <summary>
    /// Default <see cref="Metadata"/> factory - supports creation of <see cref="Metadata"/> instances
    /// </summary>
    public class MetadataFactory : GenericFactory<Metadata>
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        protected internal MetadataFactory()
        {
        }

        /// <summary>
        /// Creates an <see cref="Metadata"/> instance
        /// </summary>
        /// <returns>The created instance</returns>
        public Metadata CreateMetadata()
        {
            return Create<Metadata>();
        }
    }
}