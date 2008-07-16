using System;
using NUnit.Framework;
using urakawa.core;
using urakawa.examples;

namespace urakawa.oldTests
{
    [TestFixture]
    public class ExampleCustomPresentationTests : oldTests.BasicPresentationTests
    {
        [SetUp]
        public override void Init()
        {
            mProject = new Project();
            mProject.AddNewPresentation();
        }
    }
}