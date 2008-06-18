package org.daisy.urakawa.progress;

import org.daisy.urakawa.command.Action;
import org.daisy.urakawa.event.progress.CancelledEvent;
import org.daisy.urakawa.event.progress.FinishedEvent;
import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 *
 */
public abstract class ProgressAction implements Action, ProgressHandler {
	protected boolean mCancel;

	/**
	 * 
	 */
	public ProgressAction() {
		mCancel = false;
	}

	/**
	 * 
	 */
	public void requestCancel() {
		mCancel = true;
	}

	/**
	 * @return bool
	 */
	public boolean hasCancelBeenRequested() {
		return mCancel;
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
