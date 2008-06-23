package org.daisy.urakawa.command;

import java.net.URI;
import java.util.LinkedList;
import java.util.List;

import org.daisy.urakawa.WithPresentation;
import org.daisy.urakawa.event.Event;
import org.daisy.urakawa.event.IEventHandler;
import org.daisy.urakawa.event.EventHandler;
import org.daisy.urakawa.event.IEventListener;
import org.daisy.urakawa.event.command.CommandAddedEvent;
import org.daisy.urakawa.event.command.CommandEvent;
import org.daisy.urakawa.event.command.CommandExecutedEvent;
import org.daisy.urakawa.event.command.CommandUnExecutedEvent;
import org.daisy.urakawa.exception.IsNotInitializedException;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.exception.MethodParameterIsOutOfBoundsException;
import org.daisy.urakawa.media.data.IMediaData;
import org.daisy.urakawa.nativeapi.IXmlDataReader;
import org.daisy.urakawa.nativeapi.IXmlDataWriter;
import org.daisy.urakawa.progress.ProgressCancelledException;
import org.daisy.urakawa.progress.IProgressHandler;
import org.daisy.urakawa.xuk.IXukAble;
import org.daisy.urakawa.xuk.XukDeserializationFailedException;
import org.daisy.urakawa.xuk.XukSerializationFailedException;

/**
 * Reference implementation
 */
