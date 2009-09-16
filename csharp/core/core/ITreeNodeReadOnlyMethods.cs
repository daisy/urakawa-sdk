using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using urakawa.media;
using urakawa.media.data.audio;
using urakawa.property;
using urakawa.xuk;

namespace urakawa.core
{
    /// <summary>
    /// Provides the read-only tree methods of a <see cref="TreeNode"/>
    /// </summary>
    public interface ITreeNodeReadOnlyMethods
    {
        AbstractTextMedia GetTextMedia();
        SequenceMedia GetTextSequenceMedia();
        AbstractImageMedia GetImageMedia();
        SequenceMedia GetImageSequenceMedia();
        Media GetMediaInTextChannel();
        Media GetMediaInImageChannel();
        ManagedAudioMedia GetManagedAudioMedia();
        AbstractAudioMedia GetAudioMedia();
        SequenceMedia GetManagedAudioSequenceMedia();
        SequenceMedia GetAudioSequenceMedia();
        Media GetMediaInAudioChannel();
        bool IsAfter(TreeNode node);
        bool IsBefore(TreeNode node);
        TreeNode MeetFirst(TreeNode node1, TreeNode node2);
        TreeNode Root { get; }
        string GetTextMediaFlattened();
        StreamWithMarkers? OpenPcmInputStreamOfManagedAudioMediaFlattened();
        Media GetManagedAudioMediaOrSequenceMedia();
        StreamWithMarkers? OpenPcmInputStreamOfManagedAudioMedia();
        TreeNode GetNextSiblingWithManagedAudio();

        TreeNode GetFirstDescendantWithText();
        TreeNode GetLastDescendantWithText();

        TreeNode GetPreviousSiblingWithText();
        TreeNode GetPreviousSiblingWithText(TreeNode upLimit);

        TreeNode GetNextSiblingWithText();
        TreeNode GetNextSiblingWithText(TreeNode upLimit);
        
        TreeNode GetFirstDescendantWithManagedAudio();
        TreeNode GetFirstAncestorWithManagedAudio();

        TreeNode GetFirstChildWithXmlElementName(string elemName);

        QualifiedName GetXmlElementQName();
        string GetXmlElementId();

        ObjectListProvider<Property> Properties { get;}
        ObjectListProvider<TreeNode> Children { get; }
        List<Type> UsedPropertyTypes { get; }
        List<Property> GetProperties(Type t);
        List<T> GetProperties<T>() where T : Property;
        Property GetProperty(Type t);
        T GetProperty<T>() where T : Property;
        bool HasProperties();
        bool HasProperties(Type t);
        bool HasProperty(Property prop);

        /// <summary>
        /// Gets the parent <see cref="TreeNode"/> of the instance,
        /// null if the instance is detached from a tree or is the root node of a tree
        /// </summary>
        /// <returns>The parent</returns>
        TreeNode Parent { get; }

        /// <summary>
        /// Make a copy of the node
        /// </summary>
        /// <param name="deep">If true, then include the node's entire subtree.  
        /// Otherwise, just copy the node itself.</param>
        /// <param name="copyProperties">If true, then include the node's property.</param>
        /// <returns>A <see cref="TreeNode"/> containing the copied data.</returns>
        TreeNode Copy(bool deep, bool copyProperties);

        /// <summary>
        /// Make a copy of the node
        /// </summary>
        /// <param name="deep">If true, then include the node's entire subtree.  
        /// Otherwise, just copy the node itself.</param>
        /// <returns>A <see cref="TreeNode"/> containing the copied data.</returns>
        TreeNode Copy(bool deep);

        /// <summary>
        /// Make a deep copy of the node including property
        /// </summary>
        /// <returns>A <see cref="TreeNode"/> containing the copied data.</returns>
        TreeNode Copy();

        TreeNode Export(Presentation destPres);

        /// <summary>
        /// Gets the next sibling of <c>this</c>
        /// </summary>
        /// <returns>The next sibling of <c>this</c> or <c>null</c> if no next sibling exists</returns>
        TreeNode NextSibling { get; }

        /// <summary>
        /// Gets the previous sibling of <c>this</c>
        /// </summary>
        /// <returns>The previous sibling of <c>this</c> or <c>null</c> if no previous sibling exists</returns>
        TreeNode PreviousSibling { get; }

        /// <summary>
        /// Tests if a given <see cref="TreeNode"/> is a sibling of <c>this</c>
        /// </summary>
        /// <param name="node">The given <see cref="TreeNode"/></param>
        /// <returns><c>true</c> if <paramref localName="node"/> is a sibling of <c>this</c>, 
        /// otherwise<c>false</c></returns>
        bool IsSiblingOf(TreeNode node);

        /// <summary>
        /// Tests if a given <see cref="TreeNode"/> is an ancestor of <c>this</c>
        /// </summary>
        /// <param name="node">The given <see cref="TreeNode"/></param>
        /// <returns><c>true</c> if <paramref localName="node"/> is an ancestor of <c>this</c>, 
        /// otherwise<c>false</c></returns>
        bool IsAncestorOf(TreeNode node);

        /// <summary>
        /// Tests if a given <see cref="TreeNode"/> is a descendant of <c>this</c>
        /// </summary>
        /// <param name="node">The given <see cref="TreeNode"/></param>
        /// <returns><c>true</c> if <paramref localName="node"/> is a descendant of <c>this</c>, 
        /// otherwise<c>false</c></returns>
        bool IsDescendantOf(TreeNode node);
    }
}