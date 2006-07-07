using System;
using NUnit.Framework;
using urakawa.core;

namespace urakawa.test.unitTests
{
	/// <summary>
	/// Summary description for ChannelTests.
	/// </summary>
	[TestFixture] 
	public class ChannelTests
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
