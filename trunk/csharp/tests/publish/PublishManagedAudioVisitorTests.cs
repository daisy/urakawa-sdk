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
        public void PublishTest_With_TreeNodeTestsSample()
        {
            Project proj = TreeNodeTests.CreateTreeNodeTestSampleProject();
            Presentation pres = proj.GetPresentation(0);
            PublishTest(pres);
        }

        [Test]
        public void PublishTest_With_Varying_PcmFormat()
        {
            Project proj = new Project();
            Presentation pres = proj.AddNewPresentation();
            //TODO: Finish test
        }

        public static void PublishTest(Presentation pres)
        {
            Project proj = pres.Project;
            Channel sourceCh = pres.ChannelsManager.GetChannelsByName("channel.audio")[0];
            Channel destCh = pres.ChannelFactory.CreateChannel();
            destCh.Language = sourceCh.Language;
            destCh.Name = String.Format("{0}.published", sourceCh.Name);
            pres.ChannelsManager.AddChannel(destCh);
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
                delegate(TreeNode node)
                    {
                        if (node.Parent == node.Presentation.RootNode) return true;
                        return false;
                    });
            PublishManagedAudioVisitor publishVisitor = new PublishManagedAudioVisitor(del, null);
            publishVisitor.SourceChannel = sourceCh;
            publishVisitor.DestinationChannel = destCh;
            publishVisitor.DestinationDirectory = publishDestination;
            pres.RootNode.AcceptDepthFirst(publishVisitor);
            publishVisitor.WriteCurrentAudioFile();
            Uri xukFile = new Uri(proj.GetPresentation(0).RootUri, "TreeNodeTestsSample.xuk");
            if (File.Exists(xukFile.LocalPath)) File.Delete(xukFile.LocalPath);
            proj.SaveXuk(xukFile);
            CheckPublishedFiles(pres.RootNode, sourceCh, destCh, null, null, null);
        }

        private static void CheckPublishedFiles(TreeNode node, Channel sourceCh, Channel destCh, Uri curWavUri,
                                                MemoryStream curAudioData, PCMFormatInfo curPCMFormat)
        {
            if (node.HasProperties(typeof (ChannelsProperty)))
            {
                ChannelsProperty chProp = node.GetProperty<ChannelsProperty>();
                ManagedAudioMedia mam = chProp.GetMedia(sourceCh) as ManagedAudioMedia;
                ExternalAudioMedia eam = chProp.GetMedia(destCh) as ExternalAudioMedia;
                Assert.AreEqual(mam == null, eam == null,
                                "There may be external audio media if and only if there is managed audio media");
                if (mam != null && eam != null)
                {
                    Assert.IsTrue(mam.Duration.IsEqualTo(eam.Duration),
                                  "Duration of managed and external audio media differs");
                    Uri eamUri = eam.Uri;
                    if (curWavUri != eam.Uri)
                    {
                        if (curWavUri != null)
                        {
                            FileStream wavFS = new FileStream(curWavUri.LocalPath, FileMode.Open, FileAccess.Read,
                                                              FileShare.ReadWrite);
                            try
                            {
                                PCMDataInfo pcmInfo = PCMDataInfo.ParseRiffWaveHeader(wavFS);
                                Assert.IsTrue(pcmInfo.IsCompatibleWith(curPCMFormat),
                                              "External audio has incompatible pcm format");
                                curAudioData.Position = 0;
                                Assert.AreEqual(curAudioData.Length, (long) pcmInfo.DataLength,
                                                "External audio has unexpected length");
                                Assert.IsTrue(
                                    PCMDataInfo.CompareStreamData(curAudioData, wavFS, (int) curAudioData.Length),
                                    "External audio contains wrong data");
                            }
                            finally
                            {
                                wavFS.Close();
                            }
                        }
                        curWavUri = eam.Uri;
                        curAudioData = new MemoryStream();
                        curPCMFormat = mam.AudioMediaData.PCMFormat;
                    }
                    Assert.IsTrue(curPCMFormat.ValueEquals(mam.AudioMediaData.PCMFormat),
                                  "Managed audio has incompatible pcm format");
                    Stream manAudio = mam.AudioMediaData.GetAudioData();
                    try
                    {
                        media.data.StreamUtils.CopyData(manAudio, curAudioData);
                    }
                    finally
                    {
                        manAudio.Close();
                    }
                }
            }
            foreach (TreeNode child in node.ListOfChildren)
            {
                CheckPublishedFiles(child, sourceCh, destCh, curWavUri, curAudioData, curPCMFormat);
            }
        }
    }
}