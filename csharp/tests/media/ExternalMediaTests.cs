using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace urakawa.media
{
    public abstract class ExternalMediaTests : IMediaTests
    {
        protected ILocated mLocatedMedia1
        {
            get { return mMedia1 as ILocated; }
        }

        protected ILocated mLocatedMedia2
        {
            get { return mMedia2 as ILocated; }
        }

        protected ILocated mLocatedMedia3
        {
            get { return mMedia3 as ILocated; }
        }

        protected ExternalMediaTests(Type extMediaTp)
            : base(extMediaTp)
        {
        }

        #region ExternalMedia tests

        public virtual void Src_Basics()
        {
            string src = "file.ext";
            Assert.AreEqual(src, mLocatedMedia1.Src, "The default src value is not 'file.ext'");
            src = "temp.txt";
            mLocatedMedia1.Src = src;
            Assert.AreEqual(src, mLocatedMedia1.Src, "Unexpected src value");
            src = "http://www.daisy.org/z3986/2005/ncx-2005-1.dtd";
            mLocatedMedia1.Src = src;
            Assert.AreEqual(src, mLocatedMedia1.Src, "Unexpected src value");
        }

        public virtual void Src_NullValue()
        {
            mLocatedMedia1.Src = null;
        }

        public virtual void Src_EmptyStringValue()
        {
            mLocatedMedia1.Src = "";
        }

        public virtual void Uri_Basics()
        {
            string src = mLocatedMedia1.Src;
            mLocatedMedia1.Src = src;
            Assert.AreEqual(new Uri(mPresentation.RootUri, src), mLocatedMedia1.Uri, "Unexpected getUri return value");
            src = "http://www.daisy.org/z3986/2005/ncx-2005-1.dtd";
            mLocatedMedia1.Src = src;
            Assert.AreEqual(new Uri(mPresentation.RootUri, src), mLocatedMedia1.Uri, "Unexpected getUri return value");
            src = "temp.txt";
            mLocatedMedia1.Src = src;
            Assert.AreEqual(new Uri(mPresentation.RootUri, src), mLocatedMedia1.Uri, "Unexpected getUri return value");
        }

        public virtual void Uri_SrcMalformedUri()
        {
            mLocatedMedia1.Src = "1 2 3 4 5.txt~";
            Uri tmp = mLocatedMedia1.Uri;
        }

        #endregion

        #region Media tests

        public override void Copy_ValueEqualsAndReferenceDiffers()
        {
            mLocatedMedia1.Src = "tempCopy.txt";
            base.Copy_ValueEqualsAndReferenceDiffers();
        }

        public override void Export_ValueEqualsPresentationsOk()
        {
            mLocatedMedia1.Src = "tempExport.txt";
            base.Export_ValueEqualsPresentationsOk();
        }

        #endregion

        #region IValueEquatable tests

        public virtual void ValueEquals_Src()
        {
            mLocatedMedia1.Src = "temp.txt";
            mLocatedMedia2.Src = "http://www.daisy.org/z3986/2005/ncx-2005-1.dtd";
            Assert.IsFalse(mMedia1.ValueEquals(mMedia2),
                           "ExternalTextMedia with different src must not be value equal");
            mLocatedMedia2.Src = mLocatedMedia1.Src;
            Assert.IsTrue(mMedia1.ValueEquals(mMedia2), "Expected ExternalMedia to be equal");
        }

        #endregion

        #region IXukAble tests

        public override void Xuk_RoundTrip()
        {
            mLocatedMedia1.Src = "temp.txt";
            base.Xuk_RoundTrip();
        }

        #endregion

        #region IValueEquatable tests

        public override void ValueEquals_Basics()
        {
            mLocatedMedia1.Src = "temp.txt";
            mLocatedMedia2.Src = "http://www.daisy.org/z3986/2005/ncx-2005-1.dtd";
            mLocatedMedia3.Src = mLocatedMedia1.Src;
            base.ValueEquals_Basics();
        }

        #endregion
    }
}