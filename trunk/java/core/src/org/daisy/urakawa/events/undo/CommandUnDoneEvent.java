package org.daisy.urakawa.events.undo;

import org.daisy.urakawa.command.ICommand;
import org.daisy.urakawa.undo.IUndoRedoManager;

/**
 * 
 *
 */
public class CommandUnDoneEvent extends UndoRedoManagerEvent {
	/**
	 * @param source
	 * @param unDone
	 */
	public CommandUnDoneEvent(IUndoRedoManager source, ICommand unDone) {
		super(source);
		mUnDoneCommand = unDone;
	}

	private ICommand mUnDoneCommand;

	/**
	 * @return data
	 */
	public ICommand getUnDoneCommand() {
		return mUnDoneCommand;
	}
}
