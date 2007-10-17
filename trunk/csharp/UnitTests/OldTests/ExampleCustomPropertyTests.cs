using System;
using System.IO;
using System.Xml;
using NUnit.Framework;
using urakawa.core;
using urakawa.examples;

namespace urakawa.unitTests.fixtures.examples
{
	/// <summary>
	/// Tests for <see cref="ExampleCustomProperty"/> and <see cref="ExampleCustomTreeNode"/>
	/// </summary>
	[TestFixture]//, Ignore("Since implementation of multiple Presentations per Project, XukIn of custon stuff does not work")] 
	public class ExampleCustomTests
	{
		private Project mProject;
		private string mDefaultFile = "../XukWorks/ExCustTestSample.xuk";


		ExampleCustomTreeNode node1, node1_1, node1_2, node1_2_1, node1_2_2, node2, node3, node3_1, node3_2, node3_3, node4, node4_1, node4_2;
		urakawa.navigation.TypeFilterNavigator<ExampleCustomTreeNode> navigator;

		public ExampleCustomTests()
		{
		}

		[SetUp] public void Init() 
		{
			string filepath = Directory.GetCurrentDirectory();

			Uri fileUri = new Uri(filepath);
			
			fileUri = new Uri(fileUri, mDefaultFile);
			mProject = new Project();
			mProject.setDataModelFactory(new ExampleCustomDataModelFactory());
			mProject.openXUK(fileUri);

			
			navigator = new urakawa.navigation.TypeFilterNavigator<ExampleCustomTreeNode>();
			TreeNode root = mProject.getPresentation(0).getRootNode();
			node1 = navigator.getNext(root);
			Assert.IsNotNull(node1, "Node 1 not found");
			Assert.AreEqual(node1.Label, "1", "Label of node 1 is not '1'");
			node1_1 = navigator.getNext(node1);
			Assert.IsNotNull(node1_1, "Node 1.1 not found");
			Assert.AreEqual(node1_1.Label, "1.1", "Label of node 1.1 is not '1.1'");
			node1_2 = navigator.getNext(node1_1);
			Assert.IsNotNull(node1_2, "Node 1.2 not found");
			Assert.AreEqual(node1_2.Label, "1.2", "Label of node 1.2 is not '1.2'");
			node1_2_1 = navigator.getNext(node1_2);
			Assert.IsNotNull(node1_2_1, "Node 1.2.1 not found");
			Assert.AreEqual(node1_2_1.Label, "1.2.1", "Label of node 1.2.1 is not '1.2.1'");
			node1_2_2 = navigator.getNext(node1_2_1);
			Assert.IsNotNull(node1_2_2, "Node 1.2.2 not found");
			Assert.AreEqual(node1_2_2.Label, "1.2.2", "Label of node 1.2.2 is not '1.2.2'");
			node2 = navigator.getNext(node1_2_2);
			Assert.IsNotNull(node2, "Node 2 not found");
			Assert.AreEqual(node2.Label, "2", "Label of node 2 is not '2'");
			node3 = navigator.getNext(node2);
			Assert.IsNotNull(node3, "Node 3 not found");
			Assert.AreEqual(node3.Label, "3", "Label of node 3 is not '3'");
			node3_1 = navigator.getNext(node3);
			Assert.IsNotNull(node3_1, "Node 3.1 not found");
			Assert.AreEqual(node3_1.Label, "3.1", "Label of node 3.1 is not '3.1'");
			node3_2 = navigator.getNext(node3_1);
			Assert.IsNotNull(node3_2, "Node 3.2 not found");
			Assert.AreEqual(node3_2.Label, "3.2", "Label of node 3.2 is not '3.2'");
			node3_3 = navigator.getNext(node3_2);
			Assert.IsNotNull(node3_3, "Node 3.3 not found");
			Assert.AreEqual(node3_3.Label, "3.3", "Label of node 3.3 is not '3.3'");
			node4 = navigator.getNext(node3_3);
			Assert.IsNotNull(node4, "Node 4 not found");
			Assert.AreEqual(node4.Label, "4", "Label of node 4 is not '4'");
			node4_1 = navigator.getNext(node4);
			Assert.IsNotNull(node4_1, "Node 4.1 not found");
			Assert.AreEqual(node4_1.Label, "4.1", "Label of node 4.1 is not '4.1'");
			node4_2 = navigator.getNext(node4_1);
			Assert.IsNotNull(node4_2, "Node 4.2 not found");
			Assert.AreEqual(node4_2.Label, "4.2", "Label of node 4.2 is not '4.2'");
		}

