using NUnit.Framework;
using urakawa.core;
using urakawa.property.channel;

namespace urakawa.oldTests
{
    /// <summary>
    /// basic tests on the Presentation object
    /// </summary> 
    public class BasicPresentationTests : TestCollectionBase
    {
        [Test]
        public void setPropertyAndCheckForNewValue()
        {
            TreeNode root = mProject.Presentations.Get(0).RootNode;
            if (mProject.Presentations.Get(0).ChannelsManager.ManagedObjects.Count == 0)
            {
                    mProject.Presentations.Get(0).ChannelFactory.Create();
            }
            Channel textCh = mProject.Presentations.Get(0).ChannelsManager.ManagedObjects.Get(0);
            if (textCh != null)
            {
                ChannelsProperty text_cp;
                if (!root.HasProperties(typeof (ChannelsProperty)))
                {
                    text_cp = mProject.Presentations.Get(0).PropertyFactory.CreateChannelsProperty();
                }
                else
                {
                    text_cp = (ChannelsProperty) root.GetProperty(typeof (ChannelsProperty));
                }
                urakawa.media.AbstractTextMedia txt = mProject.Presentations.Get(0).MediaFactory.CreateTextMedia();
                txt.Text = "hello I am the new text for the root node";
                text_cp.SetMedia(textCh, txt);

                root.AddProperty(text_cp);

                ChannelsProperty root_cp =
                    (ChannelsProperty) mProject.Presentations.Get(0).RootNode.GetProperty(typeof (ChannelsProperty));
                urakawa.media.AbstractTextMedia txt2 =
                    (urakawa.media.AbstractTextMedia) root_cp.GetMedia(textCh);

                Assert.AreEqual(txt.Text, txt2.Text);
                Assert.AreEqual(txt, txt2);
            }
        }

        [Test]
        public void IsPresentationNull()
        {
            Assert.IsNotNull(mProject.Presentations.Get(0));
        }

        [Test]
        public void IsChannelsManagerNull()
        {
            Assert.IsNotNull(mProject.Presentations.Get(0).ChannelsManager);
        }

        [Test]
        public void IsChannelFactoryNull()
        {
            Assert.IsNotNull(mProject.Presentations.Get(0).ChannelFactory);
        }

        [Test]
        public void IsTreeNodeFactoryNull()
        {
            Assert.IsNotNull(mProject.Presentations.Get(0).TreeNodeFactory);
        }

        [Test]
        public void IsMediaFactoryNull()
        {
            Assert.IsNotNull(mProject.Presentations.Get(0).MediaFactory);
        }

        [Test]
        public void IsPropertyFactoryNull()
        {
            Assert.IsNotNull(mProject.Presentations.Get(0).PropertyFactory);
        }

        [Test]
        public void TryToSetNullProperty()
        {
            urakawa.core.TreeNode root = mProject.Presentations.Get(0).RootNode;
            if (root != null)
            {
                try
                {
                    root.AddProperty(null);
                }
                catch (exception.MethodParameterIsNullException)
                {
                    return;
                }
                Assert.Fail("Expected MethodParameterIsNullException");
            }
        }

        [Test]
        public void GetRootParent()
        {
            urakawa.core.TreeNode root = mProject.Presentations.Get(0).RootNode;
            if (root != null)
            {
                Assert.IsNull(root.Parent, "Parent of root is null");
            }
        }
    }
}