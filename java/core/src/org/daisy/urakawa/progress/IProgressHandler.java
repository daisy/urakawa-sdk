package org.daisy.urakawa.progress;

import org.daisy.urakawa.event.Event;
import org.daisy.urakawa.event.IEventHandler;

/**
 *
 */
public interface IProgressHandler extends IEventHandler<Event> {
	/**
	 * @return true if the progress event has been marked as "canceled" at some
	 *         point by receivers while being dispatched to the currently
	 *         registered listeners. Otherwise false (the event has not been
	 *         canceled).
	 */
	public boolean notifyProgress();

	/**
	 * 
	 */
	public void notifyCancelled();

	/**
	 * 
	 */
	public void notifyFinished();
}
