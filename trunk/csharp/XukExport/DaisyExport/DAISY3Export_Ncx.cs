using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;

using urakawa.xuk;

namespace DaisyExport
    {
    public partial class DAISY3Export
        {
        //DAISY3Export_Ncx

        private void CreateNcxAndSmilDocuments ()
            {
            XmlDocument ncxDocument = CreateStub_NcxDocument ();

            XmlNode ncxRootNode = ncxDocument.GetElementsByTagName ( "ncx" )[0];
            XmlNode navMapNode = ncxDocument.GetElementsByTagName ( "navMap" )[0];
            Dictionary<urakawa.core.TreeNode, XmlNode> treeNode_NavNodeMap = new Dictionary<urakawa.core.TreeNode, XmlNode> ();
            uint playOrder = 0;
            int totalPageCount = 0;
            int maxNormalPageNumber = 0;
            int maxDepth = 1;
            

            foreach (urakawa.core.TreeNode urakawaNode in m_ListOfLevels)
                {
                bool IsNcxNativeNodeAdded = false;
                XmlDocument smilDocument = null;
                string smilFileName = GetNextSmilFileName;
                XmlNode navPointNode = null;
                urakawa.core.TreeNode currentHeadingTreeNode = null;
                

                urakawaNode.AcceptDepthFirst (
            delegate ( urakawa.core.TreeNode n )
                {
                QualifiedName currentQName = n.GetXmlElementQName ();
                if (currentQName != null &&
                    (currentQName.LocalName == "h1" || currentQName.LocalName == "h2" || currentQName.LocalName == "h3" || currentQName.LocalName == "h4"
                    || currentQName.LocalName == "h5" || currentQName.LocalName == "h6"))
                    {
                    currentHeadingTreeNode = n;
                    }

                urakawa.media.AbstractTextMedia txtMedia = n.GetTextMedia ();
                urakawa.media.ExternalAudioMedia externalAudio = GetExternalAudioMedia ( n );

                if (txtMedia != null && externalAudio != null)
                    {
                    if (!IsNcxNativeNodeAdded)
                        {
                        // in first pass, create smil stub document
                        smilDocument = CreateStub_SmilDocument ();
                        }

                    // create smil nodes
                    string par_id = null;
                    if (smilDocument != null)
                        {
                        XmlNode mainSeq = smilDocument.GetElementsByTagName ( "body" )[0].FirstChild;
                        XmlNode parNode = smilDocument.CreateElement ( null, "par", mainSeq.NamespaceURI );
                        par_id = GetNextID ( ID_SmilPrefix );
                        CommonFunctions.CreateAppendXmlAttribute ( smilDocument, parNode, "id", par_id );
                        mainSeq.AppendChild ( parNode );

                        XmlNode textNode = smilDocument.CreateElement ( null, "text", mainSeq.NamespaceURI );
                        CommonFunctions.CreateAppendXmlAttribute ( smilDocument, textNode, "id", GetNextID ( ID_SmilPrefix ) );
                        string dtbookID = m_TreeNode_XmlNodeMap[n].Attributes.GetNamedItem ( "id" ).Value;
                        CommonFunctions.CreateAppendXmlAttribute ( smilDocument, textNode, "src", m_ContentFileName + "#" + dtbookID );
                        parNode.AppendChild ( textNode );

                        XmlNode audioNode = smilDocument.CreateElement ( null, "audio", mainSeq.NamespaceURI );
                        CommonFunctions.CreateAppendXmlAttribute ( smilDocument, audioNode, "clipBegin", externalAudio.ClipBegin.TimeAsTimeSpan.ToString () );
                        CommonFunctions.CreateAppendXmlAttribute ( smilDocument, audioNode, "clipEnd", externalAudio.ClipEnd.TimeAsTimeSpan.ToString () );
                        CommonFunctions.CreateAppendXmlAttribute ( smilDocument, audioNode, "src", Path.GetFileName ( externalAudio.Src ) );
                        parNode.AppendChild ( audioNode );
                        }// smilDocumeent null check ends

                    // if node n is pagenum, add to pageList
                    if (currentQName != null
                        && currentQName.LocalName == "pagenum")
                        {
                        XmlNodeList listOfPages = ncxDocument.GetElementsByTagName ( "pageList" );
                        XmlNode pageListNode = null;
                        if (listOfPages == null || listOfPages.Count == 0)
                            {
                            pageListNode = ncxDocument.CreateElement ( null, "pageList", ncxRootNode.NamespaceURI );
                            ncxRootNode.AppendChild ( pageListNode );
                            }
                        else
                            {
                            pageListNode = listOfPages[0];
                            }

                        
                        XmlNode pageTargetNode = ncxDocument.CreateElement ( null, "pageTarget", pageListNode.NamespaceURI );
                        pageListNode.AppendChild ( pageTargetNode );

                        CommonFunctions.CreateAppendXmlAttribute ( ncxDocument, pageTargetNode, "playOrder", (++playOrder).ToString () );
                        string strTypeVal = n.GetXmlProperty ().GetAttribute ( "page" ).Value;
                        CommonFunctions.CreateAppendXmlAttribute ( ncxDocument, pageTargetNode, "type", strTypeVal );
                        string strPageValue = n.GetTextMediaFlattened ();
                        ++totalPageCount;

                        if (strTypeVal == "normal")
                            {
                            int tmp = int.Parse( strPageValue );
                            if (maxNormalPageNumber < tmp) maxNormalPageNumber = tmp;
                            }
                        if (strTypeVal != "special")
                            {
                            CommonFunctions.CreateAppendXmlAttribute ( ncxDocument, pageTargetNode, "value", strPageValue );
                            }

                        XmlNode navLabelNode = ncxDocument.CreateElement ( null, "navLabel", pageListNode.NamespaceURI );
                        pageTargetNode.AppendChild ( navLabelNode );

                        XmlNode txtNode = ncxDocument.CreateElement ( null, "text", pageListNode.NamespaceURI );
                        navLabelNode.AppendChild ( txtNode );
                        txtNode.AppendChild (
                            ncxDocument.CreateTextNode ( strPageValue ) );

                        XmlNode audioNode = ncxDocument.CreateElement ( null, "audio", pageListNode.NamespaceURI );
                        navLabelNode.AppendChild ( audioNode );
                        CommonFunctions.CreateAppendXmlAttribute ( ncxDocument, audioNode, "clipBegin", externalAudio.ClipBegin.TimeAsTimeSpan.ToString () );
                        CommonFunctions.CreateAppendXmlAttribute ( ncxDocument, audioNode, "clipEnd", externalAudio.ClipEnd.TimeAsTimeSpan.ToString () );
CommonFunctions.CreateAppendXmlAttribute ( ncxDocument, audioNode, "src", Path.GetFileName ( externalAudio.Src ) );

XmlNode contentNode = ncxDocument.CreateElement ( null, "content", pageListNode.NamespaceURI );
pageTargetNode.AppendChild ( contentNode );
CommonFunctions.CreateAppendXmlAttribute ( ncxDocument, contentNode, "src", smilFileName + "#" + m_TreeNode_XmlNodeMap[n].Attributes.GetNamedItem ( "id" ).Value );
                        }


                    if (!IsNcxNativeNodeAdded)
                        {
                        if (currentHeadingTreeNode != null &&
                            (currentHeadingTreeNode == n || currentHeadingTreeNode.IsAncestorOf ( n )))
                            {
                            // check and create doctitle and docauthor nodes
                            QualifiedName qName = urakawaNode.GetXmlElementQName ();
                            if (qName != null &&
                                (qName.LocalName == "doctitle" || qName.LocalName == "docauthor"))
                                {
                                XmlNode docNode = ncxDocument.CreateElement ( null,
                                    qName.LocalName == "doctitle" ? "docTitle" : "docAuthor",
                                     ncxRootNode.NamespaceURI );

                                ncxRootNode.InsertBefore ( docNode, navMapNode );

                                XmlNode docTxtNode = ncxDocument.CreateElement ( null, "text", docNode.NamespaceURI );
                                docNode.AppendChild ( docTxtNode );
                                docTxtNode.AppendChild (
                                ncxDocument.CreateTextNode ( txtMedia.Text ) );

                                // create audio node
                                XmlNode docAudioNode = ncxDocument.CreateElement ( null, "audio", docNode.NamespaceURI );
                                docNode.AppendChild ( docAudioNode );
                                CommonFunctions.CreateAppendXmlAttribute ( ncxDocument, docAudioNode, "clipBegin", externalAudio.ClipBegin.TimeAsTimeSpan.ToString () );
                                CommonFunctions.CreateAppendXmlAttribute ( ncxDocument, docAudioNode, "clipEnd", externalAudio.ClipEnd.TimeAsTimeSpan.ToString () );
                                CommonFunctions.CreateAppendXmlAttribute ( ncxDocument, docAudioNode, "src", Path.GetFileName ( externalAudio.Src ) );
                                }
                            else
                                {
                                // first create navPoints
                                navPointNode = ncxDocument.CreateElement ( null, "navPoint", navMapNode.NamespaceURI );
                                CommonFunctions.CreateAppendXmlAttribute ( ncxDocument, navPointNode, "playOrder", (++playOrder).ToString () );

                                urakawa.core.TreeNode parentNode = GetParentLevelNode ( urakawaNode );

                                if (parentNode == null)
                                    {
                                    navMapNode.AppendChild ( navPointNode );
                                    }
                                else
                                    {
                                    treeNode_NavNodeMap[parentNode].AppendChild ( navPointNode );
                                                                        }

                                                                    int navPointDepth = GetDepthOfNavPointNode ( ncxDocument, navPointNode );
                                                                    if (maxDepth < navPointDepth) maxDepth = navPointDepth;
                                                                    
                                treeNode_NavNodeMap.Add ( urakawaNode, navPointNode );
                                CommonFunctions.CreateAppendXmlAttribute ( ncxDocument, navPointNode, "class", currentHeadingTreeNode.GetProperty<urakawa.property.xml.XmlProperty> ().LocalName );

                                // create navLabel
                                XmlNode navLabel = ncxDocument.CreateElement ( null, "navLabel", navPointNode.NamespaceURI );
                                navPointNode.AppendChild ( navLabel );
                                
                                // create text node
                                XmlNode txtNode = ncxDocument.CreateElement ( null, "text", navMapNode.NamespaceURI );
                                navLabel.AppendChild ( txtNode );
                                txtNode.AppendChild (
                                ncxDocument.CreateTextNode ( currentHeadingTreeNode.GetTextMediaFlattened () ) );

                                // create audio node
                                XmlNode audioNode = ncxDocument.CreateElement ( null, "audio", navMapNode.NamespaceURI );
                                navLabel.AppendChild ( audioNode );
                                CommonFunctions.CreateAppendXmlAttribute ( ncxDocument, audioNode, "clipBegin", externalAudio.ClipBegin.TimeAsTimeSpan.ToString () );
                                CommonFunctions.CreateAppendXmlAttribute ( ncxDocument, audioNode, "clipEnd", externalAudio.ClipEnd.TimeAsTimeSpan.ToString () );
                                CommonFunctions.CreateAppendXmlAttribute ( ncxDocument, audioNode, "src", Path.GetFileName ( externalAudio.Src ) );

                                // add content node
                                if (par_id != null)
                                    {
                                    XmlNode contentNode = ncxDocument.CreateElement ( null, "content", navMapNode.NamespaceURI );
                                    navPointNode.AppendChild ( contentNode );
                                    CommonFunctions.CreateAppendXmlAttribute ( ncxDocument, contentNode, "src", smilFileName + "#" + par_id );
                                    }
                                }
                            IsNcxNativeNodeAdded = true;
                            }
                        }

                    }
                return true;
                },
                    delegate ( urakawa.core.TreeNode n ) { } );

                
                if (smilDocument != null)
                    {
                    // yet to add ellapsed time and dur.
                    AddMetadata_Smil ( smilDocument );

                    CommonFunctions.WriteXmlDocumentToFile ( smilDocument,
                        Path.Combine ( m_OutputDirectory, smilFileName ) );
                    }
                }
            
            AddMetadata_Ncx ( ncxDocument, totalPageCount.ToString (), maxNormalPageNumber.ToString (), maxDepth.ToString ()  );
            CommonFunctions.WriteXmlDocumentToFile ( ncxDocument,
                Path.Combine ( m_OutputDirectory, "TObi.ncx" ) );
            }

        private urakawa.core.TreeNode GetParentLevelNode ( urakawa.core.TreeNode node )
            {
            urakawa.core.TreeNode parentNode = node.Parent;
            QualifiedName qName = parentNode.GetXmlElementQName ();

            while (qName == null ||
                (qName != null && !qName.LocalName.StartsWith ( "level" )))
                {
                if (qName != null && qName.LocalName == "book")
                    return null;

                parentNode = parentNode.Parent;
                qName = parentNode.GetXmlElementQName ();
                }
            return parentNode;
            }

        private int GetDepthOfNavPointNode ( XmlDocument doc, XmlNode navPointNode )
            {
            XmlNode parent = navPointNode.ParentNode;
            int i = 1;
            for (i = 1; i <= 9; i++)
                {

                if (parent != null && parent.LocalName == "navMap")
                    {
                    return i;
                    }
                parent = navPointNode.ParentNode;
                }
            return i ;
            }

        private void AddMetadata_Ncx ( XmlDocument ncxDocument, string strTotalPages, string strMaxNormalPage, string strDepth )
            {
            XmlNode headNode = ncxDocument.GetElementsByTagName ( "head" )[0];

            urakawa.metadata.Metadata m = m_Presentation.GetMetadata ("dc:identifier")[0] ;
                    AddMetadataAsAttributes ( ncxDocument, headNode, "dtb:uid" , m.NameContentAttribute.Value );

                    AddMetadataAsAttributes ( ncxDocument, headNode, "dtb:depth", strDepth);
                    AddMetadataAsAttributes ( ncxDocument, headNode, "dtb:totalPageCount", strTotalPages );
                    AddMetadataAsAttributes ( ncxDocument, headNode, "dtb:maxPageNumber", strMaxNormalPage );
            }

        private void AddMetadata_Smil ( XmlDocument smilDocument )
            {
            XmlNode headNode = smilDocument.GetElementsByTagName ( "head" )[0];

            urakawa.metadata.Metadata m = m_Presentation.GetMetadata ( "dc:identifier" )[0];
            AddMetadataAsAttributes ( smilDocument, headNode, "dtb:uid", m.NameContentAttribute.Value );
            }

        private void AddMetadataAsAttributes ( XmlDocument doc, XmlNode headNode, string name, string content )
            {
            XmlNode metaNode = doc.CreateElement ( null, "meta", headNode.NamespaceURI );
            headNode.AppendChild ( metaNode );
            CommonFunctions.CreateAppendXmlAttribute ( doc, metaNode, "name", name );
            CommonFunctions.CreateAppendXmlAttribute ( doc, metaNode, "content", content);
            }

        
        private XmlDocument CreateStub_NcxDocument ()
            {
            XmlDocument NcxDocument = new XmlDocument ();
            NcxDocument.XmlResolver = null;

            NcxDocument.CreateXmlDeclaration ( "1.0", "utf-8", null );
            NcxDocument.AppendChild ( NcxDocument.CreateDocumentType ( "ncx",
                "-//NISO//DTD ncx 2005-1//EN",
                "http://www.daisy.org/z3986/2005/ncx-2005-1.dtd",
                null ) );

            XmlNode rootNode = NcxDocument.CreateElement ( null,
                "ncx",
                "http://www.daisy.org/z3986/2005/ncx/" );

            NcxDocument.AppendChild ( rootNode );


            CommonFunctions.CreateAppendXmlAttribute ( NcxDocument, rootNode, "version", "2005-1" );
            CommonFunctions.CreateAppendXmlAttribute ( NcxDocument, rootNode, "xml:lang", "en" );


            XmlNode headNode = NcxDocument.CreateElement ( null, "head", rootNode.NamespaceURI );
            rootNode.AppendChild ( headNode );

            XmlNode navMapNode = NcxDocument.CreateElement ( null, "navMap", rootNode.NamespaceURI );
            rootNode.AppendChild ( navMapNode );

            return NcxDocument;
            }



        }
    }
