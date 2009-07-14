using NUnit.Framework;

namespace urakawa.media.data
{
    [TestFixture]
    public class DataProviderManagerTests
    {
        protected DataProviderManager mManager;

        protected Presentation mPresentation
        {
            get { return mManager.Presentation; }
        }

        protected Project mProject
        {
            get { return mPresentation.Project; }
        }
        protected DataProviderManager mDataProviderManager
        {
            get { return mManager; }
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
            FileDataProvider fdp = mDataProviderManager.Presentation.DataProviderFactory.Create<FileDataProvider>(
                DataProviderFactory.AUDIO_WAV_MIME_TYPE);
            Assert.IsNotNull(fdp, "Could not create FileDataProvider");
        }
    }
}