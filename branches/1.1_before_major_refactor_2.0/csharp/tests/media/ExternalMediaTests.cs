using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace urakawa.media
{
	public abstract class ExternalMediaTests : IMediaTests
	{

		protected ExternalMedia mExternalMedia1 { get { return mMedia1 as ExternalMedia; } }
		protected ExternalMedia mExternalMedia2 { get { return mMedia2 as ExternalMedia; } }
		protected ExternalMedia mExternalMedia3 { get { return mMedia3 as ExternalMedia; } }

		protected ExternalMediaTests(string mediaXukLN, string mediaXukNS) : base(mediaXukLN, mediaXukNS)
		{
		}

		#region ExternalMedia tests

		public virtual void src_Basics()
		{
			string src = ".";
			Assert.AreEqual(src, mExternalMedia1.getSrc(), "The default src value is not '.'");
			src = "temp.txt";
			mExternalMedia1.setSrc(src);
			Assert.AreEqual(src, mExternalMedia1.getSrc(), "Unexpected src value");
			src = "http://www.daisy.org/z3986/2005/ncx-2005-1.dtd";
			mExternalMedia1.setSrc(src);
			Assert.AreEqual(src, mExternalMedia1.getSrc(), "Unexpected src value");
		}

		public virtual void setSrc_NullValue()
		{
			mExternalMedia1.setSrc(null);
		}

		public virtual void setSrc_EmptyStringValue()
		{
			mExternalMedia1.setSrc("");
		}

		public virtual void getUri_Basics()
		{
			string src = mExternalMedia1.getSrc();
			mExternalMedia1.setSrc(src);
			Assert.AreEqual(new Uri(mPresentation.getRootUri(), src), mExternalMedia1.getUri(), "Unexpected getUri return value");
			src = "http://www.daisy.org/z3986/2005/ncx-2005-1.dtd";
			mExternalMedia1.setSrc(src);
			Assert.AreEqual(new Uri(mPresentation.getRootUri(), src), mExternalMedia1.getUri(), "Unexpected getUri return value");
			src = "temp.txt";
			mExternalMedia1.setSrc(src);
			Assert.AreEqual(new Uri(mPresentation.getRootUri(), src), mExternalMedia1.getUri(), "Unexpected getUri return value");
		}

		public virtual void getUri_SrcMalformedUri()
		{
			mExternalMedia1.setSrc("1 2 3 4 5.txt~");
			mExternalMedia1.getUri();
		}

		#endregion

		#region IMedia tests
		public override void copy_valueEqualsAndReferenceDiffers()
		{
			mExternalMedia1.setSrc("tempCopy.txt");
			base.copy_valueEqualsAndReferenceDiffers();
		}

		public override void export_valueEqualsPresentationsOk()
		{
			mExternalMedia1.setSrc("tempExport.txt");
			base.export_valueEqualsPresentationsOk();
		}
		#endregion

		#region IValueEquatable tests
		public virtual void valueEquals_Src()
		{
			mExternalMedia1.setSrc("temp.txt");
			mExternalMedia2.setSrc("http://www.daisy.org/z3986/2005/ncx-2005-1.dtd");
			Assert.IsFalse(mExternalMedia1.valueEquals(mExternalMedia2), "ExternalTextMedia with different src must not be value equal");
			mExternalMedia2.setSrc(mExternalMedia1.getSrc());
			Assert.IsTrue(mExternalMedia1.valueEquals(mExternalMedia2), "Expected ExternalMedia to be equal");
		}

		#endregion

		#region IXukAble tests
		public override void Xuk_RoundTrip()
		{
			mExternalMedia1.setSrc("temp.txt");
			base.Xuk_RoundTrip();
		}
		#endregion

		#region IValueEquatable tests

		public override void valueEquals_Basics()
		{
			mExternalMedia1.setSrc("temp.txt");
			mExternalMedia2.setSrc("http://www.daisy.org/z3986/2005/ncx-2005-1.dtd");
			mExternalMedia3.setSrc(mExternalMedia1.getSrc());
			base.valueEquals_Basics();
		}

		#endregion
	}
}
