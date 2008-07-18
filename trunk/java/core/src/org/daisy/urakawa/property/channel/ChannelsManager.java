package org.daisy.urakawa.property.channel;

import java.net.URI;
import java.util.HashMap;
import java.util.LinkedList;
import java.util.List;
import java.util.Map;

import org.daisy.urakawa.WithPresentation;
import org.daisy.urakawa.exception.IsNotInitializedException;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.nativeapi.IXmlDataReader;
import org.daisy.urakawa.nativeapi.IXmlDataWriter;
import org.daisy.urakawa.progress.IProgressHandler;
import org.daisy.urakawa.progress.ProgressCancelledException;
import org.daisy.urakawa.xuk.IXukAble;
import org.daisy.urakawa.xuk.XukDeserializationFailedException;
import org.daisy.urakawa.xuk.XukSerializationFailedException;

/**
 * Reference implementation of the interface.
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 */
public final class ChannelsManager extends WithPresentation implements
		IChannelsManager {
	private Map<String, IChannel> mChannels;

	/**
	 * 
	 */
	public ChannelsManager() {
		mChannels = new HashMap<String, IChannel>();
	}
	public void addChannel(IChannel iChannel)
			throws MethodParameterIsNullException,
			ChannelAlreadyExistsException {
		try {
			addChannel(iChannel, getNewId());
		} catch (MethodParameterIsEmptyStringException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
	}

	public void addChannel(IChannel iChannel, String uid)
			throws MethodParameterIsNullException,
			ChannelAlreadyExistsException,
			MethodParameterIsEmptyStringException {
		if (iChannel == null || uid == null) {
			throw new MethodParameterIsNullException();
		}
		if (uid.length() == 0) {
			throw new MethodParameterIsEmptyStringException();
		}
		if (mChannels.values().contains(iChannel)) {
			throw new ChannelAlreadyExistsException();
		}
		if (mChannels.containsKey(uid)) {
			throw new ChannelAlreadyExistsException();
		}
		mChannels.put(uid, iChannel);
	}

	@SuppressWarnings("boxing")
	private String getNewId() {
		long i = 0;
		while (i < Integer.MAX_VALUE) {
			String newId = String.format("CHID{0:0000}", i);
			if (!mChannels.containsKey(newId))
				return newId;
			i++;
		}
		throw new RuntimeException("TOO MANY CHANNELS!!!");
	}

	public void removeChannel(IChannel iChannel)
			throws MethodParameterIsNullException, ChannelDoesNotExistException {
		if (iChannel == null) {
			throw new MethodParameterIsNullException();
		}
		try {
			removeChannel(getUidOfChannel(iChannel));
		} catch (MethodParameterIsEmptyStringException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
	}

	public void removeChannel(String uid)
			throws MethodParameterIsNullException,
			ChannelDoesNotExistException, MethodParameterIsEmptyStringException {
		IChannel iChannel = getChannel(uid);
		ClearChannelTreeNodeVisitor clChVisitor = new ClearChannelTreeNodeVisitor(
				iChannel);
		try {
			getPresentation().getRootNode().acceptDepthFirst(clChVisitor);
		} catch (IsNotInitializedException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
		mChannels.remove(uid);
	}

	public List<IChannel> getListOfChannels() {
		return new LinkedList<IChannel>(mChannels.values());
	}

	public List<String> getListOfUids() {
		return new LinkedList<String>(mChannels.keySet());
	}

	public IChannel getChannel(String uid)
			throws MethodParameterIsNullException,
			ChannelDoesNotExistException, MethodParameterIsEmptyStringException {
		if (uid == null) {
			throw new MethodParameterIsNullException();
		}
		if (uid.length() == 0) {
			throw new MethodParameterIsEmptyStringException();
		}
		if (!mChannels.keySet().contains(uid)) {
			throw new ChannelDoesNotExistException();
		}
		return mChannels.get(uid);
	}

	public String getUidOfChannel(IChannel ch)
			throws MethodParameterIsNullException, ChannelDoesNotExistException {
		if (ch == null) {
			throw new MethodParameterIsNullException();
		}
		for (String Id : mChannels.keySet()) {
			if (mChannels.get(Id) == ch) {
				return Id;
			}
		}
		throw new ChannelDoesNotExistException();
	}

	public void clearChannels() {
		for (IChannel ch : getListOfChannels()) {
			try {
				removeChannel(ch);
			} catch (MethodParameterIsNullException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!", e);
			} catch (ChannelDoesNotExistException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!", e);
			}
		}
	}

	public List<IChannel> getListOfChannels(String channelName)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
		if (channelName == null) {
			throw new MethodParameterIsNullException();
		}
		if (channelName.length() == 0) {
			throw new MethodParameterIsEmptyStringException();
		}
		List<IChannel> res = new LinkedList<IChannel>();
		for (IChannel ch : mChannels.values()) {
			try {
				if (ch.getName() == channelName)
					res.add(ch);
			} catch (IsNotInitializedException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!", e);
			}
		}
		return res;
	}

	public boolean isManagerOf(String uid)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
		if (uid == null) {
			throw new MethodParameterIsNullException();
		}
		if (uid.length() == 0) {
			throw new MethodParameterIsEmptyStringException();
		}
		return mChannels.containsKey(uid);
	}

	@Override
	protected void clear() {
		mChannels.clear();
		// super.clear();
	}

	@Override
	protected void xukInChild(IXmlDataReader source, IProgressHandler ph)
			throws MethodParameterIsNullException,
			XukDeserializationFailedException, ProgressCancelledException {
		if (source == null) {
			throw new MethodParameterIsNullException();
		}
		// To avoid event notification overhead, we bypass this:
		if (false && ph != null && ph.notifyProgress()) {
			throw new ProgressCancelledException();
		}
		boolean readItem = false;
		if (source.getNamespaceURI() == IXukAble.XUK_NS
				&& source.getLocalName() == "mChannels") {
			readItem = true;
			if (!source.isEmptyElement()) {
				while (source.read()) {
					if (source.getNodeType() == IXmlDataReader.ELEMENT) {
						if (source.getLocalName() == "mChannelItem"
								&& source.getNamespaceURI() == IXukAble.XUK_NS) {
							xukInChannelItem(source, ph);
						} else {
							super.xukInChild(source, ph);
						}
					} else if (source.getNodeType() == IXmlDataReader.END_ELEMENT) {
						break;
					}
					if (source.isEOF())
						throw new XukDeserializationFailedException();
				}
			}
		}
		if (!readItem) {
			super.xukInChild(source, ph);
		}
	}

	private void xukInChannelItem(IXmlDataReader source, IProgressHandler ph)
			throws MethodParameterIsNullException,
			XukDeserializationFailedException, ProgressCancelledException {
		if (source == null) {
			throw new MethodParameterIsNullException();
		}
		if (ph != null && ph.notifyProgress()) {
			throw new ProgressCancelledException();
		}
		String uid = source.getAttribute("uid");
		if (uid == null || uid.length() == 0) {
			throw new XukDeserializationFailedException();
		}
		boolean foundChannel = false;
		if (!source.isEmptyElement()) {
			while (source.read()) {
				if (source.getNodeType() == IXmlDataReader.ELEMENT) {
					IChannel newCh;
					try {
						newCh = getPresentation().getChannelFactory()
								.create(source.getLocalName(),
										source.getNamespaceURI());
					} catch (MethodParameterIsEmptyStringException e1) {
						// Should never happen
						throw new RuntimeException("WTF ??!", e1);
					} catch (IsNotInitializedException e1) {
						// Should never happen
						throw new RuntimeException("WTF ??!", e1);
					}
					if (newCh != null) {
						try {
							addChannel(newCh, uid);
						} catch (MethodParameterIsEmptyStringException e) {
							// Should never happen
							throw new RuntimeException("WTF ??!", e);
						} catch (ChannelAlreadyExistsException e) {
							// Should never happen
							throw new RuntimeException("WTF ??!", e);
						}
						newCh.xukIn(source, ph);
						foundChannel = true;
					} else {
						super.xukInChild(source, ph);
					}
				} else if (source.getNodeType() == IXmlDataReader.END_ELEMENT) {
					break;
				}
				if (source.isEOF())
					throw new XukDeserializationFailedException();
			}
		}
		if (!foundChannel) {
			throw new XukDeserializationFailedException();
		}
	}

	@Override
	protected void xukOutChildren(IXmlDataWriter destination, URI baseUri,
			IProgressHandler ph) throws MethodParameterIsNullException,
			XukSerializationFailedException, ProgressCancelledException {
		if (destination == null || baseUri == null) {
			throw new MethodParameterIsNullException();
		}
		if (ph != null && ph.notifyProgress()) {
			throw new ProgressCancelledException();
		}
		List<String> uids = getListOfUids();
		if (uids.size() > 0) {
			destination.writeStartElement("mChannels", IXukAble.XUK_NS);
			for (String uid : uids) {
				destination.writeStartElement("mChannelItem", IXukAble.XUK_NS);
				destination.writeAttributeString("uid", uid);
				try {
					getChannel(uid).xukOut(destination, baseUri, ph);
				} catch (MethodParameterIsEmptyStringException e) {
					// Should never happen
					throw new RuntimeException("WTF ??!", e);
				} catch (ChannelDoesNotExistException e) {
					// Should never happen
					throw new RuntimeException("WTF ??!", e);
				}
				destination.writeEndElement();
			}
			destination.writeEndElement();
		}
		// super.xukOutChildren(destination, baseUri, ph);
	}

	public boolean ValueEquals(IChannelsManager other)
			throws MethodParameterIsNullException {
		if (other == null) {
			throw new MethodParameterIsNullException();
		}
		List<String> thisUids = getListOfUids();
		List<String> otherUids = other.getListOfUids();
		if (thisUids.size() != otherUids.size())
			return false;
		for (String uid : thisUids) {
			if (!otherUids.contains(uid))
				return false;
			try {
				if (!getChannel(uid).ValueEquals(other.getChannel(uid)))
					return false;
			} catch (MethodParameterIsEmptyStringException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!", e);
			} catch (ChannelDoesNotExistException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!", e);
			}
		}
		return true;
	}

	@SuppressWarnings("unused")
	@Override
	protected void xukInAttributes(IXmlDataReader source, IProgressHandler ph)
			throws MethodParameterIsNullException,
			XukDeserializationFailedException, ProgressCancelledException {
	}

	@SuppressWarnings("unused")
	@Override
	protected void xukOutAttributes(IXmlDataWriter destination, URI baseUri,
			IProgressHandler ph) throws XukSerializationFailedException,
			MethodParameterIsNullException, ProgressCancelledException {
	}
}
