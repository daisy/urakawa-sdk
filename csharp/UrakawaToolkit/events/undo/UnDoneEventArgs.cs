using System;
using System.Collections.Generic;
using System.Text;
using urakawa.undo;

namespace urakawa.events.undo
{
	public class UnDoneEventArgs : UndoRedoManagerEventArgs
	{
		public UnDoneEventArgs(UndoRedoManager source, ICommand unDoneCmd) : base(source)
		{
			UnDoneCommand = unDoneCmd;
		}

		public readonly ICommand UnDoneCommand;
	}
}
