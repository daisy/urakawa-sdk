using System;
using System.Xml;

namespace urakawa.core
{
	/// <summary>
	/// Implementation of <see cref="CoreNode"/> interface
	/// </summary>
	public class CoreNode : ICoreNode
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
    /// The <see cref="IDOMNode"/> implementing the <see cref="IDOMNode"/>
    /// part of the <see cref="ICoreNode"/> interface
    /// </summary>
    private IDOMNode mDOMNode;

    /// <summary>
    /// Constructor setting the parent <see cref="ICoreNode"/> and the owner <see cref="Presentation"/>
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="presentation"></param>
    internal CoreNode(ICoreNode parent, Presentation presentation)
    {
      mPresentation = presentation;
      mDOMNode = new DOMNode(this, parent);
      mProperties = new property.IProperty[PROPERTY_TYPE_ARRAY.Length];
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
      int index = IndexOfPropertyType(prop.getType());
      bool retVal = (mProperties[index]!=null);
      mProperties[index] = prop;
      return retVal;
    }

    #endregion

    #region IDOMNode Members

    public void insertBefore(ICoreNode newNode, ICoreNode anchorNode)
    {
      mDOMNode.insertBefore(newNode, anchorNode);
    }

    public void insertAfter(ICoreNode newNode, ICoreNode anchorNode)
    {
      mDOMNode.insertAfter(newNode, anchorNode);
    }

    public ICoreNode removeChild(ICoreNode node)
    {
      return mDOMNode.removeChild(node);
    }

    public ICoreNode replaceChild(ICoreNode node, ICoreNode oldNode)
    {
      return replaceChild(node, oldNode);
    }

    #endregion

    #region ILimitedDOMNode Members

    public ICoreNode getParent()
    {
      return mDOMNode.getParent();
    }

    public void appendChild(ICoreNode newChild)
    {
      mDOMNode.appendChild(newChild);
    }

    public void insertChild(ICoreNode newNode, int index)
    {
      mDOMNode.insertChild(newNode, index);
    }

    public ICoreNode getChild(int index)
    {
      return mDOMNode.getChild(index);;
    }

    ICoreNode ILimitedDOMNode.removeChild(int index)
    {
      return mDOMNode.removeChild(index);
    }

    ICoreNode ILimitedDOMNode.replaceChild(ICoreNode node, int index)
    {
      return mDOMNode.replaceChild(node, index);
    }

    public int getChildCount()
    {
      return mDOMNode.getChildCount();
    }

    public int indexOfChild(ICoreNode node)
    {
      return mDOMNode.indexOfChild(node);
    }

    public ICoreNode detach()
    {
      return mDOMNode.detach();
    }

    #endregion

    #region IVisitableNode Members

    public void AcceptDepthFirst(ICoreTreeVisitor visitor)
    {
      // TODO:  Add CoreNode.AcceptDepthFirst implementation
    }

    public void AcceptBreadthFirst(ICoreTreeVisitor visitor)
    {
      // TODO:  Add CoreNode.AcceptBreadthFirst implementation
    }

    #endregion
  }
}
