using System;
using urakawa.core;

namespace urakawa.daisy.export.visitor
{
    public class PublishFlattenedManagedAudioVisitor : AbstractPublishFlattenedManagedAudioVisitor
    {
        public PublishFlattenedManagedAudioVisitor(
            TreeNodeTestDelegate treeNodeTriggersNewAudioFileDelegate,
            TreeNodeTestDelegate treeNodeMustBeSkippedDelegate)
        {
            mTreeNodeTriggersNewAudioFileDelegate = treeNodeTriggersNewAudioFileDelegate;
            mTreeNodeMustBeSkippedDelegate = treeNodeMustBeSkippedDelegate;
        }

        private TreeNodeTestDelegate mTreeNodeTriggersNewAudioFileDelegate;
        private TreeNodeTestDelegate mTreeNodeMustBeSkippedDelegate;

        public override bool TreeNodeTriggersNewAudioFile(TreeNode node)
        {
            if (mTreeNodeTriggersNewAudioFileDelegate != null)
            {
                return mTreeNodeTriggersNewAudioFileDelegate(node);
            }
            return false;
        }

        public override bool TreeNodeMustBeSkipped(TreeNode node)
        {
            if (mTreeNodeMustBeSkippedDelegate != null)
            {
                return mTreeNodeMustBeSkippedDelegate(node);
            }
            return false;
        }

        public override void DoWork()
        {
            throw new NotImplementedException();
        }
    }
}