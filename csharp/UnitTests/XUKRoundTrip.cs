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
			
      Assert.IsTrue(mProject.openXUK(fileUri), "Failed to load XUK file {0}", mDefaultFile);
    }

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
          mProject.getPresentation().getRootNode(), 
          reloadedPresentation.getRootNode(),
          true),
        "Root nodes of original and reloaded presentations are not equal");
    }
	}
}
