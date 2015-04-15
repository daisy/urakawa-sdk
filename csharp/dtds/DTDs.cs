using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace DTDs
{
    public static class DTDs
    {
        public const string DTBOOK_2005_3_MATHML = "DTDs.Resources.dtbook-2005-3_MathML.dtd";
        public const string HTML5 = "DTDs.Resources.html5.dtd";

        public static readonly Dictionary<string, string> ENTITIES_MAPPING;

        static DTDs()
        {
            ENTITIES_MAPPING = new Dictionary<String, String>();

            // HTML5 (we use a basic DTD to emulate some validation behaviour, ultimately we should use the XHTML5 schema/relaxng from EPUB3)

            ENTITIES_MAPPING.Add("html5", HTML5);

            // NCX

            //http://www.daisy.org/z3986/2005/ncx-2005-1.dtd
            ENTITIES_MAPPING.Add("ncx-2005-1.dtd", "DTDs.Resources.ncx-2005-1.dtd");
            ENTITIES_MAPPING.Add("//NISO//DTD ncx 2005-1//EN", "DTDs.Resources.ncx-2005-1.dtd");

            // DTBOOK

            //http://www.loc.gov/nls/z3986/v100/dtbook110.dtd
            ENTITIES_MAPPING.Add("dtbook110.dtd", "DTDs.Resources.dtbook110.dtd");
            ENTITIES_MAPPING.Add("//NISO//DTD dtbook v1.1.0//EN", "DTDs.Resources.dtbook110.dtd");

            //http://www.daisy.org/z3986/2005/dtbook-2005-1.dtd
            ENTITIES_MAPPING.Add("dtbook-2005.dtd", "DTDs.Resources.dtbook-2005-1.dtd");
            ENTITIES_MAPPING.Add("dtbook-2005-1.dtd", "DTDs.Resources.dtbook-2005-1.dtd");
            ENTITIES_MAPPING.Add("//NISO//DTD dtbook 2005-1//EN", "DTDs.Resources.dtbook-2005-1.dtd");

            //http://www.daisy.org/z3986/2005/dtbook-2005-2.dtd
            ENTITIES_MAPPING.Add("dtbook-2005-2.dtd", "DTDs.Resources.dtbook-2005-2.dtd");
            ENTITIES_MAPPING.Add("//NISO//DTD dtbook 2005-2//EN", "DTDs.Resources.dtbook-2005-2.dtd");

            //http://www.daisy.org/z3986/2005/dtbook-2005-3.dtd
            ENTITIES_MAPPING.Add("dtbook-2005-3.dtd", "DTDs.Resources.dtbook-2005-3.dtd");
            ENTITIES_MAPPING.Add("//NISO//DTD dtbook 2005-3//EN", "DTDs.Resources.dtbook-2005-3.dtd");

            // SMIL

            //http://www.w3.org/TR/REC-SMIL/SMIL10.dtd
            ENTITIES_MAPPING.Add("SMIL10.dtd", "DTDs.Resources.SMIL10.dtd");
            ENTITIES_MAPPING.Add("//W3C//DTD SMIL 1.0//EN", "DTDs.Resources.SMIL10.dtd");
			
            //http://www.loc.gov/nls/z3986/v100/dtbsmil110.dtd
            ENTITIES_MAPPING.Add("dtbsmil110.dtd", "DTDs.Resources.dtbsmil110.dtd");
            ENTITIES_MAPPING.Add("//NISO//DTD dtbsmil v1.1.0//EN", "DTDs.Resources.dtbsmil110.dtd");

            //http://www.daisy.org/z3986/2005/dtbsmil-2005-1.dtd
            ENTITIES_MAPPING.Add("dtbsmil-2005-1.dtd", "DTDs.Resources.dtbsmil-2005-1.dtd");
            ENTITIES_MAPPING.Add("//NISO//DTD dtbsmil 2005-1//EN", "DTDs.Resources.dtbsmil-2005-1.dtd");

            //http://www.daisy.org/z3986/2005/dtbsmil-2005-2.dtd
            ENTITIES_MAPPING.Add("dtbsmil-2005-2.dtd", "DTDs.Resources.dtbsmil-2005-2.dtd");
            ENTITIES_MAPPING.Add("//NISO//DTD dtbsmil 2005-2//EN", "DTDs.Resources.dtbsmil-2005-2.dtd");

            // OPF

            ENTITIES_MAPPING.Add("//ISBN 0-9673008-1-9//DTD OEB 1.0.1 Package//EN", "DTDs.Resources.oebpkg101.dtd");

            //http://openebook.org/dtds/oeb-1.2/oebpkg12.dtd
            ENTITIES_MAPPING.Add("oebpkg12.dtd", "DTDs.Resources.oebpkg12.dtd");
            ENTITIES_MAPPING.Add("//ISBN 0-9673008-1-9//DTD OEB 1.2 Package//EN", "DTDs.Resources.oebpkg12.dtd");

            ENTITIES_MAPPING.Add("oeb1.ent", "DTDs.Resources.oeb1.ent"); 
            ENTITIES_MAPPING.Add("//ISBN 0-9673008-1-9//DTD OEB 1.0.1 Entities//EN", "DTDs.Resources.oeb1.ent");
            
            ENTITIES_MAPPING.Add("oeb12.ent", "DTDs.Resources.oeb12.ent"); 
            ENTITIES_MAPPING.Add("//ISBN 0-9673008-1-9//DTD OEB 1.2 Entities//EN", "DTDs.Resources.oeb12.ent");


            // XHTML

            ENTITIES_MAPPING.Add("xhtml1-transitional.dtd", "DTDs.Resources.xhtml1-transitional.dtd");
            ENTITIES_MAPPING.Add("//W3C//DTD XHTML 1.0 Transitional//EN", "DTDs.Resources.xhtml1-transitional.dtd");

            ENTITIES_MAPPING.Add("xhtml1-strict.dtd", "DTDs.Resources.xhtml1-strict.dtd");
            ENTITIES_MAPPING.Add("//W3C//DTD XHTML 1.0 Strict//EN", "DTDs.Resources.xhtml1-strict.dtd");
            
            ENTITIES_MAPPING.Add("xhtml11.dtd", "DTDs.Resources.xhtml11.dtd");
            ENTITIES_MAPPING.Add("//W3C//DTD XHTML 1.1//EN", "DTDs.Resources.xhtml11.dtd");

            ENTITIES_MAPPING.Add("xhtml-math-svg-flat.dtd", "DTDs.Resources.xhtml-math-svg-flat.dtd");
            ENTITIES_MAPPING.Add("//W3C//DTD XHTML 1.1 plus MathML 2.0 plus SVG 1.1//EN", "DTDs.Resources.xhtml-math-svg-flat.dtd");

            ENTITIES_MAPPING.Add("mathml2.dtd", "DTDs.Resources.mathml2.dtd");
            ENTITIES_MAPPING.Add("//W3C//DTD MathML 2.0//EN", "DTDs.Resources.mathml2.dtd");

            ENTITIES_MAPPING.Add("xhtml-lat1.ent", "DTDs.Resources.xhtml-lat1.ent");
            ENTITIES_MAPPING.Add("HTMLlat1", "DTDs.Resources.xhtml-lat1.ent");
            ENTITIES_MAPPING.Add("//W3C//ENTITIES Latin 1 for XHTML//EN", "DTDs.Resources.xhtml-lat1.ent");

            ENTITIES_MAPPING.Add("xhtml-symbol.ent", "DTDs.Resources.xhtml-symbol.ent");
            ENTITIES_MAPPING.Add("HTMLsymbol", "DTDs.Resources.xhtml-symbol.ent");
            ENTITIES_MAPPING.Add("//W3C//ENTITIES Symbols for XHTML//EN", "DTDs.Resources.xhtml-symbol.ent");

            ENTITIES_MAPPING.Add("xhtml-special.ent", "DTDs.Resources.xhtml-special.ent");
            ENTITIES_MAPPING.Add("HTMLspecial", "DTDs.Resources.xhtml-special.ent");
            ENTITIES_MAPPING.Add("//W3C//ENTITIES Special for XHTML//EN", "DTDs.Resources.xhtml-special.ent");

            ENTITIES_MAPPING.Add("mathml2-qname-1.mod", "DTDs.Resources.mathml2-qname-1.mod");
            ENTITIES_MAPPING.Add("//W3C//ENTITIES MathML 2.0 Qualified Names 1.0//EN", "DTDs.Resources.mathml2-qname-1.mod");



            ENTITIES_MAPPING.Add("xhtml-struct-1.mod", "DTDs.Resources.xhtml-struct-1.mod");
            ENTITIES_MAPPING.Add("//W3C//ELEMENTS XHTML Document Structure 1.0//EN", "DTDs.Resources.xhtml-struct-1.mod");

            ENTITIES_MAPPING.Add("xhtml-form-1.mod", "DTDs.Resources.xhtml-form-1.mod");
            ENTITIES_MAPPING.Add("//W3C//ELEMENTS XHTML Forms 1.0//EN", "DTDs.Resources.xhtml-form-1.mod");

            ENTITIES_MAPPING.Add("xhtml-table-1.mod", "DTDs.Resources.xhtml-table-1.mod");
            ENTITIES_MAPPING.Add("//W3C//ELEMENTS XHTML Tables 1.0//EN", "DTDs.Resources.xhtml-table-1.mod");

            ENTITIES_MAPPING.Add("xhtml-object-1.mod", "DTDs.Resources.xhtml-object-1.mod");
            ENTITIES_MAPPING.Add("//W3C//ELEMENTS XHTML Embedded Object 1.0//EN", "DTDs.Resources.xhtml-object-1.mod");

            ENTITIES_MAPPING.Add("xhtml-param-1.mod", "DTDs.Resources.xhtml-param-1.mod");
            ENTITIES_MAPPING.Add("//W3C//ELEMENTS XHTML Param Element 1.0//EN", "DTDs.Resources.xhtml-param-1.mod");

            ENTITIES_MAPPING.Add("xhtml-ssismap-1.mod", "DTDs.Resources.xhtml-ssismap-1.mod");
            ENTITIES_MAPPING.Add("//W3C//ELEMENTS XHTML Server-side Image Maps 1.0//EN", "DTDs.Resources.xhtml-ssismap-1.mod");

            ENTITIES_MAPPING.Add("xhtml-csismap-1.mod", "DTDs.Resources.xhtml-csismap-1.mod");
            ENTITIES_MAPPING.Add("//W3C//ELEMENTS XHTML Client-side Image Maps 1.0//EN", "DTDs.Resources.xhtml-csismap-1.mod");

            ENTITIES_MAPPING.Add("xhtml-image-1.mod", "DTDs.Resources.xhtml-image-1.mod");
            ENTITIES_MAPPING.Add("//W3C//ELEMENTS XHTML Images 1.0//EN", "DTDs.Resources.xhtml-image-1.mod");

            ENTITIES_MAPPING.Add("xhtml-style-1.mod", "DTDs.Resources.xhtml-style-1.mod");
            ENTITIES_MAPPING.Add("//W3C//DTD XHTML Style Sheets 1.0//EN", "DTDs.Resources.xhtml-style-1.mod");

            ENTITIES_MAPPING.Add("xhtml-script-1.mod", "DTDs.Resources.xhtml-script-1.mod");
            ENTITIES_MAPPING.Add("//W3C//ELEMENTS XHTML Scripting 1.0//EN", "DTDs.Resources.xhtml-script-1.mod");

            ENTITIES_MAPPING.Add("xhtml-base-1.mod", "DTDs.Resources.xhtml-base-1.mod");
            ENTITIES_MAPPING.Add("//W3C//ELEMENTS XHTML Base Element 1.0//EN", "DTDs.Resources.xhtml-base-1.mod");

            ENTITIES_MAPPING.Add("xhtml-meta-1.mod", "DTDs.Resources.xhtml-meta-1.mod");
            ENTITIES_MAPPING.Add("//W3C//ELEMENTS XHTML Metainformation 1.0//EN", "DTDs.Resources.xhtml-meta-1.mod");

            ENTITIES_MAPPING.Add("xhtml-link-1.mod", "DTDs.Resources.xhtml-link-1.mod");
            ENTITIES_MAPPING.Add("//W3C//ELEMENTS XHTML Link Element 1.0//EN", "DTDs.Resources.xhtml-link-1.mod");

            ENTITIES_MAPPING.Add("xhtml-blkpres-1.mod", "DTDs.Resources.xhtml-blkpres-1.mod");
            ENTITIES_MAPPING.Add("//W3C//ELEMENTS XHTML Block Presentation 1.0//EN", "DTDs.Resources.xhtml-blkpres-1.mod");

            ENTITIES_MAPPING.Add("xhtml-inlpres-1.mod", "DTDs.Resources.xhtml-inlpres-1.mod");
            ENTITIES_MAPPING.Add("//W3C//ELEMENTS XHTML Inline Presentation 1.0//EN", "DTDs.Resources.xhtml-inlpres-1.mod");

            ENTITIES_MAPPING.Add("xhtml-pres-1.mod", "DTDs.Resources.xhtml-pres-1.mod");
            ENTITIES_MAPPING.Add("//W3C//ELEMENTS XHTML Presentation 1.0//EN", "DTDs.Resources.xhtml-pres-1.mod");

            ENTITIES_MAPPING.Add("xhtml-ruby-1.mod", "DTDs.Resources.xhtml-ruby-1.mod");
            ENTITIES_MAPPING.Add("//W3C//ELEMENTS XHTML Ruby 1.0//EN", "DTDs.Resources.xhtml-ruby-1.mod");

            ENTITIES_MAPPING.Add("xhtml-bdo-1.mod", "DTDs.Resources.xhtml-bdo-1.mod");
            ENTITIES_MAPPING.Add("//W3C//ELEMENTS XHTML BDO Element 1.0//EN", "DTDs.Resources.xhtml-bdo-1.mod");

            ENTITIES_MAPPING.Add("xhtml-edit-1.mod", "DTDs.Resources.xhtml-edit-1.mod");
            ENTITIES_MAPPING.Add("//W3C//ELEMENTS XHTML Editing Markup 1.0//EN", "DTDs.Resources.xhtml-edit-1.mod");

            ENTITIES_MAPPING.Add("xhtml-list-1.mod", "DTDs.Resources.xhtml-list-1.mod");
            ENTITIES_MAPPING.Add("//W3C//ELEMENTS XHTML Lists 1.0//EN", "DTDs.Resources.xhtml-list-1.mod");

            ENTITIES_MAPPING.Add("xhtml-hypertext-1.mod", "DTDs.Resources.xhtml-hypertext-1.mod");
            ENTITIES_MAPPING.Add("//W3C//ELEMENTS XHTML Hypertext 1.0//EN", "DTDs.Resources.xhtml-hypertext-1.mod");

            ENTITIES_MAPPING.Add("xhtml-blkphras-1.mod", "DTDs.Resources.xhtml-blkphras-1.mod");
            ENTITIES_MAPPING.Add("//W3C//ELEMENTS XHTML Block Phrasal 1.0//EN", "DTDs.Resources.xhtml-blkphras-1.mod");

            ENTITIES_MAPPING.Add("xhtml-blkstruct-1.mod", "DTDs.Resources.xhtml-blkstruct-1.mod");
            ENTITIES_MAPPING.Add("//W3C//ELEMENTS XHTML Block Structural 1.0//EN", "DTDs.Resources.xhtml-blkstruct-1.mod");

            ENTITIES_MAPPING.Add("xhtml-inlphras-1.mod", "DTDs.Resources.xhtml-inlphras-1.mod");
            ENTITIES_MAPPING.Add("//W3C//ELEMENTS XHTML Inline Phrasal 1.0//EN", "DTDs.Resources.xhtml-inlphras-1.mod");

            ENTITIES_MAPPING.Add("xhtml-inlstruct-1.mod", "DTDs.Resources.xhtml-inlstruct-1.mod");
            ENTITIES_MAPPING.Add("//W3C//ELEMENTS XHTML Inline Structural 1.0//EN", "DTDs.Resources.xhtml-inlstruct-1.mod");

            ENTITIES_MAPPING.Add("xhtml-text-1.mod", "DTDs.Resources.xhtml-text-1.mod");
            ENTITIES_MAPPING.Add("//W3C//ELEMENTS XHTML Text 1.0//EN", "DTDs.Resources.xhtml-text-1.mod");

            ENTITIES_MAPPING.Add("xhtml-charent-1.mod", "DTDs.Resources.xhtml-charent-1.mod");
            ENTITIES_MAPPING.Add("//W3C//ENTITIES XHTML Character Entities 1.0//EN", "DTDs.Resources.xhtml-charent-1.mod");

            ENTITIES_MAPPING.Add("xhtml11-model-1.mod", "DTDs.Resources.xhtml11-model-1.mod");
            ENTITIES_MAPPING.Add("//W3C//ENTITIES XHTML 1.1 Document Model 1.0//EN", "DTDs.Resources.xhtml11-model-1.mod");

            ENTITIES_MAPPING.Add("xhtml-attribs-1.mod", "DTDs.Resources.xhtml-attribs-1.mod");
            ENTITIES_MAPPING.Add("//W3C//ENTITIES XHTML Common Attributes 1.0//EN", "DTDs.Resources.xhtml-attribs-1.mod");

            ENTITIES_MAPPING.Add("xhtml-events-1.mod", "DTDs.Resources.xhtml-events-1.mod");
            ENTITIES_MAPPING.Add("//W3C//ENTITIES XHTML Intrinsic Events 1.0//EN", "DTDs.Resources.xhtml-events-1.mod");

            ENTITIES_MAPPING.Add("xhtml-qname-1.mod", "DTDs.Resources.xhtml-qname-1.mod");
            ENTITIES_MAPPING.Add("//W3C//ENTITIES XHTML Qualified Names 1.0//EN", "DTDs.Resources.xhtml-qname-1.mod");

            ENTITIES_MAPPING.Add("xhtml-datatypes-1.mod", "DTDs.Resources.xhtml-datatypes-1.mod");
            ENTITIES_MAPPING.Add("//W3C//ENTITIES XHTML Datatypes 1.0//EN", "DTDs.Resources.xhtml-datatypes-1.mod");

            ENTITIES_MAPPING.Add("xhtml-framework-1.mod", "DTDs.Resources.xhtml-framework-1.mod");
            ENTITIES_MAPPING.Add("//W3C//ENTITIES XHTML Modular Framework 1.0//EN", "DTDs.Resources.xhtml-framework-1.mod");

            ENTITIES_MAPPING.Add("xhtml-inlstyle-1.mod", "DTDs.Resources.xhtml-inlstyle-1.mod");
            ENTITIES_MAPPING.Add("//W3C//ENTITIES XHTML Inline Style 1.0//EN", "DTDs.Resources.xhtml-inlstyle-1.mod");


            ENTITIES_MAPPING.Add("isonum.ent", "DTDs.Resources.isonum.ent");
            ENTITIES_MAPPING.Add("isopub.ent", "DTDs.Resources.isopub.ent");
            ENTITIES_MAPPING.Add("mmlalias.ent", "DTDs.Resources.mmlalias.ent");
            ENTITIES_MAPPING.Add("mmlextra.ent", "DTDs.Resources.mmlextra.ent");
            ENTITIES_MAPPING.Add("isocyr1.ent", "DTDs.Resources.isocyr1.ent");
            ENTITIES_MAPPING.Add("isocyr2.ent", "DTDs.Resources.isocyr2.ent");
            ENTITIES_MAPPING.Add("isodia.ent", "DTDs.Resources.isodia.ent");
            ENTITIES_MAPPING.Add("isolat1.ent", "DTDs.Resources.isolat1.ent");
            ENTITIES_MAPPING.Add("isolat2.ent", "DTDs.Resources.isolat2.ent");
            ENTITIES_MAPPING.Add("isobox.ent", "DTDs.Resources.isobox.ent");
            ENTITIES_MAPPING.Add("isomfrk.ent", "DTDs.Resources.isomfrk.ent");
            ENTITIES_MAPPING.Add("isomopf.ent", "DTDs.Resources.isomopf.ent");
            ENTITIES_MAPPING.Add("isomscr.ent", "DTDs.Resources.isomscr.ent");
            ENTITIES_MAPPING.Add("isotech.ent", "DTDs.Resources.isotech.ent");
            ENTITIES_MAPPING.Add("isogrk3.ent", "DTDs.Resources.isogrk3.ent");
            ENTITIES_MAPPING.Add("isoamsr.ent", "DTDs.Resources.isoamsr.ent");
            ENTITIES_MAPPING.Add("isoamsb.ent", "DTDs.Resources.isoamsb.ent");
            ENTITIES_MAPPING.Add("isoamsc.ent", "DTDs.Resources.isoamsc.ent");
            ENTITIES_MAPPING.Add("isoamsn.ent", "DTDs.Resources.isoamsn.ent");
            ENTITIES_MAPPING.Add("isoamso.ent", "DTDs.Resources.isoamso.ent");
            ENTITIES_MAPPING.Add("isoamsa.ent", "DTDs.Resources.isoamsa.ent");

#if DEBUG
            Dictionary<string, string>.ValueCollection values = ENTITIES_MAPPING.Values;
            List<string> alreadyTested = new List<string>(values.Count);
            foreach (String resource in values)
            {
                if (alreadyTested.Contains(resource))
                {
                    continue;
                }

                alreadyTested.Add(resource);

                Stream dtdStream = Fetch(resource);
                if (dtdStream == null)
                {
                    Debugger.Break();
                }
                else
                {
                    dtdStream.Close();
                }
            }
#endif //DEBUG
        }

        private static readonly Assembly m_Assembly = Assembly.GetExecutingAssembly();

        public static Stream Fetch(string resourcePath)
        {
            //string[] names = m_Assembly.GetManifestResourceNames();

            return m_Assembly.GetManifestResourceStream(resourcePath);
        }
    }
}
