namespace urakawa
{
    /// <summary>
    /// Factory creating <see cref="Presentation"/>s
    /// </summary>
    public class PresentationFactory : GenericXukAbleFactory<Presentation>
    {

        /// <summary>
        /// Creates a <see cref="Presentation"/> of default type (that is <see cref="Presentation"/>
        /// </summary>
        /// <returns>The created <see cref="Presentation"/></returns>
        public virtual Presentation Create()
        {
            return Create<Presentation>();
        }

    }
}