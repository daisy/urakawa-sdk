using System;
using System.Collections.Generic;
using System.Text;
using urakawa.undo;

namespace urakawa.events.undo
{
	public class CommandExecutedEventArgs : CommandEventArgs
	{
		public CommandExecutedEventArgs(ICommand source) : base(source) { }
	}
}
