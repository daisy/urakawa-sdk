using System;
using System.Collections;

namespace urakawa.core
{
  /// <summary>
  /// Summary description for XmlAttributeList.
  /// </summary>
  public class XmlAttributeList : IList
  {
    ArrayList mAttributes = new ArrayList();

    public static XmlAttributeList operator + (XmlAttributeList shouldBeThis, IXmlAttribute newAttr)
    {
      shouldBeThis.Add(newAttr);
      return shouldBeThis;
    }

    public static XmlAttributeList operator - (XmlAttributeList shouldBeThis, IXmlAttribute oldAttr)
    {
      shouldBeThis.Remove(oldAttr);
      return shouldBeThis;
    }

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
          IXmlAttribute a = getByQName(value.getName(), value.getNamespace());
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

    public IXmlAttribute getByQName(string name, string ns)
    {
      foreach (IXmlAttribute a in this)
      {
        if (a.getName()==name && a.getNamespace()==ns) return a;
      }
      return null;
    }

    public void Insert(int index, IXmlAttribute attr)
    {
      if (getByQName(attr.getName(), attr.getNamespace())!=null)
      {
        throw new exception.NodeAlreadyExistException(
          "Inserting the given attribute at the given index would "
          +"cause the attribute list to have duplicate QNames");
      }
      mAttributes.Insert(index, attr);
    }

    public void Remove(IXmlAttribute attr)
    {
      mAttributes.Remove(attr);
    }

    public int Add(IXmlAttribute attr)
    {
      Insert(mAttributes.Count, attr);
      return mAttributes.Count-1;
    }

    #region IList Members

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
        // TODO:  Add XmlAttributeList.this getter implementation
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

    public void RemoveAt(int index)
    {
      mAttributes.RemoveAt(index);
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

    public bool Contains(object value)
    {
      // TODO:  Add XmlAttributeList.Contains implementation
      return mAttributes.Contains(value);
    }

    public void Clear()
    {
      mAttributes.Clear();
    }

    int IList.IndexOf(object value)
    {
      return mAttributes.IndexOf(value);
    }

    public int Add(object value)
    {
      ((IList)this).Insert(mAttributes.Count, value);
      return mAttributes.Count-1;
    }

    public bool IsFixedSize
    {
      get
      {
        return mAttributes.IsFixedSize;
      }
    }

    #endregion

    #region ICollection Members

    public bool IsSynchronized
    {
      get
      {
        return mAttributes.IsSynchronized;
      }
    }

    public int Count
    {
      get
      {
        return mAttributes.Count;
      }
    }

    public void CopyTo(Array array, int index)
    {
      mAttributes.CopyTo(array, index);
    }

    public object SyncRoot
    {
      get
      {
        return mAttributes.SyncRoot;
      }
    }

    #endregion

    #region IEnumerable Members

    public IEnumerator GetEnumerator()
    {
      return mAttributes.GetEnumerator();
    }

    #endregion
  }

}
