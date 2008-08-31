using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace urakawa.media.data
{
    [TestFixture]
    public class DataProviderManagerTests
    {
        protected DataProviderManager mManager = null;

        protected urakawa.Presentation mPresentation
        {
            get { return mManager.Presentation; }
        }

        protected urakawa.Project mProject
        {
            get { return mPresentation.Project; }
        }
        protected DataProviderManager mDataProviderManager
        {
            get { return mManager as DataProviderManager; }
        }

        [SetUp]
        public void SetUp()
        {
            Project proj = new Project();
            proj.AddNewPresentation();
            mManager = proj.GetPresentation(0).DataProviderManager;
        }

        [Test]
        public void Temp()
        {
            FileDataProvider fdp = mDataProviderManager.DataProviderFactory.CreateFileDataProvider(
                DataProviderFactory.AUDIO_WAV_MIME_TYPE);
            Assert.IsNotNull(fdp, "Could not create FileDataProvider");
        }
    }
}