using System;
using NUnit.Framework;
using urakawa.core;

namespace urakawa.test.unitTests
{
	/// <summary>
	/// Summary description for ForAnEmptyPresentation.
	/// </summary>
	[TestFixture] 
	public class ForAnEmptyPresentation : BasicPresentationTests
	{
		//urakawa.core.Presentation mPresentation = null;

		[SetUp] public void Init() 
		{
			mPresentation = new Presentation();
		}
		
	}
}
