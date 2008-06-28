using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using urakawa.media.timing;

namespace urakawa.media
{
	[TestFixture, Description("Tests for ExternalVideoMedia functionality")]
	public class ExternalVideoMediaTests : ExternalMediaTests
	{
		public ExternalVideoMediaTests()
			: base(typeof(ExternalVideoMedia).Name, ToolkitSettings.XUK_NS)
		{
		}

		protected ExternalVideoMedia mExternalVideoMedia1 { get { return mExternalMedia1 as ExternalVideoMedia; } }
		protected ExternalVideoMedia mExternalVideoMedia2 { get { return mExternalMedia2 as ExternalVideoMedia; } }
		protected ExternalVideoMedia mExternalVideoMedia3 { get { return mExternalMedia3 as ExternalVideoMedia; } }

		#region IContinuous tests

		[Test]
		public void getDuration_Basics()
		{
			Assert.IsTrue(
				mExternalVideoMedia1.getClipEnd().subtractTime(mExternalVideoMedia1.getClipBegin()).subtractTimeDelta(
					mExternalVideoMedia1.getDuration()).isEqualTo(Time.Zero),
				"Unexpected getDuration return value");
			mExternalVideoMedia1.setClipEnd(new Time(TimeSpan.FromSeconds(7.5)));
			Assert.IsTrue(
				mExternalVideoMedia1.getClipEnd().subtractTime(mExternalVideoMedia1.getClipBegin()).subtractTimeDelta(
					mExternalVideoMedia1.getDuration()).isEqualTo(Time.Zero),
				"Unexpected getDuration return value");
			mExternalVideoMedia1.setClipBegin(mExternalVideoMedia1.getClipEnd());
			Assert.IsTrue(
				mExternalVideoMedia1.getDuration().isEqualTo(new TimeDelta(0.0)),
				"Unexpected getDuration return value");
		}

		[Test]
		public void split_Basics()
		{
			testSplit(Time.Zero, new Time(TimeSpan.FromSeconds(10)), new Time(TimeSpan.FromSeconds(5)));
			testSplit(Time.Zero, new Time(TimeSpan.FromSeconds(10)), Time.Zero);
			testSplit(Time.Zero, new Time(TimeSpan.FromSeconds(10)), new Time(TimeSpan.FromSeconds(10)));
			testSplit(Time.Zero, Time.Zero, Time.Zero);
			testSplit(new Time(TimeSpan.FromSeconds(10)), new Time(TimeSpan.FromSeconds(10)), new Time(TimeSpan.FromSeconds(10)));
		}

		[Test]
		[ExpectedException(typeof(exception.MethodParameterIsNullException))]
		public void split_NullSplitPoint()
		{
			testSplit(Time.Zero, new Time(TimeSpan.FromSeconds(10)), null);
		}

		[Test]
		[ExpectedException(typeof(exception.MethodParameterIsOutOfBoundsException))]
		public void split_SplitPointAfterClip()
		{
			testSplit(Time.Zero, new Time(TimeSpan.FromSeconds(10)), new Time(TimeSpan.FromSeconds(20)));
		}

		[Test]
		[ExpectedException(typeof(exception.MethodParameterIsOutOfBoundsException))]
		public void split_SplitPointBeforeClip()
		{
			testSplit(new Time(TimeSpan.FromSeconds(10)), new Time(TimeSpan.FromSeconds(20)), Time.Zero);
		}



		private void testSplit(Time begin, Time end, Time split)
		{
			mExternalVideoMedia1.setClipEnd(Time.MaxValue);
			mExternalVideoMedia1.setClipBegin(Time.Zero);
			mExternalVideoMedia1.setClipBegin(begin);
			mExternalVideoMedia1.setClipEnd(end);
			ExternalVideoMedia secondPartVideo = mExternalVideoMedia1.split(split);
			Assert.IsTrue(
				begin.isEqualTo(mExternalVideoMedia1.getClipBegin()),
				"Unexpected clip begin, was '{0}' expected '{1}'",
				mExternalVideoMedia1.getClipBegin(), begin.ToString());
			Assert.IsTrue(
				split.isEqualTo(mExternalVideoMedia1.getClipEnd()),
				"Unexpected clip end, was '{0}' expected '{1}'",
				mExternalVideoMedia1.getClipBegin(), begin.ToString());
			Assert.IsTrue(
				split.isEqualTo(secondPartVideo.getClipBegin()),
				"Unexpected clip begin, was '{0}' expected '{1}'",
				secondPartVideo.getClipBegin(), split.ToString());
			Assert.IsTrue(
				end.isEqualTo(secondPartVideo.getClipEnd()),
				"Unexpected clip end, was '{0}' expected '{1}'",
				secondPartVideo.getClipEnd(), end.ToString());
		}

