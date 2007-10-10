using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace urakawa.media
{
	[TestFixture, Description("Tests the ExternalTextMedia functionality")]
	public class ExternalTextMediaTests
	{
		protected Project mProject;
		protected Presentation mPresentation
		{
			get { return mProject.getPresentation(0); }
		}
		protected ExternalTextMedia mMedia1;
		protected ExternalTextMedia mMedia2;
		protected ExternalTextMedia mMedia3;

		[SetUp]
		public void setUp()
		{
			mProject = new Project();
			mPresentation.setRootUri(ProjectTests.SampleXukFileDirectoryUri);
			mMedia1 = mPresentation.getMediaFactory().createMedia(typeof(ExternalTextMedia).Name, ToolkitSettings.XUK_NS) as ExternalTextMedia;
			Assert.IsNotNull(mMedia1, "The MediaFactory could not create a {1}:{0}", typeof(ExternalTextMedia).Name, ToolkitSettings.XUK_NS);
			mMedia2 = mPresentation.getMediaFactory().createMedia(typeof(ExternalTextMedia).Name, ToolkitSettings.XUK_NS) as ExternalTextMedia;
			Assert.IsNotNull(mMedia2, "The MediaFactory could not create a {1}:{0}", typeof(ExternalTextMedia).Name, ToolkitSettings.XUK_NS);
			mMedia3 = mPresentation.getMediaFactory().createMedia(typeof(ExternalTextMedia).Name, ToolkitSettings.XUK_NS) as ExternalTextMedia;
			Assert.IsNotNull(mMedia3, "The MediaFactory could not create a {1}:{0}", typeof(ExternalTextMedia).Name, ToolkitSettings.XUK_NS);
		}

		[Test, Description("Testing valueEuqals basics")]
		public void valueEuqals_Basics()
		{
			mMedia2.setLanguage("da");//Ensure that mMedia2 differs from mMedia1 and mMedia3
			IValueEquatableBasicTestUtils.valueEquals_BasicTests<IMedia>(mMedia1, mMedia2, mMedia3);
		}

		[Test, Description("Testing copy basics")]
		public void copy_Basics()
		{
			mMedia1.setLanguage("da");
			mMedia1.setSrc("test.txt");
			IMediaTestUtils.copy_valueEqualsButReferenceDiffers(mMedia1);
		}

		[Test, Description("Testing export basics")]
		public void export_Basics()
		{
			mMedia1.setLanguage("da");
			mMedia1.setSrc("test.txt");
			Presentation destPres = mProject.getDataModelFactory().createPresentation();
			mProject.addPresentation(destPres);
			IMediaTestUtils.export_valueEqualsPresentationsOk(mMedia1, destPres);
			destPres.setRootUri(new Uri("http://localhost"));
			mMedia1.setSrc("http://localhost/test.txt");
			IMediaTestUtils.export_valueEqualsPresentationsOk(mMedia1, destPres);
		}

		[Test, Description("Testing getText with local relative src")]
		public void getText_localSrc()
		{
			mMedia1.setSrc("temp.txt");
			Uri file = new Uri(mPresentation.getRootUri(), mMedia1.getSrc());
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
				mMedia1.getText().StartsWith(text),
				"xuk.xsd does not as expected start with:\n{0}",
				text);
			if (System.IO.File.Exists(file.LocalPath)) System.IO.File.Delete(file.LocalPath);
		}

		[Test]
		[Description("Tests getText with an external http src")]
		[Explicit("Requires being online and takes a bit of time especially on slow connections")]
		public void getText_httpSrc()
		{
			mMedia1.setSrc("http://www.daisy.org/z3986/2005/ncx-2005-1.dtd");
			string expectedStart = "<!-- NCX 2005-1 DTD  2005-06-26";
			Assert.IsTrue(
				mMedia1.getText().StartsWith(expectedStart), 
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
			mMedia1.setSrc("filedoesnotexist.txt");
			mMedia1.getText();
		}

		[Test, Description("Testing setText with local relative src")]
		public void setText_localSrc()
		{
			mMedia1.setSrc("temp.txt");
			string text = "setText: Test line Ê¯Â∆ÿ≈@£$Ä\nSecond test line\tincluding a tab";
			mMedia1.setText(text);
			Assert.AreEqual(text, mMedia1.getText(), "The ExternalTextMedia did not return the expected text");
			Uri file = new Uri(mPresentation.getRootUri(), mMedia1.getSrc());
			Assert.IsTrue(System.IO.File.Exists(file.LocalPath), "The file '{0}' containing the data does not exist", file.LocalPath);
			System.IO.File.Delete(file.LocalPath);
		}

		[Test, Description("Testing setText with external http src - expected to throw an exception")]
		[ExpectedException(typeof(exception.CannotWriteToExternalFileException))]
		public void setText_httpSrc()
		{
			mMedia1.setSrc("http://www.daisy.org/z3986/2005/ncx-2005-1.dtd");
			mMedia1.setText("Text to replace ncx version 2005-1 DTD");

		}

		[Test, Description("Tests the basics of get/setLanguage")]
		public void language_Basics()
		{
			IMediaTestUtils.language_Basics(mMedia1);
		}

		[Test, Description("Tests if the expected exception occurs upon setting the language to an empty string")]
		[ExpectedException(typeof(exception.MethodParameterIsEmptyStringException))]
		public void setLanguage_EmptyString()
		{
			mMedia1.setLanguage("");
		}
	}
}
