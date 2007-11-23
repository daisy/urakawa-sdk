using System;
using System.Collections.Generic;
using System.Text;

namespace urakawa.events
{
	public interface IChangeNotifier
	{
		/// <summary>
		/// Event fired after the <see cref="IChangeNotifier"/> has changed. 
		/// The event fire before any change specific event 
		/// </summary>
		event EventHandler<urakawa.events.DataModelChangedEventArgs> changed;
	}
}
