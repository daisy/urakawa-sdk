using System;
using System.Collections.Generic;
using System.Text;

using urakawa.xuk;

namespace urakawa.property.alt
{
    public sealed class AlternateContentFactory: GenericWithPresentationFactory<AlternateContent>
    {
        public override string GetTypeNameFormatted()
        {
            return XukStrings.AlternateContentFactory;
        }

        public AlternateContentFactory(Presentation pres)
            : base(pres)
        {
        }

        public AlternateContent Create (string description)
        {
            
            AlternateContent altContent = Create<AlternateContent> ();
            altContent.Description = description;
            return altContent;
        }

        public AlternateContent Create()
        {

            return Create<AlternateContent>();
            
        }
    }
}
