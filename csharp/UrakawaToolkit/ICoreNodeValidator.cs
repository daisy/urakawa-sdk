using System;

namespace urakawa.core
{
	/// <summary>
	/// Summary description for ICoreNodeValidator.
	/// </summary>
	public interface ICoreNodeValidator
	{
		bool canSetProperty(IProperty newProp, ICoreNode contextNode);
		
		bool canRemoveChild(ICoreNode node);
		
		bool canInsertBefore(ICoreNode node, ICoreNode anchorNode); 
		
		bool canInsertAfter(ICoreNode node, ICoreNode anchorNode);

		bool canReplaceChild(ICoreNode node, ICoreNode oldNode); 
		
		bool canReplaceChild(ICoreNode node, int index, ICoreNode contextNode); 
		
		bool canRemoveChild(int index, ICoreNode contextNode);

		bool canAppendChild(ICoreNode node, ICoreNode contextNode); 

		bool canDetach();
	}
}
