using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using NUnit.Framework;

namespace urakawa.media
{
    public abstract class IMediaTests
    {
        protected IMedia mMedia1;
        protected IMedia mMedia2;
        protected IMedia mMedia3;
        protected Project mProject;

        protected Presentation mPresentation
        {
            get { return mProject.GetPresentation(0); }
        }

        protected string mDefaultMediaXukLocalName;
        protected string mDefaultMediaXukNamespaceUri;

        protected IMediaTests(string mediaXukLN, string mediaXukNS)
        {
            mDefaultMediaXukLocalName = mediaXukLN;
            mDefaultMediaXukNamespaceUri = mediaXukNS;
        }

        [SetUp]
        public virtual void SetUp()
        {
            mProject = new Project();
            mProject.AddNewPresentation();
            mPresentation.RootUri = ProjectTests.SampleXukFileDirectoryUri;
            SetUpMedia();
        }

        public void SetUpMedia()
        {
            mMedia1 = mPresentation.MediaFactory.CreateMedia(mDefaultMediaXukLocalName, mDefaultMediaXukNamespaceUri);
            Assert.IsNotNull(mMedia1, "The MediaFactory could not create a {1}:{0}", typeof (ExternalAudioMedia).Name,
                             ToolkitSettings.XUK_NS);
            mMedia2 = mPresentation.MediaFactory.CreateMedia(mDefaultMediaXukLocalName, mDefaultMediaXukNamespaceUri);
            Assert.IsNotNull(mMedia2, "The MediaFactory could not create a {1}:{0}", typeof (ExternalAudioMedia).Name,
                             ToolkitSettings.XUK_NS);
            mMedia3 = mPresentation.MediaFactory.CreateMedia(mDefaultMediaXukLocalName, mDefaultMediaXukNamespaceUri);
            Assert.IsNotNull(mMedia3, "The MediaFactory could not create a {1}:{0}", typeof (ExternalAudioMedia).Name,
                             ToolkitSettings.XUK_NS);
        }

        #region IMedia tests

        public virtual void Copy_ValueEqualsAndReferenceDiffers()
        {
            mMedia1.Language = "da-DK";
            IMedia cpM = mMedia1.Copy();
            Assert.IsTrue(mMedia1.ValueEquals(cpM), "A copy of a IMedia must have the same value as the original");
            Assert.IsFalse(Type.ReferenceEquals(mMedia1, cpM), "A copy must not be the same instance as the original");
        }

        public virtual void Export_ValueEqualsPresentationsOk()
        {
            mMedia1.Language = "en";
            Presentation destPres = mProject.DataModelFactory.CreatePresentation();
            mProject.AddPresentation(destPres);
            Presentation sourcePres = mMedia1.MediaFactory.Presentation;
            IMedia expM = mMedia1.Export(destPres);
            Assert.AreEqual(sourcePres, mMedia1.MediaFactory.Presentation,
                            "Presentation of export source must not change");
            Assert.AreEqual(destPres, expM.MediaFactory.Presentation,
                            "Exported IMedia must belong to the destination Presentation");
            Assert.IsTrue(mMedia1.ValueEquals(expM), "The exported IMedia must have the same value as the source");
        }

        public virtual void Language_Basics()
        {
            string text = "da-DK";
            mMedia1.Language = text;
            Assert.AreEqual(text, mMedia1.Language, "getLanguage does not return the expected value '{0}'", text);
            text = "en";
            mMedia1.Language = text;
            Assert.AreEqual(text, mMedia1.Language, "getLanguage does not return the expected value '{0}'", text);
            mMedia1.Language = null;
            Assert.IsNull(mMedia1.Language, "getLanguage does not return the expected null value");
        }

        public virtual void Language_EmptyString()
        {
            mMedia1.Language = "";
        }

        #endregion

        #region IValueEquatable tests

        public virtual void ValueEquals_NewCreatedEquals()
        {
            Assert.IsTrue(mMedia1.ValueEquals(mMedia2), "Two newly created IMedia must be value equal");
        }


        public virtual void ValueEquals_Basics()
        {
            mMedia1.Language = "da";
            mMedia2.Language = "en";
            mMedia3.Language = mMedia1.Language;
            IValueEquatableBasicTestUtils.ValueEquals_BasicTests<IMedia>(mMedia1, mMedia2, mMedia3);
        }

        public virtual void ValueEquals_Language()
        {
            mMedia1.Language = "da";
            mMedia2.Language = "en";
            Assert.IsFalse(mMedia1.ValueEquals(mMedia2), "IMedia with different languages should not have equal values");
            mMedia2.Language = mMedia1.Language;
            Assert.IsTrue(mMedia1.ValueEquals(mMedia2), "Expected IMedias to have the same value");
        }

        #endregion

        #region IXukAble tests

        public virtual void Xuk_RoundTrip()
        {
            mMedia1.Language = "da";
            IXukAbleBasicTestUtils.XukInOut_RoundTrip<IMedia>(mMedia1, mPresentation.MediaFactory.CreateMedia,
                                                              mPresentation);
        }

        #endregion
    }
}