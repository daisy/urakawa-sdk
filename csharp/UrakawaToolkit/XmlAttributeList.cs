using System;

namespace urakawa.core
{
	/// <summary>
	/// Summary description for XmlAttributeList.
	/// </summary>
	public class XmlAttributeList:System.Collections.IList
	{
		private System.Collections.SortedList lstAttributes = new System.Collections.SortedList();

    public IXmlAttribute getAttributeByQName(string name, string ns)
    {
      foreach (IXmlAttribute a in this)
      {
        if (a.getName()==name && a.getNamespace()==ns)
        {
          return a;
        }
      }
      return null;
    }

		public int Add(IXmlAttribute newAttr)
		{
			XmlAttributeList tmpList =  this;
			tmpList += newAttr;
		}

		public void remove(IXmlAttribute removableAttr)
		{
			//TODO: confirmation that the item exists in the list, removal
			XmlAttributeList tmpList = this;
			tmpList -= removableAttr;
		}


		public IXmlAttribute this[int index]
		{
			get
			{
				return (IXmlAttribute)lstAttributes.GetByIndex(index);
			}
			set
			{
				lstAttributes[index] = new System.Collections.Dict value;
				XmlAttribute tmpAttr = (XmlAttribute)lstAttributes.GetByIndex(index);
				if(tmpAttr.getName() == value.getName() && tmpAttr.getNamespace() == value.getNamespace())
				{
					tmpAttr.setValue(value.getValue());
				}
			}
		}

		public static XmlAttributeList operator + (XmlAttributeList shouldBeThis, IXmlAttribute newAttr) 
		{
			int iAttrLocation = shouldBeThis.lstAttributes.IndexOfKey(newAttr.getNamespace()+":"+newAttr.getName());
			if(iAttrLocation == -1)
				shouldBeThis.lstAttributes.Add(newAttr.getNamespace()+":"+newAttr.getName(),newAttr);
			else
				throw(new urakawa.exception.NodeAlreadyExistException("An attribute named " + newAttr.getNamespace()+":"+newAttr.getName() +  " already exists"));
			return shouldBeThis;
		}

		public static XmlAttributeList operator - (XmlAttributeList shouldBeThis, IXmlAttribute removableAttr) 
		{
			int iAttrLocation = shouldBeThis.lstAttributes.IndexOfKey(removableAttr.getNamespace()+":"+removableAttr.getName());
			if(iAttrLocation>=0)
				shouldBeThis.lstAttributes.RemoveAt(iAttrLocation);
			else
				throw(new urakawa.exception.NodeDoesNotExistException("An attribute named " + removableAttr.getNamespace()+":"+removableAttr.getName() +  " does not exists"));
			return shouldBeThis;
		}



		public int Count
		{
			get
			{
				return lstAttributes.Count;
			}
		}
		#region IList Members

		int System.Collections.IList.Add(object o)
		{
			if (!typeof(IXmlAttribute).IsAssignableFrom(o.GetType()))
			{
				throw(new urakawa.exception.MethodParameterIsWrongTypeException("object must implement IXmlAttribute"));
			}
			this.Add((IXmlAttribute)o);

			return IndexOf(o); 
		}

		public bool IsReadOnly
		{
			get
			{
				
				return lstAttributes.IsReadOnly;
			}
		}

		object System.Collections.IList.this[int index]
		{
			get
			{
				return ((System.Collections.DictionaryEntry)lstAttributes.GetByIndex(index)).Value;
			}
			set
			{
				// TODO:  Add XmlAttributeList.System.Collections.IList.this setter implementation
				if(value.GetType() == typeof(XmlAttribute))
					this[index] = (XmlAttribute)value;
				else
					throw(new urakawa.exception.MethodParameterIsWrongTypeException("Expected an XmlAttribute"));
			}
		}

		public void RemoveAt(int index)
		{
			if(lstAttributes.Count<index)
				lstAttributes.RemoveAt(index);
			else
				throw(new urakawa.exception.MethodParameterIsOutOfBoundsException("No attribute exists at index " + index.ToString()));
		}

		public void Insert(int index, object value)
		{
			if(value.GetType() == typeof(XmlAttribute))
			{
				XmlAttributeList tmpList = this;
				tmpList += (XmlAttribute)value;
			}
		}

		public void Remove(object value)
		{
			if(value.GetType() == typeof(XmlAttribute))
			{
				string tmpKey = ((XmlAttribute)value).getNamespace() + ":" + ((XmlAttribute)value).getName();
				if(lstAttributes.ContainsKey(tmpKey))
					lstAttributes.Remove(tmpKey);
				else
					throw(new urakawa.exception.NodeDoesNotExistException("The requested XmlAttribute does not exist in thi XmlAttributeList"));

			}
			else
				throw(new urakawa.exception.MethodParameterIsWrongTypeException("The object to remove was not a XmlAttribute"));
		}

		public bool Contains(object value)
		{
			// TODO:  Add XmlAttributeList.Contains implementation
			return lstAttributes.ContainsValue(value);
		}

		public void Clear()
		{
			lstAttributes.Clear();
		}

		public int IndexOf(object value)
		{
			// TODO:  Add XmlAttributeList.IndexOf implementation
			return lstAttributes.IndexOfValue(value);
		}

		public bool IsFixedSize
		{
			get
			{
				// TODO:  Add XmlAttributeList.IsFixedSize getter implementation
				return lstAttributes.IsFixedSize;
			}
		}

		#endregion

		#region ICollection Members

		public bool IsSynchronized
		{
			get
			{
				return lstAttributes.IsSynchronized;
			}
		}

		public void CopyTo(Array array, int index)
		{
			lstAttributes.Values.CopyTo(array, index);
		}

		public object SyncRoot
		{
			get
			{
				// TODO:  Add XmlAttributeList.SyncRoot getter implementation
				return lstAttributes.SyncRoot;
			}
		}

		#endregion

		#region IEnumerable Members

		public System.Collections.IEnumerator GetEnumerator()
		{
			// TODO:  Add XmlAttributeList.GetEnumerator implementation
			
			return lstAttributes.Values.GetEnumerator();
		}

		#endregion
	}

}
