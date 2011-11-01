using System;
using System.Collections.Generic;
using System.Text;
using urakawa.metadata.daisy;

namespace urakawa.daisy
{
    public static class DiagramContentModelStrings
    {
        public const string NS_PREFIX_XML = "xml";
        public const string NS_URL_XML = "http://www.w3.org/XML/1998/namespace";

        public const string NS_PREFIX_DIAGRAM = "d";
        public const string NS_URL_DIAGRAM = "http://www.daisy.org/ns/z3986/authoring/features/description/";

        public const string NS_PREFIX_ZAI = "z";
        public const string NS_URL_ZAI = "http://www.daisy.org/ns/z3986/authoring/";

        public const string NS_PREFIX_XLINK = "xlink";
        public const string NS_URL_XLINK = "http://www.w3.org/1999/xlink";

        public static string StripNSPrefix(string str)
        {
            //str.Split(':')[1]

            int index = str.IndexOf(':');
            if (index == -1) return str;
            return str.Substring(index + 1);
        }

        // XUK token to define the DIAGRAM XML element name of a particular description instance
        // (represented by AltContent, and stored as a metadata item)
        // Metadata name for AltContent (description instance)

        public const string DescriptionName = "description-name";

        // DIAGRAM XML elements (in 'd'-prefixed namespace http://www.daisy.org/ns/z3986/authoring/features/description/ )
        // Metadata value for AltContent (description instance), with name="description-name"

        public const string D_Summary = NS_PREFIX_DIAGRAM + ":summary";
        public const string D_LondDesc = NS_PREFIX_DIAGRAM + ":longDesc";
        public const string D_Tactile = NS_PREFIX_DIAGRAM + ":tactile";
        public const string D_Tour = NS_PREFIX_DIAGRAM + ":tour";
        public const string D_SimplifiedImage = NS_PREFIX_DIAGRAM + ":simplifiedImage";
        public const string D_SimplifiedLanguageDescription = NS_PREFIX_DIAGRAM + ":simplifiedLanguageDescription";

        public const string Annotation = "annotation"; // well, this one isn't in the "d" namespace, but is used equally

        public const string D_Description = NS_PREFIX_DIAGRAM + ":description";
        public const string D_Body = NS_PREFIX_DIAGRAM + ":body";
        public const string D_Head = NS_PREFIX_DIAGRAM + ":head";

        // DIAGRAM XML elements (in unprefixed namespace http://www.daisy.org/ns/z3986/authoring/ )

        public const string Object = "object";
        public const string Block = "block";
        public const string P = "p";

        public const string Meta = "meta";

        // Generic XML attributes
        // ALL:  Metadata name for AltContent (description instance)

        public const string XmlId = NS_PREFIX_XML + ":id";
        public const string XmlLang = NS_PREFIX_XML + ":lang";

        // DIAGRAM XML attributes (in unprefixed or 'd'-prefixed namespaces)
        // ALL: Metadata name for AltContent (description instance)

        public const string Ref = "ref";
        public const string By = "by";
        public const string Role = "role";
        public const string Src = "src";
        public const string SrcType = "srctype";
        public const string Age = "age";

        // Metadata XML attributes
        // ALL: Metadata optional attribute name

        public const string Rel = "rel";
        public const string Resource = "resource";
        public const string About = "about";

        public const string Property = "property";
        public const string Content = "content";

        // Special Urakawa SDK "Not Acknowledged" entity, for metadata with empty name/content pair, with only optional attributes
        // Metadata name and value (both)

        public const string NA = "N/A";
        public const string NA_NoSlash = "NA";

        // DIAGRAM "diagram"-prefixed metadata qualifiers

        public const string DIAGRAM_Purpose = "diagram:purpose"; // Metadata name
        public const string DIAGRAM_TargetAge = "diagram:targetAge"; // Metadata name
        public const string DIAGRAM_targetGrade = "diagram:targetGrade"; // Metadata name
        public const string DIAGRAM_DescriptionQuality = "diagram:descriptionQuality"; // Metadata name
        public const string DIAGRAM_Credentials = "diagram:credentials"; // Metadata name
        public const string DIAGRAM_QueryConcept = "diagram:queryConcept"; // Metadata name

        public const string DIAGRAM_CurrentVersion = "diagram:currentVersion"; // Metadata optional attribute value (for name="rel")
        public const string DIAGRAM_ThisVersion = "diagram:thisVersion"; // Metadata optional attribute value (for name="rel")
        public const string DIAGRAM_PreviousVersion = "diagram:previousVersion"; // Metadata optional attribute value (for name="rel")
        public const string DIAGRAM_AlternateVersion = "diagram:alternateVersion"; // Metadata optional attribute value (for name="rel")
        public const string DIAGRAM_Repository = "diagram:repository"; // Metadata optional attribute value (for name="rel")

        // DIAGRAM "Z3986"-prefixed metadata qualifiers

        public const string Z3986_Name = "z3986:name"; // Metadata name
        public const string Z3986_Version = "z3986:version"; // Metadata name

