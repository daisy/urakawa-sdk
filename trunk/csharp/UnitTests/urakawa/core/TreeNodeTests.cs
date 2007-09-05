using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using NUnit.Framework;
using urakawa;
using urakawa.core;
using urakawa.property.channel;
using urakawa.media;
using urakawa.media.data.audio;

namespace unittests.urakawa.core
{
	[TestFixture]
	public class TreeNodeTests
	{
		protected Uri mProjectDirectory;
		protected Project mProject;
		protected Presentation mPresentation
		{
			get { return mProject.getPresentation(); }
		}
		protected TreeNode mRootNode;

		public TreeNodeTests()
		{
			mProjectDirectory = new Uri(ProjectTests.SampleXukFileDirectoryUri, "TreeNodeTestsSample/");
		}

		private ManagedAudioMedia createAudioMedia(string waveFileName)
		{
			ManagedAudioMedia res = mPresentation.getMediaFactory().createAudioMedia() as ManagedAudioMedia;
			Assert.IsNotNull(res, "Could not create a ManagedAudioMedia");
			res.getMediaData().appendAudioDataFromRiffWave(Path.Combine(mPresentation.getBaseUri().LocalPath, waveFileName));
			return res;
		}

		private TextMedia createTextMedia(string text)
		{
			TextMedia res = mPresentation.getMediaFactory().createTextMedia() as TextMedia;
			Assert.IsNotNull(res, "Could not create TextMedia");
			res.setText(text);
			return res;
		}

		private TreeNode createTreeNode(string waveFileName, string text)
		{
			Channel audioChannel = mPresentation.getChannelsManager().getListOfChannels("channel.audio")[0];
			Channel textChannel = mPresentation.getChannelsManager().getListOfChannels("channel.text")[0];
			TreeNode node;
			ChannelsProperty chProp;
			node = mPresentation.getTreeNodeFactory().createNode();
			chProp = mPresentation.getPropertyFactory().createChannelsProperty();
			node.addProperty(chProp);
			chProp.setMedia(audioChannel,	createAudioMedia(waveFileName));
			chProp.setMedia(textChannel, createTextMedia(text));
			return node;
		}

		[SetUp]
		public void setUp()
		{
			if (Directory.Exists(Path.Combine(mProjectDirectory.LocalPath, "Data")))
			{
				Directory.Delete(Path.Combine(mProjectDirectory.LocalPath, "Data"), true);
			}
			mProject = new Project(mProjectDirectory);
			PCMFormatInfo pcmFmt = mPresentation.getMediaDataManager().getDefaultPCMFormat();
			pcmFmt.setNumberOfChannels(1);
			pcmFmt.setSampleRate(22050);
			pcmFmt.setBitDepth(16);

			Channel audioChannel = mPresentation.getChannelFactory().createChannel();
			audioChannel.setName("channel.audio");
			mPresentation.getChannelsManager().addChannel(audioChannel);

			Channel textChannel = mPresentation.getChannelFactory().createChannel();
			textChannel.setName("channel.text");
			mPresentation.getChannelsManager().addChannel(textChannel);
			
			mRootNode = mProject.getPresentation().getRootNode();
			Assert.IsNotNull(mRootNode, "The root node of the newly created Presentation is null");

			mRootNode.appendChild(createTreeNode("SamplePDTB2.wav", "Sample PDTB V2"));

			TreeNode node = mPresentation.getTreeNodeFactory().createNode();
			mRootNode.appendChild(node);
			node.appendChild(createTreeNode("Section1.wav", "Section 1"));
			TreeNode subNode = mPresentation.getTreeNodeFactory().createNode();
			node.appendChild(subNode);
			subNode.appendChild(createTreeNode("ParagraphWith.wav", "Paragraph with"));
			subNode.appendChild(createTreeNode("Emphasis.wav", "emphasis"));
			subNode.appendChild(createTreeNode("And.wav", "and"));
			subNode.appendChild(createTreeNode("PageBreak.wav", "page break"));
		}

		[Test]
		public void export_valueEqualsAfterExport()
		{
			Uri exportDestProjUri = new Uri(mPresentation.getBaseUri(), "ExportDestination/");
			if (Directory.Exists(exportDestProjUri.LocalPath))
			{
				Directory.Delete(exportDestProjUri.LocalPath, true);
			}
			TreeNode nodeToExport = mProject.getPresentation().getRootNode().getChild(1);
			Project exportDestProj = new Project(exportDestProjUri);
			TreeNode exportedNode = nodeToExport.export(exportDestProj.getPresentation());
			Assert.AreSame(
				exportedNode.getPresentation(), exportDestProj.getPresentation(), 
				"The exported TreeNode does not belong to the destination Presentation");
			exportDestProj.getPresentation().setRootNode(exportedNode);
			bool valueEquals = nodeToExport.valueEquals(exportedNode);
			Assert.IsTrue(valueEquals, "The exported TreeNode did not have the same value as the original");
		}
	}
}
