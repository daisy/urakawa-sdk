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

namespace urakawa.media.data.audio
{
    [TestFixture, Description("Tests the ManagedAudioMedia functionality")]
    public class ManagedAudioMediaTests
    {		
        protected Project mProject;
        protected Presentation mPresentation
        {
            get { return mProject.getPresentation(0); }
        }
        protected ManagedAudioMedia mMedia1;
        protected ManagedAudioMedia mMedia2;

        [SetUp]
        public void setUp()
        {
            Uri projectDir = new Uri(ProjectTests.SampleXukFileDirectoryUri, "MediaTestsSample/");
            if (Directory.Exists(Path.Combine(projectDir.LocalPath, "Data")))
            {
                Directory.Delete(Path.Combine(projectDir.LocalPath, "Data"), true);
            }
            mProject = new Project();
						mPresentation.setRootUri(projectDir);
            mMedia1 = mPresentation.getMediaFactory().createMedia("ManagedAudioMedia", urakawa.ToolkitSettings.XUK_NS) as ManagedAudioMedia;
            Assert.IsNotNull(mMedia1, "Could not create a ManagedAudioMedia");
            mMedia2 = mPresentation.getMediaFactory().createMedia("ManagedAudioMedia", urakawa.ToolkitSettings.XUK_NS) as ManagedAudioMedia;
            Assert.IsNotNull(mMedia2, "Could not create a ManagedAudioMedia");
        }

        [Test, Description("Tests valueEquals for basic medias")]
        public void valueEquals_Basics()
        {
            Assert.IsFalse(mMedia1.valueEquals(null), "a created media shouldn't equal null");
            Assert.IsTrue(mMedia1.valueEquals(mMedia1),"a media should equal itself");
            Assert.IsTrue(mMedia1.valueEquals(mMedia2),"two identically created medias should be equal");
        }

        [Test, Description("Tests valueEquals focusing on the language property")]
        public void valueEquals_LangEquality()
        {
            mMedia1.setLanguage(null);
            mMedia2.setLanguage(null);
            Assert.IsTrue(mMedia1.valueEquals(mMedia2), "medias with same (null) lang should be equal");
            mMedia1.setLanguage("en");
            mMedia2.setLanguage("en");
            Assert.IsTrue(mMedia1.valueEquals(mMedia2), "medias with same (\"en\") lang should be equal");
            mMedia2.setLanguage("fr");
            Assert.IsFalse(mMedia1.valueEquals(mMedia2), "medias with different lang shouldn't be equal");

        }

        [Test, Description("Tests valueEquals focusing on the media data")]
        public void valueEquals_MediaData()
        {
            MediaData data1 = mPresentation.getMediaDataFactory().createMediaData("WavAudioMediaData", urakawa.ToolkitSettings.XUK_NS);
            MediaData data2 = mPresentation.getMediaDataFactory().createMediaData("WavAudioMediaData", urakawa.ToolkitSettings.XUK_NS);
            mMedia1.setMediaData(data1);
            mMedia2.setMediaData(data1);
            Assert.IsTrue(mMedia1.valueEquals(mMedia2), "two medias with the same data should be equal");
            mMedia2.setMediaData(data2);
            Assert.IsTrue(data1.valueEquals(data2), "[Pre-Condition] media datas should be equal");
            Assert.IsTrue(mMedia1.valueEquals(mMedia2), "two medias with equal data should be equal");
            data2.setName("foo");
            Assert.IsFalse(data1.valueEquals(data2), "[Pre-Condition] media datas shouldn't be equal");
            Assert.IsFalse(data1.valueEquals(data2) && !mMedia1.valueEquals(mMedia2), "two medias with different data shouldn't be equal");
            
        }
    }
}
