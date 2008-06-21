package org.daisy.urakawa.property.channel;

import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.media.IMedia;
import org.daisy.urakawa.media.ISequenceMedia;
import org.daisy.urakawa.media.ITextMedia;

/**
 *
 */
public class TextChannel extends ChannelImpl {
	/**
	 * @param chMgr
	 * @throws MethodParameterIsNullException
	 */
	public TextChannel(IChannelsManager chMgr)
			throws MethodParameterIsNullException {
		super(chMgr);
	}

	@Override
	public boolean canAccept(IMedia m) throws MethodParameterIsNullException {
		if (!super.canAccept(m))
			return false;
		if (m instanceof ITextMedia)
			return true;
		if (m instanceof ISequenceMedia) {
			for (IMedia sm : ((ISequenceMedia) m).getListOfItems()) {
				if (!(sm instanceof ITextMedia))
					return false;
			}
			return true;
		}
		return false;
	}
}
