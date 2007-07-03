package org.daisy.urakawa.undo;

import org.daisy.urakawa.XmlDataReader;
import org.daisy.urakawa.XmlDataWriter;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.xuk.XukDeserializationFailedException;
import org.daisy.urakawa.xuk.XukSerializationFailedException;

/**
 * Reference implementation of the interface.
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 */
public class UndoRedoManagerImpl implements UndoRedoManager {
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

	public void XukIn(XmlDataReader source)
			throws MethodParameterIsNullException,
			XukDeserializationFailedException {
	}

	public void XukOut(XmlDataWriter destination)
			throws MethodParameterIsNullException,
			XukSerializationFailedException {
	}

	public String getXukLocalName() {
		return null;
	}

	public String getXukNamespaceURI() {
		return null;
	}

	public void flushCommands() {
	}

	public void cancelTransaction()
			throws UndoRedoTransactionIsNotStartedException {
	}

	public void endTransaction()
			throws UndoRedoTransactionIsNotStartedException {
	}

	public boolean isTransactionActive() {
		return false;
	}

	public void startTransaction() {
	}
}
