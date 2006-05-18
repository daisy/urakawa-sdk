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
			//TODO: actual implementation, for now we return false as default, signifying that all was not done
			return false;
		}

		public bool XUKout(System.Xml.XmlWriter destination)
		{
			//TODO: actual implementation, for now we return false as default, signifying that all was not done
			return false;
		}
		#endregion


  }
}
