package org.daisy.urakawa.property.channel;

import org.daisy.urakawa.core.ITreeNode;
import org.daisy.urakawa.core.visitor.ITreeNodeVisitor;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.media.DoesNotAcceptMediaException;
import org.daisy.urakawa.media.IMedia;

/**
 * ITreeNodeVisitor that clears all media content for a IChannel
 */
public class ClearChannelTreeNodeVisitor implements ITreeNodeVisitor {
	private IChannel mChannelToClear;

	/**
	 * @param chToClear
	 */
	public ClearChannelTreeNodeVisitor(IChannel chToClear) {
		mChannelToClear = chToClear;
	}

	/**
	 * @return IChannel
	 */
	public IChannel getChannelToClear() {
		return mChannelToClear;
	}

	public void postVisit(ITreeNode node) throws MethodParameterIsNullException {
		if (node == null) {
			throw new MethodParameterIsNullException();
		}
		IChannelsProperty chProp = node
				.<IChannelsProperty> getProperty(IChannelsProperty.class);
		if (chProp != null) {
			IMedia m;
			try {
				m = chProp.getMedia(mChannelToClear);
			} catch (ChannelDoesNotExistException e) {
				// Ignore this.
				e.printStackTrace();
				m = null;
			}
			if (m != null) {
				try {
					chProp.setMedia(mChannelToClear, null);
				} catch (ChannelDoesNotExistException e) {
					// Should never happen
					throw new RuntimeException("WTF ??!", e);
				} catch (DoesNotAcceptMediaException e) {
					// Should never happen
					throw new RuntimeException("WTF ??!", e);
				}
			}
		}
	}

	@SuppressWarnings("unused")
	public boolean preVisit(ITreeNode node)
			throws MethodParameterIsNullException {
		return true;
	}
}
