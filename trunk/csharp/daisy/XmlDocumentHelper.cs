using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using urakawa.ExternalFiles;
using urakawa.xuk;

namespace urakawa.daisy
{
    public static class XmlDocumentHelper
    {
        public static XmlDocument CreateStub_DTBDocument(string language, string strInternalDTD, List<ExternalFileData> list_ExternalStyleSheets)
        {
            XmlDocument DTBDocument = new XmlDocument();
            DTBDocument.XmlResolver = null;

            DTBDocument.CreateXmlDeclaration("1.0", "utf-8", null);
            DTBDocument.AppendChild(DTBDocument.CreateDocumentType("dtbook",
                "-//NISO//DTD dtbook 2005-3//EN",
                "http://www.daisy.org/z3986/2005/dtbook-2005-3.dtd",
                strInternalDTD));

            if (list_ExternalStyleSheets.Count > 0)
            {
                foreach (ExternalFileData efd in list_ExternalStyleSheets)
                {
                    if (efd is CSSExternalFileData)
                    {
                        DTBDocument.AppendChild(
                        DTBDocument.CreateProcessingInstruction("xml-stylesheet", "type=\"text/css\" href=\"" + efd.OriginalRelativePath + "\""));
                    }
                    else if (efd is XSLTExternalFileData)
                    {
                        DTBDocument.AppendChild(
                        DTBDocument.CreateProcessingInstruction("xml-stylesheet", "type=\"text/xsl\" href=\"" + efd.OriginalRelativePath + "\""));
                    }
                }
            }


            XmlNode DTBNode = DTBDocument.CreateElement(null,
                "dtbook",
                "http://www.daisy.org/z3986/2005/dtbook/");

            DTBDocument.AppendChild(DTBNode);


            CreateAppendXmlAttribute(DTBDocument, DTBNode, "version", "2005-3");
            CreateAppendXmlAttribute(DTBDocument, DTBNode, XmlReaderWriterHelper.XmlLang, (String.IsNullOrEmpty(language) ? "en-US" : language));


            XmlNode headNode = DTBDocument.CreateElement(null, "head", DTBNode.NamespaceURI);
            DTBNode.AppendChild(headNode);
            XmlNode bookNode = DTBDocument.CreateElement(null, "book", DTBNode.NamespaceURI);
            DTBNode.AppendChild(bookNode);

            return DTBDocument;
        }

        public static XmlDocument CreateStub_XhtmlDocument(string language, string strInternalDTD, List<ExternalFileData> list_ExternalStyleSheets)
        {
            XmlDocument XhtmlDocument = new XmlDocument();
            XhtmlDocument.XmlResolver = null;

            //XhtmlDocument.CreateXmlDeclaration("1.0", "utf-8", null);
            //XhtmlDocument.AppendChild(XhtmlDocument.CreateDocumentType("html",
            //"-//NISO//DTD dtbook 2005-3//EN",
            //"http://www.daisy.org/z3986/2005/dtbook-2005-3.dtd",
            //strInternalDTD));

            XmlNode rootNode = XhtmlDocument.CreateElement(null,
                "   html",
                "http://www.w3.org/1999/xhtml");
            CreateAppendXmlAttribute(XhtmlDocument, rootNode, "xmlns:epub", "http://www.idpf.org/2007/ops");
            XhtmlDocument.AppendChild(rootNode);

            XmlNode headNode = XhtmlDocument.CreateElement(null, "head", rootNode.NamespaceURI);
            rootNode.AppendChild(headNode);

            if (list_ExternalStyleSheets.Count > 0)
            {
                foreach (ExternalFileData efd in list_ExternalStyleSheets)
                {
                    if (efd is CSSExternalFileData)
                    {
                        XhtmlDocument.AppendChild(
                        XhtmlDocument.CreateProcessingInstruction("xml-stylesheet", "type=\"text/css\" href=\"" + efd.OriginalRelativePath + "\""));
                    }
                    else if (efd is XSLTExternalFileData)
                    {
                        XhtmlDocument.AppendChild(
                        XhtmlDocument.CreateProcessingInstruction("xml-stylesheet", "type=\"text/xsl\" href=\"" + efd.OriginalRelativePath + "\""));
                    }
                }

            }

            XmlNode bodyNode = XhtmlDocument.CreateElement(null, "body", rootNode.NamespaceURI);
            rootNode.AppendChild(bodyNode);

            return XhtmlDocument;
        }


        public static XmlNode GetFirstChildElementOrSelfWithName(XmlNode root, bool deep, string localName, string namespaceUri)
        {
            foreach (XmlNode node in GetChildrenElementsOrSelfWithName(root, deep, localName, namespaceUri, true))
            {
                return node;
            }
            return null;
        }

