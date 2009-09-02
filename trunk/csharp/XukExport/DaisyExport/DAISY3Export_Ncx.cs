using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;

namespace DaisyExport
    {
    public partial class DAISY3Export
        {
        //DAISY3Export_Ncx


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
