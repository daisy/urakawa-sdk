using System;
using System.Collections.Generic;
using System.Text;
using urakawa.undo;

namespace urakawa.events.undo
{
	public class TransactionEndedEventArgs : UndoRedoManagerEventArgs
	{
		public TransactionEndedEventArgs(UndoRedoManager source) : base(source) { }
	}
}
