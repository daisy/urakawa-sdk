using System;
using NUnit.Framework;
using urakawa.core;
using urakawa.examples;

namespace urakawa.unitTests
{
	[TestFixture]
	public class ExampleCustomPresentationTests : testbase.BasicPresentationTests
	{


		[SetUp]
		public override void Init()
		{
			mProject = new Project();
			mProject.setDataModelFactory(new ExampleCustomDataModelFactory());
			mProject.addNewPresentation();
		}
	}
}
