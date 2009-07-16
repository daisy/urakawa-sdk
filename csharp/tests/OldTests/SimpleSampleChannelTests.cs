using NUnit.Framework;
using urakawa.property.channel;

namespace urakawa.oldTests
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
            ch = mProject.Presentations.Get(0).ChannelsManager.GetManagedObject("c1");
            Assert.AreEqual("EnglishVoice", ch.Name);
            ch = mProject.Presentations.Get(0).ChannelsManager.GetManagedObject("c2");
            Assert.AreEqual("DanishVoice", ch.Name);
            ch = mProject.Presentations.Get(0).ChannelsManager.GetManagedObject("c3");
            Assert.AreEqual("DanishText", ch.Name);
            ch = mProject.Presentations.Get(0).ChannelsManager.GetManagedObject("c4");
            Assert.AreEqual("Custom channel", ch.Name);
            ch = mProject.Presentations.Get(0).ChannelsManager.GetManagedObject("c5");
            Assert.AreEqual("Video channel", ch.Name);
            ch = mProject.Presentations.Get(0).ChannelsManager.GetManagedObject("c6");
            Assert.AreEqual("Image channel", ch.Name);
        }
    }
}