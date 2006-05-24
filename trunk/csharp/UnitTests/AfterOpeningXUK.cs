using System;
using NUnit.Framework;
using urakawa.core;

namespace urakawa.test.unitTests
{
	/// <summary>
	/// this is a class library to be used with NUnit unit testing software
	/// </summary>
	[TestFixture] 
	public class AfterOpeningXUK
	{
		private urakawa.xuk.Project mProject;
		private string mDefaultFile = "../XukWorks/simplesample.xuk";

		[SetUp] public void Init() 
		{
			mProject = new urakawa.xuk.Project();
			
			string filepath = System.IO.Directory.GetCurrentDirectory();

			Uri fileUri = new Uri(filepath);
			
			fileUri = new Uri(fileUri, mDefaultFile);
			
			mProject.openXUK(fileUri);
		}

		[Test] public void IsPresentationNull()
		{
			Assert.IsNotNull(mProject.getPresentation());
		}

		[Test] public void IsRootNodeNull()
		{
			if (mProject.getPresentation() != null)
			{
				Assert.IsNotNull(mProject.getPresentation().getRootNode());
			}
		}
		[Test] public void IsChannelsManagerNull()
		{
			if (mProject.getPresentation() != null)
			{
				Assert.IsNotNull(mProject.getPresentation().getChannelsManager());
			}
		}
		[Test] public void CopyNode()
		{
			if (mProject.getPresentation() != null)
			{
				CoreNode root = mProject.getPresentation().getRootNode();
				CoreNode node_copy = root.copy(true);

				Assert.AreEqual(root, node_copy);
			}
		}
	}
}
