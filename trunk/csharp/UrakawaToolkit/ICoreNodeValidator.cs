using System;

namespace urakawa.core
{
	/// <summary>
	/// Summary description for ICoreNodeValidator.
	/// </summary>
	public interface ICoreNodeValidator
	{
		bool canSetProperty(IProperty newProp);
		
		bool canRemoveChild(TreeNode node);
		
		bool canInsertBefore(TreeNode node, TreeNode anchorNode); 
		
		bool canInsertAfter(TreeNode node, TreeNode anchorNode);

		bool canReplaceChild(TreeNode node, TreeNode oldNode); 
		
		bool canReplaceChild(TreeNode node, int index); 
		
		bool canRemoveChild(int index);

		bool canAppendChild(BasicTreeNode node); 

		bool canInsertBefore(BasicTreeNode node, int anchorNodeIndex); 

		bool canDetach();
	}
}
