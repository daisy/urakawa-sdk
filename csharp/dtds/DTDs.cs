using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace DTDs
{
    public static class DTDs
    {
        public static string DTBOOK_2005_3 = "DTDs.Resources.dtbook-2005-3.dtd";

        public static readonly Dictionary<string, string> ENTITIES_MAPPING;

        static DTDs()
        {
            ENTITIES_MAPPING = new Dictionary<String, String>();

            // NCX

            ENTITIES_MAPPING.Add("//NISO//DTD%20ncx%202005-1//EN", "DTDs.Resources.ncx-2005-1.dtd");

            // DTBOOK

            ENTITIES_MAPPING.Add("dtbook110.dtd", "DTDs.Resources.dtbook110.dtd");
            ENTITIES_MAPPING.Add("//NISO//DTD%20dtbook%202005-1//EN", "DTDs.Resources.dtbook-2005-1.dtd");
            ENTITIES_MAPPING.Add("//NISO//DTD%20dtbook%202005-2//EN", "DTDs.Resources.dtbook-2005-2.dtd");
            ENTITIES_MAPPING.Add("//NISO//DTD%20dtbook%202005-3//EN", DTBOOK_2005_3);

            // SMIL

            ENTITIES_MAPPING.Add("//NISO//DTD%20dtbsmil%202005-2//EN", "DTDs.Resources.dtbsmil-2005-2.dtd");
            ENTITIES_MAPPING.Add("//NISO//DTD%20dtbsmil%202005-1//EN", "DTDs.Resources.dtbsmil-2005-1.dtd");

            // OPF

            ENTITIES_MAPPING.Add("//ISBN%200-9673008-1-9//DTD%20OEB%201.0.1%20Package//EN", "DTDs.Resources.oebpkg101.dtd");
            ENTITIES_MAPPING.Add("//ISBN%200-9673008-1-9//DTD%20OEB%201.0.1%20Entities//EN", "DTDs.Resources.oeb1.ent");
            ENTITIES_MAPPING.Add("oeb1.ent", "DTDs.Resources.oeb1.ent");

            ENTITIES_MAPPING.Add("//ISBN%200-9673008-1-9//DTD%20OEB%201.2%20Package//EN", "DTDs.Resources.oebpkg12.dtd");
            ENTITIES_MAPPING.Add("//ISBN%200-9673008-1-9//DTD%20OEB%201.2%20Entities//EN", "DTDs.Resources.oeb12.ent");
            ENTITIES_MAPPING.Add("oeb12.ent", "DTDs.Resources.oeb12.ent");

            // XHTML

            // -//W3C//DTD XHTML 1.0 Transitional//EN
            // http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd

            ENTITIES_MAPPING.Add("xhtml-lat1.ent", "DTDs.Resources.xhtml-lat1.ent");
            ENTITIES_MAPPING.Add("xhtml-symbol.ent", "DTDs.Resources.xhtml-symbol.ent");
            ENTITIES_MAPPING.Add("xhtml-special.ent", "DTDs.Resources.xhtml-special.ent");

            ENTITIES_MAPPING.Add("HTMLlat1", "DTDs.Resources.xhtml-lat1.ent");
            ENTITIES_MAPPING.Add("HTMLsymbol", "DTDs.Resources.xhtml-symbol.ent");
            ENTITIES_MAPPING.Add("HTMLspecial", "DTDs.Resources.xhtml-special.ent");

            ENTITIES_MAPPING.Add("//W3C//ENTITIES%20Latin%201%20for%20XHTML//EN", "DTDs.Resources.xhtml-lat1.ent");
            ENTITIES_MAPPING.Add("//W3C//ENTITIES%20Symbols%20for%20XHTML//EN", "DTDs.Resources.xhtml-symbol.ent");
            ENTITIES_MAPPING.Add("//W3C//ENTITIES%20Special%20for%20XHTML//EN", "DTDs.Resources.xhtml-special.ent");

            ENTITIES_MAPPING.Add("//W3C//DTD%20XHTML%201.0%20Transitional//EN", "DTDs.Resources.xhtml1-transitional.dtd");
            ENTITIES_MAPPING.Add("//W3C//DTD%20XHTML%201.1//EN", "DTDs.Resources.xhtml11.dtd");
            
            ENTITIES_MAPPING.Add("//W3C//DTD%20XHTML%201.1%20plus%20MathML%202.0%20plus%20SVG%201.1//EN", "DTDs.Resources.xhtml-math-svg-flat.dtd");

            ENTITIES_MAPPING.Add("//W3C//ENTITIES%20MathML%202.0%20Qualified%20Names%201.0//EN", "DTDs.Resources.mathml2.dtd");
        }

        private static Assembly m_Assembly = Assembly.GetExecutingAssembly();

        public static Stream Fetch(string resourcePath)
        {
            //string[] names = m_Assembly.GetManifestResourceNames();

            return m_Assembly.GetManifestResourceStream(resourcePath);
        }
    }
}
