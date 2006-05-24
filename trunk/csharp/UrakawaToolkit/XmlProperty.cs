using System;
using urakawa.exception;

namespace urakawa.core
{
	/// <summary>
	/// 
	/// </summary>
	public class XmlProperty : IXmlProperty, IXmlPropertyValidator	
	{
		public static bool testChildNames(string parentname, string[] childnames)
		{
			bool rVal = true;
			string tempXml = "<" + parentname +">";
			for(int i=0;i<childnames.Length;i++)
			{
				tempXml += "<" + childnames[i] + " />";
			}
			tempXml += "</" + parentname + ">";
			rVal = testFragment(parentname, tempXml, System.Xml.XmlNodeType.Element);

			return rVal;
		}

		public static bool testAttributes(string nodename,string[] namespaces, string[] names, string[] values)
		{
			return true;
		}

		public static bool testFragment(string nameOfContext, string fragment, System.Xml.XmlNodeType typeToTest)
		{
			bool rVal = true;
			string strQuitePossiblyTheNeededDTD = "";
			strQuitePossiblyTheNeededDTD = System.IO.File.OpenText("xuk.dtd").ReadToEnd();
			//TODO: figure out where to store the path to the DTD, possibly also the loaded contents of the DTD
			//TODO: something that means that I don't have to discard the <?xml... ?> in this ugly manner
			if(strQuitePossiblyTheNeededDTD.IndexOf("?>",0)>-1)
				strQuitePossiblyTheNeededDTD = strQuitePossiblyTheNeededDTD.Substring(strQuitePossiblyTheNeededDTD.IndexOf("?>",0)+2);

			System.Xml.XmlParserContext tmpContext 
				= new System.Xml.XmlParserContext(
				null,
				new System.Xml.XmlNamespaceManager(new System.Xml.NameTable()),
				nameOfContext,
				/*pubId*/null,
				/*sysId*/null,
				strQuitePossiblyTheNeededDTD,
				".",
				"",
				System.Xml.XmlSpace.Default,
				System.Text.Encoding.UTF8
				);

			try
			{
				System.Xml.XmlValidatingReader testReader = new System.Xml.XmlValidatingReader(
					fragment,
					System.Xml.XmlNodeType.Element,
					tmpContext
					);
				testReader.ValidationType = System.Xml.ValidationType.DTD;

				while(!testReader.EOF)
				{
					try
					{
						testReader.Read();
					}
					catch(System.Xml.Schema.XmlSchemaException exSchema)
					{
						if(testReader.Depth <= 1 && testReader.NodeType == typeToTest)
						{
							rVal = false;
							break;
						}
					}

				}
			}
			catch(Exception eAnything)
			{
				rVal = false;
			}
			return rVal;
		}


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
			XmlProperty tmpProp = (XmlProperty)this.getOwner().getPresentation().getPropertyFactory().createProperty(this.getPropertyType());
			tmpProp.mName = this.mName;
			tmpProp.mNamespace = this.mNamespace;
			tmpProp.mOwner = this.mOwner;
			for(int i=0;i<this.mAttributes.Count;i++)
                tmpProp.mAttributes += this.mAttributes[i].copy();
			return tmpProp;
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
			return mNamespace;
		}
		public void setNamespace(string newNamespace)
		{
			mNamespace = newNamespace;
			
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
			return PropertyType.XML;
		}

		public ICoreNode getOwner()
		{
            return mOwner;			
		}

    public void setQName(string name, string ns)
    {
    }

	#region IXUKable members 

