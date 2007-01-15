using System;
using NUnit.Framework;
using urakawa.core;

namespace urakawa.unitTests.fixtures.standalone
{
	/// <summary>
	/// Summary description for ForAnEmptyPresentation.
	/// </summary>
	[TestFixture] 
	public class ForAnEmptyPresentation : testbase.BasicPresentationTests
	{
		[SetUp] public void Init() 
		{
			mProject = new Project();
		}
	}
}
