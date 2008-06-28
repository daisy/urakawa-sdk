using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace urakawa.media
{
	[TestFixture, Description("Tests for ExternalImageMedia")]
	public class ExternalImageMediaTests : ExternalMediaTests
	{
		public ExternalImageMediaTests()
			: base(typeof(ExternalImageMedia).Name, ToolkitSettings.XUK_NS)
		{
		}

		protected ExternalImageMedia mExternalImageMedia1 { get { return mExternalMedia1 as ExternalImageMedia;}}
		protected ExternalImageMedia mExternalImageMedia2 { get { return mExternalMedia2 as ExternalImageMedia;}}
		protected ExternalImageMedia mExternalImageMedia3 { get { return mExternalMedia3 as ExternalImageMedia; } }

		#region ISized tests
		[Test]
		public void height_Basics()
		{
			Assert.AreEqual(0, mExternalImageMedia1.getHeight(), "Default image height must be 0");
			mExternalImageMedia1.setHeight(10);
			Assert.AreEqual(10, mExternalImageMedia1.getHeight(), "Unexpected getHeight return value");
			mExternalImageMedia1.setHeight(800);
			Assert.AreEqual(800, mExternalImageMedia1.getHeight(), "Unexpected getHeight return value");
			mExternalImageMedia1.setHeight(0);
			Assert.AreEqual(0, mExternalImageMedia1.getHeight(), "Unexpected getHeight return value");
		}
		[Test]
		[ExpectedException(typeof(exception.MethodParameterIsOutOfBoundsException))]
		public void setHeight_NegativeValue()
		{
			mExternalImageMedia1.setHeight(-10);
		}
		[Test]
		public void width_Basics()
		{
			Assert.AreEqual(0, mExternalImageMedia1.getWidth(), "Default image height must be 0");
			mExternalImageMedia1.setWidth(10);
			Assert.AreEqual(10, mExternalImageMedia1.getWidth(), "Unexpected getWidth return value");
			mExternalImageMedia1.setWidth(800);
			Assert.AreEqual(800, mExternalImageMedia1.getWidth(), "Unexpected getWidth return value");
			mExternalImageMedia1.setWidth(0);
			Assert.AreEqual(0, mExternalImageMedia1.getWidth(), "Unexpected getWidth return value");
		}
		[Test]
		[ExpectedException(typeof(exception.MethodParameterIsOutOfBoundsException))]
		public void setWidth_NegativeValue()
		{
			mExternalImageMedia1.setWidth(-10);
		}
		#endregion

		#region IValueEquatable tests
		[Test]
		public override void valueEquals_Basics()
		{
			base.valueEquals_Basics();
		}

		[Test]
		public override void valueEquals_NewCreatedEquals()
		{
			base.valueEquals_NewCreatedEquals();
		}

		[Test]
		public override void valueEquals_Language()
		{
			base.valueEquals_Language();
		}

		[Test]
		public override void valueEquals_Src()
		{
			base.valueEquals_Src();
		}

		[Test]
		public void valueEquals_Height()
		{
			mExternalImageMedia1.setHeight(0);
			mExternalImageMedia2.setHeight(40);
			Assert.IsFalse(
				mExternalImageMedia1.ValueEquals(mExternalImageMedia2),
				"ExternalImageMedia with different hight values can not be value equal");
			mExternalImageMedia1.setHeight(mExternalImageMedia2.getHeight());
			Assert.IsTrue(
				mExternalImageMedia1.ValueEquals(mExternalImageMedia2),
				"Expected ExternalImageMedia to be equal");
		}

		[Test]
		public void valueEquals_Width()
		{
			mExternalImageMedia1.setWidth(0);
			mExternalImageMedia2.setWidth(40);
			Assert.IsFalse(
				mExternalImageMedia1.ValueEquals(mExternalImageMedia2),
				"ExternalImageMedia with different hight values can not be value equal");
			mExternalImageMedia1.setWidth(mExternalImageMedia2.getWidth());
			Assert.IsTrue(
				mExternalImageMedia1.ValueEquals(mExternalImageMedia2),
				"Expected ExternalImageMedia to be equal");
		}

		#endregion

		#region IXukAble tests
		[Test]
		public override void Xuk_RoundTrip()
		{
			mExternalImageMedia1.setHeight(600);
			mExternalImageMedia1.setWidth(800);
			base.Xuk_RoundTrip();
		}
		#endregion

		#region IMedia tests

		[Test]
		public override void copy_valueEqualsAndReferenceDiffers()
		{
			mExternalImageMedia1.setHeight(300);
			mExternalImageMedia1.setWidth(200);
			base.copy_valueEqualsAndReferenceDiffers();
		}

		[Test]
		public override void export_valueEqualsPresentationsOk()
		{
			mExternalImageMedia1.setHeight(200);
			mExternalImageMedia1.setWidth(200);
			base.export_valueEqualsPresentationsOk();
		}

		[Test]
		public override void language_Basics()
		{
			base.language_Basics();
		}

		[Test]
		[ExpectedException(typeof(exception.MethodParameterIsEmptyStringException))]
		public override void setLanguage_EmptyString()
		{
			base.setLanguage_EmptyString();
		}

		#endregion

		#region ExternalMedia tests

		[Test]
		public override void src_Basics()
		{
			base.src_Basics();
		}

		[Test]
		[ExpectedException(typeof(exception.MethodParameterIsNullException))]
		public override void setSrc_NullValue()
		{
			base.setSrc_NullValue();
		}

		[Test]
		[ExpectedException(typeof(exception.MethodParameterIsEmptyStringException))]
		public override void setSrc_EmptyStringValue()
		{
			base.setSrc_EmptyStringValue();
		}

		[Test]
		public override void getUri_Basics()
		{
			base.getUri_Basics();
		}

		[Test]
		[ExpectedException(typeof(exception.InvalidUriException))]
		public override void getUri_SrcMalformedUri()
		{
			base.getUri_SrcMalformedUri();
		}

		#endregion

	}
}
