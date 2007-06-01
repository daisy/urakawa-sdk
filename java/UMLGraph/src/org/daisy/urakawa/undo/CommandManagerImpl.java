package org.daisy.urakawa.undo;

/**
 * Reference implementation of the interface.
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
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
