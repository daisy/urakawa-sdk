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
				mExternalVideoMedia1.ClipEnd.subtractTime(mExternalVideoMedia1.ClipBegin).subtractTimeDelta(
					mExternalVideoMedia1.Duration).isEqualTo(Time.Zero),
				"Unexpected GetDuration return value");
			mExternalVideoMedia1.ClipEnd = new Time(TimeSpan.FromSeconds(7.5));
			Assert.IsTrue(
				mExternalVideoMedia1.ClipEnd.subtractTime(mExternalVideoMedia1.ClipBegin).subtractTimeDelta(
					mExternalVideoMedia1.Duration).isEqualTo(Time.Zero),
				"Unexpected GetDuration return value");
			mExternalVideoMedia1.ClipBegin = mExternalVideoMedia1.ClipEnd;
			Assert.IsTrue(
				mExternalVideoMedia1.Duration.isEqualTo(new TimeDelta(0.0)),
				"Unexpected GetDuration return value");
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
			mExternalVideoMedia1.ClipEnd = Time.MaxValue;
			mExternalVideoMedia1.ClipBegin = Time.Zero;
			mExternalVideoMedia1.ClipBegin = begin;
			mExternalVideoMedia1.ClipEnd = end;
			ExternalVideoMedia secondPartVideo = mExternalVideoMedia1.Split(split);
			Assert.IsTrue(
				begin.isEqualTo(mExternalVideoMedia1.ClipBegin),
				"Unexpected clip begin, was '{0}' expected '{1}'",
				mExternalVideoMedia1.ClipBegin, begin.ToString());
			Assert.IsTrue(
				split.isEqualTo(mExternalVideoMedia1.ClipEnd),
				"Unexpected clip end, was '{0}' expected '{1}'",
				mExternalVideoMedia1.ClipBegin, begin.ToString());
			Assert.IsTrue(
				split.isEqualTo(secondPartVideo.ClipBegin),
				"Unexpected clip begin, was '{0}' expected '{1}'",
				secondPartVideo.ClipBegin, split.ToString());
			Assert.IsTrue(
				end.isEqualTo(secondPartVideo.ClipEnd),
				"Unexpected clip end, was '{0}' expected '{1}'",
				secondPartVideo.ClipEnd, end.ToString());
		}

		#endregion

		#region IClipped tests

		[Test]
		public void clipBegin_Basics()
		{
			Assert.IsTrue(
				Time.Zero.isEqualTo(mExternalVideoMedia1.ClipBegin),
				"The default value of clipBegin is {0} and not {1} as expected",
				mExternalVideoMedia1.ClipBegin.ToString(), Time.Zero.ToString());
			Time val = new Time(TimeSpan.FromSeconds(10));
			mExternalVideoMedia1.ClipBegin = val;
			Assert.IsTrue(val.isEqualTo(mExternalVideoMedia1.ClipBegin), "Unexpected clipBegin return value");
			Assert.IsFalse(Type.ReferenceEquals(val, mExternalVideoMedia1.ClipBegin), "ClipBegin was must not be set by reference");
			val = new Time(10);
			mExternalVideoMedia1.ClipBegin = val;
			Assert.IsTrue(val.isEqualTo(mExternalVideoMedia1.ClipBegin), "Unexpected clipBegin return value");
			val = mExternalVideoMedia1.ClipEnd;
			mExternalVideoMedia1.ClipBegin = val;
			Assert.IsTrue(val.isEqualTo(mExternalVideoMedia1.ClipBegin), "Unexpected clipBegin return value");
		}

		[Test]
		[ExpectedException(typeof(exception.MethodParameterIsNullException))]
		public void setClipBegin_NullValue()
		{
			mExternalVideoMedia1.ClipBegin = null;
		}

		[Test]
		[ExpectedException(typeof(exception.MethodParameterIsOutOfBoundsException))]
		public void setClipBegin_OutOfBounds_GtClipEnd()
		{
			mExternalVideoMedia1.ClipEnd = new Time(TimeSpan.FromSeconds(10));
			mExternalVideoMedia1.ClipBegin = new Time(TimeSpan.FromSeconds(20));
		}

		[Test]
		[ExpectedException(typeof(exception.MethodParameterIsOutOfBoundsException))]
		public void setClipBegin_OutOfBounds_Negative()
		{
			mExternalVideoMedia1.ClipBegin = new Time(TimeSpan.FromSeconds(-10));
		}

		[Test]
		public void clipEnd_Basics()
		{
			Assert.IsTrue(
				Time.MaxValue.isEqualTo(mExternalVideoMedia1.ClipEnd),
				"The default value of clipBegin is {0} and not {1} as expected",
				mExternalVideoMedia1.ClipEnd.ToString(), Time.MaxValue.ToString());
			Time val = new Time(TimeSpan.FromSeconds(10));
			mExternalVideoMedia1.ClipEnd = val;
			Assert.IsTrue(val.isEqualTo(mExternalVideoMedia1.ClipEnd), "Unexpected clipBegin return value");
			Assert.IsFalse(Type.ReferenceEquals(val, mExternalVideoMedia1.ClipBegin), "ClipBegin was must not be set by reference");
			val = new Time(10);
			mExternalVideoMedia1.ClipEnd = val;
			Assert.IsTrue(val.isEqualTo(mExternalVideoMedia1.ClipEnd), "Unexpected clipBegin return value");
			val = mExternalVideoMedia1.ClipBegin;
			mExternalVideoMedia1.ClipEnd = val;
			Assert.IsTrue(val.isEqualTo(mExternalVideoMedia1.ClipEnd), "Unexpected clipBegin return value");
		}

		[Test]
		[ExpectedException(typeof(exception.MethodParameterIsNullException))]
		public void clipEnd_NullValue()
		{
			mExternalVideoMedia1.ClipEnd = null;
		}

		[Test]
		[ExpectedException(typeof(exception.MethodParameterIsOutOfBoundsException))]
		public void clipEnd_OutOfBoundsValue_LtClipBegin()
		{
			mExternalVideoMedia1.ClipBegin = new Time(TimeSpan.FromSeconds(10));
			mExternalVideoMedia1.ClipEnd = new Time(TimeSpan.FromSeconds(5));
		}

		#endregion
		
		#region ISized tests
		[Test]
		public void height_Basics()
		{
			Assert.AreEqual(0, mExternalVideoMedia1.Height, "Default image height must be 0");
			mExternalVideoMedia1.Height = 10;
			Assert.AreEqual(10, mExternalVideoMedia1.Height, "Unexpected getHeight return value");
			mExternalVideoMedia1.Height = 800;
			Assert.AreEqual(800, mExternalVideoMedia1.Height, "Unexpected getHeight return value");
			mExternalVideoMedia1.Height = 0;
			Assert.AreEqual(0, mExternalVideoMedia1.Height, "Unexpected getHeight return value");
		}
		[Test]
		[ExpectedException(typeof(exception.MethodParameterIsOutOfBoundsException))]
		public void setHeight_NegativeValue()
		{
			mExternalVideoMedia1.Height = -10;
		}
		[Test]
		public void width_Basics()
		{
			Assert.AreEqual(0, mExternalVideoMedia1.Width, "Default image height must be 0");
			mExternalVideoMedia1.Width = 10;
			Assert.AreEqual(10, mExternalVideoMedia1.Width, "Unexpected getWidth return value");
			mExternalVideoMedia1.Width = 800;
			Assert.AreEqual(800, mExternalVideoMedia1.Width, "Unexpected getWidth return value");
			mExternalVideoMedia1.Width = 0;
			Assert.AreEqual(0, mExternalVideoMedia1.Width, "Unexpected getWidth return value");
		}
		[Test]
		[ExpectedException(typeof(exception.MethodParameterIsOutOfBoundsException))]
		public void setWidth_NegativeValue()
		{
			mExternalVideoMedia1.Width = -10;
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
			mExternalVideoMedia1.ClipBegin = Time.Zero;
			mExternalVideoMedia2.ClipBegin = new Time(TimeSpan.FromSeconds(10));
			Assert.IsFalse(mExternalVideoMedia1.ValueEquals(mExternalVideoMedia2), "ExternalVideoMedia with different clipBegin can not be value equal");
			mExternalVideoMedia2.ClipBegin = mExternalVideoMedia1.ClipBegin;
			Assert.IsTrue(mExternalVideoMedia1.ValueEquals(mExternalVideoMedia2), "Expected ExternalVideoMedia to be value equal");
		}

		[Test]
		public void valueEquals_ClipEnd()
		{
			mExternalVideoMedia1.ClipEnd = Time.Zero;
			mExternalVideoMedia2.ClipEnd = new Time(TimeSpan.FromSeconds(10));
			Assert.IsFalse(mExternalVideoMedia1.ValueEquals(mExternalVideoMedia2), "ExternalVideoMedia with different clipEnd can not be value equal");
			mExternalVideoMedia2.ClipEnd = mExternalVideoMedia1.ClipEnd;
			Assert.IsTrue(mExternalVideoMedia1.ValueEquals(mExternalVideoMedia2), "Expected ExternalVideoMedia to be value equal");
		}

		[Test]
		public void valueEquals_Height()
		{
			mExternalVideoMedia1.Height = 0;
			mExternalVideoMedia2.Height = 40;
			Assert.IsFalse(
				mExternalVideoMedia1.ValueEquals(mExternalVideoMedia2),
				"ExternalImageMedia with different hight values can not be value equal");
			mExternalVideoMedia1.Height = mExternalVideoMedia2.Height;
			Assert.IsTrue(
				mExternalVideoMedia1.ValueEquals(mExternalVideoMedia2),
				"Expected ExternalImageMedia to be equal");
		}

		[Test]
		public void valueEquals_Width()
		{
			mExternalVideoMedia1.Width = 0;
			mExternalVideoMedia2.Width = 40;
			Assert.IsFalse(
				mExternalVideoMedia1.ValueEquals(mExternalVideoMedia2),
				"ExternalImageMedia with different hight values can not be value equal");
			mExternalVideoMedia1.Width = mExternalVideoMedia2.Width;
			Assert.IsTrue(
				mExternalVideoMedia1.ValueEquals(mExternalVideoMedia2),
				"Expected ExternalImageMedia to be equal");
		}

		#endregion

		#region IXukAble tests
		[Test]
		public override void Xuk_RoundTrip()
		{
			mExternalVideoMedia1.ClipBegin = new Time(10000);
			mExternalVideoMedia1.ClipBegin = new Time(15000);
			mExternalVideoMedia1.Height = 600;
			mExternalVideoMedia1.Width = 800;
			base.Xuk_RoundTrip();
		}
		#endregion

		#region IMedia tests

		[Test]
		public override void copy_valueEqualsAndReferenceDiffers()
		{
			mExternalVideoMedia1.ClipBegin = new Time(10000);
			mExternalVideoMedia1.ClipBegin = new Time(15000);
			mExternalVideoMedia1.Height = 600;
			mExternalVideoMedia1.Width = 800;
			base.copy_valueEqualsAndReferenceDiffers();
		}

		[Test]
		public override void export_valueEqualsPresentationsOk()
		{
			mExternalVideoMedia1.ClipBegin = new Time(10000);
			mExternalVideoMedia1.ClipBegin = new Time(15000);
			mExternalVideoMedia1.Height = 600;
			mExternalVideoMedia1.Width = 800;
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
