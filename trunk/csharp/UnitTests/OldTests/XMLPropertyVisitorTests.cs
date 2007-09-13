using System;
using NUnit.Framework;
using urakawa.core;
using urakawa.property.xml;

namespace urakawa.unitTests.testbase
{
	/// <summary>
	/// Summary description for VisitorTests.
	/// </summary>
	public class XmlPropertyVisitorTests : TestCollectionBase
	{
		[Test] public void VisitXmlPropertyName()
		{
			XmlPropertyElementNameVisitor vis = new XmlPropertyElementNameVisitor();

			vis.addElementName("level", "");

			mProject.getPresentation().getRootNode().acceptDepthFirst(vis);

			System.Collections.Generic.IList<TreeNode> list = vis.getResults();

			Assert.IsNotNull(list);

			//this particular file (SampleDTB2Ver1.xuk) has one level node
			Assert.AreEqual(1, list.Count);
		}

	}
}
