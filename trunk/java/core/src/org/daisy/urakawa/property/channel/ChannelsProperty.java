package org.daisy.urakawa.property.channel;

import java.net.URI;
import java.util.HashMap;
import java.util.LinkedList;
import java.util.List;
import java.util.Map;

import org.daisy.urakawa.FactoryCannotCreateTypeException;
import org.daisy.urakawa.IPresentation;
import org.daisy.urakawa.core.ITreeNode;
import org.daisy.urakawa.event.DataModelChangedEvent;
import org.daisy.urakawa.event.Event;
import org.daisy.urakawa.event.IEventHandler;
import org.daisy.urakawa.event.EventHandler;
import org.daisy.urakawa.event.IEventListener;
import org.daisy.urakawa.event.property.channel.ChannelMediaMapEvent;
import org.daisy.urakawa.exception.IsNotInitializedException;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.media.DoesNotAcceptMediaException;
import org.daisy.urakawa.media.IMedia;
import org.daisy.urakawa.nativeapi.IXmlDataReader;
import org.daisy.urakawa.nativeapi.IXmlDataWriter;
import org.daisy.urakawa.progress.ProgressCancelledException;
import org.daisy.urakawa.progress.IProgressHandler;
import org.daisy.urakawa.property.IProperty;
import org.daisy.urakawa.property.Property;
import org.daisy.urakawa.xuk.IXukAble;
import org.daisy.urakawa.xuk.XukDeserializationFailedException;
import org.daisy.urakawa.xuk.XukSerializationFailedException;

/**
 * Reference implementation of the interface.
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 */
public class ChannelsProperty extends Property implements IChannelsProperty {
	private Map<IChannel, IMedia> mMapChannelToMediaObject = new HashMap<IChannel, IMedia>();
	protected IEventHandler<Event> mChannelMediaMapEventNotifier = new EventHandler();
	protected IEventListener<ChannelMediaMapEvent> mChannelMediaMapEventListener = new IEventListener<ChannelMediaMapEvent>() {
		public <K extends ChannelMediaMapEvent> void eventCallback(K event)
				throws MethodParameterIsNullException {
			if (event == null) {
				throw new MethodParameterIsNullException();
			}
			if (event.getSourceChannelsProperty() == ChannelsProperty.this) {
				if (event.getMappedMedia() != null)
					event.getMappedMedia().registerListener(
							ChannelsProperty.this.mBubbleEventListener,
							DataModelChangedEvent.class);
				if (event.getPreviousMedia() != null)
					event.getPreviousMedia().unregisterListener(
							ChannelsProperty.this.mBubbleEventListener,
							DataModelChangedEvent.class);
			} else {
				throw new RuntimeException("WFT ??! This should never happen.");
			}
		}
	};

	@Override
	public <K extends DataModelChangedEvent> void notifyListeners(K event)
			throws MethodParameterIsNullException {
		if (event == null) {
			throw new MethodParameterIsNullException();
		}
		if (ChannelMediaMapEvent.class.isAssignableFrom(event.getClass())) {
			mChannelMediaMapEventNotifier.notifyListeners(event);
		}
		super.notifyListeners(event);
	}

	@Override
	public <K extends DataModelChangedEvent> void registerListener(
			IEventListener<K> listener, Class<K> klass)
			throws MethodParameterIsNullException {
		if (klass == null || listener == null) {
			throw new MethodParameterIsNullException();
		}
		if (ChannelMediaMapEvent.class.isAssignableFrom(klass)) {
			mChannelMediaMapEventNotifier.registerListener(listener, klass);
		} else {
			super.registerListener(listener, klass);
		}
	}

	@Override
	public <K extends DataModelChangedEvent> void unregisterListener(
			IEventListener<K> listener, Class<K> klass)
			throws MethodParameterIsNullException {
		if (klass == null || listener == null) {
			throw new MethodParameterIsNullException();
		}
		if (ChannelMediaMapEvent.class.isAssignableFrom(klass)) {
			mChannelMediaMapEventNotifier.unregisterListener(listener, klass);
		} else {
			super.unregisterListener(listener, klass);
		}
	}

