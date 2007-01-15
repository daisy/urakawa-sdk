using System;
using System.IO;
using System.Xml;
using NUnit.Framework;
using urakawa.metadata;
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
			Assert.IsTrue(mProject.saveXUK(wr), "failed to write presentation to memory stream");
			wr.Flush();
			Project reloadedProject = new Project();
			wr = null;
			memStream.Position = 0;
			StreamReader srd = new StreamReader(memStream);
			string temp = srd.ReadToEnd();
			srd = null;
			memStream.Position = 0;
			StreamReader strrd = new StreamReader(memStream, System.Text.Encoding.UTF8);
			string content = strrd.ReadToEnd();
			memStream.Position = 0;
			XmlTextReader rd = new XmlTextReader(memStream);
			Assert.IsTrue(reloadedProject.openXUK(rd), "Failed to reload project from memory stream");
			rd.Close();
			bool rootsEqual = mProject.getPresentation().getRootNode().ValueEquals(
				reloadedProject.getPresentation().getRootNode());
			Assert.IsTrue(
			  rootsEqual,
			  "Root nodes of original and reloaded presentations are not equal");
			System.Collections.Generic.IList<IMetadata> origMetadata = mProject.getMetadataList();
			System.Collections.Generic.IList<IMetadata> reloadedMetadata = mProject.getMetadataList();
			Assert.AreEqual(origMetadata.Count, reloadedMetadata.Count, "Different number of metadata items in reloaded project");
			foreach (IMetadata oIMeta in origMetadata)
			{
				bool foundInReloaded = false;
				foreach (IMetadata rIMeta in reloadedMetadata)
				{
					if (oIMeta.ValueEquals(rIMeta))
					{
						foundInReloaded = true;
						break;
					}
					Assert.IsTrue(foundInReloaded, String.Format(
						"Could not find Metadata {0}", oIMeta.getName()));
				}
			}
		}
	}
}
