using System;

namespace urakawa.core
{
	/// <summary>
	/// Summary description for TreeNodeValidator.
	/// </summary>
	public class TreeNodeValidator : ICoreNodeValidator
	{
		public TreeNodeValidator()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		#region ICoreNodeValidator Members

		//@todo
		//implement all of these functions

		public bool canSetProperty(IProperty newProp, ICoreNode contextNode)
		{
			return false;
		}
		
		public bool canRemoveChild(ICoreNode node)
		{
			return false;
		}
		
		public bool canInsertBefore(ICoreNode node, ICoreNode anchorNode)
		{
			return false;
		}
		
		public bool canInsertAfter(ICoreNode node, ICoreNode anchorNode)
		{
			return false;
		}

		public bool canReplaceChild(ICoreNode node, ICoreNode oldNode)
		{
			return false;
		}
		
		public bool canReplaceChild(ICoreNode node, int index, ICoreNode contextNode)
		{
			return false;
		}
		
		public bool canRemoveChild(int index, ICoreNode contextNode)
		{
			return false;
		}

		public bool canAppendChild(ICoreNode node, ICoreNode contextNode)
		{
			return false;
		}

		public bool canDetach(ICoreNode contextNode)
		{
			return false;
		}

		#endregion
	}
}
