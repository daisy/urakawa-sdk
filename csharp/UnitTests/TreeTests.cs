using System;
using NUnit.Framework;
using urakawa.core;

namespace urakawa.test.unitTests
{
	/// <summary>
	/// Summary description for TreeTests.
	/// </summary>
	[TestFixture] 
	public class TreeTests
	{
		private urakawa.project.Project mProject;
		private string mDefaultFile = "../XukWorks/simplesample.xuk";

		[SetUp] public void Init() 
		{
			mProject = new urakawa.project.Project();
			
			string filepath = System.IO.Directory.GetCurrentDirectory();

			Uri fileUri = new Uri(filepath);
			
			fileUri = new Uri(fileUri, mDefaultFile);
			
			bool openSucces = mProject.openXUK(fileUri);
			Assert.IsTrue(openSucces, String.Format("Could not open xuk file {0}", mDefaultFile));
		}

		[Test] public void CopyNode()
		{
			if (mProject.getPresentation() != null)
			{
				CoreNode root = mProject.getPresentation().getRootNode();
				Console.WriteLine("Testing deep copying:");
				CoreNode node_copy = root.copy(true);
				Assert.IsTrue(CoreNode.areCoreNodesEqual(root, node_copy, true), "The copy is just a reference of the CoreNode itself");

			}
		}

		[Test] 
		[ExpectedException(typeof(exception.NodeDoesNotExistException))]
		public void InsertNewNodeAsSiblingOfRoot()
		{
			if (mProject.getPresentation() == null)
			{
				return;
			}
			CoreNode new_node = mProject.getPresentation().getCoreNodeFactory().createNode();
			CoreNode root = mProject.getPresentation().getRootNode();
			if (root == null)
			{
				return;
			}

			root.insertAfter(new_node, root);
		}

		[Test] public void AddChildToRoot()
		{
			if (mProject.getPresentation() == null)
			{
				return;
			}
			CoreNode new_node = mProject.getPresentation().getCoreNodeFactory().createNode();
			CoreNode root = mProject.getPresentation().getRootNode();
			if (root == null)
			{
				return;
			}
			
			root.appendChild(new_node);

			Assert.AreEqual(root.getChildCount(), 1, "root has wrong number of children");
			Assert.AreEqual(root.getChild(0), new_node, "root has wrong child");

		}

	}
}
