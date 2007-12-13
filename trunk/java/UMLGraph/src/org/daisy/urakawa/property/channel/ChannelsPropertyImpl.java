package org.daisy.urakawa.property.channel;

import java.net.URI;
import java.util.HashMap;
import java.util.LinkedList;
import java.util.List;
import java.util.Map;

import org.daisy.urakawa.FactoryCannotCreateTypeException;
import org.daisy.urakawa.Presentation;
import org.daisy.urakawa.core.TreeNode;
import org.daisy.urakawa.exception.IsNotInitializedException;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.media.DoesNotAcceptMediaException;
import org.daisy.urakawa.media.Media;
import org.daisy.urakawa.property.Property;
import org.daisy.urakawa.property.PropertyImpl;
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
public class ChannelsPropertyImpl extends PropertyImpl implements
		ChannelsProperty {
	private Map<Channel, Media> mMapChannelToMediaObject;

	/**
	 * @param chToMediaMapper
	 */
	public ChannelsPropertyImpl(Map<Channel, Media> chToMediaMapper) {
		if (chToMediaMapper == null) {
			throw new RuntimeException(new MethodParameterIsNullException());
		}
		mMapChannelToMediaObject = chToMediaMapper;
		mMapChannelToMediaObject.clear();
	}

	/**
	 * 
	 */
	public ChannelsPropertyImpl() {
		this(new HashMap<Channel, Media>());
	}

	@Override
	public boolean canBeAddedTo(TreeNode potentialOwner)
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

	public Media getMedia(Channel channel)
			throws MethodParameterIsNullException, ChannelDoesNotExistException {
		if (channel == null) {
			throw new MethodParameterIsNullException();
		}
		try {
			if (!getPresentation().getChannelsManager().getListOfChannels()
					.contains(channel)) {
				throw new ChannelDoesNotExistException();
			}
		} catch (IsNotInitializedException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
		if (!mMapChannelToMediaObject.containsKey(channel))
			return null;
		return (Media) mMapChannelToMediaObject.get(channel);
	}

	public void setMedia(Channel channel, Media media)
			throws MethodParameterIsNullException,
			ChannelDoesNotExistException, DoesNotAcceptMediaException {
		if (channel == null) {
			throw new MethodParameterIsNullException();
		}
		try {
			if (!getPresentation().getChannelsManager().getListOfChannels()
					.contains(channel)) {
				throw new ChannelDoesNotExistException();
			}
		} catch (IsNotInitializedException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
		if (media != null) {
			if (!channel.canAccept(media)) {
				throw new DoesNotAcceptMediaException();
			}
		}
		@SuppressWarnings("unused")
		Media prevMedia = null;
		if (mMapChannelToMediaObject.containsKey(channel))
			prevMedia = mMapChannelToMediaObject.get(channel);
		mMapChannelToMediaObject.put(channel, media);
	}

	@SuppressWarnings("unchecked")
	public List<Channel> getListOfUsedChannels() {
		List<Channel> res = new LinkedList<Channel>();
		try {
			List<Channel> list = getPresentation().getChannelsManager()
					.getListOfChannels();
			for (Channel ch : list) {
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
	public ChannelsProperty copy() throws FactoryCannotCreateTypeException,
			IsNotInitializedException {
		return (ChannelsProperty) copyProtected();
	}

	@Override
	protected Property copyProtected() throws FactoryCannotCreateTypeException,
			IsNotInitializedException {
		ChannelsProperty theCopy = (ChannelsProperty) super.copyProtected();
		if (theCopy == null) {
			throw new FactoryCannotCreateTypeException();
		}
		for (Channel ch : getListOfUsedChannels()) {
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
	public ChannelsProperty export(Presentation destPres)
			throws FactoryCannotCreateTypeException, IsNotInitializedException,
			MethodParameterIsNullException {
		return (ChannelsProperty) exportProtected(destPres);
	}

	@Override
	@SuppressWarnings("unchecked")
	protected Property exportProtected(Presentation destPres)
			throws FactoryCannotCreateTypeException, IsNotInitializedException,
			MethodParameterIsNullException {
		ChannelsProperty chExport = (ChannelsProperty) super
				.exportProtected(destPres);
		if (chExport == null) {
			throw new FactoryCannotCreateTypeException();
		}
		for (Channel ch : getListOfUsedChannels()) {
			Channel exportDestCh = null;
			List<Channel> list = destPres.getChannelsManager()
					.getListOfChannels();
			for (Channel dCh : list) {
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
	protected void xukInChild(XmlDataReader source)
			throws MethodParameterIsNullException,
			XukDeserializationFailedException {
		if (source == null) {
			throw new MethodParameterIsNullException();
		}
		boolean readItem = false;
		if (source.getNamespaceURI() == XukAbleImpl.XUK_NS) {
			readItem = true;
			if (source.getLocalName() == "mChannelMappings") {
				xukInChannelMappings(source);
			} else {
				readItem = false;
			}
		}
		if (!(readItem || source.isEmptyElement())) {
			source.readSubtree().close();
		}
	}

	private void xukInChannelMappings(XmlDataReader source)
			throws MethodParameterIsNullException,
			XukDeserializationFailedException {
		if (source == null) {
			throw new MethodParameterIsNullException();
		}
		if (!source.isEmptyElement()) {
			while (source.read()) {
				if (source.getNodeType() == XmlDataReader.ELEMENT) {
					if (source.getLocalName() == "mChannelMapping"
							&& source.getNamespaceURI() == XukAbleImpl.XUK_NS) {
						XUKInChannelMapping(source);
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

	private void XUKInChannelMapping(XmlDataReader source)
			throws MethodParameterIsNullException,
			XukDeserializationFailedException {
		if (source == null) {
			throw new MethodParameterIsNullException();
		}
		String channelRef = source.getAttribute("channel");
		while (source.read()) {
			if (source.getNodeType() == XmlDataReader.ELEMENT) {
				Media newMedia;
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
					Channel channel;
					try {
						channel = getPresentation().getChannelsManager()
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
					if (channel == null) {
						throw new XukDeserializationFailedException();
					}
					try {
						setMedia(channel, newMedia);
					} catch (ChannelDoesNotExistException e) {
						// Should never happen
						throw new RuntimeException("WTF ??!", e);
					} catch (DoesNotAcceptMediaException e) {
						// Should never happen
						throw new RuntimeException("WTF ??!", e);
					}
					newMedia.xukIn(source);
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

	@Override
	protected void xukOutChildren(XmlDataWriter destination, URI baseUri)
			throws XukSerializationFailedException,
			MethodParameterIsNullException {
		destination.writeStartElement("mChannelMappings", XukAbleImpl.XUK_NS);
		List<Channel> channelsList = getListOfUsedChannels();
		for (Channel channel : channelsList) {
			destination
					.writeStartElement("mChannelMapping", XukAbleImpl.XUK_NS);
			destination.writeAttributeString("channel", channel.getUid());
			Media media;
			try {
				media = getMedia(channel);
			} catch (ChannelDoesNotExistException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!", e);
			}
			if (media == null) {
				throw new XukSerializationFailedException();
			}
			media.xukOut(destination, baseUri);
			destination.writeEndElement();
		}
		destination.writeEndElement();
		super.xukOutChildren(destination, baseUri);
	}

	@Override
	public boolean ValueEquals(Property other)
			throws MethodParameterIsNullException {
		if (other == null) {
			throw new MethodParameterIsNullException();
		}
		if (!super.ValueEquals(other))
			return false;
		ChannelsProperty otherChProp = (ChannelsProperty) other;
		List<Channel> chs = getListOfUsedChannels();
		List<Channel> otherChs = otherChProp.getListOfUsedChannels();
		if (chs.size() != otherChs.size())
			return false;
		for (Channel ch : chs) {
			Channel otherCh = null;
			for (Channel ch2 : otherChs) {
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
