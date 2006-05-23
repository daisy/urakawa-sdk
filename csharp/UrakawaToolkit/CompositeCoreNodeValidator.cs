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
      return true;
    }

    public bool canInsert(ICoreNode node, int index, ICoreNode contextNode)
    {
      return true;
    }
		
		public bool canInsertBefore(ICoreNode node, ICoreNode anchorNode)
		{
			return true;
		}
		
		public bool canInsertAfter(ICoreNode node, ICoreNode anchorNode)
		{
			return true;
		}

		public bool canReplaceChild(ICoreNode node, ICoreNode oldNode)
		{
			return true;
		}
		
		public bool canReplaceChild(ICoreNode node, int index, ICoreNode contextNode)
		{
			return true;
		}
		
		public bool canRemoveChild(int index, ICoreNode contextNode)
		{
			return true;
		}

		public bool canAppendChild(ICoreNode node, ICoreNode contextNode)
		{
			return true;
		}

		public bool canDetach(ICoreNode contextNode)
		{
			return true;
		}

		#endregion
	}
}

