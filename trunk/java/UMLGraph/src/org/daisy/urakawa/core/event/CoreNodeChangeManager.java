package org.daisy.urakawa.core.event;

import org.daisy.urakawa.exceptions.MethodParameterIsNullException;

/**
 * <p>
 * <i>This is a part of the Event Listener design pattern (variation of the <a
 * href="http://en.wikipedia.org/wiki/Observer_pattern">Observer</a> pattern,
 * also known as Publish / Subscribe), as implemented in the API design of the
 * Urakawa SDK, to provide an event mechanism for listening to changes in the
 * data model.</i>
 * </p>
 * <p>
 * Classes that implement this interface are responsible for managing the
 * (un-)registration of the {@link CoreNodeChangeListener} event listeners, and
 * for notifying the {@link org.daisy.urakawa.core.CoreNode} changes to all
 * registered listeners.
 * </p>
 * <ul>
 * <li>
 * <p>
 * To avoid memory leaks and bad memory access, the design pattern requires that
 * the calls to register/unregister are always performed <b>in pairs</b> (this
 * logic respects the object ref-counting rule). This falls into the scope of
 * the responsibility of the application programmer.
 * </p>
 * <p>
 * However, there are possible scenarios where listener instances can
 * "disappear" without unregistering from this manager (e.g. de-allocation
 * because of a crash inside the listener, leaving the rest of the application
 * alive). This phenomenon is known as "<b>lapsed listeners</b>", where the
 * listener doesn't get garbage-collected because this manager still holds a
 * weak reference to it. At worst (when memory management is explicit, aka
 * "unmanaged"), this causes crashes because of bad memory pointer access when
 * propagating the event to a non-existing listener. API/SDK implementations
 * should cater for such scenario.
 * </p>
 * </li>
 * <li>
 * <p>
 * Another point to consider is the relative <b>lifetime</b> of the
 * {@link CoreNodeChangeEvent}, when it is passed to registered listeners
 * during the notification propagation. The general assumption is that the owner
 * of the event object is the caller of the
 * {@link CoreNodeChangeManager#notifyCoreNodeChangeListeners(CoreNodeChangeEvent)}
 * method. As such, receivers of the event should consider the event instance as
 * a temporary value and not hold a reference to it. Instead, the event or its
 * values can be duplicated for local use.
 * </p>
 * </li>
 * <li>
 * <p>
 * <b>Thread-safety</b> issues: concurrent modification may occur when
 * adding/removing listeners. This design does not specify coding constraints
 * and does not enforce thread-safe constructs. API implementors and application
 * programmers are responsible for ensuring the best behaviour in multi-threaded
 * environment, or can guarantee normal operation only in single-threaded mode.
 * </p>
 * <p>
 * Also in the context of thread-safety, a final piece of advice for
 * implementors is to avoid registering listeners inside the constructor. The
 * reason is because if such class is subclassed, the super constructor
 * registers the listener before it is fully initialized (i.e. by the subclass).
 * This can create serious race-condition issues because the listener can
 * potentially receive events before it is fully initialized. The recommended
 * design pattern is to separate <b>construct</b> and <b>initialize</b>. The
 * whole of the Urakawa object-oriented design makes use of interfaces rather
 * than concrete classes and of factories instead of explicit constructors,
 * which provides sufficient flexibility to implement a construct-initialize
 * object instanciation pattern.
 * </p>
 * </li>
 * </ul>
 * 
 * @see CoreNodeChangeListener#coreNodeChanged(CoreNodeChangeEvent)
 * @see CoreNodeChangeEvent
 * @depend - Aggregation 0..n CoreNodeChangeListener
 */
public interface CoreNodeChangeManager {

	/**
	 * <p>
	 * Adds an event listener.
	 * </p>
	 * <p>
	 * Does nothing if it is already registered.
	 * </p>
	 * 
	 * @throws MethodParameterIsNullException
	 *             if listener is null.
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 * @param listener
	 *            the event listener to add
	 */
	public void registerCoreNodeChangeListener(CoreNodeChangeListener listener)
			throws MethodParameterIsNullException;

	/**
	 * <p>
	 * Removes an event listener.
	 * </p>
	 * <p>
	 * Silently ignores if it is not actually registered.
	 * </p>
	 * 
	 * @throws MethodParameterIsNullException
	 *             if listener is null.
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 * @param listener
	 *            the event listener to remove. Cannot be null.
	 */
	public void unregisterCoreNodeChangeListener(CoreNodeChangeListener listener)
			throws MethodParameterIsNullException;

	/**
	 * <p>
	 * Notifies of the registered listeners.
	 * </p>
	 * <p>
	 * Typically, this method is called by a
	 * {@link org.daisy.urakawa.core.CoreNode} when its state has changed.
	 * </p>
	 * <p>
	 * This method iterates through all the registered listeners, and forwards
	 * the notification event to each {@link CoreNodeChangeListener} via its
	 * {@link CoreNodeChangeListener#coreNodeChanged(CoreNodeChangeEvent)}
	 * callback method.
	 * </p>
	 * <p>
	 * There can be many listeners, but by design there is <b>no guarantee</b>
	 * that the order of notification will match the order of registration.
	 * </p>
	 * 
	 * @throws MethodParameterIsNullException
	 *             if changeEvent is null.
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 * @param changeEvent
	 *            the change specification to broadcast to all registered
	 *            listeners. Cannot be null.
	 */
	public void notifyCoreNodeChangeListeners(CoreNodeChangeEvent changeEvent)
			throws MethodParameterIsNullException;
}
