using System;
using System.Collections.Generic;
using System.Text;
using urakawa.core.property;

namespace urakawa.core
{
	/// <summary>
	/// Provides the write-only tree methods of a <see cref="ICoreNode"/>
	/// </summary>
	public interface ICoreNodeWriteOnlyMethods
	{
		/// <summary>
		/// Sets the parent <see cref="ICoreNode"/>. For internal use only, should not be called by users
		/// </summary>
		/// <param name="node">The new parent</param>
		void setParent(ICoreNode node);

		/// <summary>
		/// Inserts a <see cref="ICoreNode"/> child at a given index. 
		/// The index of any children at or after the given index are increased by one
		/// </summary>
		/// <param name="node">The new child <see cref="ICoreNode"/> to insert,
		/// must be between 0 and the number of children as returned by member method 
		/// <see cref="ICoreNodeReadOnlyMethods.getChildCount"/></param>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when <paramref localName="node"/> is null</exception>
		/// <param name="insertIndex">The index at which to insert the new child</param>
		/// <exception cref="exception.MethodParameterIsOutOfBoundsException">
		/// Thrown when <paramref localName="insertIndex"/> is out if range, 
		/// that is not between 0 and <c><see cref="ICoreNodeReadOnlyMethods.getChildCount"/>()</c></exception>
		void insert(ICoreNode node, int insertIndex);

		/// <summary>
		/// Detaches the instance <see cref="ICoreNode"/> from it's parent's children
		/// </summary>
		/// <returns>The detached <see cref="ICoreNode"/> (i.e. <c>this</c>)</returns>
		ICoreNode detach();


		/// <summary>
		/// Removes the child at a given index. 
		/// </summary>
		/// <param name="index">The given index</param>
		/// <returns>The removed child</returns>
		/// <exception cref="exception.MethodParameterIsOutOfBoundsException">
		/// Thrown when <paramref localName="index"/> is out of bounds, 
		/// that is not the index of a child 
		/// (child indexes range from 0 to <c><see cref="ICoreNodeReadOnlyMethods.getChildCount"/>()-1</c>)
		/// </exception>
		ICoreNode removeChild(int index);

		/// <summary>
		/// Removes a given <see cref="ICoreNode"/> child. 
		/// </summary>
		/// <param name="node">The <see cref="ICoreNode"/> child to remove</param>
		/// <returns>The removed child</returns>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when parameter <paramref localName="node"/> is null</exception>
		/// <exception cref="exception.NodeDoesNotExistException">
		/// Thrown when <paramref localName="node"/> is not a child of the instance <see cref="ICoreNode"/></exception>
		ICoreNode removeChild(ICoreNode node);

		/// <summary>
		/// Inserts a new <see cref="ICoreNode"/> child before the given child.
		/// </summary>
		/// <param name="newChild">The new <see cref="ICoreNode"/> child node</param>
		/// <param name="anchorNode">The child before which to insert the new child</param>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when parameters <paramref localName="newChild"/> and/or <paramref localName="anchorNode"/> 
		/// have null values</exception>
		/// <exception cref="exception.NodeDoesNotExistException">
		/// Thrown when <paramref localName="anchorNode"/> is not a child of the instance <see cref="ICoreNode"/></exception>
		void insertBefore(ICoreNode newChild, ICoreNode anchorNode);

		/// <summary>
		/// Inserts a new <see cref="ICoreNode"/> child after the given child.
		/// </summary>
		/// <param name="newNode">The new <see cref="ICoreNode"/> child node</param>
		/// <param name="anchorNode">The child after which to insert the new child</param>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when parameters <paramref localName="newNode"/> and/or <paramref localName="anchorNode"/> 
		/// have null values</exception>
		/// <exception cref="exception.NodeDoesNotExistException">
		/// Thrown when <paramref localName="anchorNode"/> is not a child of the instance <see cref="ICoreNode"/></exception>
		void insertAfter(ICoreNode newNode, ICoreNode anchorNode);

		/// <summary>
		/// Replaces the child <see cref="ICoreNode"/> at a given index with a new <see cref="ICoreNode"/>
		/// </summary>
		/// <param name="node">The new <see cref="ICoreNode"/> with which to replace</param>
		/// <param name="index">The index of the child <see cref="ICoreNode"/> to replace</param>
		/// <returns>The replaced child <see cref="ICoreNode"/></returns>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when parameter <paranref localName="node"/> is null</exception>
		/// <exception cref="exception.MethodParameterIsOutOfBoundsException">
		/// Thrown when index is out if range, 
		/// that is when <paramref localName="index"/> is not between 0 
		/// and <c><see cref="ICoreNodeReadOnlyMethods.getChildCount"/>()-1</c>c></exception>
		ICoreNode replaceChild(ICoreNode node, int index);

