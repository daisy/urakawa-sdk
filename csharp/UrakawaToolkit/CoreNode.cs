using System;
using System.Collections;
using System.Xml;

namespace urakawa.core
{
	/// <summary>
	/// Implementation of <see cref="CoreNode"/> interface
	/// </summary>
	public class CoreNode : TreeNode, ICoreNode
  {
    /// <summary>
    /// An array holding the possible <see cref="PropertyType"/> 
    /// of <see cref="IProperty"/>s associated with the class.
    /// This array defines the indexing within the private member array <see cref="mProperties"/>
    /// of <see cref="IProperty"/>s of a given <see cref="PropertyType"/>
    /// </summary>
    private static PropertyType[] PROPERTY_TYPE_ARRAY 
      = (PropertyType[])Enum.GetValues(typeof(PropertyType));
    
    /// <summary>
    /// Gets the index of a given <see cref="PropertyType"/> 
    /// within <see cref="PROPERTY_TYPE_ARRAY"/>/<see cref="mProperties"/>
    /// </summary>
    /// <param name="type">The <see cref="PropertyType"/> for which to find the index</param>
    /// <returns>The index</returns>
    private static int IndexOfPropertyType(PropertyType type)
    {
      for (int i=0; i<PROPERTY_TYPE_ARRAY.Length; i++)
      {
        if (PROPERTY_TYPE_ARRAY[i]==type) return i;
      }
      return -1;
    }

    /// <summary>
    /// The owner <see cref="Presentation"/>
    /// </summary>
    private IPresentation mPresentation;


    /// <summary>
    /// Contains the <see cref="IProperty"/>s of the node
    /// </summary>
    private IProperty[] mProperties;

    /// <summary>
    /// Constructor setting the owner <see cref="Presentation"/>
    /// </summary>
    /// <param name="presentation"></param>
    internal CoreNode(IPresentation presentation)
    {
      mPresentation = presentation;
      mProperties = new IProperty[PROPERTY_TYPE_ARRAY.Length];
    }

    #region ICoreNode Members

    /// <summary>
    /// Gets the <see cref="Presentation"/> owning the <see cref="ICoreNode"/>
    /// </summary>
    /// <returns>The owner</returns>
    public IPresentation getPresentation()
    {
      return mPresentation;
    }

    /// <summary>
    /// Gets the <see cref="IProperty"/> of the given <see cref="PropertyType"/>
    /// </summary>
    /// <param name="type">The given <see cref="PropertyType"/></param>
    /// <returns>The <see cref="IProperty"/> of the given <see cref="PropertyType"/>,
    /// <c>null</c> if no property of the given <see cref="PropertyType"/> has been set</returns>
    public IProperty getProperty(PropertyType type)
    {
      return mProperties[IndexOfPropertyType(type)];
    }

    /// <summary>
    /// Sets a <see cref="IProperty"/>, possible overwriting previously set <see cref="IProperty"/>
    /// of the same <see cref="PropertyType"/>
    /// </summary>
    /// <param name="prop">The <see cref="IProperty"/> to set. 
    /// If <c>null</c> is passed, an <see cref="exception.MethodParameterIsNullException"/> is thrown</param>
    /// <returns>A <see cref="bool"/> indicating if a previously set <see cref="IProperty"/>
    /// was overwritten
    /// </returns>
    public bool setProperty(IProperty prop)
    {
      if (prop==null) throw new exception.MethodParameterIsNullException("No PropertyType was given");
      int index = IndexOfPropertyType(prop.getPropertyType());
      bool retVal = (mProperties[index]!=null);
      mProperties[index] = prop;
      return retVal;
    }

	/// <summary>
	/// Remove a property from the node's properties array
	/// Leave the slot available in the properties array (its size is fixed), but 
	/// make sure the contents are gone
	/// </summary>
	/// <param name="type">Specify the type of property to remove</param>
	/// <returns>The property which was just removed, or null if it did not exist</returns>
	public IProperty removeProperty(PropertyType type)
	{
		IProperty removedProperty = null;

		for (int i = 0; i<mProperties.Length; i++)
		{
			if (mProperties[i] != null && 
				mProperties[i].getPropertyType() == type)
			{
				removedProperty = mProperties[i];
				//we need to leave the slot in the array, just leave it empty
				mProperties[i] = null;
			}
		}

		if (removedProperty != null)
		{
			//a property which was just removed no longer has an owner
			//so set it to null
			removedProperty.setOwner(null);
		}
		return removedProperty;
	}

