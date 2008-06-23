package org.daisy.urakawa.property.channel;

import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.media.Media;
import org.daisy.urakawa.media.SequenceMedia;
import org.daisy.urakawa.media.TextMedia;

/**
 *
 */
public class TextChannel extends ChannelImpl {
	/**
	 * @param chMgr
	 * @throws MethodParameterIsNullException
	 */
	public TextChannel(ChannelsManager chMgr)
			throws MethodParameterIsNullException {
		super(chMgr);
	}

	@Override
	public boolean canAccept(Media m) throws MethodParameterIsNullException {
		if (!super.canAccept(m))
			return false;
		if (m instanceof TextMedia)
			return true;
		if (m instanceof SequenceMedia) {
			for (Media sm : ((SequenceMedia) m).getListOfItems()) {
				if (!(sm instanceof TextMedia))
					return false;
			}
			return true;
		}
		return false;
	}
}
