using urakawa.xuk;

namespace urakawa.command
{
    /// <summary>
    /// Factory for creating <see cref="Command"/>s
    /// </summary>
    public class CommandFactory : GenericWithPresentationFactory<Command>
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        protected internal CommandFactory()
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