using System;
using System.IO;
using System.Text;
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
        //[Test]
        //public void AreRootNodesEqualAfterSaveAndReload()
        //{
        //    StringWriter swr = new StringWriter();
        //    XmlTextWriter subWr = new XmlTextWriter(swr);
        //    XmlWriterSettings wrSet = new XmlWriterSettings();
        //    wrSet.Indent = true;
        //    XmlWriter wr = XmlWriter.Create(subWr, wrSet);
        //    mProject.saveXUK(wr, mProject.getPresentation(0).getRootUri());
        //    wr.Flush();
        //    Project reloadedProject = new Project();
        //    wr = null;
        //    StringReader srd = new StringReader(swr.ToString());
        //    XmlTextReader rd = new XmlTextReader(mProject.getPresentation(0).getRootUri().ToString(), srd);
        //    reloadedProject.openXUK(rd);
        //    rd.Close();
        //    bool rootsEqual = mProject.getPresentation(0).getRootNode().valueEquals(
        //        reloadedProject.getPresentation(0).getRootNode());
        //    Assert.IsTrue(
        //      rootsEqual,
        //      "Root nodes of original and reloaded presentations are not equal");
        //    System.Collections.Generic.IList<Metadata> origMetadata = mProject.getPresentation(0).getListOfMetadata();
        //    System.Collections.Generic.IList<Metadata> reloadedMetadata = mProject.getPresentation(0).getListOfMetadata();
        //    Assert.AreEqual(origMetadata.Count, reloadedMetadata.Count, "Different number of metadata items in reloaded project");
        //    foreach (Metadata oIMeta in origMetadata)
        //    {
        //        bool foundInReloaded = false;
        //        foreach (Metadata rIMeta in reloadedMetadata)
        //        {
        //            if (oIMeta.ValueEquals(rIMeta))
        //            {
        //                foundInReloaded = true;
        //                break;
        //            }
        //            Assert.IsTrue(foundInReloaded, String.Format(
        //                "Could not find Metadata {0}", oIMeta.getName()));
        //        }
        //    }
        //}
	}
}
