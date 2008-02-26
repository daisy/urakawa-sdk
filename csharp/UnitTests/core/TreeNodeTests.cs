using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using NUnit.Framework;
using urakawa.property.channel;
using urakawa.media;
using urakawa.media.data.audio;
using urakawa.events.core;

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
			Presentation pres = proj.addNewPresentation();
			pres.setRootUri(projDir);
			if (Directory.Exists(Path.Combine(projDir.LocalPath, "Data")))
			{
                try
                {
                    Directory.Delete(Path.Combine(projDir.LocalPath, "Data"), true);
                }
                catch (Exception e)
                {
                    // Added by Julien as the deletion sometimes fails (?)
                    System.Diagnostics.Debug.Print("Oops, could not delete directory {0}: {1}",
                        Path.Combine(projDir.LocalPath, "Data"), e.Message);
                }
			}

			pres.getMediaDataManager().setDefaultPCMFormat(new PCMFormatInfo(1, 22050, 16));

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
			mRootNode.changed += new EventHandler<urakawa.events.DataModelChangedEventArgs>(mRootNode_changed);
			mRootNode.languageChanged += new EventHandler<urakawa.events.LanguageChangedEventArgs>(mRootNode_languageChanged);
			mRootNode.childAdded += new EventHandler<ChildAddedEventArgs>(mRootNode_childAdded);
			mRootNode.childRemoved += new EventHandler<ChildRemovedEventArgs>(mRootNode_childRemoved);
			mRootNode.propertyAdded += new EventHandler<PropertyAddedEventArgs>(mRootNode_propertyAdded);
			mRootNode.propertyRemoved += new EventHandler<PropertyRemovedEventArgs>(mRootNode_propertyRemoved);
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
			exportDestProj.addNewPresentation();
			exportDestProj.getPresentation(0).setRootUri(exportDestProjUri);
			TreeNode exportedNode = nodeToExport.export(exportDestProj.getPresentation(0));
			Assert.AreSame(
				exportedNode.getPresentation(), exportDestProj.getPresentation(0), 
				"The exported TreeNode does not belong to the destination Presentation");
			exportDestProj.getPresentation(0).setRootNode(exportedNode);
			bool valueEquals = nodeToExport.valueEquals(exportedNode);
			Assert.IsTrue(valueEquals, "The exported TreeNode did not have the same value as the original");
		}

		#region Event tests

		/// <summary>
		/// Tests that the <see cref="TreeNode.childAdded"/> event occurs when children are added 
		/// - also tests if <see cref="TreeNode.childAdded"/> bubbles, i.e. triggers <see cref="TreeNode.changed"/> events
		/// </summary>
		[Test]
		public void childAdded_eventOccursAndBubble()
		{
			int beforeCount;
			int changedBeforeCount;
			beforeCount = mChildAddedEventCount;
			changedBeforeCount = mChangedEventCount;
			mRootNode.appendChild(mPresentation.getTreeNodeFactory().createNode());
			assertChildAddedEventOccured(beforeCount, changedBeforeCount);
			beforeCount = mChildAddedEventCount;
			changedBeforeCount = mChangedEventCount;
			mRootNode.insert(mPresentation.getTreeNodeFactory().createNode(), 0);
			assertChildAddedEventOccured(beforeCount, changedBeforeCount);
		}

		/// <summary>
		/// Tests that the event args and sender of the <see cref="TreeNode.childAdded"/> event are correct
		/// </summary>
		[Test]
		public void childAdded_eventArgsAndSenderCorrect()
		{
			TreeNode addee;
			addee = mPresentation.getTreeNodeFactory().createNode();
			mRootNode.appendChild(addee);
			Assert.AreSame(
				addee, mLatestChildAddedEventArgs.AddedChild,
				"The AddedChild member of the ChildAddedEventArgs is unexpectedly not TreeNode that was added");
			Assert.AreSame(
				mRootNode, mLatestChildAddedEventArgs.SourceTreeNode,
				"The SourceTreeNode is unexpectedly not the root TreeNode");
			Assert.AreSame(
				mRootNode, mLatestChildAddedSender,
				"The sender of the ChildAdded event was unexpectedly not the root TreeNode");
			addee = mPresentation.getTreeNodeFactory().createNode();
			mRootNode.insert(addee, 0);
			Assert.AreSame(
				addee, mLatestChildAddedEventArgs.AddedChild,
				"The AddedChild member of the ChildAddedEventArgs is unexpectedly not TreeNode that was added");
			Assert.AreSame(
				mRootNode, mLatestChildAddedEventArgs.SourceTreeNode,
				"The SourceTreeNode is unexpectedly not the root TreeNode");
			Assert.AreSame(
				mRootNode, mLatestChildAddedSender,
				"The sender of the ChildAdded event was unexpectedly not the root TreeNode");

		}

		private void assertChildAddedEventOccured(int childAddedBeforeCount, int changedBeforeCount)
		{
			Assert.AreEqual(
				childAddedBeforeCount + 1, mChildAddedEventCount,
				"mChildAddedEventCount was not increased by one, indicating that the childAdded event did not occur");
			Assert.AreEqual(
				changedBeforeCount + 1, mChangedEventCount,
				"mChangedEventCount was not increased by one, indicating that the changed event did not occur");
			Assert.AreSame(
				mLatestChildAddedEventArgs, mLatestChangedEventArgs,
				"The latest changed event args are not the same as the latest child added event args");
			Assert.AreSame(
				mLatestChildAddedSender, mLatestChangedSender,
				"The latest changed event sender is not the same as the latest child added event sender");
		}

		[Test]
		public void childRemoved_eventOccursAndBubble()
		{
			int beforeCount;
			int changedBeforeCount;
			beforeCount = mChildRemovedEventCount;
			changedBeforeCount = mChangedEventCount;
			TreeNode  removedChild = mRootNode.removeChild(1);
			assertChildRemovedEventOccured(beforeCount, changedBeforeCount);
			beforeCount = mChildRemovedEventCount;
			changedBeforeCount = mChangedEventCount;
			removedChild.childRemoved += new EventHandler<ChildRemovedEventArgs>(mRootNode_childRemoved);
			removedChild.changed += new EventHandler<urakawa.events.DataModelChangedEventArgs>(mRootNode_changed);
			try
			{
				removedChild.removeChild(removedChild.getChild(removedChild.getChildCount() - 1));
				assertChildRemovedEventOccured(beforeCount, changedBeforeCount);
			}
			finally
			{
				removedChild.childRemoved -= new EventHandler<ChildRemovedEventArgs>(mRootNode_childRemoved);
				removedChild.changed -= new EventHandler<urakawa.events.DataModelChangedEventArgs>(mRootNode_changed);
			}

		}

		private void assertChildRemovedEventOccured(int childRemovedBeforeCount, int changedBeforeCount)
		{
			Assert.AreEqual(
				childRemovedBeforeCount + 1, mChildRemovedEventCount,
				"mChildRemovedEventCount was not increased by one, indicating that the childRemoved event did not occur");
			Assert.AreEqual(
				changedBeforeCount + 1, mChangedEventCount,
				"mChangedEventCount was not increased by one, indicating that the changed event did not occur");
			Assert.AreSame(
				mLatestChildRemovedEventArgs, mLatestChangedEventArgs,
				"The latest changed event args are not the same as the latest child removed event args");
			Assert.AreSame(
				mLatestChildRemovedSender, mLatestChangedSender,
				"The latest changed event sender is not the same as the latest child removed event sender");
		}

		/// <summary>
		/// Tests that the event args and sender of the <see cref="TreeNode.childRemoved"/> event are correct
		/// </summary>
		[Test]
		public void childRemoved_eventArgsAndSenderCorrect()
		{
			int pos = 1;
			TreeNode removedChild = mRootNode.removeChild(pos);
			Assert.AreSame(
				mRootNode, mLatestChildRemovedSender, 
				"The sender of the childRemoved event must be the TreeNode from which the the child was removed");
			Assert.AreSame(
				removedChild, mLatestChildRemovedEventArgs.RemovedChild,
				"The RemovedChild member of the child removed event args must be the child that was removed");
			Assert.AreEqual(
				pos, mLatestChildRemovedEventArgs.RemovedPosition,
				"The RemovedPosition member of the child removed event args must be the position of the child before it was removed");
			removedChild.childRemoved += new EventHandler<ChildRemovedEventArgs>(mRootNode_childRemoved);
			pos = removedChild.getChildCount() - 1;
			TreeNode removedChild2 = removedChild.removeChild(pos);
			Assert.AreSame(
				removedChild, mLatestChildRemovedSender,
				"The sender of the childRemoved event must be the TreeNode from which the the child was removed");
			Assert.AreSame(
				removedChild2, mLatestChildRemovedEventArgs.RemovedChild,
				"The RemovedChild member of the child removed event args must be the child that was removed");
			Assert.AreEqual(
				pos, mLatestChildRemovedEventArgs.RemovedPosition,
				"The RemovedPosition member of the child removed event args must be the position of the child before it was removed");

		}


		private PropertyRemovedEventArgs mLatestPropertyRemovedEventArgs;
		private object mLatestPropertyRemovedSender;
		private int mPropertyRemovedEventCount = 0;
		void mRootNode_propertyRemoved(object sender, PropertyRemovedEventArgs e)
		{
			mLatestPropertyRemovedSender = sender;
			mLatestPropertyRemovedEventArgs = e;
			mPropertyRemovedEventCount++;
		}

		private PropertyAddedEventArgs mLatestPropertyAddedEventArgs;
		private object mLatestPropertyAddedSender;
		private int mPropertyAddedEventCount = 0;
		void mRootNode_propertyAdded(object sender, PropertyAddedEventArgs e)
		{
			mLatestPropertyAddedSender = sender;
			mLatestPropertyAddedEventArgs = e;
			mPropertyAddedEventCount++;
		}

		private events.LanguageChangedEventArgs mLatestLanguageChangedEventArgs;
		private object mLatestLanguageChangedSender;
		private int mLanguageChangedEventCount = 0;
		void mRootNode_languageChanged(object sender, urakawa.events.LanguageChangedEventArgs e)
		{
			mLatestLanguageChangedSender = sender;
			mLatestLanguageChangedEventArgs = e;
			mLanguageChangedEventCount++;
		}

		private ChildRemovedEventArgs mLatestChildRemovedEventArgs;
		private object mLatestChildRemovedSender;
		private int mChildRemovedEventCount = 0;
		void mRootNode_childRemoved(object sender, ChildRemovedEventArgs e)
		{
			mLatestChildRemovedSender = sender;
			mLatestChildRemovedEventArgs = e;
			mChildRemovedEventCount++;
		}

		private ChildAddedEventArgs mLatestChildAddedEventArgs;
		private object mLatestChildAddedSender;
		private int mChildAddedEventCount = 0;
		void mRootNode_childAdded(object sender, ChildAddedEventArgs e)
		{
			mLatestChildAddedSender = sender;
			mLatestChildAddedEventArgs = e;
			mChildAddedEventCount++;
		}

		private events.DataModelChangedEventArgs mLatestChangedEventArgs;
		private object mLatestChangedSender;
		private int mChangedEventCount = 0;
		void mRootNode_changed(object sender, urakawa.events.DataModelChangedEventArgs e)
		{
			mLatestChangedSender = sender;
			mLatestChangedEventArgs = e;
			mChangedEventCount++;
		}

		#endregion
	}
}
