using System;
using NUnit.Framework;

namespace urakawa.test.unitTests
{
	/// <summary>
	/// this is a class library to be used with NUnit unit testing software
	/// </summary>
	[TestFixture] 
	public class AfterOpeningXUK
	{
		private urakawa.xuk.Project mProject;
		private string mDefaultFile = "../XukWorks/sample.xuk";

		[SetUp] public void Init() 
		{
			mProject = new urakawa.xuk.Project();
			//find the dll's directory
			string filepath = System.IO.Path.GetDirectoryName
				(System.Reflection.Assembly.GetEntryAssembly().Location );
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
	}
}