	/// <summary>
	/// Make a copy of the node.  The copy has the same presentation and no parent.
	/// </summary>
	/// <param name="deep">If true, then include the node's entire subtree.  
	/// Otherwise, just copy the node itself.</param>
	/// <returns>A <see cref="CoreNode"/> containing the copied data.</returns>
	public CoreNode copy(bool deep)
	{
		CoreNode theCopy = (CoreNode)this.getPresentation().getCoreNodeFactory().createNode();
	
		//copy the properties
		for (int i=0; i<this.mProperties.Length; i++)
		{
			if (this.mProperties[i] != null)
			{
				theCopy.setProperty(this.mProperties[i].copy());
			}
		}
		
		//copy the children
		if (deep == true)
		{
			for (int i=0; i<this.getChildCount(); i++)
			{
				//@todo does getType work this way?
				if (this.getChild(i).GetType() == typeof(urakawa.core.CoreNode))
				{
					theCopy.appendChild(((CoreNode)this.getChild(i)).copy(true));
				}
				else
				{
					//@todo what would be the graceful thing to do if it's not a core node?
				}
			}
		}

		return theCopy;
	}

    #endregion

    #region IVisitableCoreNode Members

    /// <summary>
    /// Accept a <see cref="ICoreNodeVisitor"/> in depth first mode
    /// </summary>
    /// <param name="visitor">The <see cref="ICoreNodeVisitor"/></param>
    public void acceptDepthFirst(ICoreNodeVisitor visitor)
    {
      if (visitor.preVisit(this))
      {
        for (int i=0; i<getChildCount(); i++)
        {
          ((ICoreNode)getChild(i)).acceptDepthFirst(visitor);
        }
      }
      visitor.postVisit(this);
    }

    /// <summary>
    /// Accept a <see cref="ICoreNodeVisitor"/> in breadth first mode
    /// </summary>
    /// <param name="visitor">The <see cref="ICoreNodeVisitor"/></param>
    /// <remarks>HACK: Not yet implemented, does nothing!!!!</remarks>
    public void acceptBreadthFirst(ICoreNodeVisitor visitor)
    {
      // TODO:  Add CoreNode.AcceptBreadthFirst implementation
    }

    #endregion

	#region IXUKable members 

	public bool XUKin(System.Xml.XmlReader source)
	{
		if (source == null)
		{
			throw new exception.MethodParameterIsNullException("Xml Reader is null");
		}

		//if we are not at the opening tag of a core node element, return false
		if (!(source.Name == "CoreNode" && source.NodeType == System.Xml.XmlNodeType.Element))
		{
			return false;
		}

		System.Diagnostics.Debug.WriteLine("XUKin: CoreNode");

		bool bPropertiesParsed = false;
		bool bNewNodesParsed = false;
		bool bFoundNodes = false;


		bool bSkipOneRead = false;

		//this part is TRICKY because it deals with nested elements
		//read until this CoreNode element ends, or until the document ends
		while (!(source.Name == "CoreNode" && 
			source.NodeType == System.Xml.XmlNodeType.EndElement) &&
			source.EOF == false)
		{
			//we might want to skip this
			if (bSkipOneRead == false)
			{
				source.Read();
			}
			else
			{
				bSkipOneRead = false;
			}
			
			//add the properties for this CoreNode
			if (source.Name == "mProperties" && source.NodeType == System.Xml.XmlNodeType.Element)
			{
				bPropertiesParsed = XUKin_Properties(source);
			}

			//if you encounter a CoreNode child, process it recursively
			else if (source.Name == "CoreNode" && source.NodeType == System.Xml.XmlNodeType.Element)
			{
				bool bTmpResult = false;					
				CoreNode newNode = new CoreNode(this.mPresentation);

				//process the XUK file on this new node
				bTmpResult = newNode.XUKin(source);
				
				if (bTmpResult == true)
				{
					this.appendChild(newNode);
				}

				//this is just error handling
				//accumulate the result of processing for all nodes found so far
				if (bFoundNodes == false)
				{					
					bNewNodesParsed = bTmpResult;
				}
				else
				{
					bNewNodesParsed = bNewNodesParsed && bTmpResult;
				}

				bFoundNodes = true;

				//VERY IMPORTANT PART
				//if we are at the end of a child CoreNode, read to the next element
				//and flag the system to skip one read the next time around the loop
				//this part is very important
				//if we don't call source.read() here, then the loop will exit because
				//it will see a </CoreNode>.  
				//and if we don't skip the next source.read(), it will never see
				//any new elements that are starting.
				if(source.Name == "CoreNode" && 
					source.NodeType == System.Xml.XmlNodeType.EndElement)
				{
					source.Read();
					bSkipOneRead = true;
				}
			}
		}
		

		//chlid nodes are not required, so if we didn't find any, just
		//return the results of the properties as the result of our processing
		if (bFoundNodes == false)
		{
			return bPropertiesParsed;
		}
		//if we did find child nodes, then judge our success by taking both
		//node and property processing into account
		else
		{
			return (bNewNodesParsed && bPropertiesParsed);
		}
	}

