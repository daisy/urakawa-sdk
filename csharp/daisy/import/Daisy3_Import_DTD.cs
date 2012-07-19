using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml;
using AudioLib;
using urakawa.core;
using urakawa.xuk;
#if ENABLE_DTDSHARP
using DtdSharp;
#else
using Org.System.Xml.Sax;
using Constants = Org.System.Xml.Sax.Constants;
using AElfred;
using Kds.Xml.Expat;
#endif //ENABLE_DTDSHARP


namespace urakawa.daisy.import
{
    public partial class Daisy3_Import
    {
        public const string INTERNAL_DTD_NAME = "DTBookLocalDTD.dtd";

#if ENABLE_DTDSHARP
        private Dictionary<string, List<string>> m_listOfMixedContentXmlElementNames = new Dictionary<string, List<string>>();
#else
        private List<string> m_listOfMixedContentXmlElementNames = new List<string>();
#endif


        protected void parseContentDocument_DTD(bool isHTML, string book_FilePath, Project project, XmlDocument xmlDoc, TreeNode parentTreeNode, string filePath, out string dtdUniqueResourceId)
        {
            dtdUniqueResourceId = null;

            //xmlNode.OwnerDocument
            string dtdID = xmlDoc.DocumentType == null ? string.Empty
            : !string.IsNullOrEmpty(xmlDoc.DocumentType.SystemId) ? xmlDoc.DocumentType.SystemId
            : !string.IsNullOrEmpty(xmlDoc.DocumentType.PublicId) ? xmlDoc.DocumentType.PublicId
            : xmlDoc.DocumentType.Name;

            if (dtdID == @"html"
                && string.IsNullOrEmpty(xmlDoc.DocumentType.SystemId)
                && string.IsNullOrEmpty(xmlDoc.DocumentType.PublicId))
            {
                dtdID = @"html5";
            }

            if (dtdID.Contains(@"xhtml1")
                //systemId.Contains(@"xhtml11.dtd")
                //|| systemId.Contains(@"xhtml1-strict.dtd")
                //|| systemId.Contains(@"xhtml1-transitional.dtd")
                )
            {
                dtdID = @"http://www.w3.org/xhtml-math-svg-flat.dtd";
            }

            if (!string.IsNullOrEmpty(dtdID) && !dtdID.StartsWith(@"http://"))
            {
                dtdID = @"http://www.daisy.org/" + dtdID;
            }

            if (string.IsNullOrEmpty(dtdID))
            {
                return;
            }

#if ENABLE_DTDSHARP
                            Stream dtdStream = LocalXmlUrlResolver.mapUri(new Uri(dtdID, UriKind.Absolute), out dtdUniqueResourceId);

                            if (!string.IsNullOrEmpty(dtdUniqueResourceId))
                            {
                                DebugFix.Assert(dtdStream != null);

                                List<string> list;
                                m_listOfMixedContentXmlElementNames.TryGetValue(dtdUniqueResourceId, out list);

                                if (list == null)
                                {
                                    if (dtdStream != null)
                                    {
                                        list = new List<string>();
                                        m_listOfMixedContentXmlElementNames.Add(dtdUniqueResourceId, list);

                                        initMixedContentXmlElementNamesFromDTD(dtdUniqueResourceId, dtdStream);
                                    }
                                    else
                                    {
#if DEBUG
                                        Debugger.Break();
#endif
                                    }
                                }
                                else
                                {
                                    if (dtdStream != null)
                                    {
                                        dtdStream.Close();
                                    }
                                }
                            }
                            else
                            {
#if DEBUG
                                Debugger.Break();
#endif
                            }
#else

            //#if DEBUG
            //            try
            //            {
            //                string str1 = Org.System.Xml.Sax.Resources.GetString(Org.System.Xml.Sax.RsId.AttIndexOutOfBounds);
            //            }
            //            catch (Exception ex)
            //            {
            //                Debugger.Break();
            //            }

            //            try
            //            {
            //                string str4 = Kds.Xml.Sax.Constants.GetString(Kds.Xml.Sax.RsId.CannotResolveEntity);
            //            }
            //            catch (Exception ex)
            //            {
            //                Debugger.Break();
            //            }

            //            try
            //            {
            //                string str5 = Kds.Xml.Expat.Constants.GetString(Kds.Xml.Expat.RsId.AccessingBaseUri);
            //            }
            //            catch (Exception ex)
            //            {
            //                Debugger.Break();
            //            }

            //            try
            //            {
            //                string str3 = Kds.Text.Resources.GetString(Kds.Text.RsId.ArrayOutOfBounds);
            //            }
            //            catch (Exception ex)
            //            {
            //                Debugger.Break();
            //            }

            //            try
            //            {
            //                string str2 =
            //                    Org.System.Xml.Resources.GetString(Org.System.Xml.RsId.InternalNsError);
            //            }
            //            catch (Exception ex)
            //            {
            //                Debugger.Break();
            //            }
            //#endif



            IXmlReader reader = null;


            //string dll = @"SaxNET.dll";
            ////#if NET40
            ////                            dll = @"\SaxNET_NET4.dll";
            ////#endif
            //string appFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            //string dtdPath = Path.Combine(appFolder, dll);
            //Assembly assembly = Assembly.LoadFrom(dtdPath);
            //                            try
            //                            {
            //                                reader = SaxReaderFactory.CreateReader(assembly, null);
            //                            }
            //                            catch (Exception e)
            //                            {
            //#if DEBUG
            //                                Debugger.Break();
            //#endif
            //                            }



            //reader = new SaxDriver();
            reader = new ExpatReader();

            if (reader == null)
            {
                return;
            }
            //Type readerType = reader.GetType();

            reader.EntityResolver = new SaxEntityResolver();

            SaxErrorHandler errorHandler = new SaxErrorHandler();
            reader.ErrorHandler = errorHandler;


            if (reader is SaxDriver)
            {
                //"namespaces"
                try
                {
                    reader.SetFeature(Constants.NamespacesFeature, true);
                }
                catch (Exception e)
                {
#if DEBUG
                    Debugger.Break();
#endif
                }

                //"namespace-prefixes"
                try
                {
                    reader.SetFeature(Constants.NamespacePrefixesFeature, true);
                }
                catch (Exception e)
                {
#if DEBUG
                    Debugger.Break();
#endif
                }

                //"external-general-entities"
                try
                {
                    reader.SetFeature(Constants.ExternalGeneralFeature, true);
                }
                catch (Exception e)
                {
#if DEBUG
                    Debugger.Break();
#endif
                }

                //"external-parameter-entities"
                try
                {
                    reader.SetFeature(Constants.ExternalParameterFeature, true);
                }
                catch (Exception e)
                {
#if DEBUG
                    Debugger.Break();
#endif
                }

                //"xmlns-uris"
                try
                {
                    reader.SetFeature(Constants.XmlNsUrisFeature, true);
                }
                catch (Exception e)
                {
#if DEBUG
                    Debugger.Break();
#endif
                }

                //"resolve-dtd-uris"
                try
                {
                    reader.SetFeature(Constants.ResolveDtdUrisFeature, true);
                }
                catch (Exception e)
                {
#if DEBUG
                    Debugger.Break();
#endif
                }
            }


            if (reader is ExpatReader)
            {
                // http://xml.org/sax/features/namespaces
                try
                {
                    reader.SetFeature(Constants.NamespacesFeature, true);
                }
                catch (Exception e)
                {
#if DEBUG
                    Debugger.Break();
#endif
                }

                // http://xml.org/sax/features/external-general-entities
                try
                {
                    reader.SetFeature(Constants.ExternalGeneralFeature, true);
                }
                catch (Exception e)
                {
#if DEBUG
                    Debugger.Break();
#endif
                }

                // http://xml.org/sax/features/external-parameter-entities
                try
                {
                    reader.SetFeature(Constants.ExternalParameterFeature, true);
                }
                catch (Exception e)
                {
#if DEBUG
                    Debugger.Break();
#endif
                }

                // http://xml.org/sax/features/resolve-dtd-uris
                try
                {
                    reader.SetFeature(Constants.ResolveDtdUrisFeature, true);
                }
                catch (Exception e)
                {
#if DEBUG
                    Debugger.Break();
#endif
                }

                // http://xml.org/sax/features/lexical-handler/parameter-entities
                try
                {
                    reader.SetFeature(Constants.LexicalParameterFeature, true);
                }
                catch (Exception e)
                {
#if DEBUG
                    Debugger.Break();
#endif
                }

                if (false)
                {
                    try
                    {
                        reader.SetFeature("http://kd-soft.net/sax/features/skip-internal-entities",
                                          false);
                    }
                    catch (Exception e)
                    {
#if DEBUG
                        Debugger.Break();
#endif
                    }

                    try
                    {
                        reader.SetFeature(
                            "http://kd-soft.net/sax/features/parse-unless-standalone", true);
                    }
                    catch (Exception e)
                    {
#if DEBUG
                        Debugger.Break();
#endif
                    }

                    try
                    {
                        reader.SetFeature("http://kd-soft.net/sax/features/parameter-entities", true);
                    }
                    catch (Exception e)
                    {
#if DEBUG
                        Debugger.Break();
#endif
                    }

                    try
                    {
                        reader.SetFeature("http://kd-soft.net/sax/features/standalone-error", true);
                    }
                    catch (Exception e)
                    {
#if DEBUG
                        Debugger.Break();
#endif
                    }
                }

                // SUPPORTED, but then NOT SUPPORTED (deeper inside Expat C# wrapper code)

                //                                    // http://xml.org/sax/features/namespace-prefixes
                //                                    try
                //                                    {
                //                                        reader.SetFeature(Constants.NamespacePrefixesFeature, true);
                //                                    }
                //                                    catch (Exception e)
                //                                    {
                //#if DEBUG
                //                                        Debugger.Break();
                //#endif
                //                                    }

                //                                    // http://xml.org/sax/features/xmlns-uris
                //                                    try
                //                                    {
                //                                        reader.SetFeature(Constants.XmlNsUrisFeature, true);
                //                                    }
                //                                    catch (Exception e)
                //                                    {
                //#if DEBUG
                //                                        Debugger.Break();
                //#endif
                //                                    }
                //                                    // http://xml.org/sax/features/validation
                //                                    try
                //                                    {
                //                                        reader.SetFeature(Constants.ValidationFeature, true);
                //                                    }
                //                                    catch (Exception e)
                //                                    {
                //#if DEBUG
                //                                        Debugger.Break();
                //#endif
                //                                    }

                //                                    // http://xml.org/sax/features/unicode-normalization-checking
                //                                    try
                //                                    {
                //                                        reader.SetFeature(Constants.UnicodeNormCheckFeature, true);
                //                                    }
                //                                    catch (Exception e)
                //                                    {
                //#if DEBUG
                //                                        Debugger.Break();
                //#endif
                //                                    }


                // NOT SUPPORTED:


                // http://xml.org/sax/features/xml-1.1
                //                                    try
                //                                    {
                //                                        reader.SetFeature(Constants.Xml11Feature, true);
                //                                    }
                //                                    catch (Exception e)
                //                                    {
                //#if DEBUG
                //                                        Debugger.Break();
                //#endif
                //                                    }

                // http://xml.org/sax/features/xml-declaration
                //                                    try
                //                                    {
                //                                        reader.SetFeature(Constants.XmlDeclFeature, true);
                //                                    }
                //                                    catch (Exception e)
                //                                    {
                //#if DEBUG
                //                                        Debugger.Break();
                //#endif
                //                                    }

                // http://xml.org/sax/features/use-external-subset
                //                                    try
                //                                    {
                //                                        reader.SetFeature(Constants.UseExternalSubsetFeature, true);
                //                                    }
                //                                    catch (Exception e)
                //                                    {
                //#if DEBUG
                //                                        Debugger.Break();
                //#endif
                //                                    }

                // http://xml.org/sax/features/reader-control
                //                                    try
                //                                    {
                //                                        reader.SetFeature(Constants.ReaderControlFeature, true);
                //                                    }
                //                                    catch (Exception e)
                //                                    {
                //#if DEBUG
                //                                        Debugger.Break();
                //#endif
                //                                    }
            }

            SaxContentHandler handler = new SaxContentHandler(m_listOfMixedContentXmlElementNames);

            try
            {
                reader.DtdHandler = handler;
            }
            catch (Exception e)
            {
#if DEBUG
                Debugger.Break();
#endif
                errorHandler.AddMessage("Cannot set dtd handler: " + e.Message);
            }

            try
            {
                reader.ContentHandler = handler;
            }
            catch (Exception e)
            {
#if DEBUG
                Debugger.Break();
#endif
                errorHandler.AddMessage("Cannot set content handler: " + e.Message);
            }

            try
            {
                reader.LexicalHandler = handler;
            }
            catch (Exception e)
            {
#if DEBUG
                Debugger.Break();
#endif
                errorHandler.AddMessage("Cannot set lexical handler: " + e.Message);
            }

            try
            {
                reader.DeclHandler = handler;
            }
            catch (Exception e)
            {
#if DEBUG
                Debugger.Break();
#endif
                errorHandler.AddMessage("Cannot set declaration handler: " + e.Message);
            }

            string rootElementName = isHTML ? @"html" : @"dtbook";
            string dtdWrapper = "<?xml version=\"1.0\" encoding=\"UTF-8\"?><!DOCTYPE " + rootElementName + " SYSTEM \"" + dtdID + "\"><" + rootElementName + "></" + rootElementName + ">";
            //StringReader strReader = new StringReader(dtdWrapper);
            Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(dtdWrapper));
            TextReader txtReader = new StreamReader(stream, Encoding.UTF8);
            InputSource input = new InputSource<TextReader>(txtReader, dtdID + "/////SYSID");
            input.Encoding = "UTF-8";
            input.PublicId = "??";

