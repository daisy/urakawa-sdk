using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Xsl;
using System.Xml;
using NUnit.Framework;

namespace XukToZed
{
    [TestFixture]
    public class XukToZedTests {

        public XukToZedTests()
        { 
        }

        [Test]
        public void FirstTest()
        {
            XukToZed testObject = new XukToZed(@"..\..\XukToZed.xslt");
            Assert.IsNotNull(testObject);
            testObject.OuputDir = @"../../output";

            XmlReader testDoc = XmlReader.Create(@"C:\ObiTest\First_Obi_Test_nonamespaces.xuk");
            XmlReaderSettings readSettings = new XmlReaderSettings();
            readSettings.XmlResolver = null;
            XmlReader testDoc = XmlReader.Create(@"C:\ObiTest\First_Obi_Test.xuk");

            testObject.WriteZed(testDoc);

        }
    }


    public class XukToZed
    {
        System.Xml.Xsl.XslCompiledTransform theTransformer = new XslCompiledTransform(true);
        private string strOutputDir = ".";

        public XukToZed(string pathToStylesheet)
        {
            theTransformer.Load(pathToStylesheet);
        }

        public string OuputDir
        {
            get
            { 
                return strOutputDir;
            }
            set
            {
                strOutputDir = value;
            }
        }

        public void WriteZed(XmlReader input)
        {
            System.IO.StringWriter dataHolder = new System.IO.StringWriter();
            XmlWriter results = XmlWriter.Create((System.IO.TextWriter)dataHolder);
            try
            {
                theTransformer.Transform(input, results);
            }
            catch
            { 
                results = null;
            }

            XmlDocument resDoc = new XmlDocument();
            resDoc.LoadXml(dataHolder.ToString());

            XmlWriterSettings fileSettings = new XmlWriterSettings();
            fileSettings.Indent = true;

            XmlNode ncxTree = resDoc.DocumentElement.SelectSingleNode("//ncx");
            XmlWriter ncxFile = XmlWriter.Create(strOutputDir + "/navigation.ncx",fileSettings);
            ncxFile.WriteNode(ncxTree.CreateNavigator(), false);
            ncxFile.Close();

            XmlNode smilTree = resDoc.DocumentElement.SelectSingleNode("//smil");
            XmlWriter smilFile = XmlWriter.Create(strOutputDir + "/everything.smil",fileSettings);
            smilFile.WriteNode(smilTree.CreateNavigator(), false);
            smilFile.Close();
        }
    }
}
