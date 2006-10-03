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
			Assert.IsTrue(mProject.saveXUK(wr), "failed to write presentation to memory stream");
			wr.Flush();
			project.Project reloadedProject = new project.Project();
			wr = null;
			memStream.Position = 0;
			StreamReader srd = new StreamReader(memStream);
			string temp = srd.ReadToEnd();
			srd = null;
			memStream.Position = 0;
			XmlTextReader rd = new XmlTextReader(memStream);
			Assert.IsTrue(reloadedProject.openXUK(rd), "Failed to reload project from memory stream");
			rd.Close();
			Assert.IsTrue(
				CoreNode.areCoreNodesEqual(
				(CoreNode)(mProject.getPresentation().getRootNode()), 
				reloadedProject.getPresentation().getRootNode(),
				true),
				"Root nodes of original and reloaded presentations are not equal");
			System.Collections.IList origMetadata = mProject.getMetadataList();
			System.Collections.IList reloadedMetadata = mProject.getMetadataList();
			Assert.AreEqual(origMetadata.Count, reloadedMetadata.Count, "Different number of metadata items in reloaded project");
			foreach (project.IMetadata oIMeta in origMetadata)
			{
				if (typeof(project.Metadata).IsAssignableFrom(oIMeta.GetType()))
				{
					project.Metadata oMeta = (project.Metadata)oIMeta;
					bool foundInReloaded = false;
					foreach (project.IMetadata rIMeta in reloadedMetadata)
					{
						if (typeof(project.Metadata).IsAssignableFrom(rIMeta.GetType()))
						{
							if (project.Metadata.AreEqual(oMeta, (project.Metadata)rIMeta)) foundInReloaded = true;
						}
					}
					Assert.IsTrue(foundInReloaded, String.Format(
						"Could not find Metadata {0}", oMeta.getName()));
				}
			}
		}
	}
}
