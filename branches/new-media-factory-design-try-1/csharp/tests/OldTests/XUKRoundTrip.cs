using System;
using NUnit.Framework;
using urakawa.core;
using urakawa.oldTests;
using System.IO;

namespace urakawa.oldTests

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