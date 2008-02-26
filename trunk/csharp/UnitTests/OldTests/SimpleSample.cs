using System;
using NUnit.Framework;
using urakawa.core;
using urakawa.property.channel;
using urakawa.unitTests.testbase;

namespace urakawa.unitTests.fixtures.xukfiles.simplesample
{

	[TestFixture]
	public class SimpleSampleChannelTests : ChannelTests
	{
		[TestFixtureSetUp]
		public void InitFixture()
		{
			mDefaultFile = "../../XukWorks/simplesample.xuk";
		}

		[Test]
		public void ChannelNameTests()
		{
			Channel ch;
			ch = mProject.getPresentation(0).getChannelsManager().getChannel("c1");
			Assert.AreEqual("EnglishVoice", ch.getName());
			ch = mProject.getPresentation(0).getChannelsManager().getChannel("c2");
			Assert.AreEqual("DanishVoice", ch.getName());
			ch = mProject.getPresentation(0).getChannelsManager().getChannel("c3");
			Assert.AreEqual("DanishText", ch.getName());
			ch = mProject.getPresentation(0).getChannelsManager().getChannel("c4");
			Assert.AreEqual("Custom channel", ch.getName());
			ch = mProject.getPresentation(0).getChannelsManager().getChannel("c5");
			Assert.AreEqual("Video channel", ch.getName());
			ch = mProject.getPresentation(0).getChannelsManager().getChannel("c6");
			Assert.AreEqual("Image channel", ch.getName());
		}
	}

	[TestFixture]
	public class SimpleSampleBasicPresentationTests : BasicPresentationTests
	{
		[TestFixtureSetUp]
		public void InitFixture()
		{
			mDefaultFile = "../../XukWorks/simplesample.xuk";
		}
	}
}