		/// <summary>
		/// Replaces an existing child <see cref="ICoreNode"/> with i new one
		/// </summary>
		/// <param name="node">The new child with which to replace</param>
		/// <param name="oldNode">The existing child node to replace</param>
		/// <returns>The replaced <see cref="ICoreNode"/> child</returns>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when parameters <paramref localName="node"/> and/or <paramref localName="oldNode"/> 
		/// have null values</exception>
		/// <exception cref="exception.NodeDoesNotExistException">
		/// Thrown when <paramref localName="oldNode"/> is not a child of the instance <see cref="ICoreNode"/></exception>
		ICoreNode replaceChild(ICoreNode node, ICoreNode oldNode);

		/// <summary>
		/// Appends a child <see cref="ICoreNode"/> to the end of the list of children
		/// </summary>
		/// <param name="node">The new child to append</param>
		void appendChild(ICoreNode node);

		/// <summary>
		/// Appends the children of a given <see cref="ICoreNode"/> to <c>this</c>, 
		/// leaving the given <see cref="ICoreNode"/> without children
		/// </summary>
		/// <param name="node">The given <see cref="ICoreNode"/></param>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when parameter <paramref localName="node"/> is <c>null</c>
		/// </exception>
		/// <exception cref="exception.NodeInDifferentPresentationException">
		/// Thrown when parameter <paramref localName="node"/> belongs to a different <see cref="ICorePresentation"/>
		/// </exception>
		/// <exception cref="exception.NodeIsAncestorException">
		/// Thrown when parameter <paramref localName="node"/> is an ancestor of <c>this</c>
		/// </exception>
		/// <exception cref="exception.NodeIsDescendantException">
		/// Thrown when <paramref localName="node"/> is a descendant of <c>this</c>
		/// </exception>
		/// <exception cref="exception.NodeIsSelfException">
		/// Thrown when parameter <paramref localName="node"/> is identical to <c>this</c>
		/// </exception>
		void appendChildrenOf(ICoreNode node);

		/// <summary>
		/// Swaps <c>this</c> with a given <see cref="ICoreNode"/> 
		/// </summary>
		/// <param name="node">The given <see cref="ICoreNode"/></param>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when parameter <paramref localName="node"/> is <c>null</c>
		/// </exception>
		/// <exception cref="exception.NodeInDifferentPresentationException">
		/// Thrown when parameter <paramref localName="node"/> belongs to a different <see cref="ICorePresentation"/>
		/// </exception>
		/// <exception cref="exception.NodeIsAncestorException">
		/// Thrown when parameter <paramref localName="node"/> is an ancestor of <c>this</c>
		/// </exception>
		/// <exception cref="exception.NodeIsDescendantException">
		/// Thrown when <paramref localName="node"/> is a descendant of <c>this</c>
		/// </exception>
		/// <exception cref="exception.NodeIsSelfException">
		/// Thrown when parameter <paramref localName="node"/> is identical to <c>this</c>
		/// </exception>
		void swapWith(ICoreNode node);

		/// <summary>
		/// Splits <c>this</c> at the child at a given <paramref localName="index"/>, 
		/// producing a new <see cref="ICoreNode"/> with the children 
		/// at indexes <c><paramref localName="index"/></c> to <c>getChildCount()-1</c> 
		/// and leaving <c>this</c> with the children at indexes <c>0</c> to <paramref localName="index"/>-1
		/// </summary>
		/// <param name="index">The index of the child at which to split</param>
		/// <param name="copyProperties">
		/// A <see cref="bool"/> indicating the <see cref="Property"/>s of <c>this</c> 
		/// should be copied to the new <see cref="ICoreNode"/>
		/// </param>
		/// <returns>
		/// The new <see cref="ICoreNode"/> with the children 
		/// at indexes <c><paramref localName="index"/></c> to <c>getChildCount()-1</c> 
		/// and optionally with a copy of the <see cref="Property"/>s
		/// </returns>
		/// <exception cref="exception.MethodParameterIsOutOfBoundsException">
		/// Thrown when <paramref localName="index"/> is out of bounds, 
		/// that is not between <c>0</c> and <c>getChildCount()-1</c>
		/// </exception>
		ICoreNode splitChildren(int index, bool copyProperties);

		/// <summary>
		/// Swaps <c>this</c> with the previous sibling of <c>this</c>
		/// </summary>
		/// <returns>
		/// A <see cref="bool"/> indicating if the swap was succesfull 
		/// (the swap is not succesfull when there is no previous sibling).
		/// </returns>
		bool swapWithPreviousSibling();

		/// <summary>
		/// Swaps <c>this</c> with the next sibling of <c>this</c>
		/// </summary>
		/// <returns>
		/// A <see cref="bool"/> indicating if the swap was succesfull 
		/// (the swap is not succesfull when there is no next sibling).
		/// </returns>
		bool swapWithNextSibling();
	}
}