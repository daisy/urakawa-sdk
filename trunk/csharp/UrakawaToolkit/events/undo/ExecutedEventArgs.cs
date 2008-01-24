using System;
using System.Collections.Generic;
using System.Text;
using urakawa.undo;

namespace urakawa.events.undo
{
	public class ExecutedEventArgs : CommandEventArgs
	{
		public ExecutedEventArgs(ICommand source) : base(source) { }
	}
}
