using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using urakawa.core;
using urakawa.media.data;
using NUnit.Framework;

namespace urakawa.unitTests.mediaDataTests
{
	/// <summary>
	/// Base clas for fixtures testing functionality in the <see cref="urakawa.media.data"/> namespace
	/// </summary>
	public class MediaDataTestBase : testbase.TestCollectionBase
	{
		protected Uri mXukFileUri = new Uri(new Uri(Directory.GetCurrentDirectory()), "../XukWorks/MediaDataSample/MediaDataSample.xuk");

		[SetUp]
		public void Init()
		{
			mProject = new Project();
			bool openOK = mProject.openXUK(mXukFileUri);
			Assert.IsTrue(openOK, "Could not open Xuk file {0}", mXukFileUri.AbsolutePath);
		}

		[TearDown]
		public void Terminate()
		{
			Init();
			//Delete any files in the data directory not used by the FileDataProviderManager
			FileDataProviderManager dataProvMngr = (FileDataProviderManager)mProject.getPresentation().getDataProviderManager();
			DirectoryInfo dataDI = new DirectoryInfo(dataProvMngr.getDataFileDirectoryFullPath());
			foreach (FileInfo file in dataDI.GetFiles())
			{
				bool found = false;
				foreach (FileDataProvider fileDataProv in dataProvMngr.getListOfManagedDataProviders())
				{
					if (file.FullName.ToLower() == fileDataProv.getDataFileFullPath())
					{
						found = true;
						break;
					}
				}
				if (!found) file.Delete();
			}
		}
	}
}