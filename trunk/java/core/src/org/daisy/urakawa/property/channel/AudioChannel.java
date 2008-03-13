package org.daisy.urakawa.property.channel;

import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.media.AudioMedia;
import org.daisy.urakawa.media.Media;

/**
 *
 */
public class AudioChannel extends ChannelImpl {
	/**
	 * @param chMgr
	 * @throws MethodParameterIsNullException
	 */
	public AudioChannel(ChannelsManager chMgr)
			throws MethodParameterIsNullException {
		super(chMgr);
	}

	@Override
	public boolean canAccept(Media m) throws MethodParameterIsNullException {
		if (!super.canAccept(m))
			return false;
		if (m instanceof AudioMedia)
			return true;
		return false;
	}
}
