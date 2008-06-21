package org.daisy.urakawa.event.undo;

import org.daisy.urakawa.undo.IUndoRedoManager;

/**
 * 
 *
 */
public class TransactionCancelledEvent extends UndoRedoManagerEvent {
	/**
	 * @param source
	 */
	public TransactionCancelledEvent(IUndoRedoManager source) {
		super(source);
	}
}
