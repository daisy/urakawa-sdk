using System;
using NUnit.Framework;
using urakawa.core;
using urakawa.oldTests;

namespace urakawa.oldTests
{
    [TestFixture]
    public class SimpleSampleBasicPresentationTests : BasicPresentationTests
    {
        [TestFixtureSetUp]
        public void InitFixture()
        {
            mDefaultFile = "../../XukWorks/simplesample.xuk";
        }
    }
}