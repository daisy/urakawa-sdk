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
			StringWriter swr = new StringWriter();
			XmlTextWriter wr = new XmlTextWriter(swr);
			Assert.IsTrue(mProject.saveXUK(wr), "failed to write presentation to memory stream");
			wr.Flush();
			Project reloadedProject = new Project();
			wr = null;
			StringReader srd = new StringReader(swr.ToString());
			XmlTextReader rd = new XmlTextReader(srd);
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
