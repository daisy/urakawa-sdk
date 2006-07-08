using System;
using System.IO;
using System.Xml;
using NUnit.Framework;
using urakawa.core;

namespace urakawa.unitTests.testbase
{
	/// <summary>
	/// Tests for checking if the XUK format is "round-trip" proof,
	/// </summary>
	public class RoundTrip : TestCollectionBase
	{
		[Test]
		public void AreRootNodesEqualAfterSaveAndReload()
		{
			MemoryStream memStream = new MemoryStream();
			XmlTextWriter wr = new XmlTextWriter(memStream, System.Text.Encoding.UTF8);
			Assert.IsTrue(mProject.getPresentation().XUKout(wr), "failed to write presentation to memory stream");
			wr.Flush();
			Presentation reloadedPresentation = new Presentation();
			wr = null;
			memStream.Position = 0;
			XmlTextReader rd = new XmlTextReader(memStream);
			while (rd.Read())
			{
				if (rd.NodeType==System.Xml.XmlNodeType.Element && rd.LocalName=="Presentation") break;
			}
			Assert.IsTrue(reloadedPresentation.XUKin(rd), "Failed to reload presentation from memory stream");
			rd.Close();
			Assert.IsTrue(
				CoreNode.areCoreNodesEqual(
				((Presentation)mProject.getPresentation()).getRootNode(), 
				reloadedPresentation.getRootNode(),
				true),
				"Root nodes of original and reloaded presentations are not equal");
		}
	}
}
