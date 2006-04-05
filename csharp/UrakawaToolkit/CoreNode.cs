using System;
using System.Xml;

namespace urakawa.core
{
	/// <summary>
	/// Implementation of <see cref="CoreNode"/> interface
	/// </summary>
	public class CoreNode : DOMNode, ICoreNode
  {
    /// <summary>
    /// An array holding the possible <see cref="property.PropertyType"/> 
    /// of <see cref="property.IProperty"/>s associated with the class.
    /// This array defines the indexing within the private member array <see cref="mProperties"/>
    /// of <see cref="property.IProperty"/>s of a given <see cref="property.PropertyType"/>
    /// </summary>
    private static property.PropertyType[] PROPERTY_TYPE_ARRAY 
      = (property.PropertyType[])Enum.GetValues(typeof(property.PropertyType));

    /// <summary>
    /// Gets the index of a given <see cref="property.PropertyType"/> 
    /// within <see cref="PROPERTY_TYPE_ARRAY"/>/<see cref="mProperties"/>
    /// </summary>
    /// <param name="type">The <see cref="property.PropertyType"/> for which to find the index</param>
    /// <returns>The index</returns>
    private static int IndexOfPropertyType(property.PropertyType type)
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
    private Presentation mPresentation;


    /// <summary>
    /// An array holding the <see cref="property.IProperty"/>
    /// associated with the instance
    /// </summary>
    private property.IProperty[] mProperties;

    /// <summary>
    /// Constructor setting the owner <see cref="Presentation"/>
    /// </summary>
    /// <param name="presentation"></param>
    internal CoreNode(Presentation presentation)
    {
      mPresentation = presentation;
      mProperties = new property.IProperty[PROPERTY_TYPE_ARRAY.Length];
      mOwner = this;//Make the instance the 'owner' of it's own base class
    }

    #region ICoreNode Members

    /// <summary>
    /// Gets the <see cref="Presentation"/> that owns the core node
    /// </summary>
    /// <returns>The owner</returns>
    public Presentation getPresentation()
    {
      return mPresentation;
    }

    /// <summary>
    /// Gets the <see cref="property.IProperty"/> of the given <see cref="property.PropertyType"/>
    /// </summary>
    /// <param name="type">The given <see cref="property.PropertyType"/></param>
    /// <returns>The <see cref="property.IProperty"/> of the given <see cref="property.PropertyType"/>,
    /// <c>null</c> if no property of the given <see cref="property.PropertyType"/> has been set</returns>
    public property.IProperty getProperty(property.PropertyType type)
    {
      return mProperties[IndexOfPropertyType(type)];
    }

    /// <summary>
    /// Sets a <see cref="property.IProperty"/>, possible overwriting previously set <see cref="property.IProperty"/>
    /// of the same <see cref="property.PropertyType"/>
    /// </summary>
    /// <param name="prop">The <see cref="property.IProperty"/> to set. 
    /// If <c>null</c> is passed, an <see cref="exception.MethodParameterIsNullException"/> is thrown</param>
    /// <returns>A <see cref="bool"/> indicating if a previously set <see cref="property.IProperty"/>
    /// was overwritten
    /// </returns>
    public bool setProperty(property.IProperty prop)
    {
      if (prop==null) throw new exception.MethodParameterIsNullException("No PropertyType was given");
      int index = IndexOfPropertyType(prop.getPropertyType());
      bool retVal = (mProperties[index]!=null);
      mProperties[index] = prop;
      return retVal;
    }

    #endregion

    #region IVisitableNode Members

    /// <summary>
    /// Accept a <see cref="ICoreTreeVisitor"/> in depth first mode
    /// </summary>
    /// <param name="visitor">The <see cref="ICoreTreeVisitor"/></param>
    public void AcceptDepthFirst(ICoreTreeVisitor visitor)
    {
      if (visitor.preVisit(this))
      {
        for (int i=0; i<getChildCount(); i++)
        {
          getChild(i).AcceptDepthFirst(visitor);
        }
      }
      visitor.postVisit(this);
    }

    #endregion
  }
}
