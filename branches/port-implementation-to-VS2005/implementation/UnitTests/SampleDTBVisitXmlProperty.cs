using System;
using NUnit.Framework;
using urakawa.core;
using urakawa.unitTests.testbase;

namespace urakawa.unitTests.fixtures.xukfiles.sampledtb
{
	/// <summary>
	/// Summary description for SampleDTBVisitXmlProperty.
	/// </summary>
	[TestFixture]
	public class SampleDTBVisitXmlProperty : XmlPropertyVisitorTests
	{
		private string mDefaultFile = "../XukWorks/SampleDTB2Ver1.xuk";

		[SetUp] public void Init() 
		{
			mProject = new urakawa.project.Project();
			
			string filepath = System.IO.Directory.GetCurrentDirectory();

			Uri fileUri = new Uri(filepath);
			
			fileUri = new Uri(fileUri, mDefaultFile);
			
			Assert.IsTrue(mProject.openXUK(fileUri), "Failed to load XUK file {0}", mDefaultFile);
		}
	}
}
