using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using System.Reflection;

namespace XukToZed
{
    /// <summary>
    /// This class helps resolve references inside the assembly
    /// We need it because the XSLT files are embedded resources and they reference each other.
    /// The logic for this code was copied from http://msmvps.com/blogs/jayharlow/archive/2005/01/24/33766.aspx
    /// </summary>
    public class XslResolver : XmlResolver
    {
        private Type mType;

        public XslResolver(Type type)
        {
            mType = type;
        }

        public override System.Net.ICredentials Credentials
        {
            set { throw new Exception("The method or operation is not implemented."); }
        }

        public override object GetEntity(Uri absoluteUri, string role, Type ofObjectToReturn)
        {
            if (File.Exists(absoluteUri.AbsolutePath))
            {
                return new FileStream(absoluteUri.AbsolutePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            }
            else
            {
                string name = Path.GetFileName(absoluteUri.AbsolutePath);
                System.Globalization.CultureInfo culture = System.Globalization.CultureInfo.CurrentCulture;
                Stream stream = null;
                //Try the specific culture
                stream = GetManifestResourceStream(culture, name);
                //Try the neutral culture
                if (stream == null && !culture.IsNeutralCulture) stream = GetManifestResourceStream(culture.Parent, name);
                //Try the default culture
                if (stream == null) stream = mType.Assembly.GetManifestResourceStream("XukToZed." + name);
                if (stream == null) throw new FileNotFoundException("File not found: " + name);
                return stream;
            }
        }

        public override Uri ResolveUri(Uri baseUri, string relativeUri)
        {
            if (baseUri == null) baseUri = new Uri(mType.Assembly.Location, true);
            return base.ResolveUri(baseUri, relativeUri);
        }
        private Stream GetManifestResourceStream(System.Globalization.CultureInfo culture,string name)
        {
            try
            {
                Assembly satellite = mType.Assembly.GetSatelliteAssembly(culture);
                return satellite.GetManifestResourceStream(mType, name);
            }
            catch (Exception e)
            {
                return null;
            }

        }
    }
}
