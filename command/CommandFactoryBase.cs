namespace urakawa.command
{
    public abstract class CommandFactoryBase : GenericWithPresentationFactory<Command>
    {
        protected CommandFactoryBase(Presentation pres) : base(pres)
        {
        }

        /// <summary>
        /// Creates a <see cref="CompositeCommand"/>
        /// </summary>
        /// <returns>The created composite command</returns>
        public CompositeCommand CreateCompositeCommand()
        {
            return Create<CompositeCommand>();
        }
    }
}