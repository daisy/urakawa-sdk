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
            ch = mProject.GetPresentation(0).ChannelsManager.GetChannel("c1");
            Assert.AreEqual("EnglishVoice", ch.Name);
            ch = mProject.GetPresentation(0).ChannelsManager.GetChannel("c2");
            Assert.AreEqual("DanishVoice", ch.Name);
            ch = mProject.GetPresentation(0).ChannelsManager.GetChannel("c3");
            Assert.AreEqual("DanishText", ch.Name);
            ch = mProject.GetPresentation(0).ChannelsManager.GetChannel("c4");
            Assert.AreEqual("Custom channel", ch.Name);
            ch = mProject.GetPresentation(0).ChannelsManager.GetChannel("c5");
            Assert.AreEqual("Video channel", ch.Name);
            ch = mProject.GetPresentation(0).ChannelsManager.GetChannel("c6");
            Assert.AreEqual("Image channel", ch.Name);
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