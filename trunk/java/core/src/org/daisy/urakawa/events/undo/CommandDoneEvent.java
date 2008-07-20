package org.daisy.urakawa.events.undo;

import org.daisy.urakawa.command.ICommand;
import org.daisy.urakawa.undo.IUndoRedoManager;

/**
 *
 */
public class CommandDoneEvent extends UndoRedoManagerEvent {
	/**
	 * @param source
	 * @param done
	 */
	public CommandDoneEvent(IUndoRedoManager source, ICommand done) {
		super(source);
		mDoneCommand = done;
	}

	private ICommand mDoneCommand;

	/**
	 * @return data
	 */
	public ICommand getDoneCommand() {
		return mDoneCommand;
	}
}
