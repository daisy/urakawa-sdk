using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using NUnit.Framework;

namespace urakawa.media.data
{
    public abstract class IDataProviderTests
    {
        private Project mmProject;

        protected Project mProject
        {
            get { return mmProject; }
        }

        protected Presentation mPresentation
        {
            get { return mProject.GetPresentation(0); }
        }

        protected DataProvider mDataProvider1;
        protected DataProvider mDataProvider2;
        protected DataProvider mDataProvider3;

        private Type mDefaultDataProviderType;

        private Uri mRootUri;

        protected IDataProviderTests(string provXukLN, string provXukNS)
        {
            mDefaultDataProviderType = typeof (FileDataProvider);
        }

        [SetUp]
        public void SetUp()
        {
            mmProject = new Project();
            mProject.AddNewPresentation();
            string str = "file://" + Path.Combine(Path.GetTempPath(), Path.GetRandomFileName()) + Path.DirectorySeparatorChar;

            mRootUri = new Uri(str);
            if (!Directory.Exists(mRootUri.LocalPath)) Directory.CreateDirectory(mRootUri.LocalPath);
           
            mPresentation.RootUri = mRootUri;
            mDataProvider1 = mPresentation.DataProviderFactory.Create(mDefaultDataProviderType, "text/plain");
            Assert.IsNotNull(
                mDataProvider1,
                "DataProviderFactory cannot create a {0}",
                mDefaultDataProviderType);
            mDataProvider2 = mPresentation.DataProviderFactory.Create(mDefaultDataProviderType, "text/plain");
            Assert.IsNotNull(
                mDataProvider2,
                "DataProviderFactory cannot create a {0}",
                mDefaultDataProviderType);
            mDataProvider3 = mPresentation.DataProviderFactory.Create(mDefaultDataProviderType, "text/plain");
            Assert.IsNotNull(
                mDataProvider3,
                "DataProviderFactory cannot create a {0}",
                mDefaultDataProviderType);
        }

        [TearDown]
        public void TearDown()
        {
            if (Directory.Exists(mRootUri.LocalPath)) Directory.Delete(mRootUri.LocalPath, true);
        }


        public virtual void GetUid_Basics()
        {
            Assert.AreEqual(
                mDataProvider1.Presentation.DataProviderManager.GetUidOfManagedObject(mDataProvider1),
                mDataProvider1.Uid,
                "DataProvider.getUid did not return the expected value");
        }

        public virtual void GetInputStream_InitialState()
        {
            Stream inpStm = mDataProvider1.GetInputStream();
            try
            {
                Assert.AreEqual(0, inpStm.Position, "Unexpected initial position of input stream");
                Assert.AreEqual(0, inpStm.Length, "Unexpected initial length of input stream");
                Assert.IsTrue(inpStm.CanRead, "Can not read from the retrieved input Stream");
                Assert.IsFalse(inpStm.CanWrite, "Input Stream must be read-only");
                Assert.IsTrue(inpStm.CanSeek, "Input stream is not seekable");
            }
            finally
            {
                inpStm.Close();
            }
        }

        public virtual void GetInputStream_CanGetMultiple()
        {
            Stream inpStm1, inpStm2;
            inpStm1 = mDataProvider1.GetInputStream();
            try
            {
                inpStm2 = mDataProvider1.GetInputStream();
                inpStm2.Close();
            }
            finally
            {
                inpStm1.Close();
            }
        }

        public virtual void GetOutputStream_InitialState()
        {
            Stream outStm = mDataProvider1.GetOutputStream();
            try
            {
                Assert.AreEqual(0, outStm.Position, "Unexpected initial Position of output Stream");
                Assert.AreEqual(0, outStm.Length, "Unexpected initial Length of output Stream");
                Assert.IsTrue(outStm.CanWrite, "Can not write to the retrieved output Stream");
                Assert.IsFalse(outStm.CanRead, "Output Stream must be write-only");
                Assert.IsTrue(outStm.CanSeek, "Output stream is not seekable");
            }
            finally
            {
                outStm.Close();
            }
        }

        public virtual void GetOutputStream_CannotGetMultiple()
        {
            Stream outStm1 = mDataProvider1.GetOutputStream();
            try
            {
                Stream outStm2 = mDataProvider1.GetOutputStream();
                outStm2.Close();
            }
            finally
            {
                outStm1.Close();
            }
        }

