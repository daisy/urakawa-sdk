using System;
using System.Collections.Generic;
using System.Text;
using urakawa.media.data;
using NUnit.Framework;

namespace urakawa.unitTests.mediaDataTests
{
	/// <summary>
	/// Basic tests for <see cref="urakawa.media.data"/> class methods
	/// </summary>
	[TestFixture]
	public class BasicMediaDataTests : MediaDataTestBase
	{
		[Test]
		public void CheckNumberOfFileDataProviders()
		{
			int count = mProject.getPresentation().getDataProviderManager().getListOfManagedDataProviders().Count;
			Assert.AreEqual(
				2, count, "Invalid number of DataProviders, expected 2, but found {0:0}", count);
		}
	}
}
