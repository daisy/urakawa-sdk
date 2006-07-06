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
    /// Creates a <see cref="IMetadata"/> instance of the given <see cref="MetadataType"/> string representation
    /// </summary>
    /// <param name="type">The given metadata type</param>
    /// <returns>The created instance</returns>
    public IMetadata createMetadata(string type)
    {
      switch (type)
      {
        case "Metadata":
          return new Metadata();
        default:
          throw new exception.OperationNotValidException(
            String.Format("Can not create IMetadata instance of type {0}", type));
      }
    }

    #endregion
  }
}
