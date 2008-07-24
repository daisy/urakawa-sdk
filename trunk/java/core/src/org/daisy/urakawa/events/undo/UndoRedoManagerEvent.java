package org.daisy.urakawa.events.undo;

import org.daisy.urakawa.events.DataModelChangedEvent;
import org.daisy.urakawa.undo.UndoRedoManager;

/**
 *
 *
 */
public class UndoRedoManagerEvent extends DataModelChangedEvent
{
    /**
     * @param source
     */
    public UndoRedoManagerEvent(UndoRedoManager source)
    {
        super(source);
        mSourceUndoRedoManager = source;
    }

    private UndoRedoManager mSourceUndoRedoManager;

    /**
     * @return data
     */
    public UndoRedoManager getSourceUndoRedoManager()
    {
        return mSourceUndoRedoManager;
    }
}
