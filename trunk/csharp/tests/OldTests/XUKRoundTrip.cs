using System;
using NUnit.Framework;
using urakawa.core;
using urakawa.unitTests.testbase;
using System.IO;

namespace urakawa.unitTests.fixtures.xukfiles.roundtrip

{
    /// <summary>
    /// Summary description for RoundTrip.
    /// </summary>
    [TestFixture]
    public class XUKRoundTrip : RoundTrip
    {
        [TestFixtureSetUp]
        public virtual void InitFixture()
        {
            mDefaultFile = "../../XukWorks/roundTripTestSample.xuk";
        }
    }
}