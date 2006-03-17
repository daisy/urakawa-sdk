using System;

namespace urakawa.core.property
{
	/// <summary>
	/// 
	/// </summary>
	public enum PropertyType
	{
		None,
		StructureProperty,
		SMILProperty,
		ChannelsProperty
	}

	public interface IProperty
	{
		PropertyType getType();
	}

	public class XmlProperty:IProperty
	{
		private string mName;
		private string mNameSpace;
		private System.Collections.SortedList mAttrList = new System.Collections.SortedList();

		public XmlProperty()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public string getName()
		{
			return mName;
		}
		public void setName(string newName)
		{
			mName = newName;
		}

		public string getNameSpace()
		{
			return mNameSpace;
		}
		public void setNameSpace(string newNameSpace)
		{
			mNameSpace = newNameSpace;
			
		}

		public string getAttributeValue(string attrName)
		{
			//TODO: the actual fetching!
			return "";
		}
		public string getAttributeValue(string attrName, string attrNameSpace)
		{
			string rVal = "";
				
			return "";
		}

		public void setAttributeValue(string attrName, string newValue)
		{
			if(!TestQName(attrName))
				throw(new NonAllowedQNameException());

			setAttributeValue(attrName,"",newValue);
		}
		public void setAttributeValue(string attrName, string attrNameSpace, string newValue)
		{
			if(!TestQName(attrName))
			{
				throw(new NonAllowedQNameException());
			}
			if(!(TestQName(attrNameSpace)|| attrNameSpace == ""))
			{
				throw(new NonAllowedQNameException());
			}

			string strLocalAttrName = attrNameSpace + ":" + attrName;
			int indexOfPreviousAttrValue = mAttrList.IndexOfKey(strLocalAttrName);
			if(indexOfPreviousAttrValue != -1)
			{
				mAttrList.Add(strLocalAttrName,newValue);
			}
			else
			{
				mAttrList.SetByIndex(indexOfPreviousAttrValue,newValue);
			}
		}


		public bool TestQName(string newQName)
		{
			//TODO: find out exactly what we want to allow for QNames
			//TODO: test that the supplied string folows those rules
			//TODO: test that this RegEx actually matches as expected!

			//marisa added .Count==0, it might work (didn't compile before)
			bool rVal = true;
			if(System.Text.RegularExpressions.Regex.Matches
				(newQName,"[_a-zA-Z]+[_a-zA-Z0-9]*").Count == 0)
				rVal = false;
			return rVal;

		}

		
		public PropertyType getType()
		{
			//TODO
			//something more clever; marisa did this to make it compile
			return PropertyType.None;
		}

	}

	public class NonAllowedQNameException: System.Exception
	{
		public NonAllowedQNameException()
		{
		}
		override public string Message
		{
			get
			{
				return "The supplied string did not match the RegEx '[_a-zA-Z]+[_a-zA-Z0-9]*'"; 
			}
		}
	}

	
}
