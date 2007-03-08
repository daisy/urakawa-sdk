using System;
using System.Collections;

namespace urakawa.properties.xml
{
  /// <summary>
  /// Summary description for XmlAttributeList.
  /// </summary>
  public class XmlAttributeList : IList
  {
    ArrayList mAttributes = new ArrayList();

    /// <summary>
    /// Adds an <see cref="IXmlAttribute"/> to a <see cref="XmlAttributeList"/>
    /// </summary>
    /// <param name="shouldBeThis">
    /// The <see cref="XmlAttributeList"/> to which to add an <see cref="IXmlAttribute"/>
    /// </param>
    /// <param name="newAttr">
    /// The <see cref="IXmlAttribute"/> to add
    /// </param>
    /// <returns>
    /// The <see cref="XmlAttributeList"/> to which the <see cref="IXmlAttribute"/> was added
    /// </returns>
    public static XmlAttributeList operator + (XmlAttributeList shouldBeThis, IXmlAttribute newAttr)
    {
      shouldBeThis.Add(newAttr);
      return shouldBeThis;
    }

    /// <summary>
    /// Removes an <see cref="IXmlAttribute"/> from a <see cref="XmlAttributeList"/>
    /// </summary>
    /// <param name="shouldBeThis">
    /// The <see cref="XmlAttributeList"/> from which to remove an <see cref="IXmlAttribute"/>
    /// </param>
    /// <param name="oldAttr">
    /// The <see cref="IXmlAttribute"/> to remove
    /// </param>
    /// <returns>
    /// The <see cref="XmlAttributeList"/> from which an <see cref="IXmlAttribute"/> was removed
    /// </returns>
    public static XmlAttributeList operator - (XmlAttributeList shouldBeThis, IXmlAttribute oldAttr)
    {
      shouldBeThis.Remove(oldAttr);
      return shouldBeThis;
    }

    /// <summary>
    /// Default indexed - retrieves <see cref="IXmlAttribute"/>s by index
    /// </summary>
    public IXmlAttribute this[int index]
    {
      get
      {
        return (IXmlAttribute)mAttributes[index];
      }
      set
      {
        if (value == null)
        {
          mAttributes.RemoveAt(index);
        }
        else
        {
          IXmlAttribute a = getByQName(value.getLocalName(), value.getNamespaceUri());
          if (a==null || mAttributes.IndexOf(a)==index)
          {
            mAttributes[index] = value;
          }
          else 
          {
            throw new exception.NodeAlreadyExistException(
              "Setting the given attribute at the given index would "
              +"cause the attribute list to have duplicate QNames");
          }
        }
      }
    }

    /// <summary>
    /// Gets an <see cref="IXmlAttribute"/> by it's QName
    /// </summary>
		/// <param name="name">The local localName part of the QName</param>
		/// <param name="ns">The namespace part of the QName</param>
    /// <returns>
    /// The <see cref="IXmlAttribute"/> with the given QName, 
    /// <c>null</c> if no such <see cref="IXmlAttribute"/> exists
    /// </returns>
    public IXmlAttribute getByQName(string name, string ns)
    {
      foreach (IXmlAttribute a in this)
      {
        if (a.getLocalName()==name && a.getNamespaceUri()==ns) return a;
      }
      return null;
    }

    /// <summary>
    /// Inserts a given <see cref="IXmlAttribute"/> at a given index. 
    /// </summary>
    /// <param name="index">The given index. Must be between <c>0</c> and <c><see cref="Count"/></c>. </param>
    /// <param name="attr">The given <see cref="IXmlAttribute"/></param>
    /// <exception cref="exception.MethodParameterIsNullException">
    /// Thrown when <paramref localName="attr"/> is null
    /// </exception>
    /// <exception cref="exception.NodeAlreadyExistException">
    /// Thrown when another <see cref="IXmlAttribute"/> already exists with the sane QName
    /// </exception>
    /// <exception cref="exception.MethodParameterIsOutOfBoundsException">
    /// Thrown when <paramref localName="index"/> is out of bounds
    /// </exception>
    public void Insert(int index, IXmlAttribute attr)
    {
      if (attr==null)
      {
        throw new exception.MethodParameterIsNullException(
          "The IXmlAttribute to insert must not be null");
      }
      if (getByQName(attr.getLocalName(), attr.getNamespaceUri())!=null)
      {
        throw new exception.NodeAlreadyExistException(
          "Inserting the given attribute at the given index would "
          +"cause the attribute list to have duplicate QNames");
      }
      try
      {
        mAttributes.Insert(index, attr);
      }
      catch (ArgumentOutOfRangeException e)
      {
        throw new exception.MethodParameterIsOutOfBoundsException(
          "The index at which to insert was out of bounds of the XmlAttributeList", e);
      }
    }

    /// <summary>
    /// Removes a given <see cref="IXmlAttribute"/>
    /// </summary>
    /// <param name="attr">The <see cref="IXmlAttribute"/> to remove</param>
    public void Remove(IXmlAttribute attr)
    {
      mAttributes.Remove(attr);
    }

