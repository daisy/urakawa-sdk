using System;
using NUnit.Framework;
using urakawa.core;

namespace urakawa.test.unitTests
{
	/// <summary>
	/// this is a class library to be used with NUnit unit testing software
	/// </summary>
	[TestFixture] 
	public class BasicPresentationTests
	{
		private urakawa.project.Project mProject;
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
					(ChannelsProperty)mProject.getPresentation().getRootNode().getProperty(PropertyType.CHANNEL);
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
			if (mProject.getPresentation() != null)
			{
				Assert.IsNotNull(mProject.getPresentation().getRootNode());
			}
		}
		[Test] public void IsChannelsManagerNull()
		{
			if (mProject.getPresentation() != null)
			{
				Assert.IsNotNull(mProject.getPresentation().getChannelsManager());
			}
		}


	
  }
}