        public const string Z3986_Profile = "z3986:profile"; // Metadata optional attribute value (for name="rel")

        // Dublin Core "DC"-prefixed metadata qualifiers
        // ALL: Metadata name

        public const string DC_AccessRights = "dc:accessRights";

        // already in SupportedMetadata_Z39862005, but with upper case first letter
        //"dc:identifier"
        //"dc:language"
        //"dc:creator"
        //"dc:rights"
        //"dc:description"

        public const string XLINK_Href = NS_PREFIX_XLINK + ":href";


        private static List<string> m_MetadataValues = null;
        public static List<string> MetadataValues_ForDescriptionName
        {
            get
            {
                if (m_MetadataValues == null)
                {
                    m_MetadataValues = new List<string>();

                    m_MetadataValues.Add(D_Summary);
                    m_MetadataValues.Add(D_LondDesc);
                    m_MetadataValues.Add(D_Tactile);
                    //m_MetadataValues.Add(D_Tour); At the moment, this is a simple metadata attribute
                    m_MetadataValues.Add(D_SimplifiedImage);
                    m_MetadataValues.Add(D_SimplifiedLanguageDescription);

                    m_MetadataValues.Add(Annotation);
                }
                return m_MetadataValues;
            }
        }

        private static List<string> m_MetadataNames_ForAltContentDescriptionInstance = null;
        public static List<string> MetadataNames_ForAltContentDescriptionInstance
        {
            get
            {
                if (m_MetadataNames_ForAltContentDescriptionInstance == null)
                {
                    m_MetadataNames_ForAltContentDescriptionInstance = new List<string>();

                    m_MetadataNames_ForAltContentDescriptionInstance.Add(DescriptionName);

                    m_MetadataNames_ForAltContentDescriptionInstance.Add(XmlId);
                    m_MetadataNames_ForAltContentDescriptionInstance.Add(XmlLang);

                    m_MetadataNames_ForAltContentDescriptionInstance.Add(Ref);
                    m_MetadataNames_ForAltContentDescriptionInstance.Add(By);
                    m_MetadataNames_ForAltContentDescriptionInstance.Add(Role);
                    m_MetadataNames_ForAltContentDescriptionInstance.Add(Src);
                    m_MetadataNames_ForAltContentDescriptionInstance.Add(SrcType);
                    m_MetadataNames_ForAltContentDescriptionInstance.Add(Age);

                    m_MetadataNames_ForAltContentDescriptionInstance.Add(D_Tour);
                    m_MetadataNames_ForAltContentDescriptionInstance.Add(XLINK_Href);
                }
                return m_MetadataNames_ForAltContentDescriptionInstance;
            }
        }

        private static List<string> m_MetadataNames_Generic = null;
        public static List<string> MetadataNames_Generic
        {
            get
            {
                if (m_MetadataNames_Generic == null)
                {
                    m_MetadataNames_Generic = new List<string>();

                    m_MetadataNames_Generic.Add(XmlId);
                    m_MetadataNames_Generic.Add(XmlLang);

                    m_MetadataNames_Generic.Add(NA);
                    m_MetadataNames_Generic.Add(NA_NoSlash);

                    m_MetadataNames_Generic.Add(Rel);
                    m_MetadataNames_Generic.Add(Resource);
                    m_MetadataNames_Generic.Add(About);

                    m_MetadataNames_Generic.Add(DIAGRAM_Purpose);
                    m_MetadataNames_Generic.Add(DIAGRAM_TargetAge);
                    m_MetadataNames_Generic.Add(DIAGRAM_targetGrade);
                    m_MetadataNames_Generic.Add(DIAGRAM_DescriptionQuality);
                    m_MetadataNames_Generic.Add(DIAGRAM_Credentials);
                    m_MetadataNames_Generic.Add(DIAGRAM_QueryConcept);

                    m_MetadataNames_Generic.Add(DIAGRAM_CurrentVersion);
                    m_MetadataNames_Generic.Add(DIAGRAM_ThisVersion);
                    m_MetadataNames_Generic.Add(DIAGRAM_PreviousVersion);
                    m_MetadataNames_Generic.Add(DIAGRAM_AlternateVersion);
                    m_MetadataNames_Generic.Add(DIAGRAM_Repository);

                    m_MetadataNames_Generic.Add(Z3986_Name);
                    m_MetadataNames_Generic.Add(Z3986_Version);
                    m_MetadataNames_Generic.Add(Z3986_Profile);

                    m_MetadataNames_Generic.Add(DC_AccessRights);

                    foreach (var def in SupportedMetadata_Z39862005.DefinitionSet.Definitions)
                    {
                        m_MetadataNames_Generic.Add(def.Name.ToLower());
                        if (def.Synonyms != null)
                        {
                            foreach (var syn in def.Synonyms)
                            {
                                m_MetadataNames_Generic.Add(syn.ToLower());
                            }
                        }
                    }
                }
                return m_MetadataNames_Generic;
            }
        }
    }
}
