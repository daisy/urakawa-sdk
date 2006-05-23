using System;

namespace urakawa.core
{
	/// <summary>
	/// Interface for validators validating operations on <see cref="ICoreNode"/>s
	/// </summary>
	public interface ICoreNodeValidator
	{
    /// <summary>
    /// Determines if a given <see cref="IProperty"/> can be set for a given context <see cref="ICoreNode"/>
    /// </summary>
    /// <param name="newProp">The given <see cref="IProperty"/></param>
    /// <param name="contextNode">The comntext <see cref="ICoreNode"/></param>
    /// <returns>A <see cref="bool"/> indicating if the <see cref="IProperty"/> can be set</returns>
		bool canSetProperty(IProperty newProp, ICoreNode contextNode);
		
    /// <summary>
    /// Determines if a given child <see cref="ICoreNode"/> can be removed it's parent
    /// </summary>
    /// <param name="node">The given child <see cref="ICoreNode"/></param>
    /// <returns>A <see cref="bool"/> indicating if <paramref name="node"/> can be removed from it's parent</returns>
		bool canRemoveChild(ICoreNode node);
		
    /// <summary>
    /// Determines if a given <see cref="ICoreNode"/> can be inserted before a given anchor <see cref="ICoreNode"/>
    /// </summary>
    /// <param name="node">The given <see cref="ICoreNode"/> to be tested for insertion</param>
    /// <param name="anchorNode">The anchor <see cref="ICoreNode"/></param>
    /// <returns>
    /// A <see cref="bool"/> indicating if <paramref name="node"/> 
    /// can be inserted as a sibling before <paramref name="anchorNode"/>
    /// </returns>
		bool canInsertBefore(ICoreNode node, ICoreNode anchorNode); 
		
    /// <summary>
    /// Determines if a given <see cref="ICoreNode"/> can be inserted after a given anchor <see cref="ICoreNode"/>
    /// </summary>
    /// <param name="node">The given <see cref="ICoreNode"/> to be tested for insertion</param>
    /// <param name="anchorNode">The anchor <see cref="ICoreNode"/></param>
    /// <returns>
    /// A <see cref="bool"/> indicating if <paramref name="node"/> 
    /// can be inserted as a sibling after <paramref name="anchorNode"/>
    /// </returns>
    bool canInsertAfter(ICoreNode node, ICoreNode anchorNode);

    /// <summary>
    /// Determines if a <see cref="ICoreNode"/> can replace another <see cref="ICoreNode"/>
    /// </summary>
    /// <param name="node">The <see cref="ICoreNode"/> to replace with</param>
    /// <param name="oldNode">The <see cref="ICoreNode"/> to be replaced</param>
    /// <returns>A <see cref="bool"/> indicating if <paramref name="node"/> 
    /// can replace <paramref name="oldNode"/> in the list of children 
    /// of the parent of <paramref name="oldNode"/></returns>
		bool canReplaceChild(ICoreNode node, ICoreNode oldNode); 
		
    /// <summary>
    /// Determines if a given <see cref="ICoreNode"/> can replace the child 
    /// of a given context <see cref="ICoreNode"/> at a given index
    /// </summary>
    /// <param name="node">The <see cref="ICoreNode"/> with which to replace</param>
    /// <param name="index">The index of the child to replace</param>
    /// <param name="contextNode">The context <see cref="ICoreNode"/></param>
    /// <returns>A <see cref="bool"/> indicating if <paramref name="node"/> can replace 
    /// the child of <see cref="context"/> at index <see cref="index"/></returns>
    bool canReplaceChild(ICoreNode node, int index, ICoreNode contextNode); 

		/// <summary>
		/// Determines if the child at a given index can be removed from a given context <see cref="ICoreNode"/>
		/// </summary>
		/// <param name="index">The given index</param>
		/// <param name="contextNode">The context <see cref="ICoreNode"/></param>
		/// <returns>A <see cref="bool"/> indicating if the child of <paramref name="contentNode"/>
		/// at index <paramref name="index"/> can be removed</returns>
		bool canRemoveChild(int index, ICoreNode contextNode);


    /// <summary>
    /// Determines if a given <see cref="ICoreNode"/> can be appended to a given context <see cref="ICoreNode"/>
    /// </summary>
    /// <param name="node">The <see cref="ICoreNode"/> to append</param>
    /// <param name="contextNode">The context <see cref="ICoreNode"/></param>
    /// <returns>A <see cref="bool"/> indocating if <paramref name="node"/> can be appended to 
    /// the list of children of <paramref name="contextNode"/></returns>
		bool canAppendChild(ICoreNode node, ICoreNode contextNode); 

    /// <summary>
    /// Determines if a given context <see cref="ICoreNode"/> can be detached from it's parent
    /// </summary>
    /// <param name="contextNode">The content <see cref="ICoreNode"/></param>
    /// <returns>
    /// A <see cref="bool"/> indicating if <paramref name="contextNode"/> can be detached from it's parent
    /// </returns>
		bool canDetach(ICoreNode contextNode);
	}
}
