using System;
using urakawa.media;

namespace urakawa.core
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	public class Channel : IChannel
	{
    private string mName;

    /// <summary>
    /// Holds the supported <see cref="MediaType"/> for the channel,
    /// the value <see cref="MediaType.EMPTY_SEQUENCE"/> signifies that 
    /// all <see cref="MediaType"/>s are supported 
    /// </summary>
    private MediaType mSupportedMediaType = MediaType.EMPTY_SEQUENCE;

    /// <summary>
    /// Sets the <see cref="MediaType"/> supported by the <see cref="Channel"/>
    /// </summary>
    /// <param name="newType">The new <see cref="MediaType"/> supported</param>
    /// <exception cref="exception.MediaTypeIsIllegalException">
    /// Thrown when the <see cref="Channel"/> has already been assigned 
    /// a <see cref="MediaType"/> to support that is different from <paramref name="newType"/>. 
    /// Alternatively if <paramref name="newType"/> has the illegal 
    /// value <see cref="MediaType.EMPTY_SEQUENCE"/>
    /// </exception>
    public void setSupportedMediaType(MediaType newType)
    {
      if (newType==MediaType.EMPTY_SEQUENCE)
      {
        throw new exception.MediaTypeIsIllegalException(
          "A Channel can not support the EMPTY_SEQUENCE media type");
      }
      if (!isMediaTypeSupported(newType))
      {
        throw new exception.MediaTypeIsIllegalException(String.Format(
          "The media type {0:d} is illegal because the Channel currently "
          +"supports the media type {0:d}",
          newType,
          mSupportedMediaType));
      }
      mSupportedMediaType = newType;
    }

    internal Channel(string name)
    {
      mName = name;
    }
    #region IChannel Members

    /// <summary>
    /// Sets the name of the <see cref="IChannel"/>
    /// </summary>
    /// <param name="name">The new name</param>
    /// <exception cref="exception.MethodParameterIsNullException">
    /// Thrown when <paramref name="name"/> is null
    /// </exception>
    /// <exception cref="exception.MethodParameterIsEmptyStringException">
    /// Thrown when <paramref name="name"/> is an empty string
    /// </exception>
    public void setName(string name)
    {
      if (mName==null) 
      {
        throw new exception.MethodParameterIsNullException(
          "Can not set channel name to null");
      }
      if (mName==String.Empty)
      {
        throw new exception.MethodParameterIsEmptyStringException(
          "Can not set channel name to the empty string");
      }
      mName = name;
    }

    /// <summary>
    /// Gets the name of the <see cref="IChannel"/>
    /// </summary>
    /// <returns>The name</returns>
    public string getName()
    {
      return mName;
    }

    /// <summary>
    /// Checks of a given <see cref="MediaType"/> is supported by the channel
    /// </summary>
    /// <param name="type">The <see cref="MediaType"/></param>
    /// <returns>A <see cref="bool"/> indicating if the <see cref="MediaType"/>
    /// is supported</returns>
    public bool isMediaTypeSupported(MediaType type)
    {
      if (mSupportedMdiaType==MediaType.EMPTY_SEQUENCE) return true;
      return (type==mSupportedMdiaType);
    }

    #endregion

		#region IXUKable members 

		public bool XUKin(System.Xml.XmlReader source)
		{
			//TODO: actual implementation, for now we return false as default, signifying that all was not done
			return false;
		}

		public bool XUKout(System.Xml.XmlWriter destination)
		{
			//TODO: actual implementation, for now we return false as default, signifying that all was not done
			return false;
		}
		#endregion

  }
}
