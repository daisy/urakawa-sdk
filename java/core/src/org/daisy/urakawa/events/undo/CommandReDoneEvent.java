package org.daisy.urakawa.events.undo;

import org.daisy.urakawa.command.ICommand;
import org.daisy.urakawa.undo.UndoRedoManager;

/**
 * 
 *
 */
public class CommandReDoneEvent extends UndoRedoManagerEvent
{
    /**
     * @param source
     * @param reDone
     */
    public CommandReDoneEvent(UndoRedoManager source, ICommand reDone)
    {
        super(source);
        mReDoneCommand = reDone;
    }

    private ICommand mReDoneCommand;

    /**
     * @return data
     */
    public ICommand getReDoneCommand()
    {
        return mReDoneCommand;
    }
}
