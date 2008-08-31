using System;
using NUnit.Framework;
using urakawa.core;
using urakawa.oldTests;

namespace urakawa.oldTests
{
    /// <summary>
    /// Summary description for SampleDTBVisitXmlProperty.
    /// </summary>
    [TestFixture]
    public class SampleDTBVisitXmlProperty : XmlPropertyVisitorTests
    {
        [TestFixtureSetUp]
        public void InitFixture()
        {
            mDefaultFile = "../../XukWorks/SampleDTB2Ver1.xuk";
        }
    }
}