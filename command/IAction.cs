using System;
using urakawa.events.command;

namespace urakawa.command
{
    /// <summary>
    /// Interface for a generic action, that can be executed and has descriptions
    /// </summary>
    public interface IAction
    {
        /// <summary>
        /// Gets a <c>bool</c> indicating if the <see cref="IAction"/> can execute
        /// </summary>
        /// <returns>The <c>bool</c></returns>
        bool CanExecute { get; }

        /// <summary>
        /// Execute the action.
        /// </summary>
        void Execute();

        /// <summary>
        /// Get a long uman-readable description of the command
        /// </summary>
        string LongDescription { get; }

        /// <summary>
        /// Gets a short humanly readable description of the command
        /// </summary>
        string ShortDescription { get; }
    }
}