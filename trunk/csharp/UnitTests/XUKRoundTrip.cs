using System;
using System.IO;
using System.Xml;
using NUnit.Framework;
using urakawa.core;

namespace urakawa.test.unitTests
{
	/// <summary>
	/// Tests for checking if the XUK format is "round-trip" proof,
	/// </summary>
  [TestFixture]
	public class XUKRoundTrip
	{

    private urakawa.xuk.Project mProject;
    private string mDefaultFile = "../../XukWorks/roundTrimTestSample.xuk";


		public XUKRoundTrip()
		{
		}

    [SetUp]
    public void Init()
    {
      mProject = new urakawa.xuk.Project();
			
      string filepath = Directory.GetCurrentDirectory();

      Uri fileUri = new Uri(filepath);
			
      fileUri = new Uri(fileUri, mDefaultFile);
			
      mProject.openXUK(fileUri);
    }

    [Test]
    public void AreRootNodesEqualAfterSaveAndReload()
    {
      MemoryStream memStream = new MemoryStream();
      XmlTextWriter wr = new XmlTextWriter(memStream, System.Text.Encoding.UTF8);
      mProject.getPresentation().XUKout(wr);
      Presentation reloadedPresentation = new Presentation();
      wr = null;
      memStream.Position = 0;
      XmlTextReader rd = new XmlTextReader(memStream);
      reloadedPresentation.XUKin(rd);
      rd.Close();
      Assert.IsFalse(
        CoreNode.areCoreNodesEqual(
          mProject.getPresentation().getRootNode(), 
          reloadedPresentation.getRootNode(),
          true),
        "Root nodes of original and reloaded presentations are not equal");
    }
	}
}