	public bool XUKout(System.Xml.XmlWriter destination)
	{
		if (destination == null)
		{
			throw new exception.MethodParameterIsNullException("Xml Writer is null");
		}

		bool bWroteProperties = true;
		bool bWroteChildNodes = true;

		destination.WriteStartElement("CoreNode");

		destination.WriteStartElement("mProperties");

		for (int i = 0; i<mProperties.Length; i++)
		{
			if (mProperties[i] != null)
			{
				bool bTmp = mProperties[i].XUKout(destination);
				bWroteProperties = bTmp && bWroteProperties;
			}
		}

		destination.WriteEndElement();

		
		for (int i = 0; i<this.getChildCount(); i++)
		{
			if (this.getChild(i).GetType() == typeof(CoreNode))
			{
				bool bTmp = ((CoreNode)this.getChild(i)).XUKout(destination);
				bWroteChildNodes = bTmp && bWroteChildNodes;
			}
			else
			{
				//@todo
				//will this case ever arise?
			}
		}

		destination.WriteEndElement();

		return (bWroteProperties && bWroteChildNodes);
	}
	#endregion

	/// <summary>
	/// helper function to read in the properties and invoke their respective XUKin methods
	/// </summary>
	/// <param name="source"></param>
	/// <returns></returns>
	private bool XUKin_Properties(System.Xml.XmlReader source)
	{
		if (!(source.Name == "mProperties" && 
			source.NodeType == System.Xml.XmlNodeType.Element))
		{
			return false;
		}

		System.Diagnostics.Debug.WriteLine("XUKin: CoreNode::Properties");

		bool bXmlPropertyProcessed = false;
		bool bXmlPropertyFound = false;
		bool bChannelsPropertyFound = false;
		bool bChannelsPropertyProcessed = false;

		while (!(source.Name == "mProperties" &&
			source.NodeType == System.Xml.XmlNodeType.EndElement)
			&&
			source.EOF == false)
		{
			source.Read();

			//set the xml property for this node
			if (source.Name == "XmlProperty" && 
				source.NodeType == System.Xml.XmlNodeType.Element)
			{
				bXmlPropertyFound = true;

				XmlProperty newXmlProp = 
					(XmlProperty)mPresentation.getPropertyFactory().createProperty
					(PropertyType.XML);

				bXmlPropertyProcessed = newXmlProp.XUKin(source);
				if (bXmlPropertyProcessed == true)
				{
					this.setProperty(newXmlProp);
				}
			}

			//set the channels property for this node
			else if (source.Name == "ChannelsProperty" &&
				source.NodeType == System.Xml.XmlNodeType.Element)
			{
				bChannelsPropertyFound = true;

				ChannelsProperty newChannelsProp = 
					(ChannelsProperty)mPresentation.getPropertyFactory().createProperty
					(PropertyType.CHANNEL);

				bChannelsPropertyProcessed = newChannelsProp.XUKin(source);

				if (bChannelsPropertyProcessed == true)
				{
					this.setProperty(newChannelsProp);
				}
			}
		}

		//now, decide what to return

		bool bXmlPropertyOk = false;
		bool bChannelsPropertyOk = false;

		//if we found an xml property, make sure it was processed ok
		if (bXmlPropertyFound == true)
		{
			bXmlPropertyOk = bXmlPropertyProcessed;
		}
		//if we didn't find one, that's ok too
		else
		{
			bXmlPropertyOk = true;
		}
		//if we found a channels property, make sure it was processed ok
		if (bChannelsPropertyFound == true)
		{
			bChannelsPropertyOk = bChannelsPropertyProcessed;
		}
		//if we didn't find one, that's ok too
		else
		{
			bChannelsPropertyOk = true;
		}

		return (bChannelsPropertyOk && bXmlPropertyOk);
	}
  }
}
