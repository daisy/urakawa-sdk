package org.daisy.urakawa.commands;

import org.daisy.urakawa.exceptions.CannotRedoException;
import org.daisy.urakawa.exceptions.CannotUndoException;

/**
 * Classes realizing this interface must store the history of Commands
 * and maintain the current Undo/Redo state (pointer in the dynamic stack of Commands).
 *
 * @depend - Composition(history) 1..n Command
 */
public interface CommandManager {
    /**
     * @return a human-readable name for the next Undoable Command
     * @tagvalue Exceptions "CannotUndo"
     * @see #canUndo()
     * @see org.daisy.urakawa.commands.Command#getUnExecuteShortDescription()
     */
    public String getUndoShortDescription() throws CannotUndoException;

    /**
     * undoes the last executed Command
     *
     * @tagvalue Exceptions "CannotUndo"
     * @see #canUndo()
     * @see org.daisy.urakawa.commands.Command#unExecute() ()
     */
    public void undo() throws CannotUndoException;

    /**
     * @return a human-readable name for the next Redoable Command
     * @tagvalue Exceptions "CannotRedo"
     * @see #canRedo()
     * @see org.daisy.urakawa.commands.Command#getExecuteShortDescription()
     */
    public String getRedoShortDescription() throws CannotRedoException;

    /**
     * redoes the last undone Command
     *
     * @tagvalue Exceptions "CannotRedo"
     * @see #canRedo()
     * @see org.daisy.urakawa.commands.Command#execute()
     */
    public void redo() throws CannotRedoException;

    /**
     * Executes and registers the given Command in the history,
     * deleting all following undone Commands in the history (on the "right hand side"), if any.
     * In some special cases (e.g. user typing text letter by letter, but undo/redo applies to full word or sentence),
     * this method my take the responsibility to automatically merge a series of Commands into a CompositeCommand.
     *
     * @param command the Command to register in the history and to execute.
     */
    public void execute(Command command);

    /**
     * @return false if the history is empty, otherwise true if the last executed Command (via "execute()" or "redo()")) is undoable.
     * @see org.daisy.urakawa.commands.Command#canUnExecute()
     */
    public boolean canUndo();

    /**
     * @return false if the history is empty, otherwise true if the last undone Command (via "undo()")) is redoable.
     */
    public boolean canRedo();
}
