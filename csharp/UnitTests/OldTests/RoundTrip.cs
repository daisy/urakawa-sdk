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
			mProject.saveXUK(wr);
			wr.Flush();
			Project reloadedProject = new Project();
			wr = null;
			StringReader srd = new StringReader(swr.ToString());
			XmlTextReader rd = new XmlTextReader(srd);
			reloadedProject.openXUK(rd);
			rd.Close();
			bool rootsEqual = mProject.getPresentation().getRootNode().valueEquals(
				reloadedProject.getPresentation().getRootNode());
			Assert.IsTrue(
			  rootsEqual,
			  "Root nodes of original and reloaded presentations are not equal");
			System.Collections.Generic.IList<Metadata> origMetadata = mProject.getPresentation().getMetadataList();
			System.Collections.Generic.IList<Metadata> reloadedMetadata = mProject.getPresentation().getMetadataList();
			Assert.AreEqual(origMetadata.Count, reloadedMetadata.Count, "Different number of metadata items in reloaded project");
			foreach (Metadata oIMeta in origMetadata)
			{
				bool foundInReloaded = false;
				foreach (Metadata rIMeta in reloadedMetadata)
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
