package org.daisy.urakawa.event.undo;

import org.daisy.urakawa.undo.UndoRedoManager;

/**
 * 
 *
 */
public class TransactionCancelledEvent extends UndoRedoManagerEvent {
	/**
	 * @param source
	 */
	public TransactionCancelledEvent(UndoRedoManager source) {
		super(source);
	}
}
