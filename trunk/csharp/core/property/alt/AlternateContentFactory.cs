using urakawa.xuk;

namespace urakawa.property.alt
{
    public sealed class AlternateContentFactory : GenericWithPresentationFactory<AlternateContent>
    {
        public override string GetTypeNameFormatted()
        {
            return XukStrings.AlternateContentFactory;
        }

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
