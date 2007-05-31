using System;
using NUnit.Framework;
using urakawa.core;
using urakawa.properties.channel;

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
			ICoreNode root = mProject.getPresentation().getRootNode();
			System.Collections.Generic.IList<Channel> channels = 
				mProject.getPresentation().getChannelsManager().getListOfChannels();
			Channel textCh = null;
			foreach (Channel ch in channels)
			{
				if (ch.isMediaTypeSupported(urakawa.media.MediaType.TEXT)) 
				{
					textCh = ch;
					break;
				}
			}
			if (textCh!=null)
			{
				ChannelsProperty text_cp = mProject.getPresentation().getPropertyFactory().createChannelsProperty();
				urakawa.media.ITextMedia txt = (urakawa.media.ITextMedia)mProject.getPresentation().getMediaFactory().createMedia(urakawa.media.MediaType.TEXT);
				txt.setText("hello I am the new text for the root node");
				text_cp.setMedia(textCh, txt);

				root.setProperty(text_cp);

				ChannelsProperty root_cp = 
					(ChannelsProperty)mProject.getPresentation().getRootNode().getProperty(typeof(ChannelsProperty));
				urakawa.media.ITextMedia txt2 = 
					(urakawa.media.ITextMedia)root_cp.getMedia(textCh);

				Assert.AreEqual(txt.getText(), txt2.getText());
				Assert.AreEqual(txt, txt2);

			}
		}

		[Test] 
		public void IsPresentationNull()
		{
			Assert.IsNotNull(mProject.getPresentation());
		}

		[Test] 
		public void IsChannelsManagerNull()
		{
			Assert.IsNotNull(mProject.getPresentation().getChannelsManager());
		}
		
		[Test] 
		public void IsChannelFactoryNull()
		{
			Assert.IsNotNull(mProject.getPresentation().getChannelFactory());
		}
		[Test] 
		public void IsCoreNodeFactoryNull()
		{
			Assert.IsNotNull(mProject.getPresentation().getCoreNodeFactory());
		}
		[Test] 
		public void IsMediaFactoryNull()
		{
			Assert.IsNotNull(mProject.getPresentation().getMediaFactory());
		}
		[Test] 
		public void IsPropertyFactoryNull()
		{
			Assert.IsNotNull(mProject.getPresentation().getPropertyFactory());
		}
		
		[Test] 
		public void TryToSetNullProperty()
		{
			urakawa.core.ICoreNode root = mProject.getPresentation().getRootNode();
			if (root != null)
			{
				try
				{
					root.setProperty(null);
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
			urakawa.core.ICoreNode root = mProject.getPresentation().getRootNode();
			if (root != null)
			{
				Assert.IsNull(root.getParent(), "Parent of root is null");
			}
		}


	
  }
}
