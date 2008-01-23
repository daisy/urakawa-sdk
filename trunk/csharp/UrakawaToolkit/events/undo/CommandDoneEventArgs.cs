using System;
using System.Collections.Generic;
using System.Text;
using urakawa.undo;

namespace urakawa.events.undo
{
	public class CommandDoneEventArgs : UndoRedoManagerEventArgs
	{
		public CommandDoneEventArgs(UndoRedoManager source, ICommand doneCmd)
			: base(source)
		{
			DoneCommand = doneCmd;
		}

		public readonly ICommand DoneCommand;
	}
}
