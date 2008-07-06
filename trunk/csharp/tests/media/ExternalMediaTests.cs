using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace urakawa.media
{
    public abstract class ExternalMediaTests : IMediaTests
    {
        protected ExternalMedia mExternalMedia1
        {
            get { return mMedia1 as ExternalMedia; }
        }

        protected ExternalMedia mExternalMedia2
        {
            get { return mMedia2 as ExternalMedia; }
        }

        protected ExternalMedia mExternalMedia3
        {
            get { return mMedia3 as ExternalMedia; }
        }

        protected ExternalMediaTests(string mediaXukLN, string mediaXukNS) : base(mediaXukLN, mediaXukNS)
        {
        }

        #region ExternalMedia tests

        public virtual void Src_Basics()
        {
            string src = ".";
            Assert.AreEqual(src, mExternalMedia1.Src, "The default src value is not '.'");
            src = "temp.txt";
            mExternalMedia1.Src = src;
            Assert.AreEqual(src, mExternalMedia1.Src, "Unexpected src value");
            src = "http://www.daisy.org/z3986/2005/ncx-2005-1.dtd";
            mExternalMedia1.Src = src;
            Assert.AreEqual(src, mExternalMedia1.Src, "Unexpected src value");
        }

        public virtual void Src_NullValue()
        {
            mExternalMedia1.Src = null;
        }

        public virtual void Src_EmptyStringValue()
        {
            mExternalMedia1.Src = "";
        }

        public virtual void Uri_Basics()
        {
            string src = mExternalMedia1.Src;
            mExternalMedia1.Src = src;
            Assert.AreEqual(new Uri(mPresentation.RootUri, src), mExternalMedia1.Uri, "Unexpected getUri return value");
            src = "http://www.daisy.org/z3986/2005/ncx-2005-1.dtd";
            mExternalMedia1.Src = src;
            Assert.AreEqual(new Uri(mPresentation.RootUri, src), mExternalMedia1.Uri, "Unexpected getUri return value");
            src = "temp.txt";
            mExternalMedia1.Src = src;
            Assert.AreEqual(new Uri(mPresentation.RootUri, src), mExternalMedia1.Uri, "Unexpected getUri return value");
        }

        public virtual void Uri_SrcMalformedUri()
        {
            mExternalMedia1.Src = "1 2 3 4 5.txt~";
            Uri tmp = mExternalMedia1.Uri;
        }

        #endregion

        #region IMedia tests

        public override void Copy_ValueEqualsAndReferenceDiffers()
        {
            mExternalMedia1.Src = "tempCopy.txt";
            base.Copy_ValueEqualsAndReferenceDiffers();
        }

        public override void Export_ValueEqualsPresentationsOk()
        {
            mExternalMedia1.Src = "tempExport.txt";
            base.Export_ValueEqualsPresentationsOk();
        }

        #endregion

        #region IValueEquatable tests

        public virtual void ValueEquals_Src()
        {
            mExternalMedia1.Src = "temp.txt";
            mExternalMedia2.Src = "http://www.daisy.org/z3986/2005/ncx-2005-1.dtd";
            Assert.IsFalse(mExternalMedia1.ValueEquals(mExternalMedia2),
                           "ExternalTextMedia with different src must not be value equal");
            mExternalMedia2.Src = mExternalMedia1.Src;
            Assert.IsTrue(mExternalMedia1.ValueEquals(mExternalMedia2), "Expected ExternalMedia to be equal");
        }

        #endregion

        #region IXukAble tests

        public override void Xuk_RoundTrip()
        {
            mExternalMedia1.Src = "temp.txt";
            base.Xuk_RoundTrip();
        }

        #endregion

        #region IValueEquatable tests

        public override void ValueEquals_Basics()
        {
            mExternalMedia1.Src = "temp.txt";
            mExternalMedia2.Src = "http://www.daisy.org/z3986/2005/ncx-2005-1.dtd";
            mExternalMedia3.Src = mExternalMedia1.Src;
            base.ValueEquals_Basics();
        }

        #endregion
    }
}