        public static IEnumerable<XmlNode> GetChildrenElementsOrSelfWithName(XmlNode root, bool deep, string localName, string namespaceUri, bool breakOnFirstFound)
        {
            if (root.NodeType == XmlNodeType.Document)
            {
                XmlNode element = null;
                XmlDocument doc = (XmlDocument)root;
                IEnumerator docEnum = doc.GetEnumerator();
                while (docEnum.MoveNext())
                {
                    XmlNode node = (XmlNode)docEnum.Current;

                    if (node != null
                        && node.NodeType == XmlNodeType.Element)
                    {
                        element = node;
                        break; // first element is ok.
                    }
                }

                if (element == null)
                {
                    yield break;
                }

                foreach (XmlNode childNode in GetChildrenElementsOrSelfWithName(element, deep, localName, namespaceUri, breakOnFirstFound))
                {
                    yield return childNode;

                    if (breakOnFirstFound)
                    {
                        yield break;
                    }
                }

                yield break;
            }

            if (root.NodeType != XmlNodeType.Element)
            {
                yield break;
            }

            if (root.LocalName == localName || root.Name == localName)
            {
                if (!String.IsNullOrEmpty(namespaceUri))
                {
                    if (root.NamespaceURI == namespaceUri)
                    {
                        yield return root;

                        if (breakOnFirstFound)
                        {
                            yield break;
                        }
                    }
                }
                else
                {
                    yield return root;

                    if (breakOnFirstFound)
                    {
                        yield break;
                    }
                }
            }

            IEnumerator enumerator = root.GetEnumerator();
            while (enumerator.MoveNext())
            {
                XmlNode node = (XmlNode)enumerator.Current;

                if (node.NodeType != XmlNodeType.Element)
                {
                    continue;
                }

                if (deep)
                {
                    foreach (XmlNode childNode in GetChildrenElementsOrSelfWithName(node, deep, localName, namespaceUri, breakOnFirstFound))
                    {
                        yield return childNode;

                        if (breakOnFirstFound)
                        {
                            yield break;
                        }
                    }
                }
                else
                {
                    if (node.LocalName == localName || node.Name == localName)
                    {
                        if (!String.IsNullOrEmpty(namespaceUri))
                        {
                            if (node.NamespaceURI == namespaceUri)
                            {
                                yield return node;

                                if (breakOnFirstFound)
                                {
                                    yield break;
                                }
                            }
                        }
                        else
                        {
                            yield return node;

                            if (breakOnFirstFound)
                            {
                                yield break;
                            }
                        }
                    }
                }
            }

            yield break;
        }

        //public static void WriteXmlDocumentToFile(XmlDocument xmlDoc, string path)
        //{
        //    //SEE 
        //    //SaveXukAction.WriteXmlDocument(xmlDoc, path);
        //    //if (!File.Exists(path))
        //    //{
        //    //    File.Create(path).Close();
        //    //}
        //    //XmlTextWriter writer = null;
        //    //try
        //    //{
        //    //    writer = new XmlTextWriter(path, null);
        //    //    writer.Formatting = Formatting.Indented;
        //    //    xmlDoc.Save(writer);
        //    //}
        //    //finally
        //    //{
        //    //    if (writer != null)
        //    //    {
        //    //        writer.Close();
        //    //    }
        //    //}
        //}

        public static XmlAttribute CreateAppendXmlAttribute(XmlDocument xmlDoc, XmlNode node, string name, string val)
        {
            XmlAttribute attr = xmlDoc.CreateAttribute(name);
            attr.Value = val;
            node.Attributes.Append(attr);
            return attr;
        }

        public static XmlAttribute CreateAppendXmlAttribute(XmlDocument xmlDoc, XmlNode node, string name, string val, string strNamespace)
        {
            XmlAttribute attr = null;
            if (name.IndexOf(':') >= 0)
            {
                //name.Contains(":")
                string[] splitArray = name.Split(':');

                if (splitArray[0] == XmlReaderWriterHelper.NS_PREFIX_XMLNS)
                {
                    string nsURI = node.GetNamespaceOfPrefix(splitArray[1]);
                    if (!String.IsNullOrEmpty(nsURI))
                    {
                        attr = xmlDoc.CreateAttribute(splitArray[0], splitArray[1], nsURI);
                    }
                }
                attr = xmlDoc.CreateAttribute(splitArray[0], splitArray[1], strNamespace);


                //XmlNode parentNode = xmlDoc.DocumentElement;

                //string parentAttributeName = "xmlns:" + splitArray[0];

                //if (parentNode != null
                //    && parentNode.Attributes != null
                //    && parentNode.Attributes.GetNamedItem(parentAttributeName) != null
                //    && parentNode.Attributes.GetNamedItem(parentAttributeName).Value == strNamespace)
                //{
                //    //System.Console.WriteLine ( parentNode.Name );
                //    // do nothing
                //}
                //else if (parentNode != null)
                //{
                //    CreateAppendXmlAttribute(xmlDoc, parentNode, parentAttributeName, strNamespace);
                //}
                //attr = xmlDoc.CreateAttribute(name, "SYSTEM");
            }
            else
            {
                attr = xmlDoc.CreateAttribute(name);
            }
            attr.Value = val;
            node.Attributes.Append(attr);
            return attr;
        }
    }
}
