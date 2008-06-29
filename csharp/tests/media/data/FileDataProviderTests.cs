using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using NUnit.Framework;

namespace urakawa.media.data
{
	[TestFixture]
	public class FileDataProviderTests : IDataProviderTests
	{
		protected FileDataProvider mFileDataProvider1 { get { return mDataProvider1 as FileDataProvider; } }
		protected FileDataProvider mFileDataProvider2 { get { return mDataProvider2 as FileDataProvider; } }
		protected FileDataProvider mFileDataProvider3 { get { return mDataProvider3 as FileDataProvider; } }

		public FileDataProviderTests()
			: base(typeof(FileDataProvider).Name, ToolkitSettings.XUK_NS)
		{
		}

		#region IDataProvider tests
		[Test]
		public override void getInputStream_InitialState()
		{
			base.getInputStream_InitialState();
		}

		[Test]
		public override void getInputStream_CanGetMultiple()
		{
			base.getInputStream_CanGetMultiple();
		}

		[Test]
		public override void getOutputStream_InitialState()
		{
			base.getOutputStream_InitialState();
		}

		[Test]
		[ExpectedException(typeof(exception.OutputStreamOpenException))]
		public override void getOutputStream_CannotGetMultiple()
		{
			base.getOutputStream_CannotGetMultiple();
		}

		[Test]
		public override void getOutputStream_RetrieveDataWritten()
		{
			base.getOutputStream_RetrieveDataWritten();
		}

		[Test]
		public override void getUid_Basics()
		{
			base.getUid_Basics();
		}

		[Test]
		public override void delete_Basics()
		{
			base.delete_Basics();
		}

		[Test]
		public void delete_DataFilesDeleted()
		{
			FileDataProviderManager mngr = mPresentation.DataProviderManager as FileDataProviderManager;
			string path = mngr.DataFileDirectoryFullPath;
			Assert.Greater(mngr.ListOfDataProviders.Count, 0, "The manager does not manage any DataProviders");
			foreach (IDataProvider prov in mngr.ListOfDataProviders)
			{
				Stream outStm = prov.GetOutputStream();
				try
				{
					outStm.WriteByte(0xAA);//Ensure that files are created
				}
				finally
				{
					outStm.Close();
				}
				prov.Delete();
			}
			Assert.AreEqual(
				0, mngr.ListOfDataProviders.Count, 
				"The manager still contains DataProviders after they are all deleted");
			Assert.IsTrue(
				Directory.Exists(path), 
				"The data file directory of the FileDataManager does not exist");
			Assert.AreEqual(
				0, Directory.GetFiles(path).Length,
				"The data file directory of the FileDataManager is not empty");

		}

		[Test]
		[ExpectedException(typeof(exception.InputStreamsOpenException))]
		public override void delete_OpenInputStream()
		{
			base.delete_OpenInputStream();
		}

		[Test]
		[ExpectedException(typeof(exception.OutputStreamOpenException))]
		public override void delete_OpenOutputStream()
		{
			base.delete_OpenOutputStream();
		}

		[Test]
		public override void copy_Basics()
		{
			base.copy_Basics();
		}
		#endregion

		#region IValueEquatable tests
		[Test]
		public override void valueEquals_Basics()
		{
			base.valueEquals_Basics();
		}

		[Test]
		public override void valueEquals_MimeType()
		{
			base.valueEquals_MimeType();
		}
		
		#endregion
	}
}
