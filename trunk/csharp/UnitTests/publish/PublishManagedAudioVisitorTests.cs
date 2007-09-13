using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using NUnit.Framework;
using urakawa.core;
using urakawa.property.channel;
using urakawa.media;
using urakawa.media.data.audio;
namespace urakawa.publish
{
	[TestFixture]
	public class PublishManagedAudioVisitorTests
	{
		[Test]
		public void publishTest_with_TreeNodeTestsSample()
		{
			Project proj = TreeNodeTests.createTreeNodeTestSampleProject();
			Presentation pres = proj.getPresentation();
			Channel sourceCh = pres.getChannelsManager().getListOfChannels("channel.audio")[0];
			Channel destCh = pres.getChannelFactory().createChannel();
			destCh.setLanguage(sourceCh.getLanguage());
			destCh.setName(String.Format("{0}.published", sourceCh.getName()));
			pres.getChannelsManager().addChannel(destCh);
			Uri publishDestination = new Uri(pres.getBaseUri(), "AudioPublishDestination/");
			if (Directory.Exists(publishDestination.LocalPath)) Directory.Delete(publishDestination.LocalPath);
			Directory.CreateDirectory(publishDestination.LocalPath);
			TreeNodeTestDelegate del = new TreeNodeTestDelegate(
				delegate(TreeNode node) { 
					if (node.getParent() == node.getPresentation().getRootNode()) return true; 
					return false; 
				});
			PublishManagedAudioVisitor publishVisitor = new PublishManagedAudioVisitor(
				sourceCh, destCh, publishDestination, del);
			pres.getRootNode().acceptDepthFirst(publishVisitor);
			publishVisitor.writeCurrentAudioFile();
			Uri xukFile = new Uri(proj.getPresentation().getBaseUri(), "TreeNodeTestsSample.xuk");
			if (File.Exists(xukFile.LocalPath)) File.Delete(xukFile.LocalPath);
			proj.saveXUK(xukFile);
			bool publishedFilesOk = checkPublishedFiles(pres.getRootNode(), sourceCh, destCh);
			Assert.IsTrue(publishedFilesOk, "The published files did  not correspond to the source audio data");
		}

		private bool checkPublishedFiles(TreeNode node, Channel sourceCh, Channel destCh)
		{
			if (node.hasProperties(typeof(ChannelsProperty)))
			{
				ChannelsProperty chProp = node.getProperty<ChannelsProperty>();
				ManagedAudioMedia mam = chProp.getMedia(sourceCh) as ManagedAudioMedia;
				ExternalAudioMedia eam = chProp.getMedia(destCh) as ExternalAudioMedia;
				if (mam != null && eam != null)
				{
					//TODO: Check that the audio data in mam corresponds to the audio data pointed to by eam.
				}
				else if (mam != null || eam!=null)
				{
					return false;
				}

			}
			foreach (TreeNode child in node.getListOfChildren())
			{
				if (!checkPublishedFiles(child, sourceCh, destCh)) return false;
			}
			return true;
		}
	}

}
