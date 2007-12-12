package org.daisy.urakawa.undo;

import java.net.URI;
import java.util.LinkedList;
import java.util.List;
import java.util.Stack;

import org.daisy.urakawa.WithPresentationImpl;
import org.daisy.urakawa.exception.IsNotInitializedException;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.media.data.MediaData;
import org.daisy.urakawa.xuk.XmlDataReader;
import org.daisy.urakawa.xuk.XmlDataWriter;
import org.daisy.urakawa.xuk.XukAbleImpl;
import org.daisy.urakawa.xuk.XukDeserializationFailedException;
import org.daisy.urakawa.xuk.XukSerializationFailedException;

/**
 * Reference implementation of the interface.
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 */
public class UndoRedoManagerImpl extends WithPresentationImpl implements
		UndoRedoManager {
	private Stack<Command> mUndoStack;
	private Stack<Command> mRedoStack;
	private Stack<CompositeCommand> mActiveTransactions;

	/**
	 * 
	 */
	public UndoRedoManagerImpl() {
		mUndoStack = new Stack<Command>();
		mRedoStack = new Stack<Command>();
		mActiveTransactions = new Stack<CompositeCommand>();
	}

	public void flushCommands() throws UndoRedoTransactionIsNotEndedException {
		if (isTransactionActive()) {
			throw new UndoRedoTransactionIsNotEndedException();
		}
		mUndoStack.clear();
		mRedoStack.clear();
	}

	protected void pushCommand(Command command)
			throws CannotExecuteIrreversibleCommandException {
		if (isTransactionActive()) {
			if (!command.canUnExecute()) {
				throw new CannotExecuteIrreversibleCommandException();
			}
			try {
				mActiveTransactions.peek().append(command);
			} catch (MethodParameterIsNullException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!", e);
			}
		} else {
			if (command.canUnExecute()) {
				mUndoStack.push(command);
				mRedoStack.clear();
			} else {
				try {
					flushCommands();
				} catch (UndoRedoTransactionIsNotEndedException e) {
					// Should never happen
					throw new RuntimeException("WTF ??!", e);
				}
			}
		}
	}

	public void execute(Command command) throws MethodParameterIsNullException,
			CannotExecuteIrreversibleCommandException,
			CommandCannotExecuteException {
		if (command == null)
			throw new MethodParameterIsNullException();
		pushCommand(command);
		command.execute();
	}

	public void undo() throws CannotUndoException,
			UndoRedoTransactionIsNotEndedException {
		if (isTransactionActive()) {
			throw new UndoRedoTransactionIsNotEndedException();
		}
		if (mUndoStack.size() == 0)
			throw new CannotUndoException();
		try {
			mUndoStack.peek().unExecute();
		} catch (CommandCannotUnExecuteException e) {
			throw new CannotUndoException();
		}
		mRedoStack.push(mUndoStack.pop());
	}

	public void redo() throws CannotRedoException,
			UndoRedoTransactionIsNotEndedException {
		if (isTransactionActive()) {
			throw new UndoRedoTransactionIsNotEndedException();
		}
		if (mRedoStack.size() == 0)
			throw new CannotRedoException();
		try {
			mRedoStack.peek().execute();
		} catch (CommandCannotExecuteException e) {
			throw new CannotRedoException();
		}
		mUndoStack.push(mRedoStack.pop());
	}

	public String getRedoShortDescription() throws CannotRedoException,
			UndoRedoTransactionIsNotEndedException {
		if (mRedoStack.size() == 0) {
			throw new CannotRedoException();
		}
		if (isTransactionActive()) {
			throw new UndoRedoTransactionIsNotEndedException();
		}
		return mRedoStack.peek().getShortDescription();
	}

	public String getUndoShortDescription() throws CannotUndoException,
			UndoRedoTransactionIsNotEndedException {
		if (mUndoStack.size() == 0) {
			throw new CannotUndoException();
		}
		if (isTransactionActive()) {
			throw new UndoRedoTransactionIsNotEndedException();
		}
		return mUndoStack.peek().getShortDescription();
	}

	public List<Command> getListOfUndoStackCommands() {
		return new LinkedList<Command>(mUndoStack);
	}

	public List<Command> getListOfRedoStackCommands() {
		return new LinkedList<Command>(mRedoStack);
	}

	public List<Command> getListOfCommandsInCurrentTransactions() {
		List<Command> res = new LinkedList<Command>();
		for (CompositeCommand trans : mActiveTransactions) {
			res.addAll(trans.getListOfCommands());
		}
		return res;
	}

	public List<MediaData> getListOfUsedMediaData() {
		List<MediaData> res = new LinkedList<MediaData>();
		List<Command> commands = new LinkedList<Command>();
		commands.addAll(getListOfUndoStackCommands());
		commands.addAll(getListOfRedoStackCommands());
		commands.addAll(getListOfCommandsInCurrentTransactions());
		for (Command cmd : commands) {
			for (MediaData md : cmd.getListOfUsedMediaData()) {
				if (!res.contains(md))
					res.add(md);
			}
		}
		return res;
	}

	public boolean canUndo() {
		if (isTransactionActive())
			return false;
		if (mUndoStack.size() == 0)
			return false;
		return true;
	}

	public boolean canRedo() {
		if (isTransactionActive())
			return false;
		if (mRedoStack.size() == 0)
			return false;
		return true;
	}

	public void startTransaction(String shortDesc, String longDesc)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
		if (shortDesc == null || longDesc == null) {
			throw new MethodParameterIsNullException();
		}
		if (shortDesc.length() == 0) {
			throw new MethodParameterIsEmptyStringException();
		}
		CompositeCommand newTrans;
		try {
			newTrans = getPresentation().getCommandFactory()
					.createCompositeCommand();
		} catch (IsNotInitializedException e1) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e1);
		}
		try {
			newTrans.setShortDescription(shortDesc);
			newTrans.setLongDescription(longDesc);
		} catch (MethodParameterIsNullException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		} catch (MethodParameterIsEmptyStringException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
		mActiveTransactions.push(newTrans);
	}

	public void endTransaction()
			throws UndoRedoTransactionIsNotStartedException {
		if (!isTransactionActive()) {
			throw new UndoRedoTransactionIsNotStartedException();
		}
		try {
			pushCommand(mActiveTransactions.pop());
		} catch (CannotExecuteIrreversibleCommandException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
	}

	public void cancelTransaction()
			throws UndoRedoTransactionIsNotStartedException {
		if (!isTransactionActive()) {
			throw new UndoRedoTransactionIsNotStartedException();
		}
		try {
			mActiveTransactions.pop().unExecute();
		} catch (CommandCannotUnExecuteException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
	}

	public boolean isTransactionActive() {
		return (mActiveTransactions.size() > 0);
	}

	@Override
	protected void clear() {
		while (isTransactionActive()) {
			try {
				endTransaction();
			} catch (UndoRedoTransactionIsNotStartedException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!", e);
			}
		}
		try {
			flushCommands();
		} catch (UndoRedoTransactionIsNotEndedException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
		// super.clear();
	}

	@Override
	protected void xukInChild(XmlDataReader source)
			throws XukDeserializationFailedException {
		boolean readItem = false;
		if (source.getNamespaceURI() == XukAbleImpl.XUK_NS) {
			String str = source.getLocalName();
			if (str == "mUndoStack") {
				xukInCommandStack(source, mUndoStack);
			} else if (str == "mRedoStack") {
				xukInCommandStack(source, mRedoStack);
			} else if (str == "mActiveTransactions") {
				xukInCommandStack(source, mActiveTransactions);
			}
		}
		if (!(readItem || source.isEmptyElement())) {
			source.readSubtree().close();
		}
	}

	private <T extends Command> void xukInCommandStack(XmlDataReader source,
			Stack<T> stack) throws XukDeserializationFailedException {
		if (!source.isEmptyElement()) {
			while (source.read()) {
				if (source.getNodeType() == XmlDataReader.ELEMENT) {
					Command cmd = null;
					try {
						cmd = getPresentation().getCommandFactory()
								.createCommand(source.getLocalName(),
										source.getNamespaceURI());
					} catch (MethodParameterIsNullException e1) {
						// Should never happen
						throw new RuntimeException("WTF ??!", e1);
					} catch (MethodParameterIsEmptyStringException e1) {
						// Should never happen
						throw new RuntimeException("WTF ??!", e1);
					} catch (IsNotInitializedException e1) {
						// Should never happen
						throw new RuntimeException("WTF ??!", e1);
					}
					/*
					 * TODO: Check that cmd is of type T if
					 * (!(cmd.getClass().isAssignableFrom(T))) { throw new
					 * XukDeserializationFailedException(); }
					 */
					stack.push((T) cmd);
					try {
						cmd.xukIn(source);
					} catch (MethodParameterIsNullException e) {
						// Should never happen
						throw new RuntimeException("WTF ??!", e);
					}
				} else if (source.getNodeType() == XmlDataReader.END_ELEMENT) {
					break;
				}
				if (source.isEOF())
					throw new XukDeserializationFailedException();
			}
		}
	}

	@Override
	protected void xukOutChildren(XmlDataWriter destination, URI baseUri)
			throws XukSerializationFailedException {
		destination.writeStartElement("mUndoStack", XukAbleImpl.XUK_NS);
		for (Command cmd : mUndoStack) {
			try {
				cmd.xukOut(destination, baseUri);
			} catch (MethodParameterIsNullException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!", e);
			}
		}
		destination.writeEndElement();
		destination.writeStartElement("mRedoStack", XukAbleImpl.XUK_NS);
		for (Command cmd : mRedoStack) {
			try {
				cmd.xukOut(destination, baseUri);
			} catch (MethodParameterIsNullException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!", e);
			}
		}
		destination.writeEndElement();
		destination
				.writeStartElement("mActiveTransactions", XukAbleImpl.XUK_NS);
		for (CompositeCommand cmd : mActiveTransactions) {
			try {
				cmd.xukOut(destination, baseUri);
			} catch (MethodParameterIsNullException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!", e);
			}
		}
		destination.writeEndElement();
		// super.xukOutChildren(destination, baseUri);
	}
}
