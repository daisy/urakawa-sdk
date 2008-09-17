using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using NUnit.Framework;
using urakawa.xuk;

namespace urakawa.media
{
    public abstract class IMediaTests
    {
        protected Media mMedia1;
        protected Media mMedia2;
        protected Media mMedia3;
        protected Project mProject;

        protected Presentation mPresentation
        {
            get { return mProject.GetPresentation(0); }
        }

        protected Type mDefaultMediaType;

        protected IMediaTests(Type defMediaTp)
        {
            mDefaultMediaType = defMediaTp;
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
            mMedia1 = mPresentation.MediaFactory.Create(mDefaultMediaType);
            Assert.IsNotNull(mMedia1, "The MediaFactory could not create a {1}", mDefaultMediaType);
            mMedia2 = mPresentation.MediaFactory.Create(mDefaultMediaType);
            Assert.IsNotNull(mMedia2, "The MediaFactory could not create a {1}", mDefaultMediaType);
            mMedia3 = mPresentation.MediaFactory.Create(mDefaultMediaType);
            Assert.IsNotNull(mMedia3, "The MediaFactory could not create a {1}", mDefaultMediaType);
        }

        #region Media tests

        public virtual void Copy_ValueEqualsAndReferenceDiffers()
        {
            mMedia1.Language = "da-DK";
            Media cpM = mMedia1.Copy();
            Assert.IsTrue(mMedia1.ValueEquals(cpM), "A copy of a Media must have the same value as the original");
            Assert.IsFalse(Type.ReferenceEquals(mMedia1, cpM), "A copy must not be the same instance as the original");
        }

        public virtual void Export_ValueEqualsPresentationsOk()
        {
            mMedia1.Language = "en";
            Presentation destPres = mProject.PresentationFactory.Create();
            mProject.AddPresentation(destPres);
            Presentation sourcePres = mMedia1.MediaFactory.Presentation;
            Media expM = mMedia1.Export(destPres);
            Assert.AreEqual(sourcePres, mMedia1.MediaFactory.Presentation,
                            "Presentation of export source must not change");
            Assert.AreEqual(destPres, expM.MediaFactory.Presentation,
                            "Exported Media must belong to the destination Presentation");
            Assert.IsTrue(mMedia1.ValueEquals(expM), "The exported Media must have the same value as the source");
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
            Assert.IsTrue(mMedia1.ValueEquals(mMedia2), "Two newly created Media must be value equal");
        }


        public virtual void ValueEquals_Basics()
        {
            mMedia1.Language = "da";
            mMedia2.Language = "en";
            mMedia3.Language = mMedia1.Language;
            IValueEquatableBasicTestUtils.ValueEquals_BasicTests<Media>(mMedia1, mMedia2, mMedia3);
        }

        public virtual void ValueEquals_Language()
        {
            mMedia1.Language = "da";
            mMedia2.Language = "en";
            Assert.IsFalse(mMedia1.ValueEquals(mMedia2), "Media with different languages should not have equal values");
            mMedia2.Language = mMedia1.Language;
            Assert.IsTrue(mMedia1.ValueEquals(mMedia2), "Expected IMedias to have the same value");
        }

        #endregion

        #region IXukAble tests

        public virtual void Xuk_RoundTrip()
        {
            mMedia1.Language = "da";
            IXukAbleBasicTestUtils.XukInOut_RoundTrip<Media>(mMedia1, mPresentation.MediaFactory.Create,
                                                              mPresentation);
        }

        #endregion
    }
}