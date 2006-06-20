using System;
using System.Xml;
using urakawa.exception;

namespace urakawa.core
{
	/// <summary>
	/// Default implementation of <see cref="IXmlProperty"/> interface
	/// </summary>
  public class XmlProperty : IXmlProperty, IXmlPropertyValidator	
  {
    private static bool testChildNames(string parentname, string[] childnames)
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

    private static bool testAttributes(string nodename, string[] names, string[] namespaces, string[] values)
    {
      string tmpXml = "";
      tmpXml += "<" + nodename + " ";
      for(int i=0;i<names.Length;i++)
      {
        if(namespaces[i] != "")
          tmpXml+= namespaces[i] + ":";
        tmpXml += names[i] + "=\"";
        tmpXml += values[i] + "\" ";
      }
      tmpXml += "/>";
      return testFragment(nodename,tmpXml,System.Xml.XmlNodeType.Attribute);
    }

    private static bool testFragment(string nameOfContext, string fragment, System.Xml.XmlNodeType typeToTest)
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
          catch(System.Xml.Schema.XmlSchemaException)
          {
            if(testReader.Depth <= 1 && testReader.NodeType == typeToTest)
            {
              rVal = false;
              break;
            }
          }

        }
      }
      catch(Exception)
      {
        rVal = false;
      }
      return rVal;
    }


    private string mName;
    private string mNamespace;
    private XmlAttributeList mAttributes = new XmlAttributeList();
    private ICoreNode mOwner;

    internal XmlProperty(string newName, string newNamespace)
    {
      if(newNamespace!=null)
      {
        mNamespace = newNamespace;
      }
      else
      {
        throw new exception.MethodParameterIsNullException("XmlProperty cannot have null for namespace. Empty string is allowed");
      }

      if(newName!=null)
      {
        if(newName!="")
        {
          mName = newName;
        }
        else
        {
          throw new exception.MethodParameterIsEmptyStringException("XmlProperty needs a name.");
        }
      }
      else
      {
        throw new exception.MethodParameterIsNullException("XmlProperty cannot have null for name.");
      }

    }

    IProperty IProperty.copy()
    {
      return this.copy();
    }

    /// <summary>
    /// Crreates a copy of the <see cref="XmlProperty"/>
    /// </summary>
    /// <returns></returns>
    public XmlProperty copy()
    {
      XmlProperty tmpProp = (XmlProperty)this.getOwner().getPresentation().getPropertyFactory().createXmlProperty(
        getName(), getNamespace());
      foreach (IXmlAttribute a in getListOfAttributes())
      {
        tmpProp.setAttribute(a);
      }
      return tmpProp;
    }

    /// <summary>
    /// Gets the local name
    /// </summary>
    /// <returns>The local name</returns>
    public string getName()
    {
      return mName;
    }

    /// <summary>
    /// Sets the local name
    /// </summary>
    /// <param name="newName">The new local name</param>
    public void setName(string newName)
    {
      setQName(newName, getNamespace());
    }

    /// <summary>
    /// Gets the namespace
    /// </summary>
    /// <returns>The namespace</returns>
    public string getNamespace()
    {
      return mNamespace;
    }

    /// <summary>
    /// Sets the namespace
    /// </summary>
    /// <param name="newNamespace">The new namespace</param>
    public void setNamespace(string newNamespace)
    {
      setQName(getName(), newNamespace);
    }

    System.Collections.IList IXmlProperty.getListOfAttributes()
    {
      return mAttributes;
    }

    /// <summary>
    /// Gets a <see cref="XmlAttributeList"/> of the <see cref="IXmlAttribute"/>s of the <see cref="XmlProperty"/>
    /// </summary>
    /// <returns></returns>
    public XmlAttributeList getListOfAttributes()
    {
      return mAttributes;
    }

    /// <summary>
    /// Gets the <see cref="XMLType"/> of the <see cref="XmlProperty"/>
    /// </summary>
    /// <returns></returns>
    public XMLType getXMLType()
    {
      return XMLType.ELEMENT;
    }

    /// <summary>
    /// Sets an <see cref="IXmlAttribute"/>. 
    /// If the <see cref="XmlProperty"/> already has an <see cref="IXmlAttribute"/>
    /// with the same QName, this is overwritten
    /// </summary>
    /// <param name="newAttribute">The new <see cref="IXmlAttribute"/></param>
    public void setAttribute(IXmlAttribute newAttribute)
    {
      IXmlAttribute a = getAttribute(newAttribute.getName(), newAttribute.getNamespace());
      if (a!=null)
      {
        mAttributes.Remove(a);
      }
      mAttributes.Add(newAttribute);
    }

    /// <summary>
    /// Sets an <see cref="XmlAttribute"/>. 
    /// If the <see cref="XmlProperty"/> already has an <see cref="IXmlAttribute"/>
    /// with the same QName, this is overwritten
    /// </summary>
    /// <param name="name">The local name of the new <see cref="XmlAttribute"/></param>
    /// <param name="ns">The namespace of the new <see cref="XmlAttribute"/></param>
    /// <param name="val">The value of the new <see cref="XmlAttribute"/></param>
    public void setAttribute(string name, string ns, string val)
    {
      setAttribute(new XmlAttribute(this, name, ns, val));
    }

    /// <summary>
    /// Gets an <see cref="IXmlAttribute"/> by QName
    /// </summary>
    /// <param name="name">The local name part of the QName</param>
    /// <param name="ns">The namespace part of the QName</param>
    /// <returns></returns>
    public IXmlAttribute getAttribute(string name, string ns)
    {
      return mAttributes.getByQName(name, ns);
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
    private bool TestQName(string newQName)
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

		/// <summary>
		/// Gets the <see cref="PropertyType"/> of the <see cref="XmlProperty"/>
		/// </summary>
		/// <returns><see cref="PropertyType.XML"/></returns>
    public PropertyType getPropertyType()
    {
      //TODO: something more clever; marisa did this to make it compile
      //DONE: Now returning the intented value
      return PropertyType.XML;
    }

    /// <summary>
    /// Gets the owner <see cref="ICoreNode"/> of the <see cref="XmlProperty"/>
    /// </summary>
    /// <returns></returns>
    public ICoreNode getOwner()
    {
      return mOwner;			
    }

    /// <summary>
    /// Sets the owner <see cref="ICoreNode"/> of the <see cref="ChannelsProperty"/> instance
    /// </summary>
    /// <param name="newOwner">The new owner</param>
    /// <remarks>This function is intended for internal purposes only 
    /// and should not be called by users of the toolkit</remarks>
    public void setOwner(ICoreNode newOwner)
    {
      mOwner = newOwner;
    }

    /// <summary>
    /// Sets the QName of the <see cref="XmlProperty"/>
    /// </summary>
    /// <param name="name"></param>
    /// <param name="ns"></param>
    public void setQName(string name, string ns)
    {
      mName = name;
      mNamespace = ns;
    }

    #region IXUKable members 

    /// <summary>
    /// Reads the <see cref="XmlProperty"/> from an XmlProperty element in a XUK file
    /// </summary>
    /// <param name="source">The source <see cref="XmlReader"/></param>
    /// <returns>A <see cref="bool"/> indicating if the read was succesful</returns>
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
      if (name==null || name=="") return false;

      string ns = source.GetAttribute("namespace");
      if (ns==null) ns = "";
      setQName(name, ns);

      if (source.IsEmptyElement) return true;

      bool bFoundError = false;

      while (source.Read())
      {
        if (source.NodeType==XmlNodeType.Element)
        {
          switch (source.LocalName)
          {
            case "XmlAttribute":
              XmlAttribute newAttr = new XmlAttribute(this, "dummy", "", "");
              if (newAttr.XUKin(source))
              {
                this.setAttribute(newAttr);
              }
              else
              {
                bFoundError = true;
              }
              break;
            default:
              bFoundError = true;
              break;
          }
        }
        else if (source.NodeType==XmlNodeType.EndElement)
        {
          break;
        }
        if (source.EOF) break;
        if (bFoundError) break;
      }

      return !bFoundError;
    }

    /// <summary>
    /// Writes a XmlProperty element to a XUK file representing the <see cref="XmlProperty"/>
    /// </summary>
    /// <param name="destination">The destination <see cref="XmlWriter"/></param>
    /// <returns>A <see cref="bool"/> indicating if the write was succesful</returns>
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

    /// <summary>
    /// Tests if a given <see cref="IXmlAttribute"/> can be set
    /// </summary>
    /// <param name="newName">The local name of the <see cref="IXmlAttribute"/></param>
    /// <param name="newNamespace">The namespace of the <see cref="IXmlAttribute"/></param>
    /// <param name="newValue">The value of the <see cref="IXmlAttribute"/></param>
    /// <returns>A <see cref="bool"/> indicating the result of the test</returns>
    public bool canSetAttribute(string newName, string newNamespace, string newValue)
    {
      string[] namespaces;
      string[] names;
      string[] values;
      int newAttribCount;
      IXmlAttribute attr = mAttributes.getByQName(newName, newValue);

      if (attr!=null) newAttribCount = mAttributes.Count;
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

      if(attr==null)
      {
        names[names.Length-1] = newName;
        namespaces[namespaces.Length-1] = newNamespace;
        values[values.Length-1] = newValue;

      }

      string currentQName = "";
      if(this.mNamespace != "")
        currentQName = this.mNamespace + ":";
      currentQName += this.mName;
      return XmlProperty.testAttributes(currentQName,names,namespaces,values);
    }

    /// <summary>
    /// Tests if a given <see cref="IXmlAttribute"/> can be removed 
    /// </summary>
    /// <param name="removableName">The local name of the given <see cref="IXmlAttribute"/></param>
    /// <param name="removableNamespace">The namepsace of the given <see cref="IXmlAttribute"/></param>
    /// <returns>A <see cref="bool"/> indicating the result of the test</returns>
    public bool canRemoveAttribute(string removableName, string removableNamespace)
    {
      IXmlAttribute attr = mAttributes.getByQName(removableName, removableNamespace);
      if (attr==null) return false;

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
      return XmlProperty.testAttributes(currentQName,names,namespaces,values);
    }

    /// <summary>
    /// Tests if a given QName can be set
    /// </summary>
    /// <param name="newName">The local name part of the QName</param>
    /// <param name="newNamespace">The namespace part of the QName</param>
    /// <returns>A <see cref="bool"/> indicating the result of the test</returns>
    public bool canSetQName(string newNamespace, string newName)
    {
      ICoreNode owner = this.getOwner();
      if(owner == null)
        return true;

      ICoreNode parentOfOwner = (ICoreNode)owner.getParent();
      if(parentOfOwner == null)
        return true;

      return false;
    }


    /// <summary>
    /// Tests if a given local name can be set. 
    /// Convenience method, equivlent to 
    /// <c><see cref="canSetQName"/>(newName, <see cref="IXmlProperty.getNamespace"/>())</c>
    /// </summary>
    /// <param name="newName">The locan name</param>
    /// <returns>A <see cref="bool"/> indicating the result of the test</returns>
    public bool canSetName(string newName)
    {
      return canSetQName(newName, this.mNamespace);
    }


    private class CanSetQNameFragmentCollector: ICoreNodeVisitor
    {
      public ICoreNode root = null;
      public ICoreNode toBeRenamed = null;
      public string Fragment="";

      #region ICoreNodeVisitor Members

      public bool preVisit(ICoreNode node)
      {
        bool handlingTheRoot = (root == node);

        if(handlingTheRoot)
        {
          XmlProperty tmpProp = (XmlProperty)node.getProperty(PropertyType.XML);
        }

        //only the root and the first level of children should be visited
        return handlingTheRoot;
      }

      public void postVisit(ICoreNode node)
      {
        // TODO:  Add FragmentCollector.postVisit implementation
      }

      #endregion

    }
    #endregion
  }

	

	
}
