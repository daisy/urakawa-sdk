using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using NUnit.Framework;
using urakawa.property.channel;
using urakawa.media;
using urakawa.media.data.audio;

namespace urakawa.core
{
	/// <summary>
	/// Tests fixture for testing <see cref="TreeNode"/> functionality
	/// </summary>
	[TestFixture]
	public class TreeNodeTests
	{
		/// <summary>
		/// The <see cref="Uri"/> of the <see cref="Presentation"/> directory
		/// </summary>
		protected Uri mProjectDirectory
		{
			get { return new Uri(mPresentation.getRootUri(), "/"); }
		}
		/// <summary>
		/// The <see cref="Project"/> to use for the tests
		/// </summary>
		protected Project mProject;
		/// <summary>
		/// The <see cref="Presentation"/> to use for the tests - belongs to <see cref="mProject"/>
		/// </summary>
		protected Presentation mPresentation
		{
			get { return mProject.getPresentation(0); }
		}
		/// <summary>
		/// The root <see cref="TreeNode"/> of <see cref="mPresentation"/>
		/// </summary>
		protected TreeNode mRootNode
		{
			get { return mProject.getPresentation(0).getRootNode(); }
		}

		/// <summary>
		/// Default constructor
		/// </summary>
		public TreeNodeTests()
		{

		}

		private static ManagedAudioMedia createAudioMedia(Presentation pres, string waveFileName)
		{
			ManagedAudioMedia res = pres.getMediaFactory().createAudioMedia() as ManagedAudioMedia;
			Assert.IsNotNull(res, "Could not create a ManagedAudioMedia");
			res.getMediaData().appendAudioDataFromRiffWave(Path.Combine(pres.getRootUri().LocalPath, waveFileName));
			return res;
		}

		private static TextMedia createTextMedia(Presentation pres, string text)
		{
			TextMedia res = pres.getMediaFactory().createTextMedia() as TextMedia;
			Assert.IsNotNull(res, "Could not create TextMedia");
			res.setText(text);
			return res;
		}

		private static TreeNode createTreeNode(Presentation pres, string waveFileName, string text)
		{
			Channel audioChannel = pres.getChannelsManager().getListOfChannels("channel.audio")[0];
			Channel textChannel = pres.getChannelsManager().getListOfChannels("channel.text")[0];
			TreeNode node;
			ChannelsProperty chProp;
			node = pres.getTreeNodeFactory().createNode();
			chProp = pres.getPropertyFactory().createChannelsProperty();
			node.addProperty(chProp);
			chProp.setMedia(audioChannel, createAudioMedia(pres, waveFileName));
			chProp.setMedia(textChannel, createTextMedia(pres, text));
			return node;
		}

		/// <summary>
		/// Constructs the TreeNodeSample <see cref="Project"/>
		/// </summary>
		/// <returns>The project</returns>
		public static Project createTreeNodeTestSampleProject()
		{
			Uri projDir = new Uri(ProjectTests.SampleXukFileDirectoryUri, "TreeNodeTestsSample/");
			Project proj = new Project();
			Presentation pres = proj.getPresentation(0);
			pres.setRootUri(projDir);
			if (Directory.Exists(Path.Combine(projDir.LocalPath, "Data")))
			{
				Directory.Delete(Path.Combine(projDir.LocalPath, "Data"), true);
			}

			PCMFormatInfo pcmFmt = pres.getMediaDataManager().getDefaultPCMFormat();
			pcmFmt.setNumberOfChannels(1);
			pcmFmt.setSampleRate(22050);
			pcmFmt.setBitDepth(16);

			Channel audioChannel = pres.getChannelFactory().createChannel();
			audioChannel.setName("channel.audio");
			pres.getChannelsManager().addChannel(audioChannel);

			Channel textChannel = pres.getChannelFactory().createChannel();
			textChannel.setName("channel.text");
			pres.getChannelsManager().addChannel(textChannel);
			
			TreeNode mRootNode = proj.getPresentation(0).getRootNode();
			Assert.IsNotNull(mRootNode, "The root node of the newly created Presentation is null");

			mRootNode.appendChild(createTreeNode(pres, "SamplePDTB2.wav", "Sample PDTB V2"));

			TreeNode node = pres.getTreeNodeFactory().createNode();
			mRootNode.appendChild(node);
			node.appendChild(createTreeNode(pres, "Section1.wav", "Section 1"));
			TreeNode subNode = pres.getTreeNodeFactory().createNode();
			node.appendChild(subNode);
			subNode.appendChild(createTreeNode(pres, "ParagraphWith.wav", "Paragraph with"));
			subNode.appendChild(createTreeNode(pres, "Emphasis.wav", "emphasis"));
			subNode.appendChild(createTreeNode(pres, "And.wav", "and"));
			subNode.appendChild(createTreeNode(pres, "PageBreak.wav", "page break"));
			return proj;
		}

		/// <summary>
		/// Sets up the <see cref="Project"/> and <see cref="Presentation"/> to use for the tests - run before each test
		/// </summary>
		[SetUp]
		public void setUp()
		{
			mProject = createTreeNodeTestSampleProject();
		}

		/// <summary>
		/// Test for method <see cref="TreeNode.export"/> - tests that the exported <see cref="TreeNode"/> has the same value as the original
		/// </summary>
		[Test]
		public void export_valueEqualsAfterExport()
		{
			Uri exportDestProjUri = new Uri(mPresentation.getRootUri(), "ExportDestination/");
			if (Directory.Exists(exportDestProjUri.LocalPath))
			{
				Directory.Delete(exportDestProjUri.LocalPath, true);
			}
			TreeNode nodeToExport = mProject.getPresentation(0).getRootNode().getChild(1);
			Project exportDestProj = new Project();
			exportDestProj.getPresentation(0).setRootUri(exportDestProjUri);
			TreeNode exportedNode = nodeToExport.export(exportDestProj.getPresentation(0));
			Assert.AreSame(
				exportedNode.getPresentation(), exportDestProj.getPresentation(0), 
				"The exported TreeNode does not belong to the destination Presentation");
			exportDestProj.getPresentation(0).setRootNode(exportedNode);
			bool valueEquals = nodeToExport.valueEquals(exportedNode);
			Assert.IsTrue(valueEquals, "The exported TreeNode did not have the same value as the original");
		}
	}
}
