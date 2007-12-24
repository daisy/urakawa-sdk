package org.daisy.urakawa.undo;

import java.net.URI;
import java.util.LinkedList;
import java.util.List;

import org.daisy.urakawa.WithPresentationImpl;
import org.daisy.urakawa.exception.IsNotInitializedException;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.exception.MethodParameterIsOutOfBoundsException;
import org.daisy.urakawa.media.data.MediaData;
import org.daisy.urakawa.nativeapi.XmlDataReader;
import org.daisy.urakawa.nativeapi.XmlDataWriter;
import org.daisy.urakawa.xuk.XukAble;
import org.daisy.urakawa.xuk.XukDeserializationFailedException;
import org.daisy.urakawa.xuk.XukSerializationFailedException;

/**
 * Reference implementation
 */
public class CompositeCommandImpl extends WithPresentationImpl implements
		CompositeCommand {
	private List<Command> mCommands;
	private String mLongDescription = "";
	private String mShortDescription = "";

	/**
	 * Default constructor
	 */
	public CompositeCommandImpl() {
		mCommands = new LinkedList<Command>();
	}

	public void append(Command command) throws MethodParameterIsNullException {
		try {
			insert(command, getCount());
		} catch (MethodParameterIsOutOfBoundsException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
	}

	public int getCount() {
		return mCommands.size();
	}

	public List<Command> getListOfCommands() {
		return new LinkedList<Command>(mCommands);
	}

	public void insert(Command command, int index)
			throws MethodParameterIsNullException,
			MethodParameterIsOutOfBoundsException {
		if (command == null) {
			throw new MethodParameterIsNullException();
		}
		if (index < 0 || index > mCommands.size()) {
			throw new MethodParameterIsOutOfBoundsException();
		}
		mCommands.add(index, command);
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
		for (Command command : mCommands) {
			if (!command.canExecute()) {
				return false;
			}
		}
		return true;
	}

	public boolean canUnExecute() {
		if (mCommands.size() == 0)
			return false;
		for (Command command : mCommands) {
			if (!command.canUnExecute()) {
				return false;
			}
		}
		return true;
	}

	public void execute() throws CommandCannotExecuteException {
		if (mCommands.size() == 0)
			throw new CommandCannotExecuteException();
		for (Command command : mCommands)
			command.execute();
	}

	public List<MediaData> getListOfUsedMediaData() {
		List<MediaData> res = new LinkedList<MediaData>();
		for (Command cmd : mCommands) {
			res.addAll(cmd.getListOfUsedMediaData());
		}
		return res;
	}

	public void unExecute() throws CommandCannotUnExecuteException {
		if (mCommands.size() == 0)
			throw new CommandCannotUnExecuteException();
		for (int i = mCommands.size() - 1; i >= 0; --i)
			mCommands.get(i).unExecute();
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
	public void xukInAttributes(XmlDataReader source)
			throws XukDeserializationFailedException {
		mShortDescription = source.getAttribute("shortDescription");
		mLongDescription = source.getAttribute("longDescription");
		// super.xukInAttributes(source);
	}

	@Override
	public void xukInChild(XmlDataReader source)
			throws XukDeserializationFailedException {
		// boolean readItem = false;
		if (source.getNamespaceURI() == XukAble.XUK_NS) {
			if (source.getLocalName() == "mCommands") {
				xukInCommands(source);
				// readItem = true;
			}
		}
		// if (!readItem) super.xukInChild(source);
	}

	private void xukInCommands(XmlDataReader source)
			throws XukDeserializationFailedException {
		if (!source.isEmptyElement()) {
			while (source.read()) {
				if (source.getNodeType() == XmlDataReader.ELEMENT) {
					Command cmd;
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

	@SuppressWarnings("unused")
	@Override
	public void xukOutAttributes(XmlDataWriter destination, URI baseUri)
			throws XukSerializationFailedException {
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
	public void xukOutChildren(XmlDataWriter destination, URI baseUri)
			throws XukSerializationFailedException {
		destination.writeStartElement("mCommands", XukAble.XUK_NS);
		for (Command cmd : getListOfCommands()) {
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
