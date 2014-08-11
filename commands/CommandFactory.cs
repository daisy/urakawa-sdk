using urakawa.command;
using urakawa.core;
using urakawa.media;
using urakawa.media.data.audio;
using urakawa.property.alt;
using urakawa.xuk;
using urakawa.metadata;

namespace urakawa.commands
{
    [XukNameUglyPrettyAttribute("cmdFct", "CommandFactory")]
    public class CommandFactory : CommandFactoryBase
    {
        public CommandFactory(Presentation pres)
            : base(pres)
        {
        }

        public AlternateContentMetadataAddCommand CreateAlternateContentMetadataAddCommand(TreeNode treeNode, AlternateContentProperty altContentProperty, AlternateContent altContent, Metadata metadata, MetadataAttribute metadataAttribute)
        {
            AlternateContentMetadataAddCommand command = Create<AlternateContentMetadataAddCommand>();
            command.Init(treeNode, altContentProperty, altContent, metadata, metadataAttribute);
            return command;
        }
        public AlternateContentMetadataRemoveCommand CreateAlternateContentMetadataRemoveCommand(TreeNode treeNode, AlternateContentProperty altContentProperty, AlternateContent altContent, Metadata metadata, MetadataAttribute metadataAttribute)
        {
            AlternateContentMetadataRemoveCommand command = Create<AlternateContentMetadataRemoveCommand>();
            command.Init(treeNode, altContentProperty, altContent, metadata, metadataAttribute);
            return command;
        }
        //public AlternateContentMetadataSetIdCommand CreateAlternateContentMetadataSetIdCommand(AlternateContentProperty altContentProperty, AlternateContent altContent, Metadata metadata, bool id)
        //{
        //    AlternateContentMetadataSetIdCommand command = Create<AlternateContentMetadataSetIdCommand>();
        //    command.Init(altContentProperty, altContent, metadata, id);
        //    return command;
        //}
        public AlternateContentMetadataSetNameCommand CreateAlternateContentMetadataSetNameCommand(AlternateContentProperty altContentProperty, AlternateContent altContent, MetadataAttribute metadataAttribute, string name)
        {
            AlternateContentMetadataSetNameCommand command = Create<AlternateContentMetadataSetNameCommand>();
            command.Init(altContentProperty, altContent, metadataAttribute, name);
            return command;
        }

        public AlternateContentMetadataSetContentCommand CreateAlternateContentMetadataSetContentCommand(AlternateContentProperty altContentProperty, AlternateContent altContent, MetadataAttribute metadataAttribute, string content)
        {
            AlternateContentMetadataSetContentCommand command = Create<AlternateContentMetadataSetContentCommand>();
            command.Init(altContentProperty, altContent, metadataAttribute, content);
            return command;
        }


        public AlternateContentSetManagedMediaCommand CreateAlternateContentSetManagedMediaCommand(TreeNode treeNode, AlternateContent altContent, Media media)
        {
            AlternateContentSetManagedMediaCommand command = Create<AlternateContentSetManagedMediaCommand>();
            command.Init(treeNode, altContent, media);
            return command;
        }
        public AlternateContentRemoveManagedMediaCommand CreateAlternateContentRemoveManagedMediaCommand(TreeNode treeNode, AlternateContent altContent, Media media)
        {
            AlternateContentRemoveManagedMediaCommand command = Create<AlternateContentRemoveManagedMediaCommand>();
            command.Init(treeNode, altContent, media);
            return command;
        }

        public AlternateContentAddCommand CreateAlternateContentAddCommand(TreeNode treeNode, AlternateContent altContent)
        {
            AlternateContentAddCommand command = Create<AlternateContentAddCommand>();
            command.Init(treeNode, altContent);
            return command;
        }
        public AlternateContentRemoveCommand CreateAlternateContentRemoveCommand(TreeNode treeNode, AlternateContent altContent)
        {
            AlternateContentRemoveCommand command = Create<AlternateContentRemoveCommand>();
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

        public TreeNodeRemoveCommand CreateTreeNodeRemoveCommand(TreeNode treeNode)
        {
            TreeNodeRemoveCommand command = Create<TreeNodeRemoveCommand>();
            command.Init(treeNode);
            return command;
        }
        public TreeNodeInsertCommand CreateTreeNodeInsertCommand(TreeNode treeNode, TreeNode parent, int position)
        {
            TreeNodeInsertCommand command = Create<TreeNodeInsertCommand>();
            command.Init(treeNode, parent, position);
            return command;
        }
    }
}
