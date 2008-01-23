using System;
using System.Collections.Generic;
using System.Text;
using urakawa.undo;

namespace urakawa.events.undo
{
	public class TransactionStartedEventArgs : UndoRedoManagerEventArgs
	{
		public TransactionStartedEventArgs(UndoRedoManager source) : base(source) { }
	}
}
