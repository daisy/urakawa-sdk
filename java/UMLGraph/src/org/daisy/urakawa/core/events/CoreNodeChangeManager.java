package org.daisy.urakawa.core.events;

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
 * (un-)registration of the {@link CoreNodeChangedListener} event listeners (or
 * specialized {@link CoreNodeAddedRemovedListener},
 * {@link CoreNodeRemovedListener}, etc.), and for dispatching the
 * {@link org.daisy.urakawa.core.CoreNode} change notification to all registered
 * listeners. The {@link CoreNodeChangedEvent} and its specialized sub-types are
 * containers for the information that describes the nature of the change in the
 * data model.
 * </p>
 * <p>
 * This part of the model is typically implemented by the SDK itself.
 * Application developers typically implement their own listeners to receive
 * change event notifications from the data model.
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
 * {@link CoreNodeChangedEvent}, when it is passed to registered listeners
 * during the notification propagation. The general assumption is that the owner
 * of the event object is the caller of the
 * {@link CoreNodeGenericChangeManager#notifyCoreNodeChangedListeners(CoreNodeChangedEvent)}
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
 * programmers are responsible for ensuring the best behavior in multi-threaded
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
 * object instantiation pattern.
 * </p>
 * </li>
 * </ul>
 * <p>
 * As a general programming guideline to avoid event duplication, a developer
 * should opt for:
 * </p>
 * <ul>
 * <li> <b><u>either</u></b> the generic event "bus" (i.e.
 * {@link CoreNodeChangedListener}, {@link CoreNodeChangedEvent} and
 * {@link CoreNodeGenericChangeManager#notifyCoreNodeChangedListeners(CoreNodeChangedListener)}).
 * <br>
 * This requires the listeners to check the actual class type of the received
 * event object and cast it appropriately (which can be seen as a disadvantage),
 * but on the other hand, this design pattern reduces the coupling between the
 * listeners and the manager by using the generic event middle-man ({@link CoreNodeChangedEvent}).
 * In other words, any addition to the list of existing event types has no
 * impact on the listener and manager interface definition (it does not break
 * existing implementations). </li>
 * <li> <b><u>or</u></b> the specialized listeners (e.g.
 * {@link CoreNodeAddedRemovedListener}, {@link CoreNodeAddedEvent} and
 * {@link CoreNodeAdditionRemovalManager#notifyCoreNodeAddedListeners(CoreNodeAddedEvent)}).
 * <br>
 * This is the proper application of the event listener design pattern. This
 * model injects a dependency between the manager and the listeners, because for
 * each event type, a "notify" method must have a matching callback method in
 * the listener interface. This may become a hurdle in respect to scalability,
 * because any new addition to the list of already supported event types has an
 * impact on all 3 interface components of the model (listener, manager, and
 * middle-man event type), which breaks existing implementations. Note: some
 * applications of this design pattern pass a number of different method
 * parameters corresponding to each event type, but in this API we are using
 * {@link CoreNodeChangedEvent} and its specialized sub-types as containers for
 * the data of the event (which results in a single method parameter instead of
 * potential multiple arguments)</li>
 * </ul>
 * <p>
 * The model proposed here offers the full flexibility of a rich event model.
 * </p>
 * 
 * @see CoreNodeChangedListener#coreNodeChanged(CoreNodeChangedEvent)
 * @see CoreNodeChangedEvent
 * @depend - Aggregation 0..n CoreNodeAddedRemovedListener
 * @depend - Aggregation 0..n CoreNodeRemovedListener
 */
public interface CoreNodeChangeManager extends CoreNodeGenericChangeManager,
		CoreNodeAdditionRemovalManager {
}
