using System;

namespace urakawa.core
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

    bool ICoreNodeValidator.canSetProperty(IProperty newProp, ICoreNode contextNode)
    {
      foreach (ICoreNodeValidator v in mValidators)
      {
        if (!v.canSetProperty(newProp, contextNode)) return false;
      }
      return true;
    }
		

    bool ICoreNodeValidator.canRemoveChild(ICoreNode node)
    {
      foreach (ICoreNodeValidator v in mValidators)
      {
        if (!v.canRemoveChild(node)) return false;
      }
      return true;
    }

    bool ICoreNodeValidator.canInsert(ICoreNode node, int index, ICoreNode contextNode)
    {
      foreach (ICoreNodeValidator v in mValidators)
      {
        if (!v.canInsert(node, index, contextNode)) return false;
      }
      return true;
    }
		
		bool ICoreNodeValidator.canInsertBefore(ICoreNode node, ICoreNode anchorNode)
		{
      foreach (ICoreNodeValidator v in mValidators)
      {
        if (!v.canInsertBefore(node, anchorNode)) return false;
      }
      return true;
		}
		
		bool ICoreNodeValidator.canInsertAfter(ICoreNode node, ICoreNode anchorNode)
		{
      foreach (ICoreNodeValidator v in mValidators)
      {
        if (!v.canInsertAfter(node, anchorNode)) return false;
      }
			return true;
		}

		bool ICoreNodeValidator.canReplaceChild(ICoreNode node, ICoreNode oldNode)
		{
      foreach (ICoreNodeValidator v in mValidators)
      {
        if (!v.canReplaceChild(node, oldNode)) return false;
      }
      return true;
		}
		
		bool ICoreNodeValidator.canReplaceChild(ICoreNode node, int index, ICoreNode contextNode)
		{
      foreach (ICoreNodeValidator v in mValidators)
      {
        if (!v.canReplaceChild(node, index, contextNode)) return false;
      }
			return true;
		}
		
		bool ICoreNodeValidator.canRemoveChild(int index, ICoreNode contextNode)
		{
      foreach (ICoreNodeValidator v in mValidators)
      {
        if (!v.canRemoveChild(index, contextNode)) return false;
      }
			return true;
		}

		bool ICoreNodeValidator.canAppendChild(ICoreNode node, ICoreNode contextNode)
		{
      foreach (ICoreNodeValidator v in mValidators)
      {
        if (!v.canAppendChild(node, contextNode)) return false;
      }
      return true;
		}

		bool ICoreNodeValidator.canDetach(ICoreNode contextNode)
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