        public virtual void GetOutputStream_RetrieveDataWritten()
        {
            MemoryStream memStm = StreamUtils.GetRandomMemoryStream(100*1024);
            Stream outStm = mDataProvider1.GetOutputStream();
            try
            {
                StreamUtils.CopyData(memStm, outStm);
            }
            finally
            {
                outStm.Close();
            }
            memStm.Seek(0, SeekOrigin.Begin);
            Stream inpStm = mDataProvider1.GetInputStream();
            try
            {
                Assert.AreEqual(0, inpStm.Position, "The input Stream  is not positioned at 0");
                Assert.AreEqual(
                    memStm.Length, inpStm.Length,
                    "The length of the input Stream does not equal the bytes written to the out Stream");
                Assert.IsTrue(
                    StreamUtils.CompareStreams(memStm, inpStm),
                    "The input Stream does not contain the data written to the output Stream");
            }
            finally
            {
                inpStm.Close();
            }
            outStm = mDataProvider1.GetOutputStream();
            try
            {
                memStm.Seek(0, SeekOrigin.Begin);
                StreamUtils.CopyData(memStm, outStm);
                memStm.Seek(0, SeekOrigin.Begin);
                StreamUtils.CopyData(memStm, outStm);
            }
            finally
            {
                outStm.Close();
            }
        }

        public virtual void Delete_Basics()
        {
            string uid = mDataProvider1.Uid;
            mDataProvider1.Delete();
            Assert.IsFalse(
                mPresentation.DataProviderManager.IsManagerOf(uid),
                "delete should remove the DataProvider from it's manager");
        }

        public virtual void Delete_OpenInputStream()
        {
            Stream intStm = mDataProvider1.GetInputStream();
            try
            {
                mDataProvider1.Delete();
            }
            finally
            {
                intStm.Close();
            }
        }

        public virtual void Delete_OpenOutputStream()
        {
            Stream outStm = mDataProvider1.GetOutputStream();
            try
            {
                mDataProvider1.Delete();
            }
            finally
            {
                outStm.Close();
            }
        }

        public virtual void Copy_Basics()
        {
            DataProvider cpy;
            cpy = mDataProvider1.Copy();
            Assert.AreNotEqual(cpy.Uid, mDataProvider1.Uid, "The copy cannot have the same uid as the original");
            Assert.IsTrue(mDataProvider1.ValueEquals(cpy), "The copy must have the same value as the original");
            Assert.IsFalse(Type.ReferenceEquals(mDataProvider1, cpy),
                           "The copy may not be reference equal to the original");
            Stream outStm = mDataProvider1.GetOutputStream();
            try
            {
                StreamUtils.CopyData(StreamUtils.GetRandomMemoryStream(23564), outStm);
            }
            finally
            {
                outStm.Close();
            }
            cpy = mDataProvider1.Copy();
            Assert.AreNotEqual(cpy.Uid, mDataProvider1.Uid, "The copy cannot have the same uid as the original");
            Assert.IsTrue(mDataProvider1.ValueEquals(cpy), "The copy must have the same value as the original");
            Assert.IsFalse(Type.ReferenceEquals(mDataProvider1, cpy),
                           "The copy may not be reference equal to the original");
        }

        #region IValueEquatable tests

        public virtual void ValueEquals_Basics()
        {
            MemoryStream memStmA = StreamUtils.GetRandomMemoryStream(95567);
            MemoryStream memStmB = StreamUtils.GetRandomMemoryStream(23479);
            Stream outStm1 = mDataProvider1.GetOutputStream();
            try
            {
                StreamUtils.CopyData(memStmA, outStm1);
                memStmA.Seek(0, SeekOrigin.Begin);
            }
            finally
            {
                outStm1.Close();
            }
            Stream outStm2 = mDataProvider2.GetOutputStream();
            try
            {
                StreamUtils.CopyData(memStmB, outStm2);
                memStmB.Seek(0, SeekOrigin.Begin);
            }
            finally
            {
                outStm2.Close();
            }
            Stream outStm3 = mDataProvider3.GetOutputStream();
            try
            {
                StreamUtils.CopyData(memStmA, outStm3);
                memStmA.Seek(0, SeekOrigin.Begin);
            }
            finally
            {
                outStm3.Close();
            }
            Assert.IsTrue(
                mDataProvider1.ValueEquals(mDataProvider3),
                "Two IDataProviders with the same data and no MIME type must be value equal");
            Assert.IsFalse(
                mDataProvider1.ValueEquals(mDataProvider2),
                "IDataProviders containing different data cannot be value equal");
            IValueEquatableBasicTestUtils.ValueEquals_BasicTests<DataProvider>(mDataProvider1, mDataProvider2,
                                                                                mDataProvider3);
        }

        public virtual void ValueEquals_MimeType()
        {
            DataProvider p1 = mPresentation.DataProviderFactory.Create(mDefaultDataProviderType, "wav");
            DataProvider p2 = mPresentation.DataProviderFactory.Create(mDefaultDataProviderType, "txt");
            Assert.IsFalse(p1.ValueEquals(p2), "IDataProviders with different MIME types can not be value equal");
        }

        #endregion
    }
}