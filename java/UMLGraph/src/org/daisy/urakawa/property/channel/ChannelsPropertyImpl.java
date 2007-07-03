package org.daisy.urakawa.property.channel;

import java.util.List;

import org.daisy.urakawa.FactoryCannotCreateTypeException;
import org.daisy.urakawa.Presentation;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.media.Media;
import org.daisy.urakawa.media.MediaTypeIsIllegalException;
import org.daisy.urakawa.property.Property;
import org.daisy.urakawa.property.PropertyImpl;

/**
 * Reference implementation of the interface.
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 */
public class ChannelsPropertyImpl extends PropertyImpl implements
		ChannelsProperty {
	public Property exportProperty(Presentation destPres)
			throws FactoryCannotCreateTypeException {
		ChannelsProperty destProp = (ChannelsProperty) super
				.exportProperty(destPres);
		ChannelsManager destManager = destPres.getChannelsManager();
		List<Channel> channels = getListOfUsedChannels();
		for (Channel channel : channels) {
			Channel destChannel = destManager.getEquivalentChannel(channel);
			if (destChannel == null) {
				destChannel = channel.exportChannel(destPres);
				// destManager.add(destChannel); // NO NEED TO DO THIS: because
				// the above export() uses the factory create method, and
				// therefore handles the association of the channel with its
				// manager.
			}
			Media media;
			try {
				media = getMedia(channel);
			} catch (MethodParameterIsNullException e) {
				e.printStackTrace();
				return null;
			} catch (ChannelDoesNotExistException e) {
				e.printStackTrace();
				return null;
			}
			Media destMedia = media.exportMedia(destPres);
			try {
				destProp.setMedia(destChannel, destMedia);
			} catch (MethodParameterIsNullException e) {
				e.printStackTrace();
				return null;
			} catch (ChannelDoesNotExistException e) {
				e.printStackTrace();
				return null;
			} catch (MediaTypeIsIllegalException e) {
				e.printStackTrace();
				return null;
			}
		}
		return destProp;
	}

	public List<Channel> getListOfUsedChannels() {
		return null;
	}

	public Media getMedia(Channel channel)
			throws MethodParameterIsNullException, ChannelDoesNotExistException {
		return null;
	}

	public void setMedia(Channel channel, Media media)
			throws MethodParameterIsNullException,
			ChannelDoesNotExistException, MediaTypeIsIllegalException {
	}

	public ChannelsProperty copyChannelsProperty() {
		return null;
	}
}
