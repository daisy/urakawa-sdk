using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;

namespace DaisyExport
    {
    partial class DAISY3Export
        {

        private void CreateOpfDocument ()
            {
            XmlDocument opfDocument = CreateStub_OpfDocument ();

            XmlNode manifestNode = opfDocument.GetElementsByTagName ( "manifest" )[0];
            const string mediaType_Smil = "application/smil";
            const string mediaType_Wav = "audio/x-wav";
            const string mediaType_Mpg = "audio/mpeg";
            const string mediaType_Opf = "text/xml";
            const string mediaType_Ncx = "application/x-dtbncx+xml";

            XmlNode itemNode = opfDocument.CreateElement ( null, "item", manifestNode.NamespaceURI );
            manifestNode.AppendChild ( itemNode );
            CommonFunctions.CreateAppendXmlAttribute ( opfDocument, itemNode, "href", "TObi.ncx" );
            CommonFunctions.CreateAppendXmlAttribute ( opfDocument, itemNode, "id", GetNextID ( ID_OpfPrefix ) );
            CommonFunctions.CreateAppendXmlAttribute ( opfDocument, itemNode, "media-type", mediaType_Opf );

            itemNode = opfDocument.CreateElement ( null, "item", manifestNode.NamespaceURI );
            manifestNode.AppendChild ( itemNode );
            CommonFunctions.CreateAppendXmlAttribute ( opfDocument, itemNode, "href", "TObi.opf" );
            CommonFunctions.CreateAppendXmlAttribute ( opfDocument, itemNode, "id", GetNextID ( ID_OpfPrefix ) );
            CommonFunctions.CreateAppendXmlAttribute ( opfDocument, itemNode, "media-type", mediaType_Ncx );

            // add smil files to manifest
            List<string> smilIDListInPlayOrder = new List<string> ();

            foreach (string smilFileName in m_FilesList_Smil)
                {
                itemNode = opfDocument.CreateElement ( null, "item", manifestNode.NamespaceURI );
                manifestNode.AppendChild ( itemNode );
                CommonFunctions.CreateAppendXmlAttribute ( opfDocument, itemNode, "href", smilFileName );
                string strID = GetNextID ( ID_OpfPrefix );
                CommonFunctions.CreateAppendXmlAttribute ( opfDocument, itemNode, "id", strID );
                CommonFunctions.CreateAppendXmlAttribute ( opfDocument, itemNode, "media-type", mediaType_Smil );

                smilIDListInPlayOrder.Add ( strID );
                }

            foreach (string audioFileName in m_FilesList_Audio)
                {
                itemNode = opfDocument.CreateElement ( null, "item", manifestNode.NamespaceURI );
                manifestNode.AppendChild ( itemNode );
                CommonFunctions.CreateAppendXmlAttribute ( opfDocument, itemNode, "href", audioFileName );
                string strID = GetNextID ( ID_OpfPrefix );
                CommonFunctions.CreateAppendXmlAttribute ( opfDocument, itemNode, "id", strID );

                if (string.Compare ( Path.GetExtension ( audioFileName ), ".wav", true ) == 0)
                    {
                    CommonFunctions.CreateAppendXmlAttribute ( opfDocument, itemNode, "media-type", mediaType_Wav );
                    }
                else
                    {
                    CommonFunctions.CreateAppendXmlAttribute ( opfDocument, itemNode, "media-type", mediaType_Mpg );
                    }
                }


            // create spine
            XmlNode spineNode = opfDocument.GetElementsByTagName ( "spine" )[0];
            
            foreach (string strSmilID in smilIDListInPlayOrder)
                {
                XmlNode itemRefNode = opfDocument.CreateElement ( null, "itemref", spineNode.NamespaceURI );
                spineNode.AppendChild ( itemRefNode);
                CommonFunctions.CreateAppendXmlAttribute ( opfDocument, itemRefNode, "idref", strSmilID );
                }

            CommonFunctions.WriteXmlDocumentToFile ( opfDocument,
                Path.Combine ( m_OutputDirectory, "TObi.Opf") );
            }




        private XmlDocument CreateStub_OpfDocument ()
            {
            XmlDocument document = new XmlDocument ();
            document.XmlResolver = null;

            document.CreateXmlDeclaration ( "1.0", "utf-8", null );
            document.AppendChild ( document.CreateDocumentType ( "package",
                "+//ISBN 0-9673008-1-9//DTD OEB 1.2 Package//EN",
                "http://openebook.org/dtds/oeb-1.2/oebpkg12.dtd",
                null ) );

            XmlNode rootNode = document.CreateElement ( null,
                "package",
                "http://openebook.org/namespaces/oeb-package/1.0/" );

            document.AppendChild ( rootNode );


            CommonFunctions.CreateAppendXmlAttribute ( document, rootNode, "unique-identifier", "uid" );

            XmlNode metadataNode = document.CreateElement ( null, "metadata", rootNode.NamespaceURI );
            rootNode.AppendChild ( metadataNode );

            XmlNode dcMetadataNode = document.CreateElement ( null, "dc-metadata", rootNode.NamespaceURI );
            metadataNode.AppendChild ( dcMetadataNode );
            CommonFunctions.CreateAppendXmlAttribute ( document, dcMetadataNode, "xmlns:dc", "http://purl.org/dc/elements/1.1/" );
            CommonFunctions.CreateAppendXmlAttribute ( document, dcMetadataNode, "xmlns:oebpackage", "http://openebook.org/namespaces/oeb-package/1.0/" );

            XmlNode xMetadataNode = document.CreateElement ( null, "x-metadata", rootNode.NamespaceURI );
            metadataNode.AppendChild ( xMetadataNode );

            XmlNode manifestNode = document.CreateElement ( null, "manifest", rootNode.NamespaceURI );
            rootNode.AppendChild ( manifestNode );

            XmlNode spineNode = document.CreateElement ( null, "spine", rootNode.NamespaceURI );
            rootNode.AppendChild ( spineNode );


            return document;
            }

        }
    }
