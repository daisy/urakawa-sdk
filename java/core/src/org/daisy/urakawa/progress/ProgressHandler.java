package org.daisy.urakawa.progress;

import org.daisy.urakawa.event.EventHandler;
import org.daisy.urakawa.event.progress.ProgressEvent;

/**
 *
 */
public interface ProgressHandler extends EventHandler<ProgressEvent> {
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
