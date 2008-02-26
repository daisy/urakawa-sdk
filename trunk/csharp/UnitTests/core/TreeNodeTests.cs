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
			mRootNode.changed += new EventHandler<urakawa.events.DataModelChangedEventArgs>(mTreeNode_changed);
			mRootNode.childAdded += new EventHandler<ChildAddedEventArgs>(mTreeNode_childAdded);
			mRootNode.childRemoved += new EventHandler<ChildRemovedEventArgs>(mTreeNode_childRemoved);
			mRootNode.propertyAdded += new EventHandler<PropertyAddedEventArgs>(mTreeNode_propertyAdded);
			mRootNode.propertyRemoved += new EventHandler<PropertyRemovedEventArgs>(mTreeNode_propertyRemoved);
		}

        /// <summary>
        /// Tests that an copied <see cref="TreeNode"/> has the same value as the original,
        /// without being the same instance - also tests that child <see cref="TreeNode"/>s and associated <see cref="Property"/>s
        /// are the same instances as thoose of the original
        /// </summary>
        [Test]
        public void copy_valueEqualsAfter()
        {
            urakawa.property.xml.XmlProperty newXmlProp = mPresentation.getPropertyFactory().createXmlProperty();
            newXmlProp.setQName("p", "");
            mRootNode.addProperty(newXmlProp);
            mRootNode.addProperty(mPresentation.getPropertyFactory().createChannelsProperty());
            TreeNode mRootCopy = mRootNode.copy(true);
            bool equal = mRootNode.valueEquals(mRootCopy);
            Assert.IsTrue(equal, "The copy is not the same as the original");
            Assert.AreNotSame(mRootNode, mRootCopy, "The copy is just a reference of the original itself");
            foreach (Type propType in mRootCopy.getListOfUsedPropertyTypes())
            {
                Assert.AreNotEqual(
                    mRootNode.getProperty(propType), mRootCopy.getProperty(propType),
                    "Property of copy is just a reference to the property of the original");
            }
            for (int i = 0; i < mRootCopy.getChildCount(); i++)
            {
                Assert.AreNotEqual(
                    mRootNode.getChild(i), mRootCopy.getChild(i),
                    "Child of copy is just a reference of the child of the original");
            }
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

        /// <summary>
        /// Tests that the <see cref="TreeNode.childRemoved"/> event occurs when children are removed 
        /// - also tests if <see cref="TreeNode.childRemoved"/> bubbles, i.e. triggers <see cref="TreeNode.changed"/> events
        /// </summary>
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
			removedChild.childRemoved += new EventHandler<ChildRemovedEventArgs>(mTreeNode_childRemoved);
			removedChild.changed += new EventHandler<urakawa.events.DataModelChangedEventArgs>(mTreeNode_changed);
			try
			{
				removedChild.removeChild(removedChild.getChild(removedChild.getChildCount() - 1));
				assertChildRemovedEventOccured(beforeCount, changedBeforeCount);
			}
			finally
			{
				removedChild.childRemoved -= new EventHandler<ChildRemovedEventArgs>(mTreeNode_childRemoved);
				removedChild.changed -= new EventHandler<urakawa.events.DataModelChangedEventArgs>(mTreeNode_changed);
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
			removedChild.childRemoved += new EventHandler<ChildRemovedEventArgs>(mTreeNode_childRemoved);
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

        /// <summary>
        /// Tests that the <see cref="TreeNode.propertyAdded"/> event occurs when properties are added 
        /// - also tests if <see cref="TreeNode.propertyAdded"/> bubbles, i.e. triggers <see cref="TreeNode.changed"/> events
        /// </summary>
        [Test]
        public void propertyAdded_eventOccursAndBubble()
        {
            int beforeCount;
            int changedBeforeCount;
            beforeCount = mPropertyAddedEventCount;
            changedBeforeCount = mChangedEventCount;
            urakawa.property.channel.ChannelsProperty newChProp = mPresentation.getPropertyFactory().createChannelsProperty();
            mRootNode.addProperty(newChProp);
            assertPropertyAddedEventOccured(beforeCount, changedBeforeCount, 1);
            mRootNode.removeProperty(newChProp);
            newChProp = mPresentation.getPropertyFactory().createChannelsProperty();
            property.xml.XmlProperty newXmlProp = mPresentation.getPropertyFactory().createXmlProperty();
            beforeCount = mPropertyAddedEventCount;
            changedBeforeCount = mChangedEventCount;
            mRootNode.addProperties(new List<urakawa.property.Property>(new urakawa.property.Property[] { newChProp, newXmlProp }));
            assertPropertyAddedEventOccured(beforeCount, changedBeforeCount, 2);
        }

        private void assertPropertyAddedEventOccured(int beforeCount, int changedBeforeCount, int expectedCountIncrease)
        {
            Assert.AreEqual(
                beforeCount + expectedCountIncrease, mPropertyAddedEventCount,
                "mPropertyAddedEventCount was not increased by the expected amount, "
                +"indicating that the propertyAdded event did not occur the expected number of times");
            Assert.AreEqual(
                changedBeforeCount + expectedCountIncrease, mChangedEventCount,
                "mChangedEventCount was not increased by the expected amount, "
                +"indicating that the changed event did not occur the expected number of times");
            Assert.AreSame(
                mLatestPropertyAddedEventArgs, mLatestChangedEventArgs,
                "The latest changed event args are not the same as the latest property added event args");
            Assert.AreSame(
                mLatestPropertyAddedSender, mLatestChangedSender,
                "The latest changed event sender is not the same as the latest property added event sender");
        }

        /// <summary>
        /// Tests that the event args and sender of the <see cref="TreeNode.propertyAdded"/> are correct
        /// </summary>
        [Test]
        public void propertyAddded_eventArgsAndSenderCorrect()
        {
            urakawa.property.xml.XmlProperty newXmlProp = mPresentation.getPropertyFactory().createXmlProperty();
            mRootNode.addProperty(newXmlProp);
            Assert.AreSame(
                newXmlProp, mLatestPropertyAddedEventArgs.AddedProperty,
                "The PropertyAddedEventArgs.AddedProperty must be the Property instance that was added");
            Assert.AreSame(
                mRootNode, mLatestPropertyAddedEventArgs.SourceTreeNode,
                "The PropertyAddedEventArgs.SourceTreeNode must be the TreeNode to which the property was added");
            Assert.AreSame(
                mRootNode, mLatestPropertyAddedSender,
                "The sender of the propertyAdded event must be the the TreeNode to which the property was added");
        }

        /// <summary>
        /// Tests that the <see cref="TreeNode.propertyRemoved"/> event occurs when properties are removed 
        /// - also tests if <see cref="TreeNode.propertyRemoved"/> bubbles, i.e. triggers <see cref="TreeNode.changed"/> events
        /// </summary>
        [Test]
        public void propertyRemoved_eventOccursAndBubble()
        {
            int beforeCount;
            int changedBeforeCount;
            urakawa.property.xml.XmlProperty newXmlProp = mPresentation.getPropertyFactory().createXmlProperty();
            mRootNode.addProperty(newXmlProp);
            beforeCount = mPropertyRemovedEventCount;
            changedBeforeCount = mChangedEventCount;
            mRootNode.removeProperty(newXmlProp);
            assertPropertyRemovedEventOccured(beforeCount, changedBeforeCount, 1);
            urakawa.property.channel.ChannelsProperty newChProp = mPresentation.getPropertyFactory().createChannelsProperty();
            mRootNode.addProperty(newXmlProp);
            mRootNode.addProperty(newChProp);
            beforeCount = mPropertyRemovedEventCount;
            changedBeforeCount = mChangedEventCount;


        }

        private void assertPropertyRemovedEventOccured(int beforeCount, int changedBeforeCount, int expectedCountIncrease)
        {
            Assert.AreEqual(
                beforeCount + expectedCountIncrease, mPropertyRemovedEventCount,
                "mPropertyRemovedEventCount was not increased by the expected amount, "
                + "indicating that the propertyRemoved event did not occur the expected number of times");
            Assert.AreEqual(
                changedBeforeCount + expectedCountIncrease, mChangedEventCount,
                "mChangedEventCount was not increased by the expected amount, "
                + "indicating that the changed event did not occur the expected number of times");
            Assert.AreSame(
                mLatestPropertyRemovedEventArgs, mLatestChangedEventArgs,
                "The latest changed event args are not the same as the latest property removed event args");
            Assert.AreSame(
                mLatestPropertyRemovedSender, mLatestChangedSender,
                "The latest changed event sender is not the same as the latest property removed event sender");
        }

        /// <summary>
        /// Tests that the event args and sender of the <see cref="TreeNode.propertyRemoved"/> are correct
        /// </summary>
        [Test]
        public void propertyRemoved_eventArgsAndSenderCorrect()
        {
            urakawa.property.xml.XmlProperty newXmlProp = mPresentation.getPropertyFactory().createXmlProperty();
            mRootNode.addProperty(newXmlProp);
            newXmlProp.setQName("dtbook", "");
            urakawa.property.channel.ChannelsProperty newChProp = mPresentation.getPropertyFactory().createChannelsProperty();
            mRootNode.addProperty(newChProp);
            mRootNode.removeProperty(newXmlProp);
            Assert.AreSame(
                newXmlProp, mLatestPropertyRemovedEventArgs.RemovedProperty,
                "The PropertyRemovedEventArgs.RemovedProperty must be the Property that was removed");
            Assert.AreSame(
                mRootNode, mLatestPropertyRemovedEventArgs.SourceTreeNode,
                "The PropertyRemovedEventArgs.SourceTreeNode must be the TreeNode from which the Property was removed");
            Assert.AreSame(
                mRootNode, mLatestPropertyRemovedSender,
                "The sender of the propertyRemoved event must be the TreeNode from which the Property was removed");
        }


        /// <summary>
        /// Tests that changed events bubbles correctly from child to parent TreeNodes
        /// </summary>
        [Test]
        public void changed_bubblesFromChildren()
        {
            TreeNode child = mRootNode.getChild(0);
            events.DataModelChangedEventArgs childChangedEventArgs = null;
            object childChangedSender = null;
            EventHandler<urakawa.events.DataModelChangedEventArgs> handler = 
                new EventHandler<urakawa.events.DataModelChangedEventArgs>(
                    delegate(object sender, events.DataModelChangedEventArgs e) 
                    { 
                        childChangedEventArgs = e;
                        childChangedSender = sender;
                    });
            child.changed += handler;
            try
            {
                int beforeCount = mChangedEventCount;
                child.appendChild(mPresentation.getTreeNodeFactory().createNode());
                Assert.IsNotNull(childChangedEventArgs, "The changed event of the child does not seem to have occured");
                Assert.AreSame(
                    child, childChangedSender, 
                    "The sender of the changed event on the child must be the child it self");
                Assert.AreEqual(
                    beforeCount + 1, mChangedEventCount,
                    "The mChangedEventCount did not increase by one, indicating that the changed event on the parent/root TreeNode "
                    + "did not occur as a result of the changed event on the child");
                Assert.AreSame(
                    childChangedEventArgs, mLatestChangedEventArgs,
                    "The event args of the parent/root changed event was not the same instance as thoose of the child changed evnet");
                Assert.AreSame(
                    mRootNode, mLatestChangedSender,
                    "The sender of the parent/root changed event must be the parent/root node itself");

            }
            finally
            {
                child.changed -= handler;
            }
        }

        /// <summary>
        /// Tests that changed events bubble correctly from owned Properties to owning TreeNodes
        /// </summary>
        [Test]
        public void changed_bubblesfromProperties()
        {
            urakawa.property.xml.XmlProperty xmlProp = mPresentation.getPropertyFactory().createXmlProperty();
            mRootNode.addProperty(xmlProp);
            events.DataModelChangedEventArgs propChangedEventArgs = null;
            object propChangedSender = null;
            EventHandler<urakawa.events.DataModelChangedEventArgs> handler =
                new EventHandler<urakawa.events.DataModelChangedEventArgs>(
                    delegate(object sender, events.DataModelChangedEventArgs e)
                    {
                        propChangedEventArgs = e;
                        propChangedSender = sender;
                    });
            xmlProp.changed += handler;
            try
            {
                int beforeCount = mChangedEventCount;
                xmlProp.setQName("dtbook", "");
                Assert.IsNotNull(propChangedEventArgs, "The changed event of the property does not seem to have occured");
                Assert.AreSame(
                    xmlProp, propChangedSender,
                    "The sender of the changed event on the property must be the property itself");
                Assert.AreEqual(
                    beforeCount + 1, mChangedEventCount,
                    "The mChangedEventCount did not increase by one, indicating that the changed event on the owning TreeNode "
                    + "did not occur as a result of the changed event on the owned Property");
                Assert.AreSame(
                    propChangedEventArgs, mLatestChangedEventArgs,
                    "The event args of the owning TreeNode changed event was not the same instance as thoose of the owned Property changed evnet");
                Assert.AreSame(
                    mRootNode, mLatestChangedSender,
                    "The sender of the owning TreeNode changed event must be the owning TreeNode node itself");

            }
            finally
            {
                xmlProp.changed -= handler;
            }
        }

		private PropertyAddedEventArgs mLatestPropertyAddedEventArgs;
		private object mLatestPropertyAddedSender;
		private int mPropertyAddedEventCount = 0;
		void mTreeNode_propertyAdded(object sender, PropertyAddedEventArgs e)
		{
			mLatestPropertyAddedSender = sender;
			mLatestPropertyAddedEventArgs = e;
			mPropertyAddedEventCount++;
		}

		private PropertyRemovedEventArgs mLatestPropertyRemovedEventArgs;
		private object mLatestPropertyRemovedSender;
		private int mPropertyRemovedEventCount = 0;
		void mTreeNode_propertyRemoved(object sender, PropertyRemovedEventArgs e)
		{
			mLatestPropertyRemovedSender = sender;
			mLatestPropertyRemovedEventArgs = e;
			mPropertyRemovedEventCount++;
		}

		private ChildRemovedEventArgs mLatestChildRemovedEventArgs;
		private object mLatestChildRemovedSender;
		private int mChildRemovedEventCount = 0;
		void mTreeNode_childRemoved(object sender, ChildRemovedEventArgs e)
		{
			mLatestChildRemovedSender = sender;
			mLatestChildRemovedEventArgs = e;
			mChildRemovedEventCount++;
		}

		private ChildAddedEventArgs mLatestChildAddedEventArgs;
		private object mLatestChildAddedSender;
		private int mChildAddedEventCount = 0;
		void mTreeNode_childAdded(object sender, ChildAddedEventArgs e)
		{
			mLatestChildAddedSender = sender;
			mLatestChildAddedEventArgs = e;
			mChildAddedEventCount++;
		}

		private events.DataModelChangedEventArgs mLatestChangedEventArgs;
		private object mLatestChangedSender;
		private int mChangedEventCount = 0;
		void mTreeNode_changed(object sender, urakawa.events.DataModelChangedEventArgs e)
		{
			mLatestChangedSender = sender;
			mLatestChangedEventArgs = e;
			mChangedEventCount++;
		}

		#endregion
	}
}
