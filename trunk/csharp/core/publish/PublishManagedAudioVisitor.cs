using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using urakawa.core;
using urakawa.core.visitor;
using urakawa.property.channel;
using urakawa.media;
using urakawa.media.timing;
using urakawa.media.data.audio;

namespace urakawa.publish
{
    /// <summary>
    /// Concrete implementation of the <see cref="AbstractPublishManagedAudioVisitor"/>
    /// where <see cref="TreeNodeTestDelegate"/>s are used to determine
    /// </summary>
    public class PublishManagedAudioVisitor : AbstractPublishManagedAudioVisitor
    {
        /// <summary>
        /// Constructor setting the <see cref="TreeNodeTestDelegate"/> determining when to create new audio files
        /// and when to 
        /// </summary>
        /// <param name="treeNodeTriggersNewAudioFileDelegate">
        /// The <see cref="TreeNodeTestDelegate"/> determining which <see cref="TreeNode"/>s trigger new audio files,
        /// if <c>null</c> then no <see cref="TreeNode"/>s trigger new audio files
        /// </param>
        /// <param name="treeNodeMustBeSkippedDelegate">
        /// The <see cref="TreeNodeTestDelegate"/> determining which <see cref="TreeNode"/>s should be skipped,
        /// if <c>null</c> then no <see cref="TreeNode"/>s are skipped
        /// </param>
        public PublishManagedAudioVisitor(
            TreeNodeTestDelegate treeNodeTriggersNewAudioFileDelegate,
            TreeNodeTestDelegate treeNodeMustBeSkippedDelegate)
        {
            mTreeNodeTriggersNewAudioFileDelegate = treeNodeTriggersNewAudioFileDelegate;
            mTreeNodeMustBeSkippedDelegate = treeNodeMustBeSkippedDelegate;
        }

        private TreeNodeTestDelegate mTreeNodeTriggersNewAudioFileDelegate;
        private TreeNodeTestDelegate mTreeNodeMustBeSkippedDelegate;

        /// <summary>
        /// Determines if the given <see cref="TreeNode"/> should trigger a new audio file,
        /// based on the <see cref="TreeNodeTestDelegate"/> passed with the constructor.
        /// If the delegate passed with the constructor was <c>null</c>, the method returns <c>false</c>
        /// </summary>
        /// <param name="node">The given node</param>
        /// <returns>A <see cref="bool"/> indicating if the given node triggers a new audio file</returns>
        public override bool TreeNodeTriggersNewAudioFile(TreeNode node)
        {
            if (mTreeNodeTriggersNewAudioFileDelegate != null) return mTreeNodeTriggersNewAudioFileDelegate(node);
            return false;
        }

        /// <summary>
        /// Determines if the given <see cref="TreeNode"/> should be skipped during publish visitation,
        /// based on the <see cref="TreeNodeTestDelegate"/> passed with the constructor.
        /// If the delegate passed with the constructor was <c>null</c>, the method returns <c>false</c>
        /// </summary>
        /// <param name="node">The given node</param>
        /// <returns>A <see cref="bool"/> indicating if the <see cref="TreeNode"/> should be skipped</returns>
        public override bool TreeNodeMustBeSkipped(TreeNode node)
        {
            if (mTreeNodeMustBeSkippedDelegate != null) return mTreeNodeMustBeSkippedDelegate(node);
            return false;
        }
    }

    /// <summary>
    /// Delegate to perform a boolean test on a <see cref="TreeNode"/>
    /// </summary>
    /// <param name="node">The <see cref="TreeNode"/> on which to perform the test</param>
    /// <returns>The boolean result of the test</returns>
    public delegate bool TreeNodeTestDelegate(TreeNode node);
}