		#endregion

		#region IClipped tests

		[Test]
		public void clipBegin_Basics()
		{
			Assert.IsTrue(
				Time.Zero.isEqualTo(mExternalVideoMedia1.getClipBegin()),
				"The default value of clipBegin is {0} and not {1} as expected",
				mExternalVideoMedia1.getClipBegin().ToString(), Time.Zero.ToString());
			Time val = new Time(TimeSpan.FromSeconds(10));
			mExternalVideoMedia1.setClipBegin(val);
			Assert.IsTrue(val.isEqualTo(mExternalVideoMedia1.getClipBegin()), "Unexpected clipBegin return value");
			Assert.IsFalse(Type.ReferenceEquals(val, mExternalVideoMedia1.getClipBegin()), "ClipBegin was must not be set by reference");
			val = new Time(10);
			mExternalVideoMedia1.setClipBegin(val);
			Assert.IsTrue(val.isEqualTo(mExternalVideoMedia1.getClipBegin()), "Unexpected clipBegin return value");
			val = mExternalVideoMedia1.getClipEnd();
			mExternalVideoMedia1.setClipBegin(val);
			Assert.IsTrue(val.isEqualTo(mExternalVideoMedia1.getClipBegin()), "Unexpected clipBegin return value");
		}

		[Test]
		[ExpectedException(typeof(exception.MethodParameterIsNullException))]
		public void setClipBegin_NullValue()
		{
			mExternalVideoMedia1.setClipBegin(null);
		}

		[Test]
		[ExpectedException(typeof(exception.MethodParameterIsOutOfBoundsException))]
		public void setClipBegin_OutOfBounds_GtClipEnd()
		{
			mExternalVideoMedia1.setClipEnd(new Time(TimeSpan.FromSeconds(10)));
			mExternalVideoMedia1.setClipBegin(new Time(TimeSpan.FromSeconds(20)));
		}

		[Test]
		[ExpectedException(typeof(exception.MethodParameterIsOutOfBoundsException))]
		public void setClipBegin_OutOfBounds_Negative()
		{
			mExternalVideoMedia1.setClipBegin(new Time(TimeSpan.FromSeconds(-10)));
		}

		[Test]
		public void clipEnd_Basics()
		{
			Assert.IsTrue(
				Time.MaxValue.isEqualTo(mExternalVideoMedia1.getClipEnd()),
				"The default value of clipBegin is {0} and not {1} as expected",
				mExternalVideoMedia1.getClipEnd().ToString(), Time.MaxValue.ToString());
			Time val = new Time(TimeSpan.FromSeconds(10));
			mExternalVideoMedia1.setClipEnd(val);
			Assert.IsTrue(val.isEqualTo(mExternalVideoMedia1.getClipEnd()), "Unexpected clipBegin return value");
			Assert.IsFalse(Type.ReferenceEquals(val, mExternalVideoMedia1.getClipBegin()), "ClipBegin was must not be set by reference");
			val = new Time(10);
			mExternalVideoMedia1.setClipEnd(val);
			Assert.IsTrue(val.isEqualTo(mExternalVideoMedia1.getClipEnd()), "Unexpected clipBegin return value");
			val = mExternalVideoMedia1.getClipBegin();
			mExternalVideoMedia1.setClipEnd(val);
			Assert.IsTrue(val.isEqualTo(mExternalVideoMedia1.getClipEnd()), "Unexpected clipBegin return value");
		}

		[Test]
		[ExpectedException(typeof(exception.MethodParameterIsNullException))]
		public void clipEnd_NullValue()
		{
			mExternalVideoMedia1.setClipEnd(null);
		}

		[Test]
		[ExpectedException(typeof(exception.MethodParameterIsOutOfBoundsException))]
		public void clipEnd_OutOfBoundsValue_LtClipBegin()
		{
			mExternalVideoMedia1.setClipBegin(new Time(TimeSpan.FromSeconds(10)));
			mExternalVideoMedia1.setClipEnd(new Time(TimeSpan.FromSeconds(5)));
		}

		#endregion
		
