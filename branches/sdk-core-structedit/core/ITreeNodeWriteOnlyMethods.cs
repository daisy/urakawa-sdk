using System;
using System.Collections.Generic;
using System.Text;
using urakawa.property;

namespace urakawa.core
{
    /// <summary>
    /// Provides the write-only tree methods of a <see cref="TreeNode"/>
    /// </summary>
    public interface ITreeNodeWriteOnlyMethods
    {
        /// <summary>
        /// Inserts a <see cref="TreeNode"/> child at a given index. 
        /// The index of any children at or after the given index are increased by one
        /// </summary>
        /// <param name="node">The new child <see cref="TreeNode"/> to insert,
        /// must be between 0 and the number of children as returned by member method 
        /// <see cref="ITreeNodeReadOnlyMethods.Children.Count"/></param>
        /// <exception cref="exception.MethodParameterIsNullException">
        /// Thrown when <paramref localName="node"/> is null</exception>
        /// <param name="insertIndex">The index at which to insert the new child</param>
        /// <exception cref="exception.MethodParameterIsOutOfBoundsException">
        /// Thrown when <paramref localName="insertIndex"/> is out if range, 
        /// that is not between <c>0</c> and <c>ChildCount</c></exception>
        void Insert(TreeNode node, int insertIndex);

        /// <summary>
        /// Detaches the instance <see cref="TreeNode"/> from it's parent's children
        /// </summary>
        /// <returns>The detached <see cref="TreeNode"/> (i.e. <c>this</c>)</returns>
        TreeNode Detach();

        T GetOrCreateProperty<T>() where T : Property, new();
        void AddProperties(IList<Property> props);
        void AddProperty(Property prop);
        List<Property> RemoveProperties(Type propType);
        void RemoveProperty(Property prop);


        /// <summary>
        /// Removes the child at a given index. 
        /// </summary>
        /// <param name="index">The given index</param>
        /// <returns>The removed child</returns>
        /// <exception cref="exception.MethodParameterIsOutOfBoundsException">
        /// Thrown when <paramref localName="index"/> is out of bounds, 
        /// that is not the index of a child 
        /// (child indexes range from 0 to <c>ChildCount-1</c>)
        /// </exception>
        TreeNode RemoveChild(int index);

        /// <summary>
        /// Removes a given <see cref="TreeNode"/> child. 
        /// </summary>
        /// <param name="node">The <see cref="TreeNode"/> child to remove</param>
        /// <returns>The removed child</returns>
        /// <exception cref="exception.MethodParameterIsNullException">
        /// Thrown when parameter <paramref localName="node"/> is null</exception>
        /// <exception cref="exception.NodeDoesNotExistException">
        /// Thrown when <paramref localName="node"/> is not a child of the instance <see cref="TreeNode"/></exception>
        TreeNode RemoveChild(TreeNode node);

        /// <summary>
        /// Inserts a new <see cref="TreeNode"/> child before the given child.
        /// </summary>
        /// <param name="newChild">The new <see cref="TreeNode"/> child node</param>
        /// <param name="anchorNode">The child before which to insert the new child</param>
        /// <exception cref="exception.MethodParameterIsNullException">
        /// Thrown when parameters <paramref localName="newChild"/> and/or <paramref localName="anchorNode"/> 
        /// have null values</exception>
        /// <exception cref="exception.NodeDoesNotExistException">
        /// Thrown when <paramref localName="anchorNode"/> is not a child of the instance <see cref="TreeNode"/></exception>
        void InsertBefore(TreeNode newChild, TreeNode anchorNode);

        /// <summary>
        /// Inserts a new <see cref="TreeNode"/> child after the given child.
        /// </summary>
        /// <param name="newNode">The new <see cref="TreeNode"/> child node</param>
        /// <param name="anchorNode">The child after which to insert the new child</param>
        /// <exception cref="exception.MethodParameterIsNullException">
        /// Thrown when parameters <paramref localName="newNode"/> and/or <paramref localName="anchorNode"/> 
        /// have null values</exception>
        /// <exception cref="exception.NodeDoesNotExistException">
        /// Thrown when <paramref localName="anchorNode"/> is not a child of the instance <see cref="TreeNode"/></exception>
        void InsertAfter(TreeNode newNode, TreeNode anchorNode);

