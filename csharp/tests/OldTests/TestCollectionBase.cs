using System;
using NUnit.Framework;

namespace urakawa.unitTests.testbase
{
    /// <summary>
    /// Summary description for TestCollectionBase.
    /// </summary>
    public class TestCollectionBase
    {
        protected string mDefaultFile;
        protected Project mProject;

        [SetUp]
        public virtual void Init()
        {
            mProject = new Project();

            string filepath = System.IO.Directory.GetCurrentDirectory() + "\\";

            Uri fileUri = new Uri(filepath);

            fileUri = new Uri(fileUri, mDefaultFile);

            mProject.OpenXuk(fileUri);
        }
    }
}