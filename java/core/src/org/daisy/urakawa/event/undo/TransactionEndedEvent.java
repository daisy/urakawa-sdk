package org.daisy.urakawa.event.undo;

import org.daisy.urakawa.undo.UndoRedoManager;

/**
 * 
 *
 */
public class TransactionEndedEvent extends UndoRedoManagerEvent {
	/**
	 * @param source
	 */
	public TransactionEndedEvent(UndoRedoManager source) {
		super(source);
	}
}
