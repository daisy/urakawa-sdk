package org.daisy.urakawa.event.undo;

import org.daisy.urakawa.undo.IUndoRedoManager;

/**
 * 
 *
 */
public class TransactionStartedEvent extends UndoRedoManagerEvent {
	/**
	 * @param source
	 */
	public TransactionStartedEvent(IUndoRedoManager source) {
		super(source);
	}
}
