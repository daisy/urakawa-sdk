using System;
using NUnit.Framework;
using urakawa.core;
using urakawa.unitTests.testbase;

namespace urakawa.unitTests.fixtures.xukfiles.simplesample
{
	/// <summary>
	/// Summary description for SimpleSample.
	/// </summary>
	[TestFixture]
	public class SimpleSampleTreeTests : TreeTests
	{
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
	}

	[TestFixture]
	public class SimpleSampleProjectMetadataTests: ProjectMetadataTests
	{
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
	}

	[TestFixture]
	public class SimpleSampleChannelTests : ChannelTests
	{
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

		[Test]
		public void ChannelNameTests()
		{
			IChannel ch;
			ch = mProject.getPresentation().getChannelsManager().getChannelById("c1");
			Assert.AreEqual("EnglishVoice", ch.getName());
			ch = mProject.getPresentation().getChannelsManager().getChannelById("c2");
			Assert.AreEqual("DanishVoice", ch.getName());
			ch = mProject.getPresentation().getChannelsManager().getChannelById("c3");
			Assert.AreEqual("DanishText", ch.getName());
			ch = mProject.getPresentation().getChannelsManager().getChannelById("c4");
			Assert.AreEqual("Custom channel", ch.getName());
			ch = mProject.getPresentation().getChannelsManager().getChannelById("c5");
			Assert.AreEqual("Video channel", ch.getName());
			ch = mProject.getPresentation().getChannelsManager().getChannelById("c6");
			Assert.AreEqual("Image channel", ch.getName());
		}
	}

	[TestFixture]
	public class SimpleSampleBasicPresentationTests : BasicPresentationTests
	{
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
	}
}