    /// <summary>
    /// Adds a <see cref="IXmlAttribute"/>
    /// </summary>
    /// <param name="attr">The <see cref="IXmlAttribute"/> to add</param>
    /// <returns>The index at which the <see cref="IXmlAttribute"/> was added</returns>
    /// <exception cref="exception.MethodParameterIsNullException">
    /// Thrown when <paramref localName="attr"/> is null
    /// </exception>
    /// <exception cref="exception.NodeAlreadyExistException">
    /// Thrown when another <see cref="IXmlAttribute"/> already exists with the sane QName
    /// </exception>
    public int Add(IXmlAttribute attr)
    {
      Insert(mAttributes.Count, attr);
      return mAttributes.Count-1;
    }

    /// <summary>
    /// Determines if the list contains a given <see cref="IXmlAttribute"/>
    /// </summary>
    /// <param name="attr">The given <see cref="IXmlAttribute"/></param>
    /// <returns>
    /// A <see cref="bool"/> indicating if the list contains the given <see cref="IXmlAttribute"/>
    /// </returns>
    public bool Contains(IXmlAttribute attr)
    {
      return mAttributes.Contains(attr);
    }

    #region IList Members

    /// <summary>
    /// Gets a <see cref="bool"/> indicating if the <see cref="XmlAttributeList"/> is read-only
    /// (which it never is)
    /// </summary>
    public bool IsReadOnly
    {
      get
      {
        return false;
      }
    }

    object IList.this[int index]
    {
      get
      {
        return this[index];
      }
      set
      {
        if (!typeof(IXmlAttribute).IsAssignableFrom(value.GetType()))
        {
          throw new exception.MethodParameterIsWrongTypeException(
            "The underlying XmlAttributeList can only contain IXmlAttrubutes");
        }
        this[index] = (IXmlAttribute)value;
      }
    }

    /// <summary>
    /// Removes the 
    /// </summary>
    /// <param name="index"></param>
    public void RemoveAt(int index)
    {
      try
      {
        mAttributes.RemoveAt(index);
      }
      catch (ArgumentOutOfRangeException e)
      {
        throw new exception.MethodParameterIsOutOfBoundsException(
          "Index at which to remove is out of bounds", e);
      }
    }

    void IList.Insert(int index, object value)
    {
      if (!typeof(IXmlAttribute).IsAssignableFrom(value.GetType()))
      {
        throw new exception.MethodParameterIsWrongTypeException(
          "The underlying XmlAttributeList can only contain IXmlAttrubutes");
      }
      Insert(index, (IXmlAttribute)value);
    }

    void IList.Remove(object value)
    {
      if (!typeof(IXmlAttribute).IsAssignableFrom(value.GetType()))
      {
        throw new exception.MethodParameterIsWrongTypeException(
          "The underlying XmlAttributeList can only contain IXmlAttrubutes");
      }
      Remove((IXmlAttribute)value);
    }


    bool IList.Contains(object value)
    {
      if (!typeof(IXmlAttribute).IsAssignableFrom(value.GetType()))
      {
        return false;
      }
      return this.Contains((IXmlAttribute)value);
    }

    /// <summary>
    /// Clears the <see cref="IXmlAttribute"/>s from the list
    /// </summary>
    public void Clear()
    {
      mAttributes.Clear();
    }

    int IList.IndexOf(object value)
    {
      return mAttributes.IndexOf(value);
    }

    int IList.Add(object value)
    {
      if (!typeof(IXmlAttribute).IsAssignableFrom(value.GetType()))
      {
        throw new exception.MethodParameterIsWrongTypeException(
          "The underlying XmlAttributeList can only contain IXmlAttributes");
      }
      
      return Add((IXmlAttribute)value);
    }

    /// <summary>
    /// Gets a <see cref="bool"/> indicating if the list has fixed size (which it never has)
    /// </summary>
    public bool IsFixedSize
    {
      get
      {
        return mAttributes.IsFixedSize;
      }
    }

    #endregion

    #region ICollection Members

    /// <summary>
    /// Gets a <see cref="bool"/> indicating if th list is syncronized
    /// </summary>
    public bool IsSynchronized
    {
      get
      {
        return mAttributes.IsSynchronized;
      }
    }

    /// <summary>
    /// Gets the number of <see cref="IXmlAttribute"/>s in the list
    /// </summary>
    public int Count
    {
      get
      {
        return mAttributes.Count;
      }
    }

    /// <summary>
    /// Copies the <see cref="IXmlAttribute"/>s in the list to an <see cref="Array"/>
    /// </summary>
    /// <param name="array">The destination <see cref="Array"/></param>
    /// <param name="index">The index in the destination <see cref="Array"/> at which to start the copy</param>
    public void CopyTo(Array array, int index)
    {
      mAttributes.CopyTo(array, index);
    }

    /// <summary>
    /// An object that can be used to synchronize access to 
    /// the <see cref="XmlAttributeList"/>
    /// </summary>
    public object SyncRoot
    {
      get
      {
        return mAttributes.SyncRoot;
      }
    }

    #endregion

    #region IEnumerable Members

    /// <summary>
    /// Gets an <see cref="IEnumerator"/> for the <see cref="XmlAttributeList"/>
    /// </summary>
    /// <returns>The <see cref="IEnumerator"/></returns>
    public IEnumerator GetEnumerator()
    {
      return mAttributes.GetEnumerator();
    }

    #endregion
  }

}
