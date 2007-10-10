using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace urakawa.media
{
	public static class IMediaTestUtils
	{
		public static void copy_valueEqualsButReferenceDiffers(IMedia m)
		{
			IMedia cpM = m.copy();
			Assert.IsTrue(m.valueEquals(cpM), "A copy of a IMedia must have the same value as the original");
			Assert.IsFalse(Type.ReferenceEquals(m, cpM), "A copy must not be the same instance as the original");
		}

		public static void export_valueEqualsPresentationsOk(IMedia m, Presentation destPres)
		{
			Presentation mPres = m.getMediaFactory().getPresentation();
			IMedia expM = m.export(destPres);
			Assert.AreEqual(mPres, m.getMediaFactory().getPresentation(), "Presentation of export source must not change");
			Assert.AreEqual(destPres, expM.getMediaFactory().getPresentation(), "Exported IMedia must belong to the destination Presentation");
			Assert.IsTrue(m.valueEquals(expM), "The exported IMedia must have the same value as the source");
		}

		public static void language_Basics(IMedia m)
		{
			string text = "da-DK";
			m.setLanguage(text);
			Assert.AreEqual(text, m.getLanguage(), "getLanguage does not return the expected value '{0}'", text);
			text = "en";
			m.setLanguage(text);
			Assert.AreEqual(text, m.getLanguage(), "getLanguage does not return the expected value '{0}'", text);
			m.setLanguage(null);
			Assert.IsNull(text, "getLanguage does not return the expected null value");
		}
	}
}
