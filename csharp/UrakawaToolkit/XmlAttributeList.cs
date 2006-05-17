using System;

namespace urakawa.core
{
	/// <summary>
	/// Summary description for XmlAttributeList.
	/// </summary>
	public class XmlAttributeList
	{
		private System.Collections.SortedList lstAttributes = new System.Collections.SortedList();

		public void add(IXmlAttribute newAttr)
		{
			//TODO: adding to list, checking if it's already in the list
			this += newAttr;
		}

		public void remove(IXmlAttribute removableAttr)
		{
			//TODO: confirmation that the item exists in the list, removal
			this -= removableAttr;
		}


		public IXmlAttribute this[int index]
		{
			get
			{
				return (IXmlAttribute)lstAttributes.GetByIndex(index);
			}
			set
			{
				IXmlAttribute tmpAttr = (IXmlAttribute)lstAttributes.GetByIndex(index);
				if(tmpAttr.Name = value.Name && tmpAttr.Namespace == value.Namespace)
				{
					tmpAttr.Value = value.Value;
				}
			}
		}

		public static XmlAttributeList operator + (XmlAttributeList shouldBeThis, IXmlAttribute newAttr) 
		{
			if(lstAttributes.IndexOfKey(newAttr.Namespace+":"+newAttr.Name) == -1)
				lstAttributes.Add(newAttr.Namespace+":"+newAttr.Name,newAttr);
			else
				throw(new urakawa.exception.MethodParameterIsInvalidException());
			return shouldBeThis;
		}

		public static XmlAttributeList operator - (XmlAttributeList shouldBeThis, IXmlAttribute removableAttr) 
		{
			int iAttrLocation = lstAttributes.IndexOfKey(newAttr.Namespace+":"+newAttr.Name);
			if(iAttrLocation>=0)
				lstAttributes.RemoveAt(iAttrLocation);
			else
				throw(new urakawa.exception.MethodParameterIsInvalidException());
			return shouldBeThis;
		}



		public int Count
		{
			get
			{
				return lstAttributes.Count();
			}
		}
			

	}

}