	public bool XUKin(System.Xml.XmlReader source)
	{
		if (source == null)
		{
			throw new exception.MethodParameterIsNullException("Xml Reader is null");
		}

		if (!(source.Name == "XmlProperty" &&
			source.NodeType == System.Xml.XmlNodeType.Element))
		{
			return false;
		}

		System.Diagnostics.Debug.WriteLine("XUKin: XmlProperty");


		string name = source.GetAttribute("name");
		bool bFoundName = true;
		if (name == "")
			bFoundName = false;
		else
			mName = name;

		//collect all XmlAttribute elements
		bool bProcessedXmlAttributes = true;

		while (!(source.Name == "XmlProperty" && 
			source.NodeType == System.Xml.XmlNodeType.EndElement) &&
			source.EOF == false)
		{
			source.Read();

			if (source.Name == "XmlAttribute" &&
				source.NodeType == System.Xml.XmlNodeType.Element)
			{
				
				string attr_name = source.GetAttribute("name");
				string attr_ns = source.GetAttribute("namespace");
				string attr_val = "";

				if (attr_ns == null)
				{
					attr_ns = "";
				}

				if (source.IsEmptyElement == false)
				{
					source.Read();
					if (source.NodeType == System.Xml.XmlNodeType.Text)
					{
						attr_val = source.Value;
					}
				}

				
				if (attr_name == "")
				{
					bProcessedXmlAttributes = false && bProcessedXmlAttributes;
				}
				else
				{
					IXmlAttribute attr = new XmlAttribute(this, attr_ns, attr_name, attr_val);
					this.mAttributes.add(attr);
				}

			}
		}


		//"name" is a required attribute, so make sure it was found
		return bFoundName && bProcessedXmlAttributes;
	}

	public bool XUKout(System.Xml.XmlWriter destination)
	{
		if (destination == null)
		{
			throw new exception.MethodParameterIsNullException("Xml Writer is null");
		}

		//name is required
		if (this.mName == "")
			return false;

		destination.WriteStartElement("XmlProperty");

		destination.WriteAttributeString("name", mName);

		bool bWroteAttrs = true;

		for (int i = 0; i<this.mAttributes.Count; i++)
		{
			IXmlAttribute attr = (IXmlAttribute)mAttributes[i];
			bool bTmp = attr.XUKout(destination);

			bWroteAttrs = bTmp && bWroteAttrs;
		}

		destination.WriteEndElement();

		return bWroteAttrs;
	}
	#endregion

		#region IXmlPropertyValidator Members

		public bool canSetAttribute(string newNamespace, string newName, string newValue)
		{
			string[] namespaces;
			string[] names;
			string[] values;
			int newAttribCount;
			bool matchingAttribExists = mAttributes.Contains(newNamespace + ":" + newName);

			if(matchingAttribExists) newAttribCount = mAttributes.Count;
			else newAttribCount = mAttributes.Count+1;

			names = new string[newAttribCount];
			namespaces = new string[newAttribCount];
			values = new string[newAttribCount];

			for(int i=0;i<mAttributes.Count;i++)
			{
				if(names[i] == newName && namespaces[i] == newNamespace)
				{
					values[i] = newValue;
				}
				else
				{
					names[i] = mAttributes[i].getName();
					namespaces[i] = mAttributes[i].getNamespace();
					values[i] = mAttributes[i].getValue();
				}
			}

			if(!matchingAttribExists)
			{
				names[names.Length-1] = newName;
				namespaces[namespaces.Length-1] = newNamespace;
				values[values.Length-1] = newValue;

			}

			string currentQName = "";
			if(this.mNamespace != "")
				currentQName = this.mNamespace + ":";
			currentQName += this.mName;
			return XmlProperty.testAttributes(currentQName,namespaces,namespaces,values);
		}

		public bool canRemoveAttribute(string removableNamespace, string removableName)
		{
			bool matchingAttribExists = mAttributes.Contains(removableNamespace + ":" + removableName);
			if(!matchingAttribExists)
				return false;

			string[] namespaces;
			string[] names;
			string[] values;

			names = new string[mAttributes.Count-1];
			namespaces = new string[mAttributes.Count-1];
			values = new string[mAttributes.Count-1];


			bool passedMatchingAttrib = false;
			for(int i=0;i+(passedMatchingAttrib?0:1)<mAttributes.Count;i++)
			{
				if(names[i] == removableName && namespaces[i] == removableNamespace)
				{
					passedMatchingAttrib = true;
				}
				else
				{
					names[i] = mAttributes[i+(passedMatchingAttrib?1:0)].getName();
					namespaces[i] = mAttributes[i+(passedMatchingAttrib?1:0)].getNamespace();
					values[i] = mAttributes[i+(passedMatchingAttrib?1:0)].getValue();
				}
			}

			string currentQName = "";
			if(this.mNamespace != "")
				currentQName = this.mNamespace + ":";
			currentQName += this.mName;
			return XmlProperty.testAttributes(currentQName,namespaces,namespaces,values);
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
