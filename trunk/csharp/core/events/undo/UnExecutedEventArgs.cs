using System;
using System.Collections.Generic;
using System.Text;
using urakawa.undo;

namespace urakawa.events.undo
{
	public class UnExecutedEventArgs : CommandEventArgs
	{
		public UnExecutedEventArgs(ICommand source) : base(source) { }
	}
}
