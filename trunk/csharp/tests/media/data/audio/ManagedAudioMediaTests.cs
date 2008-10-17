using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using NUnit.Framework;
using urakawa;
using urakawa.core;
using urakawa.property.channel;
using urakawa.media;
using urakawa.media.data.audio;
using urakawa.xuk;

namespace urakawa.media.data.audio
{
    [TestFixture, Description("Tests the ManagedAudioMedia functionality")]
    public class ManagedAudioMediaTests : IMediaTests
    {
        public ManagedAudioMediaTests() : base(typeof (ManagedAudioMedia))
        {
        }

        [TestFixtureSetUp]
        public void SetUpFixture()
        {
            Uri projectDir = new Uri(ProjectTests.SampleXukFileDirectoryUri, "MediaTestsSample/");
            if (Directory.Exists(Path.Combine(projectDir.LocalPath, "Data")))
            {
                Directory.Delete(Path.Combine(projectDir.LocalPath, "Data"), true);
            }
            mProject = new Project();
            mProject.AddNewPresentation().RootUri = projectDir;
        }

        [TestFixtureTearDown]
        public void TearDownFixture()
        {
            Uri projectDir = new Uri(ProjectTests.SampleXukFileDirectoryUri, "MediaTestsSample/");
            if (Directory.Exists(Path.Combine(projectDir.LocalPath, "Data")))
            {
                Directory.Delete(Path.Combine(projectDir.LocalPath, "Data"), true);
            }
        }

        private string GetPath(String fileName)
        {
            return Path.Combine(mPresentation.RootUri.LocalPath, fileName);
        }

        private Stream GetRawStream(String fileName)
        {
            Stream s = new FileStream(GetPath(fileName), FileMode.Open, FileAccess.Read, FileShare.Read);
            s.Seek(44, SeekOrigin.Begin);
            return s;
        }

        private PCMDataInfo GetInfo(string name)
        {
            FileStream fs = new FileStream(GetPath(name), FileMode.Open, FileAccess.Read, FileShare.Read);
            try
            {
                return PCMDataInfo.ParseRiffWaveHeader(fs);
            }
            finally
            {
                fs.Close();
            }
        }


        protected ManagedAudioMedia mManagedAudioMedia1
        {
            get { return mMedia1 as ManagedAudioMedia; }
        }

        protected ManagedAudioMedia mManagedAudioMedia2
        {
            get { return mMedia2 as ManagedAudioMedia; }
        }

        protected ManagedAudioMedia mManagedAudioMedia3
        {
            get { return mMedia3 as ManagedAudioMedia; }
        }

        public override void SetUp()
        {
            SetUpMedia();
        }

        [Test, Description("Tests ValueEquals focusing on the language property")]
        public void ValueEquals_LangEquality()
        {
            mManagedAudioMedia1.Language = null;
            mManagedAudioMedia2.Language = null;
            Assert.IsTrue(mManagedAudioMedia1.ValueEquals(mManagedAudioMedia2),
                          "medias with same (null) lang should be equal");
            mManagedAudioMedia1.Language = "en";
            mManagedAudioMedia2.Language = "en";
            Assert.IsTrue(mManagedAudioMedia1.ValueEquals(mManagedAudioMedia2),
                          "medias with same (\"en\") lang should be equal");
            mManagedAudioMedia2.Language = "fr";
            Assert.IsFalse(mManagedAudioMedia1.ValueEquals(mManagedAudioMedia2),
                           "medias with different lang shouldn't be equal");
        }

        [Test, Description("Tests ValueEquals focusing on the media data")]
        public void ValueEquals_MediaData()
        {
            AudioMediaData data1 = mPresentation.MediaDataFactory.Create<codec.WavAudioMediaData>();
            AudioMediaData data2 = mPresentation.MediaDataFactory.Create<codec.WavAudioMediaData>();
            mManagedAudioMedia1.AudioMediaData = data1;
            mManagedAudioMedia2.AudioMediaData = data1;
            Assert.IsTrue(mManagedAudioMedia1.ValueEquals(mManagedAudioMedia2),
                          "two medias with the same data should be equal");
            mManagedAudioMedia2.AudioMediaData = data2;
            Assert.IsTrue(data1.ValueEquals(data2), "[Pre-Condition] media datas should be equal");
            Assert.IsTrue(mManagedAudioMedia1.ValueEquals(mManagedAudioMedia2),
                          "two medias with equal data should be equal");
            data2.Name = "foo";
            Assert.IsFalse(data1.ValueEquals(data2), "[Pre-Condition] media datas shouldn't be equal");
            Assert.IsFalse(data1.ValueEquals(data2) && !mManagedAudioMedia1.ValueEquals(mManagedAudioMedia2),
                           "two medias with different data shouldn't be equal");
        }

        #region Media tests

        [Test]
        public override void Language_Basics()
        {
            base.Language_Basics();
        }

        [Test]
        [ExpectedException(typeof (exception.MethodParameterIsEmptyStringException))]
        public override void Language_EmptyString()
        {
            base.Language_EmptyString();
        }

        private void AppendAudioData(string filename, ManagedAudioMedia amd)
        {
            PCMDataInfo info = GetInfo("audiotest1-mono-22050Hz-16bits.wav");
            mManagedAudioMedia1.AudioMediaData.PCMFormat = info;
            Stream fs = GetRawStream("audiotest1-mono-22050Hz-16bits.wav");
            try
            {
                amd.AudioMediaData.AppendAudioData(fs, info.Duration);
            }
            finally
            {
                fs.Close();
            }
        }

        [Test]
        public override void Copy_ValueEqualsAndReferenceDiffers()
        {
            AppendAudioData("audiotest1-mono-22050Hz-16bits.wav", mManagedAudioMedia1);
            base.Copy_ValueEqualsAndReferenceDiffers();
        }

        [Test]
        public override void Export_ValueEqualsPresentationsOk()
        {
            AppendAudioData("audiotest1-mono-22050Hz-16bits.wav", mManagedAudioMedia1);
            base.Export_ValueEqualsPresentationsOk();
        }

        #endregion

        #region IXukAble tests

        [Test]
        public override void Xuk_RoundTrip()
        {
            base.Xuk_RoundTrip();
        }

        #endregion

        #region IValueEquatable tests

        [Test]
        public override void ValueEquals_Basics()
        {
            base.ValueEquals_Basics();
        }

        [Test]
        public override void ValueEquals_Language()
        {
            base.ValueEquals_Language();
        }

        [Test]
        public override void ValueEquals_NewCreatedEquals()
        {
            base.ValueEquals_NewCreatedEquals();
        }

        #endregion
    }
}