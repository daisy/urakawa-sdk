using System;
using NUnit.Framework;
using urakawa.core;

namespace urakawa.unitTests.testbase
{
	/// <summary>
	/// Summary description for TreeTests.
	/// </summary>
	public class TreeTests : TestCollectionBase
	{

		protected bool TestSetup()
		{
			if (mProject == null) return false;
			if (mProject.getPresentation(0) == null) return false;
			if (mProject.getPresentation(0).getRootNode() == null) return false;
			return true;
		}


		[Test]
		public void CopyNode()
		{
			if (!TestSetup()) return;
			TreeNode root = mProject.getPresentation(0).getRootNode();
			TreeNode node_copy = root.copy(true);
			bool equal = root.valueEquals(node_copy);
			Assert.IsTrue(equal, "The copy is not the same as the original");
			Assert.AreNotEqual(root, node_copy, "The copy is just a reference of the original itself");
			foreach (Type propType in node_copy.getListOfUsedPropertyTypes())
			{
				Assert.AreNotEqual(
					root.getProperty(propType), node_copy.getProperty(propType),
					"Property of copy is just a reference to the property of the original");
			}
			for (int i = 0; i < root.getChildCount(); i++)
			{
				Assert.AreNotEqual(
					root.getChild(i), node_copy.getChild(i),
					"Child of copy is just a reference of the child of the original");
			}
		}

		[Test]
		public void InsertNewNodeAsSiblingOfRoot()
		{
			if (!TestSetup()) return;
			TreeNode new_node = mProject.getPresentation(0).getTreeNodeFactory().createNode();
			TreeNode root = mProject.getPresentation(0).getRootNode();
			try
			{
				root.insertAfter(new_node, root);
			}
			catch (exception.NodeDoesNotExistException)
			{
				return;
			}
			Assert.Fail("Expected NodeDoesNotExistException");
		}

		[Test]
		public void AppendChild()
		{
			if (!TestSetup()) return;
			TreeNode new_node = mProject.getPresentation(0).getTreeNodeFactory().createNode();
			TreeNode root = mProject.getPresentation(0).getRootNode();
			root.appendChild(new_node);
			Assert.AreEqual(root.getChildCount() - 1, root.indexOf(new_node), "A newly appended child is at the last index");
		}

		[Test]
		public void AppendSameNodeTwice()
		{
			if (!TestSetup()) return;
			TreeNode root = mProject.getPresentation(0).getRootNode();
			TreeNode new_node = mProject.getPresentation(0).getTreeNodeFactory().createNode();
			root.appendChild(new_node);
			try
			{
				root.appendChild(new_node);
			}
			catch (exception.NodeNotDetachedException)
			{
				return;
			}
			Assert.Fail("Expected NodeNotDetachedException");
		}

		[Test]
		public void InsertSameNodeTwice()
		{
			if (!TestSetup()) return;
			TreeNode root = mProject.getPresentation(0).getRootNode();
			TreeNode new_node = mProject.getPresentation(0).getTreeNodeFactory().createNode();
			root.insert(new_node, 0);
			try
			{
				root.insert(new_node, 0);
			}
			catch (exception.NodeNotDetachedException)
			{
				return;
			}
			Assert.Fail("Expected NodeNotDetachedException");
		}

		[Test]
		public void InsertNullNode()
		{
			if (!TestSetup()) return;
			TreeNode root = mProject.getPresentation(0).getRootNode();
			TreeNode nullNode = null;
			try
			{
				root.insert(nullNode, 0);
			}
			catch (exception.MethodParameterIsNullException)
			{
				return;
			}
			Assert.Fail("Expected MethodParameterIsNullException");
		}

		[Test]
		public void InsertNodeAtIndexBeyondEnd()
		{
			if (!TestSetup()) return;
			TreeNode root = mProject.getPresentation(0).getRootNode();
			root.appendChild(mProject.getPresentation(0).getTreeNodeFactory().createNode());
			root.appendChild(mProject.getPresentation(0).getTreeNodeFactory().createNode());
			root.appendChild(mProject.getPresentation(0).getTreeNodeFactory().createNode());
			try
			{
				root.insert(mProject.getPresentation(0).getTreeNodeFactory().createNode(), root.getChildCount() + 2);
			}
			catch (exception.MethodParameterIsOutOfBoundsException)
			{
				return;
			}
			Assert.Fail("Expected MethodParameterIsOutOfBoundsException");
		}

		[Test]
		public void InsertNodeAtNegativeIndex()
		{
			if (!TestSetup()) return;
			TreeNode root = mProject.getPresentation(0).getRootNode();
			try
			{
				root.insert(mProject.getPresentation(0).getTreeNodeFactory().createNode(), -1);
			}
			catch (exception.MethodParameterIsOutOfBoundsException)
			{
				return;
			}
			Assert.Fail("Expected MethodParameterIsOutOfBoundsException");
		}

