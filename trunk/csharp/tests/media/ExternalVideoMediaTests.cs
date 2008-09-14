using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using urakawa.media.timing;
using urakawa.xuk;

namespace urakawa.media
{
    [TestFixture, Description("Tests for ExternalVideoMedia functionality")]
    public class ExternalVideoMediaTests : ExternalMediaTests
    {
        public ExternalVideoMediaTests()
            : base(typeof (ExternalVideoMedia))
        {
        }

        protected ExternalVideoMedia mExternalVideoMedia1
        {
            get { return mLocatedMedia1 as ExternalVideoMedia; }
        }

        protected ExternalVideoMedia mExternalVideoMedia2
        {
            get { return mLocatedMedia2 as ExternalVideoMedia; }
        }

        protected ExternalVideoMedia mExternalVideoMedia3
        {
            get { return mLocatedMedia3 as ExternalVideoMedia; }
        }

        #region IContinuous tests

        [Test]
        public void Duration_Basics()
        {
            Assert.IsTrue(
                mExternalVideoMedia1.ClipEnd.SubtractTime(mExternalVideoMedia1.ClipBegin).SubtractTimeDelta(
                    mExternalVideoMedia1.Duration).IsEqualTo(Time.Zero),
                "Unexpected GetDuration return value");
            mExternalVideoMedia1.ClipEnd = new Time(TimeSpan.FromSeconds(7.5));
            Assert.IsTrue(
                mExternalVideoMedia1.ClipEnd.SubtractTime(mExternalVideoMedia1.ClipBegin).SubtractTimeDelta(
                    mExternalVideoMedia1.Duration).IsEqualTo(Time.Zero),
                "Unexpected GetDuration return value");
            mExternalVideoMedia1.ClipBegin = mExternalVideoMedia1.ClipEnd;
            Assert.IsTrue(
                mExternalVideoMedia1.Duration.IsEqualTo(new TimeDelta(0.0)),
                "Unexpected GetDuration return value");
        }

        [Test]
        public void Split_Basics()
        {
            testSplit(Time.Zero, new Time(TimeSpan.FromSeconds(10)), new Time(TimeSpan.FromSeconds(5)));
            testSplit(Time.Zero, new Time(TimeSpan.FromSeconds(10)), Time.Zero);
            testSplit(Time.Zero, new Time(TimeSpan.FromSeconds(10)), new Time(TimeSpan.FromSeconds(10)));
            testSplit(Time.Zero, Time.Zero, Time.Zero);
            testSplit(new Time(TimeSpan.FromSeconds(10)), new Time(TimeSpan.FromSeconds(10)),
                      new Time(TimeSpan.FromSeconds(10)));
        }

        [Test]
        [ExpectedException(typeof (exception.MethodParameterIsNullException))]
        public void Split_NullSplitPoint()
        {
            testSplit(Time.Zero, new Time(TimeSpan.FromSeconds(10)), null);
        }

        [Test]
        [ExpectedException(typeof (exception.MethodParameterIsOutOfBoundsException))]
        public void Split_SplitPointAfterClip()
        {
            testSplit(Time.Zero, new Time(TimeSpan.FromSeconds(10)), new Time(TimeSpan.FromSeconds(20)));
        }

