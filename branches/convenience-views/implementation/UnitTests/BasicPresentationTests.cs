using System;
using NUnit.Framework;
using urakawa.core;

namespace urakawa.unitTests.testbase
{
	/// <summary>
	/// basic tests on the Presentation object
	/// </summary> 
	public class BasicPresentationTests : TestCollectionBase
	{
		[Test] public void setPropertyAndCheckForNewValue()
		{
			CoreNode root = mProject.getPresentation().getRootNode();
			System.Collections.ArrayList channels = (System.Collections.ArrayList)
				mProject.getPresentation().getChannelsManager().getListOfChannels();

			int i = 0;
			for (i = 0; i<channels.Count; i++)
			{
				if (((Channel)channels[i]).isMediaTypeSupported(urakawa.media.MediaType.TEXT) == true)
				{
					break;
				}
			}

			if (i < channels.Count)
			{
				ChannelsProperty text_cp = mProject.getPresentation().getPropertyFactory().createChannelsProperty();
				urakawa.media.TextMedia txt = (urakawa.media.TextMedia)mProject.getPresentation().getMediaFactory().createMedia(urakawa.media.MediaType.TEXT);
				txt.setText("hello I am the new text for the root node");
				text_cp.setMedia((Channel)channels[i], txt);

				root.setProperty(text_cp);

				ChannelsProperty root_cp = 
					(ChannelsProperty)mProject.getPresentation().getRootNode().getProperty(typeof(ChannelsProperty));
				urakawa.media.TextMedia txt2 = 
					(urakawa.media.TextMedia)root_cp.getMedia((Channel)channels[i]);

				Assert.AreEqual(txt.getText(), txt2.getText());
				Assert.AreEqual(txt, txt2);

			}
		}
		[Test] public void IsPresentationNull()
		{
			Assert.IsNotNull(mProject.getPresentation());
		}

		[Test] public void IsRootNodeNull()
		{
			Assert.IsNotNull(mProject.getPresentation().getRootNode());
		}
		[Test] public void IsChannelsManagerNull()
		{
			Assert.IsNotNull(mProject.getPresentation().getChannelsManager());
		}
		
		[Test] public void IsChannelFactoryNull()
		{
			Assert.IsNotNull(mProject.getPresentation().getChannelFactory());
		}
		[Test] public void IsCoreNodeFactoryNull()
		{
			Assert.IsNotNull(mProject.getPresentation().getCoreNodeFactory());
		}
		[Test] public void IsMediaFactoryNull()
		{
			Assert.IsNotNull(mProject.getPresentation().getMediaFactory());
		}
		[Test] public void IsPropertyFactoryNull()
		{
			Assert.IsNotNull(mProject.getPresentation().getPropertyFactory());
		}
		[Test] public void TryToSetNullProperty()
		{
			urakawa.core.CoreNode root = mProject.getPresentation().getRootNode();
			if (root != null)
			{
				bool bGotException = false;
				try
				{
					root.setProperty(null);
				}
				catch (urakawa.exception.MethodParameterIsNullException)
				{
					bGotException = true;
				}
				if (bGotException == false)
				{
					Assert.Fail("Got no exception where one was expected");
				}
			}
		}
		[Test] public void GetRootParent()
		{
			urakawa.core.CoreNode root = mProject.getPresentation().getRootNode();
			if (root != null)
			{
				Assert.IsNull(root.getParent(), "Parent of root is null");
			}
		}


	
  }
}
