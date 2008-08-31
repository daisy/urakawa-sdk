using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using NUnit.Framework;
using urakawa.xuk;

namespace urakawa.media.data
{
    [TestFixture]
    public class FileDataProviderTests : IDataProviderTests
    {
        protected FileDataProvider mFileDataProvider1
        {
            get { return mDataProvider1 as FileDataProvider; }
        }

        protected FileDataProvider mFileDataProvider2
        {
            get { return mDataProvider2 as FileDataProvider; }
        }

        protected FileDataProvider mFileDataProvider3
        {
            get { return mDataProvider3 as FileDataProvider; }
        }

        public FileDataProviderTests()
            : base(typeof (FileDataProvider).Name, XukAble.XUK_NS)
        {
        }

        #region IDataProvider tests

        [Test]
        public override void GetInputStream_InitialState()
        {
            base.GetInputStream_InitialState();
        }

        [Test]
        public override void GetInputStream_CanGetMultiple()
        {
            base.GetInputStream_CanGetMultiple();
        }

        [Test]
        public override void GetOutputStream_InitialState()
        {
            base.GetOutputStream_InitialState();
        }

        [Test]
        [ExpectedException(typeof (exception.OutputStreamOpenException))]
        public override void GetOutputStream_CannotGetMultiple()
        {
            base.GetOutputStream_CannotGetMultiple();
        }

        [Test]
        public override void GetOutputStream_RetrieveDataWritten()
        {
            base.GetOutputStream_RetrieveDataWritten();
        }

        [Test]
        public override void GetUid_Basics()
        {
            base.GetUid_Basics();
        }

        [Test]
        public override void Delete_Basics()
        {
            base.Delete_Basics();
        }

        [Test]
        public void Delete_DataFilesDeleted()
        {
            DataProviderManager mngr = mPresentation.DataProviderManager as DataProviderManager;
            string path = mngr.DataFileDirectoryFullPath;
            Assert.Greater(mngr.ListOfDataProviders.Count, 0, "The manager does not manage any DataProviders");
            foreach (IDataProvider prov in mngr.ListOfDataProviders)
            {
                Stream outStm = prov.GetOutputStream();
                try
                {
                    outStm.WriteByte(0xAA); //Ensure that files are created
                }
                finally
                {
                    outStm.Close();
                }
                prov.Delete();
            }
            Assert.AreEqual(
                0, mngr.ListOfDataProviders.Count,
                "The manager still contains DataProviders after they are all deleted");
            Assert.IsTrue(
                Directory.Exists(path),
                "The data file directory of the FileDataManager does not exist");
            Assert.AreEqual(
                0, Directory.GetFiles(path).Length,
                "The data file directory of the FileDataManager is not empty");
        }

        [Test]
        [ExpectedException(typeof (exception.InputStreamsOpenException))]
        public override void Delete_OpenInputStream()
        {
            base.Delete_OpenInputStream();
        }

        [Test]
        [ExpectedException(typeof (exception.OutputStreamOpenException))]
        public override void Delete_OpenOutputStream()
        {
            base.Delete_OpenOutputStream();
        }

        [Test]
        public override void Copy_Basics()
        {
            base.Copy_Basics();
        }

        #endregion

        #region IValueEquatable tests

        [Test]
        public override void ValueEquals_Basics()
        {
            base.ValueEquals_Basics();
        }

        [Test]
        public override void ValueEquals_MimeType()
        {
            base.ValueEquals_MimeType();
        }

        #endregion
    }
}