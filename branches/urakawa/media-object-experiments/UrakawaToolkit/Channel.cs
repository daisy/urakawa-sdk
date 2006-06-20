using System;
using System.Xml;
using urakawa.media;

namespace urakawa.core
{
	/// <summary>
	/// Default implementation of the <see cref="IChannel"/> interface
	/// </summary>
	public class Channel : IChannel
	{
    private string mName;

	  //this ID is needed to track data read in from the XML file
	  //as far as marisa knows right now, none of our code ensures its uniqueness
	  private string mId;

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
    public void setName(string name)
    {
      if (mName==null) 
      {
        throw new exception.MethodParameterIsNullException(
          "Can not set channel name to null");
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
      if (mSupportedMediaType==MediaType.EMPTY_SEQUENCE) return true;
      return (type==mSupportedMediaType);
    }

    #endregion


	  #region IXUKable members 

	  //marisa's comment: i don't think this one is required
    /// <summary>
    /// Reads the <see cref="Channel"/> from a Channel element in a XUK document
    /// </summary>
    /// <param name="source">An <see cref="XmlReader"/> from which to read the Channel element</param>
    /// <returns>A <see cref="bool"/> indicating if the <see cref="Channel"/> was succesfully read</returns>
	  public bool XUKin(System.Xml.XmlReader source)
	  {
      if (source == null)
      {
        throw new exception.MethodParameterIsNullException("Xml Reader is null");
      }
      if (!(source.Name == "Channel" && 
        source.NodeType == System.Xml.XmlNodeType.Element))
      {
        return false;
      }
      string id = source.GetAttribute("id");
      if (id==null || id=="") return false;
      setId(id);
      if (source.IsEmptyElement) return false;
      bool bFoundError = false;
      string name = "";
      if (!source.IsEmptyElement)
      {
        while (source.Read())
        {
          if (source.NodeType==XmlNodeType.Text)
          {
            name += source.Value;
          }
          else if (source.NodeType==XmlNodeType.SignificantWhitespace)
          {
            name += source.Value;
          }
          else if (source.NodeType==XmlNodeType.Element)
          {
            bFoundError = true;
          }
          else if (source.NodeType==XmlNodeType.EndElement)
          {
            break;
          }

          if (source.EOF) break;
          if (bFoundError) break;
        }
      }
      if (!bFoundError)
      {
        setName(name);
        return true;
      }
      else
      {
        return false;
      }
    }

    /// <summary>
    /// Writes the <see cref="Channel"/> to a Channel element in a XUK document
    /// </summary>
    /// <param name="destination"></param>
    /// <returns></returns>
	  public bool XUKout(System.Xml.XmlWriter destination)
	  {
		  bool bWroteChannel = true;

		  destination.WriteStartElement("Channel");

		  if (mId != "")
		  {
			  destination.WriteAttributeString("id", mId);
		  }
		  else
		  {
			  bWroteChannel = false;
		  }

		  destination.WriteString(this.mName);

		  destination.WriteEndElement();

		  return bWroteChannel;
	  }
	  #endregion

	  internal void setId(string id)
	  {
		  mId = id;
	  }

    /// <summary>
    /// Gets the id of the channel, corresponding to the id attribute in the XUK file
    /// </summary>
    /// <returns>The id</returns>
	  public string getId()
	  {
		  return mId;
	  }
  }
}
