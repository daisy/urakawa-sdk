using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using urakawa.media.timing;

namespace urakawa.media
{
	[TestFixture, Description("Tests for ExternalAudioMedia functionality")]
	public class ExternalAudioMediaTests : ExternalMediaTests
	{
		public ExternalAudioMediaTests()
			: base(typeof(ExternalAudioMedia).Name, ToolkitSettings.XUK_NS)
		{
		}

		protected ExternalAudioMedia mExternalAudioMedia1 { get { return mExternalMedia1 as ExternalAudioMedia; } }
		protected ExternalAudioMedia mExternalAudioMedia2 { get { return mExternalMedia2 as ExternalAudioMedia; } }
		protected ExternalAudioMedia mExternalAudioMedia3 { get { return mExternalMedia3 as ExternalAudioMedia; } }


		[Test]
		public void isContinuous()
		{
			Assert.IsTrue(mExternalAudioMedia1.IsContinuous, "isContinuous() inexpectedly returned false");
		}

		[Test]
		public void isDiscrete()
		{
			Assert.IsFalse(mExternalAudioMedia1.IsDiscrete, "isDiscrete() inexpectedly returned true");
		}

		[Test]
		public void isSequence()
		{
			Assert.IsFalse(mExternalAudioMedia1.IsSequence, "isSequence() inexpectedly returned true");
		}

		#region IContinuous tests

