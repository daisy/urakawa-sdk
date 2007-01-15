using System;
using NUnit.Framework;
using urakawa.core;
using urakawa.unitTests.testbase;
using System.IO;

namespace urakawa.unitTests.fixtures.xukfiles

{
	/// <summary>
	/// Tests for opening and saving XUK files
	/// </summary>
	/// 
	[TestFixture] public class XUKOpen
	{
		protected string mDefaultFile = @"..\XUKWorks\simplesample.xuk";

		/// <summary>
		/// Tests opening of XUK files
		/// </summary>
		[Test] public void OpenXUK()
		{
			urakawa.project.Project proj;
			OpenXUK(out proj, mDefaultFile);
		}

		private void OpenXUK(out urakawa.project.Project proj, string file)
		{
			proj = new urakawa.project.Project();
			
			string filepath = Directory.GetCurrentDirectory();

			Uri fileUri = new Uri(filepath);
			
			fileUri = new Uri(fileUri, file);
			bool opened = proj.openXUK(fileUri);
			Assert.IsTrue(opened, "Failed to load XUK file {0}", mDefaultFile);
		}

		[Test] public void DeleteChannel()
		{
			urakawa.project.Project proj;
			OpenXUK(out proj, mDefaultFile);
			ChannelsManager chMgr = proj.getPresentation().getChannelsManager();
			IChannel ch = (IChannel)chMgr.getListOfChannels()[0];
			chMgr.removeChannel(ch);
			chMgr.addChannel(ch);
			urakawa.examples.CollectMediaFromChannelCoreNodeVisitor collVis
				= new urakawa.examples.CollectMediaFromChannelCoreNodeVisitor(ch);
			proj.getPresentation().getRootNode().acceptDepthFirst(collVis);
			Assert.AreEqual(
				0, collVis.CollectedMedia.Length, 
				"The channel unexpectedly contained media after being deleted and re-added");

		}


    		
	}
}
