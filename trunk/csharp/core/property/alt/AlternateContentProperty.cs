using System;
using System.Collections.Generic;
using System.Text;

using urakawa.xuk;

namespace urakawa.property.alt
{
    public class AlternateContentProperty:Property
    {
        public override string GetTypeNameFormatted()
        {
            return XukStrings.AlternateContentProperty;
        }

        private AlternateContents m_AlternateContents;

        public AlternateContentProperty()
        {
            m_AlternateContents = null;
        }
        
        public void SetAlternateContents(AlternateContents contents)
        {
            if (contents == null) throw new exception.MethodParameterIsNullException ("Contents instance is null");

            m_AlternateContents = contents;
        }


    }
}
