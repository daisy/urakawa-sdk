using urakawa.xuk;

namespace urakawa.command
{
    /// <summary>
    /// Factory for creating <see cref="Command"/>s
    /// </summary>
    public class CommandFactory : GenericWithPresentationFactory<Command>
    {
        public override string GetTypeNameFormatted()
        {
            return XukStrings.CommandFactory;
        }
        public CommandFactory(Presentation pres) : base(pres)
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