            reader.Parse(input);

#endif //ENABLE_DTDSHARP
        }

        private string ExtractInternalDTD(XmlDocumentType docType)
        {
            if (docType == null) return null;

            string completeString = docType.OuterXml;
            if (completeString.IndexOf('[') >= 0 // completeString.Contains("[")
                &&
                completeString.IndexOf(']') >= 0) // completeString.Contains("]"))
            {
                string DTDString = completeString.Split('[')[1];
                DTDString = DTDString.Split(']')[0];

                if (!string.IsNullOrEmpty(DTDString))
                {
                    DTDString = DTDString.Replace("\r\n", "\n");
                    DTDString = DTDString.Replace("\n", "\r\n");
                    return DTDString;
                }
            }

            return null;
        }


#if ENABLE_DTDSHARP
        private void initMixedContentXmlElementNamesFromDTD(string dtdUniqueResourceId, Stream dtdStream)
        {
            List<string> list;
            m_listOfMixedContentXmlElementNames.TryGetValue(dtdUniqueResourceId, out list);

            DebugFix.Assert(list != null);

            if (list == null)
            {
                return;
            }

            DTD dtd = null;
            try
            {
                // NOTE: the Stream is automatically closed by the parser, see Scanner.ReadNextChar()
                DTDParser parser = new DTDParser(new StreamReader(dtdStream, Encoding.UTF8));
                dtd = parser.Parse(true);
            }
            catch (Exception ex)
            {
#if DEBUG
                Debugger.Break();
#endif
                dtdStream.Close();
            }

            if (dtd != null)
            {
                foreach (DictionaryEntry entry in dtd.Elements)
                {
                    DTDElement dtdElement = (DTDElement)entry.Value;
                    DTDItem item = dtdElement.Content;
                    if (isMixedContent(item))
                    {
                        if (!list.Contains(dtdElement.Name))
                        {
                            list.Add(dtdElement.Name);
                        }
                    }
                }


                foreach (DictionaryEntry entry in dtd.Entities)
                {
                    DTDEntity dtdEntity = (DTDEntity)entry.Value;

                    if (dtdEntity.ExternalId == null)
                    {
                        continue;
                    }

                    string system = dtdEntity.ExternalId.System;
                    if (dtdEntity.ExternalId is DTDPublic)
                    {
                        string pub = ((DTDPublic)dtdEntity.ExternalId).Pub;
                        if (!string.IsNullOrEmpty(pub))
                        {
                            system = pub; //.Replace(" ", "%20");
                        }
                    }

                    string normalisedUri = system.Replace("%20", " ").Replace(" //", "//").Replace("// ", "//");

                    foreach (String key in DTDs.DTDs.ENTITIES_MAPPING.Keys)
                    {
                        if (normalisedUri.Contains(key))
                        {
                            string subResource = DTDs.DTDs.ENTITIES_MAPPING[key];
                            Stream stream = DTDs.DTDs.Fetch(subResource);

                            if (stream != null)
                            {
                                initMixedContentXmlElementNamesFromDTD(dtdUniqueResourceId, stream);
                            }
                            else
                            {
#if DEBUG
                                Debugger.Break();
#endif
                            }

                            break;
                        }
                    }
                }
            }
        }

