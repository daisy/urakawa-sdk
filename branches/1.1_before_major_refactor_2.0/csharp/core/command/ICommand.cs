using System;
using System.Collections.Generic;
using System.Text;
using urakawa.xuk;
using urakawa.events;
using urakawa.events.undo;
using ExecutedEventArgs=urakawa.events.command.ExecutedEventArgs;
using UnExecutedEventArgs=urakawa.events.command.UnExecutedEventArgs;

namespace urakawa.command
{
    /// <summary>
    /// Classes realizing this interface must store the state of the object(s) affected by the command
    /// execution (including exception/redo). Implementations may choose various techniques suitable in terms
    /// of performance and memory usage (storage of the transition or the full object snapshot.)
    /// </summary>
    public interface ICommand : IWithPresentation, IXukAble, IChangeNotifier, IAction
    {
        /// <summary>
        /// Event fired after the <see cref="ICommand"/> has been executed
        /// </summary>
        event EventHandler<ExecutedEventArgs> executed;

        /// <summary>
        /// Event fired after the <see cref="ICommand"/> has been un-executed
        /// </summary>
        event EventHandler<UnExecutedEventArgs> unExecuted;

        /// <summary>
        /// Execute the reverse command.
        /// </summary>
        /// <exception cref="urakawa.exception.CannotUndoException">Thrown when the command cannot be reversed.</exception>
        void unExecute();

        /// <summary>
        /// True if the command is reversible.
        /// </summary>
        bool canUnExecute();

        /// <summary>
        /// Gets a list of the <see cref="media.data.MediaData"/> used by the Command
        /// </summary>
        /// <returns></returns>
        List<media.data.MediaData> getListOfUsedMediaData();
    }
}