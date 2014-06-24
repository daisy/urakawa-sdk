using System;
using System.Collections.Generic;
using System.Text;
using urakawa.exception;

namespace urakawa.xuk
{
    public sealed class UglyPrettyName
    {
        public readonly string Ugly;
        public readonly string Pretty;

        public UglyPrettyName(string ugly, string pretty)
        {
            Ugly = ugly;
            Pretty = pretty;
        }

        public string z(bool pretty)
        {
            return pretty ? Pretty : Ugly;
        }

        public bool Match(string str)
        {
            return str == Ugly || str == Pretty;
        }
    }

    public class QualifiedName
    {
        public readonly UglyPrettyName LocalName;

        public readonly string NamespaceUri;

        public QualifiedName(UglyPrettyName ln, string nsuri)
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
