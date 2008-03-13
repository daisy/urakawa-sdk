using System;
using System.Collections.Generic;
using System.Text;
using urakawa.undo;

namespace urakawa.events.undo
{
	public class TransactionCancelledEventArgs : UndoRedoManagerEventArgs
	{
		public TransactionCancelledEventArgs(UndoRedoManager source) : base(source) { }
	}
}