		[Test] public void TestExCustDataLoaded()
		{
			TestRootNodeCustomPropData(mProject);
			TestRootNodeFirstChildCustTreeNodeData(mProject);
		}

		private void TestRootNodeCustomPropData(Project proj)
		{
			ExampleCustomProperty rootExCustProp = 
				(ExampleCustomProperty)proj.getPresentation(0).getRootNode()
				  .getProperty(typeof(ExampleCustomProperty));
			Assert.AreEqual("Test Data", rootExCustProp.CustomData);
		}

		private void TestRootNodeFirstChildCustTreeNodeData(Project proj)
		{
			ExampleCustomTreeNode firstCh = (ExampleCustomTreeNode)proj.getPresentation(0).getRootNode().getChild(0);
			Assert.AreEqual("Test Ex Cust Tree Node Data", firstCh.CustomTreeNodeData);
		}

		[Test] public void TestExCustPropSaved()
		{
			TestRootNodeCustomPropData(mProject);
			TestRootNodeFirstChildCustTreeNodeData(mProject);
			MemoryStream memStream = new MemoryStream();
			XmlTextWriter wr = new XmlTextWriter(memStream, System.Text.Encoding.UTF8);
			mProject.saveXUK(wr, mProject.getPresentation(0).getRootUri());
			wr.Flush();
			wr = null;
			memStream.Position = 0;
			StreamReader srd = new StreamReader(memStream, System.Text.Encoding.UTF8);
			string content = srd.ReadToEnd();
			memStream.Position = 0;
			Project reloadedProject = new Project();
			reloadedProject.setDataModelFactory(new ExampleCustomDataModelFactory());
			XmlTextReader rd = new XmlTextReader(memStream);
			reloadedProject.openXUK(rd);
			rd.Close();
			TestRootNodeCustomPropData(reloadedProject);
			TestRootNodeFirstChildCustTreeNodeData(reloadedProject);
		}

		[Test]
		public void TestTypeFilterNavigator()
		{
			urakawa.navigation.TypeFilterNavigator<ExampleCustomTreeNode> navigator 
				= new urakawa.navigation.TypeFilterNavigator<ExampleCustomTreeNode>();
			TreeNode root = mProject.getPresentation(0).getRootNode();
			ExampleCustomTreeNode nod1 = navigator.getNext(root);
			Assert.IsNotNull(nod1, "Node 1 not found");
			Assert.AreEqual(nod1.Label, "1", "Label of node 1 is not '1'");
			ExampleCustomTreeNode nod1_1 = navigator.getNext(nod1);
			Assert.IsNotNull(nod1_1, "Node 1.1 not found");
			Assert.AreEqual(nod1_1.Label, "1.1", "Label of node 1.1 is not '1.1'");
		}

		[Test]
		public void TestNextSibling()
		{
			Assert.AreSame(navigator.getNextSibling(node1_1), node1_2, "The next sibling of node 1.2 is not node 1.2");
			Assert.AreSame(navigator.getNextSibling(node1_2_1), node1_2_2, "The next sibling of node 1.2.1 is not node 1.2.2");
			Assert.AreSame(navigator.getNextSibling(node3_1), node3_2, "The next sibling of node 3.1 is not node 3.2");
			Assert.AreSame(navigator.getNextSibling(node4_1), node4_2, "The next sibling of node 4.1 is not node 4.2");
			Assert.IsNull(navigator.getNextSibling(node1), "Node 1 unexceptedly has a next sibling");
			Assert.IsNull(navigator.getNextSibling(node2), "Node 2 unexceptedly has a next sibling");
			Assert.IsNull(navigator.getNextSibling(node3), "Node 3 unexceptedly has a next sibling");
			Assert.IsNull(navigator.getNextSibling(node4), "Node 4 unexceptedly has a next sibling");
		}

