using System;
using NUnit.Framework;
using urakawa.core;

namespace urakawa.test.unitTests
{
	/// <summary>
	/// Tests for <see cref="ExampleCustomProperty"/>
	/// </summary>
	[TestFixture] public class ExampleCustomPropertyTests
	{
		private urakawa.project.Project mProject;

		public ExampleCustomPropertyTests()
		{
		}

		[SetUp] public void Init() 
		{
			Presentation pres = new Presentation(new urakawa.examples.ExampleCustomPropertyFactory(pres));
			mProject = new urakawa.project.Project(pres, new urakawa.project.MetadataFactory());
		}
	}
}
