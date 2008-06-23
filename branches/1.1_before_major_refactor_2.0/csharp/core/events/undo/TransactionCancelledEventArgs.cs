using System;
using System.Collections.Generic;
using System.Text;
using urakawa.undo;

namespace urakawa.events.undo
{
    /// <summary>
    /// Arguments of the <see cref="UndoRedoManager.transactionCancelled"/> command
    /// </summary>
	public class TransactionCancelledEventArgs : UndoRedoManagerEventArgs
	{
        /// <summary>
        /// Constructor setting the source <see cref="UndoRedoManager"/>
        /// </summary>
        /// <param name="source">The source <see cref="UndoRedoManager"/></param>
		public TransactionCancelledEventArgs(UndoRedoManager source) : base(source) { }
	}
}