        [Test]
        [ExpectedException(typeof (exception.MethodParameterIsOutOfBoundsException))]
        public void Split_SplitPointBeforeClip()
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
                begin.IsEqualTo(mExternalVideoMedia1.ClipBegin),
                "Unexpected clip begin, was '{0}' expected '{1}'",
                mExternalVideoMedia1.ClipBegin, begin.ToString());
            Assert.IsTrue(
                split.IsEqualTo(mExternalVideoMedia1.ClipEnd),
                "Unexpected clip end, was '{0}' expected '{1}'",
                mExternalVideoMedia1.ClipBegin, begin.ToString());
            Assert.IsTrue(
                split.IsEqualTo(secondPartVideo.ClipBegin),
                "Unexpected clip begin, was '{0}' expected '{1}'",
                secondPartVideo.ClipBegin, split.ToString());
            Assert.IsTrue(
                end.IsEqualTo(secondPartVideo.ClipEnd),
                "Unexpected clip end, was '{0}' expected '{1}'",
                secondPartVideo.ClipEnd, end.ToString());
        }

        #endregion

        #region IClipped tests

        [Test]
        public void ClipBegin_Basics()
        {
            Assert.IsTrue(
                Time.Zero.IsEqualTo(mExternalVideoMedia1.ClipBegin),
                "The default value of clipBegin is {0} and not {1} as expected",
                mExternalVideoMedia1.ClipBegin.ToString(), Time.Zero.ToString());
            Time val = new Time(TimeSpan.FromSeconds(10));
            mExternalVideoMedia1.ClipBegin = val;
            Assert.IsTrue(val.IsEqualTo(mExternalVideoMedia1.ClipBegin), "Unexpected clipBegin return value");
            Assert.IsFalse(Type.ReferenceEquals(val, mExternalVideoMedia1.ClipBegin),
                           "ClipBegin was must not be set by reference");
            val = new Time(10);
            mExternalVideoMedia1.ClipBegin = val;
            Assert.IsTrue(val.IsEqualTo(mExternalVideoMedia1.ClipBegin), "Unexpected clipBegin return value");
            val = mExternalVideoMedia1.ClipEnd;
            mExternalVideoMedia1.ClipBegin = val;
            Assert.IsTrue(val.IsEqualTo(mExternalVideoMedia1.ClipBegin), "Unexpected clipBegin return value");
        }

        [Test]
        [ExpectedException(typeof (exception.MethodParameterIsNullException))]
        public void ClipBegin_NullValue()
        {
            mExternalVideoMedia1.ClipBegin = null;
        }

        [Test]
        [ExpectedException(typeof (exception.MethodParameterIsOutOfBoundsException))]
        public void ClipBegin_OutOfBounds_GtClipEnd()
        {
            mExternalVideoMedia1.ClipEnd = new Time(TimeSpan.FromSeconds(10));
            mExternalVideoMedia1.ClipBegin = new Time(TimeSpan.FromSeconds(20));
        }

        [Test]
        [ExpectedException(typeof (exception.MethodParameterIsOutOfBoundsException))]
        public void ClipBegin_OutOfBounds_Negative()
        {
            mExternalVideoMedia1.ClipBegin = new Time(TimeSpan.FromSeconds(-10));
        }

        [Test]
        public void ClipEnd_Basics()
        {
            Assert.IsTrue(
                Time.MaxValue.IsEqualTo(mExternalVideoMedia1.ClipEnd),
                "The default value of clipBegin is {0} and not {1} as expected",
                mExternalVideoMedia1.ClipEnd.ToString(), Time.MaxValue.ToString());
            Time val = new Time(TimeSpan.FromSeconds(10));
            mExternalVideoMedia1.ClipEnd = val;
            Assert.IsTrue(val.IsEqualTo(mExternalVideoMedia1.ClipEnd), "Unexpected clipBegin return value");
            Assert.IsFalse(Type.ReferenceEquals(val, mExternalVideoMedia1.ClipBegin),
                           "ClipBegin was must not be set by reference");
            val = new Time(10);
            mExternalVideoMedia1.ClipEnd = val;
            Assert.IsTrue(val.IsEqualTo(mExternalVideoMedia1.ClipEnd), "Unexpected clipBegin return value");
            val = mExternalVideoMedia1.ClipBegin;
            mExternalVideoMedia1.ClipEnd = val;
            Assert.IsTrue(val.IsEqualTo(mExternalVideoMedia1.ClipEnd), "Unexpected clipBegin return value");
        }

        [Test]
        [ExpectedException(typeof (exception.MethodParameterIsNullException))]
        public void ClipEnd_NullValue()
        {
            mExternalVideoMedia1.ClipEnd = null;
        }

        [Test]
        [ExpectedException(typeof (exception.MethodParameterIsOutOfBoundsException))]
        public void ClipEnd_OutOfBoundsValue_LtClipBegin()
        {
            mExternalVideoMedia1.ClipBegin = new Time(TimeSpan.FromSeconds(10));
            mExternalVideoMedia1.ClipEnd = new Time(TimeSpan.FromSeconds(5));
        }

        #endregion

        #region ISized tests

        [Test]
        public void Height_Basics()
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
        [ExpectedException(typeof (exception.MethodParameterIsOutOfBoundsException))]
        public void Height_NegativeValue()
        {
            mExternalVideoMedia1.Height = -10;
        }

        [Test]
        public void Width_Basics()
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
        [ExpectedException(typeof (exception.MethodParameterIsOutOfBoundsException))]
        public void Width_NegativeValue()
        {
            mExternalVideoMedia1.Width = -10;
        }

        #endregion

        #region IValueEquatable tests

        [Test]
        public override void ValueEquals_Basics()
        {
            base.ValueEquals_Basics();
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
        public void valueEquals_ClipBegin()
        {
            mExternalVideoMedia1.ClipBegin = Time.Zero;
            mExternalVideoMedia2.ClipBegin = new Time(TimeSpan.FromSeconds(10));
            Assert.IsFalse(mExternalVideoMedia1.ValueEquals(mExternalVideoMedia2),
                           "ExternalVideoMedia with different clipBegin can not be value equal");
            mExternalVideoMedia2.ClipBegin = mExternalVideoMedia1.ClipBegin;
            Assert.IsTrue(mExternalVideoMedia1.ValueEquals(mExternalVideoMedia2),
                          "Expected ExternalVideoMedia to be value equal");
        }

        [Test]
        public void valueEquals_ClipEnd()
        {
            mExternalVideoMedia1.ClipEnd = Time.Zero;
            mExternalVideoMedia2.ClipEnd = new Time(TimeSpan.FromSeconds(10));
            Assert.IsFalse(mExternalVideoMedia1.ValueEquals(mExternalVideoMedia2),
                           "ExternalVideoMedia with different clipEnd can not be value equal");
            mExternalVideoMedia2.ClipEnd = mExternalVideoMedia1.ClipEnd;
            Assert.IsTrue(mExternalVideoMedia1.ValueEquals(mExternalVideoMedia2),
                          "Expected ExternalVideoMedia to be value equal");
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
        public void ValueEquals_Width()
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

        #region Media tests

        [Test]
        public override void Copy_ValueEqualsAndReferenceDiffers()
        {
            mExternalVideoMedia1.ClipBegin = new Time(10000);
            mExternalVideoMedia1.ClipBegin = new Time(15000);
            mExternalVideoMedia1.Height = 600;
            mExternalVideoMedia1.Width = 800;
            base.Copy_ValueEqualsAndReferenceDiffers();
        }

        [Test]
        public override void Export_ValueEqualsPresentationsOk()
        {
            mExternalVideoMedia1.ClipBegin = new Time(10000);
            mExternalVideoMedia1.ClipBegin = new Time(15000);
            mExternalVideoMedia1.Height = 600;
            mExternalVideoMedia1.Width = 800;
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