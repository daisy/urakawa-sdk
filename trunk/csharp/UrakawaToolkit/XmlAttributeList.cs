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

		public void add(IXmlAttribute newAttr)
		{
			//TODO: adding to list, checking if it's already in the list
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

		public bool IsReadOnly
		{
			get
			{
				// TODO:  Add XmlAttributeList.IsReadOnly getter implementation
				return false;
			}
		}

		object System.Collections.IList.this[int index]
		{
			get
			{
				// TODO:  Add XmlAttributeList.System.Collections.IList.this getter implementation
				return lstAttributes.GetByIndex(index);
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
			return false;
		}

		public void Clear()
		{
			// TODO:  Add XmlAttributeList.Clear implementation
		}

		public int IndexOf(object value)
		{
			// TODO:  Add XmlAttributeList.IndexOf implementation
			return 0;
		}

		public int Add(object value)
		{
			// TODO:  Add XmlAttributeList.Add implementation
			return 0;
		}

		public bool IsFixedSize
		{
			get
			{
				// TODO:  Add XmlAttributeList.IsFixedSize getter implementation
				return false;
			}
		}

		#endregion

		#region ICollection Members

		public bool IsSynchronized
		{
			get
			{
				// TODO:  Add XmlAttributeList.IsSynchronized getter implementation
				return false;
			}
		}

		public void CopyTo(Array array, int index)
		{
			// TODO:  Add XmlAttributeList.CopyTo implementation
		}

		public object SyncRoot
		{
			get
			{
				// TODO:  Add XmlAttributeList.SyncRoot getter implementation
				return null;
			}
		}

		#endregion

		#region IEnumerable Members

		public System.Collections.IEnumerator GetEnumerator()
		{
			// TODO:  Add XmlAttributeList.GetEnumerator implementation
			return lstAttributes.GetEnumerator();
		}

		#endregion
	}

}
