using System;
using NUnit.Framework;
using urakawa.core;

namespace urakawa.test.unitTests
{
	/// <summary>
	/// basic tests on the Presentation object
	/// </summary> 
	public class BasicPresentationTests
	{
		protected urakawa.core.Presentation mPresentation;

		[Test] public void setPropertyAndCheckForNewValue()
		{
			CoreNode root = mPresentation.getRootNode();
			System.Collections.ArrayList channels = (System.Collections.ArrayList)
				mPresentation.getChannelsManager().getListOfChannels();

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
				ChannelsProperty text_cp = mPresentation.getPropertyFactory().createChannelsProperty();
				urakawa.media.TextMedia txt = (urakawa.media.TextMedia)mPresentation.getMediaFactory().createMedia(urakawa.media.MediaType.TEXT);
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
			Assert.IsNotNull(mPresentation);
		}

		[Test] public void IsRootNodeNull()
		{
			Assert.IsNotNull(mPresentation.getRootNode());
		}
		[Test] public void IsChannelsManagerNull()
		{
			Assert.IsNotNull(mPresentation.getChannelsManager());
		}
		
		[Test] public void IsChannelFactoryNull()
		{
			Assert.IsNotNull(mPresentation.getChannelFactory());
		}
		[Test] public void IsCoreNodeFactoryNull()
		{
			Assert.IsNotNull(mPresentation.getCoreNodeFactory());
		}
		[Test] public void IsMediaFactoryNull()
		{
			Assert.IsNotNull(mPresentation.getMediaFactory());
		}
		[Test] public void IsPropertyFactoryNull()
		{
			Assert.IsNotNull(mPresentation.getPropertyFactory());
		}
		[Test] public void TryToSetNullProperty()
		{
			urakawa.core.CoreNode root = mPresentation.getRootNode();
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
			urakawa.core.CoreNode root = mPresentation.getRootNode();
			if (root != null)
			{
				Assert.IsNull(root.getParent(), "Parent of root is null");
			}
		}


	
  }
}
