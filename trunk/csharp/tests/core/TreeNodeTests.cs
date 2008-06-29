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
			get { return new Uri(mPresentation.RootUri, "/"); }
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
			get { return mProject.GetPresentation(0); }
		}
		/// <summary>
		/// The mRootNode <see cref="TreeNode"/> of <see cref="mPresentation"/>
		/// </summary>
		protected TreeNode mRootNode
		{
			get { return mPresentation.RootNode; }
		}

		/// <summary>
		/// Default constructor
		/// </summary>
		public TreeNodeTests()
		{

		}

		private static ManagedAudioMedia createAudioMedia(Presentation pres, string waveFileName)
		{
			ManagedAudioMedia res = pres.MediaFactory.CreateAudioMedia() as ManagedAudioMedia;
			Assert.IsNotNull(res, "Could not create a ManagedAudioMedia");
			res.MediaData.AppendAudioDataFromRiffWave(Path.Combine(pres.RootUri.LocalPath, waveFileName));
			return res;
		}

		private static TextMedia createTextMedia(Presentation pres, string text)
		{
			TextMedia res = pres.MediaFactory.CreateTextMedia() as TextMedia;
			Assert.IsNotNull(res, "Could not create TextMedia");
			res.Text = text;
			return res;
		}

		private static TreeNode createTreeNode(Presentation pres, string waveFileName, string text)
		{
			Channel audioChannel = pres.ChannelsManager.getListOfChannels("channel.audio")[0];
			Channel textChannel = pres.ChannelsManager.getListOfChannels("channel.text")[0];
			TreeNode node;
			ChannelsProperty chProp;
			node = pres.TreeNodeFactory.CreateNode();
			chProp = pres.PropertyFactory.createChannelsProperty();
			node.AddProperty(chProp);
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
			Presentation pres = proj.AddNewPresentation();
			pres.RootUri = projDir;
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

			pres.MediaDataManager.DefaultPCMFormat = new PCMFormatInfo(1, 22050, 16);

			Channel audioChannel = pres.ChannelFactory.createChannel();
			audioChannel.setName("channel.audio");
			pres.ChannelsManager.addChannel(audioChannel);

			Channel textChannel = pres.ChannelFactory.createChannel();
			textChannel.setName("channel.text");
			pres.ChannelsManager.addChannel(textChannel);
			
			TreeNode mRootNode = proj.GetPresentation(0).RootNode;
			Assert.IsNotNull(mRootNode, "The mRootNode node of the newly created Presentation is null");

			mRootNode.AppendChild(createTreeNode(pres, "SamplePDTB2.wav", "Sample PDTB V2"));

			TreeNode node = pres.TreeNodeFactory.CreateNode();
			mRootNode.AppendChild(node);
			node.AppendChild(createTreeNode(pres, "Section1.wav", "Section 1"));
			TreeNode subNode = pres.TreeNodeFactory.CreateNode();
			node.AppendChild(subNode);
			subNode.AppendChild(createTreeNode(pres, "ParagraphWith.wav", "Paragraph with"));
			subNode.AppendChild(createTreeNode(pres, "Emphasis.wav", "emphasis"));
			subNode.AppendChild(createTreeNode(pres, "And.wav", "and"));
			subNode.AppendChild(createTreeNode(pres, "PageBreak.wav", "page break"));
			return proj;
		}

		/// <summary>
		/// Sets up the <see cref="Project"/> and <see cref="Presentation"/> to use for the tests - run before each test
		/// </summary>
		[SetUp]
		public void setUp()
		{
			mProject = createTreeNodeTestSampleProject();
			mRootNode.Changed += new EventHandler<urakawa.events.DataModelChangedEventArgs>(mTreeNode_changed);
			mRootNode.ChildAdded += new EventHandler<ChildAddedEventArgs>(mTreeNode_childAdded);
			mRootNode.ChildRemoved += new EventHandler<ChildRemovedEventArgs>(mTreeNode_childRemoved);
			mRootNode.PropertyAdded += new EventHandler<PropertyAddedEventArgs>(mTreeNode_propertyAdded);
			mRootNode.PropertyRemoved += new EventHandler<PropertyRemovedEventArgs>(mTreeNode_propertyRemoved);
		}

        /// <summary>
        /// Tests that an copied <see cref="TreeNode"/> has the same value as the original,
        /// without being the same instance - also tests that child <see cref="TreeNode"/>s and associated <see cref="Property"/>s
        /// are the same instances as thoose of the original
        /// </summary>
        [Test]
        public void copy_valueEqualsAfter()
        {
            urakawa.property.xml.XmlProperty newXmlProp = mPresentation.PropertyFactory.createXmlProperty();
            newXmlProp.setQName("p", "");
            mRootNode.AddProperty(newXmlProp);
            mRootNode.AddProperty(mPresentation.PropertyFactory.createChannelsProperty());
            TreeNode mRootCopy = mRootNode.Copy(true);
            bool equal = mRootNode.ValueEquals(mRootCopy);
            Assert.IsTrue(equal, "The copy is not the same as the original");
            Assert.AreNotSame(mRootNode, mRootCopy, "The copy is just a reference of the original itself");
            foreach (Type propType in mRootCopy.ListOfUsedPropertyTypes)
            {
                Assert.AreNotEqual(
                    mRootNode.GetProperty(propType), mRootCopy.GetProperty(propType),
                    "Property of copy is just a reference to the property of the original");
            }
            for (int i = 0; i < mRootCopy.ChildCount; i++)
            {
                Assert.AreNotEqual(
                    mRootNode.GetChild(i), mRootCopy.GetChild(i),
                    "Child of copy is just a reference of the child of the original");
            }
        }



		/// <summary>
		/// Test for method <see cref="TreeNode.Export"/> - tests that the exported <see cref="TreeNode"/> has the same value as the original
		/// </summary>
		[Test]
		public void export_valueEqualsAfterExport()
		{
			Uri exportDestProjUri = new Uri(mPresentation.RootUri, "ExportDestination/");
			if (Directory.Exists(exportDestProjUri.LocalPath))
			{
				Directory.Delete(exportDestProjUri.LocalPath, true);
			}
			TreeNode nodeToExport = mPresentation.RootNode.GetChild(1);
			Project exportDestProj = new Project();
			exportDestProj.AddNewPresentation();
			exportDestProj.GetPresentation(0).RootUri = exportDestProjUri;
			TreeNode exportedNode = nodeToExport.Export(exportDestProj.GetPresentation(0));
			Assert.AreSame(
				exportedNode.Presentation, exportDestProj.GetPresentation(0), 
				"The exported TreeNode does not belong to the destination Presentation");
			exportDestProj.GetPresentation(0).RootNode = exportedNode;
			bool valueEquals = nodeToExport.ValueEquals(exportedNode);
			Assert.IsTrue(valueEquals, "The exported TreeNode did not have the same value as the original");
        }

        /// <summary>
        /// Tests the functionality of <see cref="TreeNode.getChildCount"/>
        /// </summary>
        [Test]
        public void getChildCount()
        {
            int initCount = mRootNode.ChildCount;
            mRootNode.AppendChild(mPresentation.TreeNodeFactory.CreateNode());
            Assert.AreEqual(initCount + 1, mRootNode.ChildCount, "Child count should increase by one when appending a child");
            mRootNode.AppendChild(mPresentation.TreeNodeFactory.CreateNode());
            mRootNode.AppendChild(mPresentation.TreeNodeFactory.CreateNode());
            for (int index = 0; index < mRootNode.ChildCount; index++)
            {
                Assert.IsNotNull(mRootNode.GetChild(index), String.Format("No child at index {0:0} that is within bounds", index));
            }
            initCount = mRootNode.ChildCount;
            mRootNode.RemoveChild(0);
            Assert.AreEqual(initCount - 1, mRootNode.ChildCount, "Child count should decrease by one when removing a child");
        }

        /// <summary>
        /// Tests the functionality of the <see cref="TreeNode.Detach"/>
        /// </summary>
        [Test]
        [ExpectedException(typeof(exception.NodeDoesNotExistException))]
        public void detach()
        {
            TreeNode newNode = mPresentation.TreeNodeFactory.CreateNode();
            mRootNode.AppendChild(newNode);
            newNode.Detach();
            Assert.IsNull(newNode.Parent, "Parent of detached child must be null");
            mRootNode.IndexOf(newNode);
        }

        /// <summary>
        /// Tests the basics of the <see cref="TreeNode.removeChild"/> method
        /// </summary>
        [Test]
        [ExpectedException(typeof(exception.NodeDoesNotExistException))]
        public void removeChild_basics()
        {
            TreeNode newNode = mPresentation.TreeNodeFactory.CreateNode();
            mRootNode.AppendChild(newNode);
            mRootNode.RemoveChild(newNode);
            Assert.IsNull(newNode.Parent, "Parent of removed child must be null");
            mRootNode.IndexOf(newNode);
        }

        /// <summary>
        /// Tests that the proper exception is thrown when trying to remove a <see cref="TreeNode"/> that is not a child
        /// </summary>
        [Test]
        [ExpectedException(typeof(exception.NodeDoesNotExistException))]
        public void removeChild_nonChild()
        {
            TreeNode rootChild = mPresentation.TreeNodeFactory.CreateNode();
            mRootNode.AppendChild(rootChild);
            TreeNode newNode = mPresentation.TreeNodeFactory.CreateNode();
            rootChild.AppendChild(newNode);
            mRootNode.RemoveChild(newNode);
        }

        /// <summary>
        /// Tests the basics of the <see cref="TreeNode.IndexOf"/>
        /// </summary>
        [Test]
        public void indexOf_basics()
        {
            mRootNode.AppendChild(mPresentation.TreeNodeFactory.CreateNode());
            mRootNode.AppendChild(mPresentation.TreeNodeFactory.CreateNode());
            mRootNode.AppendChild(mPresentation.TreeNodeFactory.CreateNode());
            for (int index = 0; index < mRootNode.ChildCount; index++)
            {
                Assert.AreEqual(index, mRootNode.IndexOf(mRootNode.GetChild(index)));
            }
        }

        /// <summary>
        /// Tests that a <see cref="exception.NodeDoesNotExistException"/> calling <see cref="TreeNode.IndexOf"/>
        /// with a non-child
        /// </summary>
        [Test]
        [ExpectedException(typeof(exception.NodeDoesNotExistException))]
        public void indexOf_nonChild()
        {
            TreeNode rootChild = mPresentation.TreeNodeFactory.CreateNode();
            mRootNode.AppendChild(rootChild);
            TreeNode newNode = mPresentation.TreeNodeFactory.CreateNode();
            rootChild.AppendChild(newNode);
            mRootNode.IndexOf(newNode);
        }

        [Test]
        public void replaceChild_basics()
        {
            TreeNode rootChild = mPresentation.TreeNodeFactory.CreateNode();
            mRootNode.AppendChild(mPresentation.TreeNodeFactory.CreateNode());
            mRootNode.AppendChild(mPresentation.TreeNodeFactory.CreateNode());
            mRootNode.AppendChild(mPresentation.TreeNodeFactory.CreateNode());
            mRootNode.AppendChild(mPresentation.TreeNodeFactory.CreateNode());
            mRootNode.AppendChild(rootChild);
            mRootNode.AppendChild(mPresentation.TreeNodeFactory.CreateNode());
            mRootNode.AppendChild(mPresentation.TreeNodeFactory.CreateNode());
            int index = mRootNode.IndexOf(rootChild);
            TreeNode newNode = mPresentation.TreeNodeFactory.CreateNode();
            mRootNode.ReplaceChild(newNode, rootChild);
            Assert.AreEqual(newNode, mRootNode.GetChild(index));
        }

        [Test]
        [ExpectedException(typeof(exception.NodeDoesNotExistException))]
        public void replaceChild_nonChild()
        {
            TreeNode rootChild = mPresentation.TreeNodeFactory.CreateNode();
            mRootNode.AppendChild(mPresentation.TreeNodeFactory.CreateNode());
            mRootNode.AppendChild(mPresentation.TreeNodeFactory.CreateNode());
            mRootNode.AppendChild(mPresentation.TreeNodeFactory.CreateNode());
            mRootNode.AppendChild(mPresentation.TreeNodeFactory.CreateNode());
            mRootNode.AppendChild(rootChild);
            mRootNode.AppendChild(mPresentation.TreeNodeFactory.CreateNode());
            mRootNode.AppendChild(mPresentation.TreeNodeFactory.CreateNode());
            TreeNode newNode = mPresentation.TreeNodeFactory.CreateNode();
            TreeNode nonChildNode = mPresentation.TreeNodeFactory.CreateNode();
            rootChild.AppendChild(nonChildNode);
            mRootNode.ReplaceChild(newNode, nonChildNode);
        }


        #region insert/append tests


        [Test]
        public void appendChild_basics()
        {
            TreeNode new_node = mPresentation.TreeNodeFactory.CreateNode();
            mRootNode.AppendChild(new_node);
            Assert.AreEqual(mRootNode.ChildCount - 1, mRootNode.IndexOf(new_node), "A newly appended child is at the last index");
        }

        [Test]
        [ExpectedException(typeof(exception.NodeNotDetachedException))]
        public void append_sameNodeTwice()
        {
            TreeNode new_node = mPresentation.TreeNodeFactory.CreateNode();
            mRootNode.AppendChild(new_node);
            mRootNode.AppendChild(new_node);
        }


        [Test]
        [ExpectedException(typeof(exception.NodeDoesNotExistException))]
        public void insertAfter_NewNodeAsSiblingOfRoot()
        {
            mRootNode.InsertAfter(mPresentation.TreeNodeFactory.CreateNode(), mRootNode);
        }

        [Test]
        [ExpectedException(typeof(exception.NodeNotDetachedException))]
        public void insert_SameNodeTwice()
        {
            TreeNode new_node = mPresentation.TreeNodeFactory.CreateNode();
            mRootNode.Insert(new_node, 0);
            mRootNode.Insert(new_node, 0);
        }

        [Test]
        [ExpectedException(typeof(exception.MethodParameterIsNullException))]
        public void insert_NullNode()
        {
            TreeNode nullNode = null;
            mRootNode.Insert(nullNode, 0);
        }

        [Test]
        [ExpectedException(typeof(exception.MethodParameterIsOutOfBoundsException))]
        public void insert_NodeAtIndexBeyondEnd()
        {
            mRootNode.AppendChild(mPresentation.TreeNodeFactory.CreateNode());
            mRootNode.AppendChild(mPresentation.TreeNodeFactory.CreateNode());
            mRootNode.AppendChild(mPresentation.TreeNodeFactory.CreateNode());
            mRootNode.Insert(mPresentation.TreeNodeFactory.CreateNode(), mRootNode.ChildCount + 2);
        }

        [Test]
        [ExpectedException(typeof(exception.MethodParameterIsOutOfBoundsException))]
        public void insert_NodeAtNegativeIndex()
        {
            mRootNode.Insert(mPresentation.TreeNodeFactory.CreateNode(), -1);
        }

        [Test]
        public void insertAfter_atExpectedIndex()
        {
            mRootNode.AppendChild(mPresentation.TreeNodeFactory.CreateNode());
            mRootNode.AppendChild(mPresentation.TreeNodeFactory.CreateNode());
            mRootNode.AppendChild(mPresentation.TreeNodeFactory.CreateNode());
            int index = mRootNode.ChildCount / 2;
            TreeNode anchorNode = mRootNode.GetChild(index);
            TreeNode newNode = mPresentation.TreeNodeFactory.CreateNode();
            mRootNode.InsertAfter(newNode, anchorNode);
            int newIndex = mRootNode.IndexOf(newNode);
            Assert.AreEqual(index + 1, newIndex);
        }

        [Test]
        [ExpectedException(typeof(exception.NodeDoesNotExistException))]
        public void insertAfter_nonChildAnchor()
        {
            TreeNode newNode = mPresentation.TreeNodeFactory.CreateNode();
            TreeNode mRootNodeChild = mPresentation.TreeNodeFactory.CreateNode();
            TreeNode anchorNode = mPresentation.TreeNodeFactory.CreateNode();
            mRootNode.AppendChild(mRootNodeChild);
            mRootNodeChild.AppendChild(anchorNode);
            mRootNode.InsertAfter(anchorNode, newNode);
        }

        [Test]
        public void insertBefore_atExpectedIndex()
        {
            mRootNode.AppendChild(mPresentation.TreeNodeFactory.CreateNode());
            mRootNode.AppendChild(mPresentation.TreeNodeFactory.CreateNode());
            mRootNode.AppendChild(mPresentation.TreeNodeFactory.CreateNode());
            int index = mRootNode.ChildCount / 2;
            TreeNode anchorNode = mRootNode.GetChild(index);
            TreeNode newNode = mPresentation.TreeNodeFactory.CreateNode();
            mRootNode.InsertBefore(newNode, anchorNode);
            Assert.AreEqual(index, mRootNode.IndexOf(newNode));
        }

        [Test]
        [ExpectedException(typeof(exception.NodeDoesNotExistException))]
        public void insertBefore_nonChildAnchor()
        {
            TreeNode newNode = mPresentation.TreeNodeFactory.CreateNode();
            TreeNode rootChild = mPresentation.TreeNodeFactory.CreateNode();
            TreeNode anchorNode = mPresentation.TreeNodeFactory.CreateNode();
            mRootNode.AppendChild(rootChild);
            rootChild.AppendChild(anchorNode);
            mRootNode.InsertBefore(anchorNode, newNode);
        }

        #endregion

        #region Event tests

        /// <summary>
		/// Tests that the <see cref="TreeNode.ChildAdded"/> event occurs when children are added 
		/// - also tests if <see cref="TreeNode.ChildAdded"/> bubbles, i.e. triggers <see cref="TreeNode.Changed"/> events
		/// </summary>
		[Test]
		public void childAdded_eventOccursAndBubble()
		{
			int beforeCount;
			int changedBeforeCount;
			beforeCount = mChildAddedEventCount;
			changedBeforeCount = mChangedEventCount;
			mRootNode.AppendChild(mPresentation.TreeNodeFactory.CreateNode());
			assertChildAddedEventOccured(beforeCount, changedBeforeCount);
			beforeCount = mChildAddedEventCount;
			changedBeforeCount = mChangedEventCount;
			mRootNode.Insert(mPresentation.TreeNodeFactory.CreateNode(), 0);
			assertChildAddedEventOccured(beforeCount, changedBeforeCount);
		}

		/// <summary>
		/// Tests that the event args and sender of the <see cref="TreeNode.ChildAdded"/> event are correct
		/// </summary>
		[Test]
		public void childAdded_eventArgsAndSenderCorrect()
		{
			TreeNode addee;
			addee = mPresentation.TreeNodeFactory.CreateNode();
			mRootNode.AppendChild(addee);
			Assert.AreSame(
				addee, mLatestChildAddedEventArgs.AddedChild,
				"The AddedChild member of the ChildAddedEventArgs is unexpectedly not TreeNode that was added");
			Assert.AreSame(
				mRootNode, mLatestChildAddedEventArgs.SourceTreeNode,
				"The SourceTreeNode is unexpectedly not the mRootNode TreeNode");
			Assert.AreSame(
				mRootNode, mLatestChildAddedSender,
				"The sender of the ChildAdded event was unexpectedly not the mRootNode TreeNode");
			addee = mPresentation.TreeNodeFactory.CreateNode();
			mRootNode.Insert(addee, 0);
			Assert.AreSame(
				addee, mLatestChildAddedEventArgs.AddedChild,
				"The AddedChild member of the ChildAddedEventArgs is unexpectedly not TreeNode that was added");
			Assert.AreSame(
				mRootNode, mLatestChildAddedEventArgs.SourceTreeNode,
				"The SourceTreeNode is unexpectedly not the mRootNode TreeNode");
			Assert.AreSame(
				mRootNode, mLatestChildAddedSender,
				"The sender of the ChildAdded event was unexpectedly not the mRootNode TreeNode");

		}

		private void assertChildAddedEventOccured(int childAddedBeforeCount, int changedBeforeCount)
		{
			Assert.AreEqual(
				childAddedBeforeCount + 1, mChildAddedEventCount,
				"mChildAddedEventCount was not increased by one, indicating that the ChildAdded event did not occur");
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
        /// Tests that the <see cref="TreeNode.ChildRemoved"/> event occurs when children are removed 
        /// - also tests if <see cref="TreeNode.ChildRemoved"/> bubbles, i.e. triggers <see cref="TreeNode.Changed"/> events
        /// </summary>
        [Test]
		public void childRemoved_eventOccursAndBubble()
		{
			int beforeCount;
			int changedBeforeCount;
			beforeCount = mChildRemovedEventCount;
			changedBeforeCount = mChangedEventCount;
			TreeNode  removedChild = mRootNode.RemoveChild(1);
			assertChildRemovedEventOccured(beforeCount, changedBeforeCount);
			beforeCount = mChildRemovedEventCount;
			changedBeforeCount = mChangedEventCount;
			removedChild.ChildRemoved += new EventHandler<ChildRemovedEventArgs>(mTreeNode_childRemoved);
			removedChild.Changed += new EventHandler<urakawa.events.DataModelChangedEventArgs>(mTreeNode_changed);
			try
			{
				removedChild.RemoveChild(removedChild.GetChild(removedChild.ChildCount - 1));
				assertChildRemovedEventOccured(beforeCount, changedBeforeCount);
			}
			finally
			{
				removedChild.ChildRemoved -= new EventHandler<ChildRemovedEventArgs>(mTreeNode_childRemoved);
				removedChild.Changed -= new EventHandler<urakawa.events.DataModelChangedEventArgs>(mTreeNode_changed);
			}

		}

		private void assertChildRemovedEventOccured(int childRemovedBeforeCount, int changedBeforeCount)
		{
			Assert.AreEqual(
				childRemovedBeforeCount + 1, mChildRemovedEventCount,
				"mChildRemovedEventCount was not increased by one, indicating that the ChildRemoved event did not occur");
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
		/// Tests that the event args and sender of the <see cref="TreeNode.ChildRemoved"/> event are correct
		/// </summary>
		[Test]
		public void childRemoved_eventArgsAndSenderCorrect()
		{
			int pos = 1;
			TreeNode removedChild = mRootNode.RemoveChild(pos);
			Assert.AreSame(
				mRootNode, mLatestChildRemovedSender, 
				"The sender of the ChildRemoved event must be the TreeNode from which the the child was removed");
			Assert.AreSame(
				removedChild, mLatestChildRemovedEventArgs.RemovedChild,
				"The RemovedChild member of the child removed event args must be the child that was removed");
			Assert.AreEqual(
				pos, mLatestChildRemovedEventArgs.RemovedPosition,
				"The RemovedPosition member of the child removed event args must be the position of the child before it was removed");
			removedChild.ChildRemoved += new EventHandler<ChildRemovedEventArgs>(mTreeNode_childRemoved);
			pos = removedChild.ChildCount - 1;
			TreeNode removedChild2 = removedChild.RemoveChild(pos);
			Assert.AreSame(
				removedChild, mLatestChildRemovedSender,
				"The sender of the ChildRemoved event must be the TreeNode from which the the child was removed");
			Assert.AreSame(
				removedChild2, mLatestChildRemovedEventArgs.RemovedChild,
				"The RemovedChild member of the child removed event args must be the child that was removed");
			Assert.AreEqual(
				pos, mLatestChildRemovedEventArgs.RemovedPosition,
				"The RemovedPosition member of the child removed event args must be the position of the child before it was removed");

		}

        /// <summary>
        /// Tests that the <see cref="TreeNode.PropertyAdded"/> event occurs when properties are added 
        /// - also tests if <see cref="TreeNode.PropertyAdded"/> bubbles, i.e. triggers <see cref="TreeNode.Changed"/> events
        /// </summary>
        [Test]
        public void propertyAdded_eventOccursAndBubble()
        {
            int beforeCount;
            int changedBeforeCount;
            beforeCount = mPropertyAddedEventCount;
            changedBeforeCount = mChangedEventCount;
            urakawa.property.channel.ChannelsProperty newChProp = mPresentation.PropertyFactory.createChannelsProperty();
            mRootNode.AddProperty(newChProp);
            assertPropertyAddedEventOccured(beforeCount, changedBeforeCount, 1);
            mRootNode.RemoveProperty(newChProp);
            newChProp = mPresentation.PropertyFactory.createChannelsProperty();
            property.xml.XmlProperty newXmlProp = mPresentation.PropertyFactory.createXmlProperty();
            beforeCount = mPropertyAddedEventCount;
            changedBeforeCount = mChangedEventCount;
            mRootNode.AddProperties(new List<urakawa.property.Property>(new urakawa.property.Property[] { newChProp, newXmlProp }));
            assertPropertyAddedEventOccured(beforeCount, changedBeforeCount, 2);
        }

        private void assertPropertyAddedEventOccured(int beforeCount, int changedBeforeCount, int expectedCountIncrease)
        {
            Assert.AreEqual(
                beforeCount + expectedCountIncrease, mPropertyAddedEventCount,
                "mPropertyAddedEventCount was not increased by the expected amount, "
                +"indicating that the PropertyAdded event did not occur the expected number of times");
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
        /// Tests that the event args and sender of the <see cref="TreeNode.PropertyAdded"/> are correct
        /// </summary>
        [Test]
        public void propertyAddded_eventArgsAndSenderCorrect()
        {
            urakawa.property.xml.XmlProperty newXmlProp = mPresentation.PropertyFactory.createXmlProperty();
            mRootNode.AddProperty(newXmlProp);
            Assert.AreSame(
                newXmlProp, mLatestPropertyAddedEventArgs.AddedProperty,
                "The PropertyAddedEventArgs.AddedProperty must be the Property instance that was added");
            Assert.AreSame(
                mRootNode, mLatestPropertyAddedEventArgs.SourceTreeNode,
                "The PropertyAddedEventArgs.SourceTreeNode must be the TreeNode to which the property was added");
            Assert.AreSame(
                mRootNode, mLatestPropertyAddedSender,
                "The sender of the PropertyAdded event must be the the TreeNode to which the property was added");
        }

        /// <summary>
        /// Tests that the <see cref="TreeNode.PropertyRemoved"/> event occurs when properties are removed 
        /// - also tests if <see cref="TreeNode.PropertyRemoved"/> bubbles, i.e. triggers <see cref="TreeNode.Changed"/> events
        /// </summary>
        [Test]
        public void propertyRemoved_eventOccursAndBubble()
        {
            int beforeCount;
            int changedBeforeCount;
            urakawa.property.xml.XmlProperty newXmlProp = mPresentation.PropertyFactory.createXmlProperty();
            mRootNode.AddProperty(newXmlProp);
            beforeCount = mPropertyRemovedEventCount;
            changedBeforeCount = mChangedEventCount;
            mRootNode.RemoveProperty(newXmlProp);
            assertPropertyRemovedEventOccured(beforeCount, changedBeforeCount, 1);
            urakawa.property.channel.ChannelsProperty newChProp = mPresentation.PropertyFactory.createChannelsProperty();
            mRootNode.AddProperty(newXmlProp);
            mRootNode.AddProperty(newChProp);
            beforeCount = mPropertyRemovedEventCount;
            changedBeforeCount = mChangedEventCount;


        }

        private void assertPropertyRemovedEventOccured(int beforeCount, int changedBeforeCount, int expectedCountIncrease)
        {
            Assert.AreEqual(
                beforeCount + expectedCountIncrease, mPropertyRemovedEventCount,
                "mPropertyRemovedEventCount was not increased by the expected amount, "
                + "indicating that the PropertyRemoved event did not occur the expected number of times");
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
        /// Tests that the event args and sender of the <see cref="TreeNode.PropertyRemoved"/> are correct
        /// </summary>
        [Test]
        public void propertyRemoved_eventArgsAndSenderCorrect()
        {
            urakawa.property.xml.XmlProperty newXmlProp = mPresentation.PropertyFactory.createXmlProperty();
            mRootNode.AddProperty(newXmlProp);
            newXmlProp.setQName("dtbook", "");
            urakawa.property.channel.ChannelsProperty newChProp = mPresentation.PropertyFactory.createChannelsProperty();
            mRootNode.AddProperty(newChProp);
            mRootNode.RemoveProperty(newXmlProp);
            Assert.AreSame(
                newXmlProp, mLatestPropertyRemovedEventArgs.RemovedProperty,
                "The PropertyRemovedEventArgs.RemovedProperty must be the Property that was removed");
            Assert.AreSame(
                mRootNode, mLatestPropertyRemovedEventArgs.SourceTreeNode,
                "The PropertyRemovedEventArgs.SourceTreeNode must be the TreeNode from which the Property was removed");
            Assert.AreSame(
                mRootNode, mLatestPropertyRemovedSender,
                "The sender of the PropertyRemoved event must be the TreeNode from which the Property was removed");
        }


        /// <summary>
        /// Tests that changed events bubbles correctly from child to parent TreeNodes
        /// </summary>
        [Test]
        public void changed_bubblesFromChildren()
        {
            TreeNode child = mRootNode.GetChild(0);
            events.DataModelChangedEventArgs childChangedEventArgs = null;
            object childChangedSender = null;
            EventHandler<urakawa.events.DataModelChangedEventArgs> handler = 
                new EventHandler<urakawa.events.DataModelChangedEventArgs>(
                    delegate(object sender, events.DataModelChangedEventArgs e) 
                    { 
                        childChangedEventArgs = e;
                        childChangedSender = sender;
                    });
            child.Changed += handler;
            try
            {
                int beforeCount = mChangedEventCount;
                child.AppendChild(mPresentation.TreeNodeFactory.CreateNode());
                Assert.IsNotNull(childChangedEventArgs, "The changed event of the child does not seem to have occured");
                Assert.AreSame(
                    child, childChangedSender, 
                    "The sender of the changed event on the child must be the child it self");
                Assert.AreEqual(
                    beforeCount + 1, mChangedEventCount,
                    "The mChangedEventCount did not increase by one, indicating that the changed event on the parent/mRootNode TreeNode "
                    + "did not occur as a result of the changed event on the child");
                Assert.AreSame(
                    childChangedEventArgs, mLatestChangedEventArgs,
                    "The event args of the parent/mRootNode changed event was not the same instance as thoose of the child changed evnet");
                Assert.AreSame(
                    mRootNode, mLatestChangedSender,
                    "The sender of the parent/mRootNode changed event must be the parent/mRootNode node itself");

            }
            finally
            {
                child.Changed -= handler;
            }
        }

        /// <summary>
        /// Tests that changed events bubble correctly from owned Properties to owning TreeNodes
        /// </summary>
        [Test]
        public void changed_bubblesfromProperties()
        {
            urakawa.property.xml.XmlProperty xmlProp = mPresentation.PropertyFactory.createXmlProperty();
            mRootNode.AddProperty(xmlProp);
            events.DataModelChangedEventArgs propChangedEventArgs = null;
            object propChangedSender = null;
            EventHandler<urakawa.events.DataModelChangedEventArgs> handler =
                new EventHandler<urakawa.events.DataModelChangedEventArgs>(
                    delegate(object sender, events.DataModelChangedEventArgs e)
                    {
                        propChangedEventArgs = e;
                        propChangedSender = sender;
                    });
            xmlProp.Changed += handler;
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
                xmlProp.Changed -= handler;
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
