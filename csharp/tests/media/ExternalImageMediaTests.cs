using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using urakawa.xuk;

namespace urakawa.media
{
    [TestFixture]
    public class ExternalImageMediaTests : ExternalMediaTests
    {
        public ExternalImageMediaTests()
            : base(typeof (ExternalImageMedia))
        {
        }

        protected ExternalImageMedia mExternalImageMedia1
        {
            get { return mLocatedMedia1 as ExternalImageMedia; }
        }

        protected ExternalImageMedia mExternalImageMedia2
        {
            get { return mLocatedMedia2 as ExternalImageMedia; }
        }

        protected ExternalImageMedia mExternalImageMedia3
        {
            get { return mLocatedMedia3 as ExternalImageMedia; }
        }

        #region ISized tests

        [Test]
        public void Height_Basics()
        {
            Assert.AreEqual(0, mExternalImageMedia1.Height, "Default image height must be 0");
            mExternalImageMedia1.Height = 10;
            Assert.AreEqual(10, mExternalImageMedia1.Height, "Unexpected getHeight return value");
            mExternalImageMedia1.Height = 800;
            Assert.AreEqual(800, mExternalImageMedia1.Height, "Unexpected getHeight return value");
            mExternalImageMedia1.Height = 0;
            Assert.AreEqual(0, mExternalImageMedia1.Height, "Unexpected getHeight return value");
        }

        [Test]
        [ExpectedException(typeof (exception.MethodParameterIsOutOfBoundsException))]
        public void Height_NegativeValue()
        {
            mExternalImageMedia1.Height = -10;
        }

        [Test]
        public void Width_Basics()
        {
            Assert.AreEqual(0, mExternalImageMedia1.Width, "Default image height must be 0");
            mExternalImageMedia1.Width = 10;
            Assert.AreEqual(10, mExternalImageMedia1.Width, "Unexpected getWidth return value");
            mExternalImageMedia1.Width = 800;
            Assert.AreEqual(800, mExternalImageMedia1.Width, "Unexpected getWidth return value");
            mExternalImageMedia1.Width = 0;
            Assert.AreEqual(0, mExternalImageMedia1.Width, "Unexpected getWidth return value");
        }

        [Test]
        [ExpectedException(typeof (exception.MethodParameterIsOutOfBoundsException))]
        public void Width_NegativeValue()
        {
            mExternalImageMedia1.Width = -10;
        }

        #endregion

        #region IValueEquatable tests

        [Test]
        public override void ValueEquals_Basics()
        {
            base.ValueEquals_Basics();
        }

        [Test]
        public override void ValueEquals_NewCreatedEquals()
        {
            base.ValueEquals_NewCreatedEquals();
        }

        [Test]
        public override void ValueEquals_Language()
        {
            base.ValueEquals_Language();
        }

        [Test]
        public override void ValueEquals_Src()
        {
            base.ValueEquals_Src();
        }

        [Test]
        public void ValueEquals_Height()
        {
            mExternalImageMedia1.Height = 0;
            mExternalImageMedia2.Height = 40;
            Assert.IsFalse(
                mExternalImageMedia1.ValueEquals(mExternalImageMedia2),
                "ExternalImageMedia with different hight values can not be value equal");
            mExternalImageMedia1.Height = mExternalImageMedia2.Height;
            Assert.IsTrue(
                mExternalImageMedia1.ValueEquals(mExternalImageMedia2),
                "Expected ExternalImageMedia to be equal");
        }

        [Test]
        public void ValueEquals_Width()
        {
            mExternalImageMedia1.Width = 0;
            mExternalImageMedia2.Width = 40;
            Assert.IsFalse(
                mExternalImageMedia1.ValueEquals(mExternalImageMedia2),
                "ExternalImageMedia with different hight values can not be value equal");
            mExternalImageMedia1.Width = mExternalImageMedia2.Width;
            Assert.IsTrue(
                mExternalImageMedia1.ValueEquals(mExternalImageMedia2),
                "Expected ExternalImageMedia to be equal");
        }

        #endregion

        #region IXukAble tests

        [Test]
        public override void Xuk_RoundTrip()
        {
            mExternalImageMedia1.Height = 600;
            mExternalImageMedia1.Width = 800;
            base.Xuk_RoundTrip();
        }

        #endregion

        #region Media tests

        [Test]
        public override void Copy_ValueEqualsAndReferenceDiffers()
        {
            mExternalImageMedia1.Height = 300;
            mExternalImageMedia1.Width = 200;
            base.Copy_ValueEqualsAndReferenceDiffers();
        }

        [Test]
        public override void Export_ValueEqualsPresentationsOk()
        {
            mExternalImageMedia1.Height = 200;
            mExternalImageMedia1.Width = 200;
            base.Export_ValueEqualsPresentationsOk();
        }

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

        #endregion

        #region ExternalMedia tests

        [Test]
        public override void Src_Basics()
        {
            base.Src_Basics();
        }

        [Test]
        [ExpectedException(typeof (exception.MethodParameterIsNullException))]
        public override void Src_NullValue()
        {
            base.Src_NullValue();
        }

        [Test]
        [ExpectedException(typeof (exception.MethodParameterIsEmptyStringException))]
        public override void Src_EmptyStringValue()
        {
            base.Src_EmptyStringValue();
        }

        [Test]
        public override void Uri_Basics()
        {
            base.Uri_Basics();
        }

        [Test]
        [ExpectedException(typeof (exception.InvalidUriException))]
        public override void Uri_SrcMalformedUri()
        {
            base.Uri_SrcMalformedUri();
        }

        #endregion
    }
}