using System;
using NUnit.Framework;
using urakawa.core;

namespace urakawa.test.unitTests
{
	/// <summary>
	/// Summary description for VisitorTests.
	/// </summary>
	[TestFixture]
	public class VisitorTests
	{
		private urakawa.project.Project mProject;
		private string mDefaultFile = "../../XukWorks/SampleDTB2Ver1.xuk";

		[SetUp] public void Init() 
		{
			mProject = new urakawa.project.Project();
			
			string filepath = System.IO.Directory.GetCurrentDirectory();

			Uri fileUri = new Uri(filepath);
			
			fileUri = new Uri(fileUri, mDefaultFile);
			
			Assert.IsTrue(mProject.openXUK(fileUri), "Failed to load XUK file {0}", mDefaultFile);
		}

		[Test] public void VisitXmlPropertyName()
		{
			XmlPropertyElementNameVisitor vis = new XmlPropertyElementNameVisitor();

			vis.addElementName("level");

			mProject.getPresentation().getRootNode().acceptDepthFirst(vis);

			System.Collections.ArrayList list = (System.Collections.ArrayList)vis.getResults();

			Assert.IsNotNull(list);

			//this particular file (SampleDTB2Ver1.xuk) has one level node
			Assert.AreEqual(1, list.Count);
		}

	}
}
