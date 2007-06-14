package org.daisy.urakawa.undo;

import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * Reference implementation of the interface.
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 */
public class CommandManagerImpl implements CommandManager {
	public boolean canRedo() {
		return false;
	}

	public boolean canUndo() {
		return false;
	}

	public void execute(Command command) throws MethodParameterIsNullException {
	}

	public String getRedoShortDescription() throws CannotRedoException {
		return null;
	}

	public String getUndoShortDescription() throws CannotUndoException {
		return null;
	}

	public void redo() throws CannotRedoException {
	}

	public void undo() throws CannotUndoException {
	}
}
