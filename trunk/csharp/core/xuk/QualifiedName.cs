using System;
using System.Collections.Generic;
using System.Text;
using urakawa.exception;

namespace urakawa.xuk
{
    public class QualifiedName
    {
        public readonly XukAble.UglyPrettyName LocalName;

        public readonly string NamespaceUri;

        public QualifiedName(XukAble.UglyPrettyName ln, string nsuri)
        {
            if (ln == null)
            {
                throw new MethodParameterIsNullException("QualifiedName(XukAble.UglyPrettyName ln)");
            }

            LocalName = ln;
            NamespaceUri = nsuri;
        }

        public string GetFlatString(bool pretty)
        {
            if (string.IsNullOrEmpty(NamespaceUri))
            {
                return LocalName.z(pretty);
            }

            return String.Format("{0}:{1}", NamespaceUri, LocalName.z(pretty));
        }
    }
}
