using System;
using System.Xml;

namespace urakawa.core
{
	/// <summary>
	/// Implementation of <see cref="CoreNode"/> interface
	/// </summary>
	public class CoreNode : ICoreNode
  {
    #region ICoreNode Members

    public Presentation getPresentation()
    {
      // TODO:  Add CoreNode.getPresentation implementation
      return null;
    }

    public property.IProperty getProperty(property.PropertyType type)
    {
      // TODO:  Add CoreNode.getProperty implementation
      return null;
    }

    public void setProperty(property.IProperty prop)
    {
      // TODO:  Add CoreNode.setProperty implementation
    }

    #endregion

    #region IDOMNode Members

    public void insertBefore(ICoreNode newNode, ICoreNode anchorNode)
    {
      // TODO:  Add CoreNode.insertBefore implementation
    }

    public void insertAfter(ICoreNode newNode, ICoreNode anchorNode)
    {
      // TODO:  Add CoreNode.insertAfter implementation
    }

    public ICoreNode removeChild(ICoreNode node)
    {
      // TODO:  Add CoreNode.removeChild implementation
      return null;
    }

    public ICoreNode replaceChild(ICoreNode node, ICoreNode oldNode)
    {
      // TODO:  Add CoreNode.replaceChild implementation
      return null;
    }

    public ICoreNode detach()
    {
      // TODO:  Add CoreNode.detach implementation
      return null;
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

    #region ILimitedDOMNode Members

    public ICoreNode getParent()
    {
      // TODO:  Add CoreNode.getParent implementation
      return null;
    }

    public void appendChild(ICoreNode newChild)
    {
      // TODO:  Add CoreNode.appendChild implementation
    }

    public void insert(ICoreNode newNode, int index)
    {
      // TODO:  Add CoreNode.insert implementation
    }

    public ICoreNode getChild(int index)
    {
      // TODO:  Add CoreNode.getChild implementation
      return null;
    }

    ICoreNode ILimitedDOMNode.removeChild(int index)
    {
      // TODO:  Add CoreNode.Urakawa.Core.ILimitedDOMNode.removeChild implementation
      return null;
    }

    ICoreNode ILimitedDOMNode.replaceChild(ICoreNode node, int index)
    {
      // TODO:  Add CoreNode.Urakawa.Core.ILimitedDOMNode.replaceChild implementation
      return null;
    }

    public int getChildCount()
    {
      // TODO:  Add CoreNode.getChildCount implementation
      return 0;
    }

    public int indexOf(ICoreNode node)
    {
      // TODO:  Add CoreNode.indexOf implementation
      return 0;
    }

    #endregion
  }
}
