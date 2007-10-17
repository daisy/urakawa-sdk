using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using NUnit.Framework;
using urakawa;
using urakawa.core;
using urakawa.property.channel;
using urakawa.media;
using urakawa.media.data.audio;

namespace urakawa.media.data.audio
{
	[TestFixture, Description("Tests the ManagedAudioMedia functionality")]
	public class ManagedAudioMediaTests : IMediaTests
	{
		public ManagedAudioMediaTests()
			: base(typeof(ManagedAudioMedia).Name, ToolkitSettings.XUK_NS)
		{
		}


		protected ManagedAudioMedia mManagedAudioMedia1 { get { return mMedia1 as ManagedAudioMedia; } }
		protected ManagedAudioMedia mManagedAudioMedia2 { get { return mMedia2 as ManagedAudioMedia; } }
		protected ManagedAudioMedia mManagedAudioMedia3 { get { return mMedia3 as ManagedAudioMedia; } }

		public override void setUp()
		{
			Uri projectDir = new Uri(ProjectTests.SampleXukFileDirectoryUri, "MediaTestsSample/");
			if (Directory.Exists(Path.Combine(projectDir.LocalPath, "Data")))
			{
				Directory.Delete(Path.Combine(projectDir.LocalPath, "Data"), true);
			}
			mProject = new Project();
			mPresentation.setRootUri(projectDir);
			setUpMedia();
		}

		[Test, Description("Tests valueEquals focusing on the language property")]
		public void valueEquals_LangEquality()
		{
			mManagedAudioMedia1.setLanguage(null);
			mManagedAudioMedia2.setLanguage(null);
			Assert.IsTrue(mManagedAudioMedia1.valueEquals(mManagedAudioMedia2), "medias with same (null) lang should be equal");
			mManagedAudioMedia1.setLanguage("en");
			mManagedAudioMedia2.setLanguage("en");
			Assert.IsTrue(mManagedAudioMedia1.valueEquals(mManagedAudioMedia2), "medias with same (\"en\") lang should be equal");
			mManagedAudioMedia2.setLanguage("fr");
			Assert.IsFalse(mManagedAudioMedia1.valueEquals(mManagedAudioMedia2), "medias with different lang shouldn't be equal");

		}

		[Test, Description("Tests valueEquals focusing on the media data")]
		public void valueEquals_MediaData()
		{
			MediaData data1 = mPresentation.getMediaDataFactory().createMediaData("WavAudioMediaData", urakawa.ToolkitSettings.XUK_NS);
			MediaData data2 = mPresentation.getMediaDataFactory().createMediaData("WavAudioMediaData", urakawa.ToolkitSettings.XUK_NS);
			mManagedAudioMedia1.setMediaData(data1);
			mManagedAudioMedia2.setMediaData(data1);
			Assert.IsTrue(mManagedAudioMedia1.valueEquals(mManagedAudioMedia2), "two medias with the same data should be equal");
			mManagedAudioMedia2.setMediaData(data2);
			Assert.IsTrue(data1.valueEquals(data2), "[Pre-Condition] media datas should be equal");
			Assert.IsTrue(mManagedAudioMedia1.valueEquals(mManagedAudioMedia2), "two medias with equal data should be equal");
			data2.setName("foo");
			Assert.IsFalse(data1.valueEquals(data2), "[Pre-Condition] media datas shouldn't be equal");
			Assert.IsFalse(data1.valueEquals(data2) && !mManagedAudioMedia1.valueEquals(mManagedAudioMedia2), "two medias with different data shouldn't be equal");
		}

		#region IMedia tests
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
		[Test]
		public override void copy_valueEqualsAndReferenceDiffers()
		{
			base.copy_valueEqualsAndReferenceDiffers();
		}
		[Test]
		public override void export_valueEqualsPresentationsOk()
		{
			base.export_valueEqualsPresentationsOk();
		}
		#endregion

		#region IXukAble tests
		[Test]
		public override void Xuk_RoundTrip()
		{
			base.Xuk_RoundTrip();
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
		public override void valueEquals_NewCreatedEquals()
		{
			base.valueEquals_NewCreatedEquals();
		}
		#endregion
	}
}
