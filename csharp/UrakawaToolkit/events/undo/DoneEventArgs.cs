using System;
using System.Collections.Generic;
using System.Text;
using urakawa.undo;

namespace urakawa.events.undo
{
	public class DoneEventArgs : UndoRedoManagerEventArgs
	{
		public DoneEventArgs(UndoRedoManager source, ICommand doneCmd)
			: base(source)
		{
			DoneCommand = doneCmd;
		}

		public readonly ICommand DoneCommand;
	}
}
