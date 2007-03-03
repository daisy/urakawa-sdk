using System;
using urakawa.metadata;
using NUnit.Framework;

namespace urakawa.unitTests.testbase
{
	/// <summary>
	/// Tests for <see cref="Project"/> <see cref="urakawa.project.IMetadata"/>
	/// </summary>
 
  [TestFixture] public class ProjectMetadataTests
	{
		protected Project mProject;

		private string mDefaultFile = "../XukWorks/simplesample.xuk";

		[SetUp]
		public void Init()
		{
			mProject = new Project();

			string filepath = System.IO.Directory.GetCurrentDirectory();

			Uri fileUri = new Uri(filepath);

			fileUri = new Uri(fileUri, mDefaultFile);

			bool openSucces = mProject.openXUK(fileUri);
			Assert.IsTrue(openSucces, String.Format("Could not open xuk file {0}", mDefaultFile));
		}


    [Test] public void AppendMetadataTest()
    {
			//First remove any metadata with the test name
			mProject.deleteMetadata("testAppendName");
			IMetadata newMeta = mProject.getMetadataFactory().createMetadata();
      newMeta.setName("testAppendName");
      mProject.appendMetadata(newMeta);
			System.Collections.Generic.IList<IMetadata> retrMetas = mProject.getMetadataList("testAppendName");
      Assert.AreEqual(1, retrMetas.Count, "Retrieved metadata list has wrong count");
      Assert.AreEqual(retrMetas[0], newMeta, "The retrieved metadata is not the same as the added");
    }

    [Test] public void RetrieveMetadataListTest()
    {
      bool foundLNN = false;
      bool foundOHA = false;
      bool foundOther = false;
      foreach (Metadata md in mProject.getMetadataList("dc:Author"))
      {
        switch (md.getContent())
        {
          case "Laust Skat Nielsen":
            foundLNN = true;
            break;
          case "Ole Holst Andersen":
            foundOHA = true;
            break;
          default:
            foundOther = true;
            break;
        }
      }
      Assert.IsTrue(foundLNN, "Cound not find dc:Author 'Laust Skat Nielsen'");
      Assert.IsTrue(foundOHA, "Cound not find dc:Author 'Ole Holst Andersen'");
      Assert.IsFalse(foundOther, "Found dc:Author besides 'Laust Skat Nielsen' and 'Ole Holst Andersen'");
      int dcAuthorCount = 0;
      int dcTitleCount = 0;
      int dcSubjectCount = 0;
      int otherCount = 0;
      foreach (Metadata md in mProject.getMetadataList())
      {
        switch (md.getName())
        {
          case "dc:Author":
            dcAuthorCount++;
            break;
          case "dc:Title":
            dcTitleCount++;
            break;
          case "dc:Subject":
            dcSubjectCount++;
            break;
          default:
            otherCount++;
            break;
        }
      }
    }


	}
}
