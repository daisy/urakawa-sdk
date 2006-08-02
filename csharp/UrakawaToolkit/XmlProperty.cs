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
		/* LNN: This code might no longer be needed
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
		*/

		private static bool testAttributes(urakawa.core.Presentation owningPresentation, string nodename, string[] names, string[] namespaces, string[] values)
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
      return testFragment(owningPresentation,nodename,tmpXml,System.Xml.XmlNodeType.Attribute);
    }

		/// <summary>
		/// Accepts an XML fragment as string. Tests that the children of the root node is permittet according to the DTD in the Presentation
		/// </summary>
		/// <param name="ownerOfNode">The Presentation to test against</param>
		/// <param name="nameOfContext">name of the root of the fragment</param>
		/// <param name="fragment">The XML fragment to test</param>
		/// <param name="typeToTest">Should be either Element or Attribute. Any errors arising from other types of nodes than those of the given type will be ignored.</param>
		/// <returns>true if fragment is OK or no DTD is present to test against, otherwise false.</returns>
    public static bool testFragment(urakawa.core.Presentation ownerOfNode, string nameOfContext, string fragment, System.Xml.XmlNodeType typeToTest)
    {
      bool rVal = true;

			if(ownerOfNode == null)
				return true;
	
			ownerOfNode.mDtdRules.DocTypeName = nameOfContext;

      try
      {
        System.Xml.XmlValidatingReader testReader = new System.Xml.XmlValidatingReader(
          fragment,
          System.Xml.XmlNodeType.Element,
          ownerOfNode.mDtdRules
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
    /// Sets the QName of the <see cref="XmlProperty"/>
    /// </summary>
    /// <param name="name"></param>
    /// <param name="ns"></param>
    public void setQName(string name, string ns)
    {
      mName = name;
      mNamespace = ns;
    }

    /// <summary>
    /// Gets the QName of the <see cref="XmlProperty"/>
    /// </summary>
    /// <returns>The QName in the form [ns:]localname</returns>
		public string getQName()
		{
			return ((mNamespace=="")?(mNamespace + ":") : ("")) + mName;
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

    #region IProperty members
    
    IProperty IProperty.copy()
    {
      return this.copy();
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
    #endregion

    #region IXUKAble members 

    /// <summary>
    /// Reads the <see cref="XmlProperty"/> from an XmlProperty element in a XUK file
    /// </summary>
    /// <param name="source">The source <see cref="XmlReader"/></param>
    /// <returns>A <see cref="bool"/> indicating if the read was succesful</returns>
    public bool XUKIn(System.Xml.XmlReader source)
    {
      if (source == null)
      {
        throw new exception.MethodParameterIsNullException("Xml Reader is null");
      }
			if (source.Name != "XmlProperty") return false;
			if (source.NamespaceURI != urakawa.ToolkitSettings.XUK_NS) return false;
			if (source.NodeType != System.Xml.XmlNodeType.Element) return false;

      string name = source.GetAttribute("name");
      if (name==null || name=="") return false;

      string ns = source.GetAttribute("namespace");
      if (ns==null) ns = "";
      setQName(name, ns);

      if (source.IsEmptyElement) return true;

      while (source.Read())
      {
        if (source.NodeType==XmlNodeType.Element)
        {
					if (source.NamespaceURI != urakawa.ToolkitSettings.XUK_NS) return false;
          switch (source.LocalName)
          {
            case "XmlAttribute":
              XmlAttribute newAttr = new XmlAttribute(this, "dummy", "", "");
              if (!newAttr.XUKIn(source)) return false;
              setAttribute(newAttr);
              break;
            default:
							return false;
          }
        }
        else if (source.NodeType==XmlNodeType.EndElement)
        {
          break;
        }
        if (source.EOF) break;
      }
      return true;
    }

    /// <summary>
    /// Writes a XmlProperty element to a XUK file representing the <see cref="XmlProperty"/>
    /// </summary>
    /// <param name="destination">The destination <see cref="XmlWriter"/></param>
    /// <returns>A <see cref="bool"/> indicating if the write was succesful</returns>
    public bool XUKOut(System.Xml.XmlWriter destination)
    {
      if (destination == null)
      {
        throw new exception.MethodParameterIsNullException("Xml Writer is null");
      }

      //name is required
      if (this.mName == "") return false;

      destination.WriteStartElement("XmlProperty", urakawa.ToolkitSettings.XUK_NS);

      destination.WriteAttributeString("name", mName);

			destination.WriteAttributeString("namespace", mNamespace);

			foreach (IXmlAttribute attr in mAttributes)
			{
				if (!attr.XUKOut(destination)) return false;
			}

			destination.WriteEndElement();

      return true;
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
			if(getOwner() == null)
				return true;
			if(getOwner().getPresentation()==null)
				return true;
			if(!getOwner().getPresentation().GetType().IsInstanceOfType(typeof(Presentation)))
				return true;

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
      return XmlProperty.testAttributes((Presentation)getOwner().getPresentation(),currentQName,names,namespaces,values);
    }

    /// <summary>
    /// Tests if a given <see cref="IXmlAttribute"/> can be removed 
    /// </summary>
    /// <param name="removableName">The local name of the given <see cref="IXmlAttribute"/></param>
    /// <param name="removableNamespace">The namepsace of the given <see cref="IXmlAttribute"/></param>
    /// <returns>A <see cref="bool"/> indicating the result of the test</returns>
    public bool canRemoveAttribute(string removableName, string removableNamespace)
    {
			if(getOwner() == null)
				return true;
			if(getOwner().getPresentation()==null)
				return true;
			if(!getOwner().getPresentation().GetType().IsInstanceOfType(typeof(Presentation)))
				return true;

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

			urakawa.core.Presentation owningPresentation = (Presentation) this.getOwner().getPresentation();

      return XmlProperty.testAttributes(owningPresentation,currentQName,names,namespaces,values);
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

			if(!owner.getPresentation().GetType().IsInstanceOfType(typeof(Presentation)))
				return true;
			Presentation containingPresentation = (Presentation)owner.getPresentation();
			if(containingPresentation == null)
				return true;


		//3 first tests indicate that a node outside a tree can always have it's name set.
			string newQName = (newNamespace==""?newNamespace+":":"")+newName;

			while(parentOfOwner.getProperty(typeof(XmlProperty)) == null && parentOfOwner.getParent() != null)
			{
				parentOfOwner = (ICoreNode)parentOfOwner.getParent();
			}
			if(parentOfOwner.getProperty(typeof(XmlProperty)) == null && parentOfOwner.getParent() == null)
			{
				//root reached without finding any XmlProperty on an owning node, so it seems we are renaming the XML root element, meaning that we don't test if it OK to have this new name inside it's parent.
			}
			else
			{
				CanSetQNameFragmentCollector isNameValidInsideParent = new CanSetQNameFragmentCollector();
				isNameValidInsideParent.newQName = newQName;
				isNameValidInsideParent.root = parentOfOwner;
				isNameValidInsideParent.toBeRenamed = owner;
				parentOfOwner.acceptDepthFirst(isNameValidInsideParent);
				if(!XmlProperty.testFragment(containingPresentation,newQName,isNameValidInsideParent.Fragment,XmlNodeType.Element))
					return false;
			}

			CanSetQNameFragmentCollector areChildrenValid = new CanSetQNameFragmentCollector();
			areChildrenValid.newQName = newQName;
			areChildrenValid.root = owner;
			areChildrenValid.toBeRenamed = owner;
			parentOfOwner.acceptDepthFirst(areChildrenValid);
			if(!XmlProperty.testFragment(containingPresentation,newQName,areChildrenValid.Fragment,XmlNodeType.Element))
				return false;

      return true;
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
			public string newQName="";

      #region ICoreNodeVisitor Members

      public bool preVisit(ICoreNode node)
      {
        bool handlingTheRoot = (root == node);
				bool thisNodeHasNoXmlAttribute = false;
				IXmlProperty tmpProp = (IXmlProperty)node.getProperty(typeof(XmlProperty));

				if(handlingTheRoot)
				{
					if(tmpProp!=null)
					{
						Fragment += "<";
						if(node != toBeRenamed)
						{
							if(tmpProp.getNamespace()!="")
							{
								Fragment += tmpProp.getNamespace() + ":";
							}
							Fragment += tmpProp.getName();
						}
						else
						{
							Fragment += newQName;
						}
						Fragment += " >";
					}
					else
					{
						throw(new urakawa.exception.NodeDoesNotExistException("Cannot validate name change on CoreNodes that do not have an XmlPorperty!"));
					}
				}
				else
				{
					//visiting a childnode
					if(tmpProp!=null)
					{
						Fragment += "<";
						if(node != toBeRenamed)
						{
							if(tmpProp.getNamespace()!="")
							{
								Fragment += tmpProp.getNamespace() + ":";
							}
							Fragment += tmpProp.getName();
						}
						else
						{
							Fragment += newQName;
						}
						Fragment += " />";
					}
					else
					{
						thisNodeHasNoXmlAttribute = true;
					}
				}

        //only the root and the first level of children with XmlAttribute should be visited
        return handlingTheRoot||thisNodeHasNoXmlAttribute;
      }

      public void postVisit(ICoreNode node)
      {
				bool handlingTheRoot = (root == node);
				IXmlProperty tmpProp = (IXmlProperty)node.getProperty(typeof(XmlProperty));

				if(handlingTheRoot)
				{
					if(tmpProp!=null)
					{
						Fragment += "</";
						if(node==toBeRenamed)
						{
							Fragment += newQName;
						}
						else
						{
							if(tmpProp.getNamespace()!="")
							{
								Fragment += tmpProp.getNamespace() + ":";
							}
							Fragment += tmpProp.getName();
						}
						Fragment += " >";
					}
				}
      }

      #endregion

    }
    #endregion
  }

	

	
}
