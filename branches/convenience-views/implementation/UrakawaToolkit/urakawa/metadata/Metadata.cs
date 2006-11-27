using System;
using System.Xml;

namespace urakawa.metadata
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

    #region IXUKAble Members

    /// <summary>
    /// Reads the <see cref="Metadata"/> instance from a XUK Metadata element
    /// </summary>
    /// <param name="source">The source <see cref="XmlReader"/></param>
    /// <returns>A <see cref="bool"/> indicating if the instance was succesfully read</returns>
    public bool XukIn(XmlReader source)
    {
      if (source == null)
      {
        throw new exception.MethodParameterIsNullException("Xml Reader is null");
      }
			if (source.NodeType != XmlNodeType.Element) return false;
      mName = source.GetAttribute("name");
      mContent = source.GetAttribute("content");
      mScheme = source.GetAttribute("scheme");
      if (mScheme==null) mScheme = "";
			if (source.IsEmptyElement) return true;
      while (source.Read())
      {
        if (source.NodeType==XmlNodeType.EndElement) break;
        if (source.EOF) return false;
      }
      return true;
    }

    /// <summary>
    /// Writes the instance to a XUK Metadata element
    /// </summary>
    /// <param name="destination">The destination <see cref="XmlWriter"/></param>
    /// <returns>A <see cref="bool"/> indicating success or failure</returns>
    public bool XukOut(XmlWriter destination)
    {
			destination.WriteStartElement(getXukLocalName(), getXukNamespaceUri());
      destination.WriteAttributeString("name", getName());
      destination.WriteAttributeString("content", getContent());
      destination.WriteAttributeString("scheme", getScheme());
      destination.WriteEndElement();
      return false;
    }

		
		/// <summary>
		/// Gets the local name part of the QName representing a <see cref="Metadata"/> in Xuk
		/// </summary>
		/// <returns>The local name part</returns>
		public string getXukLocalName()
		{
			return this.GetType().Name;
		}

		/// <summary>
		/// Gets the namespace uri part of the QName representing a <see cref="Metadata"/> in Xuk
		/// </summary>
		/// <returns>The namespace uri part</returns>
		public string getXukNamespaceUri()
		{
			return urakawa.ToolkitSettings.XUK_NS;
		}


    #endregion

		#region IValueEquatable<IMetadata> Members

		/// <summary>
		/// Determines if <c>this</c> is value equal to another given <see cref="IMetadata"/>
		/// </summary>
		/// <param name="other">The other <see cref="IMetadata"/></param>
		/// <returns>The result as a <see cref="bool"/></returns>
		public bool ValueEquals(IMetadata other)
		{
			if (!(other is Metadata)) return false;
			Metadata mOther = (Metadata)other;
			if (getName() != mOther.getName()) return false;
			if (getContent() != mOther.getContent()) return false;
			if (getScheme() != mOther.getScheme()) return false;
			return true;
		}

		#endregion
	}
}
