package org.daisy.urakawa.events.undo;

import org.daisy.urakawa.undo.UndoRedoManager;

/**
 * 
 *
 */
public class TransactionStartedEvent extends UndoRedoManagerEvent
{
    /**
     * @param source
     */
    public TransactionStartedEvent(UndoRedoManager source)
    {
        super(source);
    }
}
