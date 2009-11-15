using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DTDs
{
    public static class DTDs
    {
        private static Assembly m_Assembly = Assembly.GetExecutingAssembly();
        public static Stream Fetch(string resourcePath)
        {
            //string[] names = m_Assembly.GetManifestResourceNames();

            return m_Assembly.GetManifestResourceStream(resourcePath);
        }
    }
}
