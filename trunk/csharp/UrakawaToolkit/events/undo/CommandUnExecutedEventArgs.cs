using System;
using System.Collections.Generic;
using System.Text;
using urakawa.undo;

namespace urakawa.events.undo
{
	public class CommandUnExecutedEventArgs : CommandEventArgs
	{
		public CommandUnExecutedEventArgs(ICommand source) : base(source) { }
	}
}
