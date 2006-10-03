using System;
using System.IO;
using System.Xml;
using NUnit.Framework;
using urakawa.core;
using urakawa.examples;

namespace urakawa.unitTests.fixtures.examples
{
	/// <summary>
	/// Tests for <see cref="ExampleCustomProperty"/>
	/// </summary>
	[TestFixture] public class ExampleCustomPropertyTests
	{
		private urakawa.project.Project mProject;
		private string mDefaultFile = "../XukWorks/ExCustPropTestSample.xuk";

		public ExampleCustomPropertyTests()
		{
		}

		[SetUp] public void Init() 
		{
			mProject = new urakawa.project.Project();
			mProject.getPresentation().setPropertyFactory(
				new ExampleCustomPropertyFactory());
			string filepath = Directory.GetCurrentDirectory();

			Uri fileUri = new Uri(filepath);
			
			fileUri = new Uri(fileUri, mDefaultFile);
			
			Assert.IsTrue(mProject.openXUK(fileUri), "Failed to load XUK file {0}", mDefaultFile);
		}

		[Test] public void TestExCustPropLoaded()
		{
			TestRootNodeCustomData(mProject);
		}

		private void TestRootNodeCustomData(urakawa.project.Project proj)
		{
			ExampleCustomProperty rootExCustProp = 
				(ExampleCustomProperty)proj.getPresentation().getRootNode()
				  .getProperty(typeof(ExampleCustomProperty));
			Assert.AreEqual("Test Data", rootExCustProp.CustomData);
		}

		[Test] public void TestExCustPropSaved()
		{
			MemoryStream memStream = new MemoryStream();
			XmlTextWriter wr = new XmlTextWriter(memStream, System.Text.Encoding.UTF8);
			Assert.IsTrue(mProject.saveXUK(wr), "failed to write project to memory stream");
			wr.Flush();
			Presentation reloadedPresentation = new Presentation();
			wr = null;
			memStream.Position = 0;
			urakawa.project.Project reloadedProject = new urakawa.project.Project();
			reloadedProject.getPresentation().setPropertyFactory(
				new ExampleCustomPropertyFactory());
			XmlTextReader rd = new XmlTextReader(memStream);
			Assert.IsTrue(
				reloadedProject.openXUK(rd),
				"Failed to reopen project");
			rd.Close();
			TestRootNodeCustomData(reloadedProject);
		}
	}
}
