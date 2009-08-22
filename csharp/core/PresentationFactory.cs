using urakawa.xuk;

namespace urakawa
{
    /// <summary>
    /// Factory creating <see cref="Presentation"/>s
    /// </summary>
    public class PresentationFactory : GenericXukAbleFactory<Presentation>
    {
        public override string GetTypeNameFormatted()
        {
            return XukStrings.PresentationFactory;
        }

        /// <summary>
        /// Creates a <see cref="Presentation"/> of default type (that is <see cref="Presentation"/>
        /// </summary>
        /// <returns>The created <see cref="Presentation"/></returns>
        public virtual Presentation Create(Project proj)
        {
            Presentation pres = Create<Presentation>();
            pres.Project = proj;

            if (pres.IsPrettyFormat())
            {
                pres.WarmUpAllFactories();
            }

            return pres;
        }

    }
}