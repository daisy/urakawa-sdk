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
//            testObject.OuputDir = @"C:/svnroot/Urakawa/trunk/urakawa/implementation/XukToZed/XukToZed/output";
            testObject.OuputDir = @"../../output";
            testObject.contextFolderName = @"C:\ObiTest";

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
        private string strContextFolder = ".";

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

        public string contextFolderName
        {
            get
            {
                return strContextFolder;
            }
            set
            {
                strContextFolder = value;
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

            #region this region only needed for debugging, will be removed
            string[] strSmilFiles = System.IO.Directory.GetFiles(strOutputDir, "*.smil");
            foreach(string aSmilFile in strSmilFiles)
            {
                System.IO.File.Delete(aSmilFile);
            }
            #endregion

            XmlDocument resDoc = new XmlDocument();
            resDoc.LoadXml(dataHolder.ToString());

            XmlWriterSettings fileSettings = new XmlWriterSettings();
            fileSettings.Indent = true;

            resDoc.Save(strOutputDir + "/raw.xml");

 
            XmlNamespaceManager xPathNSManager = new XmlNamespaceManager((XmlNameTable)new NameTable());
            xPathNSManager.AddNamespace("smil", "http://www.w3.org/2001/SMIL20/Language");
            xPathNSManager.AddNamespace("opf", "http://openebook.org/namespaces/oeb-package/1.0/");
            xPathNSManager.AddNamespace("ncx", "http://www.daisy.org/z3986/2005/ncx/");


            XmlNode ncxTree = resDoc.DocumentElement.SelectSingleNode("//ncx:ncx", xPathNSManager);
            XmlWriter ncxFile = XmlWriter.Create(strOutputDir + "/navigation.ncx",fileSettings);
            ncxFile.WriteNode(ncxTree.CreateNavigator(), false);
            ncxFile.Close();
            ncxTree.ParentNode.RemoveChild(ncxTree); //remove the written bit


            XmlNode opfTree = resDoc.DocumentElement.SelectSingleNode("//opf:package", xPathNSManager);
            XmlWriter opfFile = XmlWriter.Create(strOutputDir + "/" + "package" + ".opf", fileSettings);
            opfFile.WriteNode(opfTree.CreateNavigator(), false);
            opfFile.Close();
            opfTree.ParentNode.RemoveChild(opfTree); //remove the written bit

            XmlNodeList smilTrees = resDoc.DocumentElement.SelectNodes("//smil:smil", xPathNSManager);

            for (int i = smilTrees.Count - 1; i > -1; i--)
            {
                XmlElement newRoot = (XmlElement)smilTrees[i];
                XmlWriter smilFile = XmlWriter.Create(strOutputDir + "/" + newRoot.GetAttribute("filename") + ".smil",fileSettings);
                newRoot.RemoveAttribute("filename");
                smilFile.WriteNode(newRoot.CreateNavigator(),false);
                smilFile.Close();
                newRoot.ParentNode.RemoveChild(newRoot);
            }

            resDoc.Save(strOutputDir + "/raw.xml");

            XmlNodeList filesToCopy = resDoc.DocumentElement.SelectNodes("filenames/file",xPathNSManager);
            foreach(XmlNode fileNode in filesToCopy)
            {
                string strSourceFileName = strContextFolder + "\\" + fileNode.InnerText;
                strSourceFileName = strSourceFileName.Replace("\\", "/");

                string strDestFileName = fileNode.InnerText.Substring((fileNode.InnerText.LastIndexOf("/") > 0) ? fileNode.InnerText.LastIndexOf("/")+1 : 0);
                strDestFileName = OuputDir + "\\" + strDestFileName;
                strDestFileName = strDestFileName.Replace("\\", "/");
                try
                {
                    System.IO.File.Copy(strSourceFileName,strDestFileName, true);
                }
                catch (Exception eAnything)
                {
                    System.Diagnostics.Debug.WriteLine(eAnything.ToString());
                }
            }


        }
    }
}
