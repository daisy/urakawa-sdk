using System;
using System.Collections.Generic;
using System.Xml;
using urakawa.xuk;

namespace urakawa.metadata
{
	/// <summary>
	/// Default implementation of 
	/// </summary>
	public class Metadata : IXukAble
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
			mAttributes.Add("content", "");
    }


    #region Metadata Members

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
			return mAttributes["content"];
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
			mAttributes["content"] = newContent;
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
			if (name == "name") setName(value);
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
		/// <returns>A <see cref="List{String}"/> containing the attribute names</returns>
		public List<string> getOptionalAttributeNames()
		{
			List<string> names = new List<string>(mAttributes.Keys);
			foreach (string name in new List<string>(names))
			{
				if (mAttributes[name] == "") names.Remove(name);
			}
			return names;
		}

    #endregion

		
		#region IXUKAble members

		/// <summary>
		/// Reads the <see cref="Metadata"/> from a Metadata xuk element
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
		public void xukIn(XmlReader source)
		{
			if (source == null)
			{
				throw new exception.MethodParameterIsNullException("Can not xukIn from an null source XmlReader");
			}
			if (source.NodeType != XmlNodeType.Element)
			{
				throw new exception.XukException("Can not read Metadata from a non-element node");
			}
			mAttributes.Clear();
			try
			{
				xukInAttributes(source);
				if (!source.IsEmptyElement)
				{
					while (source.Read())
					{
						if (source.NodeType == XmlNodeType.Element)
						{
							xukInChild(source);
						}
						else if (source.NodeType == XmlNodeType.EndElement)
						{
							break;
						}
						if (source.EOF) throw new exception.XukException("Unexpectedly reached EOF");
					}
				}

			}
			catch (exception.XukException e)
			{
				throw e;
			}
			catch (Exception e)
			{
				throw new exception.XukException(
					String.Format("An exception occured during xukIn of Metadata: {0}", e.Message),
					e);
			}
		}

		/// <summary>
		/// Reads the attributes of a Metadata xuk element.
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
		protected virtual void xukInAttributes(XmlReader source)
		{
			if (source.MoveToFirstAttribute())
			{
				bool moreAttrs = true;
				while (moreAttrs)
				{
					setOptionalAttributeValue(source.Name, source.Value);
					moreAttrs = source.MoveToNextAttribute();
				}
				source.MoveToElement();
			}
		}

		/// <summary>
		/// Reads a child of a Metadata xuk element. 
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
		protected virtual void xukInChild(XmlReader source)
		{
			bool readItem = false;
			// Read known children, when read set readItem to true


			if (!(readItem || source.IsEmptyElement))
			{
				source.ReadSubtree().Close();//Read past unknown child 
			}
		}

		/// <summary>
		/// Write a Metadata element to a XUK file representing the <see cref="Metadata"/> instance
		/// </summary>
		/// <param name="destination">The destination <see cref="XmlWriter"/></param>
		/// <param name="baseUri">
		/// The base <see cref="Uri"/> used to make written <see cref="Uri"/>s relative, 
		/// if <c>null</c> absolute <see cref="Uri"/>s are written
		/// </param>
		public void xukOut(XmlWriter destination, Uri baseUri)
		{
			if (destination == null)
			{
				throw new exception.MethodParameterIsNullException(
					"Can not xukOut to a null XmlWriter");
			}

			try
			{
				destination.WriteStartElement(getXukLocalName(), getXukNamespaceUri());
				xukOutAttributes(destination, baseUri);
				xukOutChildren(destination, baseUri);
				destination.WriteEndElement();

			}
			catch (exception.XukException e)
			{
				throw e;
			}
			catch (Exception e)
			{
				throw new exception.XukException(
					String.Format("An exception occured during xukOut of Metadata: {0}", e.Message),
					e);
			}
		}

		/// <summary>
		/// Writes the attributes of a Metadata element
		/// </summary>
		/// <param name="destination">The destination <see cref="XmlWriter"/></param>
		/// <param name="baseUri">
		/// The base <see cref="Uri"/> used to make written <see cref="Uri"/>s relative, 
		/// if <c>null</c> absolute <see cref="Uri"/>s are written
		/// </param>
		protected virtual void xukOutAttributes(XmlWriter destination, Uri baseUri)
		{
			destination.WriteAttributeString("name", getName());
			foreach (string a in getOptionalAttributeNames())
			{
				if (a != "name")
				{
					destination.WriteAttributeString(a, getOptionalAttributeValue(a));
				}
			}
		}

		/// <summary>
		/// Write the child elements of a Metadata element.
		/// </summary>
		/// <param name="destination">The destination <see cref="XmlWriter"/></param>
		/// <param name="baseUri">
		/// The base <see cref="Uri"/> used to make written <see cref="Uri"/>s relative, 
		/// if <c>null</c> absolute <see cref="Uri"/>s are written
		/// </param>
		protected virtual void xukOutChildren(XmlWriter destination, Uri baseUri)
		{

		}

		/// <summary>
		/// Gets the local name part of the QName representing a <see cref="Metadata"/> in Xuk
		/// </summary>
		/// <returns>The local name part</returns>
		public virtual string getXukLocalName()
		{
			return this.GetType().Name;
		}

		/// <summary>
		/// Gets the namespace uri part of the QName representing a <see cref="Metadata"/> in Xuk
		/// </summary>
		/// <returns>The namespace uri part</returns>
		public virtual string getXukNamespaceUri()
		{
			return urakawa.ToolkitSettings.XUK_NS;
		}

		#endregion


		#region IValueEquatable<Metadata> Members

		/// <summary>
		/// Determines if <c>this</c> is value equal to another given <see cref="Metadata"/>
		/// </summary>
		/// <param name="other">The other <see cref="Metadata"/></param>
		/// <returns>The result as a <see cref="bool"/></returns>
		public bool ValueEquals(Metadata other)
		{
			if (!(other is Metadata)) return false;
			Metadata mOther = (Metadata)other;
			if (getName() != other.getName()) return false;
			List<string> names = getOptionalAttributeNames();
			List<string> otherNames = getOptionalAttributeNames();
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
