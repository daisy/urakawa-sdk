using System;
using urakawa.core;
using urakawa.core.property;

namespace urakawa.validation.core
{
	/// <summary>
	/// Composite implementation of <see cref="ICoreNodeValidator"/> 
	/// consisting of zero or more <see cref="ICoreNodeValidator"/> members.
	/// Any test will pass if and only if the corresponding test on all members also pass
	/// </summary>
  public class CompositeCoreNodeValidator : ICoreNodeValidator
  {
    private ICoreNodeValidator[] mValidators;

    internal CompositeCoreNodeValidator(ICoreNodeValidator[] validators)
    {
      mValidators = validators;
      
    }

    #region ICoreNodeValidator Members

    /// <summary>
    /// Determines if a given <see cref="IProperty"/> can be set for a given context <see cref="ICoreNode"/>
    /// </summary>
    /// <param name="newProp">The given <see cref="IProperty"/></param>
    /// <param name="contextNode">The comntext <see cref="ICoreNode"/></param>
    /// <returns>A <see cref="bool"/> indicating if the <see cref="IProperty"/> can be set</returns>
    public bool canSetProperty(IProperty newProp, ICoreNode contextNode)
    {
      foreach (ICoreNodeValidator v in mValidators)
      {
        if (!v.canSetProperty(newProp, contextNode)) return false;
      }
      return true;
    }
		

    /// <summary>
    /// Determines if a given child <see cref="ICoreNode"/> can be removed it's parent
    /// </summary>
    /// <param name="node">The given child <see cref="ICoreNode"/></param>
    /// <returns>A <see cref="bool"/> indicating if <paramref name="node"/> can be removed from it's parent</returns>
    public bool canRemoveChild(ICoreNode node)
    {
      foreach (ICoreNodeValidator v in mValidators)
      {
        if (!v.canRemoveChild(node)) return false;
      }
      return true;
    }

    /// <summary>
    /// Determines if a given <see cref="ICoreNode"/> can be inserted as a child 
    /// of a given context <see cref="ICoreNode"/> at a given index
    /// </summary>
    /// <param name="node">The <see cref="ICoreNode"/> to insert</param>
    /// <param name="index">The index at which to insert</param>
    /// <param name="contextNode">The context <see cref="ICoreNode"/></param>
    /// <returns>A <see cref="bool"/> indicating if <paramref name="node"/> 
    /// can be inserted as a child of <paramref name="context"/> 
    /// at index <paramref name="index"/></returns>
    public bool canInsert(ICoreNode node, int index, ICoreNode contextNode)
    {
      foreach (ICoreNodeValidator v in mValidators)
      {
        if (!v.canInsert(node, index, contextNode)) return false;
      }
      return true;
    }
		
    /// <summary>
    /// Determines if a given <see cref="ICoreNode"/> can be inserted before a given anchor <see cref="ICoreNode"/>
    /// </summary>
    /// <param name="node">The given <see cref="ICoreNode"/> to be tested for insertion</param>
    /// <param name="anchorNode">The anchor <see cref="ICoreNode"/></param>
    /// <returns>
    /// A <see cref="bool"/> indicating if <paramref name="node"/> 
    /// can be inserted as a sibling before <paramref name="anchorNode"/>
    /// </returns>
    public bool canInsertBefore(ICoreNode node, ICoreNode anchorNode)
		{
      foreach (ICoreNodeValidator v in mValidators)
      {
        if (!v.canInsertBefore(node, anchorNode)) return false;
      }
      return true;
		}
		
    /// <summary>
    /// Determines if a given <see cref="ICoreNode"/> can be inserted after a given anchor <see cref="ICoreNode"/>
    /// </summary>
    /// <param name="node">The given <see cref="ICoreNode"/> to be tested for insertion</param>
    /// <param name="anchorNode">The anchor <see cref="ICoreNode"/></param>
    /// <returns>
    /// A <see cref="bool"/> indicating if <paramref name="node"/> 
    /// can be inserted as a sibling after <paramref name="anchorNode"/>
    /// </returns>
    public bool canInsertAfter(ICoreNode node, ICoreNode anchorNode)
		{
      foreach (ICoreNodeValidator v in mValidators)
      {
        if (!v.canInsertAfter(node, anchorNode)) return false;
      }
			return true;
		}

