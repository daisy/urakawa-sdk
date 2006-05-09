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
    /// Contains the <see cref="IProperties"/> of the node
    /// </summary>
    private property.IProperty[] mProperties;

//    /// <summary>
//    /// Contains the children of the node
//    /// </summary>
//    /// <remarks>All items in <see cref="mChildren"/> MUST be <see cref="ICoreNode"/>s</remarks>
//    private IList mChildren = new ArrayList();
//
//    /// <summary>
//    /// The parent <see cref="IBasicTreeNode"/>
//    /// </summary>
//    private IBasicTreeNode mParent;

    /// <summary>
    /// Constructor setting the owner <see cref="Presentation"/>
    /// </summary>
    /// <param name="presentation"></param>
    internal CoreNode(Presentation presentation)
    {
      mPresentation = presentation;
      mProperties = new property.IProperty[PROPERTY_TYPE_ARRAY.Length];
    }


//    #region ICoreNode Members
//
//
//    #endregion
//
//    #region ITreeNode Members
//
//    /// <summary>
//    /// Gets the index of a given child <see cref="ITreeNode"/>
//    /// </summary>
//    /// <param name="node">The given child <see cref="ITreeNode"/></param>
//    /// <returns>The index of the given child</returns>
//    /// <exception cref="exception.MethodParameterIsNullException">
//    /// Thrown when parameter <paranref name="node"/> is null</exception>
//    /// <exception cref="exception.NodeDoesNotExistException">
//    /// Thrown when <paramref name="node"/> is not a child of the <see cref="ITreeNode"/></exception>
//    public int indexOf(ITreeNode node)
//    {
//      if (node!=null)
//      {
//        int index = mChildren.IndexOf(node);
//        if (index==-1)
//        {
//          throw new exception.NodeDoesNotExistException(
//            "The given node is not a child and thus does not have an index");
//        }
//        return index;
//      }
//      throw new exception.MethodParameterIsNullException("Can not find index of null node");
//    }
//
//
//    /// <summary>
//    /// Removes a given <see cref="ITreeNode"/> child. 
//    /// </summary>
//    /// <param name="node">The <see cref="ITreeNode"/> child to remove</param>
//    /// <returns>The removed child</returns>
//    /// <exception cref="exception.MethodParameterIsNullException">
//    /// Thrown when parameter <paramref name="node"/> is null</exception>
//    /// <exception cref="exception.NodeDoesNotExistException">
//    /// Thrown when <paramref name="node"/> is not a child of the instance <see cref="ITreeNode"/></exception>
//    public ITreeNode removeChild(ITreeNode node)
//    {
//      return (ITreeNode)removeChild(indexOf(node));
//    }
//
//    /// <summary>
//    /// Inserts a new <see cref="ITreeNode"/> child before the given child.
//    /// </summary>
//    /// <param name="newChild">The new <see cref="ITreeNode"/> child node</param>
//    /// <param name="anchorNode">The child before which to insert the new child</param>
//    /// <exception cref="exception.MethodParameterIsNullException">
//    /// Thrown when parameters <paramref name="newChild"/> and/or <paramref name="anchorNode"/> 
//    /// have null values</exception>
//    /// <exception cref="exception.NodeDoesNotExistException">
//    /// Thrown when <paramref name="anchorNode"/> is not a child of the instance <see cref="ITreeNode"/></exception>
//    public void insertBefore(ITreeNode newChild, ITreeNode anchorNode)
//    {
//      insertBefore(newChild, indexOf(anchorNode));
//    }
//
//    /// <summary>
//    /// Inserts a <see cref="ITreeNode"/> child after a given anchor child <see cref="ITreeNode"/>
//    /// </summary>
//    /// <param name="newChild">The new child <see cref="ITreeNode"/> to insert</param>
//    /// <param name="anchorNodeIndex">The index of the anchor child <see cref="ITreeNode"/>
//    /// after which to insert the new child</param>
//    public void insertAfter(ITreeNode newChild, int anchorNodeIndex)
//    {
//      if (anchorNodeIndex<0 || getChildCount()<=anchorNodeIndex)
//      {
//        throw new exception.MethodParameterIsOutOfBoundsException(
//          "Anchor index is out of bounds");
//      }
//      if (anchorNodeIndex<(getChildCount()-1))
//      {
//        insertBefore(newChild, anchorNodeIndex+1);
//      }
//      else
//      {
//        appendChild(newChild);
//      }
//    }
//
//    /// <summary>
//    /// Inserts a new <see cref="ITreeNode"/> child after the given child.
//    /// </summary>
//    /// <param name="newNode">The new <see cref="ITreeNode"/> child node</param>
//    /// <param name="anchorNode">The child after which to insert the new child</param>
//    /// <exception cref="exception.MethodParameterIsNullException">
//    /// Thrown when parameters <paramref name="newNode"/> and/or <paramref name="anchorNode"/> 
//    /// have null values</exception>
//    /// <exception cref="exception.NodeDoesNotExistException">
//    /// Thrown when <paramref name="anchorNode"/> is not a child of the instance <see cref="ITreeNode"/></exception>
//    public void insertAfter(ITreeNode newNode, ITreeNode anchorNode)
//    {
//      insertAfter(newNode, indexOf(anchorNode));
//    }
//
//    /// <summary>
//    /// Replaces an existing child <see cref="ITreeNode"/> with i new one
//    /// </summary>
//    /// <param name="node">The new child with which to replace</param>
//    /// <param name="oldNode">The existing child node to replace</param>
//    /// <returns>The replaced <see cref="ITreeNode"/> child</returns>
//    /// <exception cref="exception.MethodParameterIsNullException">
//    /// Thrown when parameters <paramref name="node"/> and/or <paramref name="oldNode"/> 
//    /// have null values</exception>
//    /// <exception cref="exception.NodeDoesNotExistException">
//    /// Thrown when <paramref name="oldNode"/> is not a child of the instance <see cref="ITreeNode"/></exception>
//    public ITreeNode replaceChild(ITreeNode node, ITreeNode oldNode)
//    {
//      return replaceChild(node, indexOf(oldNode));
//    }
//
//    /// <summary>
//    /// Replaces the child <see cref="ITreeNode"/> at a given index with a new <see cref="ITreeNode"/>
//    /// </summary>
//    /// <param name="node">The new <see cref="ITreeNode"/> with which to replace</param>
//    /// <param name="index">The index of the child <see cref="ITreeNode"/> to replace</param>
//    /// <returns>The replaced child <see cref="ITreeNode"/></returns>
//    /// <exception cref="exception.MethodParameterIsNullException">
//    /// Thrown when parameter <paranref name="node"/> is null</exception>
//    /// <exception cref="exception.MethodParameterIsOutOfBoundsException">
//    /// Thrown when index is out if range, 
//    /// that is when<c><paramref name="index"/> &lt; 0</c> 
//    /// or <c><see cref="IBasicTreeNode.getChildCount"/>() &lt; <paramref name="index"/></c></exception>
//    public ITreeNode replaceChild(ITreeNode node, int index)
//    {
//      if (index<0 || getChildCount()<=index)
//      {
//        throw new exception.MethodParameterIsOutOfBoundsException(
//          "Child index is out of bounds");
//      }
//      ITreeNode oldNode = (ITreeNode)mChildren[index];
//      mChildren[index] = node;
//      return oldNode;
//    }
//
//    #endregion
//
//    #region IBasicTreeNode Members
//
//    public IBasicTreeNode getChild(int index)
//    {
//      if (index<0 || getChildCount()<=index)
//      {
//        throw new exception.MethodParameterIsOutOfBoundsException(
//          "Child index is out of bounds");
//      }
//      return (IBasicTreeNode)mChildren[index];
//    }
//
//    public IBasicTreeNode removeChild(int index)
//    {
//      if (index<0 || getChildCount()<=index)
//      {
//        throw new exception.MethodParameterIsOutOfBoundsException(
//          "Child index is out of bounds");
//      }
//      IBasicTreeNode removedChild = (IBasicTreeNode)mChildren[index];
//      mChildren.RemoveAt(index);
//      return removedChild;
//    }
//
//    public void appendChild(IBasicTreeNode newChild)
//    {
//      mChildren.Add(newChild);
//    }
//
//    public void insertBefore(IBasicTreeNode newChild, int anchorNodeIndex)
//    {
//      if (index<0 || getChildCount()<=index)
//      {
//        throw new exception.MethodParameterIsOutOfBoundsException(
//          "Anchor index is out of bounds");
//      }
//      mChildren.Insert(newChild, anchorNodeIndex);
//
//    }
//
//    public IBasicTreeNode detach()
//    {
//      // TODO:  Add CoreNode.detach implementation
//      return null;
//    }
//
//    public IBasicTreeNode getParent()
//    {
//      return mParent;
//    }
//
//    public int getChildCount()
//    {
//      // TODO:  Add CoreNode.getChildCount implementation
//      return 0;
//    }
//
//    #endregion
//
//    #region IVisitableCoreNode Members
//
//    /// <summary>
//    /// Accept a <see cref="ICoreNodeVisitor"/> in depth first mode
//    /// </summary>
//    /// <param name="visitor">The <see cref="ICoreNodeVisitor"/></param>
//    public void AcceptDepthFirst(ICoreNodeVisitor visitor)
//    {
//      if (visitor.preVisit(this))
//      {
//        for (int i=0; i<getChildCount(); i++)
//        {
//          ((ICoreNode)getChild(i)).AcceptDepthFirst(visitor);
//        }
//      }
//      visitor.postVisit(this);
//    }
//
//    public void AcceptBreadthFirst(ICoreNodeVisitor visitor)
//    {
//      // TODO:  Add CoreNode.AcceptBreadthFirst implementation
//    }
//
//    #endregion

    #region ICoreNode Members

    /// <summary>
    /// Gets the <see cref="Presentation"/> owning the <see cref="ICoreNode"/>
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

    #region IVisitableCoreNode Members

    public void AcceptDepthFirst(ICoreNodeVisitor visitor)
    {
      // TODO:  Add CoreNode.AcceptDepthFirst implementation
    }

    public void AcceptBreadthFirst(ICoreNodeVisitor visitor)
    {
      // TODO:  Add CoreNode.AcceptBreadthFirst implementation
    }

    #endregion
  }
}
