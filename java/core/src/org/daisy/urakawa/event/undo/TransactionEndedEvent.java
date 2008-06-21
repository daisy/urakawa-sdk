package org.daisy.urakawa.event.undo;

import org.daisy.urakawa.undo.IUndoRedoManager;

/**
 * 
 *
 */
public class TransactionEndedEvent extends UndoRedoManagerEvent {
	/**
	 * @param source
	 */
	public TransactionEndedEvent(IUndoRedoManager source) {
		super(source);
	}
}
