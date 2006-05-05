using System;

namespace urakawa.core
{
	/// <summary>
	/// Summary description for ICoreNodeValidator.
	/// </summary>
	public interface ICoreNodeValidator
	{
		/*commented out until the required data types have been 
		 * implemented, otherwise it doesn't compile
		 */
		/*
		//throws MethodParameterIsNullException;
		boolean canSetProperty(IProperty newProp);
		
		//throws NodeDoesNotExistException, MethodParameterIsNullException;
		boolean canRemoveChild(TreeNode node);
		
		//throws MethodParameterIsNullException, NodeDoesNotExistException;
		boolean canInsertBefore(TreeNode node, TreeNode anchorNode); 
		
		//throws NodeDoesNotExistException, MethodParameterIsNullException;
		boolean canInsertAfter(TreeNode node, TreeNode anchorNode);

		//throws NodeDoesNotExistException, MethodParameterIsNullException;
		boolean canReplaceChild(TreeNode node, TreeNode oldNode); 
		
		//throws MethodParameterIsOutOfBoundsException, MethodParameterIsNullException;
		boolean canReplaceChild(TreeNode node, int index); 
		
		// throws MethodParameterIsOutOfBoundsException;
		boolean canRemoveChild(int index);

		//throws MethodParameterIsNullException;
		boolean canAppendChild(BasicTreeNode node); 

		//throws MethodParameterIsNullException, MethodParameterIsOutOfBoundsException;
		boolean canInsertBefore(BasicTreeNode node, int anchorNodeIndex); 

		boolean canDetach();*/
	}
}
