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
			TreeNode root = mProject.GetPresentation(0).RootNode;
			if (mProject.GetPresentation(0).ChannelsManager.getListOfChannels().Count == 0)
			{
				mProject.GetPresentation(0).ChannelsManager.addChannel(
					mProject.GetPresentation(0).ChannelFactory.createChannel("Channel", ToolkitSettings.XUK_NS));
			}
			Channel textCh = mProject.GetPresentation(0).ChannelsManager.getListOfChannels()[0];
			if (textCh!=null)
			{
				ChannelsProperty text_cp;
				if (!root.hasProperties(typeof(ChannelsProperty)))
				{
					text_cp = mProject.GetPresentation(0).PropertyFactory.createChannelsProperty();
				}
				else
				{
					text_cp = (ChannelsProperty)root.getProperty(typeof(ChannelsProperty));
				}
				urakawa.media.ITextMedia txt = mProject.GetPresentation(0).MediaFactory.createTextMedia();
				txt.setText("hello I am the new text for the root node");
				text_cp.setMedia(textCh, txt);

				root.addProperty(text_cp);

				ChannelsProperty root_cp = 
					(ChannelsProperty)mProject.GetPresentation(0).RootNode.getProperty(typeof(ChannelsProperty));
				urakawa.media.ITextMedia txt2 = 
					(urakawa.media.ITextMedia)root_cp.getMedia(textCh);

				Assert.AreEqual(txt.getText(), txt2.getText());
				Assert.AreEqual(txt, txt2);

			}
		}

		[Test] 
		public void IsPresentationNull()
		{
			Assert.IsNotNull(mProject.GetPresentation(0));
		}

		[Test] 
		public void IsChannelsManagerNull()
		{
			Assert.IsNotNull(mProject.GetPresentation(0).ChannelsManager);
		}
		
		[Test] 
		public void IsChannelFactoryNull()
		{
			Assert.IsNotNull(mProject.GetPresentation(0).ChannelFactory);
		}
		[Test] 
		public void IsTreeNodeFactoryNull()
		{
			Assert.IsNotNull(mProject.GetPresentation(0).TreeNodeFactory);
		}
		[Test] 
		public void IsMediaFactoryNull()
		{
			Assert.IsNotNull(mProject.GetPresentation(0).MediaFactory);
		}
		[Test] 
		public void IsPropertyFactoryNull()
		{
			Assert.IsNotNull(mProject.GetPresentation(0).PropertyFactory);
		}
		
		[Test] 
		public void TryToSetNullProperty()
		{
			urakawa.core.TreeNode root = mProject.GetPresentation(0).RootNode;
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
			urakawa.core.TreeNode root = mProject.GetPresentation(0).RootNode;
			if (root != null)
			{
				Assert.IsNull(root.getParent(), "Parent of root is null");
			}
		}


	
  }
}
