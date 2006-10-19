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
			if (mProject.getPresentation() == null)
			{
				return false;
			}
			if (mProject.getPresentation().getRootNode() == null)
			{
				return false;
			}
			return true;
		}


		[Test]
		public void CopyNode()
		{
			if (!TestSetup()) return;
			CoreNode root = mProject.getPresentation().getRootNode();
			Console.WriteLine("Testing deep copying:");
			CoreNode node_copy = root.copy(true);
			bool equal = CoreNode.areCoreNodesEqual(root, node_copy, true);
			Assert.IsTrue(equal, "The copy is not the same as the original");
			Assert.AreNotEqual(root, node_copy, "The copy is just a reference of the original itself");
			foreach (Type propType in node_copy.getUsedPropertyTypes())
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
		[ExpectedException(typeof(exception.NodeDoesNotExistException))]
		public void InsertNewNodeAsSiblingOfRoot()
		{
			if (!TestSetup()) return;
			CoreNode new_node = mProject.getPresentation().getCoreNodeFactory().createNode();
			CoreNode root = mProject.getPresentation().getRootNode();

			root.insertAfter(new_node, root);
		}

		[Test]
		public void AppendChild()
		{
			if (!TestSetup()) return;
			CoreNode new_node = mProject.getPresentation().getCoreNodeFactory().createNode();
			CoreNode root = mProject.getPresentation().getRootNode();
			root.appendChild(new_node);
			Assert.AreEqual(root.getChildCount() - 1, root.indexOf(new_node), "A newly appended child is at the last index");
		}

		[Test]
		[ExpectedException(typeof(exception.NodeNotDetachedException))]
		public void AppendSameNodeTwice()
		{
			if (!TestSetup()) return;
			CoreNode root = mProject.getPresentation().getRootNode();
			CoreNode new_node = mProject.getPresentation().getCoreNodeFactory().createNode();
			root.appendChild(new_node);
			root.appendChild(new_node);
		}

		[Test]
		[ExpectedException(typeof(exception.NodeNotDetachedException))]
		public void InsertSameNodeTwice()
		{
			if (!TestSetup()) return;
			CoreNode root = mProject.getPresentation().getRootNode();
			CoreNode new_node = mProject.getPresentation().getCoreNodeFactory().createNode();
			root.insert(new_node, 0);
			root.insert(new_node, 0);
		}

		[Test]
		[ExpectedException(typeof(exception.MethodParameterIsNullException))]
		public void InsertNullNode()
		{
			if (!TestSetup()) return;
			CoreNode root = mProject.getPresentation().getRootNode();
			CoreNode nullNode = null;
			root.insert(nullNode, 0);
		}

		[Test]
		[ExpectedException(typeof(exception.MethodParameterIsOutOfBoundsException))]
		public void InsertNodeAtIndexBeyondEnd()
		{
			if (!TestSetup()) return;
			CoreNode root = mProject.getPresentation().getRootNode();
			root.appendChild(mProject.getPresentation().getCoreNodeFactory().createNode());
			root.appendChild(mProject.getPresentation().getCoreNodeFactory().createNode());
			root.appendChild(mProject.getPresentation().getCoreNodeFactory().createNode());
			root.insert(mProject.getPresentation().getCoreNodeFactory().createNode(), root.getChildCount() + 2);
		}

		[Test]
		[ExpectedException(typeof(exception.MethodParameterIsOutOfBoundsException))]
		public void InsertNodeAtNegativeIndex()
		{
			if (!TestSetup()) return;
			CoreNode root = mProject.getPresentation().getRootNode();
			root.insert(mProject.getPresentation().getCoreNodeFactory().createNode(), -1);
		}

		[Test]
		public void GetChildCount()
		{
			if (!TestSetup()) return;
			CoreNode root = mProject.getPresentation().getRootNode();
			int initCount = root.getChildCount();
			root.appendChild(mProject.getPresentation().getCoreNodeFactory().createNode());
			Assert.AreEqual(initCount+1, root.getChildCount(), "Child count should increase by one when appending a child");
			root.appendChild(mProject.getPresentation().getCoreNodeFactory().createNode());
			root.appendChild(mProject.getPresentation().getCoreNodeFactory().createNode());
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
			CoreNode root = mProject.getPresentation().getRootNode();
			root.appendChild(mProject.getPresentation().getCoreNodeFactory().createNode());
			root.appendChild(mProject.getPresentation().getCoreNodeFactory().createNode());
			root.appendChild(mProject.getPresentation().getCoreNodeFactory().createNode());
			int index = root.getChildCount() / 2;
			CoreNode anchorNode = (CoreNode)root.getChild(index);
			CoreNode newNode = mProject.getPresentation().getCoreNodeFactory().createNode();
			root.insertAfter(newNode, anchorNode);
			Assert.AreEqual(index + 1, root.indexOf(newNode));
		}

		[Test]
		[ExpectedException(typeof(exception.NodeDoesNotExistException))]
		public void InsertAfterNonChildAnchor()
		{
			if (!TestSetup()) return;
			CoreNode root = mProject.getPresentation().getRootNode();
			CoreNode newNode = mProject.getPresentation().getCoreNodeFactory().createNode();
			CoreNode rootChild = mProject.getPresentation().getCoreNodeFactory().createNode();
			CoreNode anchorNode = mProject.getPresentation().getCoreNodeFactory().createNode();
			root.appendChild(rootChild);
			rootChild.appendChild(anchorNode);
			root.insertAfter(anchorNode, newNode);
		}

		[Test]
		public void InsertBefore()
		{
			if (!TestSetup()) return;
			CoreNode root = mProject.getPresentation().getRootNode();
			root.appendChild(mProject.getPresentation().getCoreNodeFactory().createNode());
			root.appendChild(mProject.getPresentation().getCoreNodeFactory().createNode());
			root.appendChild(mProject.getPresentation().getCoreNodeFactory().createNode());
			int index = root.getChildCount() / 2;
			CoreNode anchorNode = (CoreNode)root.getChild(index);
			CoreNode newNode = mProject.getPresentation().getCoreNodeFactory().createNode();
			root.insertBefore(newNode, anchorNode);
			Assert.AreEqual(index, root.indexOf(newNode));
		}

		[Test]
		[ExpectedException(typeof(exception.NodeDoesNotExistException))]
		public void InsertBeforeNonChildAnchor()
		{
			if (!TestSetup()) return;
			CoreNode root = mProject.getPresentation().getRootNode();
			CoreNode newNode = mProject.getPresentation().getCoreNodeFactory().createNode();
			CoreNode rootChild = mProject.getPresentation().getCoreNodeFactory().createNode();
			CoreNode anchorNode = mProject.getPresentation().getCoreNodeFactory().createNode();
			root.appendChild(rootChild);
			rootChild.appendChild(anchorNode);
			root.insertBefore(anchorNode, newNode);
		}

		[Test]
		[ExpectedException(typeof(exception.NodeDoesNotExistException))]
		public void Detach()
		{
			if (!TestSetup()) return;
			CoreNode root = mProject.getPresentation().getRootNode();
			CoreNode newNode = mProject.getPresentation().getCoreNodeFactory().createNode();
			root.appendChild(newNode);
			newNode.detach();
			Assert.IsNull(newNode.getParent(), "Parent of detached child must be null");
			root.indexOf(newNode);
		}

		[Test]
		[ExpectedException(typeof(exception.NodeDoesNotExistException))]
		public void RemoveChild()
		{
			if (!TestSetup()) return;
			CoreNode root = mProject.getPresentation().getRootNode();
			CoreNode newNode = mProject.getPresentation().getCoreNodeFactory().createNode();
			root.appendChild(newNode);
			root.removeChild(newNode);
			Assert.IsNull(newNode.getParent(), "Parent of removed child must be null");
			root.indexOf(newNode);
		}

		[Test]
		[ExpectedException(typeof(exception.NodeDoesNotExistException))]
		public void RemoveChildThatIsNotChild()
		{
			if (!TestSetup()) return;
			CoreNode root = mProject.getPresentation().getRootNode();
			CoreNode rootChild = mProject.getPresentation().getCoreNodeFactory().createNode();
			root.appendChild(rootChild);
			CoreNode newNode = mProject.getPresentation().getCoreNodeFactory().createNode();
			rootChild.appendChild(newNode);
			root.removeChild(newNode);
		}

		[Test]
		public void IndexOf()
		{
			if (!TestSetup()) return;
			CoreNode root = mProject.getPresentation().getRootNode();
			root.appendChild(mProject.getPresentation().getCoreNodeFactory().createNode());
			root.appendChild(mProject.getPresentation().getCoreNodeFactory().createNode());
			root.appendChild(mProject.getPresentation().getCoreNodeFactory().createNode());
			for (int index = 0; index < root.getChildCount(); index++)
			{
				Assert.AreEqual(index, root.indexOf(root.getChild(index)));
			}
		}

		[Test]
		[ExpectedException(typeof(exception.NodeDoesNotExistException))]
		public void IndexOfNonChild()
		{
			if (!TestSetup()) return;
			CoreNode root = mProject.getPresentation().getRootNode();
			CoreNode rootChild = mProject.getPresentation().getCoreNodeFactory().createNode();
			root.appendChild(rootChild);
			CoreNode newNode = mProject.getPresentation().getCoreNodeFactory().createNode();
			rootChild.appendChild(newNode);
			root.indexOf(newNode);
		}

		[Test]
		public void ReplaceChild()
		{
			if (!TestSetup()) return;
			CoreNode root = mProject.getPresentation().getRootNode();
			CoreNode rootChild = mProject.getPresentation().getCoreNodeFactory().createNode();
			root.appendChild(mProject.getPresentation().getCoreNodeFactory().createNode());
			root.appendChild(mProject.getPresentation().getCoreNodeFactory().createNode());
			root.appendChild(mProject.getPresentation().getCoreNodeFactory().createNode());
			root.appendChild(mProject.getPresentation().getCoreNodeFactory().createNode());
			root.appendChild(rootChild);
			root.appendChild(mProject.getPresentation().getCoreNodeFactory().createNode());
			root.appendChild(mProject.getPresentation().getCoreNodeFactory().createNode());
			int index = root.indexOf(rootChild);
			CoreNode newNode = mProject.getPresentation().getCoreNodeFactory().createNode();
			root.replaceChild(newNode, rootChild);
			Assert.AreEqual(newNode, root.getChild(index));
		}

		[Test]
		[ExpectedException(typeof(exception.NodeDoesNotExistException))]
		public void ReplaceNonChild()
		{
			if (!TestSetup()) return;
			CoreNode root = mProject.getPresentation().getRootNode();
			CoreNode rootChild = mProject.getPresentation().getCoreNodeFactory().createNode();
			root.appendChild(mProject.getPresentation().getCoreNodeFactory().createNode());
			root.appendChild(mProject.getPresentation().getCoreNodeFactory().createNode());
			root.appendChild(mProject.getPresentation().getCoreNodeFactory().createNode());
			root.appendChild(mProject.getPresentation().getCoreNodeFactory().createNode());
			root.appendChild(rootChild);
			root.appendChild(mProject.getPresentation().getCoreNodeFactory().createNode());
			root.appendChild(mProject.getPresentation().getCoreNodeFactory().createNode());
			CoreNode newNode = mProject.getPresentation().getCoreNodeFactory().createNode();
			CoreNode nonChildNode = mProject.getPresentation().getCoreNodeFactory().createNode();
			rootChild.appendChild(nonChildNode);
			root.replaceChild(newNode, nonChildNode);
		}
	}
}
