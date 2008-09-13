using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using urakawa.xuk;

namespace urakawa.media
{
    [TestFixture, Description("Tests the ExternalTextMedia functionality")]
    public class ExternalTextMediaTests : ExternalMediaTests
    {
        public ExternalTextMediaTests() : base(typeof (ExternalTextMedia).Name, XukAble.XUK_NS)
        {
        }

        protected ExternalTextMedia mExternalTextMedia1
        {
            get { return mLocatedMedia1 as ExternalTextMedia; }
        }

        protected ExternalTextMedia mExternalTextMedia2
        {
            get { return mLocatedMedia2 as ExternalTextMedia; }
        }

        protected ExternalTextMedia mExternalTextMedia3
        {
            get { return mLocatedMedia3 as ExternalTextMedia; }
        }

        [Test, Description("Testing GetText with local relative src")]
        public void Text_getWithLocalSrc()
        {
            mExternalTextMedia1.Src = "temp.txt";
            Uri file = new Uri(mPresentation.RootUri, mExternalTextMedia1.Src);
            System.IO.StreamWriter wr = new System.IO.StreamWriter(file.LocalPath, false);
            string text = "GetText: Test line Ê¯Â∆ÿ≈@£$Ä\nSecond test line\tincluding a tab";
            try
            {
                wr.Write(text);
            }
            finally
            {
                wr.Close();
            }
            Assert.IsTrue(
                mExternalTextMedia1.Text.StartsWith(text),
                "xuk.xsd does not as expected start with:\n{0}",
                text);
            if (System.IO.File.Exists(file.LocalPath)) System.IO.File.Delete(file.LocalPath);
        }

        [Test]
        [Description("Tests GetText with an external http src")]
        [Explicit("Requires being online and takes a bit of time especially on slow connections")]
        public void Text_GetWithHttpSrc()
        {
            mExternalTextMedia1.Src = "http://www.daisy.org/z3986/2005/ncx-2005-1.dtd";
            string expectedStart = "<!-- NCX 2005-1 DTD  2005-06-26";
            Assert.IsTrue(
                mExternalTextMedia1.Text.StartsWith(expectedStart),
                "xuk.xsd does not as expected start with:\n{0}",
                expectedStart);
        }

        [Test]
        [Description(
            "Tests that GetText throws an exception.DataMissingException "
            + "when the references text file does not exist")]
        [ExpectedException(typeof (exception.CannotReadFromExternalFileException))]
        public void Text_GetWithInvalidSrc()
        {
            mExternalTextMedia1.Src = "filedoesnotexist.txt";
            string tmp = mExternalTextMedia1.Text;
        }

        [Test, Description("Testing SetText with local relative src")]
        public void Text_SetWithLocalSrc()
        {
            mExternalTextMedia1.Src = "temp.txt";
            string text = "SetText: Test line Ê¯Â∆ÿ≈@£$Ä\nSecond test line\tincluding a tab";
            mExternalTextMedia1.Text = text;
            Assert.AreEqual(text, mExternalTextMedia1.Text, "The ExternalTextMedia did not return the expected text");
            Uri file = new Uri(mPresentation.RootUri, mExternalTextMedia1.Src);
            Assert.IsTrue(System.IO.File.Exists(file.LocalPath), "The file '{0}' containing the data does not exist",
                          file.LocalPath);
            System.IO.File.Delete(file.LocalPath);
        }

        [Test, Description("Testing SetText with external http src - expected to throw an exception")]
        [ExpectedException(typeof (exception.CannotWriteToExternalFileException))]
        public void Text_SetWithHttpSrc()
        {
            mExternalTextMedia1.Src = "http://www.daisy.org/z3986/2005/ncx-2005-1.dtd";
            mExternalTextMedia1.Text = "Text to replace ncx version 2005-1 DTD";
        }

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

        #endregion

        #region IXukAble tests

        [Test]
        public override void Xuk_RoundTrip()
        {
            base.Xuk_RoundTrip();
        }

        #endregion

        #region Media tests

        [Test]
        public override void Copy_ValueEqualsAndReferenceDiffers()
        {
            base.Copy_ValueEqualsAndReferenceDiffers();
        }

        [Test]
        public override void Export_ValueEqualsPresentationsOk()
        {
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