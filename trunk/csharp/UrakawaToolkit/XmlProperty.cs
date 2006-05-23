using System;
using urakawa.exception;

namespace urakawa.core
{
	/// <summary>
	/// 
	/// </summary>
	public class XmlProperty : IXmlProperty, IXmlPropertyValidator	
	{
		private string mName;
		private string mNamespace;
		private XmlAttributeList mAttributes = new XmlAttributeList();
		private ICoreNode mOwner;

		internal XmlProperty(ICoreNode owner, string newNamespace, string newName)
		{
			if(owner != null)
				mOwner = owner;
			else
				throw(new urakawa.exception.MethodParameterIsNullException("XmlProperty needs an owner"));

			if(newNamespace!=null)
				mNamespace = newNamespace;
			else
				throw(new urakawa.exception.MethodParameterIsNullException("XmlProperty cannot have null for namespace. Empty string is allowed"));

			if(newName!=null)
			{
				if(newName!="")
					mName = newName;
				else
					throw(new urakawa.exception.MethodParameterIsEmptyStringException("XmlProperty needs a name."));
			}
			else
				throw(new urakawa.exception.MethodParameterIsNullException("XmlProperty cannot have null for name."));

		}

		IProperty IProperty.copy()
		{
			return this.copy();
		}

		//dummy function
		public XmlProperty copy()
		{
			//TODO: implement actual copying!
			return null;
		}

		public XmlProperty()
		{

		}

		public string getName()
		{
			return mName;
		}
		public void setName(string newName)
		{
			mName = newName;
		}

		public string getNamespace()
		{
			return mNameSpace;
		}
		public void setNamespace(string newNameSpace)
		{
			mNameSpace = newNameSpace;
			
		}

		System.Collections.IList IXmlProperty.getListOfAttributes()
		{
			return mAttributes;
		}

		public XmlAttributeList getListOfAttributes()
		{
			return mAttributes;
		}

		public XMLType getXMLType()
		{
			//TODO: explain to me why we even need XMLType.TEXT???
			return XMLType.ELEMENT;
		}


/*
 * Old code to be discarded when I'm confident that the new way works (someone plz convince me that SVN is full enough of features that I don't need to do this!)
 * 
		public string getAttributeValue(string attrName)
		{
			string rVal = getAttributeValue(attrName,"");
			return rVal;
		}
		public string getAttributeValue(string attrName, string attrNameSpace)
		{
			string rVal = "";
			if(attrNameSpace!="")
				if(!TestQName(attrNameSpace))
					throw(new NonAllowedQNameException());
			if(!TestQName(attrName))
				throw(new NonAllowedQNameException());

			//both name parts are allowed, so let's see if we have anything matching
			string tmpCompositeName = attrNameSpace + ":" + attrName;
			int indexOfAttribute = mAttrList.IndexOfKey(tmpCompositeName);
			if(indexOfAttribute!=-1)
				rVal = (string)mAttrList.GetByIndex(indexOfAttribute);
				
			return rVal;
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
				if(newValue == "")	//setting an empty value now removes the attribute
					mAttrList.RemoveAt(indexOfPreviousAttrValue);
				else
                    mAttrList.Add(strLocalAttrName,newValue);
			}
			else
			{
				if(newValue!="")
					mAttrList.SetByIndex(indexOfPreviousAttrValue,newValue);
				//no "else", since adding an empty string would be the same as removing the attribute again.
			}
		}

*/
		public bool TestQName(string newQName)
		{
			//TODO: find out exactly what we want to allow for QNames
			//TODO: test that the supplied string folows those rules

			//TODO: test that this RegEx actually matches as expected!
			string strQNamePattern = @"\A[_a-zA-Z]+[_a-zA-Z0-9]*\Z";
			//my understanding of this RegEx: [bind to start of string][allow 1 or more underscore+letters][allow any ammount of underscore+alphanumerics][bind to end of string]

			//marisa added .Count==0, it might work (didn't compile before)
			bool rVal = true;
			if(System.Text.RegularExpressions.Regex.Matches
				(newQName,strQNamePattern).Count == 0)
				rVal = false;

			//Laust did 2 more version of same, last one being most likely to give the correct result.

			//any match at all, should be sufficient given the right RegEx
			rVal = System.Text.RegularExpressions.Regex.Match(newQName,strQNamePattern).Success;

			//the match is same length as original -> they are the same
			rVal = (System.Text.RegularExpressions.Regex.Match(newQName,strQNamePattern).Length == newQName.Length);

			return rVal;

		}

		
		public PropertyType getPropertyType()
		{
			//TODO: something more clever; marisa did this to make it compile
			//DONE: Now returning the intented value
			return PropertyType.StructureProperty;
		}

		public ICoreNode getOwner()
		{
			
		}

    public void setQName(string name, string ns)
    {
    }

		#region IXUKable members 

		public bool XUKin(System.Xml.XmlReader source)
		{
			//TODO: actual implementation, for now we return false as default, signifying that all was not done
			return false;
		}

		public bool XUKout(System.Xml.XmlWriter destination)
		{
			//TODO: actual implementation, for now we return false as default, signifying that all was not done
			return false;
		}
		#endregion

		#region IXmlPropertyValidator Members

		public bool canSetAttribute(string newNamespace, string newName, string newValue)
		{
			// TODO:  Add XmlProperty.canSetAttribute implementation
			return false;
		}

		public bool canRemoveAttribute(string removableNamespace, string removableName)
		{
			// TODO:  Add XmlProperty.canRemoveAttribute implementation
			return false;
		}

		public bool canSetQName(string newNamespace, string newName)
		{
			// TODO:  Add XmlProperty.canSetQName implementation
			return false;
		}

		public bool canSetName(string newName)
		{
			// TODO:  Add XmlProperty.canSetName implementation
			return false;
		}

		#endregion
	}

	

	
}
