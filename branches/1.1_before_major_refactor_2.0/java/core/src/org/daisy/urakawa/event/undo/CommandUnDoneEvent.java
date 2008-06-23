package org.daisy.urakawa.event.undo;

import org.daisy.urakawa.command.Command;
import org.daisy.urakawa.undo.UndoRedoManager;

/**
 * 
 *
 */
public class CommandUnDoneEvent extends UndoRedoManagerEvent {
	/**
	 * @param source
	 * @param unDone
	 */
	public CommandUnDoneEvent(UndoRedoManager source, Command unDone) {
		super(source);
		mUnDoneCommand = unDone;
	}

	private Command mUnDoneCommand;

	/**
	 * @return data
	 */
	public Command getUnDoneCommand() {
		return mUnDoneCommand;
	}
}
