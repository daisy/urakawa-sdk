using System;
using NUnit.Framework;
using urakawa.core;

namespace urakawa.test.unitTests
{
	/// <summary>
	/// this is a class library to be used with NUnit unit testing software
	/// </summary>
	[TestFixture] 
	public class AfterOpeningXUK
	{
		private urakawa.project.Project mProject;
		private string mDefaultFile = "../XukWorks/simplesample.xuk";

		[SetUp] public void Init() 
		{
			mProject = new urakawa.project.Project();
			
			string filepath = System.IO.Directory.GetCurrentDirectory();

			Uri fileUri = new Uri(filepath);
			
			fileUri = new Uri(fileUri, mDefaultFile);
			
			bool openSucces = mProject.openXUK(fileUri);
      Assert.IsTrue(openSucces, String.Format("Could not open xuk file {0}", mDefaultFile));
		}

		[Test] public void setPropertyAndCheckForNewValue()
		{
			CoreNode root = mProject.getPresentation().getRootNode();
			System.Collections.ArrayList channels = (System.Collections.ArrayList)
				mProject.getPresentation().getChannelsManager().getListOfChannels();

			int i = 0;
			for (i = 0; i<channels.Count; i++)
			{
				if (((Channel)channels[i]).isMediaTypeSupported(urakawa.media.MediaType.TEXT) == true)
				{
					break;
				}
			}

			if (i < channels.Count)
			{
				ChannelsProperty text_cp = mProject.getPresentation().getPropertyFactory().createChannelsProperty();
				urakawa.media.TextMedia txt = (urakawa.media.TextMedia)mProject.getPresentation().getMediaFactory().createMedia(urakawa.media.MediaType.TEXT);
				txt.setText("hello I am the new text for the root node");
				text_cp.setMedia((Channel)channels[i], txt);

				root.setProperty(text_cp);

				ChannelsProperty root_cp = 
					(ChannelsProperty)mProject.getPresentation().getRootNode().getProperty(PropertyType.CHANNEL);
				urakawa.media.TextMedia txt2 = 
					(urakawa.media.TextMedia)root_cp.getMedia((Channel)channels[i]);

				Assert.AreEqual(txt.getText(), txt2.getText());
				Assert.AreEqual(txt, txt2);

			}
		}
		[Test] public void IsPresentationNull()
		{
			Assert.IsNotNull(mProject.getPresentation());
		}

		[Test] public void IsRootNodeNull()
		{
			if (mProject.getPresentation() != null)
			{
				Assert.IsNotNull(mProject.getPresentation().getRootNode());
			}
		}
		[Test] public void IsChannelsManagerNull()
		{
			if (mProject.getPresentation() != null)
			{
				Assert.IsNotNull(mProject.getPresentation().getChannelsManager());
			}
		}


		[Test] public void CopyNode()
		{
			if (mProject.getPresentation() != null)
			{
				CoreNode root = mProject.getPresentation().getRootNode();
        Console.WriteLine("Testing deep copying:");
				CoreNode node_copy = root.copy(true);
				Assert.IsTrue(CoreNode.areCoreNodesEqual(root, node_copy, true), "The copy is just a reference of the CoreNode itself");

			}
		}

		[Test] 
		[ExpectedException(typeof(exception.NodeDoesNotExistException))]
		public void InsertNewNodeAsSiblingOfRoot()
		{
			if (mProject.getPresentation() == null)
			{
				return;
			}
			CoreNode new_node = mProject.getPresentation().getCoreNodeFactory().createNode();
			CoreNode root = mProject.getPresentation().getRootNode();
			if (root == null)
			{
				return;
			}

			root.insertAfter(new_node, root);
		}

		[Test] public void AddChildToRoot()
		{
			if (mProject.getPresentation() == null)
			{
				return;
			}
			CoreNode new_node = mProject.getPresentation().getCoreNodeFactory().createNode();
			CoreNode root = mProject.getPresentation().getRootNode();
			if (root == null)
			{
				return;
			}
			
			root.appendChild(new_node);

			Assert.AreEqual(root.getChildCount(), 1, "root has wrong number of children");
			Assert.AreEqual(root.getChild(0), new_node, "root has wrong child");

		}
    /// <summary>
    /// Checks that the removal of a channels disassociates the media in this channel
    /// from the core nodes.
    /// </summary>
    /// <remarks>Assumes that the file loaded into the presentation has a channel 
    /// with id c1 and that at least on piece of media is attached to that channel</remarks>
    public void RemoveChannel()
    {
      urakawa.core.IChannel c1Channel = mProject.getPresentation().getChannelsManager().getChannelById("c1");
      DetectMediaCoreNodeVisitor detVis = new DetectMediaCoreNodeVisitor(c1Channel);
      mProject.getPresentation().getRootNode().acceptDepthFirst(detVis);
      Assert.IsTrue(
        detVis.hasFoundMedia(),
        "The channel with id \"c1\" must contain media or the test will be meaningless");
      mProject.getPresentation().getChannelsManager().removeChannel(c1Channel);
      mProject.getPresentation().getChannelsManager().addChannel(c1Channel);
      detVis.reset();
      mProject.getPresentation().getRootNode().acceptDepthFirst(detVis);
      Assert.IsFalse(
        detVis.hasFoundMedia(), 
        "Found media in channel that was removed and re-added");
    }

    /// <summary>
    /// Tests the <see cref="ChannelsManager.getChannelByName"/> method
    /// </summary>
    [Test] public void GetChannelByName()
    {
      IChannel[] retrivedChs = mProject.getPresentation().getChannelsManager().getChannelByName("EnglishVoice");
    }
  }
}
