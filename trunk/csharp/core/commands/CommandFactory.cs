using urakawa.command;
using urakawa.core;
using urakawa.media;
using urakawa.media.data.audio;
using urakawa.property.alt;
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

        public CommandFactory(Presentation pres)
            : base(pres)
        {
        }

        public AlternateContentMetadataAddCommand CreateAlternateContentMetadataAddCommand(AlternateContentProperty altContent, Metadata metadata)
        {
            AlternateContentMetadataAddCommand command = Create<AlternateContentMetadataAddCommand>();
            command.Init(altContent, metadata);
            return command;
        }
        public AlternateContentMetadataRemoveCommand CreateAlternateContentMetadataRemoveCommand(AlternateContentProperty altContent, Metadata metadata)
        {
            AlternateContentMetadataRemoveCommand command = Create<AlternateContentMetadataRemoveCommand>();
            command.Init(altContent, metadata);
            return command;
        }
        public AlternateContentMetadataSetContentCommand CreateAlternateContentMetadataSetContentCommand(AlternateContentProperty altContent, Metadata metadata, string content)
        {
            AlternateContentMetadataSetContentCommand command = Create<AlternateContentMetadataSetContentCommand>();
            command.Init(altContent, metadata, content);
            return command;
        }
        public AlternateContentMetadataSetIdCommand CreateAlternateContentMetadataSetIdCommand(AlternateContentProperty altContent, Metadata metadata, bool id)
        {
            AlternateContentMetadataSetIdCommand command = Create<AlternateContentMetadataSetIdCommand>();
            command.Init(altContent, metadata, id);
            return command;
        }
        public AlternateContentMetadataSetNameCommand CreateAlternateContentMetadataSetNameCommand(AlternateContentProperty altContent, Metadata metadata, string name)
        {
            AlternateContentMetadataSetNameCommand command = Create<AlternateContentMetadataSetNameCommand>();
            command.Init(altContent, metadata, name);
            return command;
        }
        public AlternateContentSetManagedMediaCommand CreateAlternateContentSetManagedMediaCommand(AlternateContent altContent, Media media)
        {
            AlternateContentSetManagedMediaCommand command = Create<AlternateContentSetManagedMediaCommand>();
            command.Init(altContent, media);
            return command;
        }
        public AlternateContentSetRoleCommand CreateAlternateContentSetRoleCommand(AlternateContent altContent, string role)
        {
            AlternateContentSetRoleCommand command = Create<AlternateContentSetRoleCommand>();
            command.Init(altContent, role);
            return command;
        }
        public TreeNodeAddAlternateContentCommand CreateTreeNodeAddAlternateContentCommand(TreeNode treeNode, AlternateContent altContent)
        {
            TreeNodeAddAlternateContentCommand command = Create<TreeNodeAddAlternateContentCommand>();
            command.Init(treeNode, altContent);
            return command;
        }
        public TreeNodeRemoveAlternateContentCommand CreateTreeNodeRemoveAlternateContentCommand(TreeNode treeNode, AlternateContent altContent)
        {
            TreeNodeRemoveAlternateContentCommand command = Create<TreeNodeRemoveAlternateContentCommand>();
            command.Init(treeNode, altContent);
            return command;
        }

        public TreeNodeSetManagedAudioMediaCommand CreateTreeNodeSetManagedAudioMediaCommand(TreeNode treeNode, ManagedAudioMedia managedMedia, TreeNode currentTreeNode)
        {
            TreeNodeSetManagedAudioMediaCommand command = Create<TreeNodeSetManagedAudioMediaCommand>();
            command.Init(treeNode, managedMedia, currentTreeNode);
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

        public TreeNodeChangeTextCommand CreateTreeNodeChangeTextCommand(TreeNode treeNode, string text)
        {
            TreeNodeChangeTextCommand command = Create<TreeNodeChangeTextCommand>();
            command.Init(treeNode, text);
            return command;
        }
    }
}
