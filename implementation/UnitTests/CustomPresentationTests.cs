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
		public void Init()
		{
			Presentation pres = new Presentation(
				new ExampleCustomCoreNodeFactory(), null, null, new ExampleCustomPropertyFactory(), null);
			mProject = new urakawa.project.Project(pres, null);
		}
	}
}
