using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace urakawa.media
{
	[TestFixture, Description("Tests for ExternalImageMedia")]
	public class ExternalImageMediaTests : ExternalMediaTests
	{

		protected override ExternalMedia mExternalMedia1
		{
			get { return mExternalImageMedia1; }
		}

		protected override ExternalMedia mExternalMedia2
		{
			get { return mExternalImageMedia2; }
		}

		protected override ExternalMedia mExternalMedia3
		{
			get { return mExternalImageMedia3; }
		}
		protected ExternalImageMedia mExternalImageMedia1;
		protected ExternalImageMedia mExternalImageMedia2;
		protected ExternalImageMedia mExternalImageMedia3;

		[SetUp]
		public void setUp()
		{
			mProject = new Project();
			mPresentation.setRootUri(ProjectTests.SampleXukFileDirectoryUri);
			mExternalImageMedia1 = mPresentation.getMediaFactory().createMedia(typeof(ExternalImageMedia).Name, ToolkitSettings.XUK_NS) as ExternalImageMedia;
			Assert.IsNotNull(mExternalImageMedia1, "The MediaFactory could not create a {1}:{0}", typeof(ExternalImageMedia).Name, ToolkitSettings.XUK_NS);
			mExternalImageMedia2 = mPresentation.getMediaFactory().createMedia(typeof(ExternalImageMedia).Name, ToolkitSettings.XUK_NS) as ExternalImageMedia;
			Assert.IsNotNull(mExternalImageMedia2, "The MediaFactory could not create a {1}:{0}", typeof(ExternalImageMedia).Name, ToolkitSettings.XUK_NS);
			mExternalImageMedia3 = mPresentation.getMediaFactory().createMedia(typeof(ExternalImageMedia).Name, ToolkitSettings.XUK_NS) as ExternalImageMedia;
			Assert.IsNotNull(mExternalImageMedia3, "The MediaFactory could not create a {1}:{0}", typeof(ExternalImageMedia).Name, ToolkitSettings.XUK_NS);
		}

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
				mExternalImageMedia1.valueEquals(mExternalImageMedia2),
				"ExternalImageMedia with different hight values can not be value equal");
			mExternalImageMedia1.setHeight(mExternalImageMedia2.getHeight());
			Assert.IsTrue(
				mExternalImageMedia1.valueEquals(mExternalImageMedia2),
				"Expected ExternalImageMedia to be equal");
		}

		[Test]
		public void valueEquals_Width()
		{
			mExternalImageMedia1.setWidth(0);
			mExternalImageMedia2.setWidth(40);
			Assert.IsFalse(
				mExternalImageMedia1.valueEquals(mExternalImageMedia2),
				"ExternalImageMedia with different hight values can not be value equal");
			mExternalImageMedia1.setWidth(mExternalImageMedia2.getWidth());
			Assert.IsTrue(
				mExternalImageMedia1.valueEquals(mExternalImageMedia2),
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
		public override void copy_valueEqualsButReferenceDiffers()
		{
			mExternalImageMedia1.setHeight(300);
			mExternalImageMedia1.setWidth(200);
			base.copy_valueEqualsButReferenceDiffers();
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
