namespace urakawa.command
{
    /// <summary>
    /// Factory for creating <see cref="Command"/>s
    /// </summary>
    public abstract class CommandFactoryBase : GenericWithPresentationFactory<Command>
    {
        public override string GetTypeNameFormatted()
        {
            return "CommandFactoryBase";
        }

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