	/**
	 * 
	 */
	public ChannelsProperty() {
		try {
			registerListener(mChannelMediaMapEventListener,
					ChannelMediaMapEvent.class);
		} catch (MethodParameterIsNullException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
	}

	@Override
	public boolean canBeAddedTo(ITreeNode potentialOwner)
			throws MethodParameterIsNullException {
		if (potentialOwner == null) {
			throw new MethodParameterIsNullException();
		}
		if (!super.canBeAddedTo(potentialOwner))
			return false;
		if (potentialOwner.hasProperties(this.getClass()))
			return false;
		return true;
	}

	public IMedia getMedia(IChannel iChannel)
			throws MethodParameterIsNullException, ChannelDoesNotExistException {
		if (iChannel == null) {
			throw new MethodParameterIsNullException();
		}
		try {
			if (!getPresentation().getChannelsManager().getListOfChannels()
					.contains(iChannel)) {
				throw new ChannelDoesNotExistException();
			}
		} catch (IsNotInitializedException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
		if (!mMapChannelToMediaObject.containsKey(iChannel))
			return null;
		return mMapChannelToMediaObject.get(iChannel);
	}

	public void setMedia(IChannel iChannel, IMedia media)
			throws MethodParameterIsNullException,
			ChannelDoesNotExistException, DoesNotAcceptMediaException {
		if (iChannel == null) {
			throw new MethodParameterIsNullException();
		}
		try {
			if (!getPresentation().getChannelsManager().getListOfChannels()
					.contains(iChannel)) {
				throw new ChannelDoesNotExistException();
			}
		} catch (IsNotInitializedException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
		if (media != null) {
			if (!iChannel.canAccept(media)) {
				throw new DoesNotAcceptMediaException();
			}
		}
		IMedia prevMedia = null;
		if (mMapChannelToMediaObject.containsKey(iChannel))
			prevMedia = mMapChannelToMediaObject.get(iChannel);
		mMapChannelToMediaObject.put(iChannel, media);
		notifyListeners(new ChannelMediaMapEvent(this, iChannel, media,
				prevMedia));
	}

	public List<IChannel> getListOfUsedChannels() {
		List<IChannel> res = new LinkedList<IChannel>();
		try {
			List<IChannel> list = getPresentation().getChannelsManager()
					.getListOfChannels();
			for (IChannel ch : list) {
				try {
					if (getMedia(ch) != null) {
						res.add(ch);
					}
				} catch (MethodParameterIsNullException e) {
					// Should never happen
					throw new RuntimeException("WTF ??!", e);
				} catch (ChannelDoesNotExistException e) {
					// Should never happen
					throw new RuntimeException("WTF ??!", e);
				}
			}
		} catch (IsNotInitializedException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
		return res;
	}

	@Override
	public IChannelsProperty copy() throws FactoryCannotCreateTypeException,
			IsNotInitializedException {
		return (IChannelsProperty) copyProtected();
	}

	@Override
	protected IProperty copyProtected()
			throws FactoryCannotCreateTypeException, IsNotInitializedException {
		IChannelsProperty theCopy = (IChannelsProperty) super.copyProtected();
		if (theCopy == null) {
			throw new FactoryCannotCreateTypeException();
		}
		for (IChannel ch : getListOfUsedChannels()) {
			try {
				theCopy.setMedia(ch, getMedia(ch).copy());
			} catch (MethodParameterIsNullException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!", e);
			} catch (ChannelDoesNotExistException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!", e);
			} catch (DoesNotAcceptMediaException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!", e);
			}
		}
		return theCopy;
	}

	@Override
	public IChannelsProperty export(IPresentation destPres)
			throws FactoryCannotCreateTypeException, IsNotInitializedException,
			MethodParameterIsNullException {
		return (IChannelsProperty) exportProtected(destPres);
	}

	@Override
	protected IProperty exportProtected(IPresentation destPres)
			throws FactoryCannotCreateTypeException, IsNotInitializedException,
			MethodParameterIsNullException {
		IChannelsProperty chExport = (IChannelsProperty) super
				.exportProtected(destPres);
		if (chExport == null) {
			throw new FactoryCannotCreateTypeException();
		}
		for (IChannel ch : getListOfUsedChannels()) {
			IChannel exportDestCh = null;
			List<IChannel> list = destPres.getChannelsManager()
					.getListOfChannels();
			for (IChannel dCh : list) {
				if (ch.isEquivalentTo(dCh)) {
					exportDestCh = dCh;
					break;
				}
			}
			if (exportDestCh == null) {
				exportDestCh = ch.export(destPres);
				try {
					destPres.getChannelsManager().addChannel(exportDestCh);
				} catch (ChannelAlreadyExistsException e) {
					// Should never happen
					throw new RuntimeException("WTF ??!", e);
				}
			}
			try {
				chExport.setMedia(exportDestCh, getMedia(ch).export(destPres));
			} catch (ChannelDoesNotExistException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!", e);
			} catch (DoesNotAcceptMediaException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!", e);
			}
		}
		return chExport;
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
		if (source.getNamespaceURI() == IXukAble.XUK_NS) {
			readItem = true;
			if (source.getLocalName() == "mChannelMappings") {
				xukInChannelMappings(source, ph);
			} else {
				readItem = false;
			}
		}
		if (!readItem) {
			super.xukInChild(source, ph);
		}
	}

	private void xukInChannelMappings(IXmlDataReader source, IProgressHandler ph)
			throws MethodParameterIsNullException,
			XukDeserializationFailedException, ProgressCancelledException {
		if (source == null) {
			throw new MethodParameterIsNullException();
		}
		if (ph != null && ph.notifyProgress()) {
			throw new ProgressCancelledException();
		}
		if (!source.isEmptyElement()) {
			while (source.read()) {
				if (source.getNodeType() == IXmlDataReader.ELEMENT) {
					if (source.getLocalName() == "mChannelMapping"
							&& source.getNamespaceURI() == IXukAble.XUK_NS) {
						XUKInChannelMapping(source, ph);
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

	private void XUKInChannelMapping(IXmlDataReader source, IProgressHandler ph)
			throws MethodParameterIsNullException,
			XukDeserializationFailedException, ProgressCancelledException {
		if (source == null) {
			throw new MethodParameterIsNullException();
		}
		if (ph != null && ph.notifyProgress()) {
			throw new ProgressCancelledException();
		}
		String channelRef = source.getAttribute("channel");
		while (source.read()) {
			if (source.getNodeType() == IXmlDataReader.ELEMENT) {
				IMedia newMedia;
				try {
					newMedia = getPresentation().getMediaFactory().createMedia(
							source.getLocalName(), source.getNamespaceURI());
				} catch (MethodParameterIsEmptyStringException e) {
					// Should never happen
					throw new RuntimeException("WTF ??!", e);
				} catch (IsNotInitializedException e) {
					// Should never happen
					throw new RuntimeException("WTF ??!", e);
				}
				if (newMedia != null) {
					IChannel iChannel;
					try {
						iChannel = getPresentation().getChannelsManager()
								.getChannel(channelRef);
					} catch (MethodParameterIsEmptyStringException e) {
						// Should never happen
						throw new RuntimeException("WTF ??!", e);
					} catch (IsNotInitializedException e) {
						// Should never happen
						throw new RuntimeException("WTF ??!", e);
					} catch (ChannelDoesNotExistException e) {
						// Should never happen
						throw new RuntimeException("WTF ??!", e);
					}
					if (iChannel == null) {
						throw new XukDeserializationFailedException();
					}
					try {
						setMedia(iChannel, newMedia);
					} catch (ChannelDoesNotExistException e) {
						// Should never happen
						throw new RuntimeException("WTF ??!", e);
					} catch (DoesNotAcceptMediaException e) {
						// Should never happen
						throw new RuntimeException("WTF ??!", e);
					}
					newMedia.xukIn(source, ph);
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

	@Override
	protected void xukOutChildren(IXmlDataWriter destination, URI baseUri,
			IProgressHandler ph) throws XukSerializationFailedException,
			MethodParameterIsNullException, ProgressCancelledException {
		if (ph != null && ph.notifyProgress()) {
			throw new ProgressCancelledException();
		}
		destination.writeStartElement("mChannelMappings", IXukAble.XUK_NS);
		List<IChannel> channelsList = getListOfUsedChannels();
		for (IChannel iChannel : channelsList) {
			destination.writeStartElement("mChannelMapping", IXukAble.XUK_NS);
			destination.writeAttributeString("channel", iChannel.getUid());
			IMedia iMedia;
			try {
				iMedia = getMedia(iChannel);
			} catch (ChannelDoesNotExistException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!", e);
			}
			if (iMedia == null) {
				throw new XukSerializationFailedException();
			}
			iMedia.xukOut(destination, baseUri, ph);
			destination.writeEndElement();
		}
		destination.writeEndElement();
		super.xukOutChildren(destination, baseUri, ph);
	}

	@Override
	public boolean ValueEquals(IProperty other)
			throws MethodParameterIsNullException {
		if (other == null) {
			throw new MethodParameterIsNullException();
		}
		if (!super.ValueEquals(other))
			return false;
		IChannelsProperty otherChProp = (IChannelsProperty) other;
		List<IChannel> chs = getListOfUsedChannels();
		List<IChannel> otherChs = otherChProp.getListOfUsedChannels();
		if (chs.size() != otherChs.size())
			return false;
		for (IChannel ch : chs) {
			IChannel otherCh = null;
			for (IChannel ch2 : otherChs) {
				if (ch.getUid() == ch2.getUid()) {
					otherCh = ch2;
					break;
				}
			}
			if (otherCh == null)
				return false;
			try {
				if (!getMedia(ch).ValueEquals(otherChProp.getMedia(otherCh)))
					return false;
			} catch (ChannelDoesNotExistException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!", e);
			}
		}
		return true;
	}
}