		[Test]
		public void getDuration_Basics()
		{
			Assert.IsTrue(
				mExternalAudioMedia1.ClipEnd.subtractTime(mExternalAudioMedia1.ClipBegin).subtractTimeDelta(
					mExternalAudioMedia1.Duration).isEqualTo(Time.Zero),
				"Unexpected GetDuration return value");
			mExternalAudioMedia1.ClipEnd = new Time(TimeSpan.FromSeconds(7.5));
			Assert.IsTrue(
				mExternalAudioMedia1.ClipEnd.subtractTime(mExternalAudioMedia1.ClipBegin).subtractTimeDelta(
					mExternalAudioMedia1.Duration).isEqualTo(Time.Zero),
				"Unexpected GetDuration return value");
			mExternalAudioMedia1.ClipBegin = mExternalAudioMedia1.ClipEnd;
			Assert.IsTrue(
				mExternalAudioMedia1.Duration.isEqualTo(new TimeDelta(0.0)),
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
			mExternalAudioMedia1.ClipEnd = Time.MaxValue;
			mExternalAudioMedia1.ClipBegin = Time.Zero;
			mExternalAudioMedia1.ClipBegin = begin;
			mExternalAudioMedia1.ClipEnd = end;
			ExternalAudioMedia secondPartAudio = mExternalAudioMedia1.Split(split);
			Assert.IsTrue(
				begin.isEqualTo(mExternalAudioMedia1.ClipBegin),
				"Unexpected clip begin, was '{0}' expected '{1}'",
				mExternalAudioMedia1.ClipBegin, begin.ToString());
			Assert.IsTrue(
				split.isEqualTo(mExternalAudioMedia1.ClipEnd),
				"Unexpected clip end, was '{0}' expected '{1}'",
				mExternalAudioMedia1.ClipBegin, begin.ToString());
			Assert.IsTrue(
				split.isEqualTo(secondPartAudio.ClipBegin),
				"Unexpected clip begin, was '{0}' expected '{1}'",
				secondPartAudio.ClipBegin, split.ToString());
			Assert.IsTrue(
				end.isEqualTo(secondPartAudio.ClipEnd),
				"Unexpected clip end, was '{0}' expected '{1}'",
				secondPartAudio.ClipEnd, end.ToString());
		}

		#endregion

		#region IClipped tests

		[Test]
		public void clipBegin_Basics()
		{
			Assert.IsTrue(
				Time.Zero.isEqualTo(mExternalAudioMedia1.ClipBegin), 
				"The default value of clipBegin is {0} and not {1} as expected",
				mExternalAudioMedia1.ClipBegin.ToString(), Time.Zero.ToString());
			Time val = new Time(TimeSpan.FromSeconds(10));
			mExternalAudioMedia1.ClipBegin = val;
			Assert.IsTrue(val.isEqualTo(mExternalAudioMedia1.ClipBegin), "Unexpected clipBegin return value");
			Assert.IsFalse(Type.ReferenceEquals(val, mExternalAudioMedia1.ClipBegin), "ClipBegin was must not be set by reference");
			val = new Time(10);
			mExternalAudioMedia1.ClipBegin = val;
			Assert.IsTrue(val.isEqualTo(mExternalAudioMedia1.ClipBegin), "Unexpected clipBegin return value");
			val = mExternalAudioMedia1.ClipEnd;
			mExternalAudioMedia1.ClipBegin = val;
			Assert.IsTrue(val.isEqualTo(mExternalAudioMedia1.ClipBegin), "Unexpected clipBegin return value");
		}

		[Test]
		[ExpectedException(typeof(exception.MethodParameterIsNullException))]
		public void setClipBegin_NullValue()
		{
			mExternalAudioMedia1.ClipBegin = null;
		}

		[Test]
		[ExpectedException(typeof(exception.MethodParameterIsOutOfBoundsException))]
		public void setClipBegin_OutOfBounds_GtClipEnd()
		{
			mExternalAudioMedia1.ClipEnd = new Time(TimeSpan.FromSeconds(10));
			mExternalAudioMedia1.ClipBegin = new Time(TimeSpan.FromSeconds(20));
		}

		[Test]
		[ExpectedException(typeof(exception.MethodParameterIsOutOfBoundsException))]
		public void setClipBegin_OutOfBounds_Negative()
		{
			mExternalAudioMedia1.ClipBegin = new Time(TimeSpan.FromSeconds(-10));
		}

		[Test]
		public void clipEnd_Basics()
		{
			Assert.IsTrue(
				Time.MaxValue.isEqualTo(mExternalAudioMedia1.ClipEnd),
				"The default value of clipBegin is {0} and not {1} as expected",
				mExternalAudioMedia1.ClipEnd.ToString(), Time.MaxValue.ToString());
			Time val = new Time(TimeSpan.FromSeconds(10));
			mExternalAudioMedia1.ClipEnd = val;
			Assert.IsTrue(val.isEqualTo(mExternalAudioMedia1.ClipEnd), "Unexpected clipBegin return value");
			Assert.IsFalse(Type.ReferenceEquals(val, mExternalAudioMedia1.ClipBegin), "ClipBegin was must not be set by reference");
			val = new Time(10);
			mExternalAudioMedia1.ClipEnd = val;
			Assert.IsTrue(val.isEqualTo(mExternalAudioMedia1.ClipEnd), "Unexpected clipBegin return value");
			val = mExternalAudioMedia1.ClipBegin;
			mExternalAudioMedia1.ClipEnd = val;
			Assert.IsTrue(val.isEqualTo(mExternalAudioMedia1.ClipEnd), "Unexpected clipBegin return value");
		}

		[Test]
		[ExpectedException(typeof(exception.MethodParameterIsNullException))]
		public void clipEnd_NullValue()
		{
			mExternalAudioMedia1.ClipEnd = null;
		}

		[Test]
		[ExpectedException(typeof(exception.MethodParameterIsOutOfBoundsException))]
		public void clipEnd_OutOfBoundsValue_LtClipBegin()
		{
			mExternalAudioMedia1.ClipBegin = new Time(TimeSpan.FromSeconds(10));
			mExternalAudioMedia1.ClipEnd = new Time(TimeSpan.FromSeconds(5));
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
		public void valueEquals_ClipBegin()
		{
			mExternalAudioMedia1.ClipBegin = Time.Zero;
			mExternalAudioMedia2.ClipBegin = new Time(TimeSpan.FromSeconds(10));
			Assert.IsFalse(mExternalAudioMedia1.ValueEquals(mExternalAudioMedia2), "ExternalAudioMedia with different clipBegin can not be value equal");
			mExternalAudioMedia2.ClipBegin = mExternalAudioMedia1.ClipBegin;
			Assert.IsTrue(mExternalAudioMedia1.ValueEquals(mExternalAudioMedia2), "Expected ExternalAudioMedia to be value equal");
		}

		[Test]
		public void valueEquals_ClipEnd()
		{
			mExternalAudioMedia1.ClipEnd = Time.Zero;
			mExternalAudioMedia2.ClipEnd = new Time(TimeSpan.FromSeconds(10));
			Assert.IsFalse(mExternalAudioMedia1.ValueEquals(mExternalAudioMedia2), "ExternalAudioMedia with different clipEnd can not be value equal");
			mExternalAudioMedia2.ClipEnd = mExternalAudioMedia1.ClipEnd;
			Assert.IsTrue(mExternalAudioMedia1.ValueEquals(mExternalAudioMedia2), "Expected ExternalAudioMedia to be value equal");
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
		public override void copy_valueEqualsAndReferenceDiffers()
		{
			base.copy_valueEqualsAndReferenceDiffers();
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
