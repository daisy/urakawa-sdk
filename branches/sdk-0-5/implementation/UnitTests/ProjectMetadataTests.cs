using System;
using NUnit.Framework;
using urakawa.project;

namespace urakawa.unitTests.testbase
{
    /// <summary>
    /// Tests for <see cref="urakawa.project.Project"/> <see cref="urakawa.project.IMetadata"/>
    /// </summary>

    public class ProjectMetadataTests
    {
        protected Project mProject;


        [Test]
        public void AppendMetadataTest()
        {
            IMetadata newMeta = mProject.getMetadataFactory().createMetadata();
            newMeta.setName("testAppendName");
            mProject.appendMetadata(newMeta);
            System.Collections.IList retrMetas = mProject.getMetadataList("testAppendName");
            Assert.AreEqual(1, retrMetas.Count, "Retrieved metadata list has wrong length");
            Assert.AreEqual(retrMetas[0], newMeta, "The retrieved metadata is not the same as the added");
        }

        [Test]
        public void RetrieveMetadataListTest()
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


        // Added by JQ 20070402
        [Test]
        public void DeleteMetaDataTest()
        {
            // create two meta data items to be deleted
            IMetadata toDelete = mProject.getMetadataFactory().createMetadata();
            toDelete.setName("toDelete");
            mProject.appendMetadata(toDelete);
            IMetadata toDeleteToo = mProject.getMetadataFactory().createMetadata();
            toDeleteToo.setName("toDeleteToo");
            mProject.appendMetadata(toDeleteToo);
            // make sure that they were added in the first place
            System.Collections.IList retrieved = mProject.getMetadataList("toDelete");
            Assert.AreEqual(1, retrieved.Count, "Retrieved metadata list has wrong length");
            Assert.AreEqual(retrieved[0], toDelete, "The retrieved metadata is not the same as the added");
            retrieved = mProject.getMetadataList("toDeleteToo");
            Assert.AreEqual(1, retrieved.Count, "Retrieved metadata list has wrong length");
            Assert.AreEqual(retrieved[0], toDeleteToo, "The retrieved metadata is not the same as the added");
            // delete by object reference
            mProject.deleteMetadata(toDelete);
            retrieved = mProject.getMetadataList("toDelete");
            Assert.AreEqual(0, retrieved.Count, "Retrieved metadata list has wrong length");
            // delete by string
            mProject.deleteMetadata("toDeleteToo");
            retrieved = mProject.getMetadataList("toDeleteToo");
            Assert.AreEqual(0, retrieved.Count, "Retrieved metadata list has wrong length");
        }
    }
}
