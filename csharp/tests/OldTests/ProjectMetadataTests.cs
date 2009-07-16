using System;
using urakawa.metadata;
using NUnit.Framework;

namespace urakawa.oldTests
{
    /// <summary>
    /// Tests for <see cref="Project"/> <see cref="urakawa.project.Metadata"/>
    /// </summary>
    [TestFixture]
    public class ProjectMetadataTests
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

            mProject.OpenXuk(fileUri);
        }


        [Test]
        public void AppendMetadataTest()
        {
            //First remove any metadata with the test name
            mProject.Presentations.Get(0).DeleteMetadata("testAppendName");
            Metadata newMeta = mProject.Presentations.Get(0).MetadataFactory.CreateMetadata();
            newMeta.Name = "testAppendName";

            ObjectListProvider<Metadata> list = mProject.Presentations.Get(0).Metadatas;
            list.Insert(list.Count, newMeta);

            System.Collections.Generic.IList<Metadata> retrMetas =
                mProject.Presentations.Get(0).GetMetadata("testAppendName");
            Assert.AreEqual(1, retrMetas.Count, "Retrieved metadata list has wrong count");
            Assert.AreEqual(retrMetas[0], newMeta, "The retrieved metadata is not the same as the added");
        }

        private void CheckDCAuthor(out bool foundLNN, out bool foundOHA, out bool foundOther)
        {
            foundLNN = false;
            foundOHA = false;
            foundOther = false;
            foreach (Metadata md in mProject.Presentations.Get(0).GetMetadata("dc:Author"))
            {
                switch (md.Content)
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
        }

        [Test]
        public void RetrieveMetadataListTest()
        {
            bool foundLNN;
            bool foundOHA;
            bool foundOther;
            CheckDCAuthor(out foundLNN, out foundOHA, out foundOther);
            Assert.IsTrue(foundLNN, "Cound not find dc:Author 'Laust Skat Nielsen'");
            Assert.IsTrue(foundOHA, "Cound not find dc:Author 'Ole Holst Andersen'");
            Assert.IsFalse(foundOther, "Found dc:Author besides 'Laust Skat Nielsen' and 'Ole Holst Andersen'");
            int dcAuthorCount = 0;
            int dcTitleCount = 0;
            int dcSubjectCount = 0;
            int otherCount = 0;
            foreach (Metadata md in mProject.Presentations.Get(0).Metadatas.ContentsAs_YieldEnumerable)
            {
                switch (md.Name)
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

        [Test]
        public void DeleteMetadataTest()
        {
            bool foundLNN;
            bool foundOHA;
            bool foundOther;
            CheckDCAuthor(out foundLNN, out foundOHA, out foundOther);
            Assert.IsTrue(foundLNN, "Cound not find dc:Author 'Laust Skat Nielsen'");
            Assert.IsTrue(foundOHA, "Cound not find dc:Author 'Ole Holst Andersen'");
            Assert.IsFalse(foundOther, "Found dc:Author besides 'Laust Skat Nielsen' and 'Ole Holst Andersen'");
            foreach (Metadata md in mProject.Presentations.Get(0).GetMetadata("dc:Author"))
            {
                if (md.Content == "Laust Skat Nielsen") mProject.Presentations.Get(0).Metadatas.Remove(md);
            }
            CheckDCAuthor(out foundLNN, out foundOHA, out foundOther);
            Assert.IsFalse(foundLNN, "Found dc:Author 'Laust Skat Nielsen' after delete of same");
            Assert.IsTrue(foundOHA, "Cound not find dc:Author 'Ole Holst Andersen' after delete of 'Laust Skat Nielsen'");
            Assert.IsFalse(foundOther, "Found dc:Author besides 'Laust Skat Nielsen' and 'Ole Holst Andersen'");
            mProject.Presentations.Get(0).DeleteMetadata("dc:Author");
            CheckDCAuthor(out foundLNN, out foundOHA, out foundOther);
            Assert.IsFalse(foundLNN, "Found dc:Author 'Laust Skat Nielsen' after delete of all 'dc:Author's");
            Assert.IsFalse(foundOHA, "Found dc:Author 'Ole Holst Andersen' after delete of all 'dc:Author's");
            Assert.IsFalse(foundOther, "Found dc:Author besides 'Laust Skat Nielsen' and 'Ole Holst Andersen'");
        }
    }
}