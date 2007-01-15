using System;
using System.IO;
using System.Xml;
using NUnit.Framework;
using urakawa.core;
using urakawa.examples;

namespace urakawa.unitTests.fixtures.examples
{
	/// <summary>
	/// Tests for <see cref="ExampleCustomProperty"/> and <see cref="ExampleCustomCoreNode"/>
	/// </summary>
	[TestFixture] public class ExampleCustomTests
	{
		private urakawa.project.Project mProject;
		private string mDefaultFile = "../XukWorks/ExCustTestSample.xuk";

		public ExampleCustomTests()
		{
		}

		[SetUp] public void Init() 
		{
			mProject = new urakawa.project.Project(
				new Presentation(new ExampleCustomCoreNodeFactory(), null, null, new ExampleCustomPropertyFactory(), null),
				null);
			string filepath = Directory.GetCurrentDirectory();

			Uri fileUri = new Uri(filepath);
			
			fileUri = new Uri(fileUri, mDefaultFile);
			
			Assert.IsTrue(mProject.openXUK(fileUri), "Failed to load XUK file {0}", mDefaultFile);
		}

		[Test] public void TestExCustDataLoaded()
		{
			TestRootNodeCustomPropData(mProject);
			TestRootNodeFirstChildCustCoreNodedata(mProject);
		}

		private void TestRootNodeCustomPropData(urakawa.project.Project proj)
		{
			ExampleCustomProperty rootExCustProp = 
				(ExampleCustomProperty)proj.getPresentation().getRootNode()
				  .getProperty(typeof(ExampleCustomProperty));
			Assert.AreEqual("Test Data", rootExCustProp.CustomData);
		}

		private void TestRootNodeFirstChildCustCoreNodedata(urakawa.project.Project proj)
		{
			ExampleCustomCoreNode firstCh = (ExampleCustomCoreNode)proj.getPresentation().getRootNode().getChild(0);
			Assert.AreEqual("Test Ex Cust Core Node Data", firstCh.CustomCoreNodeData);
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
			urakawa.project.Project reloadedProject = new urakawa.project.Project(
				new Presentation(new ExampleCustomCoreNodeFactory(), null, null, new ExampleCustomPropertyFactory(), null),
				null);
			XmlTextReader rd = new XmlTextReader(memStream);
			Assert.IsTrue(
				reloadedProject.openXUK(rd),
				"Failed to reopen project");
			rd.Close();
			TestRootNodeCustomPropData(reloadedProject);
			TestRootNodeFirstChildCustCoreNodedata(reloadedProject);
		}
	}
}
