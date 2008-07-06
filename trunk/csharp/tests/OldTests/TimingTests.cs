using System;
using NUnit.Framework;
using urakawa.media;
using urakawa.media.timing;

namespace urakawa.unitTests.fixtures.timing
{
    [TestFixture]
    public class TimingTests
    {
        private Time t0, t500, t500V2;

        [SetUp]
        public void Initialize()
        {
            t0 = new Time(0);
            t500 = new Time(500);
            t500V2 = new Time(500);
        }

        [Test]
        public void testTimeGreaterThan()
        {
            Assert.IsTrue(t500.IsGreaterThan(t0), "t500 is not greater than t0");
            Assert.IsFalse(t0.IsGreaterThan(t500), "t0 is greater than t500");
            Assert.IsFalse(t0.IsGreaterThan(t0), "t0 is greater than t0");
            Assert.IsFalse(t500.IsGreaterThan(t500V2), "t500 is greater than t500V2");
        }

        [Test]
        public void testTimeLessThan()
        {
            Assert.IsFalse(t500.IsLessThan(t0), "t500 is less than t0");
            Assert.IsTrue(t0.IsLessThan(t500), "t0 is not less than t500");
            Assert.IsFalse(t0.IsLessThan(t0), "t0 is less than t0");
            Assert.IsFalse(t500.IsLessThan(t500V2), "t500 is less than t500V2");
        }

        [Test]
        public void testTimeEqual()
        {
            Assert.IsTrue(t0.IsEqualTo(t0), "t0 and t0 are not equal");
            Assert.IsTrue(t500.IsEqualTo(t500V2), "t500 and t500v2 are not equal");
            Assert.IsFalse(t0.IsEqualTo(t500), "t0 and t500 are equal");
        }
    }
}