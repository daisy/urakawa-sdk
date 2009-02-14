using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using urakawa.media.timing;
using urakawa.xuk;

namespace urakawa.media
{
    [TestFixture]
    public class ExternalAudioMediaTests : ExternalMediaTests
    {
        public ExternalAudioMediaTests()
            : base(typeof (ExternalAudioMedia))
        {
        }

        protected ExternalAudioMedia mExternalAudioMedia1
        {
            get { return mLocatedMedia1 as ExternalAudioMedia; }
        }

        protected ExternalAudioMedia mExternalAudioMedia2
        {
            get { return mLocatedMedia2 as ExternalAudioMedia; }
        }

        protected ExternalAudioMedia mExternalAudioMedia3
        {
            get { return mLocatedMedia3 as ExternalAudioMedia; }
        }


        [Test]
        public void IsContinuous()
        {
            Assert.IsTrue(mExternalAudioMedia1.IsContinuous, "IsContinuous() inexpectedly returned false");
        }

        [Test]
        public void IsDiscrete()
        {
            Assert.IsFalse(mExternalAudioMedia1.IsDiscrete, "IsDiscrete() inexpectedly returned true");
        }

        [Test]
        public void IsSequence()
        {
            Assert.IsFalse(mExternalAudioMedia1.IsSequence, "IsSequence() inexpectedly returned true");
        }

        #region IContinuous tests

        [Test]
        public void Duration_Basics()
        {
            Assert.IsTrue(
                mExternalAudioMedia1.ClipEnd.SubtractTime(mExternalAudioMedia1.ClipBegin).SubtractTimeDelta(
                    mExternalAudioMedia1.Duration).IsEqualTo(Time.Zero),
                "Unexpected GetDuration return value");
            mExternalAudioMedia1.ClipEnd = new Time(TimeSpan.FromSeconds(7.5));
            Assert.IsTrue(
                mExternalAudioMedia1.ClipEnd.SubtractTime(mExternalAudioMedia1.ClipBegin).SubtractTimeDelta(
                    mExternalAudioMedia1.Duration).IsEqualTo(Time.Zero),
                "Unexpected GetDuration return value");
            mExternalAudioMedia1.ClipBegin = mExternalAudioMedia1.ClipEnd;
            Assert.IsTrue(
                mExternalAudioMedia1.Duration.IsEqualTo(new TimeDelta(0.0)),
                "Unexpected GetDuration return value");
        }

        [Test]
        public void Split_Basics()
        {
            TestSplit(Time.Zero, new Time(TimeSpan.FromSeconds(10)), new Time(TimeSpan.FromSeconds(5)));
            TestSplit(Time.Zero, new Time(TimeSpan.FromSeconds(10)), Time.Zero);
            TestSplit(Time.Zero, new Time(TimeSpan.FromSeconds(10)), new Time(TimeSpan.FromSeconds(10)));
            TestSplit(Time.Zero, Time.Zero, Time.Zero);
            TestSplit(new Time(TimeSpan.FromSeconds(10)), new Time(TimeSpan.FromSeconds(10)),
                      new Time(TimeSpan.FromSeconds(10)));
        }

        [Test]
        [ExpectedException(typeof (exception.MethodParameterIsNullException))]
        public void Split_NullSplitPoint()
        {
            TestSplit(Time.Zero, new Time(TimeSpan.FromSeconds(10)), null);
        }

        [Test]
        [ExpectedException(typeof (exception.MethodParameterIsOutOfBoundsException))]
        public void Split_SplitPointAfterClip()
        {
            TestSplit(Time.Zero, new Time(TimeSpan.FromSeconds(10)), new Time(TimeSpan.FromSeconds(20)));
        }

        [Test]
        [ExpectedException(typeof (exception.MethodParameterIsOutOfBoundsException))]
        public void Split_SplitPointBeforeClip()
        {
            TestSplit(new Time(TimeSpan.FromSeconds(10)), new Time(TimeSpan.FromSeconds(20)), Time.Zero);
        }


