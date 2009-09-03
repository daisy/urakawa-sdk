using System;
using System.IO;
using AudioLib;
using NUnit.Framework;
using urakawa.core;
using urakawa.property.channel;
using urakawa.media;
using urakawa.media.data.audio;

namespace urakawa.publish
{
    [TestFixture]
    public class PublishFlattenedManagedAudioVisitorTests
    {
        [Test]
        public void PublishTest_With_TreeNodeTestsSample()
        {
            Project proj = TreeNodeTests.CreateTreeNodeTestSampleProject();
            Presentation pres = proj.Presentations.Get(0);
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
            Channel destCh = pres.ChannelFactory.Create();
            destCh.Language = sourceCh.Language;
            destCh.Name = String.Format("{0}.published", sourceCh.Name);

            Uri publishDestination = new Uri(pres.RootUri, "AudioPublishFlattenedDestination/");
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
            PublishFlattenedManagedAudioVisitor publishVisitor = new PublishFlattenedManagedAudioVisitor(del, null);
            publishVisitor.SourceChannel = sourceCh;
            publishVisitor.DestinationChannel = destCh;
            publishVisitor.DestinationDirectory = publishDestination;
            pres.RootNode.AcceptDepthFirst(publishVisitor);
            
            Uri xukFile = new Uri(proj.Presentations.Get(0).RootUri, "TreeNodeTestsSample.xuk");
            if (File.Exists(xukFile.LocalPath)) File.Delete(xukFile.LocalPath);
            proj.SaveXuk(xukFile);
            CheckPublishedFiles(pres.RootNode, sourceCh, destCh, null, null, null);
        }

        private static void CheckPublishedFiles(TreeNode node, Channel sourceCh, Channel destCh, Uri curWavUri_,
                                                MemoryStream curAudioData, PCMFormatInfo curPCMFormat)
        {
            Uri curWavUri = (curWavUri_ == null ? null : new Uri(curWavUri_.ToString()));

            if (node.HasProperties(typeof(ChannelsProperty)))
            {
                ChannelsProperty chProp = node.GetProperty<ChannelsProperty>();
                ManagedAudioMedia mam = chProp.GetMedia(sourceCh) as ManagedAudioMedia;
                ExternalAudioMedia eam = chProp.GetMedia(destCh) as ExternalAudioMedia;
                
                //Assert.AreEqual(mam == null, eam == null, "There may be external audio media if and only if there is managed audio media");
                
                if (mam != null && eam != null)
                {
                    Assert.IsTrue(mam.Duration.IsEqualTo(eam.Duration),
                                  "Duration of managed and external audio media differs");

                    if (eam.Uri != null)
                    {
                        FileStream wavFS_ = new FileStream(eam.Uri.LocalPath, FileMode.Open, FileAccess.Read, FileShare.None);
                        Stream manAudioStream = mam.AudioMediaData.OpenPcmInputStream();
                        try
                        {
                            uint dataLength;
                            AudioLibPCMFormat pcmInfo = AudioLibPCMFormat.RiffHeaderParse(wavFS_, out dataLength);

                            Assert.IsTrue(pcmInfo.IsCompatibleWith(mam.AudioMediaData.PCMFormat.Data),
                                          "External audio has incompatible pcm format");

                            wavFS_.Position += pcmInfo.ConvertTimeToBytes(eam.ClipBegin.TimeAsMillisecondFloat);

                            Assert.IsTrue(
                                AudioLibPCMFormat.CompareStreamData(manAudioStream, wavFS_, (int)manAudioStream.Length),
                                "External audio contains wrong data");
                        }
                        finally
                        {
                            wavFS_.Close();
                            manAudioStream.Close();
                        }
                    }


                    if (curWavUri != null)
                    {
                        FileStream wavFS = new FileStream(curWavUri.LocalPath, FileMode.Open, FileAccess.Read,
                                                          FileShare.None);
                        try
                        {
                            uint dataLength;
                            AudioLibPCMFormat pcmInfo = AudioLibPCMFormat.RiffHeaderParse(wavFS, out dataLength);

                            Assert.IsTrue(pcmInfo.IsCompatibleWith(curPCMFormat.Data),
                                          "External audio has incompatible pcm format");

                            curAudioData.Position = 0;

                            Assert.AreEqual(curAudioData.Length, (long)dataLength,
                                            "External audio has unexpected length");
                            Assert.IsTrue(
                                AudioLibPCMFormat.CompareStreamData(curAudioData, wavFS, (int)curAudioData.Length),
                                "External audio contains wrong data");
                        }
                        finally
                        {
                            wavFS.Close();
                        }
                    }

                    if (curWavUri == null)
                    {
                        curWavUri = new Uri(eam.Uri.ToString());
                        curAudioData = new MemoryStream();
                        curPCMFormat = mam.AudioMediaData.PCMFormat;
                    }
                    else if (curWavUri.ToString() != eam.Uri.ToString())
                    {
                        curWavUri = new Uri(eam.Uri.ToString());
                        curAudioData = new MemoryStream();
                        curPCMFormat = mam.AudioMediaData.PCMFormat;
                    }

                    Assert.IsTrue(curPCMFormat.ValueEquals(mam.AudioMediaData.PCMFormat),
                                  "Managed audio has incompatible pcm format");
                    Stream manAudio = mam.AudioMediaData.OpenPcmInputStream();
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
            foreach (TreeNode child in node.Children.ContentsAs_YieldEnumerable)
            {
                CheckPublishedFiles(child, sourceCh, destCh, curWavUri, curAudioData, curPCMFormat);
            }
        }
    }
}