using System;

namespace urakawa.project
{  
	/// <summary>
	/// Default <see cref="IMetadata"/> factory - supports creation of <see cref="Metadata"/> instances
	/// </summary>
	public class MetadataFactory : IMetadataFactory
	{
    /// <summary>
    /// Default constructor
    /// </summary>
		public MetadataFactory()
		{
    }
    #region IMetadataFactory Members

    /// <summary>
    /// Creates a <see cref="IMetadata"/> instance of the given type
    /// </summary>
    /// <param name="typeString">The string representation of the given metadata type</param>
    /// <returns>The created instance</returns>
    /// <exception cref="exception.OperationNotValidException">
    /// Thrown when the given type string representation is not recognized as a supported type
    /// </exception>
    public IMetadata createMetadata(string typeString)
    {
      switch (typeString)
      {
        case "Metadata":
          return new Metadata();
        default:
          throw new exception.OperationNotValidException(
            String.Format("Can not create IMetadata instance of type {0}", typeString));
      }
    }

    #endregion
  }
}
