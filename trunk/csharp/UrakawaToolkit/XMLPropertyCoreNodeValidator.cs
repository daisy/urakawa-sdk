using System;

namespace urakawa.core
{
	/// <summary>
	/// Summary description for XMLPropertyCoreNodeValidator.
	/// </summary>
	public class XMLPropertyCoreNodeValidator : ICoreNodeValidator
	{
		public XMLPropertyCoreNodeValidator()
		{
			//
			// TODO: Add constructor logic here
			//
		}
		#region ICoreNodeValidator Members

		public bool canSetProperty(IProperty newProp, ICoreNode contextNode)
		{
			// TODO:  Add XMLPropertyCoreNodeValidator.canSetProperty implementation
			return false;
		}

		public bool canRemoveChild(ICoreNode node)
		{
			// TODO:  Add XMLPropertyCoreNodeValidator.canRemoveChild implementation
			return false;
		}

		public bool canInsert(ICoreNode node, int index, ICoreNode contextNode)
		{
			// TODO:  Add XMLPropertyCoreNodeValidator.canInsert implementation
			return false;
		}

		public bool canInsertBefore(ICoreNode node, ICoreNode anchorNode)
		{
			// TODO:  Add XMLPropertyCoreNodeValidator.canInsertBefore implementation
			return false;
		}

		public bool canInsertAfter(ICoreNode node, ICoreNode anchorNode)
		{
			// TODO:  Add XMLPropertyCoreNodeValidator.canInsertAfter implementation
			return false;
		}

		public bool canReplaceChild(ICoreNode node, ICoreNode oldNode)
		{
			// TODO:  Add XMLPropertyCoreNodeValidator.canReplaceChild implementation
			return false;
		}

		bool urakawa.core.ICoreNodeValidator.canReplaceChild(ICoreNode node, int index, ICoreNode contextNode)
		{
			// TODO:  Add XMLPropertyCoreNodeValidator.urakawa.core.ICoreNodeValidator.canReplaceChild implementation
			return false;
		}

		bool urakawa.core.ICoreNodeValidator.canRemoveChild(int index, ICoreNode contextNode)
		{
			// TODO:  Add XMLPropertyCoreNodeValidator.urakawa.core.ICoreNodeValidator.canRemoveChild implementation
			return false;
		}

		public bool canAppendChild(ICoreNode node, ICoreNode contextNode)
		{
			// TODO:  Add XMLPropertyCoreNodeValidator.canAppendChild implementation
			return false;
		}

		public bool canDetach(ICoreNode contextNode)
		{
			// TODO:  Add XMLPropertyCoreNodeValidator.canDetach implementation
			return false;
		}

		#endregion
	}
}
