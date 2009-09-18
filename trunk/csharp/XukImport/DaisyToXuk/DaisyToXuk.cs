using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using urakawa;
using urakawa.metadata;
using core = urakawa.core;

namespace XukImport
{
    public partial class DaisyToXuk
    {
        private readonly string m_outDirectory;
        private string m_Book_FilePath;

        private string m_Xuk_FilePath;
        public string XukPath
        {
            get { return m_Xuk_FilePath; }
        }

        private Project m_Project;
        public Project Project
        {
            get { return m_Project; }
        }

        public DaisyToXuk(string bookfile, string outDir)
        {
            m_PackageUniqueIdAttr = null;
            m_Book_FilePath = bookfile;
            m_outDirectory = outDir;
            if (!m_outDirectory.EndsWith("" + Path.DirectorySeparatorChar))
            {
                m_outDirectory += Path.DirectorySeparatorChar;
            }
            if (!Directory.Exists(m_outDirectory))
            {
                Directory.CreateDirectory(m_outDirectory);
            }
            initializeProject();
            transformBook();

            m_Xuk_FilePath = Path.Combine(m_outDirectory, Path.GetFileName(m_Book_FilePath) + ".xuk");
            m_Project.SaveXuk(new Uri(m_Xuk_FilePath));
        }

        public DaisyToXuk(string bookfile)
            : this(bookfile, Path.GetDirectoryName(bookfile)) //Directory.GetParent(bookfile).FullName)
        { }

        private void initializeProject()
        {
            m_Project = new Project();
            m_Project.SetPrettyFormat(true);

            Presentation presentation = m_Project.AddNewPresentation();

            presentation.RootUri = new Uri(m_outDirectory);
            presentation.MediaDataManager.EnforceSinglePCMFormat = true;

            m_textChannel = presentation.ChannelFactory.CreateTextChannel();
            m_textChannel.Name = "Our Text Channel";

            m_audioChannel = presentation.ChannelFactory.CreateAudioChannel();
            m_audioChannel.Name = "Our Audio Channel";

            m_ImageChannel = presentation.ChannelFactory.CreateImageChannel();
            m_ImageChannel.Name = "Our Image Channel";

            /*string dataPath = presentation.DataProviderManager.DataFileDirectoryFullPath;
           if (Directory.Exists(dataPath))
           {
               Directory.Delete(dataPath, true);
           }*/
        }

        private void transformBook()
        {
            //FileInfo DTBFilePathInfo = new FileInfo(m_Book_FilePath);
            //switch (DTBFilePathInfo.Extension)

            int indexOfDot = m_Book_FilePath.LastIndexOf('.');
            if (indexOfDot < 0 || indexOfDot == m_Book_FilePath.Length - 1)
            {
                return;
            }

            string fileExt = m_Book_FilePath.Substring(indexOfDot);
            switch (fileExt)
            {
                case ".opf":
                    {
                        XmlDocument opfXmlDoc = readXmlDocument(m_Book_FilePath);
                        parseOpf(opfXmlDoc);
                        break;
                    }
                case ".xml":
                    {
                        XmlDocument contentXmlDoc = readXmlDocument(m_Book_FilePath);
                        parseMetadata(contentXmlDoc);
                        parseContentDocument(contentXmlDoc, null, m_Book_FilePath);
                        break;
                    }
                case ".epub":
                    {
                        unzipEPubAndParseOpf();
                        break;
                    }
                default:
                    break;
            }

            if (!String.IsNullOrEmpty(m_PublicationUniqueIdentifier))
            {
                Metadata meta = addMetadata("dc:Identifier", m_PublicationUniqueIdentifier, m_PublicationUniqueIdentifierNode);
            }
        }

        private core.TreeNode getTreeNodeWithXmlElementId(string id)
        {
            Presentation pres = m_Project.Presentations.Get(0);
            return getTreeNodeWithXmlElementId(pres.RootNode, id);
        }

        private static core.TreeNode getTreeNodeWithXmlElementId(core.TreeNode node, string id)
        {
            if (node.GetXmlElementId() == id) return node;

            for (int i = 0; i < node.Children.Count; i++)
            {
                core.TreeNode child = getTreeNodeWithXmlElementId(node.Children.Get(i), id);
                if (child != null)
                {
                    return child;
                }
            }
            return null;
        }

        private static XmlNode getFirstChildElementsWithName(XmlNode root, bool deep, string localName, string namespaceUri)
        {
            foreach (XmlNode node in getChildrenElementsWithName(root, deep, localName, namespaceUri, true))
            {
                return node;
            }
            return null;
        }

        private static IEnumerable<XmlNode> getChildrenElementsWithName(XmlNode root, bool deep, string localName, string namespaceUri, bool breakOnFirstFound)
        {
            if (root.NodeType == XmlNodeType.Document)
            {
                XmlNode element = null;
                XmlDocument doc = (XmlDocument) root;
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

                foreach (XmlNode childNode in getChildrenElementsWithName(element, deep, localName, namespaceUri, breakOnFirstFound))
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
                    foreach (XmlNode childNode in getChildrenElementsWithName(node, deep, localName, namespaceUri, breakOnFirstFound))
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
    }
}
