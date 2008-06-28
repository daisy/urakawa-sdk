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
			Presentation pres = proj.GetPresentation(0);
			publishTest(pres);
		}

		[Test]
		public void publishTest_with_varying_pcmFormat()
		{
			Project proj = new Project();
			Presentation pres = proj.AddNewPresentation();
			//TODO: Finish test
		}

		public static void publishTest(Presentation pres)
		{
			Project proj = pres.Project;
			Channel sourceCh = pres.ChannelsManager.getListOfChannels("channel.audio")[0];
			Channel destCh = pres.ChannelFactory.createChannel();
			destCh.setLanguage(sourceCh.getLanguage());
			destCh.setName(String.Format("{0}.published", sourceCh.getName()));
			pres.ChannelsManager.addChannel(destCh);
			Uri publishDestination = new Uri(pres.RootUri, "AudioPublishDestination/");
			if (Directory.Exists(publishDestination.LocalPath))
			{
				foreach (string file in Directory.GetFiles(publishDestination.LocalPath))
				{
					File.Delete(file);
				}
			}
			else
			{
				Directory.CreateDirectory(publishDestination.LocalPath);
			}
			TreeNodeTestDelegate del = new TreeNodeTestDelegate(
				delegate(TreeNode node) { 
					if (node.getParent() == node.Presentation.RootNode) return true; 
					return false; 
				});
			PublishManagedAudioVisitor publishVisitor = new PublishManagedAudioVisitor(del, null);
			publishVisitor.setSourceChannel(sourceCh);
			publishVisitor.setDestinationChannel(destCh);
			publishVisitor.setDestinationDirectory(publishDestination);
			pres.RootNode.acceptDepthFirst(publishVisitor);
			publishVisitor.writeCurrentAudioFile();
			Uri xukFile = new Uri(proj.GetPresentation(0).RootUri, "TreeNodeTestsSample.xuk");
			if (File.Exists(xukFile.LocalPath)) File.Delete(xukFile.LocalPath);
			proj.SaveXuk(xukFile);
			checkPublishedFiles(pres.RootNode, sourceCh, destCh, null, null, null);
		}

		private static void checkPublishedFiles(TreeNode node, Channel sourceCh, Channel destCh, Uri curWavUri, MemoryStream curAudioData, PCMFormatInfo curPCMFormat)
		{
			if (node.hasProperties(typeof(ChannelsProperty)))
			{
				ChannelsProperty chProp = node.getProperty<ChannelsProperty>();
				ManagedAudioMedia mam = chProp.getMedia(sourceCh) as ManagedAudioMedia;
				ExternalAudioMedia eam = chProp.getMedia(destCh) as ExternalAudioMedia;
				Assert.AreEqual(mam == null, eam == null, "There may be external audio media if and only if there is managed audio media");
				if (mam != null && eam != null)
				{
					Assert.IsTrue(mam.getDuration().isEqualTo(eam.getDuration()), "Duration of managed and external audio media differs");
					Uri eamUri = eam.getUri();
					if (curWavUri != eam.getUri())
					{
						if (curWavUri != null)
						{
							FileStream wavFS = new FileStream(curWavUri.LocalPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
							try
							{
								PCMDataInfo pcmInfo = PCMDataInfo.parseRiffWaveHeader(wavFS);
								Assert.IsTrue(pcmInfo.isCompatibleWith(curPCMFormat), "External audio has incompatible pcm format");
								curAudioData.Position = 0;
								Assert.AreEqual(curAudioData.Length, (long)pcmInfo.getDataLength(), "External audio has unexpected length");
								Assert.IsTrue(PCMDataInfo.compareStreamData(curAudioData, wavFS, (int)curAudioData.Length), "External audio contains wrong data");
							}
							finally
							{
								wavFS.Close();
							}
						}
						curWavUri = eam.getUri();
						curAudioData = new MemoryStream();
						curPCMFormat = mam.getMediaData().getPCMFormat();
					}
					Assert.IsTrue(curPCMFormat.ValueEquals(mam.getMediaData().getPCMFormat()), "Managed audio has incompatible pcm format");
					Stream manAudio = mam.getMediaData().getAudioData();
					try
					{
						media.data.StreamUtils.copyData(manAudio, curAudioData);
					}
					finally
					{
						manAudio.Close();
					}
				}

			}
			foreach (TreeNode child in node.getListOfChildren())
			{
				checkPublishedFiles(child, sourceCh, destCh, curWavUri, curAudioData, curPCMFormat);
			}
		}
	}

}
