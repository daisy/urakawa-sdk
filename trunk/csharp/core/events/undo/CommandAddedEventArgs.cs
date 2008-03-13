using System;
using System.Collections.Generic;
using System.Text;
using urakawa.undo;

namespace urakawa.events.undo
{
	public class CommandAddedEventArgs : CommandEventArgs
	{
		public CommandAddedEventArgs(CompositeCommand source, ICommand addee, int indx)
			:	base(source)
		{
			SourceCompositeCommand = source;
			AddedCommand = addee;
			Index = indx;
		}

		public readonly CompositeCommand SourceCompositeCommand;

		public readonly ICommand AddedCommand;

		public readonly int Index;
	}
}
