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
			Presentation pres = new Presentation(
				new Uri(System.IO.Directory.GetCurrentDirectory()),
				new ExampleCustomTreeNodeFactory(), new ExampleCustomPropertyFactory(), null, null, null, null, null, null, null, null);
			mProject = new Project(pres, null);
		}
	}
}
