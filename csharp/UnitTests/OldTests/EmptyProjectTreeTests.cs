using System;
using NUnit.Framework;
using urakawa.core;

namespace urakawa.unitTests
{
	[TestFixture]
	public class EmptyProjectTreeTests : urakawa.unitTests.testbase.TreeTests
	{
		[SetUp]
		public override void Init()
		{
			mProject = new Project();
		}
	}
}