        private void TestSplit(Time begin, Time end, Time split)
        {
            mExternalAudioMedia1.ClipEnd = Time.MaxValue;
            mExternalAudioMedia1.ClipBegin = Time.Zero;
            mExternalAudioMedia1.ClipBegin = begin;
            mExternalAudioMedia1.ClipEnd = end;
            ExternalAudioMedia secondPartAudio = mExternalAudioMedia1.Split(split);
            Assert.IsTrue(
                begin.IsEqualTo(mExternalAudioMedia1.ClipBegin),
                "Unexpected clip begin, was '{0}' expected '{1}'",
                mExternalAudioMedia1.ClipBegin, begin.ToString());
            Assert.IsTrue(
                split.IsEqualTo(mExternalAudioMedia1.ClipEnd),
                "Unexpected clip end, was '{0}' expected '{1}'",
                mExternalAudioMedia1.ClipBegin, begin.ToString());
            Assert.IsTrue(
                split.IsEqualTo(secondPartAudio.ClipBegin),
                "Unexpected clip begin, was '{0}' expected '{1}'",
                secondPartAudio.ClipBegin, split.ToString());
            Assert.IsTrue(
                end.IsEqualTo(secondPartAudio.ClipEnd),
                "Unexpected clip end, was '{0}' expected '{1}'",
                secondPartAudio.ClipEnd, end.ToString());
        }

        #endregion

        #region IClipped tests

        [Test]
        public void ClipBegin_Basics()
        {
            Assert.IsTrue(
                Time.Zero.IsEqualTo(mExternalAudioMedia1.ClipBegin),
                "The default value of clipBegin is {0} and not {1} as expected",
                mExternalAudioMedia1.ClipBegin.ToString(), Time.Zero.ToString());
            Time val = new Time(TimeSpan.FromSeconds(10));
            mExternalAudioMedia1.ClipBegin = val;
            Assert.IsTrue(val.IsEqualTo(mExternalAudioMedia1.ClipBegin), "Unexpected clipBegin return value");
            Assert.IsFalse(Type.ReferenceEquals(val, mExternalAudioMedia1.ClipBegin),
                           "ClipBegin was must not be set by reference");
            val = new Time(10);
            mExternalAudioMedia1.ClipBegin = val;
            Assert.IsTrue(val.IsEqualTo(mExternalAudioMedia1.ClipBegin), "Unexpected clipBegin return value");
            val = mExternalAudioMedia1.ClipEnd;
            mExternalAudioMedia1.ClipBegin = val;
            Assert.IsTrue(val.IsEqualTo(mExternalAudioMedia1.ClipBegin), "Unexpected clipBegin return value");
        }

        [Test]
        [ExpectedException(typeof (exception.MethodParameterIsNullException))]
        public void ClipBegin_NullValue()
        {
            mExternalAudioMedia1.ClipBegin = null;
        }

        [Test]
        [ExpectedException(typeof (exception.MethodParameterIsOutOfBoundsException))]
        public void ClipBegin_OutOfBounds_GtClipEnd()
        {
            mExternalAudioMedia1.ClipEnd = new Time(TimeSpan.FromSeconds(10));
            mExternalAudioMedia1.ClipBegin = new Time(TimeSpan.FromSeconds(20));
        }

        [Test]
        [ExpectedException(typeof (exception.MethodParameterIsOutOfBoundsException))]
        public void ClipBegin_OutOfBounds_Negative()
        {
            mExternalAudioMedia1.ClipBegin = new Time(TimeSpan.FromSeconds(-10));
        }

        [Test]
        public void ClipEnd_Basics()
        {
            Assert.IsTrue(
                Time.MaxValue.IsEqualTo(mExternalAudioMedia1.ClipEnd),
                "The default value of clipBegin is {0} and not {1} as expected",
                mExternalAudioMedia1.ClipEnd.ToString(), Time.MaxValue.ToString());
            Time val = new Time(TimeSpan.FromSeconds(10));
            mExternalAudioMedia1.ClipEnd = val;
            Assert.IsTrue(val.IsEqualTo(mExternalAudioMedia1.ClipEnd), "Unexpected clipBegin return value");
            Assert.IsFalse(Type.ReferenceEquals(val, mExternalAudioMedia1.ClipBegin),
                           "ClipBegin was must not be set by reference");
            val = new Time(10);
            mExternalAudioMedia1.ClipEnd = val;
            Assert.IsTrue(val.IsEqualTo(mExternalAudioMedia1.ClipEnd), "Unexpected clipBegin return value");
            val = mExternalAudioMedia1.ClipBegin;
            mExternalAudioMedia1.ClipEnd = val;
            Assert.IsTrue(val.IsEqualTo(mExternalAudioMedia1.ClipEnd), "Unexpected clipBegin return value");
        }