		[Test]
		public void GetChildCount()
		{
			if (!TestSetup()) return;
			TreeNode root = mProject.getPresentation(0).getRootNode();
			int initCount = root.getChildCount();
			root.appendChild(mProject.getPresentation(0).getTreeNodeFactory().createNode());
			Assert.AreEqual(initCount+1, root.getChildCount(), "Child count should increase by one when appending a child");
			root.appendChild(mProject.getPresentation(0).getTreeNodeFactory().createNode());
			root.appendChild(mProject.getPresentation(0).getTreeNodeFactory().createNode());
			for (int index=0; index<root.getChildCount(); index++)
			{
				Assert.IsNotNull(root.getChild(index), String.Format("No child at index {0:0} that is within bounds", index));
			}
			initCount = root.getChildCount();
			root.removeChild(0);
			Assert.AreEqual(initCount-1, root.getChildCount(), "Child count should decrease by one when removing a child");
		}

		[Test]
		public void InsertAfter()
		{
			if (!TestSetup()) return;
			TreeNode root = mProject.getPresentation(0).getRootNode();
			root.appendChild(mProject.getPresentation(0).getTreeNodeFactory().createNode());
			root.appendChild(mProject.getPresentation(0).getTreeNodeFactory().createNode());
			root.appendChild(mProject.getPresentation(0).getTreeNodeFactory().createNode());
			int index = root.getChildCount() / 2;
			TreeNode anchorNode = root.getChild(index);
			TreeNode newNode = mProject.getPresentation(0).getTreeNodeFactory().createNode();
			root.insertAfter(newNode, anchorNode);
			int newIndex = root.indexOf(newNode);
			Assert.AreEqual(index + 1, newIndex);
		}

		[Test]
		public void InsertAfterNonChildAnchor()
		{
			if (!TestSetup()) return;
			TreeNode root = mProject.getPresentation(0).getRootNode();
			TreeNode newNode = mProject.getPresentation(0).getTreeNodeFactory().createNode();
			TreeNode rootChild = mProject.getPresentation(0).getTreeNodeFactory().createNode();
			TreeNode anchorNode = mProject.getPresentation(0).getTreeNodeFactory().createNode();
			root.appendChild(rootChild);
			rootChild.appendChild(anchorNode);
			try
			{
				root.insertAfter(anchorNode, newNode);
			}
			catch (exception.NodeDoesNotExistException)
			{
				return;
			}
			Assert.Fail("Expected NodeDoesNotExistException");
		}

		[Test]
		public void InsertBefore()
		{
			if (!TestSetup()) return;
			TreeNode root = mProject.getPresentation(0).getRootNode();
			root.appendChild(mProject.getPresentation(0).getTreeNodeFactory().createNode());
			root.appendChild(mProject.getPresentation(0).getTreeNodeFactory().createNode());
			root.appendChild(mProject.getPresentation(0).getTreeNodeFactory().createNode());
			int index = root.getChildCount() / 2;
			TreeNode anchorNode = root.getChild(index);
			TreeNode newNode = mProject.getPresentation(0).getTreeNodeFactory().createNode();
			root.insertBefore(newNode, anchorNode);
			Assert.AreEqual(index, root.indexOf(newNode));
		}

		[Test]
		public void InsertBeforeNonChildAnchor()
		{
			if (!TestSetup()) return;
			TreeNode root = mProject.getPresentation(0).getRootNode();
			TreeNode newNode = mProject.getPresentation(0).getTreeNodeFactory().createNode();
			TreeNode rootChild = mProject.getPresentation(0).getTreeNodeFactory().createNode();
			TreeNode anchorNode = mProject.getPresentation(0).getTreeNodeFactory().createNode();
			root.appendChild(rootChild);
			rootChild.appendChild(anchorNode);
			try
			{
				root.insertBefore(anchorNode, newNode);
			}
			catch (exception.NodeDoesNotExistException)
			{
				return;
			}
			Assert.Fail("Expected NodeDoesNotExistException");
		}

		[Test]
		public void Detach()
		{
			if (!TestSetup()) return;
			TreeNode root = mProject.getPresentation(0).getRootNode();
			TreeNode newNode = mProject.getPresentation(0).getTreeNodeFactory().createNode();
			root.appendChild(newNode);
			newNode.detach();
			Assert.IsNull(newNode.getParent(), "Parent of detached child must be null");
			try
			{
				root.indexOf(newNode);
			}
			catch (exception.NodeDoesNotExistException)
			{
				return;
			}
			Assert.Fail("Expected NodeDoesNotExistException");
		}

