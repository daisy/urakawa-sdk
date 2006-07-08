using System;
using NUnit.Framework;
using urakawa.core;
using urakawa.unitTests.testbase;
using System.IO;

namespace urakawa.unitTests.fixtures.xukfiles

{
	/// <summary>
	/// Tests for opening and saving XUK files
	/// </summary>
	public class XUKOpen
	{
		protected string mDefaultFile = "";
		protected urakawa.project.Project mProject;

		[SetUp] public void Init()
		{
			mProject = new urakawa.project.Project();
		}

		/// <summary>
		/// Tests opening of XUK files
		/// </summary>
		[Test] public void OpenXUK()
		{
			mProject = new urakawa.project.Project();
			
			string filepath = Directory.GetCurrentDirectory();

			Uri fileUri = new Uri(filepath);
			
			fileUri = new Uri(fileUri, mDefaultFile);
			bool opened = mProject.openXUK(fileUri);
			Assert.IsTrue(opened, "Failed to load XUK file {0}", mDefaultFile);
		}


    		
	}
}
