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
	}
}