public class CompositeCommand extends WithPresentation implements
		ICompositeCommand {
	private List<ICommand> mCommands;
	private String mLongDescription = "";
	private String mShortDescription = "";

	/**
	 * Default constructor
	 */
	public CompositeCommand() {
		mCommands = new LinkedList<ICommand>();
	}

	public void append(ICommand iCommand) throws MethodParameterIsNullException {
		try {
			insert(iCommand, getCount());
		} catch (MethodParameterIsOutOfBoundsException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
	}

	public int getCount() {
		return mCommands.size();
	}

	public List<ICommand> getListOfCommands() {
		return new LinkedList<ICommand>(mCommands);
	}

	public void insert(ICommand iCommand, int index)
			throws MethodParameterIsNullException,
			MethodParameterIsOutOfBoundsException {
		if (iCommand == null) {
			throw new MethodParameterIsNullException();
		}
		if (index < 0 || index > mCommands.size()) {
			throw new MethodParameterIsOutOfBoundsException();
		}
		mCommands.add(index, iCommand);
		notifyListeners(new CommandAddedEvent(this, iCommand, index));
	}

	@Override
	public void clear() {
		mCommands.clear();
		mShortDescription = null;
		mLongDescription = null;
		// super.clear();
	}

	public boolean canExecute() {
		if (mCommands.size() == 0)
			return false;
		for (ICommand iCommand : mCommands) {
			if (!iCommand.canExecute()) {
				return false;
			}
		}
		return true;
	}

	public boolean canUnExecute() {
		if (mCommands.size() == 0)
			return false;
		for (ICommand iCommand : mCommands) {
			if (!iCommand.canUnExecute()) {
				return false;
			}
		}
		return true;
	}

	public void execute() throws CommandCannotExecuteException {
		if (mCommands.size() == 0)
			throw new CommandCannotExecuteException();
		for (ICommand iCommand : mCommands)
			iCommand.execute();
		try {
			notifyListeners(new CommandExecutedEvent(this));
		} catch (MethodParameterIsNullException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
	}

	public List<IMediaData> getListOfUsedMediaData() {
		List<IMediaData> res = new LinkedList<IMediaData>();
		for (ICommand cmd : mCommands) {
			res.addAll(cmd.getListOfUsedMediaData());
		}
		return res;
	}

	public void unExecute() throws CommandCannotUnExecuteException {
		if (mCommands.size() == 0)
			throw new CommandCannotUnExecuteException();
		for (int i = mCommands.size() - 1; i >= 0; --i)
			mCommands.get(i).unExecute();
		try {
			notifyListeners(new CommandUnExecutedEvent(this));
		} catch (MethodParameterIsNullException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
	}

	public String getLongDescription() {
		if (mLongDescription != null && mLongDescription != "")
			return mLongDescription;
		String cmds = "-";
		if (mCommands.size() > 0) {
			cmds = mCommands.get(0).getLongDescription();
			for (int i = 1; i < mCommands.size(); i++) {
				cmds += "\n" + getLongDescription();
			}
		}
		return cmds;
	}

	public String getShortDescription() {
		if (mShortDescription != null && mShortDescription != "")
			return mShortDescription;
		String cmds = "-";
		if (mCommands.size() > 0) {
			cmds = mCommands.get(0).getShortDescription();
			if (mCommands.size() > 1) {
				cmds += "..." + mCommands.get(mCommands.size() - 1);
			}
		}
		return cmds;
	}

	public void setLongDescription(String str)
			throws MethodParameterIsNullException {
		if (str == null) {
			throw new MethodParameterIsNullException();
		}
		mShortDescription = str;
	}

	public void setShortDescription(String str)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
		if (str == null) {
			throw new MethodParameterIsNullException();
		}
		if (str.length() == 0) {
			throw new MethodParameterIsEmptyStringException();
		}
		mLongDescription = str;
	}

	@SuppressWarnings("unused")
	@Override
	public void xukInAttributes(IXmlDataReader source, IProgressHandler ph)
			throws XukDeserializationFailedException, ProgressCancelledException {

		// To avoid event notification overhead, we bypass this:
		if (false && ph != null && ph.notifyProgress()) {
			throw new ProgressCancelledException();
		}
		mShortDescription = source.getAttribute("shortDescription");
		mLongDescription = source.getAttribute("longDescription");
		// super.xukInAttributes(source);
	}

	@Override
	public void xukInChild(IXmlDataReader source, IProgressHandler ph)
			throws XukDeserializationFailedException,
			ProgressCancelledException {

		// To avoid event notification overhead, we bypass this:
		if (false && ph != null && ph.notifyProgress()) {
			throw new ProgressCancelledException();
		}
		// boolean readItem = false;
		if (source.getNamespaceURI() == IXukAble.XUK_NS) {
			if (source.getLocalName() == "mCommands") {
				xukInCommands(source, ph);
				// readItem = true;
			}
		}
		// if (!readItem) super.xukInChild(source);
	}

	private void xukInCommands(IXmlDataReader source, IProgressHandler ph)
			throws XukDeserializationFailedException,
			ProgressCancelledException {
		if (ph != null && ph.notifyProgress()) {
			throw new ProgressCancelledException();
		}
		if (!source.isEmptyElement()) {
			while (source.read()) {
				if (source.getNodeType() == IXmlDataReader.ELEMENT) {
					ICommand cmd;
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
					if (cmd == null) {
						throw new XukDeserializationFailedException();
					}
					try {
						append(cmd);
						cmd.xukIn(source, ph);
					} catch (MethodParameterIsNullException e) {
						// Should never happen
						throw new RuntimeException("WTF ??!", e);
					}
				} else if (source.getNodeType() == IXmlDataReader.END_ELEMENT) {
					break;
				}
				if (source.isEOF())
					throw new XukDeserializationFailedException();
			}
		}
	}

	@SuppressWarnings("unused")
	@Override
	public void xukOutAttributes(IXmlDataWriter destination, URI baseUri,
			IProgressHandler ph) throws XukSerializationFailedException, ProgressCancelledException {
		if (ph != null && ph.notifyProgress()) {
			throw new ProgressCancelledException();
		}
		if (mShortDescription != null) {
			destination.writeAttributeString("shortDescription",
					mShortDescription);
		}
		if (mLongDescription != null) {
			destination.writeAttributeString("longDescription",
					mLongDescription);
		}
		// super.xukOutAttributes(destination, baseUri);
	}

	@Override
	public void xukOutChildren(IXmlDataWriter destination, URI baseUri,
			IProgressHandler ph) throws XukSerializationFailedException,
			ProgressCancelledException {
		if (ph != null && ph.notifyProgress()) {
			throw new ProgressCancelledException();
		}
		destination.writeStartElement("mCommands", IXukAble.XUK_NS);
		for (ICommand cmd : getListOfCommands()) {
			try {
				cmd.xukOut(destination, baseUri, ph);
			} catch (MethodParameterIsNullException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!", e);
			}
		}
		destination.writeEndElement();
		// super.xukOutChildren(destination, baseUri);
	}

	protected IEventHandler<Event> mCommandExecutedEventNotifier = new EventHandler();
	protected IEventHandler<Event> mCommandUnExecutedEventNotifier = new EventHandler();
	protected IEventHandler<Event> mCommandAddedEventNotifier = new EventHandler();

	public <K extends CommandEvent> void notifyListeners(K event)
			throws MethodParameterIsNullException {
		if (event == null) {
			throw new MethodParameterIsNullException();
		}
		if (CommandExecutedEvent.class.isAssignableFrom(event.getClass())) {
			mCommandExecutedEventNotifier.notifyListeners(event);
		} else if (CommandUnExecutedEvent.class.isAssignableFrom(event
				.getClass())) {
			mCommandUnExecutedEventNotifier.notifyListeners(event);
		} else if (CommandAddedEvent.class.isAssignableFrom(event.getClass())) {
			mCommandAddedEventNotifier.notifyListeners(event);
		}
		// ICommand does know about the IPresentation to which it is
		// attached, however there is no forwarding of the event upwards in the
		// hierarchy (bubbling-up). The rationale is that there would be too
		// many unfiltered CommandEvents to capture (e.g. ICompositeCommand with
		// many sub-Commands)
		// mDataModelEventNotifier.notifyListeners(event);
	}

	public <K extends CommandEvent> void registerListener(
			IEventListener<K> listener, Class<K> klass)
			throws MethodParameterIsNullException {
		if (listener == null || klass == null) {
			throw new MethodParameterIsNullException();
		}
		if (CommandExecutedEvent.class.isAssignableFrom(klass)) {
			mCommandExecutedEventNotifier.registerListener(listener, klass);
		} else if (CommandUnExecutedEvent.class.isAssignableFrom(klass)) {
			mCommandUnExecutedEventNotifier.registerListener(listener, klass);
		} else if (CommandAddedEvent.class.isAssignableFrom(klass)) {
			mCommandAddedEventNotifier.registerListener(listener, klass);
		} else {
			// ICommand does know anything about the IPresentation to which
			// it is attached, however there is no possible registration of
			// listeners
			// onto the generic event bus (used for bubbling-up).
			// mDataModelEventNotifier.registerListener(listener, klass);
		}
	}

	public <K extends CommandEvent> void unregisterListener(
			IEventListener<K> listener, Class<K> klass)
			throws MethodParameterIsNullException {
		if (listener == null || klass == null) {
			throw new MethodParameterIsNullException();
		}
		if (CommandExecutedEvent.class.isAssignableFrom(klass)) {
			mCommandExecutedEventNotifier.unregisterListener(listener, klass);
		} else if (CommandUnExecutedEvent.class.isAssignableFrom(klass)) {
			mCommandUnExecutedEventNotifier.unregisterListener(listener, klass);
		} else if (CommandAddedEvent.class.isAssignableFrom(klass)) {
			mCommandAddedEventNotifier.unregisterListener(listener, klass);
		} else {
			// ICommand does know anything about the IPresentation to which
			// it is attached, however there is no possible unregistration of
			// listeners
			// from the generic event bus (used for bubbling-up).
			// mDataModelEventNotifier.unregisterListener(listener, klass);
		}
	}
}
