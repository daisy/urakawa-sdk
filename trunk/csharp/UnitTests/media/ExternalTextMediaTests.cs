using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace urakawa.media
{
	[TestFixture, Description("Tests the ExternalTextMedia functionality")]
	public class ExternalTextMediaTests : ExternalMediaTests
	{
		protected override ExternalMedia mExternalMedia1
		{
			get { return mExternalTextMedia1; }
		}
		protected override ExternalMedia mExternalMedia2
		{
			get { return mExternalTextMedia2; }
		}
		protected override ExternalMedia mExternalMedia3
		{
			get { return mExternalTextMedia3; }
		}

		protected ExternalTextMedia mExternalTextMedia1;
		protected ExternalTextMedia mExternalTextMedia2;
		protected ExternalTextMedia mExternalTextMedia3;

		[SetUp]
		public void setUp()
		{
			mProject = new Project();
			mPresentation.setRootUri(ProjectTests.SampleXukFileDirectoryUri);
			mExternalTextMedia1 = mPresentation.getMediaFactory().createMedia(typeof(ExternalTextMedia).Name, ToolkitSettings.XUK_NS) as ExternalTextMedia;
			Assert.IsNotNull(mExternalTextMedia1, "The MediaFactory could not create a {1}:{0}", typeof(ExternalTextMedia).Name, ToolkitSettings.XUK_NS);
			mExternalTextMedia2 = mPresentation.getMediaFactory().createMedia(typeof(ExternalTextMedia).Name, ToolkitSettings.XUK_NS) as ExternalTextMedia;
			Assert.IsNotNull(mExternalTextMedia2, "The MediaFactory could not create a {1}:{0}", typeof(ExternalTextMedia).Name, ToolkitSettings.XUK_NS);
			mExternalTextMedia3 = mPresentation.getMediaFactory().createMedia(typeof(ExternalTextMedia).Name, ToolkitSettings.XUK_NS) as ExternalTextMedia;
			Assert.IsNotNull(mExternalTextMedia3, "The MediaFactory could not create a {1}:{0}", typeof(ExternalTextMedia).Name, ToolkitSettings.XUK_NS);
		}

		[Test, Description("Testing getText with local relative src")]
		public void getText_localSrc()
		{
			mExternalTextMedia1.setSrc("temp.txt");
			Uri file = new Uri(mPresentation.getRootUri(), mExternalTextMedia1.getSrc());
			System.IO.StreamWriter wr = new System.IO.StreamWriter(file.LocalPath, false);
			string text = "getText: Test line Ê¯Â∆ÿ≈@£$Ä\nSecond test line\tincluding a tab";
			try
			{
				wr.Write(text);
			}
			finally
			{
				wr.Close();
			}
			Assert.IsTrue(
				mExternalTextMedia1.getText().StartsWith(text),
				"xuk.xsd does not as expected start with:\n{0}",
				text);
			if (System.IO.File.Exists(file.LocalPath)) System.IO.File.Delete(file.LocalPath);
		}

		[Test]
		[Description("Tests getText with an external http src")]
		[Explicit("Requires being online and takes a bit of time especially on slow connections")]
		public void getText_httpSrc()
		{
			mExternalTextMedia1.setSrc("http://www.daisy.org/z3986/2005/ncx-2005-1.dtd");
			string expectedStart = "<!-- NCX 2005-1 DTD  2005-06-26";
			Assert.IsTrue(
				mExternalTextMedia1.getText().StartsWith(expectedStart), 
				"xuk.xsd does not as expected start with:\n{0}", 
				expectedStart);
		}

		[Test]
		[Description(
			"Tests that getText throws an exception.DataFileDoesNotExistException "
			+"when the references text file does not exist")]
		[ExpectedException(typeof(exception.CannotReadFromExternalFileException))]
		public void getText_invalidSrc()
		{
			mExternalTextMedia1.setSrc("filedoesnotexist.txt");
			mExternalTextMedia1.getText();
		}

		[Test, Description("Testing setText with local relative src")]
		public void setText_localSrc()
		{
			mExternalTextMedia1.setSrc("temp.txt");
			string text = "setText: Test line Ê¯Â∆ÿ≈@£$Ä\nSecond test line\tincluding a tab";
			mExternalTextMedia1.setText(text);
			Assert.AreEqual(text, mExternalTextMedia1.getText(), "The ExternalTextMedia did not return the expected text");
			Uri file = new Uri(mPresentation.getRootUri(), mExternalTextMedia1.getSrc());
			Assert.IsTrue(System.IO.File.Exists(file.LocalPath), "The file '{0}' containing the data does not exist", file.LocalPath);
			System.IO.File.Delete(file.LocalPath);
		}

		[Test, Description("Testing setText with external http src - expected to throw an exception")]
		[ExpectedException(typeof(exception.CannotWriteToExternalFileException))]
		public void setText_httpSrc()
		{
			mExternalTextMedia1.setSrc("http://www.daisy.org/z3986/2005/ncx-2005-1.dtd");
			mExternalTextMedia1.setText("Text to replace ncx version 2005-1 DTD");
		}

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

		#endregion

		#region IXukAble tests
		[Test]
		public override void Xuk_RoundTrip()
		{
			base.Xuk_RoundTrip();
		}
		#endregion

		#region IMedia tests

		[Test]
		public override void copy_valueEqualsButReferenceDiffers()
		{
			base.copy_valueEqualsButReferenceDiffers();
		}

		[Test]
		public override void export_valueEqualsPresentationsOk()
		{
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