		[Test]
		public void TestPreviousSibling()
		{
			Assert.AreSame(node1_1, navigator.getPreviousSibling(node1_2), "The previous sibling of node 1.2 is not node 1.1");
			Assert.AreSame(node1_2_1, navigator.getPreviousSibling(node1_2_2), "The previous sibling of node 1.2.2 is not node 1.2.1");
			Assert.AreSame(node3_1, navigator.getPreviousSibling(node3_2), "The previous sibling of node 3.2 is not node 3.1");
			Assert.AreSame(node4_1, navigator.getPreviousSibling(node4_2), "The previous sibling of node 4.2 is not node 4.1");
			Assert.IsNull(navigator.getPreviousSibling(node4), "Node 4 unexpectedly has a previous sibling");
			Assert.IsNull(navigator.getPreviousSibling(node3), "Node 3 unexpectedly has a previous sibling");
			Assert.IsNull(navigator.getPreviousSibling(node2), "Node 2 unexpectedly has a previous sibling");
			Assert.IsNull(navigator.getPreviousSibling(node1), "Node 1 unexpectedly has a previous sibling");
			Assert.IsNull(navigator.getPreviousSibling(node1_1), "Node 1.1 unexpectedly has a previous sibling");
			Assert.IsNull(navigator.getPreviousSibling(node1_2_1), "Node 1.2.1 unexpectedly has a previous sibling");
			Assert.IsNull(navigator.getPreviousSibling(node3_1), "Node 3.1 unexpectedly has a previous sibling");
			Assert.IsNull(navigator.getPreviousSibling(node4_1), "Node 4.1 unexpectedly has a previous sibling");
		}

		[Test]
		public void TestPrevious()
		{
			Assert.AreSame(node4_1, navigator.getPrevious(node4_2), "Previous of node 4.2 os not node 4.1");
			Assert.AreSame(node4, navigator.getPrevious(node4_1), "Previous of node 4.1 os not node 4");
			Assert.AreSame(node3_3, navigator.getPrevious(node4), "Previous of node 4 os not node 3.3");
			Assert.AreSame(node3_2, navigator.getPrevious(node3_3), "Previous of node 3.3 os not node 3.2");
			Assert.AreSame(node3_1, navigator.getPrevious(node3_2), "Previous of node 3.2 os not node 3.1");
			Assert.AreSame(node3, navigator.getPrevious(node3_1), "Previous of node 3.1 os not node 3");
			Assert.AreSame(node2, navigator.getPrevious(node3), "Previous of node 3 os not node 2");
			Assert.AreSame(node1_2_2, navigator.getPrevious(node2), "Previous of node 2 os not node 1.2.2");
			Assert.AreSame(node1_2_1, navigator.getPrevious(node1_2_2), "Previous of node 1.2.2 os not node 1.2.1");
			Assert.AreSame(node1_2, navigator.getPrevious(node1_2_1), "Previous of node 1.2.1 os not node 1.2");
			Assert.AreSame(node1_1, navigator.getPrevious(node1_2), "Previous of node 1.2 os not node 1.1");
			Assert.AreSame(node1, navigator.getPrevious(node1_1), "Previous of node 1.1 os not node 1");
			Assert.IsNull(navigator.getPrevious(node1), "The previous of node 1 is not null");
		}

		[Test]
		public void TestCopy()
		{
			TreeNode node1Copy = node1.copy();
			bool ve = node1.valueEquals(node1Copy);
			Assert.IsTrue(ve, "Node 1 and it's copy does not have the same value");
		}
	}
}
