using System;
using System.Collections.Generic;
using System.Xml;

namespace urakawa.metadata
{
	/// <summary>
	/// Default implementation of 
	/// </summary>
	public class Metadata : IMetadata
	{
    private string mName;

		private Dictionary<string, string> mAttributes;

    /// <summary>
    /// Default constructor, Name, Content and Scheme are initialized to <see cref="String.Empty"/>
    /// </summary>
		internal Metadata()
		{
			mName = "";
			mAttributes = new Dictionary<string, string>();
			mAttributes.Add("Content", "");
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

		/// <summary>
		/// Gets the content
		/// </summary>
		/// <returns>The content</returns>
		public string getContent()
		{
			return mAttributes["Content"];
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
			if (newContent == null)
			{
				throw new exception.MethodParameterIsNullException(
					"Content can not be null");
			}
			mAttributes["Content"] = newContent;
		}


		/// <summary>
		/// Gets the value of a named attribute
		/// </summary>
		/// <param name="name">The name of the attribute</param>
		/// <returns>The value of the attribute - <see cref="String.Empty"/> if the attribute does not exist</returns>
		public string getOptionalAttributeValue(string name)
		{
			if (mAttributes.ContainsKey(name))
			{
				return mAttributes[name];
			}
			return "";
		}

		/// <summary>
		/// Sets the value of a named attribute
		/// </summary>
		/// <param name="name">The name of the attribute</param>
		/// <param name="value">The new value for the attribute</param>
		public void setOptionalAttributeValue(string name, string value)
		{
			if (value == null)
			{
				throw new exception.MethodParameterIsNullException(
					"A metadata attribute can not have null value");
			}
			if (mAttributes.ContainsKey(name))
			{
				mAttributes[name] = value;
			}
			else
			{
				mAttributes.Add(name, value);
			}
		}

		/// <summary>
		/// Gets the names of all attributes with non-empty names
		/// </summary>
		/// <returns>A <see cref="IList{string}"/> containing the attribute names</returns>
		public IList<string> getOptionalAttributeNames()
		{
			List<string> names = new List<string>(mAttributes.Keys);
			foreach (string name in names)
			{
				if (mAttributes[name] == "") names.Remove(name);
			}
			return names;
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
			setName(source.GetAttribute("Name"));
			mAttributes.Clear();
			mAttributes.Add("Content", "");
			bool moreAttrs = source.MoveToFirstAttribute();
			if (moreAttrs)
			{
				while (moreAttrs)
				{
					if (source.Name != "Name")
					{
						setOptionalAttributeValue(source.Name, source.Value);
					}
					moreAttrs = source.MoveToNextAttribute();
				}
				source.MoveToElement();
			}
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
      destination.WriteAttributeString("Name", getName());
			foreach (string name in getOptionalAttributeNames())
			{
				if (name != "Name")
				{
					destination.WriteAttributeString(name, getOptionalAttributeValue(name));
				}
			}
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
			if (getName() != other.getName()) return false;
			IList<string> names = getOptionalAttributeNames();
			IList<string> otherNames = getOptionalAttributeNames();
			if (names.Count != otherNames.Count) return false;
			foreach (string name in names)
			{
				if (!otherNames.Contains(name)) return false;
				if (getOptionalAttributeValue(name) != other.getOptionalAttributeValue(name)) return false;
			}
			return true;
		}

		#endregion
	}
}
