using System;
using NUnit.Framework;
using urakawa.core;
using System.Collections;


namespace urakawa.unitTests
{

    //Added by Marisa
    //20060811
    [TestFixture]
    public class CopyInsertTest : testbase.TestCollectionBase
    {
        private string mDefaultFile = "../XukWorks/copyInsertRenameTest.xuk";
      
        [SetUp]
        public void Init()
        {
            mProject = new urakawa.project.Project();

            string filepath = System.IO.Directory.GetCurrentDirectory();

            Uri fileUri = new Uri(filepath);

            fileUri = new Uri(fileUri, mDefaultFile);

            bool openSucces = mProject.openXUK(fileUri);
            Assert.IsTrue(openSucces, String.Format("Could not open xuk file {0}", mDefaultFile));
        }

        [Test]
        public void CopyInsertRenameAndSeeIfOnlyTheCopyWasRenamed()
        {
            //get the first child of the root node and paste it under the second
            //child of the root node

            CoreNode node_a = mProject.getPresentation().getRootNode().getChild(0);

            CoreNode node_a_copy = node_a.copy(true);

            CoreNode node_b = mProject.getPresentation().getRootNode().getChild(1);

            node_b.insert(node_a_copy, 0);

            GetTextMedia(node_a_copy).setText("a-pasted");

            string renamed_label_of_pasted_node = GetTextMedia(node_a_copy).getText();
            string label_of_source_node = GetTextMedia(node_a).getText();

            Assert.AreNotEqual(renamed_label_of_pasted_node, label_of_source_node);


        }

        //copied from Obi.Project
        //in this case, it will work because the file used for this test was created by Obi
        public urakawa.media.TextMedia GetTextMedia(CoreNode node)
        {
            ChannelsProperty channelsProp = (ChannelsProperty)node.getProperty(typeof(ChannelsProperty));
            Channel textChannel;
            IList channelsList = channelsProp.getListOfUsedChannels();
            for (int i = 0; i < channelsList.Count; i++)
            {
                string channelName = ((IChannel)channelsList[i]).getName();
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