        [Test]
        [ExpectedException(typeof (exception.MethodParameterIsNullException))]
        public void ClipEnd_NullValue()
        {
            mExternalAudioMedia1.ClipEnd = null;
        }

        [Test]
        [ExpectedException(typeof (exception.MethodParameterIsOutOfBoundsException))]
        public void ClipEnd_OutOfBoundsValue_LtClipBegin()
        {
            mExternalAudioMedia1.ClipBegin = new Time(TimeSpan.FromSeconds(10));
            mExternalAudioMedia1.ClipEnd = new Time(TimeSpan.FromSeconds(5));
        }

        #endregion

        #region IValueEquatable tests

        [Test]
        public override void ValueEquals_Basics()
        {
            base.ValueEquals_Basics();
        }

        [Test]
        public override void ValueEquals_NewCreatedEquals()
        {
            base.ValueEquals_NewCreatedEquals();
        }

        [Test]
        public override void ValueEquals_Language()
        {
            base.ValueEquals_Language();
        }

        [Test]
        public override void ValueEquals_Src()
        {
            base.ValueEquals_Src();
        }

        [Test]
        public void ValueEquals_ClipBegin()
        {
            mExternalAudioMedia1.ClipBegin = Time.Zero;
            mExternalAudioMedia2.ClipBegin = new Time(TimeSpan.FromSeconds(10));
            Assert.IsFalse(mExternalAudioMedia1.ValueEquals(mExternalAudioMedia2),
                           "ExternalAudioMedia with different clipBegin can not be value equal");
            mExternalAudioMedia2.ClipBegin = mExternalAudioMedia1.ClipBegin;
            Assert.IsTrue(mExternalAudioMedia1.ValueEquals(mExternalAudioMedia2),
                          "Expected ExternalAudioMedia to be value equal");
        }

        [Test]
        public void ValueEquals_ClipEnd()
        {
            mExternalAudioMedia1.ClipEnd = Time.Zero;
            mExternalAudioMedia2.ClipEnd = new Time(TimeSpan.FromSeconds(10));
            Assert.IsFalse(mExternalAudioMedia1.ValueEquals(mExternalAudioMedia2),
                           "ExternalAudioMedia with different clipEnd can not be value equal");
            mExternalAudioMedia2.ClipEnd = mExternalAudioMedia1.ClipEnd;
            Assert.IsTrue(mExternalAudioMedia1.ValueEquals(mExternalAudioMedia2),
                          "Expected ExternalAudioMedia to be value equal");
        }

        #endregion

        #region IXukAble tests

        [Test]
        public override void Xuk_RoundTrip()
        {
            base.Xuk_RoundTrip();
        }

        #endregion

        #region Media tests

        [Test]
        public override void Copy_ValueEqualsAndReferenceDiffers()
        {
            base.Copy_ValueEqualsAndReferenceDiffers();
        }

        [Test]
        public override void Export_ValueEqualsPresentationsOk()
        {
            base.Export_ValueEqualsPresentationsOk();
        }

        [Test]
        public override void Language_Basics()
        {
            base.Language_Basics();
        }

        [Test]
        [ExpectedException(typeof (exception.MethodParameterIsEmptyStringException))]
        public override void Language_EmptyString()
        {
            base.Language_EmptyString();
        }

        #endregion

        #region ExternalMedia tests

        [Test]
        public override void Src_Basics()
        {
            base.Src_Basics();
        }

        [Test]
        [ExpectedException(typeof (exception.MethodParameterIsNullException))]
        public override void Src_NullValue()
        {
            base.Src_NullValue();
        }

        [Test]
        [ExpectedException(typeof (exception.MethodParameterIsEmptyStringException))]
        public override void Src_EmptyStringValue()
        {
            base.Src_EmptyStringValue();
        }

        [Test]
        public override void Uri_Basics()
        {
            base.Uri_Basics();
        }

        [Test]
        [ExpectedException(typeof (exception.InvalidUriException))]
        public override void Uri_SrcMalformedUri()
        {
            base.Uri_SrcMalformedUri();
        }

        #endregion
    }
}