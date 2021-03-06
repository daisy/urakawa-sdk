using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Xsl;
using System.Xml;
using NUnit.Framework;
using XukToZed;


namespace XukToZedTests
{
    [TestFixture]
    public class XukToZedTests
    {

        public XukToZedTests()
        {
        }

        [Test]
        public void FirstTest()
        {
            XukToZed.XukToZed testObject = new XukToZed.XukToZed();//@"..\..\..\XukToZed\XukToZed.xslt");
            Assert.IsNotNull(testObject);
            testObject.OuputDir = @"C:\devel\temp";
            testObject.contextFolderName = @"C:\devel\temp";

            string tmpPackageName = "someothersillyname.opf";
            testObject.TransformationArguments.AddParam("packageFilename", "", tmpPackageName);

            string tmpNcxName = "BetterNameThanJustNavigation.ncx";
            testObject.TransformationArguments.AddParam("ncxFilename", "", tmpNcxName);

            XmlReaderSettings readSettings = new XmlReaderSettings();
            readSettings.XmlResolver = null;

            //            XmlDocument testNamespacesDoc = new XmlDocument();
            //            testNamespacesDoc.Load(@"C:\ObiTest\First_Obi_Test.xuk");

            //XmlReader testDoc = XmlReader.Create(@"C:\ObiTest\obi084_1.xuk", readSettings);
            //XmlReader testDoc = XmlReader.Create(@"C:\ObiTest\First_Obi_Test.xuk",readSettings);
            XmlReader testDoc = XmlReader.Create(@"C:\devel\urakawa\trunk\urakawa\application\misc\Obi_8.5.1_Project\New Project.xuk");
      
            testObject.WriteZed(testDoc);
            Assert.IsTrue(System.IO.File.Exists(testObject.OuputDir + "/" + tmpNcxName), "NCX file missing!");
            Assert.IsTrue(System.IO.File.Exists(testObject.OuputDir + "/" + tmpPackageName), "OPF file missing!");

        }
    }

}
