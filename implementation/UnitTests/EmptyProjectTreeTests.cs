using System;
using NUnit.Framework;
using urakawa.core;

namespace urakawa.unitTests
{
	[TestFixture]
	public class EmptyProjectTreeTests : urakawa.unitTests.testbase.TreeTests
	{
		[SetUp]
		public void Init()
		{
			mProject = new urakawa.project.Project();
		}
	}
}
