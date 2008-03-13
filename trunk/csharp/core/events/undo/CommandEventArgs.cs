using System;
using System.Collections.Generic;
using System.Text;
using urakawa.undo;
using urakawa.events;

namespace urakawa.events.undo
{
	public class CommandEventArgs : DataModelChangedEventArgs
	{
		public CommandEventArgs(ICommand source)
			: base(source)
		{
			SourceCommand = source;
		}

		public readonly ICommand SourceCommand;
	}
}
