using System;
using System.Collections.Generic;
using System.Text;
using urakawa.undo;

namespace urakawa.events.undo
{
	public class ReDoneEventArgs : UndoRedoManagerEventArgs
	{
		public ReDoneEventArgs(UndoRedoManager source, ICommand reDoneCmd) : base(source)
		{
			ReDoneCommand = reDoneCmd;
		}

		public readonly ICommand ReDoneCommand;
	}
}
