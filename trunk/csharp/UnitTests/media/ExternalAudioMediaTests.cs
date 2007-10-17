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
			Assert.IsTrue(mExternalAudioMedia1.isContinuous(), "isContinuous() inexpectedly returned false");
		}

		[Test]
		public void isDiscrete()
		{
			Assert.IsFalse(mExternalAudioMedia1.isDiscrete(), "isDiscrete() inexpectedly returned true");
		}

		[Test]
		public void isSequence()
		{
			Assert.IsFalse(mExternalAudioMedia1.isSequence(), "isSequence() inexpectedly returned true");
		}

		#region IContinuous tests

		[Test]
		public void getDuration_Basics()
		{
			Assert.IsTrue(
				mExternalAudioMedia1.getClipEnd().subtractTime(mExternalAudioMedia1.getClipBegin()).subtractTimeDelta(
					mExternalAudioMedia1.getDuration()).isEqualTo(Time.Zero),
				"Unexpected getDuration return value");
			mExternalAudioMedia1.setClipEnd(new Time(TimeSpan.FromSeconds(7.5)));
			Assert.IsTrue(
				mExternalAudioMedia1.getClipEnd().subtractTime(mExternalAudioMedia1.getClipBegin()).subtractTimeDelta(
					mExternalAudioMedia1.getDuration()).isEqualTo(Time.Zero),
				"Unexpected getDuration return value");
			mExternalAudioMedia1.setClipBegin(mExternalAudioMedia1.getClipEnd());
			Assert.IsTrue(
				mExternalAudioMedia1.getDuration().isEqualTo(new TimeDelta(0.0)),
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
			mExternalAudioMedia1.setClipEnd(Time.MaxValue);
			mExternalAudioMedia1.setClipBegin(Time.Zero);
			mExternalAudioMedia1.setClipBegin(begin);
			mExternalAudioMedia1.setClipEnd(end);
			ExternalAudioMedia secondPartAudio = mExternalAudioMedia1.split(split);
			Assert.IsTrue(
				begin.isEqualTo(mExternalAudioMedia1.getClipBegin()),
				"Unexpected clip begin, was '{0}' expected '{1}'",
				mExternalAudioMedia1.getClipBegin(), begin.ToString());
			Assert.IsTrue(
				split.isEqualTo(mExternalAudioMedia1.getClipEnd()),
				"Unexpected clip end, was '{0}' expected '{1}'",
				mExternalAudioMedia1.getClipBegin(), begin.ToString());
			Assert.IsTrue(
				split.isEqualTo(secondPartAudio.getClipBegin()),
				"Unexpected clip begin, was '{0}' expected '{1}'",
				secondPartAudio.getClipBegin(), split.ToString());
			Assert.IsTrue(
				end.isEqualTo(secondPartAudio.getClipEnd()),
				"Unexpected clip end, was '{0}' expected '{1}'",
				secondPartAudio.getClipEnd(), end.ToString());
		}

		#endregion

		#region IClipped tests

		[Test]
		public void clipBegin_Basics()
		{
			Assert.IsTrue(
				Time.Zero.isEqualTo(mExternalAudioMedia1.getClipBegin()), 
				"The default value of clipBegin is {0} and not {1} as expected",
				mExternalAudioMedia1.getClipBegin().ToString(), Time.Zero.ToString());
			Time val = new Time(TimeSpan.FromSeconds(10));
			mExternalAudioMedia1.setClipBegin(val);
			Assert.IsTrue(val.isEqualTo(mExternalAudioMedia1.getClipBegin()), "Unexpected clipBegin return value");
			Assert.IsFalse(Type.ReferenceEquals(val, mExternalAudioMedia1.getClipBegin()), "ClipBegin was must not be set by reference");
			val = new Time(10);
			mExternalAudioMedia1.setClipBegin(val);
			Assert.IsTrue(val.isEqualTo(mExternalAudioMedia1.getClipBegin()), "Unexpected clipBegin return value");
			val = mExternalAudioMedia1.getClipEnd();
			mExternalAudioMedia1.setClipBegin(val);
			Assert.IsTrue(val.isEqualTo(mExternalAudioMedia1.getClipBegin()), "Unexpected clipBegin return value");
		}

		[Test]
		[ExpectedException(typeof(exception.MethodParameterIsNullException))]
		public void setClipBegin_NullValue()
		{
			mExternalAudioMedia1.setClipBegin(null);
		}

		[Test]
		[ExpectedException(typeof(exception.MethodParameterIsOutOfBoundsException))]
		public void setClipBegin_OutOfBounds_GtClipEnd()
		{
			mExternalAudioMedia1.setClipEnd(new Time(TimeSpan.FromSeconds(10)));
			mExternalAudioMedia1.setClipBegin(new Time(TimeSpan.FromSeconds(20)));
		}

		[Test]
		[ExpectedException(typeof(exception.MethodParameterIsOutOfBoundsException))]
		public void setClipBegin_OutOfBounds_Negative()
		{
			mExternalAudioMedia1.setClipBegin(new Time(TimeSpan.FromSeconds(-10)));
		}

		[Test]
		public void clipEnd_Basics()
		{
			Assert.IsTrue(
				Time.MaxValue.isEqualTo(mExternalAudioMedia1.getClipEnd()),
				"The default value of clipBegin is {0} and not {1} as expected",
				mExternalAudioMedia1.getClipEnd().ToString(), Time.MaxValue.ToString());
			Time val = new Time(TimeSpan.FromSeconds(10));
			mExternalAudioMedia1.setClipEnd(val);
			Assert.IsTrue(val.isEqualTo(mExternalAudioMedia1.getClipEnd()), "Unexpected clipBegin return value");
			Assert.IsFalse(Type.ReferenceEquals(val, mExternalAudioMedia1.getClipBegin()), "ClipBegin was must not be set by reference");
			val = new Time(10);
			mExternalAudioMedia1.setClipEnd(val);
			Assert.IsTrue(val.isEqualTo(mExternalAudioMedia1.getClipEnd()), "Unexpected clipBegin return value");
			val = mExternalAudioMedia1.getClipBegin();
			mExternalAudioMedia1.setClipEnd(val);
			Assert.IsTrue(val.isEqualTo(mExternalAudioMedia1.getClipEnd()), "Unexpected clipBegin return value");
		}

		[Test]
		[ExpectedException(typeof(exception.MethodParameterIsNullException))]
		public void clipEnd_NullValue()
		{
			mExternalAudioMedia1.setClipEnd(null);
		}

		[Test]
		[ExpectedException(typeof(exception.MethodParameterIsOutOfBoundsException))]
		public void clipEnd_OutOfBoundsValue_LtClipBegin()
		{
			mExternalAudioMedia1.setClipBegin(new Time(TimeSpan.FromSeconds(10)));
			mExternalAudioMedia1.setClipEnd(new Time(TimeSpan.FromSeconds(5)));
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
			mExternalAudioMedia1.setClipBegin(Time.Zero);
			mExternalAudioMedia2.setClipBegin(new Time(TimeSpan.FromSeconds(10)));
			Assert.IsFalse(mExternalAudioMedia1.valueEquals(mExternalAudioMedia2), "ExternalAudioMedia with different clipBegin can not be value equal");
			mExternalAudioMedia2.setClipBegin(mExternalAudioMedia1.getClipBegin());
			Assert.IsTrue(mExternalAudioMedia1.valueEquals(mExternalAudioMedia2), "Expected ExternalAudioMedia to be value equal");
		}

		[Test]
		public void valueEquals_ClipEnd()
		{
			mExternalAudioMedia1.setClipEnd(Time.Zero);
			mExternalAudioMedia2.setClipEnd(new Time(TimeSpan.FromSeconds(10)));
			Assert.IsFalse(mExternalAudioMedia1.valueEquals(mExternalAudioMedia2), "ExternalAudioMedia with different clipEnd can not be value equal");
			mExternalAudioMedia2.setClipEnd(mExternalAudioMedia1.getClipEnd());
			Assert.IsTrue(mExternalAudioMedia1.valueEquals(mExternalAudioMedia2), "Expected ExternalAudioMedia to be value equal");
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
