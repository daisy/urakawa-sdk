package org.daisy.urakawa.undo;


/**
 * The actual implementation to be implemented by the implementation team ;)
 * All method bodies must be completed for realizing the required business logic.
 * -
 * This is the DEFAULT implementation for the API/Toolkit:
 * end-users should feel free to use this class as such,
 * or they can sub-class it in order to specialize the instance creation process.
 * -
 * In addition, an end-user has the possibility to implement the
 * singleton factory pattern, so that only one instance of the factory
 * is used throughout the application life
 * (by adding a method like "static Factory getFactory()").
 *
 * @see CommandManager
 */
public class CommandManagerImpl implements CommandManager {
    /**
     * @hidden
     */
    public String getUndoShortDescription() throws CannotUndoException {
        return null;
    }

    /**
     * @hidden
     */
    public void undo() throws CannotUndoException {
    }

    /**
     * @hidden
     */
    public String getRedoShortDescription() throws CannotRedoException {
        return null;
    }

    /**
     * @hidden
     */
    public void redo() throws CannotRedoException {
    }

    /**
     * @hidden
     */
    public void execute(Command command) {
    }

    /**
     * @hidden
     */
    public boolean canUndo() {
        return false;
    }

    /**
     * @hidden
     */
    public boolean canRedo() {
        return false;
    }
}
