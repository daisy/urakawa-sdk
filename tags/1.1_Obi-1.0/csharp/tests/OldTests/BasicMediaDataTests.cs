using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using urakawa;
using urakawa.media.data;
using NUnit.Framework;

namespace urakawa.unitTests.mediaDataTests
{
	/// <summary>
	/// Basic tests for <see cref="urakawa.media.data"/> class methods
	/// </summary>
	[TestFixture]
	public class BasicMediaDataTests : MediaDataTestBase
	{


		[Test]
		public void CheckNumberOfFileDataProviders()
		{
			int count = mProject.getPresentation(0).getDataProviderManager().getListOfDataProviders().Count;
			Assert.AreEqual(
				2, count, "Invalid number of DataProviders, expected 2, but found {0:0}", count);
		}

		[Test]
		public void SetBaseUriMovesDataFiles()
		{
			Project copyProj = new Project();
			Uri copyDir = new Uri(mProject.getPresentation(0).getRootUri(), "../MediaDataSampleCopy/");
			copyProj.openXUK(new Uri(mProject.getPresentation(0).getRootUri(), "MediaDataSample.xuk"));


            ((FileDataProviderManager)copyProj.getPresentation(0).getDataProviderManager()).allowCopyDataOnUriChanged(true);


			copyProj.getPresentation(0).setRootUri(copyDir);
			bool dataProvMngrsEqual = copyProj.getPresentation(0).getDataProviderManager().valueEquals(
				mProject.getPresentation(0).getDataProviderManager());
			Assert.IsTrue(dataProvMngrsEqual, "The DataProviderManagers are not equal after setting a new BaseUri");
		}

        //[Test]
        //public void RoundTrip()
        //{
        //    MemoryStream memStr = new MemoryStream();
        //    XmlWriterSettings wrSett = new XmlWriterSettings();
        //    wrSett.CloseOutput = false;
        //    XmlWriter wr = XmlWriter.Create(memStr);
        //    mProject.saveXUK(wr, mProject.getPresentation(0).getRootUri());
        //    wr.Close();
        //    memStr.Position = 0;
        //    XmlReader rd = XmlReader.Create(memStr, new XmlReaderSettings(), mProject.getPresentation(0).getRootUri().ToString());
        //    Project reloadedProj = new Project();
        //    reloadedProj.openXUK(rd);
        //    reloadedProj.getPresentation(0).setRootUri(mProject.getPresentation(0).getRootUri());
        //    rd.Close();
        //    bool projsEqual = reloadedProj.valueEquals(mProject);
        //    Assert.IsTrue(projsEqual, "The reloaded project is not equal to the original");
        //}
	}
}
