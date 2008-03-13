package org.daisy.urakawa.event.undo;

import org.daisy.urakawa.undo.Command;
import org.daisy.urakawa.undo.UndoRedoManager;

/**
 *
 */
public class CommandDoneEvent extends UndoRedoManagerEvent {
	/**
	 * @param source
	 * @param done
	 */
	public CommandDoneEvent(UndoRedoManager source, Command done) {
		super(source);
		mDoneCommand = done;
	}

	private Command mDoneCommand;

	/**
	 * @return data
	 */
	public Command getDoneCommand() {
		return mDoneCommand;
	}
}
