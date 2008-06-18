package org.daisy.urakawa.progress;

import org.daisy.urakawa.event.Event;
import org.daisy.urakawa.event.EventHandler;

/**
 *
 */
public interface ProgressHandler extends EventHandler<Event> {
	/**
	 * 
	 */
	public void notifyProgress();

	/**
	 * 
	 */
	public void notifyCancelled();

	/**
	 * 
	 */
	public void notifyFinished();
}
