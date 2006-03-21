using System;

namespace urakawa.core
{
	/// <summary>
	/// 
	/// </summary>
	internal class DOMNode : LimitedDOMNode, IDOMNode
	{
    //TODO: Add exception handling throughout
		internal DOMNode(ICoreNode owner, ICoreNode parent) : base(owner, parent)
		{
    }
    #region IDOMNode Members

    /// <summary>
    /// See <see cref="IDOMNode.insertBefore"/>
    /// </summary>
    public void insertBefore(ICoreNode newChild, ICoreNode anchorNode)
    {
      if (newChild==null)
      {
        throw new exception.MethodParameterIsNullException("Parameter 'newChild' is null");
      }
      insertChild(newChild, indexOfChild(anchorNode));
    }

    /// <summary>
    /// See <see cref="IDOMNode.insertAfter"/>
    /// </summary>
    public void insertAfter(ICoreNode newChild, ICoreNode anchorNode)
    {
      if (newChild==null)
      {
        throw new exception.MethodParameterIsNullException("Parameter 'newChild' is null");
      }
      insertChild(newChild, indexOfChild(anchorNode));
    }

    /// <summary>
    /// See <see cref="IDOMNode.removeChild"/>
    /// </summary>
    public ICoreNode removeChild(ICoreNode node)
    {
      return removeChild(indexOfChild(node));
    }

    /// <summary>
    /// See <see cref="IDOMNode.replaceChild"/>
    /// </summary>
    public ICoreNode replaceChild(ICoreNode node, ICoreNode oldNode)
    {
      if (node==null)
      {
        throw new exception.MethodParameterIsNullException("Parameter 'node' is null");
      }
      return replaceChild(node, indexOfChild(oldNode));
    }

    #endregion
  }
}
