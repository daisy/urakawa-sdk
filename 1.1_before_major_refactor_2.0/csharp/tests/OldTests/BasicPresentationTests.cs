using System;
using NUnit.Framework;
using urakawa.core;
using urakawa.property.channel;

namespace urakawa.unitTests.testbase
{
	/// <summary>
	/// basic tests on the Presentation object
	/// </summary> 
	public class BasicPresentationTests : TestCollectionBase
	{
		[Test] 
		public void setPropertyAndCheckForNewValue()
		{
			TreeNode root = mProject.getPresentation(0).getRootNode();
			if (mProject.getPresentation(0).getChannelsManager().getListOfChannels().Count == 0)
			{
				mProject.getPresentation(0).getChannelsManager().addChannel(
					mProject.getPresentation(0).getChannelFactory().createChannel("Channel", ToolkitSettings.XUK_NS));
			}
			Channel textCh = mProject.getPresentation(0).getChannelsManager().getListOfChannels()[0];
			if (textCh!=null)
			{
				ChannelsProperty text_cp;
				if (!root.hasProperties(typeof(ChannelsProperty)))
				{
					text_cp = mProject.getPresentation(0).getPropertyFactory().createChannelsProperty();
				}
				else
				{
					text_cp = (ChannelsProperty)root.getProperty(typeof(ChannelsProperty));
				}
				urakawa.media.ITextMedia txt = mProject.getPresentation(0).getMediaFactory().createTextMedia();
				txt.setText("hello I am the new text for the root node");
				text_cp.setMedia(textCh, txt);

				root.addProperty(text_cp);

				ChannelsProperty root_cp = 
					(ChannelsProperty)mProject.getPresentation(0).getRootNode().getProperty(typeof(ChannelsProperty));
				urakawa.media.ITextMedia txt2 = 
					(urakawa.media.ITextMedia)root_cp.getMedia(textCh);

				Assert.AreEqual(txt.getText(), txt2.getText());
				Assert.AreEqual(txt, txt2);

			}
		}

		[Test] 
		public void IsPresentationNull()
		{
			Assert.IsNotNull(mProject.getPresentation(0));
		}

		[Test] 
		public void IsChannelsManagerNull()
		{
			Assert.IsNotNull(mProject.getPresentation(0).getChannelsManager());
		}
		
		[Test] 
		public void IsChannelFactoryNull()
		{
			Assert.IsNotNull(mProject.getPresentation(0).getChannelFactory());
		}
		[Test] 
		public void IsTreeNodeFactoryNull()
		{
			Assert.IsNotNull(mProject.getPresentation(0).getTreeNodeFactory());
		}
		[Test] 
		public void IsMediaFactoryNull()
		{
			Assert.IsNotNull(mProject.getPresentation(0).getMediaFactory());
		}
		[Test] 
		public void IsPropertyFactoryNull()
		{
			Assert.IsNotNull(mProject.getPresentation(0).getPropertyFactory());
		}
		
		[Test] 
		public void TryToSetNullProperty()
		{
			urakawa.core.TreeNode root = mProject.getPresentation(0).getRootNode();
			if (root != null)
			{
				try
				{
					root.addProperty(null);
				}
				catch (exception.MethodParameterIsNullException)
				{
					return;
				}
				Assert.Fail("Expected MethodParameterIsNullException");
			}
		}
		[Test] public void GetRootParent()
		{
			urakawa.core.TreeNode root = mProject.getPresentation(0).getRootNode();
			if (root != null)
			{
				Assert.IsNull(root.getParent(), "Parent of root is null");
			}
		}


	
  }
}
