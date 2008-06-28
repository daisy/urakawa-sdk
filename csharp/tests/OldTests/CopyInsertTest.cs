using System;
using System.Collections.Generic;
using NUnit.Framework;
using urakawa.core;
using urakawa.property.channel;


namespace urakawa.unitTests
{

	//Added by Marisa
	//20060811
	[TestFixture]
	public class CopyInsertTest : testbase.TestCollectionBase
	{
		[TestFixtureSetUp]
		public void InitFixture()
		{
			mDefaultFile = "../../XukWorks/copyInsertRenameTest.xuk";
		}

		[Test]
		public void CopyInsertRenameAndSeeIfOnlyTheCopyWasRenamed()
		{
			//get the first child of the root node and paste it under the second
			//child of the root node

			TreeNode node_a = mProject.GetPresentation(0).RootNode.getChild(0);
			TreeNode node_a_copy = node_a.copy(true);
			TreeNode node_b = mProject.GetPresentation(0).RootNode.getChild(1);

			node_b.insert(node_a_copy, 0);

			GetTextMedia(node_a_copy).setText("a-pasted");

			string renamed_label_of_pasted_node = GetTextMedia(node_a_copy).getText();
			string label_of_source_node = GetTextMedia(node_a).getText();

			Assert.AreNotEqual(renamed_label_of_pasted_node, label_of_source_node);


		}

		//copied from Obi.Project
		//in this case, it will work because the file used for this test was created by Obi
		public urakawa.media.TextMedia GetTextMedia(TreeNode node)
		{
			ChannelsProperty channelsProp = (ChannelsProperty)node.getProperty(typeof(ChannelsProperty));
			Channel textChannel;
			IList<Channel> channelsList = channelsProp.getListOfUsedChannels();
			for (int i = 0; i < channelsList.Count; i++)
			{
				string channelName = ((Channel)channelsList[i]).getName();
				if (channelName == "obi.text")//Project.TextChannel)
				{
					textChannel = (Channel)channelsList[i];
					return (urakawa.media.TextMedia)channelsProp.getMedia(textChannel);
				}
			}
			return null;
		}

	}

}
