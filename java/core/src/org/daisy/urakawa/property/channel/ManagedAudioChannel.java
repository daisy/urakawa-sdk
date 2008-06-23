package org.daisy.urakawa.property.channel;

import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.media.Media;
import org.daisy.urakawa.media.data.audio.ManagedAudioMedia;

/**
 *
 */
public class ManagedAudioChannel extends AudioChannel {
	/**
	 * @param chMgr
	 * @throws MethodParameterIsNullException
	 */
	public ManagedAudioChannel(ChannelsManager chMgr)
			throws MethodParameterIsNullException {
		super(chMgr);
	}

	@Override
	public boolean canAccept(Media m) throws MethodParameterIsNullException {
		if (!super.canAccept(m))
			return false;
		if (m instanceof ManagedAudioMedia)
			return true;
		return false;
	}
}
