using System;
using NUnit.Framework;
using urakawa.core;

namespace urakawa.unitTests.fixtures
{
	/// <summary>
	/// Summary description for SimpleManualProjectForTreeValidation.
	/// </summary>
	[TestFixture]
	public class EmptyProjectForTreeValidation : testbase.TreeNodeValidationTests
	{
		/// <summary>
		/// Make a very simple (empty) presentation by hand
		/// </summary>
		[SetUp]public void Init()
		{
			urakawa.Presentation pres = new Presentation();
			urakawa.project.MetadataFactory metafact = new urakawa.project.MetadataFactory();
		
			mProject = new Project(pres, metafact);

		}

	}
}