        private bool isMixedContent(DTDItem dtdItem)
        {
            if (dtdItem is DTDAny)
            {
                return false;
            }
            else if (dtdItem is DTDEmpty)
            {
                return false;
            }
            else if (dtdItem is DTDName)
            {
                return false;
            }
            else if (dtdItem is DTDChoice)
            {
                List<DTDItem> items = ((DTDChoice)dtdItem).Items;
                foreach (DTDItem item in items)
                {
                    bool b = isMixedContent(item);
                    if (b)
                    {
                        return true;
                    }
                }
            }
            else if (dtdItem is DTDSequence)
            {
                List<DTDItem> items = ((DTDSequence)dtdItem).Items;
                foreach (DTDItem item in items)
                {
                    bool b = isMixedContent(item);
                    if (b)
                    {
                        return true;
                    }
                }
            }
            else if (dtdItem is DTDMixed)
            {
                List<DTDItem> items = ((DTDMixed)dtdItem).Items;
                foreach (DTDItem item in items)
                {
                    bool b = isMixedContent(item);
                    if (b)
                    {
                        return true;
                    }
                }
            }
            else if (dtdItem is DTDPCData)
            {
                return true;
            }
            else
            {
#if DEBUG
                Debugger.Break();
#endif // DEBUG
            }

            return false;
        }
#endif //ENABLE_DTDSHARP
    }


