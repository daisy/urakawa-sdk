using urakawa.command;
using urakawa.core;
using urakawa.media.data.audio;
using urakawa.media.timing;
using urakawa.xuk;
using urakawa.metadata;

namespace urakawa.commands
{
    public class CommandFactory : CommandFactoryBase
    {
        public override string GetTypeNameFormatted()
        {
            return XukStrings.CommandFactory;
        }
        public CommandFactory(Presentation pres) : base(pres)
        {
        }

        public TreeNodeSetManagedAudioMediaCommand CreateTreeNodeSetManagedAudioMediaCommand(
                                    TreeNode treeNode, ManagedAudioMedia managedMedia)
        {
            TreeNodeSetManagedAudioMediaCommand command = Create<TreeNodeSetManagedAudioMediaCommand>();
            command.Init(treeNode, managedMedia);
            return command;
        }

        public ManagedAudioMediaInsertDataCommand CreateManagedAudioMediaInsertDataCommand(TreeNode treeNode, ManagedAudioMedia managedAudioMediaSource, long bytePositionInsert, TreeNode currentTreeNode)
        {
            ManagedAudioMediaInsertDataCommand command = Create<ManagedAudioMediaInsertDataCommand>();
            command.Init(treeNode, managedAudioMediaSource, bytePositionInsert, currentTreeNode);
            return command;
        }

        public TreeNodeAudioStreamDeleteCommand CreateTreeNodeAudioStreamDeleteCommand(TreeNodeAndStreamSelection selection, TreeNode currentTreeNode)
        {
            TreeNodeAudioStreamDeleteCommand command = Create<TreeNodeAudioStreamDeleteCommand>();
            command.Init(selection, currentTreeNode);
            return command;
        }

        public MetadataAddCommand CreateMetadataAddCommand(Metadata metadata)
        {
            MetadataAddCommand command = Create<MetadataAddCommand>();
            command.Init(metadata);
            return command;
        }

        public MetadataRemoveCommand CreateMetadataRemoveCommand(Metadata metadata)
        {
            MetadataRemoveCommand command = Create<MetadataRemoveCommand>();
            command.Init(metadata);
            return command;
        }

        public MetadataSetContentCommand CreateMetadataSetContentCommand(Metadata metadata, string content)
        {
            MetadataSetContentCommand command = Create<MetadataSetContentCommand>();
            command.Init(metadata, content);
            return command;
        }

        public MetadataSetNameCommand CreateMetadataSetNameCommand(Metadata metadata, string content)
        {
            MetadataSetNameCommand command = Create<MetadataSetNameCommand>();
            command.Init(metadata, content);
            return command;
        }

        public MetadataSetIdCommand CreateMetadataSetIdCommand(Metadata metadata, bool id)
        {
            MetadataSetIdCommand command = Create<MetadataSetIdCommand>();
            command.Init(metadata, id);
            return command;
        }

        public TreeNodeSetIsMarkedCommand CreateTreeNodeSetIsMarkedCommand(TreeNode treeNode, bool isMarked)
        {
            TreeNodeSetIsMarkedCommand command = Create<TreeNodeSetIsMarkedCommand>();
            command.Init(treeNode, isMarked);
            return command;
        }
    }
}
