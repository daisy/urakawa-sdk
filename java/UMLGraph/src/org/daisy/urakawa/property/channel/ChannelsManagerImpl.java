package org.daisy.urakawa.property.channel;

import java.net.URI;
import java.util.HashMap;
import java.util.LinkedList;
import java.util.List;
import java.util.Map;

import org.daisy.urakawa.WithPresentationImpl;
import org.daisy.urakawa.exception.IsNotInitializedException;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
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
public class ChannelsManagerImpl extends WithPresentationImpl implements
		ChannelsManager {
	private Map<String, Channel> mChannels;

	/**
	 * 
	 */
	public ChannelsManagerImpl() {
		mChannels = new HashMap<String, Channel>();
	}

	public ChannelFactory getChannelFactory() throws IsNotInitializedException {
		return getPresentation().getChannelFactory();
	}

	public void addChannel(Channel channel)
			throws MethodParameterIsNullException,
			ChannelAlreadyExistsException {
		try {
			addChannel(channel, getNewId());
		} catch (MethodParameterIsEmptyStringException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
	}

	public void addChannel(Channel channel, String uid)
			throws MethodParameterIsNullException,
			ChannelAlreadyExistsException,
			MethodParameterIsEmptyStringException {
		if (channel == null || uid == null) {
			throw new MethodParameterIsNullException();
		}
		if (uid == "") {
			throw new MethodParameterIsEmptyStringException();
		}
		if (mChannels.values().contains(channel)) {
			throw new ChannelAlreadyExistsException();
		}
		if (mChannels.containsKey(uid)) {
			throw new ChannelAlreadyExistsException();
		}
		mChannels.put(uid, channel);
	}

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

	public void removeChannel(Channel channel)
			throws MethodParameterIsNullException, ChannelDoesNotExistException {
		if (channel == null) {
			throw new MethodParameterIsNullException();
		}
		try {
			removeChannel(getUidOfChannel(channel));
		} catch (MethodParameterIsEmptyStringException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
	}

	@SuppressWarnings("unused")
	public void removeChannel(String uid)
			throws MethodParameterIsNullException,
			ChannelDoesNotExistException, MethodParameterIsEmptyStringException {
		Channel channel = getChannel(uid);
		ClearChannelTreeNodeVisitor clChVisitor = new ClearChannelTreeNodeVisitor(
				channel);
		try {
			getPresentation().getRootNode().acceptDepthFirst(clChVisitor);
		} catch (IsNotInitializedException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
		mChannels.remove(uid);
	}

	public List<Channel> getListOfChannels() {
		return new LinkedList<Channel>(mChannels.values());
	}

	public List<String> getListOfUids() {
		return new LinkedList<String>(mChannels.keySet());
	}

	public Channel getChannel(String uid)
			throws MethodParameterIsNullException,
			ChannelDoesNotExistException, MethodParameterIsEmptyStringException {
		if (uid == null) {
			throw new MethodParameterIsNullException();
		}
		if (uid == "") {
			throw new MethodParameterIsEmptyStringException();
		}
		if (!mChannels.keySet().contains(uid)) {
			throw new ChannelDoesNotExistException();
		}
		return mChannels.get(uid);
	}

	public String getUidOfChannel(Channel ch)
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
		for (Channel ch : getListOfChannels()) {
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

	public List<Channel> getListOfChannels(String channelName)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
		if (channelName == null) {
			throw new MethodParameterIsNullException();
		}
		if (channelName == "") {
			throw new MethodParameterIsEmptyStringException();
		}
		List<Channel> res = new LinkedList<Channel>();
		for (Channel ch : mChannels.values()) {
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
		if (uid == "") {
			throw new MethodParameterIsEmptyStringException();
		}
		return mChannels.containsKey(uid);
	}

	@Override
	protected void clear() {
		mChannels.clear();
		super.clear();
	}

	@Override
	protected void xukInChild(XmlDataReader source)
			throws MethodParameterIsNullException,
			XukDeserializationFailedException {
		if (source == null) {
			throw new MethodParameterIsNullException();
		}
		boolean readItem = false;
		if (source.getNamespaceURI() == XukAbleImpl.XUK_NS
				&& source.getLocalName() == "mChannels") {
			readItem = true;
			if (!source.isEmptyElement()) {
				while (source.read()) {
					if (source.getNodeType() == XmlDataReader.ELEMENT) {
						if (source.getLocalName() == "mChannelItem"
								&& source.getNamespaceURI() == XukAbleImpl.XUK_NS) {
							xukInChannelItem(source);
						} else if (!source.isEmptyElement()) {
							source.readSubtree().close();
						}
					} else if (source.getNodeType() == XmlDataReader.END_ELEMENT) {
						break;
					}
					if (source.isEOF())
						throw new XukDeserializationFailedException();
				}
			}
		}
		if (!readItem)
			super.xukInChild(source);
	}

	private void xukInChannelItem(XmlDataReader source)
			throws MethodParameterIsNullException,
			XukDeserializationFailedException {
		if (source == null) {
			throw new MethodParameterIsNullException();
		}
		String uid = source.getAttribute("uid");
		if (uid == "" || uid == null) {
			throw new XukDeserializationFailedException();
		}
		boolean foundChannel = false;
		if (!source.isEmptyElement()) {
			while (source.read()) {
				if (source.getNodeType() == XmlDataReader.ELEMENT) {
					Channel newCh;
					try {
						newCh = getChannelFactory()
								.createChannel(source.getLocalName(),
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
						newCh.xukIn(source);
						foundChannel = true;
					} else if (!source.isEmptyElement()) {
						source.readSubtree().close();
					}
				} else if (source.getNodeType() == XmlDataReader.END_ELEMENT) {
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
	protected void xukOutChildren(XmlDataWriter destination, URI baseUri)
			throws MethodParameterIsNullException,
			XukSerializationFailedException {
		if (destination == null || baseUri == null) {
			throw new MethodParameterIsNullException();
		}
		List<String> uids = getListOfUids();
		if (uids.size() > 0) {
			destination.writeStartElement("mChannels", XukAbleImpl.XUK_NS);
			for (String uid : uids) {
				destination.writeStartElement("mChannelItem",
						XukAbleImpl.XUK_NS);
				destination.writeAttributeString("uid", uid);
				try {
					getChannel(uid).xukOut(destination, baseUri);
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
		super.xukOutChildren(destination, baseUri);
	}

	@SuppressWarnings("unchecked")
	public boolean ValueEquals(ChannelsManager other)
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
}
