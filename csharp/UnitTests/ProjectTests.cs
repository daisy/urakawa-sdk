using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using urakawa;
using NUnit.Framework;

namespace unittests.urakawa
{
	[TestFixture]
	public class ProjectTests
	{
		public static Uri SampleXukFileDirectoryUri
		{
			get
			{
				return new Uri(Path.Combine(Directory.GetCurrentDirectory(), Properties.Settings.Default.SampleXukFileDirectory));
			}
		}
	}
}
