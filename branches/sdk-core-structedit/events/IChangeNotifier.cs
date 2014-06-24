using System;

namespace urakawa.events
{
    /// <summary>
    /// Interface implemented by all classes that emits event notifications
    /// </summary>
    public interface IChangeNotifier
    {
        /// <summary>
        /// Event fired after the <see cref="IChangeNotifier"/> has changed. 
        /// The event fire before any change specific event 
        /// </summary>
        event EventHandler<DataModelChangedEventArgs> Changed;
    }
}