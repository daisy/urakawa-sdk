using System;

namespace urakawa.project
{  
  /// <summary>
  /// The different types of <see cref="IMetadata"/>
  /// </summary>
  public enum MetadataType
  {
    /// <summary>
    /// The default metadata type (XML style with Name, Content and Scheme triplets)
    /// </summary>
    DEFAULT
  };

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
    /// Creates a <see cref="IMetadata"/> instance of the given <see cref="MetadataType"/>
    /// </summary>
    /// <param name="type">The given <see cref="MetadataType"/></param>
    /// <returns>The created instance</returns>
    protected IMetadata createMetadata(MetadataType type)
    {
      switch (type)
      {
        case MetadataType.DEFAULT:
          return new Metadata();
        default:
          throw new exception.OperationNotValidException(
            String.Format("Can not create IMetadata instance of type {0:d}", type));
      }
    }

    /// <summary>
    /// Creates a <see cref="IMetadata"/> instance of the given <see cref="MetadataType"/> string representation
    /// </summary>
    /// <param name="type">The given <see cref="MetadataType"/> string representation</param>
    /// <returns>The created instance</returns>
    public IMetadata createMetadata(string typeString)
    {
      MetadataType type;
      if (typeString==null)
      {
        throw new exception.MethodParameterIsNullException("type string can not be null");
      }
      try
      {
        type = (MetadataType)Enum.Parse(typeof(MetadataType), typeString);
      }
      catch (ArgumentException)
      {
        throw new exception.OperationNotValidException(
          String.Format("{0} is not a valid MetadataType", typeString));
      }
      return createMetadata(type);
    }

    #endregion
  }
}
