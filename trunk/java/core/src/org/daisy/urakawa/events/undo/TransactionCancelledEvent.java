package org.daisy.urakawa.events.undo;

import org.daisy.urakawa.undo.IUndoRedoManager;

/**
 * 
 *
 */
public class TransactionCancelledEvent extends UndoRedoManagerEvent
{
    /**
     * @param source
     */
    public TransactionCancelledEvent(IUndoRedoManager source)
    {
        super(source);
    }
}