		[Test]
		public void RemoveChild()
		{
			if (!TestSetup()) return;
			TreeNode root = mProject.getPresentation(0).getRootNode();
			TreeNode newNode = mProject.getPresentation(0).getTreeNodeFactory().createNode();
			root.appendChild(newNode);
			root.removeChild(newNode);
			Assert.IsNull(newNode.getParent(), "Parent of removed child must be null");
			try
			{
				root.indexOf(newNode);
			}
			catch (exception.NodeDoesNotExistException)
			{
				return;
			}
			Assert.Fail("Expected NodeDoesNotExistException");
		}

		[Test]
		public void RemoveChildThatIsNotChild()
		{
			if (!TestSetup()) return;
			TreeNode root = mProject.getPresentation(0).getRootNode();
			TreeNode rootChild = mProject.getPresentation(0).getTreeNodeFactory().createNode();
			root.appendChild(rootChild);
			TreeNode newNode = mProject.getPresentation(0).getTreeNodeFactory().createNode();
			rootChild.appendChild(newNode);
			try
			{
				root.removeChild(newNode);
			}
			catch (exception.NodeDoesNotExistException)
			{
				return;
			}
			Assert.Fail("Expected NodeDoesNotExistException");
		}

		[Test]
		public void IndexOf()
		{
			if (!TestSetup()) return;
			TreeNode root = mProject.getPresentation(0).getRootNode();
			root.appendChild(mProject.getPresentation(0).getTreeNodeFactory().createNode());
			root.appendChild(mProject.getPresentation(0).getTreeNodeFactory().createNode());
			root.appendChild(mProject.getPresentation(0).getTreeNodeFactory().createNode());
			for (int index = 0; index < root.getChildCount(); index++)
			{
				Assert.AreEqual(index, root.indexOf(root.getChild(index)));
			}
		}

		[Test]
		public void IndexOfNonChild()
		{
			if (!TestSetup()) return;
			TreeNode root = mProject.getPresentation(0).getRootNode();
			TreeNode rootChild = mProject.getPresentation(0).getTreeNodeFactory().createNode();
			root.appendChild(rootChild);
			TreeNode newNode = mProject.getPresentation(0).getTreeNodeFactory().createNode();
			rootChild.appendChild(newNode);
			try
			{
				root.indexOf(newNode);
			}
			catch (exception.NodeDoesNotExistException)
			{
				return;
			}
			Assert.Fail("Expected NodeDoesNotExistException");
		}

		[Test]
		public void ReplaceChild()
		{
			if (!TestSetup()) return;
			TreeNode root = mProject.getPresentation(0).getRootNode();
			TreeNode rootChild = mProject.getPresentation(0).getTreeNodeFactory().createNode();
			root.appendChild(mProject.getPresentation(0).getTreeNodeFactory().createNode());
			root.appendChild(mProject.getPresentation(0).getTreeNodeFactory().createNode());
			root.appendChild(mProject.getPresentation(0).getTreeNodeFactory().createNode());
			root.appendChild(mProject.getPresentation(0).getTreeNodeFactory().createNode());
			root.appendChild(rootChild);
			root.appendChild(mProject.getPresentation(0).getTreeNodeFactory().createNode());
			root.appendChild(mProject.getPresentation(0).getTreeNodeFactory().createNode());
			int index = root.indexOf(rootChild);
			TreeNode newNode = mProject.getPresentation(0).getTreeNodeFactory().createNode();
			root.replaceChild(newNode, rootChild);
			Assert.AreEqual(newNode, root.getChild(index));
		}

		[Test]
		public void ReplaceNonChild()
		{
			if (!TestSetup()) return;
			TreeNode root = mProject.getPresentation(0).getRootNode();
			TreeNode rootChild = mProject.getPresentation(0).getTreeNodeFactory().createNode();
			root.appendChild(mProject.getPresentation(0).getTreeNodeFactory().createNode());
			root.appendChild(mProject.getPresentation(0).getTreeNodeFactory().createNode());
			root.appendChild(mProject.getPresentation(0).getTreeNodeFactory().createNode());
			root.appendChild(mProject.getPresentation(0).getTreeNodeFactory().createNode());
			root.appendChild(rootChild);
			root.appendChild(mProject.getPresentation(0).getTreeNodeFactory().createNode());
			root.appendChild(mProject.getPresentation(0).getTreeNodeFactory().createNode());
			TreeNode newNode = mProject.getPresentation(0).getTreeNodeFactory().createNode();
			TreeNode nonChildNode = mProject.getPresentation(0).getTreeNodeFactory().createNode();
			rootChild.appendChild(nonChildNode);
			try
			{
				root.replaceChild(newNode, nonChildNode);
			}
			catch (exception.NodeDoesNotExistException)
			{
				return;
			}
			Assert.Fail("Expected NodeDoesNotExistException");
		}
	}
}
