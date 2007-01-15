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
			mProject = new Project();
			
			string filepath = System.IO.Directory.GetCurrentDirectory();

			Uri fileUri = new Uri(filepath);
			
			fileUri = new Uri(fileUri, mDefaultFile);
			bool loadSuccess = mProject.openXUK(fileUri);
			Assert.IsTrue(loadSuccess, "Failed to load XUK file {0}", mDefaultFile);
		}
	}
}
