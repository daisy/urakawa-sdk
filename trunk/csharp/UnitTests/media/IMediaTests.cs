using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using NUnit.Framework;

namespace urakawa.media
{
	public abstract class IMediaTests
	{
		protected abstract IMedia mMedia1 { get;}
		protected abstract IMedia mMedia2 { get;}
		protected abstract IMedia mMedia3 { get;}
		protected Project mProject;
		protected Presentation mPresentation
		{
			get { return mProject.getPresentation(0); }
		}

		#region IMedia tests

		public virtual void copy_valueEqualsButReferenceDiffers()
		{
			mMedia1.setLanguage("da-DK");
			IMedia cpM = mMedia1.copy();
			Assert.IsTrue(mMedia1.valueEquals(cpM), "A copy of a IMedia must have the same value as the original");
			Assert.IsFalse(Type.ReferenceEquals(mMedia1, cpM), "A copy must not be the same instance as the original");
		}

		public virtual void export_valueEqualsPresentationsOk()
		{
			mMedia1.setLanguage("en");
			Presentation destPres = mProject.getDataModelFactory().createPresentation();
			mProject.addPresentation(destPres);
			Presentation sourcePres = mMedia1.getMediaFactory().getPresentation();
			IMedia expM = mMedia1.export(destPres);
			Assert.AreEqual(sourcePres, mMedia1.getMediaFactory().getPresentation(), "Presentation of export source must not change");
			Assert.AreEqual(destPres, expM.getMediaFactory().getPresentation(), "Exported IMedia must belong to the destination Presentation");
			Assert.IsTrue(mMedia1.valueEquals(expM), "The exported IMedia must have the same value as the source");
		}

		public virtual void language_Basics()
		{
			string text = "da-DK";
			mMedia1.setLanguage(text);
			Assert.AreEqual(text, mMedia1.getLanguage(), "getLanguage does not return the expected value '{0}'", text);
			text = "en";
			mMedia1.setLanguage(text);
			Assert.AreEqual(text, mMedia1.getLanguage(), "getLanguage does not return the expected value '{0}'", text);
			mMedia1.setLanguage(null);
			Assert.IsNull(mMedia1.getLanguage(), "getLanguage does not return the expected null value");
		}

		public virtual void setLanguage_EmptyString()
		{
			mMedia1.setLanguage("");
		}

		#endregion

		#region IValueEquatable tests


		public virtual void valueEquals_Basics()
		{
			mMedia1.setLanguage("da");
			mMedia2.setLanguage("en");
			mMedia3.setLanguage(mMedia1.getLanguage());
			IValueEquatableBasicTestUtils.valueEquals_BasicTests<IMedia>(mMedia1, mMedia2, mMedia3);
		}

		public virtual void valueEquals_Language()
		{
			mMedia1.setLanguage("da");
			mMedia2.setLanguage("en");
			Assert.IsFalse(mMedia1.valueEquals(mMedia2), "IMedia with different languages should not have equal values");
			mMedia2.setLanguage(mMedia1.getLanguage());
			Assert.IsTrue(mMedia1.valueEquals(mMedia2), "Expected IMedias to have the same value");
		}

		#endregion

		#region IXukAble tests

		public virtual void Xuk_RoundTrip()
		{
			mMedia1.setLanguage("da");
			IXukAbleBasicTestUtils.XukInOut_RoundTrip<IMedia>(mMedia1, mPresentation.getMediaFactory().createMedia, mPresentation);
		}

		#endregion

	}
}
