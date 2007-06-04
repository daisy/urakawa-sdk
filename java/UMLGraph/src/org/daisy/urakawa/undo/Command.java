package org.daisy.urakawa.undo;


/**
 * Classes realizing this interface must store the state of the object(s) affected by the command execution (including undo/redo).
 * Implementations may choose various techniques suitable in terms of performance and memory usage (storage of the transition or of the full object snapshot).
 */
public interface Command {
    /**
     * executes the reverse Command
     *
     * @tagvalue Exceptions "CannotUndo"
     * @throws CannotUndoException
     */
    public void unExecute() throws CannotUndoException;

    /**
     * @return a human-readable name for the reverse Command
     * @tagvalue Exceptions "CannotUndo"
     * @throws CannotUndoException
     */
    public String getUnExecuteShortDescription() throws CannotUndoException;

    /**
     * executes the Command
     */
    public void execute();

    /**
     * @return a human-readable name for the Command
     */
    public String getExecuteShortDescription();

    /**
     * @return true if this Command is undoable (reversible).
     */
    public boolean canUnExecute();
}
