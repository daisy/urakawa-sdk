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
            XukToZed.XukToZed testObject = new XukToZed.XukToZed(@"..\..\..\XukToZed\XukToZed.xslt");
            Assert.IsNotNull(testObject);
            testObject.OuputDir = @"C:\ObiExports\Debug";
            testObject.contextFolderName = @"C:\ObiTest";

            string tmpPackageName = "someothersillyname.opf";
            testObject.TransformationArguments.AddParam("packageFilename", "", tmpPackageName);

            string tmpNcxName = "BetterNameThanJustNavigation.ncx";
            testObject.TransformationArguments.AddParam("ncxFilename", "", tmpNcxName);

            XmlReaderSettings readSettings = new XmlReaderSettings();
            readSettings.XmlResolver = null;

            //            XmlDocument testNamespacesDoc = new XmlDocument();
            //            testNamespacesDoc.Load(@"C:\ObiTest\First_Obi_Test.xuk");

            XmlReader testDoc = XmlReader.Create(@"C:\ObiTest\1 sect 2 phrase.xuk", readSettings);
            //XmlReader testDoc = XmlReader.Create(@"C:\ObiTest\First_Obi_Test.xuk",readSettings);

            testObject.WriteZed(testDoc);
            Assert.IsTrue(System.IO.File.Exists(testObject.OuputDir + "/" + tmpNcxName), "NCX file missing!");
            Assert.IsTrue(System.IO.File.Exists(testObject.OuputDir + "/" + tmpPackageName), "OPF file missing!");

        }
    }

}
