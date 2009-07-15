using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using NUnit.Framework;

namespace urakawa
{
    public static class IXukAbleBasicTestUtils
    {
        public delegate T XukQNameCreatorDelegate<T>(string ln, string ns);

        public static void XukInOut_RoundTrip<T>(T o, XukQNameCreatorDelegate<T> creatorDelegate, Presentation pres)
            where T : WithPresentation
        {
            StringBuilder sb = new StringBuilder();
            XmlWriter wr = XmlWriter.Create(sb);
            o.XukOut(wr, pres.RootUri, null);
            wr.Close();
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.ConformanceLevel = ConformanceLevel.Fragment;
            XmlReader rd = XmlReader.Create(new StringReader(sb.ToString()), settings, pres.RootUri.ToString());
            while (rd.Read())
            {
                if (rd.NodeType == XmlNodeType.Element) break;
            }
            Assert.AreEqual(rd.NodeType, XmlNodeType.Element, "Could not find an element in XukOut output xml");
            T reloaded = creatorDelegate(rd.LocalName, rd.NamespaceURI);
            Assert.IsNotNull(
                reloaded,
                "The factory could not create a {2} matching QName {1}:{0}",
                rd.LocalName, rd.NamespaceURI, typeof (T).Name);
            Assert.AreEqual(o.GetType(), reloaded.GetType(), "The reloaded {0} from Xuk had a different Type",
                            typeof (T).Name);
            reloaded.XukIn(rd, null);
            Assert.IsTrue(o.ValueEquals(reloaded), "The reloaded {0} was not value equal to the original",
                          typeof (T).Name);
        }
    }
}