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

            XmlReaderSettings readSettings = new XmlReaderSettings();
            readSettings.XmlResolver = null;

//            XmlDocument testNamespacesDoc = new XmlDocument();
//            testNamespacesDoc.Load(@"C:\ObiTest\First_Obi_Test.xuk");

            XmlReader testDoc = XmlReader.Create(@"C:\ObiTest\Obi_Test_11.xuk",readSettings);
            //XmlReader testDoc = XmlReader.Create(@"C:\ObiTest\First_Obi_Test.xuk",readSettings);

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
            catch(Exception eAnything)
            { 
                results = null;
            }

            string[] strSmilFiles = System.IO.Directory.GetFiles(strOutputDir, "*.smil");
            foreach(string aSmilFile in strSmilFiles)
            {
                System.IO.File.Delete(aSmilFile);
            }

            XmlDocument resDoc = new XmlDocument();
            resDoc.LoadXml(dataHolder.ToString());

            XmlWriterSettings fileSettings = new XmlWriterSettings();
            fileSettings.Indent = true;

            resDoc.Save(strOutputDir + "/raw.xml");

            XmlNode ncxTree = resDoc.DocumentElement.SelectSingleNode("//ncx");
            XmlWriter ncxFile = XmlWriter.Create(strOutputDir + "/navigation.ncx",fileSettings);
            ncxFile.WriteNode(ncxTree.CreateNavigator(), false);
            ncxFile.Close();

            XmlNamespaceManager xPathNSManager = new XmlNamespaceManager((XmlNameTable)new NameTable());
            xPathNSManager.AddNamespace("smil", "http://www.w3.org/2001/SMIL20/Language");

            XmlNodeList smilTrees = resDoc.DocumentElement.SelectNodes("//smil:smil",xPathNSManager);

            for (int i = smilTrees.Count - 1; i > 0; i--)
            {
                XmlElement newRoot = (XmlElement)smilTrees[i];
                XmlWriter smilFile = XmlWriter.Create(strOutputDir + "/" + newRoot.GetAttribute("filename") + ".smil",fileSettings);
                newRoot.RemoveAttribute("filename");
                smilFile.WriteNode(newRoot.CreateNavigator(), false);
                smilFile.Close();
                newRoot.ParentNode.RemoveChild(newRoot);
            }


        }
    }
}