    /// <summary>
    /// Determines if a <see cref="ICoreNode"/> can replace another <see cref="ICoreNode"/>
    /// </summary>
    /// <param name="node">The <see cref="ICoreNode"/> to replace with</param>
    /// <param name="oldNode">The <see cref="ICoreNode"/> to be replaced</param>
    /// <returns>A <see cref="bool"/> indicating if <paramref name="node"/> 
    /// can replace <paramref name="oldNode"/> in the list of children 
    /// of the parent of <paramref name="oldNode"/></returns>
    public bool canReplaceChild(ICoreNode node, ICoreNode oldNode)
		{
      foreach (ICoreNodeValidator v in mValidators)
      {
        if (!v.canReplaceChild(node, oldNode)) return false;
      }
      return true;
		}
		
    /// <summary>
    /// Determines if a given <see cref="ICoreNode"/> can replace the child 
    /// of a given context <see cref="ICoreNode"/> at a given index
    /// </summary>
    /// <param name="node">The <see cref="ICoreNode"/> with which to replace</param>
    /// <param name="index">The index of the child to replace</param>
    /// <param name="contextNode">The context <see cref="ICoreNode"/></param>
    /// <returns>A <see cref="bool"/> indicating if <paramref name="node"/> can replace 
    /// the child of <paramref name="contextNode"/> at index <paramref name="index"/></returns>
    public bool canReplaceChild(ICoreNode node, int index, ICoreNode contextNode)
		{
      foreach (ICoreNodeValidator v in mValidators)
      {
        if (!v.canReplaceChild(node, index, contextNode)) return false;
      }
			return true;
		}
		
    /// <summary>
    /// Determines if the child at a given index can be removed from a given context <see cref="ICoreNode"/>
    /// </summary>
    /// <param name="index">The given index</param>
    /// <param name="contextNode">The context <see cref="ICoreNode"/></param>
    /// <returns>A <see cref="bool"/> indicating if the child of <paramref name="contentNode"/>
    /// at index <paramref name="index"/> can be removed</returns>
    public bool canRemoveChild(int index, ICoreNode contextNode)
		{
      foreach (ICoreNodeValidator v in mValidators)
      {
        if (!v.canRemoveChild(index, contextNode)) return false;
      }
			return true;
		}

    /// <summary>
    /// Determines if a given <see cref="ICoreNode"/> can be appended to a given context <see cref="ICoreNode"/>
    /// </summary>
    /// <param name="node">The <see cref="ICoreNode"/> to append</param>
    /// <param name="contextNode">The context <see cref="ICoreNode"/></param>
    /// <returns>A <see cref="bool"/> indocating if <paramref name="node"/> can be appended to 
    /// the list of children of <paramref name="contextNode"/></returns>
    public bool canAppendChild(ICoreNode node, ICoreNode contextNode)
		{
      foreach (ICoreNodeValidator v in mValidators)
      {
        if (!v.canAppendChild(node, contextNode)) return false;
      }
      return true;
		}

    /// <summary>
    /// Determines if a given context <see cref="ICoreNode"/> can be detached from it's parent
    /// </summary>
    /// <param name="contextNode">The content <see cref="ICoreNode"/></param>
    /// <returns>
    /// A <see cref="bool"/> indicating if <paramref name="contextNode"/> can be detached from it's parent
    /// </returns>
    public bool canDetach(ICoreNode contextNode)
		{
      foreach (ICoreNodeValidator v in mValidators)
      {
        if (!v.canDetach(contextNode)) return false;
      }
      return true;
		}

		#endregion
	}
}

