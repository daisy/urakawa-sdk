using urakawa.xuk;

namespace urakawa.property.alt
{
    [XukNameUglyPrettyAttribute("acFct", "AlternateContentFactory")]
    public sealed class AlternateContentFactory : GenericWithPresentationFactory<AlternateContent>
    {
        public AlternateContentFactory(Presentation pres)
            : base(pres)
        {
        }

        protected override void InitializeInstance(AlternateContent instance)
        {
            base.InitializeInstance(instance);
        }

        public AlternateContent CreateAlternateContent()
        {
            return Create<AlternateContent>();
        }
    }
}
