using System;
using NUnit.Framework;
using urakawa.core;

namespace urakawa.oldTests
{
    /// <summary>
    /// Summary description for ForAnEmptyPresentation.
    /// </summary>
    [TestFixture]
    public class ForAnEmptyPresentation : oldTests.BasicPresentationTests
    {
        [SetUp]
        public override void Init()
        {
            mProject = new Project();
            mProject.AddNewPresentation();
        }
    }
}