        /// <summary>
        /// Replaces the child <see cref="TreeNode"/> at a given index with a new <see cref="TreeNode"/>
        /// </summary>
        /// <param name="node">The new <see cref="TreeNode"/> with which to replace</param>
        /// <param name="index">The index of the child <see cref="TreeNode"/> to replace</param>
        /// <returns>The replaced child <see cref="TreeNode"/></returns>
        /// <exception cref="exception.MethodParameterIsNullException">
        /// Thrown when parameter <paranref localName="node"/> is null</exception>
        /// <exception cref="exception.MethodParameterIsOutOfBoundsException">
        /// Thrown when index is out if range, 
        /// that is when <paramref localName="index"/> is not between <c>0</c> and <c>ChildCount-1</c></exception>
        TreeNode ReplaceChild(TreeNode node, int index);

        /// <summary>
        /// Replaces an existing child <see cref="TreeNode"/> with i new one
        /// </summary>
        /// <param name="node">The new child with which to replace</param>
        /// <param name="oldNode">The existing child node to replace</param>
        /// <returns>The replaced <see cref="TreeNode"/> child</returns>
        /// <exception cref="exception.MethodParameterIsNullException">
        /// Thrown when parameters <paramref localName="node"/> and/or <paramref localName="oldNode"/> 
        /// have null values</exception>
        /// <exception cref="exception.NodeDoesNotExistException">
        /// Thrown when <paramref localName="oldNode"/> is not a child of the instance <see cref="TreeNode"/></exception>
        TreeNode ReplaceChild(TreeNode node, TreeNode oldNode);

        /// <summary>
        /// Appends a child <see cref="TreeNode"/> to the end of the list of children
        /// </summary>
        /// <param name="node">The new child to append</param>
        void AppendChild(TreeNode node);

        /// <summary>
        /// Appends the children of a given <see cref="TreeNode"/> to <c>this</c>, 
        /// leaving the given <see cref="TreeNode"/> without children
        /// </summary>
        /// <param name="node">The given <see cref="TreeNode"/></param>
        /// <exception cref="exception.MethodParameterIsNullException">
        /// Thrown when parameter <paramref localName="node"/> is <c>null</c>
        /// </exception>
        /// <exception cref="exception.NodeInDifferentPresentationException">
        /// Thrown when parameter <paramref localName="node"/> belongs to a different <see cref="Presentation"/>
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
        void AppendChildrenOf(TreeNode node);

        /// <summary>
        /// Swaps <c>this</c> with a given <see cref="TreeNode"/> 
        /// </summary>
        /// <param name="node">The given <see cref="TreeNode"/></param>
        /// <exception cref="exception.MethodParameterIsNullException">
        /// Thrown when parameter <paramref localName="node"/> is <c>null</c>
        /// </exception>
        /// <exception cref="exception.NodeInDifferentPresentationException">
        /// Thrown when parameter <paramref localName="node"/> belongs to a different <see cref="Presentation"/>
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
        void SwapWith(TreeNode node);

        /// <summary>
        /// Splits <c>this</c> at the child at a given <paramref localName="index"/>, 
        /// producing a new <see cref="TreeNode"/> with the children 
        /// at indexes <c><paramref localName="index"/></c> to <c>GetChildCount()-1</c> 
        /// and leaving <c>this</c> with the children at indexes <c>0</c> to <paramref localName="index"/>-1
        /// </summary>
        /// <param name="index">The index of the child at which to split</param>
        /// <param name="copyProperties">
        /// A <see cref="bool"/> indicating the <see cref="Property"/>s of <c>this</c> 
        /// should be copied to the new <see cref="TreeNode"/>
        /// </param>
        /// <returns>
        /// The new <see cref="TreeNode"/> with the children 
        /// at indexes <c><paramref localName="index"/></c> to <c>GetChildCount()-1</c> 
        /// and optionally with a copy of the <see cref="Property"/>s
        /// </returns>
        /// <exception cref="exception.MethodParameterIsOutOfBoundsException">
        /// Thrown when <paramref localName="index"/> is out of bounds, 
        /// that is not between <c>0</c> and <c>GetChildCount()-1</c>
        /// </exception>
        TreeNode SplitChildren(int index, bool copyProperties);

        /// <summary>
        /// Swaps <c>this</c> with the previous sibling of <c>this</c>
        /// </summary>
        /// <returns>
        /// A <see cref="bool"/> indicating if the swap was succesfull 
        /// (the swap is not succesfull when there is no previous sibling).
        /// </returns>
        bool SwapWithPreviousSibling();

        /// <summary>
        /// Swaps <c>this</c> with the next sibling of <c>this</c>
        /// </summary>
        /// <returns>
        /// A <see cref="bool"/> indicating if the swap was succesfull 
        /// (the swap is not succesfull when there is no next sibling).
        /// </returns>
        bool SwapWithNextSibling();
    }
}