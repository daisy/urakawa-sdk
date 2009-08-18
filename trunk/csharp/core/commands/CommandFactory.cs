using System;
using System.Collections.Generic;
using urakawa.command;
using urakawa.core;
using urakawa.media.data.audio;
using urakawa.media.timing;
using urakawa.xuk;

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

        public ManagedAudioMediaInsertDataCommand CreateManagedAudioMediaInsertDataCommand(TreeNode treeNode, ManagedAudioMedia managedAudioMediaTarget, ManagedAudioMedia managedAudioMediaSource, Time timeInsert)
        {
            ManagedAudioMediaInsertDataCommand command = Create<ManagedAudioMediaInsertDataCommand>();
            command.Init(treeNode, managedAudioMediaTarget, managedAudioMediaSource, timeInsert);
            return command;
        }

        public TreeNodeAudioStreamDeleteCommand CreateTreeNodeAudioStreamDeleteCommand(TreeNodeAndStreamSelection selection)
        {
            TreeNodeAudioStreamDeleteCommand command = Create<TreeNodeAudioStreamDeleteCommand>();
            command.Init(selection);
            return command;
        }
    }
}
