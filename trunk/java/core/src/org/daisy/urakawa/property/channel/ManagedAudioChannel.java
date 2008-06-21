package org.daisy.urakawa.property.channel;

import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.media.IMedia;
import org.daisy.urakawa.media.data.audio.IManagedAudioMedia;

/**
 *
 */
public class ManagedAudioChannel extends AudioChannel {
	/**
	 * @param chMgr
	 * @throws MethodParameterIsNullException
	 */
	public ManagedAudioChannel(IChannelsManager chMgr)
			throws MethodParameterIsNullException {
		super(chMgr);
	}

	@Override
	public boolean canAccept(IMedia m) throws MethodParameterIsNullException {
		if (!super.canAccept(m))
			return false;
		if (m instanceof IManagedAudioMedia)
			return true;
		return false;
	}
}
