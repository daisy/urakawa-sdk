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
		private urakawa.xuk.Project mProject;
		private string mDefaultFile = "../XukWorks/simplesample.xuk";

		[SetUp] public void Init() 
		{
			mProject = new urakawa.xuk.Project();
			
			string filepath = System.IO.Directory.GetCurrentDirectory();

			Uri fileUri = new Uri(filepath);
			
			fileUri = new Uri(fileUri, mDefaultFile);
			
			mProject.openXUK(fileUri);
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
				CoreNode node_copy = root.copy(true);

				Assert.AreEqual(root, node_copy);
			}
		}
		[Test] public void InsertNewNodeAsSiblingOfRoot()
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
			
			bool bGotException = false;

			try
			{
				root.insertAfter(new_node, root);
			}
			catch (urakawa.exception.CheckedException)
			{
				bGotException = true;
			}

			if (bGotException == false)
			{
				Assert.Fail("Got no exception where one was expected");
			}
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
    [Test] public void RemoveChannel()
    {
      urakawa.core.IChannel c1Channel = mProject.getPresentation().getChannelsManager().getChannelById("c1");
      DetectMediaCoreNodeVisitor detVis = new DetectMediaCoreNodeVisitor(c1Channel);
      mProject.getPresentation().getRootNode().acceptDepthFirst(detVis);
      Assert.IsTrue(
        detVis.hasFoundMedia(),
        "The channel with id \"c1\" must contain media or the test wiull be meaningless");
      mProject.getPresentation().getChannelsManager().removeChannel(c1Channel);
      mProject.getPresentation().getChannelsManager().addChannel(c1Channel);
      detVis.reset();
      mProject.getPresentation().getRootNode().acceptDepthFirst(detVis);
      Assert.IsFalse(
        detVis.hasFoundMedia(), 
        "Found media in channel that was removed and re-added");
    }
  }
}
