package org.daisy.urakawa.progress;

import org.daisy.urakawa.command.IAction;
import org.daisy.urakawa.event.progress.CancelledEvent;
import org.daisy.urakawa.event.progress.FinishedEvent;
import org.daisy.urakawa.event.progress.ProgressEvent;
import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 *
 */
public abstract class ProgressAction implements IAction, IProgressHandler {
	protected boolean mCancelHasBeenRequested;

	/**
	 * 
	 */
	public ProgressAction() {
		mCancelHasBeenRequested = false;
	}

	/**
	 * 
	 */
	public void requestCancel() {
		mCancelHasBeenRequested = true;
	}

	/**
	 * @return bool
	 */
	public boolean cancelHasBeenRequested() {
		return mCancelHasBeenRequested;
	}

	/**
	 * @return information about current progress
	 */
	public abstract ProgressInformation getProgressInfo();

	public boolean notifyProgress() {
		if (cancelHasBeenRequested()) {
			return true;
		}
		ProgressInformation pi = getProgressInfo();
		if (pi == null) {
			return false;
		}
		ProgressEvent event = new ProgressEvent(pi.getCurrent(), pi.getTotal());
		try {
			notifyListeners(event);
		} catch (MethodParameterIsNullException e) {
			System.out.println("WTF ?! This should never happen !");
			e.printStackTrace();
		}
		if (event.isCancelled()) {
			return true;
		}
		return false;
	}

	public void notifyCancelled() {
		try {
			notifyListeners(new CancelledEvent());
		} catch (MethodParameterIsNullException e) {
			System.out.println("WTF ?! This should never happen.");
			e.printStackTrace();
		}
	}

	public void notifyFinished() {
		try {
			notifyListeners(new FinishedEvent());
		} catch (MethodParameterIsNullException e) {
			System.out.println("WTF ?! This should never happen.");
			e.printStackTrace();
		}
	}
}
