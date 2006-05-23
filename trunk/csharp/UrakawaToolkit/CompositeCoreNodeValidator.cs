using System;

namespace urakawa.core
{
	/// <summary>
	/// Summary description for CompositeCoreNodeValidator.
	/// </summary>
  public class CompositeCoreNodeValidator : ICoreNodeValidator
  {
    private ICoreNodeValidator[] mValidators;

    internal CompositeCoreNodeValidator(ICoreNodeValidator[] validators)
    {
      mValidators = validators;
    }

    #region ICoreNodeValidator Members

    //TODO: implement all of these functions

    public bool canSetProperty(IProperty newProp, ICoreNode contextNode)
    {
      foreach (ICoreNodeValidator v in mValidators)
      {
        if (!v.canSetProperty(newProp, contextNode)) return false;
      }
      return true;
    }
		

    public bool canRemoveChild(ICoreNode node)
    {
      foreach (ICoreNodeValidator v in mValidators)
      {
        if (!v.canRemoveChild(node)) return false;
      }
      return true;
    }

    public bool canInsert(ICoreNode node, int index, ICoreNode contextNode)
    {
      foreach (ICoreNodeValidator v in mValidators)
      {
        if (!v.canInsert(node, index, contextNode)) return false;
      }
      return true;
    }
		
		public bool canInsertBefore(ICoreNode node, ICoreNode anchorNode)
		{
      foreach (ICoreNodeValidator v in mValidators)
      {
        if (!v.canInsertBefore(node, anchorNode)) return false;
      }
      return true;
		}
		
		public bool canInsertAfter(ICoreNode node, ICoreNode anchorNode)
		{
      foreach (ICoreNodeValidator v in mValidators)
      {
        if (!v.canInsertAfter(node, anchorNode)) return false;
      }
			return true;
		}

		public bool canReplaceChild(ICoreNode node, ICoreNode oldNode)
		{
      foreach (ICoreNodeValidator v in mValidators)
      {
        if (!v.canReplaceChild(node, oldNode)) return false;
      }
      return true;
		}
		
		public bool canReplaceChild(ICoreNode node, int index, ICoreNode contextNode)
		{
      foreach (ICoreNodeValidator v in mValidators)
      {
        if (!v.canReplaceChild(node, index, contextNode)) return false;
      }
			return true;
		}
		
		public bool canRemoveChild(int index, ICoreNode contextNode)
		{
      foreach (ICoreNodeValidator v in mValidators)
      {
        if (!v.canRemoveChild(index, contextNode)) return false;
      }
			return true;
		}

		public bool canAppendChild(ICoreNode node, ICoreNode contextNode)
		{
      foreach (ICoreNodeValidator v in mValidators)
      {
        if (!v.canAppendChild(node, contextNode)) return false;
      }
      return true;
		}

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

