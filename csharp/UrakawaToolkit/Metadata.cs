using System;
using System.Xml;

namespace urakawa.project
{
	/// <summary>
	/// Default implementation of 
	/// </summary>
	public class Metadata : IMetadata
	{
    private string mName;
    private string mContent;
    private string mScheme;

    /// <summary>
    /// Default constructor, Name, Content and Scheme are initialized to <see cref="String.Empty"/>
    /// </summary>
		internal Metadata()
		{
			mName = "";
      mContent = "";
      mScheme = "";
    }

    /// <summary>
    /// Gets the content
    /// </summary>
    /// <returns>The content</returns>
    public string getContent()
    {
      return mContent;
    }

    /// <summary>
    /// Sets the content
    /// </summary>
    /// <param name="newContent">The  new content value</param>
    /// <exception cref="exception.MethodParameterIsNullException">
    /// Thrown when <paramref name="newContent"/> is null
    /// </exception>
    public void setContent(string newContent)
    {
      if (newContent==null)
      {
        throw new exception.MethodParameterIsNullException(
          "Content can not be null");
      }
      mContent = newContent;
    }

    /// <summary>
    /// Gets the scheme
    /// </summary>
    /// <returns>The scheme</returns>
    public string getScheme()
    {
      return mScheme;
    }

    /// <summary>
    /// Sets the scheme
    /// </summary>
    /// <param name="newScheme">The new Scheme value</param>
    /// <exception cref="exception.MethodParameterIsNullException">
    /// Thrown when <paramref name="newScheme"/> is null
    /// </exception>
    public void setScheme(string newScheme)
    {
      if (newScheme==null)
      {
        throw new exception.MethodParameterIsNullException(
          "Scheme can not be null");
      }
      mScheme = newScheme;
    }

		/// <summary>
		/// Tests if two <see cref="Metadata"/> instances are equal
		/// </summary>
		/// <param name="m1">The first <see cref="Metadata"/> instance</param>
		/// <param name="m2">The second <see cref="Metadata"/> instance</param>
		/// <returns></returns>
		public static bool AreEqual(Metadata m1, Metadata m2)
		{
			if (m1.getName()!=m2.getName()) return false;
			if (m1.getContent()!=m2.getContent()) return false;
			if (m1.getScheme()!=m2.getScheme()) return false;
			return true;
		}


    #region IMetadata Members

    /// <summary>
    /// Gets the name
    /// </summary>
    /// <returns>The name</returns>
    public string getName()
    {
      return mName;
    }

    /// <summary>
    /// Sets the name
    /// </summary>
    /// <param name="newName">The new name value</param>
    /// <exception cref="exception.MethodParameterIsNullException">
    /// Thrown when <paramref name="newName"/> is null
    /// </exception>
    public void setName(string newName)
    {
      if (newName==null)
      {
        throw new exception.MethodParameterIsNullException(
          "The name can not be null");
      }
      mName = newName;
    }

    #endregion

    #region IXUKable Members

    /// <summary>
    /// Reads the <see cref="Metadata"/> instance from a XUK Metadata element
    /// </summary>
    /// <param name="source">The source <see cref="XmlReader"/></param>
    /// <returns>A <see cref="bool"/> indicating if the instance was succesfully read</returns>
    public bool XUKin(XmlReader source)
    {
      if (source == null)
      {
        throw new exception.MethodParameterIsNullException("Xml Reader is null");
      }

      if (!(source.Name == "Metadata" &&
        source.NodeType == System.Xml.XmlNodeType.Element))
      {
        return false;
      }

      mName = source.GetAttribute("name");
      mContent = source.GetAttribute("content");
      mScheme = source.GetAttribute("scheme");
      if (mScheme==null) mScheme = "";
      if (!source.IsEmptyElement)
      {
        while (source.Read())
        {
          if (source.NodeType==XmlNodeType.EndElement) return true;
          if (source.EOF) return false;
        }
      }
      return true;
    }

    /// <summary>
    /// Writes the instance to a XUK Metadata element
    /// </summary>
    /// <param name="destination">The destination <see cref="XmlWriter"/></param>
    /// <returns>A <see cref="bool"/> indicating success or failure</returns>
    public bool XUKout(XmlWriter destination)
    {
      destination.WriteStartElement("Metadata");
      destination.WriteAttributeString("name", getName());
      destination.WriteAttributeString("content", getContent());
      destination.WriteAttributeString("scheme", getScheme());
      destination.WriteEndElement();
      return false;
    }

    #endregion
  }
}
