using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace urakawa.daisy
{
    public static class XmlDocumentHelper
    {
        public static XmlDocument CreateStub_DTBDocument(string language, string strInternalDTD)
        {
            XmlDocument DTBDocument = new XmlDocument();
            DTBDocument.XmlResolver = null;

            DTBDocument.CreateXmlDeclaration("1.0", "utf-8", null);
            DTBDocument.AppendChild(DTBDocument.CreateDocumentType("dtbook",
                "-//NISO//DTD dtbook 2005-3//EN",
                "http://www.daisy.org/z3986/2005/dtbook-2005-3.dtd",
                strInternalDTD));

            XmlNode DTBNode = DTBDocument.CreateElement(null,
                "dtbook",
                "http://www.daisy.org/z3986/2005/dtbook/");

            DTBDocument.AppendChild(DTBNode);


            CreateAppendXmlAttribute(DTBDocument, DTBNode, "version", "2005-3");
            CreateAppendXmlAttribute(DTBDocument, DTBNode, "xml:lang", (string.IsNullOrEmpty(language) ? "en-US" : language));


            XmlNode headNode = DTBDocument.CreateElement(null, "head", DTBNode.NamespaceURI);
            DTBNode.AppendChild(headNode);
            XmlNode bookNode = DTBDocument.CreateElement(null, "book", DTBNode.NamespaceURI);
            DTBNode.AppendChild(bookNode);

            return DTBDocument;
        }

        public static XmlNode GetFirstChildElementWithName(XmlNode root, bool deep, string localName, string namespaceUri)
        {
            foreach (XmlNode node in GetChildrenElementsWithName(root, deep, localName, namespaceUri, true))
            {
                return node;
            }
            return null;
        }

        public static IEnumerable<XmlNode> GetChildrenElementsWithName(XmlNode root, bool deep, string localName, string namespaceUri, bool breakOnFirstFound)
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

                foreach (XmlNode childNode in GetChildrenElementsWithName(element, deep, localName, namespaceUri, breakOnFirstFound))
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
                if (!string.IsNullOrEmpty(namespaceUri))
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
                    foreach (XmlNode childNode in GetChildrenElementsWithName(node, deep, localName, namespaceUri, breakOnFirstFound))
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
                        if (!string.IsNullOrEmpty(namespaceUri))
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

        public static void WriteXmlDocumentToFile(XmlDocument xmlDoc, string path)
        {
            XmlTextWriter writer = null;
            try
            {
                if (!File.Exists(path))
                {
                    File.Create(path).Close();
                }

                writer = new XmlTextWriter(path, null);
                writer.Formatting = Formatting.Indented;
                xmlDoc.Save(writer);
            }
            finally
            {
                writer.Close();
                writer = null;
            }
        }

        public static XmlAttribute CreateAppendXmlAttribute(XmlDocument xmlDoc, XmlNode node, string name, string val)
        {
            XmlAttribute attr = xmlDoc.CreateAttribute(name);
            attr.Value = val;
            node.Attributes.Append(attr);
            return attr;
        }

        public static XmlAttribute CreateAppendXmlAttribute ( XmlDocument xmlDoc, XmlNode node, string name, string val, string strNamespace )
            {
            XmlAttribute attr = null;
            if (name.Contains ( ":" ))
                {
                string[] splitArray = name.Split ( ':' );

                XmlNode parentNode = xmlDoc.DocumentElement;

                string parentAttributeName = "xmlns:" + splitArray[0];

                if (parentNode != null &&
                    parentNode.Attributes != null && parentNode.Attributes.GetNamedItem ( parentAttributeName ) != null && parentNode.Attributes.GetNamedItem ( parentAttributeName ).Value == strNamespace)
                    {
                    //System.Console.WriteLine ( parentNode.Name );
                    // do nothing
                    }
                else if (parentNode != null)
                    {
                    CreateAppendXmlAttribute ( xmlDoc, parentNode, parentAttributeName, strNamespace );
                    }
                attr = xmlDoc.CreateAttribute ( name, "SYSTEM" );
                }
            else
                {
                attr = xmlDoc.CreateAttribute ( name );
                }
            attr.Value = val;
            node.Attributes.Append ( attr );
            return attr;
            }


        //private static XmlNode getFirstChildElementsWithName(XmlNode root, bool deep, string localName, string namespaceUri)
        //{
        //    foreach (XmlNode node in getChildrenElementsWithName(root, deep, localName, namespaceUri, true))
        //    {
        //        return node;
        //    }
        //    return null;
        //}

        //private static IEnumerable<XmlNode> getChildrenElementsWithName(XmlNode root, bool deep, string localName, string namespaceUri, bool breakOnFirstFound)
        //{
        //    if (root.NodeType == XmlNodeType.Document)
        //    {
        //        XmlNode element = null;
        //        XmlDocument doc = (XmlDocument)root;
        //        IEnumerator docEnum = doc.GetEnumerator();
        //        while (docEnum.MoveNext())
        //        {
        //            XmlNode node = (XmlNode)docEnum.Current;

        //            if (node != null
        //                && node.NodeType == XmlNodeType.Element)
        //            {
        //                element = node;
        //                break; // first element is ok.
        //            }
        //        }

        //        if (element == null)
        //        {
        //            yield break;
        //        }

        //        foreach (XmlNode childNode in getChildrenElementsWithName(element, deep, localName, namespaceUri, breakOnFirstFound))
        //        {
        //            yield return childNode;

        //            if (breakOnFirstFound)
        //            {
        //                yield break;
        //            }
        //        }

        //        yield break;
        //    }

        //    if (root.NodeType != XmlNodeType.Element)
        //    {
        //        yield break;
        //    }

        //    if (root.LocalName == localName || root.Name == localName)
        //    {
        //        if (!string.IsNullOrEmpty(namespaceUri))
        //        {
        //            if (root.NamespaceURI == namespaceUri)
        //            {
        //                yield return root;

        //                if (breakOnFirstFound)
        //                {
        //                    yield break;
        //                }
        //            }
        //        }
        //        else
        //        {
        //            yield return root;

        //            if (breakOnFirstFound)
        //            {
        //                yield break;
        //            }
        //        }
        //    }

        //    IEnumerator enumerator = root.GetEnumerator();
        //    while (enumerator.MoveNext())
        //    {
        //        XmlNode node = (XmlNode)enumerator.Current;

        //        if (node.NodeType != XmlNodeType.Element)
        //        {
        //            continue;
        //        }

        //        if (deep)
        //        {
        //            foreach (XmlNode childNode in getChildrenElementsWithName(node, deep, localName, namespaceUri, breakOnFirstFound))
        //            {
        //                yield return childNode;

        //                if (breakOnFirstFound)
        //                {
        //                    yield break;
        //                }
        //            }
        //        }
        //        else
        //        {
        //            if (node.LocalName == localName || node.Name == localName)
        //            {
        //                if (!string.IsNullOrEmpty(namespaceUri))
        //                {
        //                    if (node.NamespaceURI == namespaceUri)
        //                    {
        //                        yield return node;

        //                        if (breakOnFirstFound)
        //                        {
        //                            yield break;
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    yield return node;

        //                    if (breakOnFirstFound)
        //                    {
        //                        yield break;
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    yield break;
        //}
    }
}