#if !ENABLE_DTDSHARP

    class SaxContentHandler : IDtdHandler, IContentHandler, ILexicalHandler, IDeclHandler
    {
        private List<string> m_listOfMixedContentXmlElementNames;
        public SaxContentHandler(List<string> list)
        {
            m_listOfMixedContentXmlElementNames = list;
        }

        /* IDtdHandler */

        public void NotationDecl(string name, string publicId, string systemId)
        {
            bool debug = true;
        }

        public void UnparsedEntityDecl(string name, string publicId, string systemId, string notationName)
        {
            bool debug = true;
        }

        /* IContentHandler */

        public void SetDocumentLocator(ILocator locator)
        {
            bool debug = true;
        }

        public void StartDocument()
        {
            bool debug = true;
        }

        public void EndDocument()
        {
            bool debug = true;
        }

        public void StartPrefixMapping(string prefix, string uri)
        {
            bool debug = true;
        }

        public void EndPrefixMapping(string prefix)
        {
            bool debug = true;
        }

        public virtual void StartElement(
                   string uri, string localName, string qName, IAttributes atts)
        {
            bool debug = true;
        }

        public virtual void EndElement(string uri, string localName, string qName)
        {
            bool debug = true;
        }

        public void Characters(char[] ch, int start, int length)
        {
            bool debug = true;
        }

        public void IgnorableWhitespace(char[] ch, int start, int length)
        {
            bool debug = true;
        }

        public void ProcessingInstruction(string target, string data)
        {
            bool debug = true;
        }

        public void SkippedEntity(string name)
        {
            bool debug = true;
        }

        /* ILexicalhandler */

        public void StartDtd(string name, string publicId, string systemId)
        {
            bool debug = true;
        }

        public void EndDtd()
        {
            bool debug = true;
        }

        public void StartEntity(string name)
        {
            bool debug = true;
        }

        public void EndEntity(string name)
        {
            bool debug = true;
        }

        public void StartCData()
        {
            bool debug = true;
        }

        public void EndCData()
        {
            bool debug = true;
        }

        public void Comment(char[] ch, int start, int length)
        {
            bool debug = true;
        }

        /* IDeclHandler */

        public void ElementDecl(string name, string model)
        {
            if (model.Contains("#PCDATA") && !m_listOfMixedContentXmlElementNames.Contains(name))
            {
                m_listOfMixedContentXmlElementNames.Add(name);
            }
        }

        public void AttributeDecl(string eName, string aName, string aType,
                                  string mode, string aValue)
        {
            bool debug = true;
        }

        public void InternalEntityDecl(string name, string value)
        {
            bool debug = true;
        }

        public void ExternalEntityDecl(string name, string publicId, string systemId)
        {
            const string pubIdStr = "<!ENTITY {0} PUBLIC \"{1}\" SYSTEM \"{2}\">";
            const string sysIdStr = "<!ENTITY {0} SYSTEM \"{1}\">";
            string declStr;
            if (publicId != String.Empty)
                declStr = String.Format(pubIdStr, name, publicId, systemId);
            else
                declStr = String.Format(sysIdStr, name, systemId);
        }
    }

    class SaxErrorHandler : IErrorHandler
    {
        public void AddMessage(string msg)
        {
            Console.WriteLine(msg);
        }

        /* IErrorHandler */

        public void Warning(ParseError error)
        {
            string msg = "Warning: " + error.Message;
            if (error.BaseException != null)
                msg = msg + Environment.NewLine + error.BaseException.Message;

            Console.WriteLine(msg);
        }

        public void Error(ParseError error)
        {
            string msg = "Error: " + error.Message;
            if (error.BaseException != null)
                msg = msg + Environment.NewLine + error.BaseException.Message;

            Console.WriteLine(msg);
        }

        public void FatalError(ParseError error)
        {
            error.Throw();
        }
    }

    class SaxEntityResolver : IEntityResolver
    {
        public InputSource GetExternalSubset(string name, string baseUri)
        {
            return null;
        }

        public InputSource ResolveEntity(string name, string publicId, string baseUri, string systemId)
        {
            string dtdUniqueResourceId;
            Stream dtdStream = LocalXmlUrlResolver.mapUri(new Uri(systemId, UriKind.Absolute), out dtdUniqueResourceId);

            if (!string.IsNullOrEmpty(dtdUniqueResourceId))
            {
                DebugFix.Assert(dtdStream != null);

                TextReader txtReader = new StreamReader(dtdStream, Encoding.UTF8);
                return new InputSource<TextReader>(txtReader, systemId);

                //return new InputSource<Stream>(dtdStream, systemId);
            }

            return null;
        }
    }
#endif //ENABLE_DTDSHARP
}