		#region ISized tests
		[Test]
		public void height_Basics()
		{
			Assert.AreEqual(0, mExternalVideoMedia1.getHeight(), "Default image height must be 0");
			mExternalVideoMedia1.setHeight(10);
			Assert.AreEqual(10, mExternalVideoMedia1.getHeight(), "Unexpected getHeight return value");
			mExternalVideoMedia1.setHeight(800);
			Assert.AreEqual(800, mExternalVideoMedia1.getHeight(), "Unexpected getHeight return value");
			mExternalVideoMedia1.setHeight(0);
			Assert.AreEqual(0, mExternalVideoMedia1.getHeight(), "Unexpected getHeight return value");
		}
		[Test]
		[ExpectedException(typeof(exception.MethodParameterIsOutOfBoundsException))]
		public void setHeight_NegativeValue()
		{
			mExternalVideoMedia1.setHeight(-10);
		}
		[Test]
		public void width_Basics()
		{
			Assert.AreEqual(0, mExternalVideoMedia1.getWidth(), "Default image height must be 0");
			mExternalVideoMedia1.setWidth(10);
			Assert.AreEqual(10, mExternalVideoMedia1.getWidth(), "Unexpected getWidth return value");
			mExternalVideoMedia1.setWidth(800);
			Assert.AreEqual(800, mExternalVideoMedia1.getWidth(), "Unexpected getWidth return value");
			mExternalVideoMedia1.setWidth(0);
			Assert.AreEqual(0, mExternalVideoMedia1.getWidth(), "Unexpected getWidth return value");
		}
		[Test]
		[ExpectedException(typeof(exception.MethodParameterIsOutOfBoundsException))]
		public void setWidth_NegativeValue()
		{
			mExternalVideoMedia1.setWidth(-10);
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
		public void valueEquals_ClipBegin()
		{
			mExternalVideoMedia1.setClipBegin(Time.Zero);
			mExternalVideoMedia2.setClipBegin(new Time(TimeSpan.FromSeconds(10)));
			Assert.IsFalse(mExternalVideoMedia1.ValueEquals(mExternalVideoMedia2), "ExternalVideoMedia with different clipBegin can not be value equal");
			mExternalVideoMedia2.setClipBegin(mExternalVideoMedia1.getClipBegin());
			Assert.IsTrue(mExternalVideoMedia1.ValueEquals(mExternalVideoMedia2), "Expected ExternalVideoMedia to be value equal");
		}

		[Test]
		public void valueEquals_ClipEnd()
		{
			mExternalVideoMedia1.setClipEnd(Time.Zero);
			mExternalVideoMedia2.setClipEnd(new Time(TimeSpan.FromSeconds(10)));
			Assert.IsFalse(mExternalVideoMedia1.ValueEquals(mExternalVideoMedia2), "ExternalVideoMedia with different clipEnd can not be value equal");
			mExternalVideoMedia2.setClipEnd(mExternalVideoMedia1.getClipEnd());
			Assert.IsTrue(mExternalVideoMedia1.ValueEquals(mExternalVideoMedia2), "Expected ExternalVideoMedia to be value equal");
		}

		[Test]
		public void valueEquals_Height()
		{
			mExternalVideoMedia1.setHeight(0);
			mExternalVideoMedia2.setHeight(40);
			Assert.IsFalse(
				mExternalVideoMedia1.ValueEquals(mExternalVideoMedia2),
				"ExternalImageMedia with different hight values can not be value equal");
			mExternalVideoMedia1.setHeight(mExternalVideoMedia2.getHeight());
			Assert.IsTrue(
				mExternalVideoMedia1.ValueEquals(mExternalVideoMedia2),
				"Expected ExternalImageMedia to be equal");
		}

		[Test]
		public void valueEquals_Width()
		{
			mExternalVideoMedia1.setWidth(0);
			mExternalVideoMedia2.setWidth(40);
			Assert.IsFalse(
				mExternalVideoMedia1.ValueEquals(mExternalVideoMedia2),
				"ExternalImageMedia with different hight values can not be value equal");
			mExternalVideoMedia1.setWidth(mExternalVideoMedia2.getWidth());
			Assert.IsTrue(
				mExternalVideoMedia1.ValueEquals(mExternalVideoMedia2),
				"Expected ExternalImageMedia to be equal");
		}

		#endregion

		#region IXukAble tests
		[Test]
		public override void Xuk_RoundTrip()
		{
			mExternalVideoMedia1.setClipBegin(new Time(10000));
			mExternalVideoMedia1.setClipBegin(new Time(15000));
			mExternalVideoMedia1.setHeight(600);
			mExternalVideoMedia1.setWidth(800);
			base.Xuk_RoundTrip();
		}
		#endregion

		#region IMedia tests

		[Test]
		public override void copy_valueEqualsAndReferenceDiffers()
		{
			mExternalVideoMedia1.setClipBegin(new Time(10000));
			mExternalVideoMedia1.setClipBegin(new Time(15000));
			mExternalVideoMedia1.setHeight(600);
			mExternalVideoMedia1.setWidth(800);
			base.copy_valueEqualsAndReferenceDiffers();
		}

		[Test]
		public override void export_valueEqualsPresentationsOk()
		{
			mExternalVideoMedia1.setClipBegin(new Time(10000));
			mExternalVideoMedia1.setClipBegin(new Time(15000));
			mExternalVideoMedia1.setHeight(600);
			mExternalVideoMedia1.setWidth(800);
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
