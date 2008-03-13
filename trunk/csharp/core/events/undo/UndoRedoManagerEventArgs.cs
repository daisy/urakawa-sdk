using System;
using System.Collections.Generic;
using System.Text;
using urakawa.undo;

namespace urakawa.events.undo
{
	public class UndoRedoManagerEventArgs : DataModelChangedEventArgs
	{
		public UndoRedoManagerEventArgs(UndoRedoManager source)
			: base(source)
		{
			SourceUndoRedoManager = source;
		}

		public readonly UndoRedoManager SourceUndoRedoManager;
	}
}
